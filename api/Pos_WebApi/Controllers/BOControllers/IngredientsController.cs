using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Models;
using Pos_WebApi.Models.FilterModels;
using Newtonsoft.Json;
using LinqKit;
using log4net;
using System.Linq.Expressions;

namespace Pos_WebApi.Controllers {
    public class IngredientsController : ApiController {
        GenericRepository<Ingredients> svc;

        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IngredientsController() {
            svc = new GenericRepository<Ingredients>(db);
        }

        // GET api/Ingredients
        public IEnumerable<Ingredients> GetIngredients() {
            return svc.GetAll();
        }
        public PagedResult<Ingredients> GetUniqueCodeIngredients(string uniqueCode)
        {
                return svc.GetPaged(s => s.Code == uniqueCode , s => "Id" , 0 , 2000000);
        }

        public PagedResult<Ingredients> GetIngredients(int page, int pageSize) {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }
        /// <summary>
        /// Get paged filter ingredients
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="filters">Usage of Description to map entities that contains string in sales and extend Description </param>
        /// <returns></returns>
        public PagedResult<Ingredients> GetIngredients(int page, int pageSize, string filters) {
            {
                var flts = JsonConvert.DeserializeObject<Ingredients>(filters);
                //Expression<Func<Ingredients, bool>> _predicate = PredicateBuilder.True<Ingredients>();
                if (!String.IsNullOrEmpty(flts.Description))
                    return svc.GetPaged(p => p.Description.ToUpper().Contains(flts.Description.Trim().ToUpper())
                                                                         || p.SalesDescription.ToUpper().Contains(flts.Description.Trim().ToUpper())
                                                                         || p.ExtendedDescription.ToUpper().Contains(flts.Description.ToUpper()), s => "Id", page, pageSize);
                else
                    return svc.GetPaged(s => true, s => "Id", page, pageSize);
            }
        }
        // GET api/Ingredients/5
        public IEnumerable<Ingredients> GetIngredients(long id) {
            return svc.FindBy(x => x.Id == id);
        }


        // PUT api/Ingredients/5
        [HttpPut]
        public HttpResponseMessage PutIngredients(IEnumerable<Ingredients> model) {
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

        // POST api/Ingredients
        public HttpResponseMessage PostIngredients(Ingredients model) {
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


        // DELETE api/Ingredients/5
        public HttpResponseMessage DeleteIngredients(long id) {
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
