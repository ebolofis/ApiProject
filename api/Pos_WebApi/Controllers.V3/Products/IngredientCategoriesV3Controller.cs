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
    [RoutePrefix("api/v3/IngredientCategories")]
    public class IngredientCategoriesV3Controller : BasicV3Controller
    {
        IIngredientCategoriesFlows flow;
        public IngredientCategoriesV3Controller(IIngredientCategoriesFlows flow)
        {
            this.flow = flow;
        }

        /// <summary>
        /// Upsert a list of Ingredient Categories from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpsertIngredientCategories")]
        [Authorize]
        public HttpResponseMessage UpsertIngredientCategories(List<IngredCategoriesSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = flow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get all ingredient categories
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetAllIngredientCategories")]
        [Authorize]
        public HttpResponseMessage GetAllIngredientCategories()
        {
            List<IngredientCategoriesModel> res = flow.GetAllIngredientCategories(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

    }
}
