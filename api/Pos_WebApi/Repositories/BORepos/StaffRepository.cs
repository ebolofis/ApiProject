using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using log4net;

namespace Pos_WebApi.Repositories.BORepos
{
    public class StaffRepository
    {
        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StaffRepository(PosEntities db)
        {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<StaffDTO> GetAll(Expression<Func<StaffDTO, bool>> predicate = null)
        {
            var query = (from q in DbContext.Staff
                         join qq in DbContext.StaffAuthorization on q.Id equals qq.StaffId into f
                         from sa in f.DefaultIfEmpty()
                         join qqq in DbContext.AssignedPositions on q.Id equals qqq.StaffId into ff
                         from ap in ff.DefaultIfEmpty()
                         join qqqq in DbContext.DA_Stores on q.DASTORE equals qqqq.Id into fff
                         from das in fff.DefaultIfEmpty()
                         where q.IsDeleted == null || q.IsDeleted == false
                         select new
                         {
                             Id = q.Id,
                             Code = q.Code,
                             FirstName = q.FirstName,
                             LastName = q.LastName,
                             ImageUri = q.ImageUri,
                             Password = q.Password,
                             Identification = q.Identification,
                             DaStoreId = q.DASTORE,
                             isAdmin= q.isAdmin,
                             LogInAfterOrder = q.LogInAfterOrder,
                             DaStoreDescription = das != null ? das.Title : null,

                             StaffAuthorizationId = sa != null ? sa.Id : -1,
                             AuthorizedGroupId = sa != null ? sa.AuthorizedGroupId : null,
                             StaffPositionId = ap != null ? ap.StaffPositionId : null,
                             AssignedPositionsId = ap != null ? ap.Id : -1
                         }).GroupBy(g => g.Id).Select(s => new StaffDTO
                         {
                             Id = s.FirstOrDefault().Id,
                             Code = s.FirstOrDefault().Code,
                             FirstName = s.FirstOrDefault().FirstName,
                             LastName = s.FirstOrDefault().LastName,
                             ImageUri = s.FirstOrDefault().ImageUri,
                             Password = s.FirstOrDefault().Password,
                             Identification = s.FirstOrDefault().Identification,
                             DaStoreId = s.FirstOrDefault().DaStoreId,
                             DaStoreDescription = s.FirstOrDefault().DaStoreDescription,
                             isAdmin= s.FirstOrDefault().isAdmin,
                             LogInAfterOrder = s.FirstOrDefault().LogInAfterOrder,
                             StaffAuthorization = s.Where(w => w.StaffAuthorizationId > 0).Select(ss => new StaffAuthorizationDTO
                             {
                                 Id = ss.StaffAuthorizationId,
                                 StaffId = ss.Id,
                                 AuthorizedGroupId = ss.AuthorizedGroupId
                             }).Distinct().ToList(),
                             StaffPositions = s.Where(w => w.AssignedPositionsId > 0).Select(ss => new StaffPositionDTO
                             {
                                 StaffId = ss.Id,
                                 StaffPositionId = ss.StaffPositionId,
                                 Id = ss.AssignedPositionsId
                             }).Distinct().ToList()
                         });

            if (predicate != null)
                return query.Where(predicate);


            return query;
        }

        public PagedResult<StaffDTO> GetPaged(Expression<Func<StaffDTO, bool>> predicate, Expression<Func<StaffDTO, string>> sortpredicate, int page = 1, int pageSize = 10)
        {
            var query = this.GetAll(predicate).OrderBy(sortpredicate);

            var result = new PagedResult<StaffDTO>();
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

        public StaffDTO Add(StaffDTO model)
        {
            bool isExist = false;
            var allStaffs = DbContext.Staff.Select(s => s).Where(w => w.IsDeleted == null).ToList();
            var staff = model.ToModel();
            if (string.IsNullOrEmpty(model.Identification) == false)
            {
                isExist = (from s in allStaffs where s.Identification == staff.Identification || s.Code == staff.Code select s).Any();
            }
            else
            {
                isExist = (from s in allStaffs where s.Code == staff.Code select s).Any();
            }
            if (isExist == false)
            {
                DbContext.Staff.Add(staff);
                model.Id = staff.Id;
                return model;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<StaffDTO> AddRange(IEnumerable<StaffDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Add(entity);
            }
            return entities;
        }


        public StaffDTO Update(StaffDTO model)
        {
            var entity = DbContext.Staff.Include(i => i.AssignedPositions).Include(i => i.AssignedPositions).FirstOrDefault(x => x.Id == model.Id);
            var staff = model.UpdateModel(entity);

            foreach (var ap in model.StaffPositions.Where(w=>w.IsDeleted == false))
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

            foreach (var sa in model.StaffAuthorization.Where(w => w.IsDeleted == false))
            {
                if (sa.Id != 0)
                {
                    DbContext.Entry(sa.ToModel()).State = EntityState.Modified;
                }
                else
                {
                    DbContext.Entry(sa.ToModel()).State = EntityState.Added;
                }
            }

            var apsToDelete = model.StaffPositions.Where(w => w.IsDeleted == true);
            foreach (var btn in apsToDelete)
            {
                var b = DbContext.AssignedPositions.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
              //      DbContext.Entry(btn.ToModel()).State = EntityState.Deleted;
                DbContext.AssignedPositions.Remove(b);
            }

            var saToDelete = model.StaffAuthorization.Where(w => w.IsDeleted == true);
            foreach (var btn in saToDelete)
            {
               // DbContext.Entry(btn.ToModel()).State = EntityState.Deleted;
                var b = DbContext.StaffAuthorization.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.StaffAuthorization.Remove(b);
            }


            DbContext.Entry(staff).State = EntityState.Modified;
            model.Id = staff.Id;
            return model;
        }

        public IEnumerable<StaffDTO> UpdateRange(IEnumerable<StaffDTO> entities)
        {
            bool isExist = false;
            var allStaffs = DbContext.Staff.Select(s => s).Where(w => w.IsDeleted == null).ToList();
            foreach (var entity in entities)
            {
                if (string.IsNullOrEmpty(entity.Identification) == false)
                {
                    isExist = (from s in allStaffs where s.Id != entity.Id && (s.Identification == entity.Identification || s.Code == entity.Code) select s).Any();
                }
                else
                {
                    isExist = (from s in allStaffs where s.Id != entity.Id && s.Code == entity.Code select s).Any();
                }
                if (isExist == false)
                {
                    this.Update(entity);
                }
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
            var test = DbContext.Order.FirstOrDefault(f => f.StaffId == id);
            var entitytoRemove = DbContext.Staff.FirstOrDefault(x => x.Id == id);

            if (test == null && entitytoRemove != null)
            {
                try
                {

                    foreach (var item in DbContext.AssignedPositions.Where(w => w.StaffId == entitytoRemove.Id).ToList())
                    {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in DbContext.StaffAuthorization.Where(w => w.StaffId == entitytoRemove.Id).ToList())
                    {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }

                    DbContext.Staff.Remove(entitytoRemove);
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting Products : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
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
