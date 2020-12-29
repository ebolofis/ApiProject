using Pos_WebApi.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Pos_WebApi.Models;
using log4net;

namespace Pos_WebApi.Repositories.BORepos
{
    public class RegionRepository
    {
        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RegionRepository(PosEntities db)
        {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<RegionDTO> GetAll(Expression<Func<RegionDTO, bool>> predicate = null)
        {
            var query = from q in DbContext.Region.Include(i => i.PosInfo_Region_Assoc.Select(s => s.PosInfo))
                                                   .Include(i => i.Table)

                        select new RegionDTO
                        {
                            Id = q.Id,
                            Description = q.Description,
                            StartTime = q.StartTime,
                            EndTime = q.EndTime,
                            BluePrintPath = q.BluePrintPath,
                            MaxCapacity = q.MaxCapacity,
                            IsLocker = q.IsLocker,
                            AssociatedPos = q.PosInfo_Region_Assoc.Select(s => new PosInfo_Region_AssocDTO
                            {
                                Id = s.Id,
                                PosInfoId = s.PosInfoId,
                                RegionId = s.RegionId,
                                RegionDescription = s.Region.Description

                            }).ToList(),
                            AssociatedTables = q.Table.Select(s => new TableDTO
                            {
                                Id = s.Id,
                                Code = s.Code,
                                SalesDescription = s.SalesDescription,
                                Description = s.Description,
                                MinCapacity = s.MinCapacity,
                                MaxCapacity = s.MaxCapacity,
                                RegionId = s.RegionId,
                                Status = s.Status,
                                YPos = s.YPos,
                                XPos = s.XPos,
                                IsOnline = s.IsOnline,
                                ReservationStatus = s.ReservationStatus,
                                Shape = s.Shape,
                                TurnoverTime = s.TurnoverTime,
                                ImageUri = s.ImageUri,
                                Width = s.Width,
                                Height = s.Height,
                                Angle = s.Angle,
                                IsDeleted = s.IsDeleted

                            }).ToList(),

                        };
            if (predicate != null)
                return query.Where(predicate);
            return query;
        }

        public PagedResult<RegionDTO> GetPaged(Expression<Func<RegionDTO, bool>> predicate, Expression<Func<RegionDTO, string>> sortpredicate, int page = 1, int pageSize = 10)
        {
            var query = this.GetAll(predicate);/*.AsExpandable()*///.OrderBy(sortpredicate);

            var result = new PagedResult<RegionDTO>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            if (skip < 0)
                skip = 0;
            result.Results = query.OrderBy(o => o.Description).Skip(skip).Take(pageSize).ToList();
            return result;
        }

        public RegionDTO Add(RegionDTO model)
        {
            var entity = model.ToModel();

            DbContext.Region.Add(entity);
            model.Id = entity.Id;
            return model;
        }

        public IEnumerable<RegionDTO> AddRange(IEnumerable<RegionDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Add(entity);
            }
            return entities;
        }


        public RegionDTO Update(RegionDTO model)
        {

            var entity = DbContext.Region
                                  //.Include(i => i.Table)
                                  .Include(i => i.PosInfo_Region_Assoc)
                                  .FirstOrDefault(x => x.Id == model.Id);
            entity = model.UpdateModel(entity);

            #region Update Insert Section
            foreach (var ap in model.AssociatedTables.Where(w => (w.IsDeleted ?? false) == false))
            {
                if (ap.Id != 0)
                {
                    var m = DbContext.Table.FirstOrDefault(x => x.Id == ap.Id);
                    if (m != null)
                    {
                        DbContext.Entry(ap.UpdateModel(m)).State = EntityState.Modified;
                        //   DbContext.Entry(det.ToModel()).State = EntityState.Modified;
                    }
                //    DbContext.Entry(ap.ToModel()).State = EntityState.Modified;
                }
                else
                {
                    DbContext.Entry(ap.ToModel()).State = EntityState.Added;
                }
            }

            foreach (var ap in model.AssociatedPos.Where(w => w.IsDeleted == false))
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
            var aplToDelete = model.AssociatedTables.Where(w => w.IsDeleted == true);
            foreach (var btn in aplToDelete)
            {
                var b = DbContext.Table.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                {
                    var canDelete = DbContext.OrderDetail.FirstOrDefault(x => x.TableId == b.Id);
                    if (canDelete != null)
                    {
                        b.IsDeleted = true;
                        DbContext.Entry(b).State = EntityState.Modified;
                    }
                    else
                    {
                        DbContext.Table.Remove(b);
                    }
                }
            }

            var arToDelete = model.AssociatedPos.Where(w => w.IsDeleted == true);
            foreach (var btn in arToDelete)
            {
                var b = DbContext.PosInfo_Region_Assoc.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.PosInfo_Region_Assoc.Remove(b);
            }

            #endregion

            DbContext.Entry(entity).State = EntityState.Modified;
            model.Id = entity.Id;
            return model;
        }

        public IEnumerable<RegionDTO> UpdateRange(IEnumerable<RegionDTO> entities)
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
            var test = DbContext.OrderDetailInvoices.FirstOrDefault(f => f.RegionId == id);
            var entitytoRemove = DbContext.Region.FirstOrDefault(x => x.Id == id);

            if (test == null && entitytoRemove != null)
            {
                try
                {
                    foreach (var item in DbContext.Table.Where(w => w.RegionId == entitytoRemove.Id).ToList())
                    {
                        item.IsDeleted = true;
                        DbContext.Entry(item).State = EntityState.Modified;
                    }
                    foreach (var item in DbContext.PosInfo_Region_Assoc.Where(w => w.PosInfoId == entitytoRemove.Id).ToList())
                    {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }

                    DbContext.Region.Remove(entitytoRemove);
                    return true;
                }
                catch (Exception ex)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting Region : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    //entitytoRemove.IsDeleted = true;
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