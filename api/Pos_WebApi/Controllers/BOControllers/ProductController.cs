using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using Pos_WebApi.Models.FilterModels;
using Newtonsoft.Json;
using System;
using log4net;

namespace Pos_WebApi.Controllers {
    public class ProductController : ApiController {
        ProductRepository svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProductController() {
            svc = new ProductRepository(db);
        }

        // GET api/Product
        public IEnumerable<ProductDTO> GetProducts() {
            return svc.GetAll();
        }
        /// <summary>
        /// Function that returns a list of products paged by pagesize
        /// </summary>
        /// <param name="page">variable to get specific page</param>
        /// <param name="pageSize">the length of list results per page if -1 getsAll</param>
        /// <param name="filters">Reference on predicate that filters Id Descriptions Code ProductCategoryId</param>
        /// <param name="orFilter">if apply true then query uses OR function predicate else it acts as AND</param>
        /// <returns></returns>
        public PagedResult<ProductDTO> GetProduct(int page, int pageSize, string filters, bool orFilter = false) {
            {
                var flts = JsonConvert.DeserializeObject<ProductFilter>(filters);
                bool usedelete = (flts.IsDeleted == true) ? true : false;
                if (orFilter)
                    return svc.GetPaged(flts.orpredicate, s => "Id", page, pageSize, usedelete);
                else
                    return svc.GetPaged(flts.predicate, s => "Id", page, pageSize, usedelete);
            }
        }

