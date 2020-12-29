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
    public class StaffScheduleController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // GET api/StaffSchedule
        public IEnumerable<StaffSchedule> GetStaffSchedules(string storeid)
        {
            var staffschedule = db.StaffSchedule.Include(s => s.Department);
            return staffschedule.AsEnumerable();
        }

        // GET api/StaffSchedule/5
        public StaffSchedule GetStaffSchedule(long id, string storeid)
        {
            StaffSchedule staffschedule = db.StaffSchedule.Find(id);
            if (staffschedule == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return staffschedule;
        }

        // PUT api/StaffSchedule/5
        public HttpResponseMessage PutStaffSchedule(long id, string storeid, StaffSchedule staffschedule)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != staffschedule.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(staffschedule).State = EntityState.Modified;

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

        // POST api/StaffSchedule
        public HttpResponseMessage PostStaffSchedule(StaffSchedule staffschedule, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.StaffSchedule.Add(staffschedule);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, staffschedule);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = staffschedule.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/StaffSchedule/5
        public HttpResponseMessage DeleteStaffSchedule(long id, string storeid)
        {
            StaffSchedule staffschedule = db.StaffSchedule.Find(id);
            if (staffschedule == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.StaffSchedule.Remove(staffschedule);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, staffschedule);
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