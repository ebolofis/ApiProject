using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
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
using System.Dynamic;
using System.Text;


namespace Pos_WebApi.Controllers
{
    //[Authorize]
    //public class ProductController : ApiController
    //{
    //    private PosEntities db = new PosEntities(false);

    //    // GET api/Product
    //    public IEnumerable<Product> GetProducts(string storeid)
    //    {
    //        var products = db.Product.Include(i => i.PricelistDetail).Include(i => i.ProductCategories).Include(i => i.Kitchen).Include(i => i.Kds).AsNoTracking().AsEnumerable();
    //        //var query = (from p in products
    //        //             select new 
    //        //             {
    //        //                 Id = p.Id,
    //        //                 Code = p.Code,
    //        //                 Description = p.Description,
    //        //                 ExtraDescription = p.ExtraDescription,
    //        //                 ImageUri = p.ImageUri,
    //        //                 Kds = new Kds
    //        //                 {
    //        //                     Id = p.Kds.Id,
    //        //                     Description = p.Kds.Description,
    //        //                     Status = p.Kds.Status,
    //        //                     PosInfoId = p.Kds.PosInfoId
    //        //                 },
    //        //                 KdsId = p.KdsId,
    //        //                 Kitchen = p.Kitchen,
    //        //                 KitchenId = p.KitchenId,
    //        //                 PreparationTime = p.PreparationTime,
    //        //                 PricelistDetail = p.PricelistDetail.Select(s => new PricelistDetail
    //        //                 {
    //        //                     Id = s.Id,
    //        //                     IngredientId = s.IngredientId,
    //        //                     MinRequiredExtras = s.MinRequiredExtras,
    //        //                     Price = s.Price,
    //        //                     Pricelist = s.Pricelist,
    //        //                     PricelistId = s.PricelistId,
    //        //                     PriceWithout = s.PriceWithout,
    //        //                     ProductId = s.ProductId,
    //        //                     TaxId= s.TaxId,
    //        //                     VatId = s.VatId,
    //        //                     Vat = s.Vat
    //        //                 }),
    //        //                 ProductCategories = p.ProductCategories,
    //        //                 ProductCategoryId = p.ProductCategoryId,
    //        //                 Qty = p.Qty,
    //        //                 SalesDescription = p.SalesDescription,
    //        //                 UnitId = p.UnitId
    //        //             }).ToList();
    //        return products;
    //    }
    //    public Object GetProducts(string storeid, int fromPage, int pageSize, string cd_mask, string filters, string sidx)
    //    {

    //        //IEnumerable<Product> products = db.Product.Include(i => i.PricelistDetail).Include(i => i.ProductCategories).Include(i => i.Kitchen).Include(i => i.Kds).AsNoTracking();

    //        var skiped = fromPage * pageSize;
    //        sidx = sidx.Replace("ProductCategoryDesc", "ProductCategories.Description");
    //        sidx = sidx.Replace("KitchenDesc", "Kitchen.Description");
    //        sidx = sidx.Replace("KdsDesc", "Kds.Description");
    //        sidx = sidx.Replace("UnitAbbr", "Units.Description");

            
            
    //        string orderby = sidx;//.Substring(sidx.IndexOf(',') + 2);
    //        if (String.IsNullOrEmpty(orderby))
    //            orderby = "Description";
    //        //if (!String.IsNullOrWhiteSpace(sidx))
    //        //{

    //        //    string sortcol = sidx;
    //        //    string thenby = sortcol;
    //        //    try
    //        //    {
    //        //        sortcol = sidx.Substring(0, sidx.IndexOf(' '));
    //        //        orderby = sidx.Substring(sidx.IndexOf(',') + 2);
    //        //    }
    //        //    catch
    //        //    {
    //        //    }
    //        //    //      products = db.Product.AsQueryable<Product>();//.ThenBy(thenby);
    //        //}
    //        StringBuilder sb = new StringBuilder();
    //        if (!String.IsNullOrWhiteSpace(cd_mask))
    //        {
    //            skiped = 0;
    //            //products = products.Where(w => w.Description.ToLower().Contains(cd_mask.ToLower()));
    //            sb.Append(@"Description.ToLower().StartsWith(""" + cd_mask.ToLower() + @""")");
    //        }
    //        if (!String.IsNullOrWhiteSpace(filters))
    //        {
    //            skiped = 0;
    //            SearchObject filter = JsonConvert.DeserializeObject<SearchObject>(filters);

