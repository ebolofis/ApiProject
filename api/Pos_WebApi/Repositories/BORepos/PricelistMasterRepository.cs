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

    public class PricelistMasterRepository
    {
        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PricelistMasterRepository(PosEntities db)
        {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<PricelistMasterDTO> GetAll(Expression<Func<PricelistMasterDTO, bool>> predicate = null)
        {
            var query = DbContext.PricelistMaster.Include(i => i.SalesType_PricelistMaster_Assoc.Select(s=>s.SalesType)).Select(s=> new PricelistMasterDTO
            {
                             Id = s.Id,
                             Description = s.Description,
                             Status = s.Status,
                             Active = s.Active,
                             AssociatedSalesTypes = s.SalesType_PricelistMaster_Assoc.Select(ss => new SalesType_PricelistMaster_AssocDTO
                             {
                                 Id = ss.Id,
                                 PricelistMasterId = ss.PricelistMasterId,
                                 SalesTypeId = ss.SalesTypeId,
                                 SalesTypeDescription = ss.SalesType.Description
                             }).Distinct().ToList(),

                         });

            if (predicate != null)
                return query.Where(predicate);
            return query;
        }

        public PagedResult<PricelistMasterDTO> GetPaged(Expression<Func<PricelistMasterDTO, bool>> predicate, Expression<Func<PricelistMasterDTO, string>> sortpredicate, int page = 1, int pageSize = 10)
        {
            var query = this.GetAll(predicate).OrderBy(sortpredicate);

            var result = new PagedResult<PricelistMasterDTO>();
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

        public PricelistMasterDTO Add(PricelistMasterDTO model)
        {
            var entity = model.ToModel();

            DbContext.PricelistMaster.Add(entity);
            model.Id = entity.Id;
            return model;
        }

        public IEnumerable<PricelistMasterDTO> AddRange(IEnumerable<PricelistMasterDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Add(entity);
            }
            return entities;
        }


        public PricelistMasterDTO Update(PricelistMasterDTO model)
        {
            var entity = DbContext.PricelistMaster.FirstOrDefault(x => x.Id == model.Id);
            var staff = model.UpdateModel(entity);

            foreach (var det in model.AssociatedSalesTypes.Where(w => w.IsDeleted == false))
            {
                if (det.Id != 0)
                {
                    var m = DbContext.SalesType_PricelistMaster_Assoc.FirstOrDefault(x => x.Id == det.Id);
                    if (m != null)
                    {
                        DbContext.Entry(det.UpdateModel(m)).State = EntityState.Modified;
                        //   DbContext.Entry(det.ToModel()).State = EntityState.Modified;
                    }
                }
                else
                {
                    DbContext.Entry(det.ToModel()).State = EntityState.Added;
                }
            }

            var apsToDelete = model.AssociatedSalesTypes.Where(w => w.IsDeleted == true);
            foreach (var btn in apsToDelete)
            {
                var b = DbContext.SalesType_PricelistMaster_Assoc.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    //      DbContext.Entry(btn.ToModel()).State = EntityState.Deleted;
                    DbContext.SalesType_PricelistMaster_Assoc.Remove(b);
            }

            DbContext.Entry(staff).State = EntityState.Modified;
            model.Id = staff.Id;
            return model;
        }

        public IEnumerable<PricelistMasterDTO> UpdateRange(IEnumerable<PricelistMasterDTO> entities)
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
            var test = DbContext.Pricelist.FirstOrDefault(f => f.PricelistMasterId == id);
            var entitytoRemove = DbContext.PricelistMaster.FirstOrDefault(x => x.Id == id);

            if (test == null && entitytoRemove != null)
            {
                try
                {

                    foreach (var item in DbContext.SalesType_PricelistMaster_Assoc.Where(w => w.PricelistMasterId == entitytoRemove.Id).ToList())
                    {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    
                    DbContext.PricelistMaster.Remove(entitytoRemove);
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting Products : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    //entitytoRemove.IsDeleted = true;
                    //DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                    return true;
                }
            }
            else
            {
                try
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("PricelistMaster Is in use "));
                    //entitytoRemove.IsDeleted = true;
                    //DbContext.Entry(entitytoRemove).State = EntityState.Modified;
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Marking as Deleting Products : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
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