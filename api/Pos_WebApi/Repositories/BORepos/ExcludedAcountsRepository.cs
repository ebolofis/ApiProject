using log4net;
using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Pos_WebApi.Repositories.BORepos
{
    public class ExcludedAcountsRepository
    {

        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ExcludedAcountsRepository(PosEntities db)
        {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<ExcludedAccountsDTO> GetAll(Expression<Func<ExcludedAccountsDTO, bool>> predicate = null)
        {
            var query = from q in DbContext.PosInfoDetail_Excluded_Accounts.Include(i => (i.PosInfoDetail.PosInfo))
                                                                         .Include(i => i.Accounts)

                        select new ExcludedAccountsDTO
                        {
                            Id = q.Id,
                            GroupId = q.PosInfoDetail.GroupId,
                            PosInfoDetailId = q.PosInfoDetailId,
                            AccountId = q.AccountId,
                            PosInfoId = q.PosInfoDetail.PosInfoId
                        };
            if (predicate != null)
                return query.Where(predicate);
            return query;
        }



        public PagedResult<ExcludedAccountsDTO> GetPaged(Expression<Func<ExcludedAccountsDTO, bool>> predicate, Expression<Func<ExcludedAccountsDTO, string>> sortpredicate, int page = 1, int pageSize = 10)
        {
            var query = this.GetAll(predicate);/*.AsExpandable()*///.OrderBy(sortpredicate);

            var result = new PagedResult<ExcludedAccountsDTO>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            if (skip < 0)
                skip = 0;
            result.Results = query.OrderBy(o => o.GroupId).Skip(skip).Take(pageSize).ToList();

            return result;
        }


        public ExcludedAccountsDTO Add(ExcludedAccountsDTO model)
        {
            var entity = model.ToModel();

            DbContext.PosInfoDetail_Excluded_Accounts.Add(entity);
            model.Id = entity.Id;
            return model;
        }

        public IEnumerable<ExcludedAccountsDTO> AddRange(IEnumerable<ExcludedAccountsDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Add(entity);
            }
            return entities;
        }

        public ExcludedAccountsDTO Update(ExcludedAccountsDTO model)
        {
            if (model.Id == 0)
                return Add(model);
            if (model.IsDeleted)
            {
                Delete(model.Id);
                return model;
            }
            var entity = DbContext.PosInfoDetail_Excluded_Accounts.FirstOrDefault(x => x.Id == model.Id);
                entity = model.UpdateModel(entity);
            return model;
        }



        public IEnumerable<ExcludedAccountsDTO> UpdateRange(IEnumerable<ExcludedAccountsDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Update(entity);
            }
            return entities;
        }


        public bool Delete(long? id)
        {
            var entitytoRemove = DbContext.PosInfoDetail_Excluded_Accounts.FirstOrDefault(x => x.Id == id);

            if (entitytoRemove != null)
            {
                try
                {

                    DbContext.PosInfoDetail_Excluded_Accounts.Remove(entitytoRemove);
                    return true;
                }
                catch (Exception ex)
                {
                    //  Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting ExcludedAccountsDTO : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    //  entitytoRemove.IsDeleted = true;
                    logger.Error(ex.ToString());
                    DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                    return true;
                }
            }
            else
            {
                try
                {
               //     entitytoRemove.IsDeleted = true;
                    DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    //  Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Marking as Deleting ExcludedAccountsDTO : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    return false;
                }

            }
        }

        public IEnumerable<long> DeleteRange(IEnumerable<long> ids)
        {
            foreach (var id in ids)
            {
                this.Delete(id);
            }
            return ids;
        }



        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }
    }
}