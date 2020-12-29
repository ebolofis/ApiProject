using Pos_WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data;
using System.Data.Entity.Infrastructure;
using Pos_WebApi.Models;
using log4net;

namespace Pos_WebApi.Repositories
{
    public class Repository<TEntity> :
       IRepository<TEntity> where TEntity : class
    {
        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Repository(PosEntities db)
        {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = this.DbContext.Set<TEntity>();
            return query;
        }

        //public  Expression<Func<TEntity, bool>> PredicateBuilder(IEnumerable<WhereClauseModel> parameters)
        //{
        //    foreach(var param in parameters)
        //    {
        //        var predicate = PredicateBuilder.False<TEntity>();
        //        foreach (string keyword in keywords)
        //        {
        //            string temp = keyword;
        //            predicate = predicate.Or(p => p.Description.Contains(temp));
        //        }
        //        return predicate;

        //    }


        //    return null;
        //}

        public PagedResult<TEntity> GetPaged(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, string>> sortpredicate, int page = 0, int pageSize = 10)
        {
            var query = this.DbContext.Set<TEntity>().Where(predicate).OrderBy(sortpredicate);

            var result = new PagedResult<TEntity>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }


        public IQueryable<TEntity> Include(Expression<Func<TEntity, object>> predicate)
        {
            IQueryable<TEntity> query = this.DbContext.Set<TEntity>().Include(predicate);
            return query;
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = this.DbContext.Set<TEntity>().Where(predicate);
            return query;
        }

        public TEntity Add(TEntity entity)
        {
            return this.DbContext.Set<TEntity>().Add(entity);
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.DbContext.Set<TEntity>().Add(entity);
            }
            return entities;
        }

        public IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbContext.Entry(entity).State = EntityState.Modified;
            }
            return entities;
        }

        public IEnumerable<TEntity> DeleteRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbContext.Set<TEntity>().Remove(entity);
            }
            return entities;
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entitytoRemove = DbContext.Set<TEntity>().Where(predicate).FirstOrDefault();
            if (entitytoRemove != null)
                DbContext.Set<TEntity>().Remove(entitytoRemove);
        }

        public void Update(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
        }


        public bool SaveChanges()
        {
            try
            {
                DbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                // return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                return false;
            }
        }
    }


    public class WhereClauseModel
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public byte ClauseType { get; set; }
    }
}
