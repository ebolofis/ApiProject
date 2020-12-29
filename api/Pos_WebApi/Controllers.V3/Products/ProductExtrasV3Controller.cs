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
    [RoutePrefix("api/v3/ProductExtras")]
    public class ProductExtrasV3Controller : BasicV3Controller
    {
        IProductExtrasFlows flow;
        public ProductExtrasV3Controller(IProductExtrasFlows flow)
        {
            this.flow = flow;
        }

        /// <summary>
        /// Upsert a list of Product Extras from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpsertProductExtras")]
        [Authorize]
        public HttpResponseMessage UpsertProductExtras(List<ProductExtrasSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = flow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
