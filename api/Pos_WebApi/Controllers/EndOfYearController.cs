using log4net;
using Pos_WebApi.Helpers;
using Pos_WebApi.Repositories.BORepos;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers {
    [Authorize]
    public class EndOfYearController : ApiController {

        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        GenericRepository<EndOfYear> svc;

        public EndOfYearController() {
            svc = new GenericRepository<EndOfYear>(db);
        }

        /// <summary>
        /// First Opperation of moving data to histry tables procedure
        /// This is a resource that providing a year and a job name 
        /// Creates a job on your sql agent that is stored to: 
        /// 1) move all data of transactions to historic tables ,2) delete current registers ,3)reindex your database
        /// </summary>
        /// <param name="storeid">By this on call provided xml of datapases initiallize your database name to job created</param>
        /// <param name="year">This is a value required to store all data ,that HAVE BEEN marked as closed by FOday.Date belonging to providing year, into historic tables</param>
        /// <param name="jobname">This is not a required field but it used to charaterize the closing register|| if not provided auto ClosingYear(PDATE) will be applied </param>
        /// <returns> Responce of sql statement when apply job to agent as HTTP response msg  </returns>
        [HttpPut]
        [Route("api/{storeid}/Endofyear/Movetohistory/{year}/{jobname}")]
        public HttpResponseMessage MoveToHistory(string storeid, string year, string jobname) {
            //Make Correction  checkes about policy 
            if (String.IsNullOrEmpty(year)) {
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, Symposium.Resources.Errors.NOCLOSINGDATE);
                //if last year MethodNotAllowed
            }

            //Create transaction sql procedural call with its dependencies according to Database scripts installation 
            string job = "", dbname = "", owner = "", history_sqlProc = "";

            try {
                string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\UsersToDatabases.xml";
                Installations installations = XmlHelpers.ParseXmlDoc(xml);
                Symposium.Models.Models.DBInfoModel sinstance = installations.Stores.Where(w => w.Id == new Guid(storeid)).FirstOrDefault();
                dbname = sinstance.DataBase;
                owner = sinstance.DataBaseUsername;

                job = (String.IsNullOrEmpty(jobname)) ? "ClosingYear" + year : jobname;
                history_sqlProc = "Exec dbo.sp_Execute_CloseOfYear 'Exec dbo.MoveDataToHistoric " + year + "','" + job + "','" + dbname + "','" + owner + "'";
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, ex);
            }

            //Executing Procedure view sql job creation
            //Responce here is the response from the agent that the job has been assigned
            try {
                db = new PosEntities(false, Guid.Parse(storeid));
                var procRet = db.Database.ExecuteSqlCommand(history_sqlProc);
                db.SaveChanges();
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, procRet);
                return response;

            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, ex);
            }
        }
        // GET api/EndOfYear/5
        /// <summary>
        /// This is a resource to Provide user next available Year value to Close year transactions
        /// Checkes last register and past year and returns an obj = { valid , lastyear } 
        /// </summary>
        /// <param name="now">Dummy get var to provide unique resource on server </param>
        /// <returns> { valid , lastyear } </returns>
        public HttpResponseMessage GetValidEndOfYear(bool now) {
            try {
                EndOfYear lastReg = db.EndOfYear.OrderByDescending(q => q.ClosedYear).FirstOrDefault();
                if (lastReg == null) {
                    Object emptyret = new { valid = true, newClosingDate = DateTime.Now.Year - 1, };
                    return Request.CreateResponse(HttpStatusCode.OK, emptyret);
                }
                //LAST CLOSE was before current year and current date is bigger than 10 JAN
                Object ret =
                    new {
                        valid = (
                        lastReg.ClosedYear < DateTime.Now.Year - 1 &&
                        DateTime.Now.Month >= 1 &&
                        DateTime.Now.Day >= 10
                        ),
                        newClosingDate = DateTime.Now.Year - 1,
                    };
                return Request.CreateResponse(HttpStatusCode.OK, ret);
            } catch (Exception) {
                Object erroret = new { valid = false, newClosingDate = -1 };
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, erroret);
            }
        }

        /// <summary>
        /// Resource to load all entities of EndOfYears
        /// </summary>
        /// <returns>IEnumerable<EndOfYear> set of entities that have been registeres </returns>
        public IEnumerable<EndOfYear> GetEndOfYears() { return svc.GetAll(); }
        //public PagedResult<EndOfYear> GetEndOfYear(int page, int pageSize) { return svc.GetPaged(x => true, s => "Id", page, pageSize); }

        /// <summary>
        /// Generic service with model of EndOfYear to update self entity
        /// </summary>
        /// <param name="model">Object modified</param>
        /// <returns></returns>
        public HttpResponseMessage PutEndOfYear(EndOfYear model) {
            if (!ModelState.IsValid) {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ModelState);
            }
            svc.Update(model);

            try {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/EndOfYear/5
        /// <summary>
        /// Generic functionality function to delete EndOfYear register on DB
        /// </summary>
        /// <param name="id">Registers id to delete</param>
        /// <returns>Http 200 or error on </returns>
        public HttpResponseMessage DeleteEndOfYear(long id) {
            svc.Delete(x => x.Id == id);
            try {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [AllowAnonymous]
        public HttpResponseMessage Options() { return new HttpResponseMessage { StatusCode = HttpStatusCode.OK }; }
        protected override void Dispose(bool disposing) { db.Dispose(); base.Dispose(disposing); }
    }

}