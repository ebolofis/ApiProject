using log4net;
using Pos_WebApi.Models;
using Pos_WebApi.Repositories.BORepos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.BOControllers
{
    public class IngredientCategoriesController : ApiController
    {
        GenericRepository<IngredientCategories> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IngredientCategoriesController()
        {
            svc = new GenericRepository<IngredientCategories>(db);
        }

        public IEnumerable<IngredientCategories> GetIngredientCategories()
        {
            return svc.GetAll();
        }
        public PagedResult<IngredientCategories> GetIngredientCategories(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }
        public IngredientCategories GetIngredientCategories(long id)
        {
            return svc.FindBy(x => x.Id == id).FirstOrDefault();
        }

        [HttpPut]
        public HttpResponseMessage PutIngredientCategories(IEnumerable<IngredientCategories> model)
        {
            if (ModelState.IsValid)
            {
                svc.UpdateRange(model);
                try
                {
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
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
        [HttpPost]
        public HttpResponseMessage PostIngredientCategories(IngredientCategories model)
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

        public HttpResponseMessage DeleteIngredientCategories(long id)
        {
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
