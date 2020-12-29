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
    public class PriceList_EffectiveHoursController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/PriceList_EffectiveHours
        public IEnumerable<PriceList_EffectiveHours> GetPriceList_EffectiveHours(string storeid)
        {
            var pricelist_effectivehours = db.PriceList_EffectiveHours.Include(p => p.Pricelist);
            return pricelist_effectivehours.AsEnumerable();
        }

        // GET api/PriceList_EffectiveHours/5
        public PriceList_EffectiveHours GetPriceList_EffectiveHours(long id, string storeid)
        {
            PriceList_EffectiveHours pricelist_effectivehours = db.PriceList_EffectiveHours.Find(id);
            if (pricelist_effectivehours == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return pricelist_effectivehours;
        }

        // PUT api/PriceList_EffectiveHours/5
        public HttpResponseMessage PutPriceList_EffectiveHours(long id, string storeid, PriceList_EffectiveHours pricelist_effectivehours)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != pricelist_effectivehours.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(pricelist_effectivehours).State = EntityState.Modified;

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

        // POST api/PriceList_EffectiveHours
        public HttpResponseMessage PostPriceList_EffectiveHours(PriceList_EffectiveHours pricelist_effectivehours, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.PriceList_EffectiveHours.Add(pricelist_effectivehours);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, pricelist_effectivehours);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = pricelist_effectivehours.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/PriceList_EffectiveHours/5
        public HttpResponseMessage DeletePriceList_EffectiveHours(long id, string storeid)
        {
            PriceList_EffectiveHours pricelist_effectivehours = db.PriceList_EffectiveHours.Find(id);
            if (pricelist_effectivehours == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.PriceList_EffectiveHours.Remove(pricelist_effectivehours);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, pricelist_effectivehours);
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