using LinqKit;
using log4net;
using Pos_WebApi.Models;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Pos_WebApi.Repositories.BORepos {
    public class ProductPricesRepository {
        protected PosEntities DbContext;
        protected BussinessRepository br;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProductPricesRepository(PosEntities db) {
            this.DbContext = db;

            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            br = new BussinessRepository(db);
        }
        /// <summary>
        /// Model to cover Prices over Pricelists missmatch
        /// Used as Ingredient this model returns on Id the ingredientId of ProductPrices grouped by its property then by 
        /// </summary>
        public class MissingPrices {
            public MissingPrices(long? pid, List<long> pll) {
                Id = pid;
                PricelistIds = pll;
            }
            public long? Id { get; set; }
            public List<long> PricelistIds { get; set; }
        }
        /// <summary>
        /// DAO providing difference of ProductPrices vs Pricelists
        /// Each Pricelist register must have a ProductPrice assigned to a Product
        /// Values that must have is ProductId || IngredientId ,vatId , Price and PricelistId.
        /// First Get a list of Pricelists witch for each one of them you have to have a Price over each product
        /// then group Prices by Product or Ingredient id to get collection of assigned Pricelists ,
        /// Find diff then return an obj collection 
        /// </summary>
        /// <param name="asIngr">if override to true functionality changes to ingredients</param>
        /// <returns>Missing Prices Array of { Id:ProductId , PlsIds : [  pricelist[i].Id ] } </returns>
        public List<MissingPrices> getMissingProductPrices(bool asIngr = false) {
            List<long> pls = DbContext.Pricelist.Select(q => (long)q.Id).ToList();
            List<MissingPrices> res = new List<MissingPrices>();
            try {
                if (asIngr == true) {
                    var pps = DbContext.PricelistDetail.Where(w => w.IngredientId != null).Select(m => new { m.IngredientId, m.PricelistId }).GroupBy(g => g.IngredientId);
                    foreach (var p in pps) {
                        List<long> ids = p.Select(q => (long)q.PricelistId).ToList();
                        List<long> list3 = pls.Except(ids).ToList();
                        if (list3.Count() > 0) {
                            long? pid = p.Select(q => q.IngredientId).FirstOrDefault();
                            MissingPrices g = new MissingPrices(pid, list3);
                            res.Add(g);
                        }
                    }
                } else {
                    var pps = DbContext.PricelistDetail.Where(w => w.ProductId != null).Select(m => new { m.ProductId, m.PricelistId }).GroupBy(g => g.ProductId);
                    foreach (var p in pps) {
                        List<long> ids = p.Select(q => (long)q.PricelistId).ToList();
                        List<long> list3 = pls.Except(ids).ToList();
                        if (list3.Count() > 0) {
                            long? pid = p.Select(q => q.ProductId).FirstOrDefault();
                            MissingPrices g = new MissingPrices(pid, list3);
                            res.Add(g);
                        }
                    }
                }
            } catch (Exception ex) {
                logger.Error(ex.ToString());
            }
            return res;
        }
        /// <summary>
        /// DAO providing fix over missmatch of product Prices or Ingredient Prices.
        /// Asks MissingProducts DAO above and creates a register foreach ProductPrices missing witch is being pushed in an array to save
        /// </summary>
        /// <param name="vatId">Vat.Id provided to default apply Missmatches</param>
        /// <param name="asIngr">if override to true functionality changes to ingredients</param>
        /// <returns> Bool eval of action </returns>
        public bool fixMissingProductPrices(long vatId, bool asIngr = false) {
            try {
                var missing = getMissingProductPrices(asIngr);
                foreach (var m in missing) {
                    foreach (long plid in m.PricelistIds) {
                        PricelistDetail newreg = createPDMissing(m.Id, plid, vatId, asIngr);
                        PricelistDetail ppn = DbContext.PricelistDetail.Add(newreg);
                        DbContext.Entry(ppn).State = EntityState.Added;
                    }
                }
                DbContext.SaveChanges();
                return true;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return false;
            }
        }
        /// <summary>
        /// Returns a PricelistDetail work as toModel of dynamic input fields
        /// Creates ProductPrices Missing DTOs for products and Ingredients
        /// </summary>
        /// <param name="Id">Id of reference Product.Id or Ingredient.Id</param>
        /// <param name="plId">Missing Pricelist.Id from ProductPrices vs Pricelists</param>
        /// <param name="vatId">Prefered or default Vat.Id to apply on models</param>
        /// <param name="asIngr">if this true then return of ProductDetail is for ingredient Prices so ProductId || IngredientId will be casted according to this</param>
        /// <returns>a PricelistDetail Model missing from collection created by </returns>
        public PricelistDetail createPDMissing(long? Id, long plId, long vatId, bool asIngr = false) {
            PricelistDetail n = new PricelistDetail();
            try {
                n.Id = 0;
                n.ProductId = (!asIngr) ? Id : null;
                n.IngredientId = (asIngr) ? Id : null;
                n.PricelistId = plId;
                n.VatId = vatId;
                n.Price = 0;
                n.PriceWithout = 0;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return null;
            }
            return n;
        }

        /// <summary>
        /// Crud get Product Prices by filters and order sortpredicate 
        /// First Filter Ingredients by predicate and page vars then extend query result over PricelistDetail to provide a collection of Ingredient Prices
        /// </summary>
        /// <param name="predicate">Base AND Filter model</param>
        /// <param name="sortpredicate">Unused</param>
        /// <param name="page"> Param to skip entries by Page * Pagesize to get pagginated data</param>
        /// <param name="pageSize"> Param to characterize Size of a Page returned and used also on skip of pageResults </param>
        /// <returns>List of product Prices filtered by input filters</returns>
        public PagedResult<ProductPricesModel> GetPaged(Expression<Func<TempProductsWithCategoriesFlat, bool>> predicate = null, Expression<Func<ProductPricesModel, string>> sortpredicate = null, int page = 1, int pageSize = 10) {
            var query = br.ProductsWithCategoriesFlat(predicate).AsQueryable();

            var result = new PagedResult<ProductPricesModel>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.AsExpandable().Count();

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            if (skip < 0) skip = 0;


            query = query.AsExpandable().OrderBy(o => o.ProductCode).Skip(skip).Take(pageSize);

            var baseQuery = (from q in query
                             join qq in DbContext.PricelistDetail on q.ProductId equals qq.ProductId into f
                             from pd in f.DefaultIfEmpty()
                             join qqq in DbContext.Pricelist on pd.PricelistId equals qqq.Id into ff
                             from pl in ff.DefaultIfEmpty()
                             select new {
                                 ProductCode = q.ProductCode,
                                 ProductId = q.ProductId,
                                 Description = q.Description,
                                 Price = pd != null ? pd.Price : 0,
                                 PriceWithout = pd != null ? pd.PriceWithout : 0,
                                 PriceListId = pd != null ? pd.PricelistId : null,
                                 PricelistMasterId = pd != null && pl != null ? pl.PricelistMasterId : null,
                                 PricelistDetailId = pd != null ? pd.Id : 0,
                                 VatId = pd != null ? pd.VatId : null,
                                 TaxId = pd != null ? pd.TaxId : null,
                                 ProductCategoryId = q.ProductCategoryId,
                                 LookUpPriceListId = pd != null && pl != null ? pl.LookUpPriceListId : null,
                                 Percentage = pd != null && pl != null ? pl.Percentage : null,
                             }).OrderBy(o => o.ProductCode).GroupBy(g => g.ProductId).
                             Select(s => new ProductPricesModel {
                                 ProductId = s.FirstOrDefault().ProductId,
                                 ProductCode = s.FirstOrDefault().ProductCode,
                                 Description = s.FirstOrDefault().Description,
                                 ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                                 ProductPricesModelDetails = s.Select(ss => new ProductPricesModelDetails {
                                     Price = ss.Price,
                                     PriceWithout = ss.PriceWithout,
                                     PriceListId = ss.PriceListId,
                                     PricelistMasterId = ss.PricelistMasterId,
                                     PricelistDetailId = ss.PricelistDetailId,
                                     VatId = ss.VatId,
                                     TaxId = ss.TaxId,
                                     LookUpPriceListId = ss.LookUpPriceListId,
                                     Percentage = ss.Percentage,
                                 }).Where(w => w.PricelistDetailId != 0).ToList()
                             });
            result.Results = baseQuery.ToList();
            return result;
        }

        /// <summary>
        /// Crud get Ingredient Prices by filters and order sortpredicate 
        /// First Filter Ingredients by predicate and page vars then extend query result over PricelistDetail to provide a collection of Ingredient Prices
        /// </summary>
        /// <param name="predicate">Base AND Filter model</param>
        /// <param name="sortpredicate">Unused</param>
        /// <param name="page"> Param to skip entries by Page * Pagesize to get pagginated data</param>
        /// <param name="pageSize"> Param to characterize Size of a Page returned and used also on skip of pageResults </param>
        /// <returns>List of product Prices filtered by input filters</returns>
        public PagedResult<ProductPricesModel> GetPagedIngredients(Expression<Func<TempProductsWithCategoriesFlat, bool>> predicate = null, Expression<Func<ProductPricesModel, string>> sortpredicate = null, int page = 1, int pageSize = 10) {
            var query = from q in DbContext.Ingredients
                        where q.IsDeleted == null || q.IsDeleted == false
                        select new TempProductsWithCategoriesFlat {
                            ProductId = q.Id,
                            Description = q.Description,
                            ProductCode = q.Code,
                            ProductCategoryCode = "", ProductCategoryId = 0, ProductCategoryDesc = "", CategoryId = 0, CategoryDesc = ""
                        };
            if (predicate != null) {
                query = query.Where(predicate);
            }

            var result = new PagedResult<ProductPricesModel>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.AsExpandable().Count();

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            if (skip < 0) skip = 0;

            query = query.AsExpandable().OrderBy(o => o.ProductCode).Skip(skip).Take(pageSize);

            var baseQuery = (from q in query
                             join qq in DbContext.PricelistDetail on q.ProductId equals qq.IngredientId into f
                             from pd in f.DefaultIfEmpty()
                             join qqq in DbContext.Pricelist on pd.PricelistId equals qqq.Id into ff
                             from pl in ff.DefaultIfEmpty()
                             select new {
                                 ProductCode = q.ProductCode,
                                 ProductId = q.ProductId,
                                 Description = q.Description,
                                 Price = pd != null ? pd.Price : 0,
                                 PriceWithout = pd != null ? pd.PriceWithout : 0,
                                 PriceListId = pd != null ? pd.PricelistId : null,
                                 PricelistMasterId = pd != null && pl != null ? pl.PricelistMasterId : null,
                                 PricelistDetailId = pd != null ? pd.Id : 0,
                                 VatId = pd != null ? pd.VatId : null,
                                 TaxId = pd != null ? pd.TaxId : null,
                                 ProductCategoryId = q.ProductCategoryId,
                                 LookUpPriceListId = pd != null && pl != null ? pl.LookUpPriceListId : null,
                                 Percentage = pd != null && pl != null ? pl.Percentage : null,
                             }).OrderBy(o => o.ProductCode).GroupBy(g => g.ProductId).
                             Select(s => new ProductPricesModel {
                                 ProductId = s.FirstOrDefault().ProductId,
                                 ProductCode = s.FirstOrDefault().ProductCode,
                                 Description = s.FirstOrDefault().Description,
                                 ProductCategoryId = s.FirstOrDefault().ProductCategoryId,
                                 ProductPricesModelDetails = s.Select(ss => new ProductPricesModelDetails {
                                     Price = ss.Price,
                                     PriceWithout = ss.PriceWithout,
                                     PriceListId = ss.PriceListId,
                                     PricelistMasterId = ss.PricelistMasterId,
                                     PricelistDetailId = ss.PricelistDetailId,
                                     VatId = ss.VatId,
                                     TaxId = ss.TaxId,
                                     LookUpPriceListId = ss.LookUpPriceListId,
                                     Percentage = ss.Percentage,
                                 }).Where(w => w.PricelistDetailId != 0).ToList()

                             });

            result.Results = baseQuery.ToList();
            return result;
        }

        /// <summary>
        /// Crud Update Range Action 
        /// Filters pricesModel givven on Id !=0 to get allready saved registers 
        /// Apply Changes on models and mark them as modified to act Context Save changes on return
        /// </summary>
        /// <param name="prices"> Array of PricelistDetails to update </param>
        /// <returns> Bool: true on action complete and false on error </returns>
        public bool UpdateProductPrices(IEnumerable<PricelistDetail> prices) {
            var pricesIds = prices.Where(w => w.Id != 0).Select(s => s.Id);
            var pds = DbContext.PricelistDetail.Where(w => pricesIds.Contains(w.Id)).ToList();

            foreach (var detail in pds) {
                var currentpr = prices.FirstOrDefault(f => f.Id == detail.Id);
                detail.Price = currentpr.Price;
                detail.VatId = currentpr.VatId;
                detail.TaxId = currentpr.TaxId;
                detail.PriceWithout = currentpr.PriceWithout;

                DbContext.Entry(detail).State = EntityState.Modified;
            }
            //Insert new Prices
            try {
                var newPrices = prices.Where(w => w.Id == 0).ToList();
                foreach (var detail in newPrices) {
                    DbContext.PricelistDetail.Add(detail);
                }
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return false;
            }

            return true;
        }
        /// <summary>
        /// Creates a groups of objects by Product and Pricelist and takes First rejister to apply 
        /// then it removes more than one rejisters same to these {ProductId AND PricelistId}
        /// </summary>
        /// <returns>true if sql command success else false on error catch</returns>
        public bool RemoveDuplicatesFromProductPrices() {
            try {
                var doublePrices = DbContext.PricelistDetail.Where(w => w.ProductId != null)
                                      .GroupBy(g => new { g.ProductId, g.PricelistId })
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
                    sb.AppendLine(string.Format("Update orderdetail set PricelistDetailid= {0} where pricelistDetailid in ({1})", keyToFixed.KeyToKeep,
                                            str));
                    sb.AppendLine(string.Format("Delete from pricelistDetail where id in ({0})", str));
                }
                sb.AppendLine(string.Format("Update pricelistDetail set Price= 0 where price is null"));
                DbContext.Database.ExecuteSqlCommand(sb.ToString());
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error removing  duplicate Prices | " + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                return false;

            }
            return true;

        }

        /// <summary>
        /// Creates a groups of objects by Ingredient and Pricelist and takes First rejister to apply 
        /// then it removes more than one rejisters same to these {IngredientId AND PricelistId}
        /// </summary>
        /// <returns>true if sql command success else false on error catch</returns>
        public bool RemoveDuplicatesFromIngredientPrices() {
            try {
                var doublePrices = DbContext.PricelistDetail.Where(w => w.IngredientId != null)
                                      .GroupBy(g => new { g.IngredientId, g.PricelistId })
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
                    sb.AppendLine(string.Format("Update OrderDetailIgredients set PricelistDetailid= {0} where pricelistDetailid in ({1})", keyToFixed.KeyToKeep,
                                            str));
                    sb.AppendLine(string.Format("Delete from pricelistDetail where id in ({0})", str));
                }
                sb.AppendLine(string.Format("Update pricelistDetail set PriceWithout= 0 where PriceWithout is null"));
                DbContext.Database.ExecuteSqlCommand(sb.ToString());
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// This is a script to auto correct Prices that Pricelists have a lookuppricelist id
        /// Those Prices have to lookup price on lookuppricelist id and apply a persentance (%) of its value assigned to its own Price field
        /// Used to correct prices on Pricelist Add or Update
        /// </summary>
        /// <param name="affectedPrices">Pricelists that affected ( w.LookUpPriceListId != null )</param>
        /// <returns>An on sql Command Action Success Responce true else on error catch false</returns>
        public bool ReculculatePercentagePrices(IEnumerable<Pricelist> affectedPrices) {
            StringBuilder sb = new StringBuilder();
            try {

                foreach (var pl in affectedPrices) {
                    var master = DbContext.Pricelist.Find(pl.LookUpPriceListId);

                    sb.AppendLine("update upl ");
                    sb.AppendLine(string.Format("set upl.Price = Round(lpl.Price * cast({0} as float),2) ", ((double)(pl.Percentage / 100)).ToString(CultureInfo.GetCultureInfo("en-US"))));
                    sb.AppendLine(" from PricelistDetail upl");
                    sb.AppendLine(string.Format(" join PricelistDetail lpl on upl.ProductId = lpl.ProductId  and lpl.IngredientId is null and lpl.PricelistId = {0}", master.Id));
                    sb.AppendLine(string.Format(" where upl.PricelistId = {0}", pl.Id));

                    sb.AppendLine(" update upl");
                    sb.AppendLine(string.Format(" set upl.Price = Round(lpl.Price * cast({0} as float), 2) ", ((double)(pl.Percentage / 100)).ToString(CultureInfo.GetCultureInfo("en-US"))));
                    sb.AppendLine(" from PricelistDetail upl ");
                    sb.AppendLine(string.Format(" join PricelistDetail lpl on upl.IngredientId = lpl.IngredientId  and lpl.ProductId is null and lpl.PricelistId = {0} ", master.Id));
                    sb.AppendLine(string.Format(" where upl.PricelistId  = {0}", pl.Id));

                    sb.AppendLine(" Insert into PricelistDetail(PricelistId, ProductId, Price, VatId, TaxId, PriceWithout)");
                    sb.AppendLine(string.Format(" select PricelistId = {0}, ProductId, Price = Round(Price * cast({1} as float) ,2), VatId,TaxId, PriceWithout from PricelistDetail where ProductId not in (", pl.Id, ((double)(pl.Percentage / 100)).ToString(CultureInfo.GetCultureInfo("en-US"))));
                    sb.AppendLine(string.Format("   (select ProductId from PricelistDetail PricelistId where PricelistId = {0} and IngredientId is null )", pl.Id));
                    sb.AppendLine(string.Format(") and PricelistId = {0} and IngredientId is null", master.Id));

                    sb.AppendLine(" Insert into PricelistDetail(PricelistId, IngredientId, Price, VatId, TaxId, PriceWithout)");
                    sb.AppendLine(string.Format(" select PricelistId = {0}, IngredientId, Price = Round(Price * cast({1} as float) ,2) , VatId,TaxId, PriceWithout from PricelistDetail where IngredientId not in (", pl.Id, ((double)(pl.Percentage / 100)).ToString(CultureInfo.GetCultureInfo("en-US"))));
                    sb.AppendLine(string.Format("   (select IngredientId from PricelistDetail PricelistId where PricelistId = {0} and ProductId is null )", pl.Id));
                    sb.AppendLine(string.Format(") and PricelistId = {0} and ProductId is null", master.Id));
                }
                DbContext.Database.ExecuteSqlCommand(sb.ToString());
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return false;
            }
            return true;
        }

        public int SaveChanges() {
            return DbContext.SaveChanges();
        }
    }
}
