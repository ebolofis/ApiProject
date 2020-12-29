using log4net;
using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using Pos_WebApi.Repositories.BORepos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.BOControllers
{
    [RoutePrefix("api/{storeId}/ExcludedAcounts")]
    public class ExcludedAcountsController : ApiController
    {
        ExcludedAcountsRepository svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ExcludedAcountsController()
        {
            svc = new ExcludedAcountsRepository(db);
        }

        // GET api/AuthorizedGroup
        [Route("GetAll")]
        public IEnumerable<ExcludedAccountsDTO> GetExcludedPricelists(string storeId)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new ExcludedAcountsRepository(db);

            return svc.GetAll();
        }

        [Route("GetPaged")]
        public PagedResult<ExcludedAccountsDTO> GetExcludedPricelistsPaged(string storeId, int page, int pageSize)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new ExcludedAcountsRepository(db);

            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }

        [Route("Add")]
        [HttpPost]
        public HttpResponseMessage InsertExcludedPricelists(string storeId, ExcludedAccountsDTO model)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new ExcludedAcountsRepository(db);

            if (ModelState.IsValid)
            {
                svc.Add(model);
                try
                {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                    // response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                    return response;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }


        [Route("Update")]
        [HttpPut]
        public HttpResponseMessage Update(string storeId, IEnumerable<ExcludedAccountsDTO> model)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new ExcludedAcountsRepository(db);

            if (ModelState.IsValid)
            {
                svc.UpdateRange(model);

                try
                {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.UPDATEFAILED);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                //response.Headers.Location = new Uri(Url.Link("DefaultApi", modl);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();

            base.Dispose(disposing);
        }

    }
}