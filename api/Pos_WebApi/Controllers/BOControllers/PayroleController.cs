using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using Pos_WebApi.Models.FilterModels;
using Pos_WebApi.Repositories.BORepos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.BOControllers {

    public class PayroleController : ApiController {

        private PosEntities db = new PosEntities(false);
        PayroleRepository<PayroleDTO> svc;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public PayroleController() {
            svc = new PayroleRepository<PayroleDTO>(db);
        }

        /// <summary>
        /// Simple Generic GetAll promise
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PayroleDTO> GetPayroles() {
            return svc.GetAll();
        }

        public PagedResult<PayroleDTO> GetPayrole(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }

        public class PaymentPageFilter {
            public int page { get; set; }
            public int pageSize { get; set; }
            public string filters { get; set; }
        }
        /// <summary>
        /// Get paged Result of data filtered by filters string entity of PayroleFilter Entities 
        /// this creates a predicate and uses generic reposityory with predicate constructed
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        [Route("api/{storeId}/PostPayrolePagedFilter")]
        [HttpPost]
        public PagedResult<PayroleDTO> GetPayrolePagedFilter(string storeId, PaymentPageFilter ppf) {
        //public PagedResult<Payrole> GetPayrolePagedFilter(string storeId, PaymentPageFilter ppf) {
            try {
                db = new PosEntities(false, Guid.Parse(storeId));
                svc = new PayroleRepository<PayroleDTO>(db);
                var flts = JsonConvert.DeserializeObject<PayroleFilter>(ppf.filters);
                return svc.GetPaged(flts.predicate, s => "Id", ppf.page, ppf.pageSize);

            } catch (Exception ex) {
                logger.Error(ex.ToString());
                var e = ex;
                throw;
            }

        }


        /// <summary>
        /// this is a Payrole single get by id parsed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Payrole GetPayroleByid(long id) {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }
        /// <summary>
        /// Resource to get staff entiity with identification field
        /// this is used from Front-End UI to get entity of a Staff that refered to an identification provided
        /// </summary>
        /// <param name="identification"></param>
        /// <returns></returns>
        public Staff GetPayroleStaff(string identification) {
            try {
                Staff ret = db.Staff.Where(x => x.Identification == identification).FirstOrDefault();
                return ret;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// a Generic multiPUT resource 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Payrole/5
        [HttpPut]
        public HttpResponseMessage PutPayrole(IEnumerable<PayroleDTO> model) {
            if (!ModelState.IsValid) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
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
        /// <summary>
        /// Single post entity with generic controller
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Payrole
        public HttpResponseMessage PostPayrole(PayroleDTO model) {
            if (ModelState.IsValid) {
                svc.Add(model);
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
        /// <summary>
        /// Single Delete by id provided of payrole table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage DeletePayrole(long id) {
            svc.Delete(id);
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
