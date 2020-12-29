using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class TableController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/Table
        public IEnumerable<Table> GetTables(string storeid)
        {
            return db.Table.AsEnumerable();
        }

        public Object GetTables(string storeid, long regionId, long posInfoId, bool forPda, bool perRegion)
        {
            var tables = db.Table.Where(w => w.RegionId == regionId);
            var query = TableModelCreator(tables, posInfoId, true);
            //var query = TableModelCreatorNew(regionId, null, posInfoId, true);
            return query.AsEnumerable();
        }


        public IEnumerable<Table> GetTables(string storeid, long regionId)
        {
            return db.Table.Where(w => w.RegionId == regionId).AsEnumerable();
        }


        public Object GetTables(string storeid, long regionId, long posInfoId, bool fortable)
        {
            var tables = db.Table.Where(w => w.RegionId == regionId);
            var query = TableModelCreator(tables, posInfoId, false);
            //var query = TableModelCreatorNew(regionId, null, posInfoId, false);
            return query;
        }

        public Object GetTableById(long tableid, bool gettable, long posInfoId, bool n)
        {
            var tables = db.Table.Where(w => w.Id == tableid);
            var query = TableModelCreator(tables, posInfoId, false).FirstOrDefault();
            return query;
            var query2 = (from q in db.Table.Where(w => w.Id == tableid)
                          let ll = db.OrderDetail.Where(w => w.TableId == q.Id && w.PaidStatus != 2 && w.Status != 5)//.ToList()
                          from od in ll.DefaultIfEmpty()
                          let l3 = db.Order.Where(w => w.EndOfDayId == null && od.OrderId == w.Id)
                          from o in l3.DefaultIfEmpty()
                          let l4 = db.Product.Where(w => w.Id == od.ProductId)
                          from p in l4.DefaultIfEmpty()
                          let l5 = db.OrderDetailVatAnal.Where(w => w.OrderDetailId == od.Id)//.ToList()
                          from odiva in l5.DefaultIfEmpty()
                          let l6 = db.Vat.Where(w => w.Id == odiva.VatId)
                          from vod in l6.DefaultIfEmpty()
                          let l7 = db.PricelistDetail.Where(w => w.Id == od.PriceListDetailId)
                          from pld in l7.DefaultIfEmpty()
                          let l8 = db.Staff.Where(w => w.Id == o.StaffId)
                          from ordstaff in l8.DefaultIfEmpty()
                          let l9 = db.OrderDetailIgredients.Where(w => w.OrderDetailId == od.Id)
                          from oding in l9.DefaultIfEmpty()
                          let l10 = db.TablePaySuggestion.Where(w => w.OrderDetailId == od.Id)
                          from tps in l10.DefaultIfEmpty()
                          let l = db.OrderDetailInvoices.Where(w => w.OrderDetailId == od.Id)
                          from odi in l.DefaultIfEmpty()

                          //let l5 = OrderDetailIgredientVatAnal.Where(w=>w.OrderDetailIgredientsId == oding.Id) from odingva in  l5.DefaultIfEmpty()

                          //let l7 =  from tbl in l7.DefaultIfEmpty()
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
                              //OrderDetails = od,

                              OrderDetailId = od != null ? od.Id : -1,
                              OrderId = od != null ? od.OrderId : null,
                              PricelistDetailId = od != null ? od.PriceListDetailId : null,
                              Qty = od != null ? od.Qty : null,
                              KitchenId = od != null ? od.KitchenId : null,
                              Guid = od != null ? od.Guid : null,
                              OrderDetailStatus = od != null ? od.Status : null,
                              PaidStatus = od != null ? od.PaidStatus : null,
                              TotalAfterDiscount = od != null ? od.TotalAfterDiscount : null,
                              Discount = od != null ? od.Discount : null,
                              Couver = od != null ? od.Couver : null,
                              OrderDetailProductId = od != null ? od.ProductId : null,
                              Price = od != null ? od.Price : null,
                              IsDeleted = od != null ? od.IsDeleted : false,

                              PosId = o != null ? o.PosId : null,
                              OrderNo = o != null ? o.OrderNo : null,
                              ProductDescription = p != null ? p.SalesDescription : "",

                              VatCode = vod != null ? vod.Code : null,
                              VatDesc = vod != null ? vod.Description : "",

                              PricelistId = pld != null ? pld.PricelistId : -1,

                              StaffId = ordstaff != null ? ordstaff.Id : -1,
                              FirstName = ordstaff != null ? ordstaff.FirstName : "",
                              LastName = ordstaff != null ? ordstaff.LastName : "",

                              IngredientId = oding != null ? oding.Id : -1,
                              IngPrice = oding != null ? oding.Price : 0,
                              IngTotalAfterDiscount = oding != null ? oding.TotalAfterDiscount : 0,
                              IngQty = oding != null ? oding.Qty : 0,
                              IngDiscount = oding != null ? oding.Discount : 0,
                              //IngVatCode = 
                              //IngVatDesc = 
                              ProductCategoryId = p.ProductCategoryId,


                              AccountId = tps != null ? tps.AccountId : null,
                              Amount = tps != null ? tps.Amount : null,
                              GuestId = tps != null ? tps.GuestId : null,
                              // Guest = gst,
                              CreditCodeId = tps != null ? tps.CreditCodeId : null,
                              //   CreditCode = cc != null ? cc.Code : null,
                              IsUsed = tps != null ? tps.IsUsed : false,

                              InvoicesId = odi != null ? odi.InvoicesId : -1,
                              IsPrinted = odi != null ? odi.IsPrinted : null,
                              //IsDeleted = odi != null ? odi.IsDeleted : null,
                              Counter = odi != null ? odi.Counter : 0,
                              CreationTS = odi != null ? odi.CreationTS : null,
                              CustomerId = odi != null ? odi.CustomerId : null,
                              OrderDetailInvoicesId = odi != null ? odi.Id : -1,
                              PosInfoDetailId = odi != null ? odi.PosInfoDetailId : null,
                              OdiStaffId = odi != null ? odi.StaffId : -1,
                              //				
                              //				//Ingredients = oding,
                              //				day = od.StatusTS,
                          }).ToList()
 .GroupBy(g => g.Id).Select(s => new
 {
     Id = s.FirstOrDefault().Id,
     Angle = s.FirstOrDefault().Angle,
     Code = s.FirstOrDefault().Code,
     Description = s.FirstOrDefault().Description,
     Height = s.FirstOrDefault().Height,
     ImageUri = s.FirstOrDefault().ImageUri,
     IsOnline = s.FirstOrDefault().IsOnline,
     MaxCapacity = s.FirstOrDefault().MaxCapacity,
     MinCapacity = s.FirstOrDefault().MinCapacity,
     RegionId = s.FirstOrDefault().RegionId,
     ReservationStatus = s.FirstOrDefault().ReservationStatus,
     SalesDescription = s.FirstOrDefault().SalesDescription,
     Shape = s.FirstOrDefault().Shape,
     Status = s.FirstOrDefault().Status,
     TurnoverTime = s.FirstOrDefault().TurnoverTime,
     Width = s.FirstOrDefault().Width,
     XPos = s.FirstOrDefault().XPos,
     YPos = s.FirstOrDefault().YPos,
     OrderDetail = s.Where(w => w.OrderDetailId != -1 /*&& w.IsDeleted == false*/).Select(ss => new
     {
         PosInfoId = ss.PosId,
         Id = ss.OrderDetailId,
         ProductId = ss.OrderDetailProductId,
         OrderId = ss.OrderId,
         OrderNo = ss.OrderNo,
         Product = ss.ProductDescription,
         Price = ss.Price,
         VatCode = ss.VatCode,
         VatDesc = ss.VatDesc,
         PricelistDetailId = ss.PricelistDetailId,
         Qty = ss.Qty,
         KitchenId = ss.KitchenId,
         Guid = ss.Guid,
         Status = ss.OrderDetailStatus,
         PaidStatus = ss.PaidStatus,
         TotalAfterDiscount = ss.TotalAfterDiscount,
         Discount = ss.Discount,
         Couver = ss.Couver,
         PricelistId = ss.PricelistId,
         TotalOrderDetailIgredients = s.Sum(sm => sm.IngTotalAfterDiscount),
         ProductCategoryId = ss.ProductCategoryId,
         Staff = new
         {
             Id = ss.StaffId,
             FirstName = ss.FirstName,
             LastName = ss.LastName
         },
         OrderDetailInvoices = s.Where(w => w.OrderDetailId == ss.OrderDetailId).GroupBy(gg => gg.OrderDetailId).Select(sss => new
         {
             OrderDetailId = sss.FirstOrDefault().OrderDetailId,
             InvoicesId = sss.FirstOrDefault().InvoicesId,
             IsPrinted = sss.FirstOrDefault().IsPrinted,
             IsDeleted = sss.FirstOrDefault().IsDeleted,
             Counter = sss.FirstOrDefault().Counter,
             CreationTS = sss.FirstOrDefault().CreationTS,
             CustomerId = sss.FirstOrDefault().CustomerId,
             Id = sss.FirstOrDefault().OrderDetailInvoicesId,
             PosInfoDetailId = sss.FirstOrDefault().PosInfoDetailId,
             StaffId = sss.FirstOrDefault().OdiStaffId,
         }),
         OrderDetailIgredients = s.Where(w => w.OrderDetailId == ss.OrderDetailId && w.IngredientId != -1).GroupBy(gg => gg.OrderDetailId).Select(sss => new
         {
             OrderDetailId = sss.FirstOrDefault().OrderDetailId,
             //Description = sss.FirstOrDefault().Description,
             IngredientId = sss.FirstOrDefault().IngredientId,
             Price = sss.FirstOrDefault().IngPrice,
             TotalAfterDiscount = sss.FirstOrDefault().IngTotalAfterDiscount,
             Qty = sss.FirstOrDefault().IngQty,
             Discount = sss.FirstOrDefault().IngDiscount,
             //VatCode = sss.FirstOrDefault().VatCode,
             //VatDesc = sss.FirstOrDefault().VatDesc
         }),
         TablePaySuggestion = s.Where(w => w.OrderDetailId == ss.OrderDetailId && w.IngredientId != -1).GroupBy(gg => gg.OrderDetailId).Select(sss => new
         {
             OrderDetailId = sss.FirstOrDefault().OrderDetailId,
             AccountId = sss.FirstOrDefault().AccountId,
             Amount = sss.FirstOrDefault().Amount,
             GuestId = sss.FirstOrDefault().GuestId,
             //Guest = gst,
             CreditCodeId = sss.FirstOrDefault().CreditCodeId,
             // CreditCode = cc != null ? cc.Code : null,
             IsUsed = sss.FirstOrDefault().IsUsed
         })


     })
     //						OrderDetailInvoices = ss.Select(sss=>sss.OrderDetailInvoices).Distinct(),
 });
            var newq = from q in query2
                       select new
                       {
                           RegionId = q.RegionId,
                           Id = q.Id,
                           Code = q.Code,
                           //aa = q.OrderDetail.Select(ss => ss.PaidStatus),
                           ColorStatus = q.OrderDetail.Count() > 0
                                 ? q.Status == 1
                                  ? q.OrderDetail.All(a => a.PaidStatus == 2)
                                   ? "badge-info"//Ola Exoflimena
                                   : q.OrderDetail.Any(a => a.PaidStatus == 2)
                                    ? "badge-warning"//Merikos Exoflimeno
                                    : "badge-danger"//Mh exoflimeno
                                  : "badge-success"//Full
                                 : "",
                           Status = q.Status,
                           OrderDetail = q.OrderDetail,
                           Orders = ""//q.Orders
                       };


            if ( n != true)
                return newq.FirstOrDefault();
            else
                return query2.FirstOrDefault();
          //  return query;
        }

        private IEnumerable<Object> TableModelCreator(IEnumerable<Table> tables, long posInfoId, bool forpda)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.LazyLoadingEnabled = true;

            var validOrders = db.Order.Where(w => w.EndOfDayId == null);

            var validOds = from q in db.OrderDetail.Where(w => w.Status != 5 && w.IsDeleted == null && (w.PaidStatus != 2 || w.PaidStatus == null))
                           join qq in validOrders on q.OrderId equals qq.Id
                           select q;
            var orderStaff = from q in validOrders
                             join qq in db.Staff on q.StaffId equals qq.Id
                             select new
                             {
                                 OrderId = q.Id,
                                 Staff = new
                                 {
                                     Id = qq.Id,
                                     FirstName = qq.FirstName,
                                     LastName = qq.LastName
                                 }
                             };

            var vats = from q in db.OrderDetailVatAnal
                       join qq in validOds on q.OrderDetailId equals qq.Id
                       join v in db.Vat on q.VatId equals v.Id
                       select new
                       {
                           OrderDetailId = qq.Id,
                           VatCode = v.Code,
                           VatDesc = v.Percentage
                       };
            var tablepaySug = from q in db.TablePaySuggestion
                              join qq in validOds on q.OrderDetailId equals qq.Id
                              join qqq in db.Guest on q.GuestId equals qqq.Id into f
                              from gst in f.DefaultIfEmpty()
                              join qqqq in db.CreditCodes on q.CreditCodeId equals qqqq.Id into ff
                              from cc in ff.DefaultIfEmpty()
                              select new
                              {
                                  OrderDetailId = q.OrderDetailId,
                                  AccountId = q.AccountId,
                                  Amount = q.Amount,
                                  GuestId = q.GuestId,
                                  Guest = gst,
                                  CreditCodeId = q.CreditCodeId,
                                  CreditCode = cc != null ? cc.Code : null,
                                  IsUsed = q.IsUsed
                              };
            var validOdingsIds = from q in db.OrderDetailIgredients
                                 join vods in validOds on q.OrderDetailId equals vods.Id
                                 select new { q.Id };

            var ingVats =  from q in db.OrderDetailIgredientVatAnal
                          join qq in db.OrderDetailIgredients on q.OrderDetailIgredientsId equals qq.Id
                          join vods in validOdingsIds on q.OrderDetailIgredientsId equals vods.Id
                          join v in db.Vat on q.VatId equals v.Id
                          select new
                               {
                                   OrderDetailIgredientsId = qq.Id,
                                   VatCode = v.Code,
                                   VatDesc = v.Percentage
                               };
            var odings  = from q in db.OrderDetailIgredients
                         join qq in validOds on q.OrderDetailId equals qq.Id
                         join qqq in db.Ingredients on q.IngredientId equals qqq.Id
                         join qqqq in ingVats on q.Id equals qqqq.OrderDetailIgredientsId
                         select new {
                             OrderDetailId = q.OrderDetailId,
                             Description = qqq.Description,
                             IngredientId = q.IngredientId,
                             Price = q.Price,
                             TotalAfterDiscount = q.TotalAfterDiscount,
                             Qty = q.Qty,
                             Discount = q.Discount,
                             VatCode = qqqq.VatCode,
                             VatDesc = qqqq.VatDesc
                         };
            var orderdetails = from q in validOds
                               join o in db.Order on q.OrderId equals o.Id
                               join p in db.Product on q.ProductId equals p.Id
                               join t in tables on q.TableId equals t.Id
                               join s in orderStaff on q.OrderId equals s.OrderId
                               join v in vats on q.Id equals v.OrderDetailId
                               join qq in odings on q.Id equals qq.OrderDetailId into f
                               from oding in f.DefaultIfEmpty()
                               join qqq in db.OrderDetailInvoices on q.Id equals qqq.OrderDetailId into ff
                               from odi in ff.DefaultIfEmpty()
                               join qqqq in tablepaySug on q.Id equals qqqq.OrderDetailId into fff
                               from tps in fff.DefaultIfEmpty()
                               select new
                               {
                                   Id = q.Id,
                                   PosInfoId = o.PosId,
                                   TableId = q.TableId,
                                   OrderId = q.OrderId,
                                   OrderNo = o.OrderNo,
                                   Product = p.SalesDescription,
                                   ProductId = q.ProductId,
                                   Price = q.Price,
                                   VatCode = v.VatCode,
                                   VatDesc = v.VatDesc,
                                   PricelistDetailId = q.PriceListDetailId,
                                   PricelistId = q.PricelistDetail.PricelistId,
                                   Qty = q.Qty,
                                   KitchenId = q.KitchenId,
                                   Guid = q.Guid,
                                   Status = q.Status,
                                   PaidStatus = q.PaidStatus,
                                   OrderDetailInvoices = odi,
                                   TotalAfterDiscount = q.TotalAfterDiscount,
                                   TotalOrderDetailIgredients = oding.TotalAfterDiscount,//oding != null ? oding.TotalAfterDiscount : null,
                                   Staff = s.Staff,
                                   Couver = q.Couver,
                                   Discount = q.Discount,
                                   OrderDetailIgredients = oding,
                                   TablePaySuggestion = tps,
                                   ProductCategoryId = p.ProductCategoryId,
                                   Table = t,
                               };

            var query2a = orderdetails.ToList().GroupBy(g => new { g.TableId }).Select(s => new
            {
                Id = s.FirstOrDefault().Table.Id,
                Angle = s.FirstOrDefault().Table.Angle,
                Code = s.FirstOrDefault().Table.Code,
                Description = s.FirstOrDefault().Table.Description,
                Height = s.FirstOrDefault().Table.Height,
                ImageUri = s.FirstOrDefault().Table.ImageUri,
                IsOnline = s.FirstOrDefault().Table.IsOnline,
                MaxCapacity = s.FirstOrDefault().Table.MaxCapacity,
                MinCapacity = s.FirstOrDefault().Table.MinCapacity,
                RegionId = s.FirstOrDefault().Table.RegionId,
                ReservationStatus = s.FirstOrDefault().Table.ReservationStatus,
                SalesDescription = s.FirstOrDefault().Table.SalesDescription,
                Shape = s.FirstOrDefault().Table.Shape,
                Status = s.FirstOrDefault().Table.Status,
                TurnoverTime = s.FirstOrDefault().Table.TurnoverTime,
                Width = s.FirstOrDefault().Table.Width,
                XPos = s.FirstOrDefault().Table.XPos,
                YPos = s.FirstOrDefault().Table.YPos,
                OrderDetail = s.GroupBy(gg=>gg.Id).Select(ss => new
                {
                    Id = ss.FirstOrDefault().Id,
                    PosInfoId = ss.FirstOrDefault().PosInfoId,
                    TableId = ss.FirstOrDefault().TableId,
                    OrderId = ss.FirstOrDefault().OrderId,
                    OrderNo = ss.FirstOrDefault().OrderNo,
                    Product = ss.FirstOrDefault().Product,
                    ProductId = ss.FirstOrDefault().ProductId,
                    Price = ss.FirstOrDefault().Price,
                    VatCode = ss.FirstOrDefault().VatCode,
                    VatDesc = ss.FirstOrDefault().VatDesc,
                    PricelistDetailId = ss.FirstOrDefault().PricelistDetailId,
                    PricelistId = ss.FirstOrDefault().PricelistId,
                    Qty = ss.FirstOrDefault().Qty,
                    KitchenId = ss.FirstOrDefault().KitchenId,
                    Guid = ss.FirstOrDefault().Guid,
                    Status = ss.FirstOrDefault().Status,
                    PaidStatus = ss.FirstOrDefault().PaidStatus,
                    OrderDetailInvoices = ss.Select(sss=>sss.OrderDetailInvoices).Distinct(),
                    TotalAfterDiscount = (Decimal?)ss.FirstOrDefault().TotalAfterDiscount,
                    TotalOrderDetailIgredients = ss.Distinct().Sum(sm => sm.TotalOrderDetailIgredients) == 0 ? null : ss.Select(s1=>s1.OrderDetailIgredients) .Distinct().Sum(sm => sm.TotalAfterDiscount),
                    Staff = ss.FirstOrDefault().Staff,
                    Couver = ss.FirstOrDefault().Couver,
                    Discount = ss.FirstOrDefault().Discount,
                    OrderDetailIgredients = ss.Select(sss => sss.OrderDetailIgredients).Distinct().FirstOrDefault() != null?
                        //ss.Distinct().Sum(sm => sm.TotalOrderDetailIgredients) == 0 ? 
                        ss.Select(sss => sss.OrderDetailIgredients).Distinct().ToList<dynamic>() : new List<dynamic>(),
                    TablePaySuggestion = ss.Select(sss=>sss.TablePaySuggestion).Distinct(),
                    ProductCategoryId = ss.FirstOrDefault().ProductCategoryId,
                }).Distinct()
            });

            var query2 = (from q in tables.ToList()
                          join qq in query2a on q.Id equals qq.Id into f
                          from bus in f.DefaultIfEmpty()
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
                              //     OrderDetail = bus != null? bus.OrderDetail:null
                              OrderDetail = bus != null ? bus.OrderDetail.Distinct().ToList<dynamic>() : new List<dynamic>()

                          }).ToList();
            /*
            var query = (from q in tables
                         join r in db.PosInfo_Region_Assoc.Where(f => f.PosInfoId == posInfoId) on q.RegionId equals r.RegionId
                         join j in db.OrderDetail.Include(p => p.Product)
                         .Where(ww => (ww.PaidStatus != 2 || ww.PaidStatus == null) && ww.Status != 5 && ww.Order.EndOfDayId == null && (ww.IsDeleted ?? false == false))
                         .Select(ss => new
                         {
                             Id = ss.Id,
                             TableId = ss.TableId,
                             PosInfoId = ss.Order.PosId,
                             OrderId = ss.Order.Id,
                             OrderNo = ss.Order.OrderNo,
                             Product = ss.Product.Description,
                             ProductId = ss.ProductId,
                             Price = ss.Price,
                             VatCode = ss.PricelistDetail.Vat.Code,
                             VatDesc = ss.PricelistDetail.Vat.Percentage,
                             PricelistDetailId = ss.PriceListDetailId,
                             Qty = ss.Qty,
                             KitchenId = ss.KitchenId,
                             Guid = ss.Guid,
                             Status = ss.Status,
                             PaidStatus = ss.PaidStatus,
                             OrderDetailInvoices = ss.OrderDetailInvoices,
                             ProductCategoryId = ss.Product.ProductCategoryId,
                             OrderDetailIgredients = ss.OrderDetailIgredients.Select(so => new
                             {
                                 Description = so.Ingredients != null ? so.Ingredients.Description : "null",
                                 IngredientId = so.IngredientId,
                                 Price = so.Price,
                                 TotalAfterDiscount = so.TotalAfterDiscount,
                                 Qty = so.Qty,
                                 Discount = so.Discount,
                                 VatCode = so.PricelistDetail != null ? so.PricelistDetail.Vat.Code : -1,
                                 VatDesc = so.PricelistDetail != null ? so.PricelistDetail.Vat.Percentage : -1
                             }),
                             TotalOrderDetailIgredients = ss.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount),
                             Couver = ss.Couver,
                             Discount = ss.Discount,
                             Staff = new
                             {
                                 Id = ss.Order.Staff.Id,
                                 FirstName = ss.Order.Staff.FirstName,
                                 LastName = ss.Order.Staff.LastName
                             },
                             TotalAfterDiscount = ss.TotalAfterDiscount,
                             TablePaySuggestion = ss.TablePaySuggestion.Select(so => new
                             {
                                 AccountId = so.AccountId,
                                 Amount = so.Amount,
                                 GuestId = so.Guest != null ? so.Guest.Id : 0,
                                 Guest = so.Guest,//so.GuestId != null ? so.GuestId : null,
                                 CreditCodeId = so.CreditCodeId,
                                 CreditCode = so.CreditCodes != null ? so.CreditCodes.Code : null,
                                 IsUsed = so.IsUsed
                             }),
                             IsDeleted = ss.IsDeleted
                         })
                         on q.Id equals j.TableId into jj
                         from ja in jj.DefaultIfEmpty()
                         select new
                         {
                             Table = q,
                             OrderDetail = ja
                         }).ToList().GroupBy(g => g.Table.Id);

            var query2 = query.Select(s => new
                         {
                             Id = s.FirstOrDefault().Table.Id,
                             Angle = s.FirstOrDefault().Table.Angle,
                             Code = s.FirstOrDefault().Table.Code,
                             Description = s.FirstOrDefault().Table.Description,
                             Height = s.FirstOrDefault().Table.Height,
                             ImageUri = s.FirstOrDefault().Table.ImageUri,
                             IsOnline = s.FirstOrDefault().Table.IsOnline,
                             MaxCapacity = s.FirstOrDefault().Table.MaxCapacity,
                             MinCapacity = s.FirstOrDefault().Table.MinCapacity,
                             RegionId = s.FirstOrDefault().Table.RegionId,
                             ReservationStatus = s.FirstOrDefault().Table.ReservationStatus,
                             SalesDescription = s.FirstOrDefault().Table.SalesDescription,
                             Shape = s.FirstOrDefault().Table.Shape,
                             Status = s.FirstOrDefault().Table.Status,
                             TurnoverTime = s.FirstOrDefault().Table.TurnoverTime,
                             Width = s.FirstOrDefault().Table.Width,
                             XPos = s.FirstOrDefault().Table.XPos,
                             YPos = s.FirstOrDefault().Table.YPos,
                             OrderDetail = s.Select(ss => ss).Where(w => w.OrderDetail != null && !s.All(a => a.OrderDetail.Status == 5) && (w.OrderDetail.IsDeleted ?? false) == false
                            ).Select(sss => new  // && w.OrderDetail.OrderStatus.Where(www => www.Status == 5).Count() == 0
                            {
                                Id = sss.OrderDetail.Id,
                                PosInfoId = sss.OrderDetail.PosInfoId,
                                TableId = sss.OrderDetail.TableId,
                                OrderId = sss.OrderDetail.OrderId,
                                OrderNo = sss.OrderDetail.OrderNo,
                                Product = sss.OrderDetail.Product,
                                ProductId = sss.OrderDetail.ProductId,
                                Price = sss.OrderDetail.Price,
                                VatCode = sss.OrderDetail.VatCode,
                                VatDesc = sss.OrderDetail.VatDesc,
                                PricelistDetailId = sss.OrderDetail.PricelistDetailId,
                                Qty = sss.OrderDetail.Qty,
                                KitchenId = sss.OrderDetail.KitchenId,
                                Guid = sss.OrderDetail.Guid,
                                Status = sss.OrderDetail.Status,
                                PaidStatus = sss.OrderDetail.PaidStatus,
                                OrderDetailInvoices = sss.OrderDetail.OrderDetailInvoices,
                                TotalAfterDiscount = sss.OrderDetail.TotalAfterDiscount,
                                TotalOrderDetailIgredients = sss.OrderDetail.TotalOrderDetailIgredients,
                                Staff = sss.OrderDetail.Staff,
                                Couver = sss.OrderDetail.Couver,
                                Discount = sss.OrderDetail.Discount,
                                OrderDetailIgredients = sss.OrderDetail.OrderDetailIgredients,
                                TablePaySuggestion = sss.OrderDetail.TablePaySuggestion,
                                ProductCategoryId = sss.OrderDetail.ProductCategoryId
                            })
                         }).ToList();
              */
            var newq = from q in query2
                       select new
                       {
                           RegionId = q.RegionId,
                           Id = q.Id,
                           Code = q.Code,
                           //aa = q.OrderDetail.Select(ss => ss.PaidStatus),
                           ColorStatus = q.OrderDetail.Count() > 0
                                 ? q.Status == 1
                                  ? q.OrderDetail.All(a => a.PaidStatus == 2)
                                   ? "badge-info"//Ola Exoflimena
                                   : q.OrderDetail.Any(a => a.PaidStatus == 2)
                                    ? "badge-warning"//Merikos Exoflimeno
                                    : "badge-danger"//Mh exoflimeno
                                  : "badge-success"//Full
                                 : "",
                           Status = q.Status,
                           OrderDetail = q.OrderDetail,
                           Orders = ""//q.Orders
                       };

            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = true;

            if (forpda == true)
                return newq;
            else
                return query2;
        }


        private IEnumerable<Object> TableModelCreatorNew(Int64? regionId, Int64? tableId, long posInfoId, bool forpda)
        {
            db.Configuration.AutoDetectChangesEnabled = false;

            //var tables = db.Table.Where(w => w.RegionId == regionId);
            //if (tableId != null)
            //{
            //    tables = tables.Where(w => w.Id == tableId);
            //}
            //else
            //    tables = tables.Where(w => w.RegionId == regionId);

            var query2 = (from q in db.Table.Where(w => w.RegionId == regionId)
                          let ll = db.OrderDetail.Where(w => w.TableId == q.Id && w.PaidStatus != 2 && w.Status != 5)//.ToList()
                          from od in ll.DefaultIfEmpty()
                          let l3 = db.Order.Where(w => w.EndOfDayId == null && od.OrderId == w.Id)
                          from o in l3.DefaultIfEmpty()
                          let l4 = db.Product.Where(w => w.Id == od.ProductId)
                          from p in l4.DefaultIfEmpty()
                          let l5 = db.OrderDetailVatAnal.Where(w => w.OrderDetailId == od.Id)//.ToList()
                          from odiva in l5.DefaultIfEmpty()
                          let l6 = db.Vat.Where(w => w.Id == odiva.VatId)
                          from vod in l6.DefaultIfEmpty()
                          let l7 = db.PricelistDetail.Where(w => w.Id == od.PriceListDetailId)
                          from pld in l7.DefaultIfEmpty()
                          let l8 = db.Staff.Where(w => w.Id == o.StaffId)
                          from ordstaff in l8.DefaultIfEmpty()
                          let l9 = db.OrderDetailIgredients.Where(w => w.OrderDetailId == od.Id)
                          from oding in l9.DefaultIfEmpty()
                          let l10 = db.TablePaySuggestion.Where(w => w.OrderDetailId == od.Id)
                          from tps in l10.DefaultIfEmpty()
                          let l = db.OrderDetailInvoices.Where(w => w.OrderDetailId == od.Id)
                          from odi in l.DefaultIfEmpty()

                          //let l5 = OrderDetailIgredientVatAnal.Where(w=>w.OrderDetailIgredientsId == oding.Id) from odingva in  l5.DefaultIfEmpty()

                          //let l7 =  from tbl in l7.DefaultIfEmpty()
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
                              //OrderDetails = od,

                              OrderDetailId = od != null ? od.Id : -1,
                              OrderId = od != null ? od.OrderId : null,
                              PricelistDetailId = od != null ? od.PriceListDetailId : null,
                              Qty = od != null ? od.Qty : null,
                              KitchenId = od != null ? od.KitchenId : null,
                              Guid = od != null ? od.Guid : null,
                              OrderDetailStatus = od != null ? od.Status : null,
                              PaidStatus = od != null ? od.PaidStatus : null,
                              TotalAfterDiscount = od != null ? od.TotalAfterDiscount : null,
                              Discount = od != null ? od.Discount : null,
                              Couver = od != null ? od.Couver : null,
                              OrderDetailProductId = od != null ? od.ProductId : null,
                              Price = od != null ? od.Price : null,
                              IsDeleted = od != null ? od.IsDeleted : false,

                              PosId = o != null ? o.PosId : null,
                              OrderNo = o != null ? o.OrderNo : null,
                              ProductDescription = p != null ? p.SalesDescription : "",

                              VatCode = vod != null ? vod.Code : null,
                              VatDesc = vod != null ? vod.Description : "",

                              PricelistId = pld != null ? pld.PricelistId : -1,

                              StaffId = ordstaff != null ? ordstaff.Id : -1,
                              FirstName = ordstaff != null ? ordstaff.FirstName : "",
                              LastName = ordstaff != null ? ordstaff.LastName : "",

                              IngredientId = oding != null ? oding.Id : -1,
                              IngPrice = oding != null ? oding.Price : 0,
                              IngTotalAfterDiscount = oding != null ? oding.TotalAfterDiscount : 0,
                              IngQty = oding != null ? oding.Qty : 0,
                              IngDiscount = oding != null ? oding.Discount : 0,
                              //IngVatCode = 
                              //IngVatDesc = 
                              ProductCategoryId = p.ProductCategoryId,


                              AccountId = tps != null ? tps.AccountId : null,
                              Amount = tps != null ? tps.Amount : null,
                              GuestId = tps != null ? tps.GuestId : null,
                              // Guest = gst,
                              CreditCodeId = tps != null ? tps.CreditCodeId : null,
                              //   CreditCode = cc != null ? cc.Code : null,
                              IsUsed = tps != null ? tps.IsUsed : false,

                              InvoicesId = odi != null ? odi.InvoicesId : -1,
                              IsPrinted = odi != null ? odi.IsPrinted : null,
                              //IsDeleted = odi != null ? odi.IsDeleted : null,
                              Counter = odi != null ? odi.Counter : 0,
                              CreationTS = odi != null ? odi.CreationTS : null,
                              CustomerId = odi != null ? odi.CustomerId : null,
                              OrderDetailInvoicesId = odi != null ? odi.Id : -1,
                              PosInfoDetailId = odi != null ? odi.PosInfoDetailId : null,
                              OdiStaffId = odi != null ? odi.StaffId : -1,
                              //				
                              //				//Ingredients = oding,
                              //				day = od.StatusTS,
                          }).ToList()
         .GroupBy(g => g.Id).Select(s => new
         {
             Id = s.FirstOrDefault().Id,
             Angle = s.FirstOrDefault().Angle,
             Code = s.FirstOrDefault().Code,
             Description = s.FirstOrDefault().Description,
             Height = s.FirstOrDefault().Height,
             ImageUri = s.FirstOrDefault().ImageUri,
             IsOnline = s.FirstOrDefault().IsOnline,
             MaxCapacity = s.FirstOrDefault().MaxCapacity,
             MinCapacity = s.FirstOrDefault().MinCapacity,
             RegionId = s.FirstOrDefault().RegionId,
             ReservationStatus = s.FirstOrDefault().ReservationStatus,
             SalesDescription = s.FirstOrDefault().SalesDescription,
             Shape = s.FirstOrDefault().Shape,
             Status = s.FirstOrDefault().Status,
             TurnoverTime = s.FirstOrDefault().TurnoverTime,
             Width = s.FirstOrDefault().Width,
             XPos = s.FirstOrDefault().XPos,
             YPos = s.FirstOrDefault().YPos,
             OrderDetail = s.Where(w => w.OrderDetailId != -1 /*&& w.IsDeleted == false*/).Select(ss => new
             {
                 PosInfoId = ss.PosId,
                 Id = ss.OrderDetailId,
                 ProductId = ss.OrderDetailProductId,
                 OrderId = ss.OrderId,
                 OrderNo = ss.OrderNo,
                 Product = ss.ProductDescription,
                 Price = ss.Price,
                 VatCode = ss.VatCode,
                 VatDesc = ss.VatDesc,
                 PricelistDetailId = ss.PricelistDetailId,
                 Qty = ss.Qty,
                 KitchenId = ss.KitchenId,
                 Guid = ss.Guid,
                 Status = ss.OrderDetailStatus,
                 PaidStatus = ss.PaidStatus,
                 TotalAfterDiscount = ss.TotalAfterDiscount,
                 Discount = ss.Discount,
                 Couver = ss.Couver,
                 PricelistId = ss.PricelistId,
                 TotalOrderDetailIgredients = s.Sum(sm => sm.IngTotalAfterDiscount),
                 ProductCategoryId = ss.ProductCategoryId,
                 Staff = new
                  {
                      Id = ss.StaffId,
                      FirstName = ss.FirstName,
                      LastName = ss.LastName
                  },
                 OrderDetailInvoices = s.Where(w => w.OrderDetailId == ss.OrderDetailId).GroupBy(gg => gg.OrderDetailId).Select(sss => new
                {
                    OrderDetailId = sss.FirstOrDefault().OrderDetailId,
                    InvoicesId = sss.FirstOrDefault().InvoicesId,
                    IsPrinted = sss.FirstOrDefault().IsPrinted,
                    IsDeleted = sss.FirstOrDefault().IsDeleted,
                    Counter = sss.FirstOrDefault().Counter,
                    CreationTS = sss.FirstOrDefault().CreationTS,
                    CustomerId = sss.FirstOrDefault().CustomerId,
                    Id = sss.FirstOrDefault().OrderDetailInvoicesId,
                    PosInfoDetailId = sss.FirstOrDefault().PosInfoDetailId,
                    StaffId = sss.FirstOrDefault().OdiStaffId,
                }),
                 OrderDetailIgredients = s.Where(w => w.OrderDetailId == ss.OrderDetailId && w.IngredientId != -1).GroupBy(gg => gg.OrderDetailId).Select(sss => new
                 {
                     OrderDetailId = sss.FirstOrDefault().OrderDetailId,
                     //Description = sss.FirstOrDefault().Description,
                     IngredientId = sss.FirstOrDefault().IngredientId,
                     Price = sss.FirstOrDefault().IngPrice,
                     TotalAfterDiscount = sss.FirstOrDefault().IngTotalAfterDiscount,
                     Qty = sss.FirstOrDefault().IngQty,
                     Discount = sss.FirstOrDefault().IngDiscount,
                     //VatCode = sss.FirstOrDefault().VatCode,
                     //VatDesc = sss.FirstOrDefault().VatDesc
                 }),
                 TablePaySuggestion = s.Where(w => w.OrderDetailId == ss.OrderDetailId && w.IngredientId != -1).GroupBy(gg => gg.OrderDetailId).Select(sss => new
                  {
                      OrderDetailId = sss.FirstOrDefault().OrderDetailId,
                      AccountId = sss.FirstOrDefault().AccountId,
                      Amount = sss.FirstOrDefault().Amount,
                      GuestId = sss.FirstOrDefault().GuestId,
                      //Guest = gst,
                      CreditCodeId = sss.FirstOrDefault().CreditCodeId,
                      // CreditCode = cc != null ? cc.Code : null,
                      IsUsed = sss.FirstOrDefault().IsUsed
                  })


             })
             //						OrderDetailInvoices = ss.Select(sss=>sss.OrderDetailInvoices).Distinct(),
         });
            var newq = from q in query2
                       select new
                       {
                           RegionId = q.RegionId,
                           Id = q.Id,
                           Code = q.Code,
                           //aa = q.OrderDetail.Select(ss => ss.PaidStatus),
                           ColorStatus = q.OrderDetail.Count() > 0
                                 ? q.Status == 1
                                  ? q.OrderDetail.All(a => a.PaidStatus == 2)
                                   ? "badge-info"//Ola Exoflimena
                                   : q.OrderDetail.Any(a => a.PaidStatus == 2)
                                    ? "badge-warning"//Merikos Exoflimeno
                                    : "badge-danger"//Mh exoflimeno
                                  : "badge-success"//Full
                                 : "",
                           Status = q.Status,
                           OrderDetail = q.OrderDetail,
                           Orders = ""//q.Orders
                       };


            if (forpda == true)
                return newq;
            else
                return query2;
        }
        // GET api/Table/5
        public Table GetTable(long id, string storeid)
        {
            Table table = db.Table.Find(id);
            if (table == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return table;
        }

        public HttpResponseMessage PutTable(long id, int status, bool updatestatus)
        {
            var table = db.Table.Find(id);
            if (table != null)
            {
                table.Status = (byte)status;
                db.SaveChanges();
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception(Symposium.Resources.Errors.TABLENOTFOUND));
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT api/Table/5
        public HttpResponseMessage PutTable(long id, string storeid, Table table)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != table.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(table).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Table
        public HttpResponseMessage PostTable(Table table, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.Table.Add(table);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, table);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = table.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Table/5
        public HttpResponseMessage DeleteTable(long id, string storeid)
        {
            Table table = db.Table.Find(id);
            if (table == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Table.Remove(table);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, table);
        }

        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public static Func<PosEntities, string, IQueryable<Table>> GetCompiledTables;
    }

}