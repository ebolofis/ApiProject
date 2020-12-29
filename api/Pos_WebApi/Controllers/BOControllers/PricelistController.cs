using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Models;
using log4net;

namespace Pos_WebApi.Controllers
{
    public class PricelistController : ApiController
    {
        GenericRepository<Pricelist> svc;
        ProductPricesRepository pprepo;

        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PricelistController()
        {
            svc = new GenericRepository<Pricelist>(db);
            pprepo = new ProductPricesRepository(db);
        }

        // GET api/AuthorizedGroup
        public IEnumerable<Pricelist> GetPricelist()
        {
            return svc.GetAll();
        }

        public PagedResult<Pricelist> GetPricelist(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }

        // GET api/AuthorizedGroup/5
        public Pricelist GetPricelist(long id)
        {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }

        // PUT api/AuthorizedGroup/5
        [HttpPut]
        public HttpResponseMessage PutPricelist(IEnumerable<Pricelist> model)
        {
            try
            {
                svc.UpdateRange(model);
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
                pprepo.ReculculatePercentagePrices(model.Where(w => w.LookUpPriceListId != null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }


        // POST api/AuthorizedGroup
        public HttpResponseMessage PostPricelist(Pricelist model)
        {

            if (ModelState.IsValid)
            {

                svc.Add(model);
                try
                {

                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                    if (model.LookUpPriceListId != null)
                    {
                        var newList = new List<Pricelist>();
                        newList.Add(model);
                        pprepo.ReculculatePercentagePrices(newList);
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/AuthorizedGroup/5
        public HttpResponseMessage DeletePricelist(long id)
        {
            svc.Delete(x => x.Id == id);
            try
            {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();

            base.Dispose(disposing);
        }
    }
}
