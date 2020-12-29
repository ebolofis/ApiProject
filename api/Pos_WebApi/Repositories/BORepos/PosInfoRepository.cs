using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using LinqKit;
using log4net;

namespace Pos_WebApi.Repositories.BORepos
{
    public class PosInfoRepository
    {
        protected PosEntities DbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PosInfoRepository(PosEntities db)
        {
            this.DbContext = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<PosInfoDTO> GetAll(Expression<Func<PosInfoDTO, bool>> predicate = null)
        {
            var query = from q in DbContext.PosInfo.Include(i => i.PosInfo_Pricelist_Assoc.Select(s => s.Pricelist))
                                                   .Include(i => i.PosInfoKdsAssoc.Select(s => s.Kds))
                                                   .Include(i => i.PosInfo_Region_Assoc.Select(s => s.Region))
                                                   .Include(i => i.PosInfo_KitchenInstruction_Assoc.Select(s => s.KitchenInstruction))
                                                   .Include(i => i.PosInfo_StaffPositin_Assoc.Select(s => s.StaffPosition))
                                                   .Include(i => i.Accounts)

                        select new PosInfoDTO
                        {
                            Id = q.Id,
                            ClearTableManually = q.ClearTableManually,
                            CloseId = q.CloseId,
                            FiscalType = q.FiscalType,
                            IsOpen = q.IsOpen,
                            LogInToOrder = q.LogInToOrder,
                            ResetsReceiptCounter = q.ResetsReceiptCounter,
                            Theme = q.Theme,
                            ReceiptCount = q.ReceiptCount,
                            ViewOnly = q.ViewOnly,
                            wsIP = q.wsIP,
                            wsPort = q.wsPort,

                            Code = q.Code,
                            Description = q.Description,
                            DepartmentId = q.DepartmentId,
                            AccountId = q.AccountId,
                            ClientPos = q.ClientPos,
                            FiscalName = q.FiscalName,
                            IPAddress = q.IPAddress,
                            FODay = q.FODay,
                            Type = q.Type,
                            LoginToOrderMode = q.LoginToOrderMode ?? 0,
                            KeyboardType = q.KeyboardType ?? 0,
                            DefaultHotelId = q.DefaultHotelId,
                            CustomerDisplayGif = q.CustomerDisplayGif,
                            NfcDevice = q.NfcDevice,
                            Configuration = q.Configuration,
                            AssosiatedPricelists = q.PosInfo_Pricelist_Assoc.Select(s => new PosInfo_Pricelist_AssocDTO
                            {
                                Id = s.Id,
                                PosInfoId = s.PosInfoId,
                                PricelistId = s.PricelistId,
                                PricelistDescription = s.Pricelist.Description

                            }).ToList(),
                            AssosiatedKDS = q.PosInfoKdsAssoc.Select(s => new PosInfoKdsAssocDTO
                            {
                                Id = s.Id,
                                PosInfoId = s.PosInfoId,
                                KdsId = s.KdsId,
                                KdsDescription = s.Kds.Description

                            }).ToList(),
                            AssosiatedRegions = q.PosInfo_Region_Assoc.Select(s => new PosInfo_Region_AssocDTO
                            {
                                Id = s.Id,
                                PosInfoId = s.PosInfoId,
                                RegionId = s.RegionId,
                                RegionDescription = s.Region.Description

                            }).ToList(),
                            AssosiatedKitchenInstructions = q.PosInfo_KitchenInstruction_Assoc.Select(s => new PosInfo_KitchenInstruction_AssocDTO
                            {
                                Id = s.Id,
                                PosInfoId = s.PosInfoId,
                                KitchenInstructionId = s.KitchenInstructionId,
                                KitchenInstructionDescription = s.KitchenInstruction.Description

                            }).ToList(),
                            AssosiatedStaffPositions = q.PosInfo_StaffPositin_Assoc.Select(s => new PosInfo_StaffPosition_AssocDTO
                            {
                                Id = s.Id,
                                PosInfoId = s.PosInfoId,
                                StaffPositionId = s.StaffPositionId,
                                StaffPositionDescription = s.StaffPosition.Description

                            }).ToList(),
                            Accounts = q.AccountId != null ? new AccountsDTO
                            {
                                Id = q.Accounts.Id,
                                Description = q.Accounts.Description,
                                IsDefault = q.Accounts.IsDefault,
                                SendsTransfer = q.Accounts.SendsTransfer,
                                Type = q.Accounts.Type

                            } : null

                        };
            if (predicate != null)
                return query.Where(predicate);
            return query;
        }


        public PagedResult<PosInfoDTO> GetPaged(Expression<Func<PosInfoDTO, bool>> predicate, Expression<Func<PosInfoDTO, string>> sortpredicate, int page = 1, int pageSize = 10)
        {
            var query = this.GetAll(predicate);/*.AsExpandable()*///.OrderBy(sortpredicate);

            var result = new PagedResult<PosInfoDTO>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            if (skip < 0)
                skip = 0;
             result.Results = query.OrderBy(o=>o.Code).Skip(skip).Take(pageSize).ToList();

            return result;
        }

        public PosInfoDTO Add(PosInfoDTO model)
        {
            var entity = model.ToModel();

            DbContext.PosInfo.Add(entity);
            model.Id = entity.Id;
            return model;
        }

        public IEnumerable<PosInfoDTO> AddRange(IEnumerable<PosInfoDTO> entities)
        {
            foreach (var entity in entities)
            {
                this.Add(entity);
            }
            return entities;
        }

        public PosInfoDTO Update(PosInfoDTO model)
        {

            var entity = DbContext.PosInfo
                                  .Include(i => i.PosInfo_Pricelist_Assoc)
                                  .Include(i => i.PosInfoKdsAssoc)
                                  .Include(i => i.PosInfo_Region_Assoc)
                                  .Include(i => i.PosInfo_KitchenInstruction_Assoc)
                                  .FirstOrDefault(x => x.Id == model.Id);
            entity = model.UpdateModel(entity);

            #region Update Insert Section
            foreach (var ap in model.AssosiatedPricelists.Where(w => w.IsDeleted == false))
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

            foreach (var ap in model.AssosiatedKDS.Where(w => w.IsDeleted == false))
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

            foreach (var ap in model.AssosiatedRegions.Where(w => w.IsDeleted == false))
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


            foreach (var ap in model.AssosiatedKitchenInstructions.Where(w => w.IsDeleted == false))
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


            foreach (var ap in model.AssosiatedStaffPositions.Where(w => w.IsDeleted == false))
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
            var aplToDelete = model.AssosiatedPricelists.Where(w => w.IsDeleted == true);
            foreach (var btn in aplToDelete)
            {
                var b = DbContext.PosInfo_Pricelist_Assoc.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.PosInfo_Pricelist_Assoc.Remove(b);
            }

            var akdsToDelete = model.AssosiatedKDS.Where(w => w.IsDeleted == true);
            foreach (var btn in akdsToDelete)
            {
                var b = DbContext.PosInfoKdsAssoc.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.PosInfoKdsAssoc.Remove(b);
            }

            var arToDelete = model.AssosiatedRegions.Where(w => w.IsDeleted == true);
            foreach (var btn in arToDelete)
            {
                var b = DbContext.PosInfo_Region_Assoc.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.PosInfo_Region_Assoc.Remove(b);
            }

            var akiToDelete = model.AssosiatedKitchenInstructions.Where(w => w.IsDeleted == true);
            foreach (var btn in akiToDelete)
            {
                var b = DbContext.PosInfo_KitchenInstruction_Assoc.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.PosInfo_KitchenInstruction_Assoc.Remove(b);
            }

            var astToDelete = model.AssosiatedStaffPositions.Where(w => w.IsDeleted == true);
            foreach (var btn in astToDelete)
            {
                var b = DbContext.PosInfo_StaffPositin_Assoc.Where(w => w.Id == btn.Id).SingleOrDefault();
                if (b != null)
                    DbContext.PosInfo_StaffPositin_Assoc.Remove(b);
            }
            #endregion

            DbContext.Entry(entity).State = EntityState.Modified;
            model.Id = entity.Id;
            return model;
        }

        public IEnumerable<PosInfoDTO> UpdateRange(IEnumerable<PosInfoDTO> entities)
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
            var test = DbContext.Order.FirstOrDefault(f => f.PosId == id);
            var entitytoRemove = DbContext.PosInfo.FirstOrDefault(x => x.Id == id);

            if (test == null && entitytoRemove != null)
            {
                try
                {
                    foreach (var item in DbContext.PosInfo_Pricelist_Assoc.Where(w => w.PosInfoId == entitytoRemove.Id).ToList())
                    {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in DbContext.PosInfoKdsAssoc.Where(w => w.PosInfoId == entitytoRemove.Id).ToList())
                    {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in DbContext.PosInfo_Region_Assoc.Where(w => w.PosInfoId == entitytoRemove.Id).ToList())
                    {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in DbContext.PosInfo_KitchenInstruction_Assoc.Where(w => w.PosInfoId == entitytoRemove.Id).ToList())
                    {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }

                    foreach (var item in DbContext.PosInfoDetail.Where(w => w.PosInfoId == entitytoRemove.Id).ToList())
                    {
                        DbContext.Entry(item).State = EntityState.Deleted;
                    }

                    DbContext.PosInfo.Remove(entitytoRemove);
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Deleting PosInfo : " + id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
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
