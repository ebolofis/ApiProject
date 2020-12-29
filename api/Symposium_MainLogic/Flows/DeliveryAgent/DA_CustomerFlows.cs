using Symposium.Models.Enums;
using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Helpers.Interfaces;
using Microsoft.AspNet.SignalR;
using log4net;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Configuration;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_CustomerFlows : IDA_CustomerFlows
    {
        IDA_CustomerTasks custTasks;
        IDA_AddressesTasks addressTask;
        IConfigurationTasks configurationTasks;
        IStoreIdsPropertiesHelper storesHelper;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ICashedLoginsHelper cashedLoginsHelper;

        public DA_CustomerFlows(IDA_CustomerTasks _custTasks, IDA_AddressesTasks addressTask, IConfigurationTasks _configurationTasks, IStoreIdsPropertiesHelper storesHelper, ICashedLoginsHelper cashedLoginsHelper)
        {
            this.custTasks = _custTasks;
            this.addressTask = addressTask;
            this.configurationTasks = _configurationTasks;
            this.storesHelper = storesHelper;
            this.cashedLoginsHelper = cashedLoginsHelper;
        }

        /// <summary>
        /// Authenticate User 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>CustomerId</returns>
        public long LoginUser(DBInfoModel dbInfo, DALoginModel loginModel)
        {
            return custTasks.LoginUser(dbInfo, loginModel);
        }

        /// <summary>
        /// Authenticate User based on authtoken. On authentication failure throw exception.
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="authtoken"></param>
        /// <returns>CustomerId</returns>
        public long LoginUser(DBInfoModel dbInfo, string authtoken)
        {
            return custTasks.LoginUser(dbInfo, authtoken);
        }

        /// <summary>
        /// Search Customers 
        /// type:
        /// 0: search by firstname or lastname
        /// 1: search by Address and AddressNo 
        /// 2: ΑΦΜ
        /// 3: Phone1 ή Phone2 ή Mobile
        /// search: Λεκτικό αναζήτησης
        /// </summary>
        /// <param name="type"></param>
        /// <param name="search"></param>
        /// <returns>List of customers + addresses </returns>
        public List<DASearchCustomerModel> SearchCustomers(DBInfoModel dbInfo, DA_CustomerSearchTypeEnum type, string search)
        {
            return custTasks.SearchCustomers(dbInfo, type, search);
        }

        /// <summary>
        /// return true if mobile exists into an active customer (isDeleted=0)
        /// </summary>
        /// <param name="mobile">mobile</param>
        /// <returns></returns>
        public bool mobileExists(DBInfoModel dbInfo, string mobile)
        {
            return custTasks.mobileExists(dbInfo, mobile);
        }

        /// <summary>
        /// return true if email exists into an active customer (isDeleted=0)
        /// </summary>
        /// <param name="email">email</param>
        /// <returns></returns>
        public bool emailExists(DBInfoModel dbInfo, string email)
        {
            return custTasks.emailExists(dbInfo, email);
        }

        /// <summary>
        /// get customer (and his addresses) by Id
        /// </summary>
        /// <param name="Id">customer Id</param>
        /// <returns></returns>
        public DACustomerExtModel getCustomer(DBInfoModel dbInfo, long Id)
        {
            //1. get customer
            DACustomerModel cust = custTasks.GetCustomer(dbInfo, Id);
            DACustomerExtModel custExt = AutoMapper.Mapper.Map<DACustomerExtModel>(cust);

            //2. get address
            List<DA_AddressModel> adds = addressTask.getCustomerAddresses(dbInfo, Id);

            //3. set billing address
            if (custExt.BillingAddressesId > 0 && adds != null)
            {
                custExt.BillingAddress = adds.FirstOrDefault(x => x.Id == custExt.BillingAddressesId);
                adds.Remove(custExt.BillingAddress);
            }

            //4. set shipping address
            custExt.ShippingAddresses = adds;

            //5. return
            return custExt;
        }

        /// <summary>
        /// Add Customer (without addresses)
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long AddCustomer(DBInfoModel dbInfo, DACustomerModel Model)
        {
            //1. prepare customer model for insert
            custTasks.SanitizeInsertCustomer(Model);
            //2. check string values of customer model for illegal characters
            custTasks.SanitizeCustomer(Model);
            //3. check email uniqueness
            custTasks.CheckUniqueEmail(dbInfo, Model.Email);
            //4. check phones existance
            custTasks.CheckPhoneExistanse(Model);
            //5. insert the new customer to db
            return custTasks.AddCustomer(dbInfo, Model);
        }

        /// <summary>
        /// Add Customer (WITH addresses), from WEB
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public DACustomerExtModel AddCustomer(DBInfoModel dbInfo, DACustomerExtModel Model)
        {
            if (dbInfo == null) dbInfo = storesHelper.GetDAStore();
            //1. prepare customer model for insert
            DACustomerModel cust = AutoMapper.Mapper.Map<DACustomerModel>(Model);
            custTasks.SanitizeInsertCustomer(cust);

            //2. check string values of customer model for illegal characters
            custTasks.SanitizeCustomer(cust);

            //3. check email uniqueness
            custTasks.CheckUniqueEmail(dbInfo, cust.Email);

            //4. check phones existance
            custTasks.CheckPhoneExistanse(cust);

            //5. INSERT the new customer to db
            Model.Id = custTasks.AddCustomer(dbInfo, cust);
            cust.Id = Model.Id;

            //6. INSERT ShippingAddresses to DB
            foreach (var address in Model.ShippingAddresses)
            {
                address.Id = 0;
                address.AddressType = 0;
                address.OwnerId = Model.Id;
                if (address.Longtitude == 0 && address.Latitude == 0)
                {
                    var addr = addressTask.GeoLocationMaps(dbInfo, address);
                    address.Longtitude = addr.Longtitude;
                    address.Latitude = addr.Latitude;
                }
                address.Id = addressTask.AddAddress(dbInfo, address);
            }

            //7. INSERT BillingAddress & UPDATE customer
            if (Model.BillingAddress != null)
            {
                Model.BillingAddress.Id = 0;
                Model.BillingAddress.AddressType = 1;
                Model.BillingAddress.OwnerId = Model.Id;
                if (Model.BillingAddress.Longtitude == 0 && Model.BillingAddress.Latitude == 0)
                {
                    var addr = addressTask.GeoLocationMaps(dbInfo, Model.BillingAddress);
                    Model.BillingAddress.Longtitude = addr.Longtitude;
                    Model.BillingAddress.Latitude = addr.Latitude;
                }
                Model.BillingAddress.Id = addressTask.AddAddress(dbInfo, Model.BillingAddress);
                Model.BillingAddressesId = Model.BillingAddress.Id;
                cust.BillingAddressesId = Model.BillingAddress.Id;
                custTasks.UpdateCustomer(dbInfo, cust);
            }


            return Model;
        }

        /// <summary>
        /// Update Customer.  
        ///  we keep the existing  value for password!!!!
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="fromWeb">true: during update keep the existing value for SecretNotes AND ensure that email is required </param>
        /// <param name="CustomerId">if fromWeb=true then CustomerId is the Customer Id from authorization header</param>
        /// <returns></returns>
        public long UpdateCustomer(DBInfoModel dbInfo, DACustomerModel Model, bool fromWeb = false, long CustomerId = 0)
        {
            //1. check that customer exists in DB
            DACustomerModel existing = custTasks.GetCustomer(dbInfo, Model.Id);
            if (existing == null) throw new BusinessException("Customer does not exist");

            //2. prepare customer model for update
            custTasks.SanitizeUpdateCustomer(Model);

            //3. check string values of customer model for illegal characters
            custTasks.SanitizeCustomer(Model);

            //4. Check email validity
            if (fromWeb && (Model.Email == null || Model.Email == "")) throw new BusinessException("Email is required");
            if (Model.Email != existing.Email) custTasks.CheckUniqueEmail(dbInfo, Model.Email);

            //5. if update is from web then:
            //    a. Only the Current User is allowed to be updated.
            //    b. Keep the existing  value for 'SecretNotes' (if needed) 
            if (fromWeb)
            {
                if (Model.Id != CustomerId) throw new BusinessException("Only the Current User is allowed to be updated.");
                Model.SecretNotes = existing.SecretNotes;
            }

            //6. Keep the existing Values
            Model.Password = existing.Password;
            Model.EmailOld = existing.EmailOld;
            Model.AuthToken = existing.AuthToken;

            if (Model.Metadata == "***")
                Model.Metadata = null;
            else if (string.IsNullOrWhiteSpace(Model.Metadata))
                Model.Metadata = existing.Metadata;


            //7. Delete BillingAddress (if needed)
            if (existing.BillingAddressesId > 0 && Model.BillingAddressesId == 0)
                addressTask.DeleteAddress(dbInfo, existing.BillingAddressesId);

            //8. UPDATE customer
            return custTasks.UpdateCustomer(dbInfo, Model);
        }

        /// <summary>
        /// Delete Customer 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteCustomer(DBInfoModel dbInfo, long Id)
        {
            //1. get customer from  DB
            DACustomerModel existing = custTasks.GetCustomer(dbInfo, Id);
            if (existing == null) return 0;

            //2. Delete BillingAddress (if needed)
            if (existing.BillingAddressesId > 0)
            {
                long c = addressTask.DeleteAddress(dbInfo, existing.BillingAddressesId);
                if (c > 0) // address is deleted...
                {
                    existing.BillingAddressesId = 0;
                    custTasks.UpdateCustomer(dbInfo, existing); // update customer now because you are not sure that he is going to be deleted...
                }
            }

            //3. delete all remaining addresses
            List<DA_AddressModel> adds = addressTask.getCustomerAddresses(dbInfo, Id);
            if (adds != null)
            {
                foreach (var add in adds)
                {
                    addressTask.DeleteAddress(dbInfo, add.Id);
                }
            }

            //4. delete customer
            return custTasks.DeleteCustomer(dbInfo, Id);
        }


        /// <summary>
        /// User has forgot his password. Application create and return a session key
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="model">model containing the customer's email</param>
        /// <returns></returns>
        public string ForgotPassword(DBInfoModel dbInfo, ForgotPasswordModel model)
        {
            //1. search user by email
            List<DASearchCustomerModel> res = custTasks.SearchCustomers(dbInfo, DA_CustomerSearchTypeEnum.Email, model.Email);
            if (res == null || res.Count == 0) throw new BusinessException(Symposium.Resources.Errors.WRONGEMAIL);

            //2.create session key
            string key = custTasks.CreateSessionKey();

            //3. save SessionKey to DB
            custTasks.UpdateSessionKey(dbInfo, res[0].daCustomerModel.Id, key);

            //4.
            return key;
        }

        /// <summary>
        /// Change password based on email and Session key
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="model"></param>
        public void ChangePasswordSessionKey(DBInfoModel dbInfo, DALoginSessionKeyModel model)
        {
            //1. authenticate user based on email and session key
            long id = custTasks.LoginUserSessionKey(dbInfo, model);
            //2. change password
            custTasks.UpdatePassword(dbInfo, id, model.Password);
        }


        /// <summary>
        /// Change password based on email and old password
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="model">DAchangePasswordModel</param>
        /// <param name="CustomerId">Customer Id from Auth Model</param>
        public void ChangePassword(DBInfoModel dbInfo, DAchangePasswordModel model, long CustomerId)
        {
            //1. validate customer
            var cust = custTasks.GetCustomer(dbInfo, CustomerId);
            if (cust == null) throw new BusinessException("Customer not found");
            if (cust.Email != model.Email || cust.Password != MD5Helper.SHA1(model.OldPassword) || cust.Id != CustomerId) throw new BusinessException("Wrong Customer");

            //2. change password
            custTasks.UpdatePassword(dbInfo, CustomerId, model.NewPassword);
        }


        /// <summary>
        /// Resets password of FIRST customer with given email and mobile and clears email of other customers with given email and mobile
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        public void ResetPassword(DBInfoModel dbInfo, DAresetPasswordModel model)
        {
            List<DACustomerModel> customers = custTasks.GetCustomersByEmailMobile(dbInfo, model.Email, model.Mobile);
            if (customers.Count() > 0)
            {
                DACustomerModel firstCustomer = customers.First();
                long customerId = firstCustomer.Id;
                custTasks.UpdateOnePasswordClearOtherEmails(dbInfo, customers, customerId, model.NewPassword);
                logger.Info("Successful Reset Password for email: " + (model.Email ?? "<NULL>") + " and mobile: " + (model.Mobile ?? "<NULL>"));
            }
            else
            {
                logger.Warn("Reset password for email: " + (model.Email ?? "<NULL>") + " and mobile: " + (model.Mobile ?? "<NULL>") + " failure. No customer with this combination email & mobile found in DB.");
                throw new BusinessException(Symposium.Resources.Errors.CUSTOMERNOTFOUND);
            }
        }

        /// <summary>
        /// Create Authorization Token for a customer based on email and mobile. Return the new AuthToken
        /// </summary>
        /// <returns></returns>
        public string CreateAuthToken(DBInfoModel dbInfo, DACustomerIdentifyModel model)
        {
            List<DACustomerModel> custs = custTasks.GetCustomersByEmailMobile(dbInfo, model.Email, model.Mobile);
            string token = "";
            if (custs == null || custs.Count == 0)
            {
                logger.Warn($"No Customer found with {model.Email} and {model.Mobile}.");
                throw new BusinessException("Unknown Customer");
            }
            if (custs != null && custs.Count > 1)
            {
                logger.Warn($"Found more than one Customer with {model.Email} and {model.Mobile}.");
                throw new BusinessException("Unknown Customer");
            }
            if (string.IsNullOrWhiteSpace(custs[0].AuthToken))
            {
                cashedLoginsHelper.RemoveLogin(custs[0].Id);
                token = custTasks.CreateAuthToken();
                custs[0].AuthToken = token;

                logger.Info($"Changing Customer AuthToken with id={custs[0].Id}...");
                custTasks.UpdateCustomer(dbInfo, custs[0]);
            }
            else
            {
                token = custs[0].AuthToken;
            }
            return token;

        }


        /// <summary>
        /// Get external id 2 of customer and if there is password with given email
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public ExternalId2PasswordModel GetExternalId2(DBInfoModel dbInfo, GetExternalIdModel model)
        {
            string email = model.Email;
            ExternalId2PasswordModel externalIdPassword = new ExternalId2PasswordModel();
            string externalId = custTasks.GetExternalId2(dbInfo, email);
            if (externalId == null)
            {
                externalId = "";
            }
            externalIdPassword.ExtId2 = externalId;
            bool hasPassword = custTasks.hasPassword(dbInfo, email);
            externalIdPassword.hasPassword = hasPassword;
            return externalIdPassword;
        }

        /// <summary>
        /// Identify customer based on mobile and/or email.  
        /// Return Dictionary of int, long: 
        ///   
        ///   Key:
        ///          3 = customer found with specific/not null values for email and mobile. 
        ///          2 = customer found with specific email and empty mobile. 
        ///          1 = customer found with specific mobile and empty email. 
        ///          0 = NO customer found with specific/not null values for email and mobile. 
        ///          
        ///  Value:  The Customer Id. In case of NO customer found return CustomerId=0.
        /// 
        /// </summary>
        public Dictionary<int, long> IdentifyCustomer(DBInfoModel Store, DACustomerIdentifyModel existModel)
        {
            DACustomerIdentifyResultModel res = new DACustomerIdentifyResultModel();
            Dictionary<int, long> result = new Dictionary<int, long>();

            long customerId = custTasks.ExistMobileEmail(Store, existModel);
            result.Add(3, customerId);

            customerId = custTasks.ExistEmail(Store, existModel.Email);
            result.Add(2, customerId);

            customerId = custTasks.ExistMobile(Store, existModel.Mobile);
            result.Add(1, customerId);

            return result;


        }


        /// <summary>
        /// Get customer info from 3rd party source
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public DACustomerModel GetCustomerInfoExternal(DBInfoModel Store, string phone)
        {
            Dictionary<string, dynamic> configuration = configurationTasks.GetConfiguration("api");
            DACustomerModel customer = custTasks.GetCustomerInfoExternalByPhone(Store, phone, configuration);
            return customer;
        }

    }
}
