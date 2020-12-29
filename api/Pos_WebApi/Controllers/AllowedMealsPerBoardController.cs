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

namespace Pos_WebApi.Controllers
{
    [RoutePrefix("api/{storeId}/AllowedMealsPerBoard")]
    public class AllowedMealsPerBoardController : ApiController
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PosEntities db = new PosEntities(false);

        GenericRepository<PosInfo> svc;
        AllowedMealsPerBoardRepository repo;


        [Route("GetAll")]
        public IEnumerable<AllowedMealsPerBoardDTO> GetAll(string storeId)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new AllowedMealsPerBoardRepository(db);

            return repo.GetAll();
        }

        [Route("GetPaged")]
        public PagedResult<AllowedMealsPerBoardDTO> GetPaged(string storeId, string filters, int page, int pageSize)
        {
            //  var flts = JsonConvert.DeserializeObject<PageSetFilter>(filters);
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new AllowedMealsPerBoardRepository(db);

            return repo.GetPaged(x => true, x1 => x1.BoardId, page, pageSize);
        }

        [Route("Add")]
        [HttpPost]
        public HttpResponseMessage Insert(string storeId, AllowedMealsPerBoardDTO model)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new AllowedMealsPerBoardRepository(db);

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
                logger.Error("AllowedMealsPerBoardController Insert Error : " + ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        }

        [Route("UpdateRange")]
        [HttpPut]
        public HttpResponseMessage UpdateRange(string storeId, IEnumerable<AllowedMealsPerBoardDTO> models)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new AllowedMealsPerBoardRepository(db);

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
                    logger.Error("AllowedMealsPerBoardController UpdateRange Error : " + ex.ToString());
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

        [Route("DeleteRange")]
        [HttpDelete]
        public HttpResponseMessage DeleteRange(string storeId, IEnumerable<long> ids)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            repo = new AllowedMealsPerBoardRepository(db);

            try
            {
                var res = repo.DeleteRange(ids);
                if (repo.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.SAVEDELETEDFAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error("AllowedMealsPerBoardController DeleteRange Error : " + ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }




        //  private PosEntities db = new PosEntities(false);

        //// GET api/AllowedMealsPerBoard
        //public IEnumerable<AllowedMealsPerBoard> GetAllowedMealsPerBoard(string storeid)
        //{

        //    return db.AllowedMealsPerBoard.Include("AllowedMealsPerBoardDetails.ProductCategories").AsEnumerable();
        //}

        //public IEnumerable<AllowedMealsPerBoard> GetAllowedMealsPerBoard(string storeid, string boardId)
        //{
        //    return db.AllowedMealsPerBoard.Where(w => w.BoardId == boardId).AsEnumerable();
        //}
        //// GET api/AllowedMealsPerBoard/5
        //public AllowedMealsPerBoard GetAllowedMealsPerBoard(long id, string storeid)
        //{
        //    AllowedMealsPerBoard model = db.AllowedMealsPerBoard.Find(id);
        //    if (model == null)
        //    {
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        //    }

        //    return model;
        //}

        //// PUT api/AllowedMealsPerBoard/5
        //public HttpResponseMessage PutAllowedMealsPerBoard(long id, string storeid, AllowedMealsPerBoard model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    if (id != model.Id)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //    db.Entry(model).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        //// POST api/AllowedMealsPerBoard
        //public HttpResponseMessage PostAllowedMealsPerBoard(AllowedMealsPerBoard model, string storeid)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.AllowedMealsPerBoard.Add(model);
        //        db.SaveChanges();

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
        //        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = model.Id }));
        //        return response;
        //    }
        //    else
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //}

        //// DELETE api/AllowedMealsPerBoard/5
        //public HttpResponseMessage DeleteAllowedMealsPerBoard(long id, string storeid)
        //{
        //    AllowedMealsPerBoard model = db.AllowedMealsPerBoard.Find(id);
        //    if (model == null)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }

        //    db.AllowedMealsPerBoard.Remove(model);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, model);
        //}

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
