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
    public class InvoiceTypesController : ApiController
    {
        GenericRepository<InvoiceTypes> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public InvoiceTypesController()
        {
            svc = new GenericRepository<InvoiceTypes>(db);
        }


        public IEnumerable<InvoiceTypes> GetInvoiceTypes()
        {
            return svc.GetAll();
        }

        // GET api/InvoiceTypes
        public PagedResult<InvoiceTypes> GetInvoiceTypes(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }

        // GET api/InvoiceTypes/5
        public InvoiceTypes GetInvoiceTypes(long id)
        {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }

        [HttpPut]
        public HttpResponseMessage PutInvoiceTypes(IEnumerable<InvoiceTypes> model)
        {
          
            try
            {
                svc.UpdateRange(model);
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

        // POST api/InvoiceTypes
        public HttpResponseMessage PostInvoiceTypes(InvoiceTypes model)
        {
            //if (ModelState.IsValid)
            // {
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
            //}
            //else
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            //}
        }


        // DELETE api/InvoiceTypes/5
        public HttpResponseMessage DeleteInvoiceTypes(long id)
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
