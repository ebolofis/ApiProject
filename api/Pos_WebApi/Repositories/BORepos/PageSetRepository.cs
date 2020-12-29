using log4net;
using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net;
using System.Web.Http;


namespace Pos_WebApi.Repositories.BORepos {
    public class PageSetRepository {
        protected PosEntities DbContext;
        protected BussinessRepository boRepo;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public PageSetRepository(PosEntities db) {
            DbContext = db;
            boRepo = new BussinessRepository(db);
        }

        public IQueryable<PageSetDTO> GetAllPageSet(Expression<Func<PageSetDTO, bool>> predicate = null) {
            var query = (from q in DbContext.PageSet
                         join qq in DbContext.PagePosAssoc on q.Id equals qq.PageSetId into ff
                         from ppa in ff.DefaultIfEmpty()
                         join qqq in DbContext.PdaModule on q.Id equals qqq.PageSetId into f
                         from pda in f.DefaultIfEmpty()
                         select new //PageSetDTO
                         {
                             Id = q.Id,
                             ActivationDate = q.ActivationDate,
                             DeactivationDate = q.DeactivationDate,
                             PdaModuleId = pda != null ? pda.Id : 0,
                             PosInfoId = ppa != null ? ppa.PosInfoId : null,
                             PagePosAssocId = ppa != null ? ppa.Id : 0,
                             Description = q.Description
                         }).GroupBy(g => g.Id).Select(s => new PageSetDTO {
                             Id = s.FirstOrDefault().Id,
                             ActivationDate = s.FirstOrDefault().ActivationDate,
                             DeactivationDate = s.FirstOrDefault().DeactivationDate,
                             Description = s.FirstOrDefault().Description,
                             AssosiatedPos = s.Select(ss => new PagePosAssocDTO {
                                 Id = ss.PagePosAssocId,
                                 PageSetId = ss.Id,
                                 PosInfoId = ss.PosInfoId
                             }).Where(w => w.PosInfoId != null).Distinct().ToList()
                         });
            if (predicate != null)
                return query.Where(predicate);
            return query;
        }
        public IQueryable<PagesDTO> GetAllPages(Expression<Func<PagesDTO, bool>> predicate = null) {
            var query = from q in DbContext.Pages
                        select new PagesDTO {
                            Id = q.Id,
                            DefaultPriceListId = q.DefaultPriceList,
                            Description = q.Description,
                            ExtendedDesc = q.ExtendedDesc,
                            PageSetId = q.PageSetId,
                            Status = q.Status,
                            Sort = q.Sort
                        };
            if (predicate != null) {
                return query.Where(predicate);
            } else
                return query;
        }

        public PagedResult<PageSetDTO> GetPagedPageSet(Expression<Func<PageSetDTO, bool>> predicate = null, Expression<Func<PageSetDTO, string>> sortpredicate = null, int page = 1, int pageSize = 10) {
            var query = GetAllPageSet(predicate);

            var result = new PagedResult<PageSetDTO>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            if (skip < 0)
                skip = 0;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }
        public PagedResult<PagesDTO> GetPagedPages(Expression<Func<PagesDTO, bool>> predicate = null, Expression<Func<PagesDTO, string>> sortpredicate = null, int page = 1, int pageSize = 10) {
            var query = GetAllPages(predicate);

            var result = new PagedResult<PagesDTO>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            if (skip < 0)
                skip = 0;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }

        /// <summary>
        /// Collects and return products accorgind to a predicate filter
        /// used to get filterer results by category and productcategory functions
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<TempProductsWithCategoriesFlat> GetProducts(Expression<Func<TempProductsWithCategoriesFlat, bool>> predicate = null) {
            return boRepo.ProductsWithCategoriesFlat(predicate);
        }
        /// <summary>
        /// Paged result of products by filter entity 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="sortpredicate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedResult<TempProductsWithCategoriesFlat> GetPagedProducts(Expression<Func<TempProductsWithCategoriesFlat, bool>> predicate = null, Expression<Func<TempProductsWithCategoriesFlat, string>> sortpredicate = null, int page = 1, int pageSize = 10) {
            var query = boRepo.ProductsWithCategoriesFlat(predicate);

            var result = new PagedResult<TempProductsWithCategoriesFlat>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            if (skip < 0)
                skip = 0;
            result.Results = query.OrderBy(sortpredicate).Skip(skip).Take(pageSize).ToList();

