using log4net;
using Pos_WebApi.Helpers;
using Symposium.Helpers;
using Symposium.Models.Models.DeliveryAgent;
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
    public class WorkSheetController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/WorkSheet
        public IEnumerable<WorkSheet> GetWorkSheets(string storeid)
        {
            var worksheet = db.WorkSheet.Include(w => w.Department).Include(w => w.Staff);
            return worksheet.AsEnumerable();
        }

        // GET api/WorkSheet/5
        public WorkSheet GetWorkSheet(long id, string storeid)
        {
            WorkSheet worksheet = db.WorkSheet.Find(id);
            if (worksheet == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return worksheet;
        }

        // PUT api/WorkSheet/5
        public HttpResponseMessage PutWorkSheet(long id, string storeid, WorkSheet worksheet)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != worksheet.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(worksheet).State = EntityState.Modified;

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

        /// <summary>
        /// Εναρξη/Λήξη Βάρδιας. 
        /// <para>Εναρξη : worksheet.Type=1</para>
        /// <para>Λήξη : worksheet.Type=2</para>
        /// <para>Insert worksheet model as new record into table WorkSheet</para>
        /// <para>POST api/WorkSheet</para>
        /// </summary>
        /// <param name="worksheet">WorkSheet model</param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public HttpResponseMessage PostWorkSheet(WorkSheet worksheet, string storeid)
        {
            if (ModelState.IsValid)
            {
                worksheet.Day = DateTime.Now;
                worksheet.Hour = DateTime.Now;

            var matchedWorksheet = db.WorkSheet.Where(x => x.StaffId == worksheet.StaffId).Select(y => y.Type).ToList();

            if(matchedWorksheet.Count() ==0)
                {
                    db.WorkSheet.Add(worksheet);
                    db.SaveChanges();

                    HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.Created, worksheet);
                    resp.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = worksheet.Id }));
                    return resp;

                }
                var LastWorksheetType = matchedWorksheet[matchedWorksheet.Count - 1];
               
                if (LastWorksheetType == worksheet.Type)
                    {
                    if (LastWorksheetType ==1)
                    {
                        throw new BusinessException(Symposium.Resources.Messages.INVLADIDCLOCKIN);
                    }
                    else if(LastWorksheetType==2)
                        {
                        throw new BusinessException(Symposium.Resources.Messages.INVLADIDCLOCKOUT);
                    }
                    }

                db.WorkSheet.Add(worksheet);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, worksheet);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = worksheet.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/WorkSheet/5
        public HttpResponseMessage DeleteWorkSheet(long id, string storeid)
        {
            WorkSheet worksheet = db.WorkSheet.Find(id);
            if (worksheet == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.WorkSheet.Remove(worksheet);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, worksheet);
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