    //            if (filter.groupOp == "OR")
    //            {
    //                foreach (var r in filter.rules)
    //                {
                       
    //                    sb.Append(@" (" + r.field + @" != null && " + r.field + @".ToLower().StartsWith(""" + r.data + @"""))");
    //                }
    //            }
    //            else if (filter.groupOp == "AND")
    //            {
    //                foreach (var r in filter.rules)
    //                {
                       
    //                    if (sb.Length > 0)
    //                        sb.Append(" && ");
    //                    sb.Append(@" (" + r.field + @" != null && " + r.field + @".ToLower().StartsWith(""" + r.data + @"""))");
    //                }
    //            }
    //        }
    //        var totalRecs = 0;
    //        IEnumerable<Product> products;
    //        if (sb.Length > 0)
    //        {
    //            products = db.Product.Include(i => i.PricelistDetail)
    //                                 .Include(i => i.ProductCategories)
    //                                 .Include(i => i.Kitchen)
    //                                 .Include(i => i.Kds).AsNoTracking()
    //                                 .Where(sb.ToString()).OrderBy(orderby);
    //            totalRecs = db.Product.Where(sb.ToString()).Count();
    //        }
    //        else
    //        {
    //            products = db.Product.Include(i => i.PricelistDetail)
    //                                 .Include(i => i.ProductCategories)
    //                                 .Include(i => i.Kitchen)
    //                                 .Include(i => i.Kds).AsNoTracking()
    //                                 .OrderBy(orderby);
    //            totalRecs = db.Product.Count();
    //        }
    //        // products = products.AsEnumerable<Product>()sEnumerable<Product>();
    //        // var products = db.Product.OrderBy(o=>o.Id).Skip(skiped).Take(pageSize).Include(i => i.PricelistDetail).Include(i => i.ProductCategories).Include(i => i.Kitchen).Include(i => i.Kds).AsNoTracking().AsEnumerable();

    //        products = products.Skip(skiped).Take(pageSize);
    //        return new { products, totalRecs };
    //    }

    //    public Object GetProducts(int posid, bool customproducts, int pricelistid)
    //    {
    //        var buttonWithProductsTemp = from q in db.Product.Include("Kitchen").Include("PricelistDetail").Include("PricelistDetail.Vat").Where(w => w.IsCustom == true)
    //                                     //join qq in db.PricelistDetail.Where(w => w.PricelistId == pricelistid) on q.Id equals qq.ProductId
    //                                     //into ff
    //                                     //from q1 in ff.DefaultIfEmpty()
    //                                     select new
    //                                     {
    //                                         Id = q.Id,
    //                                         ProductId = q.Id,
    //                                         Description = q.Description,
    //                                         PreparationTime = q != null ? q.PreparationTime : 0,
    //                                         //Price = q1.Price,
    //                                         //PricelistId = q1.PricelistId,
    //                                         KdsId = q.KdsId,
    //                                         //VatId = q1.VatId,
    //                                         //Vat = q1.Vat.Percentage,
    //                                         //VatCode = q1.Vat.Code,
    //                                         KitchenId = q.KitchenId,
    //                                         KitchenCode = q.Kitchen.Code,
    //                                         ItemRegion = "1o Piato",
    //                                         RegionPosition = 1,
    //                                         //PriceListDetailId = q1.Id,
    //                                         PricelistDetails = q.PricelistDetail.Select(ss => new
    //                                                            {
    //                                                                Id = ss.Id,
    //                                                                PricelistId = ss.PricelistId,
    //                                                                Vat = ss.Vat,
    //                                                                Price = ss.Price,
    //                                                                PriceWithout = ss.PriceWithout
    //                                                            }).AsEnumerable()
    //                                     };

