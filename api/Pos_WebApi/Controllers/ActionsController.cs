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
    public class ActionsController : ApiController
    {
        private PosEntities db = new PosEntities(false);

        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/Actions
        public IEnumerable<Actions> GetActions(string storeid)
        {
            return db.Actions.AsEnumerable();
        }

        // GET api/Actions/5
        public Actions GetActions(long id, string storeid)
        {
           
            Actions actions = db.Actions.Find(id);
            if (actions == null)
            {
                logger.Info("actions == null");
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return actions;
        }

        // PUT api/Actions/5
        public HttpResponseMessage PutActions(long id, string storeid, Actions actions)
        {
            
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != actions.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(actions).State = EntityState.Modified;

            try
            {
                logger.Info("PutActions>Saving actions...");
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error("PutActions> "+ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Actions
        public HttpResponseMessage PostActions(Actions actions, string storeid)
        {
            logger.Info("PostActions>");
            if (ModelState.IsValid)
            {
                db.Actions.Add(actions);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, actions);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = actions.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Actions/5
        public HttpResponseMessage DeleteActions(long id, string storeid)
        {
            
            Actions actions = db.Actions.Find(id);
           
            if (actions == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Actions.Remove(actions);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error("DeleteActions>:"+ ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, actions);
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