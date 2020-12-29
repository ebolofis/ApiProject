using log4net;
using Pos_WebApi.Customer_Modules;
using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class StaffAuthorizationController : BaseController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/StaffAuthorization
        public IEnumerable<StaffAuthorization> GetStaffAuthorizations(string storeid)
        {
            var staffauthorization = db.StaffAuthorization.Include(s => s.AuthorizedGroup).Include(s => s.Staff);
            return staffauthorization.AsEnumerable();
        }

        // GET api/StaffAuthorization/5
        public StaffAuthorization GetStaffAuthorization(long id, string storeid)
        {
            StaffAuthorization staffauthorization = db.StaffAuthorization.Find(id);
            if (staffauthorization == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return staffauthorization;
        }

        [HttpGet]
        public bool UpdateStaffAuthorizationFromBO(long staffid, long newAuthgrId)
        {
            StaffAuthorization staffauthorization = db.StaffAuthorization.FirstOrDefault(f => f.StaffId == staffid);
            if (staffauthorization == null)
            {
                StaffAuthorization newauth = new StaffAuthorization();
                newauth.AuthorizedGroupId = newAuthgrId;
                newauth.StaffId = staffid;
                db.StaffAuthorization.Add(newauth);
                db.SaveChanges();
                return true;
            }
            else
            {
                if (staffauthorization.AuthorizedGroupId != newAuthgrId)
                {
                    staffauthorization.AuthorizedGroupId = newAuthgrId;
                    db.SaveChanges();
                }
                return true;
            }

        }


        /// <summary>
        /// CheckStaffAuthorization with Identification
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="stIdentification"></param>
        /// <returns></returns>
        // POST api/<controller>
        [Route("api/CheckStaffAuthorization/storeid/{storeid}")]
        [HttpPost]
        public Staff CheckStaffAuthorization(string storeid, StaffIdentification stIdentification)
        {
            db = new PosEntities(false, Guid.Parse(storeid));
            Staff staff = null;
            ICustomerClass instanceOfMyType = LoadCustomerClass();
            if (instanceOfMyType!=null)
                if (instanceOfMyType.CheckStaffAuthorization(stIdentification, out staff, this.ActionContext, db))
                    return staff;
            return null;
        }

        [HttpGet]
        public Staff CheckStaffAuthorization(string storeid, string username, string password)
        {
            Staff staff = null;
            ICustomerClass instanceOfMyType = LoadCustomerClass();
            if (instanceOfMyType != null)
                if (instanceOfMyType.CheckStaffAuthorization(ref username, ref password, out staff, this.ActionContext, db))
                    return staff;
            return null;
        }

        // PUT api/StaffAuthorization/5
        public HttpResponseMessage PutStaffAuthorization(long id, string storeid, StaffAuthorization staffauthorization)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != staffauthorization.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(staffauthorization).State = EntityState.Modified;

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



        // POST api/StaffAuthorization
        public HttpResponseMessage PostStaffAuthorization(StaffAuthorization staffauthorization, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.StaffAuthorization.Add(staffauthorization);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, staffauthorization);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = staffauthorization.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/StaffAuthorization/5
        public HttpResponseMessage DeleteStaffAuthorization(long id, string storeid)
        {
            StaffAuthorization staffauthorization = db.StaffAuthorization.Find(id);
            if (staffauthorization == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.StaffAuthorization.Remove(staffauthorization);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, staffauthorization);
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