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
    public class StoreMessagesController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/StoreMessages
        public IEnumerable<StoreMessages> GetStoreMessages(string storeid)
        {
            return db.StoreMessages.AsEnumerable();
        }

        /// <summary>
        ///  Επιστρέφει τα μηνύματα που εμφανίζονται στην κύρια σελίδα
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="filtered">not used</param>
        /// <returns></returns>
        public IEnumerable<StoreMessages> GetStoreMessages(string storeid, bool filtered)
        {
            var query = db.StoreMessages.Where(w => w.Status == 1 && (w.ShowFrom != null ? w.ShowFrom <= DateTime.Now : true) && (w.ShowTo != null ? w.ShowTo >= DateTime.Now : true));
            return query.AsEnumerable();
        }

        // GET api/StoreMessages/5
        public StoreMessages GetStoreMessages(long id, string storeid)
        {
            StoreMessages StoreMessages = db.StoreMessages.Find(id);
            if (StoreMessages == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return StoreMessages;
        }

        // PUT api/StoreMessages/5
        public HttpResponseMessage PutStoreMessages(long id, string storeid, StoreMessages StoreMessages)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != StoreMessages.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(StoreMessages).State = EntityState.Modified;

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

        // POST api/StoreMessages
        public HttpResponseMessage PostStoreMessages(StoreMessages StoreMessages, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.StoreMessages.Add(StoreMessages);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, StoreMessages);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = StoreMessages.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/StoreMessages/5
        public HttpResponseMessage DeleteStoreMessages(long id, string storeid)
        {
            StoreMessages StoreMessages = db.StoreMessages.Find(id);
            if (StoreMessages == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.StoreMessages.Remove(StoreMessages);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, StoreMessages);
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