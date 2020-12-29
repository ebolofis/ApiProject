using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Pos_WebApi.Models;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Repositories.BORepos;
using log4net;

namespace Pos_WebApi.Controllers
{
    public class DiscountController : ApiController
    {
        GenericRepository<Discount> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DiscountController()
        {
            svc = new GenericRepository<Discount>(db);
        }

        // GET api/Discount
        public IEnumerable<Discount> GetDiscount()
        {
            return svc.GetAll();
        }
        public PagedResult<Discount> GetDiscount(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }
        // GET api/Discount/5
        public Discount GetDiscount(long id)
        {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }


        // PUT api/Discount/5
        [HttpPut]
        public HttpResponseMessage PutDiscount(IEnumerable<Discount> model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            //if (id != model.Id)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}

            svc.UpdateRange(model);

            try
            {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Discount
        public HttpResponseMessage PostDiscount(Discount model)
        {
            if (ModelState.IsValid)
            {
                svc.Add(model);
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

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
        public HttpResponseMessage DeleteDiscount(long id)
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