    //        var buttonDetailsTemp = (from q in db.ProductRecipe
    //                                     //join qq in db.PricelistDetail.Where(w => w.PricelistId == pricelistid) on q.IngredientId equals qq.IngredientId
    //                                     //      into ff
    //                                     //from q1 in ff.DefaultIfEmpty()
    //                                 select new
    //                                 {
    //                                     ProductId = q.ProductId,
    //                                     IngredientId = q.IngredientId,
    //                                     Description = q.Ingredients.SalesDescription,
    //                                     Background = q.Ingredients.Background,
    //                                     Color = q.Ingredients.Color,
    //                                     MaxQty = q.MaxQty,
    //                                     MinQty = q.MinQty,
    //                                     Qty = q.Qty,
    //                                     Type = (byte?)0,
    //                                     Price = q.Price,
    //                                     Sort = q.Sort
    //                                     //AddCost = q1.Price,
    //                                     //RemoveCost = q1.PriceWithout
    //                                 }).Union(from q in db.ProductExtras
    //                                          //join qq in db.PricelistDetail.Where(w => w.PricelistId == pricelistid) on q.IngredientId equals qq.IngredientId
    //                                          //      into ff
    //                                          //from q1 in ff.DefaultIfEmpty()
    //                                          select new
    //                                          {
    //                                              ProductId = q.ProductId,
    //                                              IngredientId = q.IngredientId,
    //                                              Description = q.Ingredients.SalesDescription,
    //                                              Background = q.Ingredients.Background,
    //                                              Color = q.Ingredients.Color,
    //                                              MaxQty = q.MaxQty,
    //                                              MinQty = q.MinQty,
    //                                              Qty = (double?)0,
    //                                              Type = (byte?)1,
    //                                              Price = q.Price,
    //                                              Sort = q.Sort
    //                                              //AddCost = q1.Price,
    //                                              //RemoveCost = q1.PriceWithout
    //                                          });

    //        var buttonWithProductsflat = (from q in buttonWithProductsTemp
    //                                      join q1 in buttonDetailsTemp on q.ProductId equals q1.ProductId
    //                                               into ff
    //                                      from q1 in ff.DefaultIfEmpty()

    //                                      select new
    //                                      {
    //                                          Id = q.Id,
    //                                          ProductId = q.ProductId,
    //                                          Description = q.Description,
    //                                          PreparationTime = q.PreparationTime,
    //                                          //Price = q.Price,
    //                                          KdsId = q.KdsId,
    //                                          //VatId = q.VatId,
    //                                          //Vat = q.Vat,
    //                                          //VatCode = q.VatCode,
    //                                          KitchenId = q.KitchenId,
    //                                          KitchenCode = q.KitchenCode,
    //                                          ItemRegion = q.ItemRegion,
    //                                          RegionPosition = q.RegionPosition,
    //                                          //PriceListDetailId = q.PriceListDetailId,
    //                                          PricelistDetails = q.PricelistDetails,

    //                                          Detail_IngredientId = q1.IngredientId,// q1 != null ? q1.IngredientId : null,
    //                                          Detail_Background = q1.Background,
    //                                          Detail_Color = q1.Color,
    //                                          Detail_Description = q1.Description,//q1 != null ? q1.Description : null,
    //                                          Detail_MaxQty = q1.MaxQty,//q1 != null ? q1.MaxQty : null,
    //                                          Detail_MinQty = q1.MinQty,//q1 != null ? q1.MinQty : null,
    //                                          Detail_Type = q1.Type,//q1 == null || q1.Type == null ? 0 : q1.Type,
    //                                          Detail_Price = q1.Price,//q1 != null ? q1.Price : null,
    //                                          Detail_Qty = q1.Qty,//q1 != null ? q1.Qty : null
    //                                          Detail_Sort = q1.Sort,
    //                                          //Detail_AddCost = q1.AddCost,
    //                                          //Detail_RemoveCost = q1.RemoveCost,
    //                                      });
    //        var buttonWithProducts = (buttonWithProductsflat.GroupBy(g => new { g.ProductId, g.Id }).ToList().Select(s => new
    //        {
    //            Id = s.Key.Id,
    //            ProductId = s.Key.ProductId,
    //            Description = s.FirstOrDefault().Description,
    //            PreparationTime = Convert.ToInt16(s.FirstOrDefault().PreparationTime),
    //            //Price = s.FirstOrDefault().Price,
    //            KdsId = s.FirstOrDefault().KdsId,
    //            //VatId = s.FirstOrDefault().VatId,
    //            //VatCode = s.FirstOrDefault().VatCode,
    //            //Vat = s.FirstOrDefault().Vat,
    //            KitchenId = s.FirstOrDefault().KitchenId,
    //            KitchenCode = s.FirstOrDefault().KitchenCode,
    //            ItemRegion = s.FirstOrDefault().ItemRegion,
    //            RegionPosition = s.FirstOrDefault().RegionPosition,

