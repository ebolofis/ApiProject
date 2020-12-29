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

namespace Pos_WebApi.Controllers.BOControllers
{
    public class RegionLockerProductController : ApiController
    {
        GenericRepository<RegionLockerProduct> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RegionLockerProductController() {
            svc = new GenericRepository<RegionLockerProduct>(db);
        }

        // GET api/RegionLockerProduct
        public IEnumerable<RegionLockerProduct> GetRegionLockerProduct() {
            return svc.GetAll();
        }
        public PagedResult<RegionLockerProduct> GetRegionLockerProduct(int page, int pageSize) {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }
        // GET api/RegionLockerProduct/5
        public RegionLockerProduct GetRegionLockerProduct(long id) {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }

        // PUT api/RegionLockerProduct/5
        [HttpPut]
        public HttpResponseMessage PutRegionLockerProduct(IEnumerable<RegionLockerProduct> model) {
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

        // POST api/RegionLockerProduct
        public HttpResponseMessage PostRegionLockerProduct(RegionLockerProduct model) {
            if (ModelState.IsValid) {
                svc.Add(model);
                try {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/RegionLockerProduct/5
        public HttpResponseMessage DeleteRegionLockerProduct(long id) {
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
