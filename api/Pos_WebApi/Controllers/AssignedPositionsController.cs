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
    public class AssignedPositionsController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET api/AssignedPositions
        public IEnumerable<AssignedPositions> GetAssignedPositions(string storeid)
        {
            return db.AssignedPositions.AsEnumerable();
        }

        // GET api/AssignedPositions/5
        public AssignedPositions GetAssignedPositions(long id, string storeid)
        {
            AssignedPositions assignedpositions = db.AssignedPositions.Find(id);
            if (assignedpositions == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return assignedpositions;
        }

        // PUT api/AssignedPositions/5
        public HttpResponseMessage PutAssignedPositions(long id, string storeid, AssignedPositions assignedpositions)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != assignedpositions.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(assignedpositions).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error("AssignedPositionsController PutAssignedPositions Error : " + ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/AssignedPositions
        public HttpResponseMessage PostAssignedPositions(AssignedPositions assignedpositions, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.AssignedPositions.Add(assignedpositions);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, assignedpositions);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = assignedpositions.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/AssignedPositions/5
        public HttpResponseMessage DeleteAssignedPositions(long id, string storeid)
        {
            AssignedPositions assignedpositions = db.AssignedPositions.Find(id);
            if (assignedpositions == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.AssignedPositions.Remove(assignedpositions);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error("AssignedPositionsController DeleteAssignedPositions Error : " + ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, assignedpositions);
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