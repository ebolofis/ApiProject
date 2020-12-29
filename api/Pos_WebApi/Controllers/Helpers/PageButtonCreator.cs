using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Helpers {
    public class PageButtonCreator {
        public IEnumerable<Object> GetPageButtonsCreatorPda(List<dynamic> buttonWithProductsTemp, int pricelistid, PosEntities db) {

            List<long?> prodids = new List<long?>() {};

            
            foreach (var o in buttonWithProductsTemp)
            {
                //prodids.Add(o.Id);
                prodids.Add(o.ProductId);
            }

                //List<long?> prodcatids = new List<long?>() { 1, 2, 3 };
                ///Select Display Extras from Product Recipes
                var precs = (from q in db.ProductRecipe.Include("Product")//.Where(s=>prodids.Contains(s.ProductId))
                         select new {
                             ProductId = q.ProductId,
                             IngredientId = q.IngredientId,
                             Description = q.Ingredients.SalesDescription,
                             Background = q.Ingredients.Background,
                             Color = q.Ingredients.Color,
                             MaxQty = q.MaxQty,
                             MinQty = q.MinQty,
                             CanSavePrice=q.CanSavePrice,
                             Qty = q.Qty,
                             Type = ProductDetailTypeEnum.Recipe, //byte of type 0 : indicates that extra is on Recipe //Extras of this type can be excluded from ordered Item
                             Price = q.Price,
                             IngredientCategoryId = q.Ingredients.IngredientCategoryId,
                             Sort = (long?)q.Sort,
                            // CanSavePrice = q.CanSavePrice,
                         }).OrderBy(z => z.Sort).ToList<dynamic>();
            ///Select Display POS Extras from Product Extras
            var pextras = (from q in db.ProductExtras.Include("Product").Include("Product.PricelistDetail")//.Where(s => prodids.Contains(s.ProductId))
                           select new {
                               ProductId = q.ProductId,
                               IngredientId = q.IngredientId,
                               Description = q.Ingredients.SalesDescription,
                               Background = q.Ingredients.Background,
                               Color = q.Ingredients.Color,
                               MaxQty = q.MaxQty,
                               MinQty = q.MinQty,
                               CanSavePrice = q.CanSavePrice,
                               Qty = (double?)0,
                               Type = ProductDetailTypeEnum.Extras,
                               Price = q.Price,
                               IngredientCategoryId = q.Ingredients.IngredientCategoryId,
                               Sort = (long?)q.Sort,
                           }).OrderBy(z => z.Sort).ToList<dynamic>();

            ///Select Display POS Extras from Product Extras
            /*            
                        var assocs = (from q in db.Ingredient_ProdCategoryAssoc//.Where(s => prodcatids.Contains(s.ProductCategoryId))
                                      join qq in db.Product.Where(s => prodids.Contains(s.Id)) on q.ProductCategoryId equals qq.ProductCategoryId
                                      join qqq in db.Ingredients on q.IngredientId equals qqq.Id
                                      join qqqq in db.PricelistDetail.Where(s => prodids.Contains(s.ProductId) && pricelistid == s.PricelistId) on q.IngredientId equals qqqq.IngredientId
                                      select new {
                                          ProductId = (long?)qq.Id,
                                          IngredientId = q.IngredientId,
                                          Description = qqq.SalesDescription,
                                          Background = qqq.Background,
                                          Color = qqq.Color,
                                          MaxQty = (double?)1,
                                          MinQty = (double?)0,
                                          Qty = (double?)0,
                                          Type = ProductDetailTypeEnum.ExtrasAssoc,
                                          Price = qqqq.Price,
                                          Sort = (long?)q.Sort,
                                      }).OrderBy(z => z.Sort).ToList<dynamic>();
            */

                    
            var assocs = (from q in db.Ingredient_ProdCategoryAssoc//.Where(s => prodcatids.Contains(s.ProductCategoryId))
                          join qq in db.Product.Where(s => prodids.Contains(s.Id)) on q.ProductCategoryId equals qq.ProductCategoryId
                          join qqq in db.Ingredients on q.IngredientId equals qqq.Id
                          //join qqqq in db.PricelistDetail on q.IngredientId equals qqqq.IngredientId
                          //join qqqq in db.PricelistDetail.Where(s => s.PricelistId == pricelistid) on q.IngredientId equals qqqq.IngredientId
                          select new
                          {
                              
                              ProductId = (long?)qq.Id,
                              IngredientId = q.IngredientId,
                              Description = qqq.SalesDescription,
                              Background = qqq.Background,
                              Color = qqq.Color,
                              MaxQty = (double?)10,
                              MinQty = (double?)0,
                              Qty = (double?)0,
                              Type = ProductDetailTypeEnum.ExtrasAssoc,
                              Price = 0,//qqqq.Price,
                              IngredientCategoryId = qqq.IngredientCategoryId,
                              Sort = (long?)q.Sort,
                              CanSavePrice=q.CanSavePrice,
                          }).OrderBy(z => z.Sort).ToList<dynamic>();
                          

            


            List<dynamic> res = precs;
            res.AddRange(pextras);
            res.AddRange(assocs);
            List<dynamic> buttonDetailsTemp = res;//.GroupBy(x => x.IngredientId).Select(x => x.First());

            ///buttonWithProductsTemp : Dynamic PageButtons Created from Caller
            var buttonWithProductsflat = (from q in buttonWithProductsTemp
                                          join q1 in buttonDetailsTemp on q.ProductId equals q1.ProductId into ff
                                          from q1 in ff.DefaultIfEmpty()
                                          select new {
                                              Id = q.Id,
                                              PageSetId = q.PageSetId,
                                              ProductId = q.ProductId,
                                              Description = q.Description,
                                              ExtraDescription = q.ExtraDescription,
                                              SalesDescription = q.SalesDescription,
                                              PreparationTime = q.PreparationTime,
                                              Sort = q.Sort,
                                              NavigateToPage = q.NavigateToPage,
                                              SetDefaultPriceListId = q.SetDefaultPriceListId,
                                              SetDefaultSalesType = q.SetDefaultSalesType,
                                              Type = q.Type,
                                              PageId = q.PageId,
                                              Color = q.Color,
                                              Background = q.Background,
                                              KdsId = q.KdsId,
                                              Code = q.Code,
                                              KitchenId = q.KitchenId,
                                              KitchenCode = q.KitchenCode,
                                              ItemRegion = q.ItemRegion,
                                              RegionPosition = q.RegionPosition,
                                              ItemRegionAbbr = q.ItemRegionAbbr,
                                              PricelistDetails = q.PricelistDetails, // as IEnumerable<PricelistDetail>,
                                              ProductCategoryId = q.ProductCategoryId,
                                              Detail_IngredientId = q1 != null ? q1.IngredientId : null,
                                              Detail_Description = q1 != null ? q1.Description : null,
                                              Detail_Background = q1 != null ? q1.Background : null,
                                              Detail_Color = q1 != null ? q1.Color : null,
                                              Detail_MaxQty = q1 != null ? q1.MaxQty : null,
                                              Detail_MinQty = q1 != null ? q1.MinQty : null,
                                              Detail_Type = q1 == null || q1.Type == null ? null : q1.Type,
                                              Detail_Price = q1 != null ? q1.Price : null,
                                              Detail_Qty = q1 != null ? q1.Qty : null,
                                              Detail_IngredientCategoryId = q1 != null ? q1.IngredientCategoryId : null,
                                              Detail_Sort = q1 != null ? q1.Sort : null,
                                              DA_ProductId = q.DA_ProductId,
                                              DA_ProductCategoryId = q.DA_ProductCategoryId
                                          });

            var join2 = from q in buttonWithProductsflat
                        join q1 in db.PricelistDetail.AsNoTracking().Include("Vat").Where(w => w.IngredientId != null) on q.Detail_IngredientId equals q1.IngredientId into fff
                        from q1 in fff.DefaultIfEmpty()
                        select new {
                            Id = q.Id,
                            PageSetId = q.PageSetId,
                            ProductId = q.ProductId,
                            Description = q.Description,
                            ExtraDescription = q.ExtraDescription,
                            SalesDescription = q.SalesDescription,
                            PreparationTime = q.PreparationTime,
                            Sort = q.Sort,
                            NavigateToPage = q.NavigateToPage,
                            SetDefaultPriceListId = q.SetDefaultPriceListId,
                            SetDefaultSalesType = q.SetDefaultSalesType,
                            Type = q.Type,
                            PageId = q.PageId,
                            Color = q.Color,
                            Background = q.Background,
                            KdsId = q.KdsId,
                            Code = q.Code,
                            KitchenId = q.KitchenId,
                            KitchenCode = q.KitchenCode,
                            ItemRegion = q.ItemRegion,
                            RegionPosition = q.RegionPosition,
                            ItemRegionAbbr = q.ItemRegionAbbr,
                            PricelistDetails = q.PricelistDetails,//as IEnumerable<PricelistDetail>,
                            ProductCategoryId = q.ProductCategoryId,

                            Detail_IngredientId = q.Detail_IngredientId,
                            Detail_Description = q.Detail_Description,
                            Detail_Background = q.Detail_Background,
                            Detail_Color = q.Detail_Color,
                            Detail_MaxQty = q.Detail_MaxQty,
                            Detail_MinQty = q.Detail_MinQty,
                            Detail_Type = q.Detail_Type,
                            Detail_Price = q.Detail_Price,
                            Detail_Qty = q.Detail_Qty,
                            Detail_IngredientCategoryId = q.Detail_IngredientCategoryId,
                            Detail_PricelistDetails = fff.AsEnumerable<PricelistDetail>(),
                            Detail_Sort = q.Detail_Sort,
                            DA_ProductId = q.DA_ProductId,
                            DA_ProductCategoryId = q.DA_ProductCategoryId
                        };

            var buttonWithProducts = (join2.ToList().Distinct().GroupBy(g => new { g.ProductId, g.Id }).ToList().Select(s => new {
                Id = s.Key.Id,

                PageSetId = s.FirstOrDefault().PageSetId,
                ProductId = s.Key.ProductId,
                Description = s.FirstOrDefault().Description,
                ExtraDescription = s.FirstOrDefault().ExtraDescription,
                SalesDescription = s.FirstOrDefault().SalesDescription,
                PreparationTime = Convert.ToInt16(s.FirstOrDefault().PreparationTime),
                Sort = s.FirstOrDefault().Sort,
                NavigateToPage = s.FirstOrDefault().NavigateToPage,
                SetDefaultPriceListId = s.FirstOrDefault().SetDefaultPriceListId,
                SetDefaultSalesType = s.FirstOrDefault().SetDefaultSalesType,
                Type = s.FirstOrDefault().Type,
                PageId = s.FirstOrDefault().PageId,
                Color = s.FirstOrDefault().Color,
                Background = s.FirstOrDefault().Background,
                KdsId = s.FirstOrDefault().KdsId,
                Code = s.FirstOrDefault().Code,
                KitchenId = s.FirstOrDefault().KitchenId,
                KitchenCode = s.FirstOrDefault().KitchenCode,
                ItemRegion = s.FirstOrDefault().ItemRegion,
                RegionPosition = s.FirstOrDefault().RegionPosition,
                ItemRegionAbbr = s.FirstOrDefault().ItemRegionAbbr,
                PricelistDetails = s.FirstOrDefault().PricelistDetails,
                ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                DA_ProductId = s.FirstOrDefault().DA_ProductId,
                DA_ProductCategoryId = s.FirstOrDefault().DA_ProductCategoryId,
                PageButtonDetail = s.Distinct().Where(ww => ww.Detail_IngredientId != null).Select((ss, index) => new {
                    PageButtonId = s.FirstOrDefault().Id,
                    ProductId = ss.Detail_IngredientId,
                    Description = ss.Detail_Description,
                    Background = ss.Detail_Background,
                    Color = ss.Detail_Color,
                    MaxQty = ss.Detail_MaxQty,
                    MinQty = ss.Detail_MinQty,
                    Type = ss.Detail_Type,
                    Qty = ss.Detail_Qty,
                    PricelistDetails = ss.Detail_PricelistDetails,
                    plusEnabled = !(ss.Detail_MaxQty == 1 && ss.Detail_Qty == 1),
                    minusEnabled = !(ss.Detail_MinQty == 1 && ss.Detail_Qty == 1),
                    IngredientCategoryId = ss.Detail_IngredientCategoryId,
                    Sort = ss.Detail_Sort,
                }).OrderBy(oo => oo.Sort).Distinct()
                .ToList()
            })).OrderBy(o => o.Sort).AsEnumerable();

            return buttonWithProducts;
        }

        public IEnumerable<Object> GetPageButtonsCreatorPos(List<dynamic> buttonWithProductsTemp, int pricelistid, PosEntities db)
        {
            ///Select Display Extras from Product Recipes
            var precs = (from q in db.ProductRecipe.Include("Product")
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
                             Qty = q.Qty,
                             Type = ProductDetailTypeEnum.Recipe, //byte of type 0 : indicates that extra is on Recipe //Extras of this type can be excluded from ordered Item
                             Price = q.Price,
                             IngredientCategoryId = q.Ingredients.IngredientCategoryId,
                             Sort = (long?)q.Sort,
                         }).OrderBy(z => z.Sort).ToList<dynamic>();
            ///Select Display POS Extras from Product Extras
            var pextras = (from q in db.ProductExtras.Include("Product").Include("Product.PricelistDetail")
                           select new
                           {
                               ProductId = q.ProductId,
                               IngredientId = q.IngredientId,
                               Description = q.Ingredients.SalesDescription,
                               Background = q.Ingredients.Background,
                               Color = q.Ingredients.Color,
                               MaxQty = q.MaxQty,
                               MinQty = q.MinQty,
                               Qty = (double?)0,
                               Type = ProductDetailTypeEnum.Extras,
                               Price = q.Price,
                               IngredientCategoryId = q.Ingredients.IngredientCategoryId,
                               Sort = (long?)q.Sort,
                               CanSavePrice = q.CanSavePrice,
                           }).OrderBy(z => z.Sort).ToList<dynamic>();
            ///Select Display POS Extras from Product Extras
            var assocs = (from q in db.Ingredient_ProdCategoryAssoc
                          join qq in db.Product on q.ProductCategoryId equals qq.ProductCategoryId
                          join qqq in db.Ingredients on q.IngredientId equals qqq.Id
                          join qqqq in db.PricelistDetail on q.IngredientId equals qqqq.IngredientId
                          select new
                          {
                              ProductId = (long?)qq.Id,
                              IngredientId = q.IngredientId,
                              Description = qqq.SalesDescription,
                              Background = qqq.Background,
                              Color = qqq.Color,
                              MaxQty = q.MaxQty,
                              MinQty = q.MinQty,
                              Qty = (double?)0,
                              Type = ProductDetailTypeEnum.ExtrasAssoc,
                              Price = qqqq.Price,
                              IngredientCategoryId = qqq.IngredientCategoryId,
                              Sort = (long?)q.Sort,
                              CanSavePrice = q.CanSavePrice,
                          }).OrderBy(z => z.Sort).ToList<dynamic>();

            List<dynamic> res = precs;
            res.AddRange(pextras);
            res.AddRange(assocs);
            List<dynamic> buttonDetailsTemp = res;//.GroupBy(x => x.IngredientId).Select(x => x.First());

            ///buttonWithProductsTemp : Dynamic PageButtons Created from Caller
            var buttonWithProductsflat = (from q in buttonWithProductsTemp
                                          join q1 in buttonDetailsTemp on q.ProductId equals q1.ProductId into ff
                                          from q1 in ff.DefaultIfEmpty()
                                          select new
                                          {
                                              Id = q.Id,
                                              PageSetId = q.PageSetId,
                                              ProductId = q.ProductId,
                                              Description = q.Description,
                                              ExtraDescription = q.ExtraDescription,
                                              SalesDescription = q.SalesDescription,
                                              PreparationTime = q.PreparationTime,
                                              Sort = q.Sort,
                                              NavigateToPage = q.NavigateToPage,
                                              SetDefaultPriceListId = q.SetDefaultPriceListId,
                                              SetDefaultSalesType = q.SetDefaultSalesType,
                                              Type = q.Type,
                                              PageId = q.PageId,
                                              Color = q.Color,
                                              Background = q.Background,
                                              KdsId = q.KdsId,
                                              Code = q.Code,
                                              KitchenId = q.KitchenId,
                                              KitchenCode = q.KitchenCode,
                                              ItemRegion = q.ItemRegion,
                                              RegionPosition = q.RegionPosition,
                                              ItemRegionAbbr = q.ItemRegionAbbr,
                                              PricelistDetails = q.PricelistDetails, // as IEnumerable<PricelistDetail>,
                                              ProductCategoryId = q.ProductCategoryId,
                                              IsCombo = q.IsCombo,
                                              IsComboItem = q.IsComboItem,
                                              IsReturnItem = q.IsReturnItem,
                                              CanSavePrice = q.CanSavePrice,
                                              Detail_IngredientId = q1 != null ? q1.IngredientId : null,
                                              Detail_Description = q1 != null ? q1.Description : null,
                                              Detail_Background = q1 != null ? q1.Background : null,
                                              Detail_Color = q1 != null ? q1.Color : null,
                                              Detail_MaxQty = q1 != null ? q1.MaxQty : null,
                                              Detail_MinQty = q1 != null ? q1.MinQty : null,
                                              Detail_Type = q1 == null || q1.Type == null ? null : q1.Type,
                                              Detail_Price = q1 != null ? q1.Price : null,
                                              Detail_Qty = q1 != null ? q1.Qty : null,
                                              Detail_IngredientCategoryId = q1 != null ? q1.IngredientCategoryId : null,
                                              Detail_Sort = q1 != null ? q1.Sort : null,
                                              DA_ProductId = q.DA_ProductId,
                                              DA_ProductCategoryId = q.DA_ProductCategoryId
                                          });

            var join2 = from q in buttonWithProductsflat
                        join q1 in db.PricelistDetail.AsNoTracking().Include("Vat").Where(w => w.IngredientId != null) on q.Detail_IngredientId equals q1.IngredientId into fff
                        from q1 in fff.DefaultIfEmpty()
                        select new
                        {
                            Id = q.Id,
                            PageSetId = q.PageSetId,
                            ProductId = q.ProductId,
                            Description = q.Description,
                            ExtraDescription = q.ExtraDescription,
                            SalesDescription = q.SalesDescription,
                            PreparationTime = q.PreparationTime,
                            Sort = q.Sort,
                            NavigateToPage = q.NavigateToPage,
                            SetDefaultPriceListId = q.SetDefaultPriceListId,
                            SetDefaultSalesType = q.SetDefaultSalesType,
                            Type = q.Type,
                            PageId = q.PageId,
                            Color = q.Color,
                            Background = q.Background,
                            KdsId = q.KdsId,
                            Code = q.Code,
                            KitchenId = q.KitchenId,
                            KitchenCode = q.KitchenCode,
                            ItemRegion = q.ItemRegion,
                            RegionPosition = q.RegionPosition,
                            ItemRegionAbbr = q.ItemRegionAbbr,
                            PricelistDetails = q.PricelistDetails,//as IEnumerable<PricelistDetail>,
                            ProductCategoryId = q.ProductCategoryId,

                            Detail_IngredientId = q.Detail_IngredientId,
                            Detail_Description = q.Detail_Description,
                            Detail_Background = q.Detail_Background,
                            Detail_Color = q.Detail_Color,
                            Detail_MaxQty = q.Detail_MaxQty,
                            Detail_MinQty = q.Detail_MinQty,
                            Detail_Type = q.Detail_Type,
                            Detail_Price = q.Detail_Price,
                            Detail_Qty = q.Detail_Qty,
                            Detail_IngredientCategoryId = q.Detail_IngredientCategoryId,
                            Detail_PricelistDetails = fff.AsEnumerable<PricelistDetail>(),
                            Detail_Sort = q.Detail_Sort,
                            IsCombo = q.IsCombo,
                            IsComboItem = q.IsComboItem,
                            IsReturnItem = q.IsReturnItem,
                            CanSavePrice = q.CanSavePrice,
                            DA_ProductId = q.DA_ProductId,
                            DA_ProductCategoryId = q.DA_ProductCategoryId
                        };

            var buttonWithProducts = (join2.ToList().Distinct().GroupBy(g => new { g.ProductId, g.Id }).ToList().Select(s => new {
                Id = s.Key.Id,

                PageSetId = s.FirstOrDefault().PageSetId,
                ProductId = s.Key.ProductId,
                Description = s.FirstOrDefault().Description,
                ExtraDescription = s.FirstOrDefault().ExtraDescription,
                SalesDescription = s.FirstOrDefault().SalesDescription,
                PreparationTime = Convert.ToInt16(s.FirstOrDefault().PreparationTime),
                Sort = s.FirstOrDefault().Sort,
                NavigateToPage = s.FirstOrDefault().NavigateToPage,
                SetDefaultPriceListId = s.FirstOrDefault().SetDefaultPriceListId,
                SetDefaultSalesType = s.FirstOrDefault().SetDefaultSalesType,
                Type = s.FirstOrDefault().Type,
                PageId = s.FirstOrDefault().PageId,
                Color = s.FirstOrDefault().Color,
                Background = s.FirstOrDefault().Background,
                KdsId = s.FirstOrDefault().KdsId,
                Code = s.FirstOrDefault().Code,
                KitchenId = s.FirstOrDefault().KitchenId,
                KitchenCode = s.FirstOrDefault().KitchenCode,
                ItemRegion = s.FirstOrDefault().ItemRegion,
                RegionPosition = s.FirstOrDefault().RegionPosition,
                ItemRegionAbbr = s.FirstOrDefault().ItemRegionAbbr,
                PricelistDetails = s.FirstOrDefault().PricelistDetails,
                ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                IsCombo = s.FirstOrDefault().IsCombo,
                IsComboItem = s.FirstOrDefault().IsComboItem,
                IsReturnItem = s.FirstOrDefault().IsReturnItem,
                CanSavePrice = s.FirstOrDefault().CanSavePrice,
                DA_ProductId = s.FirstOrDefault().DA_ProductId,
                DA_ProductCategoryId = s.FirstOrDefault().DA_ProductCategoryId,
                PageButtonDetail = s.Distinct().Where(ww => ww.Detail_IngredientId != null).Select((ss, index) => new {
                    PageButtonId = s.FirstOrDefault().Id,
                    ProductId = ss.Detail_IngredientId,
                    IngredientId = ss.Detail_IngredientId,
                    Description = ss.Detail_Description,
                    Background = ss.Detail_Background,
                    Color = ss.Detail_Color,
                    MaxQty = ss.Detail_MaxQty,
                    MinQty = ss.Detail_MinQty,
                    CanSavePrice = s.FirstOrDefault().CanSavePrice,
                    Type = ss.Detail_Type,
                    Qty = ss.Detail_Qty,
                    PricelistDetails = ss.Detail_PricelistDetails,
                    plusEnabled = !(ss.Detail_MaxQty == 1 && ss.Detail_Qty == 1),
                    minusEnabled = !(ss.Detail_MinQty == 1 && ss.Detail_Qty == 1),
                    IngredientCategoryId = ss.Detail_IngredientCategoryId,
                    Sort = ss.Detail_Sort,
                }).OrderBy(oo => oo.Sort).Distinct()
                .ToList()
            })).OrderBy(o => o.Sort).AsEnumerable();

            return buttonWithProducts;
        }
    }
}