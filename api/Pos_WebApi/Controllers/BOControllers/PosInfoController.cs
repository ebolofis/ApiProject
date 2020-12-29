using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using log4net;

namespace Pos_WebApi.Controllers.BOControllers
{
    public class PosInfoController : ApiController
    {
        GenericRepository<PosInfo> svc;
        PosInfoRepository repo;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private PosEntities db = new PosEntities(false);
        public PosInfoController()
        {
            svc = new GenericRepository<PosInfo>(db);
            repo = new PosInfoRepository(db);

        }


        [Route("api/{storeId}/PosInfo/GetAll")]
        public IEnumerable<PosInfoDTO> GetAll(string storeId)
        {

            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new PosInfoRepository(db);
            PatchManager.Instance.RunPatches(db);
            return repo.GetAll();
        }

        [Route("api/{storeId}/PosInfo/GetPaged")]
        public PagedResult<PosInfoDTO> GetPaged(string storeId, string filters, int page, int pageSize)
        {
            //  var flts = JsonConvert.DeserializeObject<PageSetFilter>(filters);
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new PosInfoRepository(db);

            return repo.GetPaged(x => true, x1 => x1.Description, page, pageSize);
        }

        [Route("api/{storeId}/PosInfo/Add")]
        [HttpPost]
        public HttpResponseMessage Insert(string storeId, PosInfoDTO model)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new PosInfoRepository(db);

            repo.Add(model);
            try
            {
                if (repo.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                return response;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }

      //  [Route("api/{storeId}/PosInfo/UpdateRange")]
        [HttpPut]
        public HttpResponseMessage UpdateRange(string storeId, IEnumerable<PosInfoDTO> models)
        {
         //   db = new PosEntities(false, Guid.Parse(storeId));
         //   repo = new PosInfoRepository(db);

            if (ModelState.IsValid)
            {
                repo.UpdateRange(models);
                try
                {
                    if (repo.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.UPDATEFAILED);

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, models);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("api/{storeId}/PosInfo/DeleteRange")]
        [HttpDelete]
        public HttpResponseMessage DeleteRange(string storeId, IEnumerable<long> ids)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new PosInfoRepository(db);

            try
            {
                var res = repo.DeleteRange(ids);
                if (repo.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.SAVEDELETEDFAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // GET api/AuthorizedGroup
        public IEnumerable<PosInfo> GetPosInfo()
        {
            return svc.GetAll();
        }

        public PagedResult<PosInfo> GetPosInfo(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }

        // GET api/AuthorizedGroup/5
        public PosInfo GetPosInfo(long id)
        {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }
        [HttpPut]
        public HttpResponseMessage PutPosInfoDetail(IEnumerable<PosInfo> model)
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
        // PUT api/AuthorizedGroup/5
        //public HttpResponseMessage PutPosInfo(long id, PosInfo model)
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
        public HttpResponseMessage PostPosInfo(PosInfo model)
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
        public HttpResponseMessage DeletePosInfo(long id)
        {
            //mark delete as model update is deleted in bussiness logic 
            //svc.Delete(x => x.Id == id);
            //try
            //{
            //    if (svc.SaveChanges() == 0)
            //        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
            //}
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            //
            //}

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
