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
    public class PosInfo_StaffPositin_AssocController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET api/PosInfo_StaffPositin_Assoc
        public IEnumerable<PosInfo_StaffPositin_Assoc> GetPosInfo_StaffPositin_Assoc()
        {
            var posinfo_staffpositin_assoc = db.PosInfo_StaffPositin_Assoc.Include(p => p.PosInfo).Include(p => p.StaffPosition);
            return posinfo_staffpositin_assoc.AsEnumerable();
        }

        // GET api/PosInfo_StaffPositin_Assoc/5
        public PosInfo_StaffPositin_Assoc GetPosInfo_StaffPositin_Assoc(long id)
        {
            PosInfo_StaffPositin_Assoc posinfo_staffpositin_assoc = db.PosInfo_StaffPositin_Assoc.Find(id);
            if (posinfo_staffpositin_assoc == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return posinfo_staffpositin_assoc;
        }

        // PUT api/PosInfo_StaffPositin_Assoc/5
        public HttpResponseMessage PutPosInfo_StaffPositin_Assoc(long id, PosInfo_StaffPositin_Assoc posinfo_staffpositin_assoc)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != posinfo_staffpositin_assoc.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(posinfo_staffpositin_assoc).State = EntityState.Modified;

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

        // POST api/PosInfo_StaffPositin_Assoc
        public HttpResponseMessage PostPosInfo_StaffPositin_Assoc(PosInfo_StaffPositin_Assoc posinfo_staffpositin_assoc)
        {
            if (ModelState.IsValid)
            {
                db.PosInfo_StaffPositin_Assoc.Add(posinfo_staffpositin_assoc);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, posinfo_staffpositin_assoc);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = posinfo_staffpositin_assoc.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/PosInfo_StaffPositin_Assoc/5
        public HttpResponseMessage DeletePosInfo_StaffPositin_Assoc(long id)
        {
            PosInfo_StaffPositin_Assoc posinfo_staffpositin_assoc = db.PosInfo_StaffPositin_Assoc.Find(id);
            if (posinfo_staffpositin_assoc == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.PosInfo_StaffPositin_Assoc.Remove(posinfo_staffpositin_assoc);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, posinfo_staffpositin_assoc);
        }

        public HttpResponseMessage DeleteIngredient_ProdCategoryAssoc(long id, bool byPosInfoid)
        {
            IEnumerable<PosInfo_StaffPositin_Assoc> posInfo_StaffPositin_Assoc = db.PosInfo_StaffPositin_Assoc.Where(w => w.PosInfoId == id).AsEnumerable();
            if (posInfo_StaffPositin_Assoc == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            foreach (var i in posInfo_StaffPositin_Assoc)
            {
                db.PosInfo_StaffPositin_Assoc.Remove(i);
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

            return Request.CreateResponse(HttpStatusCode.OK, posInfo_StaffPositin_Assoc);
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