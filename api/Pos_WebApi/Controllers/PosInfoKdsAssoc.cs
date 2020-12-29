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
    public class PosInfoKdsAssocController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/PosInfoKdsAssoc
        public IEnumerable<PosInfoKdsAssoc> GetPosInfoKdsAssoc()
        {
            var PosInfoKdsAssoc = db.PosInfoKdsAssoc.Include(p => p.PosInfo).Include(p => p.Kds);
            return PosInfoKdsAssoc.AsEnumerable();
        }

        // GET api/PosInfoKdsAssoc/5
        public PosInfoKdsAssoc GetPosInfoKdsAssoc(long id)
        {
            PosInfoKdsAssoc PosInfoKdsAssoc = db.PosInfoKdsAssoc.Find(id);
            if (PosInfoKdsAssoc == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return PosInfoKdsAssoc;
        }

        // PUT api/PosInfoKdsAssoc/5
        public HttpResponseMessage PutPosInfoKdsAssoc(long id, PosInfoKdsAssoc PosInfoKdsAssoc)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != PosInfoKdsAssoc.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(PosInfoKdsAssoc).State = EntityState.Modified;

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

        // POST api/PosInfoKdsAssoc
        public HttpResponseMessage PostPosInfoKdsAssoc(PosInfoKdsAssoc PosInfoKdsAssoc)
        {
            if (ModelState.IsValid)
            {
                db.PosInfoKdsAssoc.Add(PosInfoKdsAssoc);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, PosInfoKdsAssoc);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = PosInfoKdsAssoc.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/PosInfoKdsAssoc/5
        public HttpResponseMessage DeletePosInfoKdsAssoc(long id)
        {
            PosInfoKdsAssoc PosInfoKdsAssoc = db.PosInfoKdsAssoc.Find(id);
            if (PosInfoKdsAssoc == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.PosInfoKdsAssoc.Remove(PosInfoKdsAssoc);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, PosInfoKdsAssoc);
        }

        public HttpResponseMessage DeletePosInfoKdsAssoc(long id, bool byPosInfoid)
        {
            IEnumerable<PosInfoKdsAssoc> PosInfoKdsAssoc = db.PosInfoKdsAssoc.Where(w => w.PosInfoId == id).AsEnumerable();
            if (PosInfoKdsAssoc == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            foreach (var i in PosInfoKdsAssoc)
            {
                db.PosInfoKdsAssoc.Remove(i);
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, PosInfoKdsAssoc);
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