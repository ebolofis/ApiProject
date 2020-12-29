using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class ValidModulesController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/ValidModules
        public IEnumerable<ValidModules> GetValidModuless(string storeid)
        {
            return db.ValidModules.AsEnumerable();
        }

        // GET api/ValidModules/5
        public ValidModules GetValidModules(long id, string storeid)
        {
            ValidModules ValidModules = db.ValidModules.Find(id);
            if (ValidModules == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return ValidModules;
        }

        // PUT api/ValidModules/5
        public HttpResponseMessage PutValidModules(long id, string storeid, ValidModules ValidModules)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != ValidModules.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(ValidModules).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/ValidModules
        public HttpResponseMessage PostValidModules(ValidModules ValidModules, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.ValidModules.Add(ValidModules);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, ValidModules);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = ValidModules.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/ValidModules/5
        public HttpResponseMessage DeleteValidModules(long id, string storeid)
        {
            ValidModules ValidModules = db.ValidModules.Find(id);
            if (ValidModules == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.ValidModules.Remove(ValidModules);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, ValidModules);
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