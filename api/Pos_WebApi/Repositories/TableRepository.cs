using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos_WebApi.Repositories
{
    public class TableRepository:IDisposable
    {
      //  BussinessRepository br;
        protected PosEntities db;

        public TableRepository(PosEntities db)
        {
            this.db = db;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            //    br = new BussinessRepository(this.DbContext);
        }

    

        public Object GetOpenTablesPerRegionStatusOnly(long regionId)
        {
            using (BussinessRepository br = new BussinessRepository(db))
            {
                var querytemp = br.ReceiptDetailsForTablesBO(i => i.RegionId == regionId);
                var ids = querytemp.Select(s => s.ReceiptId).Distinct().ToList();
                var quer1 = br.GuestPaymentsFlat(x => ids.Contains(x.InvoicesId)).ToList();
                var querytempFinal = querytemp.ToList().GroupBy(g => g.TableId).Select(s => new
                {
                    TableId = s.FirstOrDefault().TableId,
                    //  TableCode = s.FirstOrDefault().TableCode,
                    PosInfoDesc = s.Select(sss => sss.PosInfoId).Select(x => x).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1),
                    Staff = s.Select(sss => sss.StaffFirstName).Select(x => x).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1),
                    StaffIds = s.Select(sss => sss.StaffId).Select(x => x).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1),
                    ReceiptId = s.Select(sss => sss.ReceiptId).Select(x => x).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1),
                    //     StaffIds = s.Select(sss => sss.Id).Select(x => x).Distinct().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1),
                    ColorStatusId = s.Count() == 0 ? 0 //Adeio
                                                         : s.All(a => a.PaidStatus == 0) ? 1//Mi timologimeno
                                                         : s.All(a => a.PaidStatus == 1) ? 2// timologimeno
                                                         : 3,// Μερικώς Εξοφλημένο
                });
                return new { TablesWithOrder = querytempFinal, RoomsForTable = quer1 };
            }
        }

        public IEnumerable<Object> GetKitchenInstructionsForTable(long tableId)
        {
            var query = from q in db.KitchenInstructionLogger
                        join qq in db.Staff on q.StaffId equals qq.Id
                        where q.EndOfDayId == null && q.TableId == tableId
                        select new
                        {
                            Id = q.Id,
                            Message = q.Description,
                            StaffCode = qq.Code,
                            Staff = qq.FirstName,
                            SendTS = q.SendTS,
                            Status = q.Status,
                            Origin = q.PdaModulId == null?0:1
                        };
            return query.ToList();
        }

        public Object GetSingleTable(long tableid)
        {
            using (BussinessRepository br = new BussinessRepository(db))
            {
                var quer = br.ReceiptDetailsBO(x => x.EndOfDayId == null
                                                       && x.Status != 5
                                                       && x.TableId == tableid
                                                       && x.PaidStatus != 2
                                                       && ((x.InvoiceType == 2 && x.PaidStatus == 0) || (x.InvoiceType != 2 && x.PaidStatus < 2))
                                                       ).Select(q => new
                                                       {
                                                           ReceiptId = q.ReceiptsId,
                                                           RegionId = q.RegionId,
                                                           Id = q.OrderDetailId,
                                                           ReceiptNo = q.ReceiptNo,
                                                           PosInfoId = q.PosInfoId,
                                                           TableId = q.TableId,
                                                           TableCode = q.TableCode,
                                                           TableLabel = q.TableLabel,
                                                           OrderId = q.OrderId,
                                                           OrderNo = q.OrderNo,
                                                           Cover = 0,
                                                           ItemCode = q.ItemCode,
                                                           Product = q.ItemDescr,
                                                           ProductId = q.ProductId,
                                                           Price = q.Price,
                                                           VatId = q.VatId,
                                                           VatCode = q.VatCode,
                                                           VatDesc = q.ItemVatRate,
                                                           PricelistDetailId = q.PriceListDetailId,
                                                           PricelistId = q.PricelistId,
                                                           PricelistDescr = q.PricelistDescr,
                                                           Qty = q.ItemQty,
                                                           KitchenId = q.KitchenId,
                                                           Guid = q.Guid,
                                                           Status = q.Status,
                                                           StatusTS = q.StatusTS,
                                                           PaidStatus = q.PaidStatus,
                                                           TotalAfterDiscount = q.ItemGross,
                                                           Staff = q.StaffId,
                                                           Discount = q.ItemDiscount,
                                                           SalesTypeId = q.SalesTypeId,
                                                           OrderDetailIgredientsId = q.OrderDetailIgredientsId,

                                                           ProductCategoryId = q.ProductCategoryId,
                                                           ReceiptSplitedDiscount = q.ReceiptSplitedDiscount,
                                                             //TablePaySuggestion = 
                                                             // Couver = q.co
                                                             IsExtra = q.IsExtra,
                                                           ItemRemark = q.ItemRemark
                                                       }).ToList();
                var ids = quer.Select(s => s.ReceiptId).Distinct().ToList();
                var quer1 = br.GuestPaymentsFlat(x => ids.Contains(x.InvoicesId)).ToList();
                var oids = quer.Select(s => s.OrderId).Distinct();
                var quer2 = br.OrderCovers(x => oids.Contains(x.Id)).ToList();
                var counter = br.OrderCounter(x => ids.Contains(x.Id)).ToList();
                IEnumerable<long?> distInvoiceIds = quer.Select(x => x.ReceiptId).Distinct();
                var deliveryCustomers = br.GetDeliveryCustomersForSelectedInvoices(distInvoiceIds).ToList();
                IEnumerable<long> distOrderIds = quer.Select(x => x.OrderId).Distinct();
                var externalInfo = br.GetOrderExternalInfo(distOrderIds).Distinct().ToList();
                return new { Items = quer, Payments = quer1, Covers = quer2, Counter = counter, DeliveryCustomers = deliveryCustomers, ExternalInfo = externalInfo };

            }


        }


        /// <summary>
        ///  get all tables for a specific pos ordered by Region and code
        /// </summary>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        public IEnumerable<Object> GetAllTables(long posInfoId)
        {
            var availRegionsForPos = db.PosInfo_Region_Assoc.Where(w => w.PosInfoId == posInfoId).AsEnumerable();
            var query = from q in db.Table.Where(w => w.IsDeleted == null || w.IsDeleted == false)
                        join qq in availRegionsForPos on q.RegionId equals qq.RegionId
                        select new
                        {

                            Id = q.Id,
                            Angle = q.Angle,
                            Code = q.Code,
                            Description = q.Description,
                            Height = q.Height,
                            ImageUri = q.ImageUri,
                            IsOnline = q.IsOnline,
                            MaxCapacity = q.MaxCapacity,
                            MinCapacity = q.MinCapacity,
                            RegionId = q.RegionId,
                            ReservationStatus = q.ReservationStatus,
                            SalesDescription = q.SalesDescription,
                            Shape = q.Shape,
                            Status = q.Status,
                            TurnoverTime = q.TurnoverTime,
                            Width = q.Width,
                            XPos = q.XPos,
                            YPos = q.YPos,
                        };

            return query.OrderBy(o => o.RegionId).ThenBy(t => t.Code).ToList();


        }

        public void Dispose()
        {
            
        }
    }


}
