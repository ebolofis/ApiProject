using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class PosInfo_KitchenIstruction_AssocController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/Accounts
        public IEnumerable<PosInfo_KitchenInstruction_Assoc> GetPosInfo_KitchenInstruction_Assoc(string storeid)
        {
            return db.PosInfo_KitchenInstruction_Assoc.Include("PosInfo").Include("KitchenInstruction").AsNoTracking().AsEnumerable();
        }

        // GET api/Accounts/5
        public PosInfo_KitchenInstruction_Assoc GetPosInfo_KitchenInstruction_Assoc(long id, string storeid)
        {
            PosInfo_KitchenInstruction_Assoc posInfo_KitchenInstruction_Assoc = db.PosInfo_KitchenInstruction_Assoc.Find(id);
            if (posInfo_KitchenInstruction_Assoc == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return posInfo_KitchenInstruction_Assoc;
        }

        // PUT api/Accounts/5
        public HttpResponseMessage PutPosInfo_KitchenInstruction_Assoc(long id, string storeid, PosInfo_KitchenInstruction_Assoc posInfo_KitchenInstruction_Assoc)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != posInfo_KitchenInstruction_Assoc.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(posInfo_KitchenInstruction_Assoc).State = EntityState.Modified;

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

        // POST api/Accounts
        public HttpResponseMessage PostPosInfo_KitchenInstruction_Assoc(PosInfo_KitchenInstruction_Assoc posInfo_KitchenInstruction_Assoc, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.PosInfo_KitchenInstruction_Assoc.Add(posInfo_KitchenInstruction_Assoc);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, posInfo_KitchenInstruction_Assoc);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = posInfo_KitchenInstruction_Assoc.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Accounts/5
        public HttpResponseMessage DeletePosInfo_KitchenInstruction_Assoc(long id, string storeid)
        {
            PosInfo_KitchenInstruction_Assoc posInfo_KitchenInstruction_Assoc = db.PosInfo_KitchenInstruction_Assoc.Find(id);
            if (posInfo_KitchenInstruction_Assoc == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.PosInfo_KitchenInstruction_Assoc.Remove(posInfo_KitchenInstruction_Assoc);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, posInfo_KitchenInstruction_Assoc);
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
