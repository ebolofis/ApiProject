using Newtonsoft.Json;
using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Pos_WebApi.Repositories.BORepos
{
    public class GenericRepository<TEntity>// :IGenericRepository<TEntity> 
        where TEntity : class
    {

        protected PosEntities DbContext;

        public GenericRepository(PosEntities db)
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
        public IQueryable<TEntity> GetAllSorted(Expression<Func<TEntity, string>> sortpredicate) {
            IQueryable<TEntity> query = this.DbContext.Set<TEntity>().OrderBy(sortpredicate);
            return query;
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

        public string GetJsonForFiscal(KitchenInstructionLogger model)
        {
            var updatedModel = DbContext.KitchenInstructionLogger.Include(i => i.KitchenInstruction).FirstOrDefault(x => x.PosInfoId == model.PosInfoId 
                                                                                   //&& x.SendTS == model.SendTS 
                                                                                   && x.ReceivedTS == null
                                                                                   && x.ExtecrName == model.ExtecrName
                                                                                   && x.Status == 0
                                                                                   && x.EndOfDayId == null
                                                                                   && x.KicthcenInstuctionId == model.KicthcenInstuctionId 
                                                                                   && x.StaffId == model.StaffId);
            var st = DbContext.Staff.Find(updatedModel.StaffId);
            var tbl = DbContext.Table.Find(updatedModel.TableId);

            var res = new
            {
                ExtcerInstance = updatedModel.ExtecrName,
                KitchenId = updatedModel.KitchenInstruction.KitchenId,
                Message = updatedModel.Description,
                Waiter = st.LastName,
                Table = tbl.Code,
                ReceivedDate = model.SendTS.Value.ToString("yyyy/MM/dd"),
                ReceivedTime = model.SendTS.Value.ToString("HH:mm")
            };

            return JsonConvert.SerializeObject(res);

        }
        public IQueryable<TEntity> Include(Expression<Func<TEntity, object>> predicate)
        {
            IQueryable<TEntity> query = this.DbContext.Set<TEntity>().Include(predicate);
            return query;
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate) {
            IQueryable<TEntity> query = this.DbContext.Set<TEntity>().Where(predicate);
            return query;
        }
        public IQueryable<TEntity> FindBySorted(Expression<Func<TEntity, bool>> predicate , Expression<Func<TEntity, string>> sortpredicate) {
            IQueryable<TEntity> query = this.DbContext.Set<TEntity>().Where(predicate).OrderBy(sortpredicate); ;
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

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }
    }

}
