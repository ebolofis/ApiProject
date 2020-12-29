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
    public class ComboController : ApiController
    {
        GenericRepository<Combo> comboRepo;
        GenericRepository<Product> productRepo;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ComboController()
        {
            comboRepo = new GenericRepository<Combo>(db);
            productRepo = new GenericRepository<Product>(db);
        }

        public IEnumerable<Combo> GetCombo()
        {
            return comboRepo.GetAll();
        }
        public PagedResult<Combo> GetCombo(int page, int pageSize)
        {
            return comboRepo.GetPaged(x => true, s => "Id", page, pageSize);
        }
        public Combo GetCombo(long id)
        {
            return comboRepo.FindBy(x => x.Id == id).FirstOrDefault();
        }

        [HttpPut]
        public HttpResponseMessage PutCombo(IEnumerable<Combo> model)
        {
            if (ModelState.IsValid)
            {
                comboRepo.UpdateRange(model);
                foreach (Combo c in model)
                {
                    bool flagged = UpdateProductFromCombo(c.ProductComboId ?? 0);
                }
                try
                {
                    if (comboRepo.SaveChanges() == 0)
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
        public HttpResponseMessage PostCombo(Combo model)
        {
            if (ModelState.IsValid)
            {
                comboRepo.Add(model);
                bool flagged = UpdateProductFromCombo(model.ProductComboId ?? 0);
                try
                {
                    if (comboRepo.SaveChanges() == 0)
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

        public HttpResponseMessage DeleteCombo(long id)
        {
            comboRepo.Delete(x => x.Id == id);
            try
            {
                if (comboRepo.SaveChanges() == 0)
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

        private bool UpdateProductFromCombo(long productId)
        {
            IEnumerable<Product> products = productRepo.FindBy(x => x.Id == productId).AsEnumerable();
            foreach (Product p in products)
            {
                p.IsCombo = true;
            }
            productRepo.UpdateRange(products);
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