            return result;
        }
        public IQueryable<PageButtonDTO> GetPageButtons(Expression<Func<PageButtonDTO, bool>> predicate = null) {
            var query = from q in DbContext.PageButton
                        join qq in boRepo.ProductsWithCategoriesFlat() on q.ProductId equals qq.ProductId into f
                        from pwd in f.DefaultIfEmpty()
                        select new PageButtonDTO {
                            Id = q.Id,
                            ProductId = q.ProductId,
                            Background = q.Background,
                            Color = q.Color,
                            Description = q.Description,
                            SalesDescritpion = pwd != null ? pwd.Description : q.Description,
                            Sort = q.Sort,
                            PageId = q.PageId,
                            Type = q.Type,
                            NavigateToPage = q.NavigateToPage,
                            PriceListId = q.PriceListId,
                            SetDefaultPriceListId = q.SetDefaultPriceListId,
                            SetDefaultSalesType = q.SetDefaultSalesType

                        };
            if (predicate != null)
                return query.Where(predicate);
            else
                return query;
        }


        public PagesDTO AddPage(PagesDTO model) {
            var page = model.ToModel();
            foreach (var btn in page.PageButton) {
                if (btn.Type == 1 || btn.Type == 8 || btn.Type == 10 || btn.Type == 11) {
                    var prod = DbContext.Product.Where(w => w.Id == btn.ProductId).SingleOrDefault();
                    btn.PreparationTime = (short)(prod.PreparationTime ?? 0);
                    if (!String.IsNullOrEmpty(prod.ImageUri))
                        btn.ImageUri = prod.ImageUri;
                    else
                        btn.ImageUri = "";
                    btn.KdsId = prod.KitchenId;
                }
            }

            DbContext.Pages.Add(page);
            model.Id = page.Id;
            return model;
        }

        public PagesDTO UpdateButtonsToPage(PagesDTO model) {
            var entity = DbContext.Pages.Include(i => i.PageButton).FirstOrDefault(x => x.Id == model.Id);
            var page = model.UpdateModel(entity);
            foreach (var btn in page.PageButton) {
                if (btn.Type == 1 || btn.Type == 8 || btn.Type == 10 || btn.Type == 11) {
                    var prod = DbContext.Product.Where(w => w.Id == btn.ProductId).SingleOrDefault();
                    if (prod != null) {
                        btn.PreparationTime = (short)(prod.PreparationTime ?? 0);
                        btn.ImageUri = prod.ImageUri;
                        btn.KdsId = prod.KitchenId;
                    }
                }
                if (btn.Id != 0) {
                    DbContext.Entry(btn).State = EntityState.Modified;
                } else {
                    DbContext.Entry(btn).State = EntityState.Added;
                }
            }

            var btnsToDelete = model.PageButtons.Where(w => w.IsDeleted == true);
            foreach (var btn in btnsToDelete) {
                var b = DbContext.PageButton.Where(w => w.Id == btn.Id).SingleOrDefault();
                DbContext.PageButton.Remove(b);
            }
            DbContext.Entry(page).State = EntityState.Modified;
            model.Id = page.Id;
            return model;
        }

        public PageSetDTO AddPageSet(PageSetDTO model) {
            var pageset = model.ToModel();

            if (model.Pages.Count() > 0)
                foreach (var page in model.Pages) {
                    AddPage(page);
                }

            DbContext.PageSet.Add(pageset);
            return model;
        }

        public PageSetDTO UpdatePageSet(PageSetDTO model) {
            var entity = DbContext.PageSet.Include(i => i.Pages.Select(ss => ss.PageButton)).Include(i => i.PagePosAssoc).FirstOrDefault(x => x.Id == model.Id);
            var pageset = model.UpdateModel(entity);
            foreach (var item in model.Pages) {
                var entityFromModel = item.ToModel();
                if (item.Id == 0) {

                    DbContext.Pages.Add(entityFromModel);
                    DbContext.Entry(entityFromModel).State = EntityState.Added;
                } else
                    UpdateButtonsToPage(item);
            }

            foreach (var item in model.AssosiatedPos.Where(w => w.IsDeleted == false)) {
                if (item.Id != 0) {

                    var cur = entity.PagePosAssoc.FirstOrDefault(x => x.Id == item.Id);
                    cur = item.UpdateModel(cur);
                    DbContext.Entry(cur).State = EntityState.Modified;
                } else {
                    DbContext.Entry(item.ToModel()).State = EntityState.Added;
                }
            }


            var pagesToDelete = model.Pages.Where(w => w.IsDeleted == true);
            foreach (var pg in pagesToDelete) {

                foreach (var btn in pg.PageButtons) {
                    DbContext.PageButton.Remove(btn.ToModel());
                }
                var p = DbContext.Pages.Where(w => w.Id == pg.Id).SingleOrDefault();
                //DbContext.Entry(b).State = EntityState.Deleted;
                DbContext.Pages.Remove(p);
            }


            var pagePosAssocToDelete = model.AssosiatedPos.Where(w => w.IsDeleted == true);
            foreach (var btn in pagePosAssocToDelete) {
                var b = DbContext.PagePosAssoc.Where(w => w.Id == btn.Id).SingleOrDefault();
                DbContext.PagePosAssoc.Remove(b);
            }

            DbContext.Entry(pageset).State = EntityState.Modified;
            model.Id = pageset.Id;
            return model;
        }

