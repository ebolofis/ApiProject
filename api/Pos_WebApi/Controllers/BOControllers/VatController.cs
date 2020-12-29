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
    public class VatController : ApiController
    {
        GenericRepository<Vat> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public VatController()
        {
            svc = new GenericRepository<Vat>(db);
        }

        // GET api/Vat
        public IEnumerable<Vat> GetVat()
        {
            return svc.GetAll();
        }

        // GET api/Vat
        public PagedResult<Vat> GetVat(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }


        // GET api/Vat/5
        public Vat GetVat(long id)
        {
            Vat vat = svc.FindBy(x => x.Id == id).FirstOrDefault();
            if (vat == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return vat;
        }

        // PUT api/Vat/5
        [HttpPut]
        public HttpResponseMessage PutVat(IEnumerable<Vat> vat)
        {
            //if (!ModelState.IsValid)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            //}

            //if (id != vat.Id)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}
            svc.UpdateRange(vat);

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

        // POST api/Vat
        public HttpResponseMessage PostVat(Vat vat)
        {
            if (ModelState.IsValid)
            {
                svc.Add(vat);
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

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, vat);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = vat.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Vat/5
        public HttpResponseMessage DeleteVat(long id)
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
