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
    [RoutePrefix("api/{storeId}/ExcludedPricelists")]
    public class ExcludedPricelistsController : ApiController
    {

        ExludedPricelistsRepository svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ExcludedPricelistsController()
        {
            svc = new ExludedPricelistsRepository(db);
        }

        // GET api/AuthorizedGroup
        [Route("GetAll")]
        public IEnumerable<ExcludedPricelistsDTO> GetExcludedPricelists(string storeId)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new ExludedPricelistsRepository(db);

            return svc.GetAll();
        }

        [Route("GetPaged")]
        public PagedResult<ExcludedPricelistsDTO> GetExcludedPricelistsPaged(string storeId, int page, int pageSize)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new ExludedPricelistsRepository(db);

            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }

        [Route("Add")]
        [HttpPost]
        public HttpResponseMessage InsertExcludedPricelists(string storeId, ExcludedPricelistsDTO model)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new ExludedPricelistsRepository(db);

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
        public HttpResponseMessage Update(string storeId, IEnumerable<ExcludedPricelistsDTO> model)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new ExludedPricelistsRepository(db);

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
        //// GET api/AuthorizedGroup/5
        //public PosInfoDetail_Pricelist_Assoc GetPosInfoDetail_Pricelist_Assoc(long id)
        //{
        //    return svc.FindBy(x => x.Id == id).FirstOrDefault();
        //}

        //    // PUT api/AuthorizedGroup/5
        //    [HttpPut]
        //    public HttpResponseMessage PutPosInfoDetail_Pricelist_Assoc(IEnumerable<PosInfoDetail_Pricelist_Assoc> model)
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //        }

        //        //if (id != model.Id)
        //        //{
        //        //    return Request.CreateResponse(HttpStatusCode.BadRequest);
        //        //}

        //        svc.UpdateRange(model);

        //        try
        //        {
        //            if (svc.SaveChanges() == 0)
        //                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
        //        }
        //        catch (DbUpdateConcurrencyException ex)
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }

        //    // POST api/AuthorizedGroup
        //    public HttpResponseMessage PostPosInfoDetail_Pricelist_Assoc(PosInfoDetail_Pricelist_Assoc model)
        //    {

        //        if (ModelState.IsValid)
        //        {

        //            svc.Add(model);
        //            try
        //            {

        //                if (svc.SaveChanges() == 0)
        //                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
        //            }
        //            catch (DbUpdateConcurrencyException ex)
        //            {
        //                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //            }

        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
        //            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
        //            return response;
        //        }
        //        else
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //        }
        //    }

        //    // DELETE api/AuthorizedGroup/5
        //    public HttpResponseMessage DeletePosInfoDetail_Pricelist_Assoc(long id)
        //    {
        //        svc.Delete(x => x.Id == id);
        //        try
        //        {
        //            if (svc.SaveChanges() == 0)
        //                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
        //        }
        //        catch (DbUpdateConcurrencyException ex)
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }

        //    [AllowAnonymous]
        //    public HttpResponseMessage Options()
        //    {
        //        return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        //    }

        //    protected override void Dispose(bool disposing)
        //    {
        //        base.Dispose(disposing);
        //    }
    }
}