    //            //PriceListDetailId = s.FirstOrDefault().PriceListDetailId,
    //            PricelistDetails = s.FirstOrDefault().PricelistDetails,

    //            PageButtonDetail = s.Distinct().Select((ss, index) => new PageButtonDetail()
    //            {
    //                Id = index,
    //                PageButtonId = s.FirstOrDefault().Id,
    //                ProductId = ss.Detail_IngredientId,
    //                Description = ss.Detail_Description,
    //                Background = ss.Detail_Background,
    //                Color = ss.Detail_Color,
    //                MaxQty = ss.Detail_MaxQty,
    //                MinQty = ss.Detail_MinQty,
    //                Type = ss.Detail_Type,
    //                Qty = ss.Detail_Qty,
    //                Sort = (short?)ss.Detail_Sort,
    //                //AddCost = ss.Detail_AddCost,
    //                //RemoveCost = ss.Detail_RemoveCost
    //                PriceListDetailId = null,
    //                PricelistDetails = db.PricelistDetail.Include("Vat").AsNoTracking().Where(w => w.IngredientId == ss.Detail_IngredientId).AsEnumerable()
    //            }).OrderBy(o => o.Type)//.Where(w => w.ProductId == s.Key.ProductId)
    //            .ToList<PageButtonDetail>()
    //        })).AsEnumerable();

    //        return buttonWithProducts.AsEnumerable();
    //    }

    //    public IEnumerable<ProductGroupedByCategories> GetProducts(string storeid, string isgrouped)
    //    {
    //        var query = db.Product.Include(i => i.PricelistDetail).Include(i => i.ProductCategories).AsNoTracking().AsEnumerable();
    //        var q = from p in db.ProductCategories
    //                join j in query on p.Id equals j.ProductCategoryId into ps
    //                select new ProductGroupedByCategories
    //                {
    //                    Category = p.Description,
    //                    Products = ps
    //                };
    //        return q;
    //    }

    //    //For HQ
    //    //public IEnumerable<ProductGroupedByCategories> GetProducts(string storeid, string isgrouped, long pricelistid,  bool forHQ)
    //    //{
    //    //    var query = db.Product.Include(i => i.PricelistDetail).Include(i => i.ProductCategories).AsNoTracking().AsEnumerable();
    //    //    var q = from p in db.ProductCategories
    //    //            join j in query on p.Id equals j.ProductCategoryId into ps
    //    //            select new ProductGroupedByCategories
    //    //            {
    //    //                Category = p.Description,
    //    //                Products = ps
    //    //            };
    //    //    return q;
    //    //}


    //    public Object GetProducts(string recipe, bool checkrecipe, int pricelistid, long posid)
    //    {

    //        var rec = JsonConvert.DeserializeObject<RecipeRootObject>(recipe);
    //        var productrecipes = db.ProductRecipe.GroupBy(g => g.ProductId).Select(s => new
    //        {
    //            ProductId = s.Key,
    //            Count = s.Count(),
    //            Recipe = s.Select(ss => ss)
    //        });
    //        var productswithsamecount = productrecipes.Where(w => w.Count == rec.recipe.Count);
    //        foreach (var item in productswithsamecount)
    //        {
    //            List<long?> mappedrecipe = item.Recipe.Select(s => s.IngredientId).ToList();
    //            List<long?> jsonrecipe = rec.recipe.Select(s => s.ProductId).ToList();
    //            var posDepId = (long)db.PosInfo.Where(f => f.Id == posid).Select(ff => ff.DepartmentId).FirstOrDefault();
    //            var notsame = mappedrecipe.Except(jsonrecipe).ToList();
    //            if (notsame.Count == 0)
    //            {
    //                long productid = item.ProductId.Value;
    //                return GetPageButtonFromProduct(pricelistid, posDepId, productid);
    //            }
    //        }
    //        return null;
    //    }

