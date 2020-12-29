using LinqKit;
using log4net;
using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Pos_WebApi.Repositories.BORepos
{
    public class PayroleRepository<TEntity>
            where TEntity : class
    {
        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PayroleRepository(PosEntities db)
        {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }
        public IQueryable<PayroleDTO> GetAll(Expression<Func<PayroleDTO, bool>> predicate = null)
        {
            IQueryable<PayroleDTO> query = DbContext.Payrole.Select(s => new PayroleDTO
            {
                Id = s.Id,
                Identification = s.Identification,
                ActionDate = s.ActionDate,
                Type = s.Type,
                PosInfoId = s.PosInfoId,
                StaffId = s.StaffId,
                ShopId = s.ShopId,
            });
            if (predicate != null)
                return query.Where(predicate);
            else
                return query;
            //IQueryable<Payrole> query = s.DbContext.Set<Payrole>().Where(w => w.IsDeleted == null || w.IsDeleted == false);
            //return query;
        }

        public PagedResult<TEntity> GetPaged(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, string>> sortpredicate, int page = 1, int pageSize = 10)
        {
            var query = this.DbContext.Set<TEntity>().Where(predicate).OrderBy(sortpredicate);

            var result = new PagedResult<TEntity>();
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


        public PagedResult<PayroleDTO> GetPaged(Expression<Func<PayroleDTO, bool>> predicate, Expression<Func<PayroleDTO, string>> sortpredicate, int page = 1, int pageSize = 10)
        {
            var query = this.GetAll(predicate).AsExpandable();
            // var query = this.DbContext.Payrole.Where(w => w.IsDeleted == null || w.IsDeleted == false).Where(predicate).AsExpandable().OrderBy(sortpredicate);

            var result = new PagedResult<PayroleDTO>();
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


        public IQueryable<Payrole> Include(Expression<Func<Payrole, object>> predicate)
        {
            IQueryable<Payrole> query = this.DbContext.Set<Payrole>().Include(predicate);
            return query;
        }

        public IQueryable<Payrole> FindBy(Expression<Func<Payrole, bool>> predicate)
        {
            IQueryable<Payrole> query = this.DbContext.Set<Payrole>().Where(predicate);
            return query;
        }

        public PayroleDTO Add(PayroleDTO model)
        {
            var entity = model.ToModel();
            try
            {
                DbContext.Payrole.Add(entity);
                model.Id = entity.Id;
                return model;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting Payroles : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }



        public IEnumerable<PayroleDTO> AddRange(IEnumerable<PayroleDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Add(entity);
            }
            return entities;
        }



        public PayroleDTO Update(PayroleDTO model)
        {
            var entity = DbContext.Payrole.FirstOrDefault(x => x.Id == model.Id);
            entity = model.UpdateModel(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
            model.Id = entity.Id;
            return model;
        }


        public IEnumerable<PayroleDTO> UpdateRange(IEnumerable<PayroleDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Update(entity);
            }
            return entities;
        }



        public bool Delete(long? id)
        {
            var entitytoRemove = DbContext.Set<Payrole>().Where(w => w.Id == id).FirstOrDefault();
            if (entitytoRemove != null)
            {
                try
                {
                    DbContext.Set<Payrole>().Remove(entitytoRemove);
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting Payroles : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return false;
                }
            }
            return false;
        }

        public IEnumerable<Payrole> DeleteRange(IEnumerable<Payrole> entities)
        {
            foreach (var entity in entities)
            {
                DbContext.Set<Payrole>().Remove(entity);
            }
            return entities;
        }



        public bool RemoveDuplicatesPayroleExtras()
        {
            //try {
            //    var doublePrices = DbContext.PayroleExtras.ToList().GroupBy(g => new { g.PayroleId, g.IngredientId })
            //                              .Where(w => w.Count() > 1)
            //                              .Select(s => new {
            //                                  KeyToKeep = s.FirstOrDefault().Id,
            //                                  KeysToChange = s.Select(ss => ss.Id)
            //                              });
            //    StringBuilder sb = new StringBuilder();
            //    foreach (var keyToFixed in doublePrices) {
            //        var invalidKeys = keyToFixed.KeysToChange.Where(w => w != keyToFixed.KeyToKeep);
            //        var odisToFixed = DbContext.OrderDetail.Where(w => invalidKeys.Contains(w.PriceListDetailId.Value));


            //        var str = invalidKeys.Distinct().Aggregate("", (previous, next) => previous + ", " + next).Trim().Remove(0, 1);
            //        sb.AppendLine(string.Format("Delete from PayroleExtras where id in ({0})", str));
            //    }
            //    sb.AppendLine(string.Format("Update pricelistDetail set Price= 0 where price is null"));
            //    DbContext.Database.ExecuteSqlCommand(sb.ToString());
            //} catch (Exception ex) {
            //    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error removing  duplicate Prices | " + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
            //    return false;

            //}
            return true;
        }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }
    }
}
