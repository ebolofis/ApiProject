using Pos_WebApi.Modules;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Plugins;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
    //  [RoutePrefix("api")]
    public class DA_ConfigController : BasicV3Controller
    {
        IDA_ConfigFlows configFlow;

        public DA_ConfigController(IDA_ConfigFlows _configFlow)
        {
            this.configFlow = _configFlow;
        }

        /// <summary>
        /// Get DA Config  (DA_BaseUrl, StaffUserName, StaffPassword)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Config/GetConfig")]
        [Route("api/v3/Config/GetConfig")]
        [Authorize]
        public HttpResponseMessage GetConfig()
        {
            DA_ConfigModel res = configFlow.GetConfig(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get StoreId and PosId(The FirstOrDefault PosId)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Config/GetStorePos")]
        [Route("api/v3/Config/GetStorePos")]
        [Authorize]
        public HttpResponseMessage GetStorePos()
        {
            DA_GetStorePosModel res = configFlow.GetStorePos(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Is Delivery Agent (true or false)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Config/isDA")]
        [Route("api/v3/Config/isDA")]
        [Authorize]
        public HttpResponseMessage isDA()
        {
            bool res = configFlow.isDA();
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// return if api is DA Client (true/false)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Config/isdaclient")]
        [Route("api/v3/Config/isdaclient")]
        [Authorize]
        public HttpResponseMessage isDAClient()
        {
            bool res = configFlow.isDAclient();
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// return StoreId (guid)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Config/getstoreguid")]
        [Route("api/v3/Config/getstoreguid")]
        [Authorize]
        public HttpResponseMessage GetStoreGuid()
        {
            string res = configFlow.getDAStoreId();
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }



        /// <summary>
        /// return the list of statuses that are allowed to be canceled
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Config/statusCancel")]
        [Route("api/v3/Config/statusCancel")]
        [Authorize]
        public HttpResponseMessage statusCancel()
        {
            List<string> res = configFlow.getDACancel();
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// ping the DA api (from DA client api)
        /// </summary>
        /// <returns>ok</returns>
        [HttpGet, Route("api/v3/da/Config/ping")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage ping()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}