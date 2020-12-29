using Pos_WebApi.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using Pos_WebApi.Models;
using log4net;

namespace Pos_WebApi.Repositories.BORepos
{
    public class AllowedMealsPerBoardRepository
    {
        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AllowedMealsPerBoardRepository(PosEntities db)
        {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<AllowedMealsPerBoardDTO> GetAll(Expression<Func<AllowedMealsPerBoardDTO, bool>> predicate = null)
        {
            var query = from q in DbContext.AllowedMealsPerBoard.Include(i => (i.AllowedMealsPerBoardDetails).Select(s => s.ProductCategories))
                                                                .Include(i => i.Pricelist)
                        select new AllowedMealsPerBoardDTO
                        {
                            Id = q.Id,
                            BoardId = q.BoardId,
                            BoardDescription = q.BoardDescription,
                            AllowedMeals = q.AllowedMeals,
                            AllowedDiscountAmount = q.AllowedDiscountAmount,
                            AllowedMealsChild = q.AllowedMealsChild,
                            AllowedDiscountAmountChild = q.AllowedDiscountAmountChild,
                            PriceListId = q.PriceListId,
                            PricelistDescription = q.Pricelist.Description,
                            Details = q.AllowedMealsPerBoardDetails.Select(s => new AllowedMealsPerBoardDetailsDTO
                            {
                                Id = s.Id,
                                ProductCategoryId = s.ProductCategoryId,
                                AllowedMealsPerBoardId = s.AllowedMealsPerBoardId,
                                ProductCategoryDescription = s.ProductCategories.Description
                            }).ToList()
                        };
            if (predicate != null)
                return query.Where(predicate);
            return query;
        }


        public PagedResult<AllowedMealsPerBoardDTO> GetPaged(Expression<Func<AllowedMealsPerBoardDTO, bool>> predicate, Expression<Func<AllowedMealsPerBoardDTO, string>> sortpredicate, int page = 1, int pageSize = 10)
        {
            var query = this.GetAll(predicate);/*.AsExpandable()*///.OrderBy(sortpredicate);

            var result = new PagedResult<AllowedMealsPerBoardDTO>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            if (skip < 0)
                skip = 0;
            result.Results = query.OrderBy(o => o.BoardId).Skip(skip).Take(pageSize).ToList();

            return result;
        }


        public AllowedMealsPerBoardDTO Add(AllowedMealsPerBoardDTO model)
        {
            var entity = model.ToModel();

            DbContext.AllowedMealsPerBoard.Add(entity);
            model.Id = entity.Id;
            return model;
        }

        public IEnumerable<AllowedMealsPerBoardDTO> AddRange(IEnumerable<AllowedMealsPerBoardDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Add(entity);
            }
            return entities;
        }


        public AllowedMealsPerBoardDTO Update(AllowedMealsPerBoardDTO model)
        {

            if (model.Id == 0)
                return Add(model);

            if (model.IsDeleted == true)
            {
                Delete(model.Id);
                return model;
            }
            var entity = DbContext.AllowedMealsPerBoard
                                  .Include(i => i.AllowedMealsPerBoardDetails)
                                  .FirstOrDefault(x => x.Id == model.Id);
            entity = model.UpdateModel(entity);

            #region Update Insert Section
            foreach (var ap in model.Details.Where(w => w.IsDeleted == false))
            {
                if (ap.Id != 0)
                {
                    DbContext.Entry(ap.ToModel()).State = EntityState.Modified;
                }
                else
                {
                    DbContext.Entry(ap.ToModel()).State = EntityState.Added;
                }
            }

            #endregion

            #region Delete Section
            var detToDelete = model.Details.Where(w => w.IsDeleted == true);
            foreach (var del in detToDelete)
            {
                var b = DbContext.AllowedMealsPerBoardDetails.Where(w => w.Id == del.Id).SingleOrDefault();
                if (b != null)
                    DbContext.AllowedMealsPerBoardDetails.Remove(b);
            }
            #endregion

            DbContext.Entry(entity).State = EntityState.Modified;
            model.Id = entity.Id;
            return model;
        }

        public IEnumerable<AllowedMealsPerBoardDTO> UpdateRange(IEnumerable<AllowedMealsPerBoardDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Update(entity);
            }
            return entities;
        }


        public IEnumerable<long> DeleteRange(IEnumerable<long> ids)
        {
            foreach (var id in ids)
            {
                this.Delete(id);
            }
            return ids;
        }

        public bool Delete(long? id)
        {
            // var test = DbContext.Guest.FirstOrDefault(f => f. == id);
            var entitytoRemove = DbContext.AllowedMealsPerBoard.FirstOrDefault(x => x.Id == id);

            if (/*test == null &&*/ entitytoRemove != null)
            {
                try
                {
                    foreach (var item in DbContext.AllowedMealsPerBoardDetails.Where(w => w.AllowedMealsPerBoardId == entitytoRemove.Id).ToList())
                    {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }

                    DbContext.AllowedMealsPerBoard.Remove(entitytoRemove);
                    return true;
                }
                catch (Exception ex)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting AllowedMealsPerBoard : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    logger.Error(ex.ToString());
                    entitytoRemove.IsDeleted = true;
                    DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                    return true;
                }
            }
            else {
                try
                {
                    entitytoRemove.IsDeleted = true;
                    DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Marking as Deleting AllowedMealsPerBoard : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return false;
                }

            }
        }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }
    }
}