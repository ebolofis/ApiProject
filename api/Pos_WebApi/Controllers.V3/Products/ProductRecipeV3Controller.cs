using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.Products
{
    [RoutePrefix("api/v3/ProductRecipe")]
    public class ProductRecipeV3Controller : BasicV3Controller
    {
        IProductRecipeFlows flow;

        public ProductRecipeV3Controller(IProductRecipeFlows flow)
        {
            this.flow = flow;
        }

        /// <summary>
        /// Insert's Or Update a list of Product Recipe
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpsertProductRecipe")]
        [Authorize]
        public HttpResponseMessage UpsertProductRecipe(List<ProductRecipeSched_Model> Model)
        {

            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = flow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

    }
}
