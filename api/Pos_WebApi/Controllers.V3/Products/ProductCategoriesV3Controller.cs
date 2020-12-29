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
    // [RoutePrefix("api/v3/ProductCategories")]
    public class ProductCategoriesV3Controller : BasicV3Controller
    {
        IProductCategoriesFlows flow;
        public ProductCategoriesV3Controller(IProductCategoriesFlows flow)
        {
            this.flow = flow;
        }

        /// <summary>
        /// Return the list with Product Categories 
        /// </summary>
        /// <param name="enabled">enabled = 1: return only enabled Product Categories, enabled = 0 : return all </param>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/ProductCategories/get/{enabled=1}")]
        [Route("api/v3/ProductCategories/get/{enabled=1}")]
        public HttpResponseMessage GetAll([FromUri]int enabled = 1)
        {
            bool status;
            if (enabled == 1)
                status = true;
            else
                status = false;
            List<ProductCategoriesModel> res = flow.GetAll(DBInfo, status);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("api/v3/ProductCategories/GetComboList")]
        public HttpResponseMessage GetComboList()
        {
            List<ProductsCategoriesComboList> res = flow.GetComboList(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Upsert a list of Product Categories from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/ProductCategories/UpsertProductCategories")]
        [Authorize]
        public HttpResponseMessage UpsertProductCategories(List<ProductCategoriesSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = flow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update the Vat Value of Selected Product Categories
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/ProductCategories/UpdateProductCategoriesVat")]
        [Authorize]
        public HttpResponseMessage UpdateProductCategoriesVat(ChangeProdCatModel Obj)
        {
             var  res = flow.UpdateProductCategoriesVat(DBInfo, Obj);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// get list of product category ids and descriptions based on given product category ids
        /// </summary>
        /// <param name="productCategoryIdList">List<long></param>
        /// <returns>List<ProductsCategoriesComboList></returns>
        [HttpPost, Route("api/v3/ProductCategories/GetProductCategoryDescriptions")]
        [Authorize]
        public HttpResponseMessage GetProductCategoryDescriptions(ProductsCategoriesIdList productCategoryIdList)
        {
            List<ProductsCategoriesComboList> res = flow.GetProductCategoryDescriptions(DBInfo, productCategoryIdList);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}


