using Pos_WebApi.Helpers;
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
using PMSConnectionLib;
using Pos_WebApi.Controllers.Helpers;
using log4net;

namespace Pos_WebApi.Controllers
{
    public class PageButtonController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/Order
        public IEnumerable<PageButton> GetPageButtons()
        {
            return db.PageButton.OrderBy(o => o.Sort).AsEnumerable();
        }

        public IEnumerable<PageButton> GetPageButtons(int pageid)
        {
            return db.PageButton.Include(i => i.PageButtonDetail).Where(w => w.PageId == pageid).OrderBy(o => o.Sort).AsEnumerable();
        }

        /// <summary>
        /// Return page buttons for a specific Pos, PageId and Pricelist
        /// </summary>
        /// <param name="posid"></param>
        /// <param name="pageid"></param>
        /// <param name="pricelistid"></param>
        /// <returns></returns>
        public IEnumerable<Object> GetPageButtons(int posid, int pageid, int pricelistid, bool isPos = false)
        {
            try
            {
                var buttonWithProductsTemp = from q in db.PageButton
                                                .Include(i => i.Pages).AsNoTracking()
                                                 .Include(i => i.Product).AsNoTracking()
                                                 .Include(i => i.Product.PricelistDetail).AsNoTracking()
                                                 .Include(i => i.Product.PricelistDetail.Select(y => y.Vat)).AsNoTracking()
                                                 .Include(i => i.Product.Kitchen).AsNoTracking()
                                                 .Include(i => i.Product.KitchenRegion).AsNoTracking()
                                                 .Include(i => i.Product.ProductCategories).AsNoTracking()
                                                 .Where(w => w.PageId == pageid)
                                             select new
                                             {
                                                 Id = q.Id,
                                                 PageSetId = q.Pages.PageSetId,
                                                 ProductId = q.ProductId,
                                                 Code = q.Product.Code,
                                                 Description = q.Description,
                                                 ExtraDescription = q.Product.ExtraDescription,
                                                 SalesDescription = q.Product.SalesDescription,
                                                 PreparationTime = q.Product != null ? q.Product.PreparationTime : 0,
                                                 Sort = q.Sort,
                                                 ProductCategoryId = q.Product.ProductCategoryId,
                                                 NavigateToPage = q.NavigateToPage,
                                                 SetDefaultPriceListId = q.SetDefaultPriceListId,
                                                 SetDefaultSalesType = q.SetDefaultSalesType,
                                                 Type = q.Type,
                                                 PageId = q.PageId,
                                                 Color = q.Color,
                                                 Background = q.Background,
                                                 KdsId = q.Product.KdsId,
                                                 KitchenId = q.Product.KitchenId,
                                                 KitchenCode = q.Product.Kitchen.Code,
                                                 ItemRegion = q.Product.KitchenRegion.ItemRegion,
                                                 RegionPosition = q.Product.KitchenRegion.RegionPosition,
                                                 ItemRegionAbbr = q.Product.KitchenRegion.Abbr,
                                                 IsCombo = q.Product.IsCombo,
                                                 IsComboItem = q.Product.IsComboItem,
                                                 IsReturnItem = q.Product.IsReturnItem,
                                                 CanSavePrice = q.CanSavePrice,
                                                 DA_ProductId = q.Product.DAId,
                                                 DA_ProductCategoryId = q.Product.ProductCategories.DAId,
                                                 PricelistDetails = q.Product.PricelistDetail.Select(ss => new
                                                 {
                                                     Id = ss.Id,
                                                     PricelistId = ss.PricelistId,
                                                     ProductId = ss.ProductId,
                                                     Price = ss.Price,
                                                     VatId = ss.VatId,
                                                     Vat = ss.Vat,
                                                     TaxId = ss.TaxId,
                                                     Tax = ss.Tax
                                                 })
                                             };
                var buttonWithProducts = (IEnumerable<Object>)null;
                if (isPos)
                    buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPos(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
                else
                    buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPda(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
                return buttonWithProducts.AsEnumerable();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                db.Dispose();
                GC.Collect();
            }
        }


        public Object GetPageButtonsByProduct(int posid, int productid, int pricelistid, bool isPos = false)
        {
            try
            {
                var buttonWithProductsTemp = from q in db.PageButton
                                                .Include(i => i.Pages).AsNoTracking()
                                                 .Include(i => i.Product).AsNoTracking()
                                                 .Include(i => i.Product.PricelistDetail).AsNoTracking()
                                                 .Include(i => i.Product.PricelistDetail.Select(y => y.Vat)).AsNoTracking()
                                                 .Include(i => i.Product.Kitchen).AsNoTracking()
                                                 .Include(i => i.Product.KitchenRegion).AsNoTracking()
                                                 .Include(i => i.Product.ProductCategories).AsNoTracking()
                                                 .Where(w => w.Product.Id == productid)
                                             select new
                                             {
                                                 Id = q.Id,
                                                 PageSetId = q.Pages.PageSetId,
                                                 ProductId = q.ProductId,
                                                 Code = q.Product.Code,
                                                 Description = q.Description,
                                                 ExtraDescription = q.Product.ExtraDescription,
                                                 SalesDescription = q.Product.SalesDescription,
                                                 PreparationTime = q.Product != null ? q.Product.PreparationTime : 0,
                                                 Sort = q.Sort,
                                                 ProductCategoryId = q.Product.ProductCategoryId,
                                                 NavigateToPage = q.NavigateToPage,
                                                 SetDefaultPriceListId = q.SetDefaultPriceListId,
                                                 SetDefaultSalesType = q.SetDefaultSalesType,
                                                 Type = q.Type,
                                                 PageId = q.PageId,
                                                 Color = q.Color,
                                                 Background = q.Background,
                                                 KdsId = q.Product.KdsId,
                                                 KitchenId = q.Product.KitchenId,
                                                 KitchenCode = q.Product.Kitchen.Code,
                                                 ItemRegion = q.Product.KitchenRegion.ItemRegion,
                                                 RegionPosition = q.Product.KitchenRegion.RegionPosition,
                                                 ItemRegionAbbr = q.Product.KitchenRegion.Abbr,
                                                 IsCombo = q.Product.IsCombo,
                                                 IsComboItem = q.Product.IsComboItem,
                                                 IsReturnItem = q.Product.IsReturnItem,
                                                 CanSavePrice = q.CanSavePrice,
                                                 DA_ProductId = q.Product.DAId,
                                                 DA_ProductCategoryId = q.Product.ProductCategories.DAId,
                                                 PricelistDetails = q.Product.PricelistDetail.Select(ss => new
                                                 {
                                                     Id = ss.Id,
                                                     PricelistId = ss.PricelistId,
                                                     ProductId = ss.ProductId,
                                                     Price = ss.Price,
                                                     VatId = ss.VatId,
                                                     Vat = ss.Vat,
                                                     TaxId = ss.TaxId,
                                                     Tax = ss.Tax
                                                 })
                                             };
                var buttonWithProducts = (IEnumerable<Object>)null;
                if (isPos)
                    buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPos(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
                else
                    buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPda(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
                return buttonWithProducts.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                db.Dispose();
                GC.Collect();
            }
        }

        public IEnumerable<Object> GetPageButtonsByProductCategory(int posid, int productcategoryid, int pricelistid, bool isPos = false)
        {
            try
            {
                var buttonWithProductsTemp = from q in db.PageButton
                                                .Include(i => i.Pages).AsNoTracking()
                                                 .Include(i => i.Product).AsNoTracking()
                                                 .Include(i => i.Product.PricelistDetail).AsNoTracking()
                                                 .Include(i => i.Product.PricelistDetail.Select(y => y.Vat)).AsNoTracking()
                                                 .Include(i => i.Product.Kitchen).AsNoTracking()
                                                 .Include(i => i.Product.KitchenRegion).AsNoTracking()
                                                 .Include(i => i.Product.ProductCategories).AsNoTracking()
                                                 .Where(w => w.Product.ProductCategoryId == productcategoryid)
                                             select new
                                             {
                                                 Id = q.Id,
                                                 PageSetId = q.Pages.PageSetId,
                                                 ProductId = q.ProductId,
                                                 Code = q.Product.Code,
                                                 Description = q.Description,
                                                 ExtraDescription = q.Product.ExtraDescription,
                                                 SalesDescription = q.Product.SalesDescription,
                                                 PreparationTime = q.Product != null ? q.Product.PreparationTime : 0,
                                                 Sort = q.Sort,
                                                 ProductCategoryId = q.Product.ProductCategoryId,
                                                 NavigateToPage = q.NavigateToPage,
                                                 SetDefaultPriceListId = q.SetDefaultPriceListId,
                                                 SetDefaultSalesType = q.SetDefaultSalesType,
                                                 Type = q.Type,
                                                 PageId = q.PageId,
                                                 Color = q.Color,
                                                 Background = q.Background,
                                                 KdsId = q.Product.KdsId,
                                                 KitchenId = q.Product.KitchenId,
                                                 KitchenCode = q.Product.Kitchen.Code,
                                                 ItemRegion = q.Product.KitchenRegion.ItemRegion,
                                                 RegionPosition = q.Product.KitchenRegion.RegionPosition,
                                                 ItemRegionAbbr = q.Product.KitchenRegion.Abbr,
                                                 IsCombo = q.Product.IsCombo,
                                                 IsComboItem = q.Product.IsComboItem,
                                                 IsReturnItem = q.Product.IsReturnItem,
                                                 CanSavePrice = q.CanSavePrice,
                                                 DA_ProductId = q.Product.DAId,
                                                 DA_ProductCategoryId = q.Product.ProductCategories.DAId,
                                                 PricelistDetails = q.Product.PricelistDetail.Select(ss => new
                                                 {
                                                     Id = ss.Id,
                                                     PricelistId = ss.PricelistId,
                                                     ProductId = ss.ProductId,
                                                     Price = ss.Price,
                                                     VatId = ss.VatId,
                                                     Vat = ss.Vat,
                                                     TaxId = ss.TaxId,
                                                     Tax = ss.Tax
                                                 })
                                             };
                var buttonWithProducts = (IEnumerable<Object>)null;
                if (isPos)
                    buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPos(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
                else
                    buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPda(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
                return buttonWithProducts.AsEnumerable();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                db.Dispose();
                GC.Collect();
            }
        }

        public Object GetPageButtonsByDAProduct(int posid, int daproductid, int pricelistid, bool isPos = false)
        {
            try
            {
                var buttonWithProductsTemp = from q in db.PageButton
                                                .Include(i => i.Pages).AsNoTracking()
                                                 .Include(i => i.Product).AsNoTracking()
                                                 .Include(i => i.Product.PricelistDetail).AsNoTracking()
                                                 .Include(i => i.Product.PricelistDetail.Select(y => y.Vat)).AsNoTracking()
                                                 .Include(i => i.Product.Kitchen).AsNoTracking()
                                                 .Include(i => i.Product.KitchenRegion).AsNoTracking()
                                                 .Include(i => i.Product.ProductCategories).AsNoTracking()
                                                 .Where(w => w.Product.DAId == daproductid)
                                             select new
                                             {
                                                 Id = q.Id,
                                                 PageSetId = q.Pages.PageSetId,
                                                 ProductId = q.ProductId,
                                                 Code = q.Product.Code,
                                                 Description = q.Description,
                                                 ExtraDescription = q.Product.ExtraDescription,
                                                 SalesDescription = q.Product.SalesDescription,
                                                 PreparationTime = q.Product != null ? q.Product.PreparationTime : 0,
                                                 Sort = q.Sort,
                                                 ProductCategoryId = q.Product.ProductCategoryId,
                                                 NavigateToPage = q.NavigateToPage,
                                                 SetDefaultPriceListId = q.SetDefaultPriceListId,
                                                 SetDefaultSalesType = q.SetDefaultSalesType,
                                                 Type = q.Type,
                                                 PageId = q.PageId,
                                                 Color = q.Color,
                                                 Background = q.Background,
                                                 KdsId = q.Product.KdsId,
                                                 KitchenId = q.Product.KitchenId,
                                                 KitchenCode = q.Product.Kitchen.Code,
                                                 ItemRegion = q.Product.KitchenRegion.ItemRegion,
                                                 RegionPosition = q.Product.KitchenRegion.RegionPosition,
                                                 ItemRegionAbbr = q.Product.KitchenRegion.Abbr,
                                                 IsCombo = q.Product.IsCombo,
                                                 IsComboItem = q.Product.IsComboItem,
                                                 IsReturnItem = q.Product.IsReturnItem,
                                                 CanSavePrice = q.CanSavePrice,
                                                 DA_ProductId = q.Product.DAId,
                                                 DA_ProductCategoryId = q.Product.ProductCategories.DAId,
                                                 PricelistDetails = q.Product.PricelistDetail.Select(ss => new
                                                 {
                                                     Id = ss.Id,
                                                     PricelistId = ss.PricelistId,
                                                     ProductId = ss.ProductId,
                                                     Price = ss.Price,
                                                     VatId = ss.VatId,
                                                     Vat = ss.Vat,
                                                     TaxId = ss.TaxId,
                                                     Tax = ss.Tax
                                                 })
                                             };
                var buttonWithProducts = (IEnumerable<Object>)null;
                if (isPos)
                    buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPos(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
                else
                    buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPda(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
                return buttonWithProducts.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                db.Dispose();
                GC.Collect();
            }
        }

        public IEnumerable<Object> GetPageButtonsByDAProductCategory(int posid, int daproductcategoryid, int pricelistid, bool isPos = false)
        {
            try
            {
                var buttonWithProductsTemp = from q in db.PageButton
                                                .Include(i => i.Pages).AsNoTracking()
                                                 .Include(i => i.Product).AsNoTracking()
                                                 .Include(i => i.Product.PricelistDetail).AsNoTracking()
                                                 .Include(i => i.Product.PricelistDetail.Select(y => y.Vat)).AsNoTracking()
                                                 .Include(i => i.Product.Kitchen).AsNoTracking()
                                                 .Include(i => i.Product.KitchenRegion).AsNoTracking()
                                                 .Include(i => i.Product.ProductCategories).AsNoTracking()
                                                 .Where(w => w.Product.ProductCategories.DAId == daproductcategoryid)
                                             select new
                                             {
                                                 Id = q.Id,
                                                 PageSetId = q.Pages.PageSetId,
                                                 ProductId = q.ProductId,
                                                 Code = q.Product.Code,
                                                 Description = q.Description,
                                                 ExtraDescription = q.Product.ExtraDescription,
                                                 SalesDescription = q.Product.SalesDescription,
                                                 PreparationTime = q.Product != null ? q.Product.PreparationTime : 0,
                                                 Sort = q.Sort,
                                                 ProductCategoryId = q.Product.ProductCategoryId,
                                                 NavigateToPage = q.NavigateToPage,
                                                 SetDefaultPriceListId = q.SetDefaultPriceListId,
                                                 SetDefaultSalesType = q.SetDefaultSalesType,
                                                 Type = q.Type,
                                                 PageId = q.PageId,
                                                 Color = q.Color,
                                                 Background = q.Background,
                                                 KdsId = q.Product.KdsId,
                                                 KitchenId = q.Product.KitchenId,
                                                 KitchenCode = q.Product.Kitchen.Code,
                                                 ItemRegion = q.Product.KitchenRegion.ItemRegion,
                                                 RegionPosition = q.Product.KitchenRegion.RegionPosition,
                                                 ItemRegionAbbr = q.Product.KitchenRegion.Abbr,
                                                 IsCombo = q.Product.IsCombo,
                                                 IsComboItem = q.Product.IsComboItem,
                                                 IsReturnItem = q.Product.IsReturnItem,
                                                 CanSavePrice = q.CanSavePrice,
                                                 DA_ProductId = q.Product.DAId,
                                                 DA_ProductCategoryId = q.Product.ProductCategories.DAId,
                                                 PricelistDetails = q.Product.PricelistDetail.Select(ss => new
                                                 {
                                                     Id = ss.Id,
                                                     PricelistId = ss.PricelistId,
                                                     ProductId = ss.ProductId,
                                                     Price = ss.Price,
                                                     VatId = ss.VatId,
                                                     Vat = ss.Vat,
                                                     TaxId = ss.TaxId,
                                                     Tax = ss.Tax
                                                 })
                                             };
                var buttonWithProducts = (IEnumerable<Object>)null;
                if (isPos)
                    buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPos(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
                else
                    buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPda(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
                return buttonWithProducts.AsEnumerable();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                db.Dispose();
                GC.Collect();
            }
        }


        public IEnumerable<Object> GetPageButtons(int posid, bool customproducts, int pricelistid, bool isPos = false)
        {

            var posDepId = (long)db.PosInfo.Where(f => f.Id == posid).Select(ff => ff.DepartmentId).FirstOrDefault();


            var buttonWithProductsTemp = from q in db.Product.Where(w => w.IsCustom == true)
                                         join qq in db.PricelistDetail.Where(w => w.PricelistId == pricelistid) on q.Id equals qq.ProductId
                                         into ff
                                         from q1 in ff.DefaultIfEmpty()
                                         select new
                                         {
                                             Id = q.Id,
                                             // vagos
                                             //MappedPriceLists = q.TransferMappings.Where(t => t.PosDepartmentId == posDepId).Select(f => f.PriceListId),
                                             Description = q.Description,
                                             PreparationTime = q != null ? q.PreparationTime : 0,
                                             Price = q1.Price,
                                             PricelistId = q1.PricelistId,
                                             KdsId = q.KdsId,
                                             VatId = q1.VatId,
                                             Vat = q1.Vat.Percentage,
                                             VatCode = q1.Vat.Code,
                                             KitchenId = q.KitchenId,
                                             ItemRegion = q.KitchenRegion.ItemRegion,
                                             RegionPosition = q.KitchenRegion.RegionPosition,
                                             ItemRegionAbbr = q.KitchenRegion.Abbr,
                                             Code = q.Code,
                                             ProductCategoryId = q.ProductCategoryId,
                                         };

            var buttonWithProducts = (IEnumerable<Object>)null;
            if (isPos)
                buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPos(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
            else
                buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPda(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
            return buttonWithProducts.AsEnumerable();
        }

        // GET api/Order/5
        public PageButton GetPageButton(long id)
        {
            PageButton pagebutton = db.PageButton.Find(id);
            if (pagebutton == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return pagebutton;
        }

        // PUT api/Order/5
        public HttpResponseMessage PutPageButton(long id, PageButton pagebutton)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != pagebutton.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(pagebutton).State = EntityState.Modified;

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

        // POST api/Order
        public HttpResponseMessage PostPageButton(PageButton pagebutton)
        {
            if (ModelState.IsValid)
            {
                db.PageButton.Add(pagebutton);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, pagebutton);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = pagebutton.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Order/5
        public HttpResponseMessage DeletePageButton(long id)
        {
            PageButton pagebutton = db.PageButton.Find(id);
            if (pagebutton == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.PageButton.Remove(pagebutton);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, pagebutton);
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