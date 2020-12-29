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
    [RoutePrefix("api/v3/PriceList_EffectiveHours")]
    public class PriceList_EffectiveHoursV3Controller : BasicV3Controller
    {
        IPriceList_EffectiveHoursFlows flow;
        public PriceList_EffectiveHoursV3Controller(IPriceList_EffectiveHoursFlows flow)
        {
            this.flow = flow;
        }

        /// <summary>
        /// Upsert a list of Price List Effective Hours from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpsertPriceList_EffectiveHours")]
        [Authorize]
        public HttpResponseMessage UpsertPriceList_EffectiveHours(List<PriceList_EffHoursSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = flow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
