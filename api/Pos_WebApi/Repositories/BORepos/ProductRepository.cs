using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using Pos_WebApi.Models.DTOModels;
using System.Linq.Dynamic;
using LinqKit;
using Pos_WebApi.Helpers;
using System.Text;
using log4net;
using System.Web.Configuration;
using Symposium.Helpers;

namespace Pos_WebApi.Repositories.BORepos {

    public class ProductRepository //: IProductRepository
    {

        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        bool uniqueCodePolicy;

        public ProductRepository(PosEntities db) {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            uniqueCodePolicy = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "BIPolicy.Product.UniqueCode");
        }

        public IQueryable<ProductDTO> GetAll(Expression<Func<ProductDTO, bool>> predicate = null, bool usedelete = false) {
            IQueryable<ProductDTO> query = DbContext.Product.Where(w => (w.IsDeleted ?? false) == usedelete)
                                                    //.Include(i => i.ProductExtras).Include(i => i.ProductRecipe).Include(i => i.ProductBarcodes)
                                                    .Select(s => new ProductDTO {
                                                        Id = s.Id,
                                                        Description = s.Description,
                                                        ExtraDescription = s.ExtraDescription,
                                                        Qty = s.Qty,
                                                        UnitId = s.UnitId,
                                                        SalesDescription = s.SalesDescription,
                                                        PreparationTime = s.PreparationTime,
                                                        KdsId = s.KdsId,
                                                        KitchenId = s.KitchenId,
                                                        ImageUri = s.ImageUri,
                                                        ProductCategoryId = s.ProductCategoryId,
                                                        Code = s.Code,
                                                        IsReturnItem = s.IsReturnItem,
                                                        IsCustom = s.IsCustom,
                                                        KitchenRegionId = s.KitchenRegionId,
                                                        IsDeleted = s.IsDeleted,
                                                        //ProductRecipe = s.ProductExtras.Select(ss => new ProductReceipeDTO
                                                        //{
                                                        //    Id = ss.Id,
                                                        //    ProductId = ss.ProductId,
                                                        //    UnitId = ss.UnitId,
                                                        //    //Qty = ss.Qty,
                                                        //    Price = ss.Price,
                                                        //    //   IsProduct = ss.IsProduct,
                                                        //    MinQty = ss.MinQty,
                                                        //    MaxQty = ss.MaxQty,
                                                        //    IngredientId = ss.IngredientId,
                                                        //    ItemsId = ss.ItemsId,
                                                        //    ProductAsIngridientId = ss.ProductAsIngridientId,
                                                        //    // DefaultQty = ss.DefaultQty,
                                                        //    Sort = ss.Sort
                                                        //}).ToList(),
                                                        //ProductExtras = s.ProductExtras.Select(ss => new ProductExtrasDTO
                                                        //{
                                                        //    Id = ss.Id,
                                                        //    ProductId = ss.ProductId,
                                                        //    IsRequired = ss.IsRequired,
                                                        //    IngredientId = ss.IngredientId,
                                                        //    MinQty = ss.MinQty,
                                                        //    MaxQty = ss.MaxQty,
                                                        //    UnitId = ss.UnitId,
                                                        //    ItemsId = ss.ItemsId,
                                                        //    Price = ss.Price,
                                                        //    ProductAsIngridientId = ss.ProductAsIngridientId,
                                                        //    Sort = ss.Sort
                                                        //}).ToList(),
                                                        //ProductBarcodes = s.ProductBarcodes.Select(ss => new ProductBarcodeDTO
                                                        //{
                                                        //    Id = ss.Id,
                                                        //    Barcode = ss.Barcode,
                                                        //    ProductId = ss.ProductId,
                                                        //    Type = ss.Type
                                                        //}).ToList(),
                                                    });
            if (predicate != null)
                return query.Where(predicate);
            else
                return query;
            //IQueryable<Product> query = s.DbContext.Set<Product>().Where(w => w.IsDeleted == null || w.IsDeleted == false);
            //return query;
        }

        public IQueryable<Product> GetByBarcode(Expression<Func<Product, bool>> predicate = null, string barcode = "") {
            var query = (from q in DbContext.ProductBarcodes.Where(w => w.Barcode.Equals(barcode))
                         join qq in this.GetAll() on q.ProductId equals qq.Id
                         select q.Product)
                                    .Include(i => i.PricelistDetail)
                                    .Include(i => i.PricelistDetail.Select(y => y.Vat)).Distinct();

            if (predicate != null)
                return query.Where(predicate);
            else
                return query;
        }