    //    private object GetPageButtonFromProduct(int pricelistid, long posDepId, long productid)
    //    {
    //        var result = db.Product.Include(i => i.PageButton).Include(i => i.PageButton.Select(ss => ss.Pages))
    //                                 .Include(i => i.PricelistDetail)
    //                                 .Include(i => i.PricelistDetail.Select(y => y.Vat))
    //                                 .Include(i => i.Kitchen)
    //                                 .Include(i => i.KitchenRegion).Where(w => w.Id == productid);
    //        var buttonWithProductsTemp = from q in result
    //                                     //join qq in db.PricelistDetail.Where(w => w.PricelistId == pricelistid) on q.ProductId equals qq.ProductId
    //                                     //into ff
    //                                     //from q1 in ff.DefaultIfEmpty()
    //                                     select new
    //                                     {
    //                                         Id = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Id : 0,
    //                                         MappedPriceLists = q.TransferMappings.Where(t => t.PosDepartmentId == posDepId).Select(f => f.PriceListId),
    //                                         ProductId = q.Id,
    //                                         Description = q.Description,
    //                                         PreparationTime = q.PreparationTime != null ? q.PreparationTime : 0,
    //                                         //Price = q1.Price,
    //                                         Sort = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Sort : 0,
    //                                         NavigateToPage = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().NavigateToPage : null,
    //                                         SetDefaultPriceListId = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().SetDefaultPriceListId : null,
    //                                         SetDefaultSalesType = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().SetDefaultSalesType : null,
    //                                         Type = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Type : null,
    //                                         PageSetId = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Pages.PageSetId : null,
    //                                         PageId = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().PageId : null,
    //                                         Color = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Color : null,
    //                                         Background = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Background : null,
    //                                         //PricelistId = q1.PricelistId,
    //                                         KdsId = q.KdsId,
    //                                         Code = q.Code,
    //                                         KitchenCode = q.Kitchen.Code,
    //                                         ItemRegion = q.KitchenRegion.ItemRegion,
    //                                         RegionPosition = q.KitchenRegion.RegionPosition,
    //                                         ItemRegionAbbr = q.KitchenRegion.Abbr,
    //                                         // VatId = q1.VatId,
    //                                         // Vat = q1.Vat.Percentage,
    //                                         //  VatCode = q1.Vat.Code,
    //                                         KitchenId = q.KitchenId,
    //                                         // PriceListDetailId = q1.Id,
    //                                         PricelistDetails = q.PricelistDetail.Select(ss => new
    //                                         {
    //                                             Id = ss.Id,
    //                                             PricelistId = ss.PricelistId,
    //                                             ProductId = ss.ProductId,
    //                                             Price = ss.Price,
    //                                             VatId = ss.VatId,
    //                                             Vat = ss.Vat,
    //                                             TaxId = ss.TaxId,
    //                                             Tax = ss.Tax
    //                                         })
    //                                     };

    //        var buttonWithProducts = new PageButtonCreator().GetPageButtonsCreator(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);
    //        return buttonWithProducts.FirstOrDefault();
    //    }

    //    public Object GetPageButtonFromProductBC(int pricelistid, long posDepId, string barcodesearch, bool aba = false)
    //    {
    //        var productId = db.ProductBarcodes.Where(w => w.Barcode.Trim() == (barcodesearch.Trim())).Select(s => s.ProductId).FirstOrDefault();
    //        //if (productIdList = null)
       
