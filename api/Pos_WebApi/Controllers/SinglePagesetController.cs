using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    [AllowAnonymous]
    public class SinglePagesetController : ApiController
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //  private PosEntities db = new PosEntities(false);
        // GET api/<controller>
        public IEnumerable<Object> GetSinglePageSet(string storeid, long posid)
        {
            PosEntities db = new PosEntities(false, Guid.Parse(storeid));
            try
            {
                var pagesetid = db.PagePosAssoc.Where(w => w.PosInfoId == posid).FirstOrDefault().PageSetId;
                //
                var query = from q in db.PageButton
                            join qq in db.Pages on q.PageId equals qq.Id
                            join qqq in db.PageSet.Where(w => w.Id == pagesetid) on qq.PageSetId equals qqq.Id
                            join p in db.Product on q.ProductId equals p.Id
                            select new
                            {
                                ProductId = q.ProductId,
                                ProductCode = p.Code,
                                Description = p.Description,
                                PricelistId = q.PriceListId == null ? qq.DefaultPriceList : q.PriceListId

                            };

                var final = from q in query
                            join qq in db.PricelistDetail on new { PricelistId = q.PricelistId, ProductId = q.ProductId }
                                                    equals new { PricelistId = qq.PricelistId, ProductId = qq.ProductId }
                            select new
                            {
                                //					ProductId = q.ProductId,
                                ProductCode = q.ProductCode,
                                Description = q.Description,
                                PricelistId = q.PricelistId,
                                Price = Math.Round((decimal)qq.Price, 2)
                            };
                return final.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }


        }


        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}