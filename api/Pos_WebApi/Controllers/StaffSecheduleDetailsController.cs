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
    public class StaffSecheduleDetailsController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/StaffSecheduleDetails
        public IEnumerable<StaffSecheduleDetails> GetStaffSecheduleDetails(string storeid)
        {
            var staffsecheduledetails = db.StaffSecheduleDetails.Include(s => s.Staff).Include(s => s.StaffSchedule);
            return staffsecheduledetails.AsEnumerable();
        }

        // GET api/StaffSecheduleDetails/5
        public StaffSecheduleDetails GetStaffSecheduleDetails(long id, string storeid)
        {
            StaffSecheduleDetails staffsecheduledetails = db.StaffSecheduleDetails.Find(id);
            if (staffsecheduledetails == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return staffsecheduledetails;
        }

        // PUT api/StaffSecheduleDetails/5
        public HttpResponseMessage PutStaffSecheduleDetails(long id, string storeid, StaffSecheduleDetails staffsecheduledetails)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != staffsecheduledetails.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(staffsecheduledetails).State = EntityState.Modified;

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

        // POST api/StaffSecheduleDetails
        public HttpResponseMessage PostStaffSecheduleDetails(StaffSecheduleDetails staffsecheduledetails, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.StaffSecheduleDetails.Add(staffsecheduledetails);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, staffsecheduledetails);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = staffsecheduledetails.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/StaffSecheduleDetails/5
        public HttpResponseMessage DeleteStaffSecheduleDetails(long id, string storeid)
        {
            StaffSecheduleDetails staffsecheduledetails = db.StaffSecheduleDetails.Find(id);
            if (staffsecheduledetails == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.StaffSecheduleDetails.Remove(staffsecheduledetails);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, staffsecheduledetails);
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