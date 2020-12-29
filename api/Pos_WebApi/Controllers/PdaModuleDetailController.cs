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
    public class PdaModuleDetailController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/PdaModuleDetail
        public IEnumerable<PdaModuleDetail> GetPdaModuleDetail(string storeid)
        {
            return db.PdaModuleDetail.AsEnumerable();
        }

        // GET api/PdaModuleDetail
        public IEnumerable<PdaModuleDetail> GetPdaModuleDetail(string storeid, long posid, bool notloggedoff)
        {
            return db.PdaModuleDetail.Include("PdaModule").Where(w => w.LastLogoutTS == null && w.PdaModule.PosInfo.Id == posid).AsNoTracking().AsEnumerable();
        }

        // GET api/PdaModuleDetail/5
        public PdaModuleDetail GetPdaModuleDetail(long id, string storeid)
        {
            PdaModuleDetail PdaModuleDetail = db.PdaModuleDetail.Find(id);
            if (PdaModuleDetail == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return PdaModuleDetail;
        }

        // PUT api/PdaModuleDetail/5
        public HttpResponseMessage PutPdaModuleDetail(long id, string storeid, PdaModuleDetail PdaModuleDetail)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != PdaModuleDetail.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(PdaModuleDetail).State = EntityState.Modified;

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

        // POST api/PdaModuleDetail
        public HttpResponseMessage PostPdaModuleDetail(PdaModuleDetail PdaModuleDetail, string storeid)
        {
            if (ModelState.IsValid)
            {
                var isLoggedOnAllou = db.PdaModuleDetail.Where(w=>w.StaffId == PdaModuleDetail.StaffId && w.PdaModuleId != PdaModuleDetail.PdaModuleId && w.LastLogoutTS == null);
                foreach (var i in isLoggedOnAllou)
                {
                    i.LastLogoutTS = DateTime.Now;
                }
                db.PdaModuleDetail.Add(PdaModuleDetail);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, PdaModuleDetail);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = PdaModuleDetail.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/PdaModuleDetail/5
        public HttpResponseMessage DeletePdaModuleDetail(long id, string storeid)
        {
            PdaModuleDetail PdaModuleDetail = db.PdaModuleDetail.Find(id);
            if (PdaModuleDetail == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.PdaModuleDetail.Remove(PdaModuleDetail);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, PdaModuleDetail);
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