        public bool DeletePageSet(long? id) {
            //var entitytoRemove = DbContext.PageSet.FirstOrDefault(x => x.Id == id);
            // Check if the PageSet is bound to a Pos
            var PageSetIsBoundToPos = DbContext.PagePosAssoc.FirstOrDefault(i => i.PageSetId==id);
            if (PageSetIsBoundToPos == null)
              {
                logger.Info(" Proceed with deleting a not bound to Pos Pageset");
            }
            else 
            {
                logger.Info("Cannot delete a PageSet which is bound to a Pos");
                return true;
            }


            //var pageset = model.UpdateModel(entity);
            var entitytoRemove = DbContext.PageSet.Include(i => i.Pages.Select(ss => ss.PageButton)).Include(i => i.PagePosAssoc).FirstOrDefault(x => x.Id == id);



            var pagesToDelete = DbContext.Pages.Where(w => w.PageSetId == entitytoRemove.Id).ToList();
            foreach (var pg in pagesToDelete) {
                foreach (var btn in DbContext.PageButton.Where(w => w.PageId == pg.Id).ToList()) {
                    //DbContext.Entry(btn).State = EntityState.Deleted;
                    DbContext.PageButton.Remove(btn);
                }
                var p = DbContext.Pages.Where(w => w.Id == pg.Id).SingleOrDefault();
                DbContext.Pages.Remove(p);
            }

            var pagePosAssocToDelete = DbContext.PagePosAssoc.Where(w => w.PageSetId == entitytoRemove.Id).ToList();
            foreach (var ppas in pagePosAssocToDelete) {
                var b = DbContext.PagePosAssoc.Where(w => w.Id == ppas.Id).SingleOrDefault();
                DbContext.PagePosAssoc.Remove(b);
            }
            try {
                var b = DbContext.PageSet.Where(w => w.Id == id).SingleOrDefault();
                DbContext.PageSet.Remove(b);
                return true;
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting PageSet : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                //DbContext.Entry(b).State = EntityState.Deleted;
                //DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                return true;
            }
            
        }