        public Object GetPageButtonFromProductBC(int pricelistid, long posDepId, string barcodesearch,bool isPos=false)
        {// bool aba = false
            var productId = DbContext.ProductBarcodes.Where(w => w.Barcode.Trim() == (barcodesearch.Trim())).Select(s => s.ProductId).FirstOrDefault();
            //if (productIdList = null)

            var result = DbContext.Product.AsNoTracking()
                                   .Include(i => i.PageButton)
                                   .Include(i => i.PageButton.Select(ss => ss.Pages))
                                   .Include(i => i.PricelistDetail)
                                   .Include(i => i.PricelistDetail.Select(y => y.Vat))
                                   .Include(i => i.Kitchen)
                                   .Include(i => i.KitchenRegion)
                                   .Where(w => productId == w.Id);
            var buttonWithProductsTemp = from q in result
                                             //join qq in db.PricelistDetail.Where(w => w.PricelistId == pricelistid) on q.ProductId equals qq.ProductId
                                             //into ff
                                             //from q1 in ff.DefaultIfEmpty()
                                         select new {
                                             Id = /*q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Id.ToString() : */q.Code,
                                             MappedPriceLists = q.TransferMappings.Where(t => t.PosDepartmentId == posDepId).Select(f => f.PriceListId),
                                             ProductId = q.Id,
                                             Description = q.Description,
                                             PreparationTime = q.PreparationTime != null ? q.PreparationTime : 0,
                                             //    Price = q.PricelistDetail.FirstOrDefault(x => x.Id == pricelistid) != null ? q.PricelistDetail.FirstOrDefault(x => x.Id == pricelistid).Price:null,
                                             Sort = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Sort : 0,
                                             NavigateToPage = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().NavigateToPage : null,
                                             SetDefaultPriceListId = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().SetDefaultPriceListId : null,
                                             SetDefaultSalesType = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().SetDefaultSalesType : null,
                                             Type = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Type : null,
                                             PageSetId = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Pages.PageSetId : null,
                                             PageId = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().PageId : null,
                                             Color = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Color : null,
                                             Background = q.PageButton.FirstOrDefault() != null ? q.PageButton.FirstOrDefault().Background : null,
                                             PricelistId = q.PricelistDetail.FirstOrDefault(x => x.Id == pricelistid),
                                             ProductCategoryId = q.ProductCategoryId,
                                             KdsId = q.KdsId,
                                             Code = q.Code,
                                             KitchenCode = q.Kitchen.Code,
                                             ItemRegion = q.KitchenRegion.ItemRegion,
                                             RegionPosition = q.KitchenRegion.RegionPosition,
                                             ItemRegionAbbr = q.KitchenRegion.Abbr,
                                             //   VatId = q.PricelistDetail.FirstOrDefault(x => x.Id == pricelistid) != null ? q.PricelistDetail.FirstOrDefault(x => x.Id == pricelistid).VatId : null,
                                             //    Vat = q.PricelistDetail.FirstOrDefault(x => x.Id == pricelistid) != null ? q.PricelistDetail.FirstOrDefault(x => x.Id == pricelistid).Vat.Percentage : null,
                                             //    VatCode = q.PricelistDetail.FirstOrDefault(x => x.Id == pricelistid) != null ? q.PricelistDetail.FirstOrDefault(x => x.Id == pricelistid).Vat.Code : null,
                                             KitchenId = q.KitchenId,
                                             //  PriceListDetailId = q.PricelistDetail.FirstOrDefault(x => x.Id == pricelistid).Id,
                                             PricelistDetails = q.PricelistDetail.Select(ss => new {
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
                buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPos(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, DbContext);
            else
                buttonWithProducts = new PageButtonCreator().GetPageButtonsCreatorPda(buttonWithProductsTemp.ToList<dynamic>(), pricelistid, DbContext);

            return buttonWithProducts.FirstOrDefault();
        }



        public PagedResult<ProductDTO> GetPaged(Expression<Func<ProductDTO, bool>> predicate, Expression<Func<ProductDTO, string>> sortpredicate, int page = 1, int pageSize = 10, bool usedelete = false) {
            var query = this.GetAll(predicate, usedelete).AsExpandable();
            // var query = this.DbContext.Product.Where(w => w.IsDeleted == null || w.IsDeleted == false).Where(predicate).AsExpandable().OrderBy(sortpredicate);

            var result = new PagedResult<ProductDTO>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            var pageCount = (pageSize == -1) ? (double)result.RowCount : (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            if (skip < 0)
                skip = 0;
            if (pageSize > 0)
                result.Results = query.OrderBy(sortpredicate).Skip(skip).Take(pageSize).ToList();
            else
                result.Results = query.OrderBy(sortpredicate).Skip(skip).ToList();

            return result;
        }


        public IQueryable<Product> Include(Expression<Func<Product, object>> predicate) {
            IQueryable<Product> query = this.DbContext.Set<Product>().Include(predicate).Where(w => w.IsDeleted == null || w.IsDeleted == false);
            return query;
        }

        public IQueryable<Product> FindBy(Expression<Func<Product, bool>> predicate) {
            IQueryable<Product> query = this.DbContext.Set<Product>().Where(w => w.IsDeleted == null || w.IsDeleted == false).Where(predicate);
            return query;
        }
        public IQueryable<ProductDTO> FindByDetailed(long id) {
            IQueryable<ProductDTO> query = DbContext.Product.Where(w => (w.IsDeleted ?? false) == false && w.Id == id)
                                                    .Include(i => i.ProductExtras)
                                                    .Include(i => i.ProductRecipe)
                                                    .Include(i => i.ProductBarcodes)
                                                    .Select(s => new ProductDTO {
                                                        Id = s.Id,
                                                        Description = s.Description,
                                                        ExtraDescription = s.ExtraDescription,
                                                        Qty = s.Qty,
                                                        UnitId = s.UnitId,
                                                        SalesDescription = s.SalesDescription,
                                                        PreparationTime = s.PreparationTime,
                                                        KdsId = s.KdsId,
                                                        KitchenId = s.KitchenId,
                                                        ImageUri = s.ImageUri,
                                                        ProductCategoryId = s.ProductCategoryId,
                                                        Code = s.Code,
                                                        IsReturnItem = s.IsReturnItem,
                                                        IsCustom = s.IsCustom,
                                                        KitchenRegionId = s.KitchenRegionId,
                                                        IsDeleted = s.IsDeleted,
                                                        ProductRecipe = s.ProductRecipe.Select(ss => new ProductReceipeDTO {
                                                            Id = ss.Id, ProductId = ss.ProductId, UnitId = ss.UnitId, Qty = ss.Qty, Price = ss.Price,
                                                            IsProduct = ss.IsProduct, MinQty = ss.MinQty, MaxQty = ss.MaxQty,
                                                            IngredientId = ss.IngredientId, ItemsId = ss.ItemsId, ProductAsIngridientId = ss.ProductAsIngridientId,
                                                            DefaultQty = ss.DefaultQty, Sort = ss.Sort, IsDeleted = false
                                                        }).OrderBy(ss => ss.Sort).ToList(),
                                                        ProductExtras = s.ProductExtras.Select(ss => new ProductExtrasDTO {
                                                            Id = ss.Id, ProductId = ss.ProductId, IsRequired = ss.IsRequired, IngredientId = ss.IngredientId,
                                                            MinQty = ss.MinQty, MaxQty = ss.MaxQty, UnitId = ss.UnitId, ItemsId = ss.ItemsId,
                                                            Price = ss.Price, ProductAsIngridientId = ss.ProductAsIngridientId, Sort = ss.Sort, IsDeleted = false
                                                        }).OrderBy(ss => ss.Sort).ToList(),
                                                        ProductBarcodes = s.ProductBarcodes.Select(ss => new ProductBarcodeDTO {
                                                            Id = ss.Id, Barcode = ss.Barcode, ProductId = ss.ProductId, Type = ss.Type, IsDeleted = false
                                                        }).ToList(),
                                                        ProductPrices = s.PricelistDetail.Select(ss => new ProductPricesDTO {
                                                            Id = ss.Id,
                                                            PricelistId = ss.PricelistId,
                                                            ProductId = ss.ProductId,
                                                            Price = ss.Price,
                                                            VatId = ss.VatId,
                                                            TaxId = ss.TaxId
                                                        }).ToList(),
                                                    });
            return query;
        }

        public ProductDTO Add(ProductDTO model) {
            var entity = model.ToModel();
            if (!checkUniqueCode(model))
                return null;
            DbContext.Product.Add(entity);
            DbContext.SaveChanges();
            model.Id = entity.Id;
            return model;
        }


        public List<ProductDTO> AddRange(IEnumerable<ProductDTO> entities) {
            List<Product> modlist = new List<Product>();
            foreach (var model in entities) {
                //this.Add(entity);
                var entity = model.ToModel();
                if (!checkUniqueCode(model))
                    return null;
                DbContext.Product.Add(entity);

                modlist.Add(entity);
            }
            DbContext.SaveChanges();

            List<ProductDTO> retlist = new List<ProductDTO>();
            foreach (var item in modlist) {
                try {
                    ProductDTO ni = new ProductDTO().ModelToDTO(item);
                    retlist.Add(ni);
                } catch (Exception) {

                }
            }
            return retlist;
        }



        public ProductDTO Update(ProductDTO model) {
            var entity = DbContext.Product
                               //.Include(i => i.ProductRecipe)
                               //.Include(i => i.ProductExtras)
                               //.Include(i => i.ProductBarcodes)
                               .FirstOrDefault(x => x.Id == model.Id);
            if (!checkUniqueCode(model))
                return null;

            entity = model.UpdateModel(entity);
            #region Update Insert Section
            foreach (var det in model.ProductRecipe.Where(w => w.IsDeleted == false)) {
                if (det.Id != 0) {
                    var m = DbContext.ProductRecipe.FirstOrDefault(x => x.Id == det.Id);
                    if (m != null) {
                        DbContext.Entry(det.UpdateModel(m)).State = EntityState.Modified;
                        //   DbContext.Entry(det.ToModel()).State = EntityState.Modified;
                    }
                } else {
                    DbContext.Entry(det.ToModel()).State = EntityState.Added;
                }
            }

            foreach (var det in model.ProductBarcodes.Where(w => w.IsDeleted == false)) {
                if (det.Id != 0) {
                    var m = DbContext.ProductBarcodes.FirstOrDefault(x => x.Id == det.Id);
                    if (m != null) {
                        DbContext.Entry(det.UpdateModel(m)).State = EntityState.Modified;
                        //   DbContext.Entry(det.ToModel()).State = EntityState.Modified;
                    }
                    //     DbContext.Entry(det.ToModel()).State = EntityState.Modified;
                } else {
                    DbContext.Entry(det.ToModel()).State = EntityState.Added;
                }
            }

            foreach (var det in model.ProductExtras.Where(w => w.IsDeleted == false)) {
                if (det.Id != 0) {
                    var m = DbContext.ProductExtras.FirstOrDefault(x => x.Id == det.Id);
                    if (m != null) {
                        DbContext.Entry(det.UpdateModel(m)).State = EntityState.Modified;
                        //   DbContext.Entry(det.ToModel()).State = EntityState.Modified;
                    }
                    //    DbContext.Entry(det.ToModel()).State = EntityState.Modified;
                } else {
                    DbContext.Entry(det.ToModel()).State = EntityState.Added;
                }
            }


            foreach (var det in model.ProductPrices.Where(w => w.IsDeleted == false)) {
                if (det.Id != 0) {
                    var m = DbContext.PricelistDetail.FirstOrDefault(x => x.Id == det.Id);
                    if (m != null) {
                        DbContext.Entry(det.UpdateModel(m)).State = EntityState.Modified;
                        //   DbContext.Entry(det.ToModel()).State = EntityState.Modified;
                    }
                    //    DbContext.Entry(det.ToModel()).State = EntityState.Modified;
                } else {
                    DbContext.Entry(det.ToModel()).State = EntityState.Added;
                }
            }

            #endregion
            #region Delete Section
            var extrasToDelete = model.ProductExtras.Where(w => w.IsDeleted == true);
            foreach (var btn in extrasToDelete) {
                var b = DbContext.ProductExtras.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.ProductExtras.Remove(b);
            }

            var receipeToDelete = model.ProductRecipe.Where(w => w.IsDeleted == true);
            foreach (var btn in receipeToDelete) {
                var b = DbContext.ProductRecipe.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.ProductRecipe.Remove(b);
            }

            var barcodesToDelete = model.ProductBarcodes.Where(w => w.IsDeleted == true);
            foreach (var btn in barcodesToDelete) {
                var b = DbContext.ProductBarcodes.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.ProductBarcodes.Remove(b);
            }

            var pricesToDelete = model.ProductPrices.Where(w => w.IsDeleted == true);
            foreach (var btn in pricesToDelete) {
                var b = DbContext.PricelistDetail.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.PricelistDetail.Remove(b);
            }
            #endregion

            DbContext.Entry(entity).State = EntityState.Modified;
            model.Id = entity.Id;
            return model;
        }


        public IEnumerable<ProductDTO> UpdateRange(IEnumerable<ProductDTO> entities) {
            foreach (var entity in entities) {
                this.Update(entity);
            }
            return entities;
        }

        public List<long> RestoreIds(List<long> ids) {
            try {
                foreach (long pid in ids) {
                    Product prod = DbContext.Product.Where(w => w.Id == pid).FirstOrDefault();
                    prod.IsDeleted = false;
                    DbContext.Entry(prod).State = EntityState.Modified;
                }
                return ids;
            } catch (Exception e) {
                return null;
            }
        }

        public bool Delete(long? id) {
            var testProdConnection = DbContext.OrderDetail.FirstOrDefault(x => x.ProductId == id);
            var entitytoRemove = DbContext.Set<Product>()
                                          .Include(i => i.ProductBarcodes)
                                          .Include(i => i.ProductExtras)
                                          .Include(i => i.ProductRecipe)
                                          .Where(w => w.Id == id).FirstOrDefault();
            if (testProdConnection == null && entitytoRemove != null) {
                try {

                    foreach (var item in DbContext.ProductBarcodes.Where(w => w.ProductId == entitytoRemove.Id).ToList()) {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in DbContext.ProductExtras.Where(w => w.ProductId == entitytoRemove.Id).ToList()) {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in DbContext.ProductRecipe.Where(w => w.ProductId == entitytoRemove.Id).ToList()) {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in DbContext.ProductForBarcodeEod.Where(w => w.ProductId == entitytoRemove.Id).ToList()) {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in DbContext.PageButton.Where(w => w.ProductId == entitytoRemove.Id).ToList()) {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in DbContext.BoardMeals.Where(w => w.ProductId == entitytoRemove.Id).ToList()) {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in DbContext.PricelistDetail.Where(w => w.ProductId == entitytoRemove.Id).ToList()) {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }

                    foreach (var item in DbContext.TransferMappings.Where(w => w.ProductId == entitytoRemove.Id).ToList()) {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    //RegionLockerProduct Mapping on delete product
                    foreach (var item in DbContext.RegionLockerProduct.Where(w => w.ProductId == entitytoRemove.Id).ToList()) {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    //ExternalProduct Mapping on delete product
                    foreach (var item in DbContext.ExternalProductMapping.Where(w => w.ProductId == entitytoRemove.Id).ToList()) {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }

                    //while (entitytoRemove.ProductBarcodes.Count > 0){    DbContext.Set<ProductBarcodes>().Remove(entitytoRemove.ProductBarcodes.FirstOrDefault());}
                    //while (entitytoRemove.ProductExtras.Count > 0){    DbContext.Set<ProductExtras>().Remove(entitytoRemove.ProductExtras.FirstOrDefault());}
                    //while (entitytoRemove.ProductRecipe.Count > 0){    DbContext.Set<ProductRecipe>().Remove(entitytoRemove.ProductRecipe.FirstOrDefault());}
                    //while (entitytoRemove.ProductForBarcodeEod.Count > 0){    DbContext.Set<ProductForBarcodeEod>().Remove(entitytoRemove.ProductForBarcodeEod.FirstOrDefault());}
                    //while (entitytoRemove.RegionLockerProduct.Count > 0){    DbContext.Set<RegionLockerProduct>().Remove(entitytoRemove.RegionLockerProduct.FirstOrDefault());}
                    //while (entitytoRemove.PageButton.Count > 0){   DbContext.Set<PageButton>().Remove(entitytoRemove.PageButton.FirstOrDefault());}
                    //while (entitytoRemove.BoardMeals.Count > 0){DbContext.Set<BoardMeals>().Remove(entitytoRemove.BoardMeals.FirstOrDefault());}
                    //while (entitytoRemove.PricelistDetail.Count > 0){DbContext.Set<PricelistDetail>().Remove(entitytoRemove.PricelistDetail.FirstOrDefault());}

                    //if (entitytoRemove.ProductBarcodes.Count > 0){DbContext.ProductBarcodes.RemoveRange(entitytoRemove.ProductBarcodes);}
                    //if (entitytoRemove.ProductExtras.Count > 0){DbContext.ProductExtras.RemoveRange(entitytoRemove.ProductExtras);}
                    //if (entitytoRemove.ProductRecipe.Count > 0){DbContext.ProductRecipe.RemoveRange(entitytoRemove.ProductRecipe);}
                    //if (entitytoRemove.ProductForBarcodeEod.Count > 0){DbContext.ProductForBarcodeEod.RemoveRange(entitytoRemove.ProductForBarcodeEod);}
                    //if (entitytoRemove.RegionLockerProduct.Count > 0){DbContext.RegionLockerProduct.RemoveRange(entitytoRemove.RegionLockerProduct);}
                    //if (entitytoRemove.PageButton.Count > 0){DbContext.PageButton.RemoveRange(entitytoRemove.PageButton);}
                    //if (entitytoRemove.BoardMeals.Count > 0){DbContext.BoardMeals.RemoveRange(entitytoRemove.BoardMeals);}
                    //if (entitytoRemove.PricelistDetail.Count > 0){DbContext.PricelistDetail.RemoveRange(entitytoRemove.PricelistDetail);/}


                    DbContext.Set<Product>().Remove(entitytoRemove);
                    return true;
                } catch (Exception ex) {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting Products : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    logger.Error(ex.ToString());
                    entitytoRemove.IsDeleted = true;
                    DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                    return true;
                }
            } else {
                //foreach (var pb in entitytoRemove.ProductBarcodes) { DbContext.Entry(pb).State = EntityState.Modified; }
                //foreach (var ex in entitytoRemove.ProductExtras) { DbContext.Entry(ex).State = EntityState.Modified; }
                //foreach (var pe in entitytoRemove.ProductRecipe) { DbContext.Entry(pe).State = EntityState.Modified; }
                try {
                    entitytoRemove.IsDeleted = true;
                    DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                    return true;
                } catch (Exception ex) {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Marking as Deleting Products : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    logger.Error(ex.ToString());
                    return false;
                }

            }
        }

        public IEnumerable<long> DeleteRange(IEnumerable<long> ids) {
            foreach (var id in ids) {
                this.Delete(id);
            }
            return ids;
        }
        //public IEnumerable<Product> DeleteRange(IEnumerable<Product> entities) {
        //    foreach (var entity in entities) {
        //        DbContext.Set<Product>().Remove(entity);
        //    }
        //    return entities;
        //}

        public bool RemoveDuplicatesProductExtras() {
            try {
                var doublePrices = DbContext.ProductExtras.ToList().GroupBy(g => new { g.ProductId, g.IngredientId })
                                          .Where(w => w.Count() > 1)
                                          .Select(s => new {
                                              KeyToKeep = s.FirstOrDefault().Id,
                                              KeysToChange = s.Select(ss => ss.Id)
                                          });
                StringBuilder sb = new StringBuilder();
                foreach (var keyToFixed in doublePrices) {
                    var invalidKeys = keyToFixed.KeysToChange.Where(w => w != keyToFixed.KeyToKeep);
                    var odisToFixed = DbContext.OrderDetail.Where(w => invalidKeys.Contains(w.PriceListDetailId.Value));


                    var str = invalidKeys.Distinct().Aggregate("", (previous, next) => previous + ", " + next).Trim().Remove(0, 1);
                    sb.AppendLine(string.Format("Delete from ProductExtras where id in ({0})", str));
                }
                sb.AppendLine(string.Format("Update pricelistDetail set Price= 0 where price is null"));
                DbContext.Database.ExecuteSqlCommand(sb.ToString());
            } catch (Exception ex) {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error removing  duplicate Prices | " + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                logger.Error(ex.ToString());
                return false;

            }
            return true;

        }
        /// <summary>
        /// a bool function that takes an entity provided calls for products on your db and returns true
        /// 1) on registers not found with code
        /// 2) on register found with Id not (Zero as new) and Id same as entity (case of current product updating)
        /// 3) on any other case product with same code found on db and not same to update 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool checkUniqueCode(ProductDTO entity) {
            if (!uniqueCodePolicy)
                return true;
            try {
                List<Product> same = DbContext.Product.Where(sp => sp.Code == entity.Code && sp.IsDeleted != true).ToList();
                if (same.Count() < 1)
                    return true;
                else if (same.Count() == 1 && same.FirstOrDefault().Id != 0 && same.FirstOrDefault().Id == entity.Id)
                    return true;
                else
                    return false;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return false;
            }

        }
        public int SaveChanges() {
            return DbContext.SaveChanges();
        }
    }
}

