using log4net;
using Pos_WebApi.Models;
using Pos_WebApi.Repositories.BORepos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Pos_WebApi.Controllers {
    public class AccountMappingController : ApiController {
        GenericRepository<EODAccountToPmsTransfer> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AccountMappingController() {
            svc = new GenericRepository<EODAccountToPmsTransfer>(db);

        }

        // GET api/Accounts
        public IEnumerable<EODAccountToPmsTransfer> GetAccountMapping() {
            return svc.GetAll();
        }
        public PagedResult<EODAccountToPmsTransfer> GetAccountMapping(int page, int pageSize) {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }
        //GET api/Accounts/5
        public EODAccountToPmsTransfer GetAccountMapping(long id) {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }


        // PUT api/Accounts/5
        [HttpPut]
        public HttpResponseMessage PutAccountMapping(IEnumerable<EODAccountToPmsTransfer> model) {
            if (!ModelState.IsValid) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            //if (id != model.Id)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}

            svc.UpdateRange(model);

            try {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Range/AccountMappings
        /// <summary>
        /// Account Mapping Range Add controller that used in clone entity functionality
        /// Search Dublicates on db then splits configured results to existed and to add
        /// Uses Generic Findby and AddRange to implement Procedure
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="models">Range of adding entities</param>
        /// <returns>Http MSG responce and on 200 those entities that does not exist and has been added</returns>
        [HttpPost]
        [Route("api/{storeId}/AccountMappings/AddRange")]
        public HttpResponseMessage PostAccountMappings(string storeId, IEnumerable<EODAccountToPmsTransfer> models) {
            if (ModelState.IsValid) {
                //check if there are entities with same registers
                List<EODAccountToPmsTransfer> exists = new List<EODAccountToPmsTransfer>(); List<EODAccountToPmsTransfer> toadd = new List<EODAccountToPmsTransfer>();
                db = new PosEntities(false, Guid.Parse(storeId)); svc = new GenericRepository<EODAccountToPmsTransfer>(db);

                foreach (EODAccountToPmsTransfer entry in models) {
                    List<EODAccountToPmsTransfer> rrs = svc.FindBy(x => x.PosInfoId == entry.PosInfoId && x.AccountId == entry.AccountId && x.PmsRoom == entry.PmsRoom).ToList();
                    if (rrs.Count() > 0) {
                        exists.Add(entry);
                    } else {
                        toadd.Add(entry);
                    }
                }
                if (toadd.Count() > 0) {
                    svc.AddRange(toadd);
                } else {
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.ENTRIESALREADYEXISTS);
                }
                try {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, toadd);
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
        /// <summary>
        /// Single Generic Post of Account Mapping 
        /// First searches for an entry with same posinfo Account and Room 
        /// then if there is noone exists with the same data Stores ent and return model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>HTTP responce mesg of content data on success or Error of expectation</returns>
        // POST api/Accounts
        public HttpResponseMessage PostAccountMapping(EODAccountToPmsTransfer model) {
            if (ModelState.IsValid) {
                EODAccountToPmsTransfer exited = svc.FindBy(x => x.PosInfoId == model.PosInfoId && x.AccountId == model.AccountId && x.PmsRoom == model.PmsRoom).FirstOrDefault();
                if (exited == null) {
                    svc.Add(model);
                } else {
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.ENTRIESALREADYEXISTS);
                }

                try {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Accounts/5
        public HttpResponseMessage DeleteAccountMapping(long id) {
            svc.Delete(x => x.Id == id);
            try {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
            } catch (DbUpdateConcurrencyException ex) {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [AllowAnonymous]
        public HttpResponseMessage Options() {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
