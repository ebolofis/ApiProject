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
    public class ComboDetailController : ApiController
    {
        GenericRepository<ComboDetail> comboDetailRepo;
        GenericRepository<Product> productRepo;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ComboDetailController()
        {
            comboDetailRepo = new GenericRepository<ComboDetail>(db);
            productRepo = new GenericRepository<Product>(db);
        }

        public IEnumerable<ComboDetail> GetComboDetail()
        {
            return comboDetailRepo.GetAll();
        }
        public PagedResult<ComboDetail> GetComboDetail(int page, int pageSize)
        {
            return comboDetailRepo.GetPaged(x => true, s => "Id", page, pageSize);
        }
        public ComboDetail GetComboDetail(long id)
        {
            return comboDetailRepo.FindBy(x => x.Id == id).FirstOrDefault();
        }

        [HttpPut]
        public HttpResponseMessage PutComboDetail(IEnumerable<ComboDetail> model)
        {
            if (ModelState.IsValid)
            {
                comboDetailRepo.UpdateRange(model);
                foreach (ComboDetail cd in model)
                {
                    bool flagged = UpdateProductFromComboDetail(cd.ProductComboItemId ?? 0);
                }
                try
                {
                    if (comboDetailRepo.SaveChanges() == 0)
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
        public HttpResponseMessage PostComboDetail(ComboDetail model)
        {
            if (ModelState.IsValid)
            {
                comboDetailRepo.Add(model);
                bool flagged = UpdateProductFromComboDetail(model.ProductComboItemId ?? 0);
                try
                {
                    if (comboDetailRepo.SaveChanges() == 0)
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

        public HttpResponseMessage DeleteComboDetail(long id)
        {
            comboDetailRepo.Delete(x => x.Id == id);
            try
            {
                if (comboDetailRepo.SaveChanges() == 0)
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

        private bool UpdateProductFromComboDetail(long productId)
        {
            IEnumerable<Product> products = productRepo.FindBy(x => x.Id == productId).AsEnumerable();
            foreach (Product p in products)
            {
                p.IsComboItem = true;
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
