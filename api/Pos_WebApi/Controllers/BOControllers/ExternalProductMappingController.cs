using log4net;
using Pos_WebApi.Models;
using Pos_WebApi.Repositories.BORepos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.BOControllers {
    public class ExternalProductMappingController : ApiController {
        GenericRepository<ExternalProductMapping> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ExternalProductMappingController() {
            svc = new GenericRepository<ExternalProductMapping>(db);
        }

        // GET api/ExternalProductMapping
        public IEnumerable<ExternalProductMapping> GetExternalProductMapping() {
            var res =  db.ExternalProductMapping.Include("Product").AsEnumerable();
            return res;
        }

        [Route("api/{storeId}/ExternalProductPricelistDetailMapping")]
        [HttpGet]
        public ExternalProductPricelistDetailMapping GetExternalProductPricelistDetailMapping(string storeid)
        {
            db = new PosEntities(false, Guid.Parse(storeid));
            ExternalProductMapping product = db.ExternalProductMapping.Include("Product").Where(e => e.ProductEnumType == 3).FirstOrDefault();
            if (product != null)
            {
                var res = db.ExternalProductMapping.AsEnumerable();
                var res1 = db.PricelistDetail.AsEnumerable().Select(p => new PricelistDetail
                {
                    VatId = p.VatId,
                    ProductId = p.ProductId
                }).Where(pr => pr.ProductId == product.ProductId).FirstOrDefault();
                var res2 = db.OrderDetail.AsEnumerable().Select(o => new OrderDetail
                {
                    SalesTypeId = o.SalesTypeId,
                    ProductId = o.ProductId
                }).Where(pr => pr.ProductId == product.ProductId).FirstOrDefault();
                ExternalProductPricelistDetailMapping model = new ExternalProductPricelistDetailMapping();
                model.ExternalProductMapping = res.ToList();
                model.VatId = res1.VatId;
                model.SalesTypeId = res2.SalesTypeId;
                return model;
            }
            else
                return null;
        }

        public PagedResult<ExternalProductMapping> GetExternalProductMapping(int page, int pageSize) {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }
        public IEnumerable<Product> GetProductsMapped(int Extype) {
            try {
                List<Product> res = db.ExternalProductMapping.Include("Product").Where(q => q.ProductEnumType == Extype).Select( q=>q.Product ).ToList<Product>();
                if (res != null)
                    return res.AsEnumerable();
                else
                    return null;
            } catch (Exception exc) {
                logger.Error(exc.ToString());
                return null;
            }
        }
        // GET api/ExternalProductMapping/5
        public ExternalProductMapping GetExternalProductMapping(long id) {
            var res = db.ExternalProductMapping.Include("Product").Where(qq => qq.Id == id).FirstOrDefault();
            return res;
            //return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }

        // PUT api/ExternalProductMapping/5
        [HttpPut]
        public HttpResponseMessage PutExternalProductMapping(IEnumerable<ExternalProductMapping> model) {
            if (!ModelState.IsValid) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            svc.UpdateRange(model);

            try {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/ExternalProductMapping
        public HttpResponseMessage PostExternalProductMapping(ExternalProductMapping model) {
            if (ModelState.IsValid) {
                svc.Add(model);
                try {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                } catch (DbUpdateConcurrencyException ex) {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/ExternalProductMapping/5
        public HttpResponseMessage DeleteExternalProductMapping(long id) {
            svc.Delete(x => x.Id == id);
            try {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
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