    //        var result = db.Product.AsNoTracking()
    //                               .Include(i => i.PageButton)
    //                               .Include(i => i.PageButton.Select(ss => ss.Pages))
    //                               .Include(i => i.PricelistDetail)
    //                               .Include(i => i.PricelistDetail.Select(y => y.Vat))
    //                               .Include(i => i.Kitchen)
    //                               .Include(i => i.KitchenRegion)
    //                               .Where(w => productId == w.Id);
    //        var buttonWithProductsTemp = from q in result
    //                                     //join qq in db.PricelistDetail.Where(w => w.PricelistId == pricelistid) on q.ProductId equals qq.ProductId
    //                                     //into ff
    //                                     //from q1 in ff.DefaultIfEmpty()
    //                                     select new
    //                                     {
    //                                         Id = /*q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Id.ToString() : */q.Code,
    //                                         MappedPriceLists = q.TransferMappings.Where(t => t.PosDepartmentId == posDepId).Select(f => f.PriceListId),
    //                                         ProductId = q.Id,
    //                                         Description = q.Description,
    //                                         PreparationTime = q.PreparationTime != null ? q.PreparationTime : 0,
    //                                         //Price = q1.Price,
    //                                         Sort = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Sort : 0,
    //                                         NavigateToPage = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().NavigateToPage : null,
    //                                         SetDefaultPriceListId = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().SetDefaultPriceListId : null,
    //                                         SetDefaultSalesType = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().SetDefaultSalesType : null,
    //                                         Type = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Type : null,
    //                                         PageSetId = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Pages.PageSetId : null,
    //                                         PageId = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().PageId : null,
    //                                         Color = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Color : null,
    //                                         Background = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Background : null,
    //                                         //PricelistId = q1.PricelistId,
    //                                         KdsId = q.KdsId,
    //                                         Code = q.Code,
    //                                         KitchenCode = q.Kitchen.Code,
    //                                         ItemRegion = q.KitchenRegion.ItemRegion,
    //                                         RegionPosition = q.KitchenRegion.RegionPosition,
    //                                         ItemRegionAbbr = q.KitchenRegion.Abbr,
    //                                         // VatId = q1.VatId,
    //                                         // Vat = q1.Vat.Percentage,
    //                                         //  VatCode = q1.Vat.Code,
    //                                         KitchenId = q.KitchenId,
    //                                         // PriceListDetailId = q1.Id,
    //                                         PricelistDetails = q.PricelistDetail.Select(ss => new
    //                                         {
    //                                             Id = ss.Id,
    //                                             PricelistId = ss.PricelistId,
    //                                             ProductId = ss.ProductId,
    //                                             Price = ss.Price,
    //                                             VatId = ss.VatId,
    //                                             Vat = ss.Vat,
    //                                             TaxId = ss.TaxId,
    //                                             Tax = ss.Tax
    //                                         })
    //                                     };


    //        var buttonWithProducts = new PageButtonCreator().GetPageButtonsCreator(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, db);

    //        return buttonWithProducts.FirstOrDefault();
    //    }

    //    public string GetProducts(long categId, bool byCateg, bool onlyCount)
    //    {
    //        return db.Product.Where(w => w.ProductCategoryId == categId).Count().ToString();
    //    }

    //    [HttpGet]
    //    public bool UpdateProductsKitchenRegion(long kitchenRegionid, long productCategoryId)
    //    {
    //        try
    //        {
    //            var prods = db.Product.Where(f => f.ProductCategoryId == productCategoryId);

    //            //  if (prods.Count() == 0)
    //            //   return false;

    //            foreach (var prod in prods)
    //            {
    //                prod.KitchenRegionId = kitchenRegionid;
    //            }

    //            db.SaveChanges();
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            return false;
    //        }

    //    }

    //    public class Recipe
    //    {
    //        public long? ProductId { get; set; }
    //    }

    //    public class RecipeRootObject
    //    {
    //        public List<Recipe> recipe { get; set; }
    //    }

    //    // GET api/Product
    //    //public IEnumerable<ProductMappingClass> GetProducts()
    //    //{
    //    //    var query = (from s in db.Product
    //    //                 select new ProductMappingClass
    //    //                 {
    //    //                     Description = s.Description,
    //    //                     ExtraDescription = s.ExtraDescription,
    //    //                     Id = s.Id,
    //    //                     ImageUri = s.ImageUri,
    //    //                     PreparationTime = s.PreparationTime,
    //    //                     Qty = s.Qty,
    //    //                     SalesDescription = s.SalesDescription,
    //    //                     UnitId = s.UnitId,
    //    //                     KdsId = s.KdsId,
    //    //                     KitchenId = s.KitchenId
    //    //                 }).OrderBy(o=>o.Description);
    //    //    return query.AsEnumerable();
    //    //}

