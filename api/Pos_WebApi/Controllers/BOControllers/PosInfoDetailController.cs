using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Models;
using log4net;

namespace Pos_WebApi.Controllers
{
    public class PosInfoDetailController : ApiController
    {
        GenericRepository<PosInfoDetail> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public PosInfoDetailController()
        {
            svc = new GenericRepository<PosInfoDetail>(db);
        }

        // GET api/AuthorizedGroup
        public IEnumerable<PosInfoDetail> GetPosInfoDetail()
        {
            return svc.GetAll();
        }

        public PagedResult<PosInfoDetail> GetPosInfoDetail(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }
        public PagedResult<PosInfoDetail> GetPosInfoDetail(int page, int pageSize, int posInfoId)
        {
            return svc.GetPaged(x => x.PosInfoId == posInfoId, s => "Id", page, pageSize);
        }
        // GET api/AuthorizedGroup/5
        public PosInfoDetail GetPosInfoDetail(long id)
        {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }
        public IEnumerable<PosInfoDetail> GetPosInfoDetail(int posInfoId) {
            return svc.FindBy(x => x.PosInfoId == posInfoId).ToList();
        }

        // PUT api/AuthorizedGroup/5
        [HttpPut]
        public HttpResponseMessage PutPosInfoDetail(IEnumerable<PosInfoDetail> model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            //}

            //if (id != model.Id)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}

            //svc.Update(model);
            try
            {
                svc.UpdateRange(model);
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        //public HttpResponseMessage PutPosInfoDetail(long id, PosInfoDetail model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    if (id != model.Id)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //    svc.Update(model);

        //    try
        //    {
        //        if (svc.SaveChanges() == 0)
        //            return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        // POST api/AuthorizedGroup
        public HttpResponseMessage PostPosInfoDetail(PosInfoDetail model)
        {

            if (ModelState.IsValid)
            {

                svc.Add(model);
                try
                {

                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/AuthorizedGroup/5
        public HttpResponseMessage DeletePosInfoDetail(long id)
        {
            //mark delete as model update is deleted in bussiness logic 

            svc.Delete(x => x.Id == id);
            try
            {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
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
