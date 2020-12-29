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
   // [RoutePrefix("api/v3/PriceList")]
    public class PriceListV3Controller : BasicV3Controller
    {
        IPricelistFlows flow;
        public PriceListV3Controller(IPricelistFlows flow)
        {
            this.flow = flow;
        }

        /// <summary>
        /// Return the extended pricelists (active only). Every pricelist contains the list of Details.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/PriceList/getflat")]
        [Route("api/v3/PriceList/getflat")]
        public HttpResponseMessage GetPricelists()
        {
            List<PricelistExtModel> res = flow.GetExtentedList(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Upsert a list of Price Lists from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/PriceList/UpsertPricelist")]
        [Authorize]
        public HttpResponseMessage UpsertPricelist(List<PriceListSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = flow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
