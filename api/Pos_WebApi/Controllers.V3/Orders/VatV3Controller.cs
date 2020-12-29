using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3
{
   // [RoutePrefix("api/v3/Vat")]
    public class VatV3Controller : BasicV3Controller
    {
        IVatFlows flow;
        public VatV3Controller(IVatFlows flow)
        {
            this.flow = flow;
        }

        /// <summary>
        /// Return a list with all vats Included deleted
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Vat/get")]
        [Route("api/v3/Vat/get")]
        public HttpResponseMessage GetAllVats()
        {
            List<VatModel> res = flow.GetAllVats(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Upsert a list of Vats from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/Vat/UpsertVat")]
        [Authorize]
        public HttpResponseMessage UpsertVat(List<VatSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = flow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
