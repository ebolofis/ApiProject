using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3
{
   // [RoutePrefix("api/v3/Product")]
    public class ProductV3Controller : BasicV3Controller
    {
        IProductFlows flow;
        IProductBarcodesFlows barCodeFlow;

        public ProductV3Controller(IProductFlows flow, IProductBarcodesFlows barCodeFlow)
        {
            this.flow = flow;
            this.barCodeFlow = barCodeFlow;
        }

        /// <summary>
        /// Get Products(DAId, Description) For Dropdown List 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/Product/GetComboListDA")]
        public HttpResponseMessage GetComboListDA()
        {
            List<ProductsComboList> res = flow.GetComboListDA(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("api/v3/Product/GetComboList")]
        public HttpResponseMessage GetComboList()
        {
            List<ProductsComboList> res = flow.GetComboList(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Return the extended list of products (active only). Every product contains the list of (active) Extras.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Product/getflat")]
        [Route("api/v3/Product/getflat")]
        public HttpResponseMessage GetProductsExtras()
        {
            List<ProductExtModel> res = flow.GetExtentedList(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("api/v3/da/Product/getdepartmentflat/PosDepartmentId/{PosDepartmentId}/{pricelistId}")]
        [Route("api/v3/Product/getflat")]
        public HttpResponseMessage GetDepartmentExtentedList(long PosDepartmentId, long pricelistId)
        {
            List<ProductExtModel> res = flow.GetDepartmentExtentedList(DBInfo, PosDepartmentId, pricelistId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

      
        /// <summary>
        /// Return List of Pruducts Categories (acrive only). 
        /// Every List of Pruducts Categories includes the extended list of products (active only). 
        /// Every product contains the list of (active) Ingredient Categories. 
        /// Every Ingredient Category includes the extended list of Ingredients (active only).
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Product/GetSkroutzProducts/PricelistId/{PricelistId}")]
        [Route("api/v3/Product/GetSkroutzProducts/PricelistId/{PricelistId}")]
        public HttpResponseMessage GetSkroutzProducts(long PricelistId)
        {
            List<ProductCategoriesSkroutzExtModel> res = flow.GetSkroutzProducts(DBInfo, PricelistId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Upsert a list of Products from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/Product/UpsertProducts")]
        [Authorize]
        public HttpResponseMessage UpsertProducts(List<ProductSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = flow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Upsert a list of Products from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/Product/UpdateShortageProds")]
        public HttpResponseMessage UpdateShortageProds(List<ProductSched_Model> Model)
        {
            //bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            //if (!isDeliveryStore)
            //    throw new Exception("Store is not client of Delivery Agent");

            //UpsertListResultModel res = flow.InformTablesFromDAServer(Store, Model);
            //return Request.CreateResponse(HttpStatusCode.OK, res);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Upsert a list of Products from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/Product/UpsertProductBarcodes")]
        public HttpResponseMessage UpsertProductBarcodes(List<ProductBarcodesSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = barCodeFlow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// get list of product ids and descriptions based on given product ids
        /// </summary>
        /// <param name="productIdList">List<long></param>
        /// <returns>List<ProductDescription></returns>
        [HttpPost, Route("api/v3/Product/GetProductDescriptions")]
        [Authorize]
        public HttpResponseMessage GetProductDescriptions(ProductIdsList productIdList)
        {
            List<ProductDescription> res = flow.GetProductDescriptions(DBInfo, productIdList);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
