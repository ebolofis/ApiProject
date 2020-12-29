using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Repositories.BORepos;
using Pos_WebApi.Models;
using System.Linq;
using Pos_WebApi.Helpers;
using System.Text;
using log4net;

namespace Pos_WebApi.Controllers
{
    public class TransferMappingsController : ApiController
    {
        GenericRepository<TransferMappings> svc;
        LookUpRepository lookupSvc;
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TransferMappingsController()
        {
            svc = new GenericRepository<TransferMappings>(db);
            lookupSvc = new LookUpRepository(db);
        }

        // GET api/TransferMappings
        public IEnumerable<TransferMappings> GetTransferMappings()
        {
            return svc.GetAll();
        }

        /// <summary>
        /// Get TransferMappings for a specific PosDepartmentId. Return  a list of objects.
        ///if CustomerPolicy = 4 [πολλαπλά ξενοδοχεία(PmsInterface)]
       ///       Every object in the list contains:  HotelId, ProductCategoryId and a list of PricelistId.
        ///  else
        ///      Every object in the list contains:  ProductCategoryId and a list of PricelistId.
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="posDeparmentId"></param>
        /// <returns></returns>
        [Route("api/{storeId}/TransferMappings/{posDeparmentId}")]
        public IEnumerable<Object> GetTransferMappingsForPos(string storeId, long posDeparmentId)
        {
            db = new PosEntities(false, Guid.Parse(storeId));
            svc = new GenericRepository<TransferMappings>(db);

            var hi = db.HotelInfo.FirstOrDefault();

            //if CustomerPolicy = 4 [πολλαπλά ξενοδοχεία (PmsInterface)] return ProductCategories group by HotelId and ProductCategoryId. Erery object contains:  HotelId, ProductCategoryId and a list of ProductCategories
            if (hi != null && hi.Type == (int)CustomerPolicyEnum.PmsInterface)
            {
                return svc.FindBy(w => w.ProductCategoryId != null && w.PosDepartmentId == posDeparmentId)
                                                           .Select(s => new { s.HotelId, s.ProductCategoryId, PricelistId = s.PriceListId })
                                                           .GroupBy(g => new { g.HotelId, g.ProductCategoryId })
                                                           .Select(ss => new
                                                           {
                                                               HotelId = ss.Key.HotelId,
                                                               ProductCategoryId = ss.Key.ProductCategoryId,
                                                               Pricelists = ss.Select(sss => sss.PricelistId).Distinct()
                                                           })
                                                           .OrderBy(o1 => o1.HotelId).ThenBy(o => o.ProductCategoryId);

            }
            else
            {
                var hotelid = (hi != null) ? hi.HotelId : 1;

                return svc.FindBy(x => x.PosDepartmentId == posDeparmentId && x.HotelId == hotelid).Distinct()
                                                          .Where(w => w.ProductCategoryId != null && w.PosDepartmentId == posDeparmentId)
                                                          .Select(s => new { s.ProductCategoryId, PricelistId = s.PriceListId })
                                                          .GroupBy(g => g.ProductCategoryId)
                                                          .Select(ss => new { ProductCategoryId = ss.Key, Pricelists = ss.Select(sss => sss.PricelistId) })
                                                          .OrderBy(o => o.ProductCategoryId);
            }
        }

        public IEnumerable<TransferMappings> GetTransferMappings(long posDepartmentId)
        {
            return svc.FindBy(x => x.PosDepartmentId == posDepartmentId);
        }
        public PagedResult<TransferMappings> GetTransferMappings(int page, int pageSize)
        {
            return svc.GetPaged(x => true, s => "Id", page, pageSize);
        }
        public PagedResult<TransferMappings> GetTransferMappings(int page, int pageSize, long filterPosDepartmentId, string filterPmsDepartmentId, long filterPriceListId, int filterVatCode)
        {
            return svc.GetPaged(x => x.PosDepartmentId == filterPosDepartmentId
                                  && x.PmsDepartmentId == filterPmsDepartmentId
                                  && x.PriceListId == filterPriceListId
                                  && x.VatCode == filterVatCode
                                  , s => "Id", page, pageSize);
        }
        // GET api/TransferMappings/5
        //public TransferMappings GetTransferMappings(long id)
        //{
        //    return svc.FindBy(x => x.Id == id).FirstOrDefault();
        //}

        // PUT api/TransferMappings/5
        [HttpPut]
        public HttpResponseMessage PutTransferMappings(TransferMappings model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            //if (id != model.Id)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}
            if (model.HotelId == null)
            {
                var hi = db.HotelInfo.FirstOrDefault();
                model.HotelId = hi != null ? hi.HotelId ?? 1 : 1;
            }

            svc.Update(model);

            try
            {
                if (svc.SaveChanges() == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
                RemoveDuplicateTransferMappings();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/TransferMappings
        public HttpResponseMessage PostTransferMappings(IEnumerable<TransferMappings> models)
        {
            if (ModelState.IsValid)
            {
                var hi = db.HotelInfo.FirstOrDefault();
                foreach (var model in models)
                {
                    if (model.HotelId == null)
                    {

                        model.HotelId = hi != null ? hi.HotelId ?? 1 : 1;
                    }
                }
                var res = svc.AddRange(models);
                try
                {
                    if (svc.SaveChanges() == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);

                    RemoveDuplicateTransferMappings();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, models);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", res));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        public HttpResponseMessage DeleteTransferMappings(IEnumerable<long> models)
        {
            if (ModelState.IsValid)
            {
                foreach (long value in models)
                {
                    svc.Delete(x => x.Id == value);
                }
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
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        private bool RemoveDuplicateTransferMappings()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                var doublePrices = db.TransferMappings.ToList().GroupBy(g => new { g.HotelId, g.PosDepartmentId, g.ProductCategoryId, g.PriceListId, g.PmsDepartmentId })
                                   .Where(w => w.Count() > 1)
                                   .Select(s => new
                                   {
                                       KeyToKeep = s.FirstOrDefault().Id,
                                       KeysToChange = s.Select(ss => ss.Id),
                                       Double = s.Select(ss => ss)
                                   });
                foreach (var keyToFixed in doublePrices)
                {
                    var invalidKeys = keyToFixed.KeysToChange.Where(w => w != keyToFixed.KeyToKeep);

                    var str = invalidKeys.Distinct().Aggregate("", (previous, next) => previous + ", " + next).Trim().Remove(0, 1);
                    sb.AppendLine(string.Format("Delete from TransferMappings where id in ({0})", str));
                }

                db.Database.ExecuteSqlCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
               // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error RemoveDuplicateTransferMappings| " + sb.ToString()));
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error RemoveDuplicateTransferMappings| " + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                return false;
            }
            return true;
        }

        // DELETE api/TransferMappings/5
        //public HttpResponseMessage DeleteTransferMappings(long id)
        //{
        //    svc.Delete(x => x.Id == id);
        //    try
        //    {
        //        if (svc.SaveChanges() == 0)
        //            return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.INSERTFAILED);
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
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
