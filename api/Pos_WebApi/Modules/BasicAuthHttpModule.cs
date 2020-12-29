namespace Pos_WebApi.Modules
{
    using Autofac;
    using log4net;
    using Pos_WebApi.Helpers;
    using Symposium.Helpers;
    using Symposium.Helpers.Classes;
    using Symposium.Helpers.Interfaces;
    using Symposium.Models.Models;
    using Symposium.Models.Models.Delivery;
    using Symposium.Models.Models.DeliveryAgent;
    using Symposium.WebApi.MainLogic.Interfaces.Flows.Delivery;
    using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
    using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Http.Dependencies;

    /// <summary>
    /// Defines the <see cref="BasicAuthHttpModule" />
    /// </summary>
    public class BasicAuthHttpModule : IHttpModule
    {
        /// <summary>
        /// Defines the Realm
        /// </summary>
        private const string Realm = "My Realm";

        /// <summary>
        /// Defines the logger
        /// </summary>
        public ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Defines the found
        /// </summary>
        internal bool found;

        // DA_CustomerTasks cust;
        // IDA_StaffTasks staff;

        //the list of store objects from 'UsersToDatabases.xml'
        /// <summary>
        /// Defines the stores
        /// </summary>
        internal IStoreIdsPropertiesHelper stores;

        /// <summary>
        /// Gets or sets the autofac
        /// </summary>
        internal IDependencyResolver autofac { get; set; }

        internal bool isDeliveryAgent;

        internal Guid DA_StoreId;

        /// <summary>
        /// The Init
        /// </summary>
        /// <param name="context">The context<see cref="HttpApplication"/></param>
        public void Init(HttpApplication context)
        {

            // Register event handlers 
            context.AuthenticateRequest += OnApplicationAuthenticateRequest;
            context.EndRequest += OnApplicationEndRequest;

            //get autofac container
            var config = System.Web.Http.GlobalConfiguration.Configuration;
            autofac = config.DependencyResolver;

            //get the list of store objects from 'UsersToDatabases.xml' 
            stores = (IStoreIdsPropertiesHelper)autofac.GetService(typeof(IStoreIdsPropertiesHelper));

            isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");

            string storeIDRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_StoreId");
            DA_StoreId = new Guid(storeIDRaw);

        }

        /// <summary>
        /// The SetPrincipal
        /// </summary>
        /// <param name="principal">The principal<see cref="IPrincipal"/></param>
        private void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        /// <summary>
        /// Authenticate customer based on AuthToken
        /// </summary>
        /// <param name="authToken">AuthToken (see table DA_Customers)</param>
        /// <param name="role">The role<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool CheckAuthToken(string authToken,  out string role)
        {
            role = "";
            Symposium.Models.Models.DBInfoModel dbInfo=null;
            if (isDeliveryAgent)
            {
                dbInfo = stores.GetStoreById(DA_StoreId);

            }
            else
            {
                logger.Error("Call is not for Delivery Agent. Authentication by token is not supported.");
                return false;
            }
            if (dbInfo == null)
            {
                logger.Error("DA_StoreId not found in UsersToDB file.");
                return false;
            }
            role = dbInfo.Role;
            AddStoreHeader(dbInfo);

            IDA_CustomerFlows custTasks = (IDA_CustomerFlows)autofac.GetService(typeof(IDA_CustomerFlows));

            try
            {
              long customerId =  custTasks.LoginUser(dbInfo, authToken);
                if (customerId > 0)
                {
                    AddCustIdToHeader(customerId);
                    return true;
                }
            }
            catch(Exception)
            {
              // logger has already written down the reason of failure  
            }
            return false;
        }

        /// <summary>
        /// Authenticate customer/staff/userstoDBs based on 'username:password'
        /// </summary>
        /// <param name="username">The username<see cref="string"/></param>
        /// <param name="password">The password<see cref="string"/></param>
        /// <param name="role">The role<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool CheckPassword(string username, string password, out string role)
        {
            try
            {
                string sId = "";

                Symposium.Models.Models.DBInfoModel store;
                //Symposium.Models.Models.DBInfoModel user;
                var request = HttpContext.Current.Request;

                if (string.IsNullOrEmpty(stores.BaseUrl))
                {
                    stores.BaseUrl = request.Url.Scheme + "://" + request.Url.Authority + "/";
                    if (stores.BaseUrl.Substring(stores.BaseUrl.Length - 1, 1) != "/")
                        stores.BaseUrl += "/";
                }

                bool useDeliveryRouting = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drUseRouting");
                string deliveryRoutingDefaultGuid = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drDefaultGuid");


                //if api is 'Delivery Agent' then get storeId from webconfig
                if (isDeliveryAgent)
                {
                    store = stores.GetStoreById(DA_StoreId);
                }
                // Handle delivery routing ------------------------
                else if (useDeliveryRouting && !string.IsNullOrEmpty(deliveryRoutingDefaultGuid))
                {
                    store = stores.GetStoreById(new Guid(deliveryRoutingDefaultGuid));
                }
                // End Handle delivery routing ------------------------
                else
                {
                    store = stores.GetStores().Where(w => w.Username == username && w.Password == password).FirstOrDefault();
                }


                if (store == null)
                {
                    logger.Error("User not found: <null>");
                    role = "";
                    return false;
                }

                role = store.Role; // role = user.Role;
                AddStoreHeader(store);

                if (request.FilePath.Contains("/da/")) // url contains '/da/' --> validate user against tables Staff or DA_Customers
                {
                    if (username.EndsWith("@"))
                    {
                        //Check Staff Table For Staff UserName and Password and return StaffId
                        DALoginStaffModel staffLoginModel = new DALoginStaffModel();
                        staffLoginModel.Username = username.TrimEnd('@');
                        staffLoginModel.Password = password;
                        IDA_StaffFlows stafFlows = (IDA_StaffFlows)autofac.GetService(typeof(IDA_StaffFlows));
                        long staffId = stafFlows.LoginStaff(store, staffLoginModel);
                        AddStaffIdToHeader(staffId);
                        return true;
                    }
                    else
                    {
                        //Check DA_Customers for customers email and password and return CustomerId
                        DALoginModel custLoginModel = new DALoginModel();
                        custLoginModel.Email = username;
                        custLoginModel.Password = password;// Symposium.Helpers.MD5Helper.SHA1(password);
                        IDA_CustomerFlows custFlows = (IDA_CustomerFlows)autofac.GetService(typeof(IDA_CustomerFlows));
                        long customerId = custFlows.LoginUser(store, custLoginModel);
                        AddCustIdToHeader(customerId);
                        return true;
                    }
                }
                else if (request.FilePath.Contains("/dr/"))
                {
                    DeliveryRoutingStaffCredentialsModel staffCredentialsModel = new DeliveryRoutingStaffCredentialsModel();
                    staffCredentialsModel.username = username;
                    staffCredentialsModel.password = password;
                    IDeliveryRoutingFlows deliveryRoutingFlows = (IDeliveryRoutingFlows)autofac.GetService(typeof(IDeliveryRoutingFlows));
                    long staffId = deliveryRoutingFlows.staffLogin(store, staffCredentialsModel) ?? -1;
                    if (staffId != -1)
                    { 
                        AddCustIdToHeader(staffId);
                        return true;
                    }
                    return false;
                }

                return true;
            }
            catch (BusinessException bex)
            {
                logger.Warn(bex.Message);
                // throw;
                role = "";
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                // throw;
                role = "";
                return false;
            }
        }

        /// <summary>
        /// Add toy the Request the header STORE holding the whole Store.
        /// </summary>
        /// <param name="store">the store</param>
        private void AddStoreHeader(Symposium.Models.Models.DBInfoModel store)
        {
            CustomJsonSerializers jser = new CustomJsonSerializers();
            string storeJson = jser.StoreToJson(store);
            System.Web.HttpContext.Current.Request.Headers.Add("STORE", storeJson);
        }

        /// <summary>
        /// Add CustomerId to Header.
        /// </summary>
        /// <param name="CustomerId"></param>
        private void AddCustIdToHeader(long CustomerId)
        {
            System.Web.HttpContext.Current.Request.Headers.Add("CUSTOMERID", CustomerId.ToString());
        }

        /// <summary>
        /// Add StaffId to Header.
        /// </summary>
        /// <param name="StaffId"></param>
        private void AddStaffIdToHeader(long StaffId)
        {
            System.Web.HttpContext.Current.Request.Headers.Add("STAFFID", StaffId.ToString());
        }

        /// <summary>
        /// Add toy the Request the header STORE holding the whole Store.
        /// </summary>
        /// <param name="store">the store</param>
        private void AddStoreIdToHeader(Symposium.Models.Models.DBInfoModel store)
        {
            CustomJsonSerializers jser = new CustomJsonSerializers();
            string storeJson = jser.StoreToJson(store);
            System.Web.HttpContext.Current.Request.Headers.Add("STORE", storeJson);
        }

        /// <summary>
        /// The AuthenticateUser
        /// </summary>
        /// <param name="credentials">The credentials<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool AuthenticateUser(string credentials)
        {
            bool validated = false;
            string name = null;
            string password = "";
            string role = "";
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));
                if (credentials.EndsWith(":")) credentials = credentials.Replace(":", "");
                int separator = credentials.IndexOf(':');
                
                if (separator >= 0)
                {
                    name = credentials.Substring(0, separator);
                    password = credentials.Substring(separator + 1);
                    validated = CheckPassword(name, password, out role);
                }
              else
                {
                    name = credentials;
                    validated = CheckAuthToken(credentials, out role);
                }
                

                if (validated)
                {
                    var identity = new GenericIdentity(name);
                    SetPrincipal(new GenericPrincipal(identity, role != null ? role.Split(',') : null));
                }
            }
            catch (FormatException fex)
            {
                logger.Error("Credentials were not formatted correctly: " + fex.ToString());
                validated = false;

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                validated = false;

            }
            return validated;
        }

        /// <summary>
        /// Authentication Entry Point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["Authorization"];
            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    AuthenticateUser(authHeaderVal.Parameter);
                }
            }
            else
            {
                if (isDeliveryAgent)
                {
                    var u = stores.GetStoreById(DA_StoreId); 
                  
                    AddStoreHeader(u);
                }    
            }
        }

        // If the request was unauthorized, add the WWW-Authenticate header 
        // to the response.
        /// <summary>
        /// The OnApplicationEndRequest
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void OnApplicationEndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)//Access Denied - Authentication failed
            {
                //set WWW-Authenticate response header
                try
                {
                    response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", Realm));
                }
                catch (Exception ex) { }
               
            }
        }

        /// <summary>
        /// The Dispose
        /// </summary>
        public void Dispose()
        {
        }
    }
}
