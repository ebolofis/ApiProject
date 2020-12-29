using log4net;
using Newtonsoft.Json;
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
    public class PricelistDetailController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/PricelistDetail
        public IEnumerable<PricelistDetail> GetPricelistDetails(string storeid)
        {
            var pricelistdetail = db.PricelistDetail.Include(p => p.Pricelist).Include(p => p.Product).Include(i=>i.Ingredients);
            return db.PricelistDetail.AsEnumerable();
        }

        public Object GetPricelistDetails(string storeid, long? prod_cat, bool pricepanel, int page, int rows, string cd_mask)
        {
            //var products = db.Product.Include(i => i.PricelistDetail).Include("PricelistDetail.Vat").AsNoTracking().AsEnumerable();
            var products = db.Product.Include(i => i.PricelistDetail).Include("PricelistDetail.Vat").AsNoTracking();
            if (!String.IsNullOrWhiteSpace(cd_mask))
            {
//                products = products.Where(w => w.Description.IndexOf(cd_mask, StringComparison.OrdinalIgnoreCase) >= 0).AsEnumerable();
                products = products.Where(w => w.Description.ToLower().Contains(cd_mask.ToLower()));//.IndexOf(cd_mask, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (prod_cat != null && prod_cat != -1)
            {
                products = products.Where(w => w.ProductCategoryId == prod_cat);//.AsEnumerable();
            }
            int count = products.Count();
            int totalpages = 0;
            if (count > 0)
            {
                double pages = (double)count / rows;
                totalpages = Convert.ToInt16(Math.Ceiling(pages));
            }
            if (page > totalpages) page = totalpages;
            int start = count > 0 ?rows * page - rows:0;
            products = products.OrderBy(o=>o.Code).Skip(start).Take(rows);

            var prmasters = db.PricelistMaster.Include(i => i.Pricelist).AsNoTracking().AsEnumerable();

            var prdetailspemaster = db.PricelistMaster.Include(i => i.Pricelist).Include("Pricelist.PricelistDetail").Select(s => new
            {
                Pricelists = s.Pricelist.Select(ss => new { PricelistDetailCount = ss.PricelistDetail.Count}),
                Master = s
            });
            var pricelists = db.Pricelist.OrderBy(o => o.PricelistMasterId).AsNoTracking().AsEnumerable();
            var vats = db.Vat.AsEnumerable();
            return new { prmasters,pricelists, products, totalpages, page, vats };
        }

        // GET api/PricelistDetail/5
        public PricelistDetail GetPricelistDetail(long id, string storeid)
        {
            PricelistDetail pricelistdetail = db.PricelistDetail.Find(id);
            if (pricelistdetail == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return pricelistdetail;
        }

        // PUT api/PricelistDetail/5
        public HttpResponseMessage PutPricelistDetail(long id, string storeid, PricelistDetail pricelistdetail)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != pricelistdetail.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(pricelistdetail).State = EntityState.Modified;

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

        // POST api/PricelistDetail
        public HttpResponseMessage PostPricelistDetail(PricelistDetail pricelistdetail, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.PricelistDetail.Add(pricelistdetail);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, pricelistdetail);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = pricelistdetail.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/PricelistDetail/5
        public HttpResponseMessage DeletePricelistDetail(long id, string storeid)
        {
            PricelistDetail pricelistdetail = db.PricelistDetail.Find(id);
            if (pricelistdetail == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.PricelistDetail.Remove(pricelistdetail);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, pricelistdetail);
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