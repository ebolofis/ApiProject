using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class PageButtonDetailController : ApiController
    {
        private PosEntities db = new PosEntities(false);

        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/Default1
        public IEnumerable<PageButtonDetail> GetPageButtonDetails()
        {
            var pagebuttondetail = db.PageButtonDetail.Include(p => p.PageButton);
            return pagebuttondetail.AsEnumerable();
        }

        //For WebPos_HQ
        public IEnumerable<Object> GetPageButtonDetails(string storeid, long productid, long pricelistid, int idx)
        {
            var buttonDetailsTemp = (from q in db.ProductRecipe.Include("Product").Include("TransferMappings").Include(i=>i.Ingredients.PricelistDetail)
                                     where q.ProductId == productid
                                     select new
                                     {
                                         ProductId = q.ProductId,
                                         IngredientId = q.IngredientId,
                                         //Background = q.ba
                                         Description = q.Ingredients.SalesDescription,
                                         Background = q.Ingredients.Background,
                                         Color = q.Ingredients.Color,
                                         MaxQty = q.MaxQty,
                                         MinQty = q.MinQty,
                                         CanSavePrice = q.CanSavePrice,
                                         Qty = q.Qty,
                                         Type = (byte?)0,
                                         Price = q.Price,
                                         Sort = q.Sort,
                                         PricelistDetails = q.Ingredients.PricelistDetail.Where(w=>w.PricelistId == pricelistid).FirstOrDefault()
                                     }).Union(from q in db.ProductExtras.Include("Product").Include("Product.PricelistDetail")
                                              where q.ProductId == productid
                                              select new
                                              {
                                                  ProductId = q.ProductId,
                                                  IngredientId = q.IngredientId,
                                                  Description = q.Ingredients.SalesDescription,
                                                  Background = q.Ingredients.Background,
                                                  Color = q.Ingredients.Color,
                                                  MaxQty = q.MaxQty,
                                                  MinQty = q.MinQty,
                                                  CanSavePrice = q.CanSavePrice,
                                                  Qty = (double?)0,
                                                  Type = (byte?)1,
                                                  Price = q.Price,
                                                  Sort = q.Sort,
                                                  PricelistDetails = q.Ingredients.PricelistDetail.Where(w => w.PricelistId == pricelistid).FirstOrDefault()
                                              });
            var pagebuttondetail = from q in buttonDetailsTemp
                                   select new
                                   {
                                       AddCost = q.PricelistDetails.Price,
                                       Description = q.Description,
                                       Background = q.Background,
                                       Color = q.Color,
                                       MaxQty = q.MaxQty,
                                       MinQty = q.MinQty,
                                       ProductId = q.ProductId,
                                       Qty = q.Qty,
                                       CanSavePrice = q.CanSavePrice,
                                       RemoveCost = q.PricelistDetails.PriceWithout,
                                       Sort = (short?)q.Sort,
                                       Type = q.Type,
                                       Idx = idx
                                   };
            return pagebuttondetail.AsEnumerable();
        }

        // GET api/Default1/5
        public PageButtonDetail GetPageButtonDetail(long id)
        {
            PageButtonDetail pagebuttondetail = db.PageButtonDetail.Find(id);
            if (pagebuttondetail == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return pagebuttondetail;
        }

        // PUT api/Default1/5
        public HttpResponseMessage PutPageButtonDetail(long id, PageButtonDetail pagebuttondetail)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != pagebuttondetail.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(pagebuttondetail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Default1
        public HttpResponseMessage PostPageButtonDetail(PageButtonDetail pagebuttondetail)
        {
            if (ModelState.IsValid)
            {
                db.PageButtonDetail.Add(pagebuttondetail);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, pagebuttondetail);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = pagebuttondetail.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Default1/5
        public HttpResponseMessage DeletePageButtonDetail(long id)
        {
            PageButtonDetail pagebuttondetail = db.PageButtonDetail.Find(id);
            if (pagebuttondetail == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.PageButtonDetail.Remove(pagebuttondetail);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, pagebuttondetail);
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