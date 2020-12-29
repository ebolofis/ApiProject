using log4net;
using Microsoft.AspNet.SignalR;
using Pos_WebApi.Controllers.Helpers;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ServiceProcess;
using System.Collections.Generic;
using Symposium.Models.Models;
using Pos_WebApi.Helpers;
using System.Linq;
using Symposium.Helpers.Classes;

namespace Pos_WebApi.Controllers.BOControllers {
    public class ConnectionServiceController : ApiController {
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Resource that checks connection to HotelInfo entity provided with type HotelInfo = 1  || PmsInterface = 4
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/{storeId}/ConnectionService/CheckProtelSql")]
        public HttpResponseMessage CheckProtelSql(string storeId, ProtelConnectionModel model) {
            if (!ModelState.IsValid) {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ModelState);
            }
            var constring = new SqlConnectionStringBuilder();
            constring.InitialCatalog = model.databasename; //"Protel";
            constring.DataSource = model.server; //"192.168.15.31";
            constring.UserID = model.userName;//"proteluser";
            constring.Password = (model.password.Length < 20) ? model.password : StringCipher.Decrypt(model.password);//"protel915930";
            SqlConnection sqlcon = new SqlConnection(constring.ConnectionString);

            try {
                sqlcon.Open();
                string ret = "Sql connection Status:" + sqlcon.State;
                if ((int)sqlcon.State == 1) {
                    sqlcon.Close();
                    return Request.CreateResponse(HttpStatusCode.OK, ret);
                } else {
                    sqlcon.Close();
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ret);
                }
            } catch (Exception e) {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            } finally {
                if ((int)sqlcon.State == 0) {
                    sqlcon.Close();
                }
            }
        }
        [HttpPost]
        [Route("api/{storeId}/ConnectionService/StoreSql")]
        public HttpResponseMessage CheckStoreSql(string storeId) {
            string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\UsersToDatabases.xml";
            Installations installations = XmlHelpers.ParseXmlDoc(xml);
            Symposium.Models.Models.DBInfoModel store = installations.Stores.Where(w => w.Id == new Guid(storeId)).FirstOrDefault();
            var constring = new SqlConnectionStringBuilder();
            constring.InitialCatalog = store.DataBase; //"Protel";
            constring.DataSource = store.DataSource; //"192.168.15.31";
            constring.UserID = store.DataBaseUsername;//"proteluser";
            constring.Password = store.DataBasePassword;//"protel915930";

            SqlConnection sqlcon = new SqlConnection(constring.ConnectionString);

            try {
                sqlcon.Open();
                string ret = "Sql connection Status:" + sqlcon.State;
                if ((int)sqlcon.State == 1) {
                    sqlcon.Close();
                    return Request.CreateResponse(HttpStatusCode.OK, ret);
                } else {
                    sqlcon.Close();
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ret);
                }
            } catch (Exception e) {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            } finally {
                if ((int)sqlcon.State == 0) {
                    sqlcon.Close();
                }
            }
        }
        /// <summary>
        /// Resource to return models of { DisplayName , ServiceName , Status }
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/{storeId}/ConnectionService/RunningServices")]
        public HttpResponseMessage RunningServices(string storeId) {
            try {
                List<dynamic> sqlServiceNames = new List<dynamic>();

                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController sc in services) {
                    if (sc.ServiceName.Contains("SQL") || sc.DisplayName.Contains("SQL")) {
                        var t = new { DisplayName = sc.DisplayName, ServiceName = sc.ServiceName, Status = sc.Status };
                        sqlServiceNames.Add(t);
                    }
                }
                //SQL Server (SQLJVAL) MSSQL$SQLJVAL Status: "4" SQL Full - text Filter Daemon Launcher(SQLJVAL)
                //SQL Server Agent(SQLJVAL) SQLAgent$SQLJVAL Status: "1"
                return Request.CreateResponse(HttpStatusCode.OK, sqlServiceNames);
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }
        /// <summary>
        /// A Resource to Run Service by ServiceName. Flow here is when service is not running
        /// http://www.csharp-examples.net/restart-windows-service/
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="ServiceName">ServiceController.GetServices()[i].ServiceName</param>
        /// <returns>Responce Msg in HttpResponce</returns>
        [HttpPost]
        [Route("api/{storeId}/ConnectionService/StartService/{ServiceName}")]
        public HttpResponseMessage StartService(string storeId, string ServiceName) {
            try {
                ServiceController service = new ServiceController(ServiceName);
                if (service == null) {
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, string.Format(Symposium.Resources.Messages.NOSERVICEAPPLIED, ServiceName));
                }
                if (service.Status != ServiceControllerStatus.Running) {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    //ex
                    //SQL Server (SQLJVAL) MSSQL$SQLJVAL Status: "4"
                    //SQL Server Agent(SQLJVAL) SQLAgent$SQLJVAL Status: "1"
                    return Request.CreateResponse(HttpStatusCode.OK, string.Format(Symposium.Resources.Messages.SERVICESTARTEDSUCCESSFULLY, ServiceName));
                } else {
                    return Request.CreateResponse(HttpStatusCode.OK, string.Format(Symposium.Resources.Messages.SERVICECURRENTLYRUNNING, ServiceName));
                }
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }
        public class ProtelConnectionModel {
            public ProtelConnectionModel() {

            }
            public string databasename { get; set; }
            public string server { get; set; }
            public string userName { get; set; }
            public string password { get; set; }
        }
    }
}