    //    // GET api/Product/5
    //    public Product GetProduct(long id, string storeid)
    //    {
    //        Product product = db.Product.Find(id);
    //        if (product == null)
    //        {
    //            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
    //        }

    //        return product;
    //    }

    //    public IEnumerable<Object> GetProducts(string storeid, bool forAllProducts)
    //    {
    //        var productCodes = db.ProductBarcodes.Include(i => i.Product).AsNoTracking().AsEnumerable();
    //        var query = (from p in productCodes
    //                     select new
    //                     {
    //                         Id = p.Product.Id,
    //                         Code = p.Barcode,
    //                         Description = p.Product.Description.ToLower()
    //                     }).ToList();
    //        return query;
    //    }

    //    public Object GetProduct(string storeid, long productid, int pricelistid, long posDepId)
    //    {
    //        return GetPageButtonFromProduct(pricelistid, posDepId, productid);
    //    }

    //    // PUT api/Product/5
    //    public HttpResponseMessage PutProduct(long id, string storeid, Product product)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
    //        }

    //        if (id != product.Id)
    //        {
    //            return Request.CreateResponse(HttpStatusCode.BadRequest);
    //        }

    //        db.Entry(product).State = EntityState.Modified;

    //        try
    //        {
    //            db.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
    //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
    //        }

    //        return Request.CreateResponse(HttpStatusCode.OK);
    //    }

    //    // POST api/Product
    //    public HttpResponseMessage PostProduct(Product product, string storeid)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            db.Product.Add(product);
    //            db.SaveChanges();
    //            if (String.IsNullOrEmpty(product.Code) == true)
    //            {
    //                product.Code = product.Id.ToString();
    //                db.SaveChanges();
    //            }
    //            // add default transfer mappings
    //            //var similarprod = db.Product.Where(f => f.ProductCategoryId == product.ProductCategoryId).FirstOrDefault();
    //            //if (similarprod != null)
    //            //{
    //            //    var catmappings = db.TransferMappings.Where(f => f.ProductId == similarprod.Id);

    //            //    foreach (var map in catmappings)
    //            //    {
    //            //        TransferMappings newmap = new TransferMappings();
    //            //        newmap.ProductId = product.Id;
    //            //        newmap.PmsDepartmentId = map.PmsDepartmentId;
    //            //        newmap.PosDepartmentId = map.PosDepartmentId;
    //            //        newmap.PriceListId = map.PriceListId;
    //            //        newmap.SalesTypeId = map.SalesTypeId;
    //            //        newmap.VatPercentage = map.VatPercentage;

    //            //        db.TransferMappings.Add(newmap);
    //            //    }

    //            //    db.SaveChanges();
    //            //}

    //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, product);
    //            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = product.Id }));
    //            return response;
    //        }
    //        else
    //        {
    //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
    //        }
    //    }

    //    // DELETE api/Product/5
    //    public HttpResponseMessage DeleteProduct(long id, string storeid)
    //    {
    //        Product product = db.Product.Find(id);
    //        if (product == null)
    //        {
    //            return Request.CreateResponse(HttpStatusCode.NotFound);
    //        }
    //        var pagebtns = db.PageButton.Where(w => w.ProductId == id);
    //        foreach (var p in pagebtns)
    //        {
    //            db.PageButton.Remove(p);
    //        }
    //        db.Product.Remove(product);
    //        var transfermappings = db.TransferMappings.Where(w => w.ProductId == id);
    //        foreach (var p in transfermappings)
    //        {
    //            db.TransferMappings.Remove(p);
    //        }
    //        db.Product.Remove(product);
    //        try
    //        {
    //            db.SaveChanges();
    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
    //        }

    //        return Request.CreateResponse(HttpStatusCode.OK, product);
    //    }

    //    [AllowAnonymous]
    //    public HttpResponseMessage Options()
    //    {
    //        return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        db.Dispose();
    //        base.Dispose(disposing);
    //    }
    //}



    public class SearchObject
    {
        public string groupOp { get; set; }
        public List<Rule> rules { get; set; }
    }

    public class Rule
    {
        public string field { get; set; }
        public string op { get; set; }
        public string data { get; set; }
    }
}