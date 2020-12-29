using LinqKit;
using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Models;
using Pos_WebApi.Models.FilterModels;
using Pos_WebApi.Repositories;
using Pos_WebApi.Repositories.BORepos;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers {
    public class ProductPricesController : ApiController {
        ProductPricesRepository svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProductPricesController() {
            svc = new ProductPricesRepository(db);
        }

        /// <summary>
        /// Paged Result Resource of ProductPrices model
        /// </summary>
        /// <param name="page"> Param to skip entries by Page * Pagesize to get pagginated data</param>
        /// <param name="pageSize"> Param to characterize Size of a Page returned and used also on skip of pageResults </param>
        /// <param name="isIngredient">if set to true resource acts on ingredients </param>
        /// <returns> Returns a Collection of filtered Product || Ingredients </returns>
        public PagedResult<ProductPricesModel> GetProductPrices(int page, int pageSize, bool isIngredient) {
            if (isIngredient)
                return svc.GetPagedIngredients(x => true, s => "ProductCode", page, pageSize);
            else
                return svc.GetPaged(x => true, s => "ProductCode", page, pageSize);
        }

        /// <summary>
        /// A GET Resource to collect products OR ingredients filtered by page pagesize and json of filters that 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="filters"> Flat AND filter Predicate according to model provided </param>
        /// <param name="page"> Param to skip entries by Page * Pagesize to get pagginated data</param>
        /// <param name="pageSize"> Param to characterize Size of a Page returned and used also on skip of pageResults </param>
        /// <param name="isIngredient"> if set to true resource acts on ingredients </param>
        /// <returns> Returns a Collection of filtered Product || Ingredients </returns>
        public dynamic GetByFilters(string storeId, string filters, int page, int pageSize, bool isIngredient) {
            var flts = JsonConvert.DeserializeObject<ProductsWithCategoriesFlatFilters>(filters);
            if (isIngredient)
                return svc.GetPagedIngredients(flts.predicate, s => "ProductCode", page, pageSize);
            else
                return svc.GetPaged(flts.predicate, s => "ProductCode", page, pageSize);
        }

        // PUT api/ProductPrices/[{}]
        public HttpResponseMessage PutProductPrices(IEnumerable<PricelistDetail> details) {
            if (!ModelState.IsValid) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            svc.UpdateProductPrices(details);
            try {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
                var res = svc.RemoveDuplicatesFromProductPrices();
                res = svc.RemoveDuplicatesFromIngredientPrices();
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error Updating Product Prices | " + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Resource Get that returns ProductPrices or IngredientsPrices, according to bool variable Ingredients,
        /// that does not Have all PricelistsDetails Matched over PricelistIds registered
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="ingredients">Bool value if set to true BI return IngredientPrices</param>
        /// <returns>List of MissingPrices {Id: (ProductId || IngredientId), Pricelists (Ids missing)}</returns>
        [Route("api/{storeId}/ProductPrices/GetMissingPrices/{ingredients}")]
        [HttpGet]
        public HttpResponseMessage GetMissingPrices(string storeId, bool ingredients = false) {
            try {
                db = new PosEntities(false, Guid.Parse(storeId));
                svc = new ProductPricesRepository(db);
                var res = svc.getMissingProductPrices(ingredients);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        /// <summary>
        /// Post Function parse vat id and an override variable for ingredients to make action over these
        /// Functionality Calls repo witch get produvts from GetMissingPrices models and create new DTOS of PPrices
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="vatid"> ID provided to bind details created to this vat </param>
        /// <param name="ingredients"> Optional  bool variable if set to true then handling function acts on ingredients. </param>
        /// <returns> A string msg of action result over missmatches correction </returns>
        [Route("api/{storeId}/ProductPrices/FixMissingPrices/{vatid}/{ingredients}")]
        [HttpPost]
        public HttpResponseMessage FixMissingPrices(string storeId, long vatid, bool ingredients = false) {
            try {
                db = new PosEntities(false, Guid.Parse(storeId));
                svc = new ProductPricesRepository(db);
                bool success = svc.fixMissingProductPrices(vatid, ingredients);
                if (!success)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.PRICESMISSMATCHESFAILED);
                return Request.CreateResponse(HttpStatusCode.OK, "Product Prices missmatches fixed.");
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Resource to auto fix Dublicates on ProductPrices assigned to a product
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns> String of msg by action correctly applied</returns>
        [Route("api/{storeId}/ProductPrices/RemoveDuplicateProductPrices")]
        [HttpPost]
        public HttpResponseMessage RemoveDuplicateProductPrices(string storeId) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new ProductPricesRepository(db);
            var res = svc.RemoveDuplicatesFromProductPrices();
            if (res)
                return Request.CreateErrorResponse(HttpStatusCode.OK, Symposium.Resources.Messages.DUPLICATEVALUESREMOVED);
            else
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.DUPLICATEVALUESREMOVEDFAILED);
        }

        /// <summary>
        /// Resource of auto Correction to Clear dublicate ProductPrices
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns> String of msg by action correctly applied</returns>
        [Route("api/{storeId}/ProductPrices/RemoveDuplicateIngredientPrices")]
        [HttpPost]
        public HttpResponseMessage RemoveDuplicateIngredientPrices(string storeId) {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new ProductPricesRepository(db);


            var res = svc.RemoveDuplicatesFromIngredientPrices();
            if (res)
                return Request.CreateErrorResponse(HttpStatusCode.OK, Symposium.Resources.Messages.DUPLICATEVALUESREMOVED);
            else
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.DUPLICATEVALUESREMOVEDFAILED);

        }

        protected override void Dispose(bool disposing) {
            db.Dispose();

            base.Dispose(disposing);
        }
    }

}
