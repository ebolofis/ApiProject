using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Models;
using log4net;

namespace Pos_WebApi.Controllers {
    public class Ingredient_ProdCategoryAssocController : ApiController {

        GenericRepository<Ingredient_ProdCategoryAssoc> svc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Ingredient_ProdCategoryAssocController() {
            svc = new GenericRepository<Ingredient_ProdCategoryAssoc>(db);
        }

        // GET api/Ingredient_ProdCategoryAssoc
        public IEnumerable<Ingredient_ProdCategoryAssoc> GetIngredient_ProdCategoryAssoc() {
            return svc.GetAllSorted(s => "Sort");
        }
        public PagedResult<Ingredient_ProdCategoryAssoc> GetIngredient_ProdCategoryAssoc(int page, int pageSize) {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }
        // GET api/Ingredient_ProdCategoryAssoc/5
        public IEnumerable<Ingredient_ProdCategoryAssoc> GetIngredient_ProdCategoryAssoc(long pcid) {
            return svc.FindBySorted(x => x.ProductCategoryId == pcid, s => "Sort");
        }


        // PUT api/Ingredient_ProdCategoryAssoc/5
        [HttpPut]
        public HttpResponseMessage PutIngredient_ProdCategoryAssoc(IEnumerable<Ingredient_ProdCategoryAssoc> model) {
            //if (!ModelState.IsValid)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            //}

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

        // POST api/Ingredient_ProdCategoryAssoc
        [HttpPost]
        public HttpResponseMessage PostIngredient_ProdCategoryAssoc(string storeid, IEnumerable<Ingredient_ProdCategoryAssoc> model) {
            if (ModelState.IsValid) {
                svc.AddRange(model);
                try {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
                } catch (DbUpdateConcurrencyException ex) {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, model);
                return response;
            } else {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ModelState);
            }
        }


        // DELETE api/Ingredient_ProdCategoryAssoc/5
        public HttpResponseMessage DeleteIngredient_ProdCategoryAssoc(long id) {
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


        public HttpResponseMessage DeleteIngredient_ProdCategoryAssoc(IEnumerable<long> ids) {
            foreach (var id in ids) {
                svc.Delete(x => x.Id == id);
            }
            try {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
            } catch (DbUpdateConcurrencyException ex) {
                logger.Info(ex.ToString());
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
