using Pos_WebApi.Models.DTOModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using Pos_WebApi.Models;
using System.Collections.Generic;
using log4net;

namespace Pos_WebApi.Repositories.BORepos
{
    public class ExludedPricelistsRepository
    {
        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ExludedPricelistsRepository(PosEntities db)
        {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<ExcludedPricelistsDTO> GetAll(Expression<Func<ExcludedPricelistsDTO, bool>> predicate = null)
        {
            var query = from q in DbContext.PosInfoDetail_Pricelist_Assoc.Include(i => (i.PosInfoDetail.PosInfo))
                                                                         .Include(i => i.Pricelist)

                        select new ExcludedPricelistsDTO
                        {
                            Id = q.Id,
                            GroupId = q.PosInfoDetail.GroupId,
                            PosInfoDetailId = q.PosInfoDetailId,
                            PricelistId = q.PricelistId,
                            PosInfoId = q.PosInfoDetail.PosInfoId
                        };
            if (predicate != null)
                return query.Where(predicate);
            return query;
        }



        public PagedResult<ExcludedPricelistsDTO> GetPaged(Expression<Func<ExcludedPricelistsDTO, bool>> predicate, Expression<Func<ExcludedPricelistsDTO, string>> sortpredicate, int page = 1, int pageSize = 10)
        {
            var query = this.GetAll(predicate);/*.AsExpandable()*///.OrderBy(sortpredicate);

            var result = new PagedResult<ExcludedPricelistsDTO>();
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


        public ExcludedPricelistsDTO Add(ExcludedPricelistsDTO model)
        {
            var entity = model.ToModel();

            DbContext.PosInfoDetail_Pricelist_Assoc.Add(entity);
            model.Id = entity.Id;
            return model;
        }

        public IEnumerable<ExcludedPricelistsDTO> AddRange(IEnumerable<ExcludedPricelistsDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Add(entity);
            }
            return entities;
        }

        public ExcludedPricelistsDTO Update(ExcludedPricelistsDTO model)
        {
            if (model.Id == 0)
                return Add(model);
            if (model.IsDeleted)
            {
                Delete(model.Id);
                return model;
            }

            var entity = DbContext.PosInfoDetail_Pricelist_Assoc.FirstOrDefault(x => x.Id == model.Id);
            entity = model.UpdateModel(entity);
            return model;
        }



        public IEnumerable<ExcludedPricelistsDTO> UpdateRange(IEnumerable<ExcludedPricelistsDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Update(entity);
            }
            return entities;
        }


        public bool Delete(long? id)
        {
            var entitytoRemove = DbContext.PosInfoDetail_Pricelist_Assoc.FirstOrDefault(x => x.Id == id);

            if (entitytoRemove != null)
            {
                try
                {

                    DbContext.PosInfoDetail_Pricelist_Assoc.Remove(entitytoRemove);
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting ExcludedPricelistsDTO : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                   // entitytoRemove.IsDeleted = true;
                    DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                    return true;
                }
            }
            else
            {
                try
                {
              //      entitytoRemove.IsDeleted = true;
                    DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Marking as Deleting ExcludedPricelistsDTO : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
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