        public PageSetDTO CopyFromPageSet(PageSetDTO model, long sourcePageSetId, IEnumerable<long> pageIds) {

            model.Pages.Clear();
            var pagesetToCopy = DbContext.PageSet.Include(i => (i.Pages.Select(ii => ii.PageButton)))
                                                 .Include(i => i.PagePosAssoc)
                                                 .FirstOrDefault(w => w.Id == sourcePageSetId);
            var pagesToCopy = (from q in DbContext.Pages.Include(i => i.PageButton).Where(w => w.PageSetId == sourcePageSetId && pageIds.Contains(w.Id))
                               select q).ToList();

            PageSet newPageSet = model.ToModel();
            //using (TransactionHelper.BeginTransaction(DbContext))
            //{
            foreach (var pa in pagesetToCopy.PagePosAssoc) {
                var assoc = new PagePosAssoc() { PageSet = newPageSet, PosInfoId = pa.PosInfoId };
                newPageSet.PagePosAssoc.Add(assoc);
                //   DbContext.PageSet.Add(newPageSet);
            }


            foreach (var p in pagesToCopy) {
                var newPage = new Pages() {
                    DefaultPriceList = p.DefaultPriceList,
                    Description = p.Description,
                    ExtendedDesc = p.ExtendedDesc,
                    PageSet = newPageSet,
                    Sort = p.Sort,
                    Status = p.Status
                };
                foreach (var b in p.PageButton) {
                    var newButton = new PageButton() {
                        Background = b.Background,
                        Color = b.Color,
                        Description = b.Description,
                        ImageUri = b.ImageUri,
                        NavigateToPage = b.NavigateToPage,
                        PreparationTime = b.PreparationTime,
                        PriceListId = b.PriceListId,
                        ProductId = b.ProductId,
                        Price = b.Price,
                        SetDefaultPriceListId = b.SetDefaultPriceListId,
                        Type = b.Type,
                        Sort = b.Sort,
                        SetDefaultSalesType = b.SetDefaultSalesType,
                        Pages = newPage
                    };
                    newPage.PageButton.Add(newButton);
                }
                newPageSet.Pages.Add(newPage);
            }
            DbContext.PageSet.Add(newPageSet);
            DbContext.SaveChanges();
            var allPagesetPages = (DbContext.Pages.Include(i => i.PageButton).Where(w => w.PageSetId == sourcePageSetId)).ToList();
            var navBtnsSourceToCopy = from q in (allPagesetPages.SelectMany(sm => sm.PageButton).Where(w => w.NavigateToPage != null)
                                                                  .Select(s => new {
                                                                      ButtonDescription = s.Description,
                                                                      ButtonSort = s.Sort,
                                                                      PageId = s.NavigateToPage
                                                                  }))
                                      join qq in allPagesetPages on q.PageId equals qq.Id
                                      select new {
                                          ButtonDescription = q.ButtonDescription,
                                          ButtonSort = q.ButtonSort,
                                          PageId = qq.Id,
                                          PageDescription = qq.Description,
                                          PageSort = qq.Sort
                                      };
            var navBtnsToUpdate = DbContext.Pages.Where(w => w.PageSetId == newPageSet.Id).SelectMany(sm => sm.PageButton).Where(w => (w.NavigateToPage ?? 0) != 0);//.OrderBy(o=>o.Description).Distinct();
            foreach (var navToChange in navBtnsToUpdate) {
                var oldPageNav = navBtnsSourceToCopy.FirstOrDefault(x => x.ButtonDescription == navToChange.Description /* && x.ButtonSort == navToChange.Sort*/);
                if (oldPageNav != null) {
                    var page = DbContext.Pages.FirstOrDefault(x => x.PageSetId == newPageSet.Id && x.Description == oldPageNav.PageDescription/* && x.Sort == oldPageNav.PageSort*/);
                    if (page != null) {
                        navToChange.NavigateToPage = page.Id;
                    } else {
                        navToChange.NavigateToPage = null;
                    }
                } else {
                    navToChange.NavigateToPage = null;
                }
                DbContext.Entry(navToChange).State = EntityState.Modified;
            }
            DbContext.SaveChanges();
            model.Id = newPageSet.Id;
            return model;
        }
        /// <summary>
        /// UpdateFromPageSet is a function that provides functionality of copy pages from an other pageset and update model given
        /// Save changes on context and manage special buttons after they are Created and they have Ids
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sourcePageSetId">This is a param to a target pageset that you have copied pages</param>
        /// <param name="pageIds">This is an array of (long)ids that are the pages that you have copied and needs to be handled as added</param>
        /// <returns>the model that updated </returns>
        public PageSetDTO UpdateFromPageSet(PageSetDTO model, long sourcePageSetId, IEnumerable<long> pageIds) {
            //model here has filtered Pages
            try {
                //entity to copy 
                var pagesetToCopy = DbContext.PageSet.Include(i => (i.Pages.Select(ii => ii.PageButton))).FirstOrDefault(w => w.Id == sourcePageSetId);
                //pages to copy 
                var pagesToCopy = (from q in DbContext.Pages.Include(i => i.PageButton).Where(w => w.PageSetId == sourcePageSetId && pageIds.Contains(w.Id)) select q).ToList();
                //create and add pages and pagebuttons
                foreach (var p in pagesToCopy) {
                    var newPage = new PagesDTO() { DefaultPriceListId = p.DefaultPriceList, Description = p.Description, ExtendedDesc = p.ExtendedDesc, Sort = p.Sort, Status = p.Status, PageSetId = model.Id };
                    foreach (var b in p.PageButton) {
                        var newButton = new PageButtonDTO() {
                            Background = b.Background, Color = b.Color, Description = b.Description, NavigateToPage = b.NavigateToPage,
                            PriceListId = b.PriceListId, ProductId = b.ProductId,
                            //ImageUri = b.ImageUri, //PreparationTime = b.PreparationTime, //Price = b.Price,
                            SetDefaultPriceListId = b.SetDefaultPriceListId, Type = b.Type, Sort = b.Sort, SetDefaultSalesType = b.SetDefaultSalesType,
                            PageId = newPage.Id
                        };
                        newPage.PageButtons.Add(newButton);
                    }
                    model.Pages.Add(newPage);
                }

                var entity = DbContext.PageSet.Include(i => i.Pages.Select(ss => ss.PageButton)).Include(i => i.PagePosAssoc).FirstOrDefault(x => x.Id == model.Id);
                var pageset = model.UpdateModel(entity);
                //foreach pages add page or updatepagebuttons
                foreach (var item in model.Pages) {
                    var entityFromModel = item.ToModel();
                    if (item.Id == 0) {
                        DbContext.Pages.Add(entityFromModel); DbContext.Entry(entityFromModel).State = EntityState.Added;
                    } else UpdateButtonsToPage(item);
                }
                //update assosiated pos
                foreach (var item in model.AssosiatedPos.Where(w => w.IsDeleted == false)) {
                    if (item.Id != 0) {
                        var cur = entity.PagePosAssoc.FirstOrDefault(x => x.Id == item.Id); 
                        cur = item.UpdateModel(cur);
                        DbContext.Entry(cur).State = EntityState.Modified;
                    } else { DbContext.Entry(item.ToModel()).State = EntityState.Added; }
                }


                //pages that is marked as deleted
                var pagesToDelete = model.Pages.Where(w => w.IsDeleted == true);
                foreach (var pg in pagesToDelete) {
                    foreach (var btn in pg.PageButtons) { DbContext.PageButton.Remove(btn.ToModel()); }
                    var p = DbContext.Pages.Where(w => w.Id == pg.Id).SingleOrDefault();
                    //DbContext.Entry(b).State = EntityState.Deleted;
                    DbContext.Pages.Remove(p);
                }

                //assoscs that is marked as deleted
                var pagePosAssocToDelete = model.AssosiatedPos.Where(w => w.IsDeleted == true);
                foreach (var btn in pagePosAssocToDelete) {
                    var b = DbContext.PagePosAssoc.Where(w => w.Id == btn.Id).SingleOrDefault();
                    DbContext.PagePosAssoc.Remove(b);
                }
                //mark ent as modified and save changes to give Pages added new Ids 
                DbContext.Entry(pageset).State = EntityState.Modified;
                DbContext.SaveChanges();
                //For new pages added manage navigation buttons 
                var allPagesetPages = (DbContext.Pages.Include(i => i.PageButton).Where(w => w.PageSetId == sourcePageSetId)).ToList();
                var navBtnsSourceToCopy = from q in (allPagesetPages.SelectMany(sm => sm.PageButton).Where(w => w.NavigateToPage != null).Select(s => new { ButtonDescription = s.Description, ButtonSort = s.Sort, PageId = s.NavigateToPage }))
                                          join qq in allPagesetPages on q.PageId equals qq.Id
                                          select new { ButtonDescription = q.ButtonDescription, ButtonSort = q.ButtonSort, PageId = qq.Id, PageDescription = qq.Description, PageSort = qq.Sort };

                var navBtnsToUpdate = DbContext.Pages.Where(w => w.PageSetId == pageset.Id).SelectMany(sm => sm.PageButton).Where(w => (w.NavigateToPage ?? 0) != 0);
                foreach (var navToChange in navBtnsToUpdate) {
                    var oldPageNav = navBtnsSourceToCopy.FirstOrDefault(x => x.ButtonDescription == navToChange.Description);
                    if (oldPageNav != null) {
                        var page = DbContext.Pages.FirstOrDefault(x => x.PageSetId == pageset.Id && x.Description == oldPageNav.PageDescription);
                        if (page != null) { navToChange.NavigateToPage = page.Id; } else { navToChange.NavigateToPage = null; }
                    } else { navToChange.NavigateToPage = null; }
                    DbContext.Entry(navToChange).State = EntityState.Modified;
                }
                model.Id = pageset.Id;
            } catch (Exception ex) { logger.Error(ex.ToString()); return null; }
            return model;
        }

        public int SaveChanges() {

            return DbContext.SaveChanges();
        }

    }
}
