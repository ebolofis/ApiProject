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
    [RoutePrefix("api/v3/Ingredient_ProdCategoryAssoc")]
    public class Ingredient_ProdCategoryAssocV3Controller : BasicV3Controller
    {
        IIngredient_ProdCategoryAssocFlows flow;
        public Ingredient_ProdCategoryAssocV3Controller(IIngredient_ProdCategoryAssocFlows flow)
        {
            this.flow = flow;
        }

        /// <summary>
        /// Upsert a list of Ingredient ProdCategoryAssoc from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpsertIngredient_ProdCategoryAssoc")]
        [Authorize]
        public HttpResponseMessage UpsertIngredient_ProdCategoryAssoc(List<IngedProdCategAssocSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = flow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