        public IEnumerable<ProductDTO> GetUniqueProduct(string uniqueCode) {
            {
                return svc.GetAll(s => s.Code == uniqueCode);
            }
        }
        /// <summary>
        /// A function that provides an array of codes. Returns product DTOs with codes included on provided array
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="codes">Array of strings from codes of products</param>
        /// <returns>Http responce with data of product dtos found with same code</returns>
        [Route("api/{storeId}/Product/CheckUniqueProductCodes")]
        [HttpPost]
        public HttpResponseMessage CheckUniqueProductCodes(string storeId, List<ProductDTO> codes) {
            try {
                List<dynamic> ret = new List<dynamic>();
                db = new PosEntities(false, Guid.Parse(storeId)); svc = new ProductRepository(db);
                foreach (var model in codes) {
                    var rrs = svc.GetAll(s => model.Code == s.Code && model.Id != s.Id && s.IsDeleted != true).ToList();
                    ret.Add(new {
                        Id = model.Id,
                        Cnt = rrs.Count(),
                        Code = model.Code,
                        SameIds = rrs.Select(s => new { Id = s.Id, Description = s.Description }).ToList()
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, ret);
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        /// <summary>
        /// A resource providing search of products that have ProductBarcodes StartsWith search string provided
        /// </summary>
        /// <param name="barcode">Filter Search string of ProductBarcodes</param>
        /// <returns></returns>
        public IEnumerable<Product> GetProductsByBarcode(string barcode) {
            return svc.GetByBarcode(null, barcode);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pricelistid"></param>
        /// <param name="posDepId"></param>
        /// <param name="barcodesearch"></param>
        /// <param name="aba"></param>
        /// <returns></returns>
        public Object GetPageButtonFromProductBC(int pricelistid, long posDepId, string barcodesearch,bool isPos=false)
        {// bool aba = false
            return svc.GetPageButtonFromProductBC(pricelistid, posDepId, barcodesearch, isPos);
        }
        /// <summary>
        /// Resource with id field. Returns product by id asked
        /// </summary>
        /// <param name="id">Id of Product.Id as filter</param>
        /// <returns></returns>
        public Product GetProduct(long id) {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }
        /// <summary>
        /// Resource with id field. Returns product by id asked
        /// </summary>
        /// <param name="did"></param>
        /// <param name="detailed">a boolean filter to distinguish from single resource findby(id)</param>
        /// <returns></returns>
        public ProductDTO GetProductDetailed(long did, bool detailed) {
            return svc.FindByDetailed(did).FirstOrDefault();
        }
        /// <summary>
        /// Resource that restores database products that are marked as deleted
        /// a product is marked as isDeleted:true when a delete action performed and product was previously 
        /// assigned on a receipt or order as an order item
        /// that will keep product in database as its ID created constrains over entities and statistics will need its ref 
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="ids">An Array of Ids of products you wish to Restore</param>
        /// <returns></returns>
        [Route("api/{storeId}/Product/RestoreIsDeleted")]
        [HttpPut]
        public HttpResponseMessage RestoreIsDeleted(string storeid, List<long> ids) {
            db = new PosEntities(false, Guid.Parse(storeid));
            svc = new ProductRepository(db);

            if (!ModelState.IsValid) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            svc.RestoreIds(ids);

            try {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, ids);
        }
        // PUT api/Product/5
        [HttpPut]
        public HttpResponseMessage PutProduct(string storeid, ProductDTO model) {
            db = new PosEntities(false, Guid.Parse(storeid));
            svc = new ProductRepository(db);
            var prprices = new ProductPricesRepository(db);

            if (!ModelState.IsValid) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            svc.Update(model);
            try {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
                svc.RemoveDuplicatesProductExtras();
                prprices.RemoveDuplicatesFromProductPrices();
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Crud Resource Add a new product with details and prices 
        /// Uses PPRepo to save then it actcs as cleaner - correcter ewhere Remove Dublicate on Extras and Recipe
        /// </summary>
        /// <param name="model">Product Model from UI </param>
        /// <returns> HTTP with model added on data (here model has an Id assigned as save changes made on repo lvl )</returns>
        public HttpResponseMessage PostProduct(ProductDTO model) {
            var prprices = new ProductPricesRepository(db);
            if (ModelState.IsValid) {
                try {
                    svc.Add(model);
                    svc.RemoveDuplicatesProductExtras();
                    prprices.RemoveDuplicatesFromProductPrices();
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                //response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        /// <summary>
        /// Single delete product by  Id provided product on BI logic will even marked as deleted if an invoice has been created over it
        /// or fully deleted by removing its dependencies across entities in POS context
        /// </summary>
        /// <param name="id">param identifier for product to dete</param>
        /// <returns>Id of product deleted</returns>
        // DELETE api/Product/5
        public HttpResponseMessage DeleteProduct(long id) {
            try {
                var res = svc.Delete(id);
                if (res == true) {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.SAVEDELETEDFAILED);
                } else {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Deletion Failed");

                }
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        [Route("api/{storeId}/Product/AddRange")]
        [HttpPost]
        public HttpResponseMessage AddRange(string storeId, IEnumerable<ProductDTO> models) {
            db = new PosEntities(false, Guid.Parse(storeId)); svc = new ProductRepository(db);
            var prprices = new ProductPricesRepository(db);

            if (ModelState.IsValid) {
                var ret = svc.AddRange(models);
                try {
                    //if (ret.Count() <= 0)
                    //return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Updating ranged products Failed.");
                    //manage auto fix remove of extras and manage prices 
                    try {
                        svc.RemoveDuplicatesProductExtras();
                        prprices.RemoveDuplicatesFromProductPrices();
                    } catch (Exception e) {
                        logger.Error(e.ToString());
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Removing dublicates ion extras and prices.");
                    }
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, ret);
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Invalid Model State of Products.");
            }
        }

        /// <summary>
        /// Update functionality over ranged changes of products on server
        /// Once it updates and save changes proccess will try to removedublicate extras and handle null prices
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="models">Array of Edited Products<dtos> with is detailed entities</param>
        /// <returns>Models edited and successfully comited with changes on context in an http:200 response</returns>
        [Route("api/{storeId}/Product/UpdateRange")]
        [HttpPut]
        public HttpResponseMessage UpdateRange(string storeId, IEnumerable<ProductDTO> models) {
            db = new PosEntities(false, Guid.Parse(storeId)); svc = new ProductRepository(db);
            var prprices = new ProductPricesRepository(db);

            if (ModelState.IsValid) {
                svc.UpdateRange(models);
                try {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Updating ranged products Failed.");
                    //manage auto fix remove of extras and manage prices 
                    try {
                        svc.RemoveDuplicatesProductExtras();
                        prprices.RemoveDuplicatesFromProductPrices();
                    } catch (Exception e) {
                        logger.Error(e.ToString());
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Removing dublicates ion extras and prices.");
                    }
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, models);
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Invalid Model State of Products.");
            }
        }
        /// <summary>
        /// Providing a list of ids you wish to del this resource will call multi del on products repo
        /// by interating over ids to single delete product functionality
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="ids">Input Product Ids array</param>
        /// <returns>Either a msg on http or same array of ids on success</returns>
        [Route("api/{storeId}/Product/DeleteRange")]
        [HttpDelete]
        public HttpResponseMessage DeleteRange(string storeId, IEnumerable<long> ids) {
            try {
                db = new PosEntities(false, Guid.Parse(storeId));
                svc = new ProductRepository(db);

                var res = svc.DeleteRange(ids);
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Trying to save db after multiple delete failed");
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, ids);
        }

        [AllowAnonymous]
        public HttpResponseMessage Options() {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing) {
            db.Dispose();

            base.Dispose(disposing);
        }
    }
}
