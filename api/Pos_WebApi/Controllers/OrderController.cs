using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using Pos_WebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Symposium.Models.Enums;

using System.Web;
using System.Web.Http;
using Symposium.Helpers.Interfaces;
using Symposium.Helpers.Classes;

namespace Pos_WebApi.Controllers
{
    [Authorize]
    public class OrderController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        CustomJsonSerializers cjson = new CustomJsonSerializers() ; 
        // GET api/Order
        public IEnumerable<Order> GetOrders()
        {
            return db.Order.AsEnumerable();
        }

        //RECEIPT VIEW PAGER
        public Object GetOrders(string date, long posid, int page, int rows, int? searchNo, bool searchReceipt)
        {
            db.Configuration.LazyLoadingEnabled = true;
            var query = db.Order//.Include(i => i.EndOfDay).Include(i => i.Transactions).Include("OrderDetail").Include("OrderDetail.OrderDetailInvoices")
                .Where(w => w.EndOfDayId == null && w.PosId == posid).AsNoTracking().Select(s => new //Order()
                {
                    Id = s.Id,
                    Day = s.Day,
                    EndOfDayId = s.EndOfDayId,
                    Staff = new
                    {
                        Id = s.Staff.Id,
                        FirstName = s.Staff.FirstName,
                        LastName = s.Staff.LastName,
                        ImageUri = s.Staff.ImageUri,
                        Code = s.Staff.Code
                    },
                    StaffId = s.StaffId,
                    OrderNo = s.OrderNo,
                    ReceiptNo = s.ReceiptNo,
                    OrderStatus = s.OrderStatus.Select(ss => new
                    {
                        Id = ss.Id,
                        Status = ss.Status,
                        TimeChanged = ss.TimeChanged,
                        StaffId = ss.StaffId
                    }),
                    PosId = s.PosId,
                    PosInfo = new
                    {
                        Code = s.PosInfo.Code,
                        Description = s.PosInfo.Description,
                        Id = s.PosInfo.Id
                    },
                    Total = s.Total,
                    FODay = s.EndOfDay.FODay,
                    AccountId = s.Transactions.FirstOrDefault() != null ? s.Transactions.FirstOrDefault().AccountId : null,
                    PreparationTime = s.OrderDetail.Max(m => m.PreparationTime),
                    SalesType = s.OrderDetail.Select(ss => ss.SalesTypeId).Distinct().Count() == 1 ? s.OrderDetail.FirstOrDefault().SalesType.Abbreviation : null,
                    OrderDetail = s.OrderDetail.Select(ss => new
                    {
                        Id = ss.Id,
                        ProductId = ss.ProductId,
                        Description = ss.Product.Description,
                        VatCode = ss.PricelistDetail.Vat.Code,
                        VatDesc = ss.PricelistDetail.Vat.Percentage,
                        Qty = ss.Qty,
                        SalesTypeId = ss.SalesTypeId,
                        SalesType = ss.SalesType.Abbreviation,
                        Price = ss.Price,
                        OrderDetailIgredients = ss.OrderDetailIgredients.Select(sss => new
                        {
                            Id = sss.Id,
                            Qty = sss.Qty,
                            IngredientId = sss.IngredientId,
                            Description = sss.Ingredients.Description,
                            Price = sss.Price,
                            Units = sss.UnitId,
                            VatCode = sss.PricelistDetail.Vat.Code,
                            VatDesc = sss.PricelistDetail.Vat.Percentage
                        })
                    })
                });
            if (searchNo != null)
            {
                if (searchReceipt == true)
                {
                    query = query.Where(w => w.ReceiptNo == searchNo);
                }
                else
                {
                    query = query.Where(w => w.OrderNo == searchNo);
                }
            }
            int count = query.Count();
            int totalpages = 1;
            if (count > 0)
            {
                double pages = (double)count / rows;
                totalpages = Convert.ToInt16(Math.Ceiling(pages));

                if (page > totalpages) page = totalpages;
                int start = rows * page - rows;
                query = query.OrderByDescending(o => o.Id).Skip(start).Take(rows);
            }
            db.Configuration.LazyLoadingEnabled = false;
            return new { query, totalpages, page };
        }

        #region NEW RECEIPT VIEW PAGER
        //public Object GetOrders(string pidids, string date, long posid, int page, int rows, int? searchNo, bool searchReceipt, bool nopriced, bool nopaid)
        //{
        //    List<long> posinfodetids = JsonConvert.DeserializeObject<List<long>>(pidids);
        //    var orderinvoices = db.OrderDetailInvoices.Where(w => w.PosInfoDetailId != null && posinfodetids.Contains(w.PosInfoDetailId.Value)).AsQueryable();
        //    var orders = db.Order.Where(w => w.EndOfDayId == null && w.PosId == posid).AsQueryable();
        //    if (searchNo != null)
        //    {
        //        if (searchReceipt == true)
        //        {
        //            orderinvoices = orderinvoices.Where(w => w.Counter == searchNo);
        //        }
        //        else
        //        {
        //            orders = orders.Where(w => w.OrderNo == searchNo);
        //        }
        //    }
        //    var q1 = (from q in orderinvoices
        //              // join gu in db.Guest on q.CustomerId equals SqlFunctions.StringConvert((decimal?)gu.Id)
        //              join j in db.OrderDetail on q.OrderDetailId equals j.Id
        //              join jj in db.Product on j.ProductId equals jj.Id
        //              join jjj in db.PosInfoDetail on q.PosInfoDetailId equals jjj.Id
        //              join jjjj in db.Staff on q.StaffId equals jjjj.Id
        //              join os in db.OrderDetailIgredients on j.Id equals os.OrderDetailId into ff
        //              from jodi in ff.DefaultIfEmpty()
        //              join ing in db.Ingredients on jodi.IngredientId equals ing.Id into fff
        //              from jing in fff.DefaultIfEmpty()
        //              join jpos in db.PosInfo.Include(i => i.Department) on jjj.PosInfoId equals jpos.Id
        //              join jord in orders on j.OrderId equals jord.Id
        //              join jpda in db.PdaModule on jord.PdaModuleId equals jpda.Id into jpdaleft
        //              from jpda2 in jpdaleft.DefaultIfEmpty()
        //              join jtr in db.Transactions on j.TransactionId equals jtr.Id into jtrleft
        //              from jtr2 in jtrleft.DefaultIfEmpty()
        //              //  join gu in db.Guest on q.CustomerId equals SqlFunctions.StringConvert((decimal?)gu.Id) into guj
        //              //  from gu in db.Guest.Where(f => SqlFunctions.StringConvert((double?)f.Id) == q.CustomerId).DefaultIfEmpty()
        //              join jg in db.Guest on j.GuestId equals jg.Id into jjg
        //              from jjjg in jjg.DefaultIfEmpty()
        //              join jtble in db.Table on j.TableId equals jtble.Id into jjtbl
        //              from jjjtbl in jjtbl.DefaultIfEmpty()
        //              select new
        //              {
        //                  // CustId = jjjg != null ? SqlFunctions.StringConvert((decimal)jjjg.Id) : "",
        //                  Cust = jjjg,
        //                  TableCode = jjjtbl.Code,
        //                  PosInfoDetailId = q.PosInfoDetailId,
        //                  Counter = q.Counter,
        //                  PosInfoDetail = jjj,

        //                  CreationTS = q.CreationTS,
        //                  IsPrinted = q.IsPrinted,
        //                  AccountObj = jtr2.Accounts != null ? jtr2.Accounts : null,
        //                  Account = jtr2.Accounts != null ? jtr2.Accounts.Description : "",
        //                  AccountId = jtr2.Accounts != null ? jtr2.Accounts.Id : -1,
        //                  StaffId = q.StaffId,
        //                  Staff = new
        //                  {
        //                      Id = jjjj.Id,
        //                      FirstName = jjjj.FirstName,
        //                      LastName = jjjj.LastName,
        //                      ImageUri = jjjj.ImageUri,
        //                      Code = jjjj.Code
        //                  },
        //                  OrderDetail = new
        //                  {
        //                      Id = j.Id,
        //                      Guid = j.Guid,
        //                      ProductId = j.ProductId,
        //                      Description = jj.Description,
        //                      OrderId = j.OrderId,
        //                      OrderNo = jord.OrderNo,
        //                      PaidStatus = j.PaidStatus,
        //                      Status = j.Status,
        //                      Couver = j.Couver,
        //                      //VatCode = ss.PricelistDetail.Vat.Code,
        //                      // VatDesc = ss.PricelistDetail.Vat.Percentage,
        //                      Qty = j.Qty,
        //                      SalesTypeId = j.SalesTypeId,
        //                      //SalesType = j.SalesType.Abbreviation,
        //                      Price = j.Price,
        //                      Total = (double?)j.Price * j.Qty,
        //                      Discount = j.Discount,
        //                      TotalAfterDiscount = j.TotalAfterDiscount,
        //                      //TotalAfterDiscount = ((double?)j.Price * j.Qty) - (j.Discount != null ? (double?)j.Discount : 0),
        //                      TransactionId = j.TransactionId,
        //                      //OrderDetailIgredients = jodi,
        //                      OrderDetailIgredients = new
        //                      {
        //                          Id = jodi != null ? jodi.Id : 0,
        //                          OrderDetailId = jodi != null ? jodi.OrderDetailId : null,
        //                          Qty = jodi != null ? jodi.Qty : null,
        //                          IngredientId = jodi != null ? jodi.IngredientId : null,
        //                          Description = jing.Description,
        //                          Price = jodi != null ? jodi.Price : null,
        //                          Units = jodi != null ? jodi.UnitId : null,
        //                          Discount = jodi != null ? jodi.Discount : null,
        //                          TotalAfterDiscount = jodi != null ? jodi.TotalAfterDiscount : null,
        //                          //     VatCode = sss.PricelistDetail.Vat.Code,
        //                          //     VatDesc = sss.PricelistDetail.Vat.Percentage
        //                      },
        //                      OrderDetailInvoices = new
        //                      {
        //                          Id = q.Id,
        //                          Counter = q.Counter,
        //                          PosInfoDetailId = q.PosInfoDetailId,
        //                          OrderDetailId = q.OrderDetailId,
        //                          StaffId = q.StaffId,
        //                          IsInvoice = q.PosInfoDetail.IsInvoice,
        //                          CreateTransaction = q.PosInfoDetail.CreateTransaction,
        //                          FiscalType = q.PosInfoDetail.FiscalType,
        //                          IsAlsoPaid = j.TransactionId != null,//j.OrderDetailInvoices.Where(ww => ww.PosInfoDetail.CreateTransaction == true).Count() > 0,
        //                          IsAlsoCanceled = j.OrderDetailInvoices.Where(ww => ww.PosInfoDetail.IsCancel == true).Count() > 0
        //                      }
        //                  },
        //                  Product = jj,
        //                  OrderNo = jord.OrderNo,
        //                  OrderId = jord.Id,
        //                  PosInfo = jpos,
        //                  Department = jpos.Department != null ? jpos.Department.Description : null,
        //                  PdaModule = jpda2
        //              }).ToList().GroupBy(g => new { g.PosInfoDetailId, g.Counter, g.PosInfoDetail.GroupId }).ToList();
        //    var query = q1.Select(sg => new
        //    {
        //        // CustId = sg.FirstOrDefault().CustId,
        //        Customer = sg.FirstOrDefault().Cust //!= null ? new     //sg.FirstOrDefault().Cust,
        //            //{
        //            //    RoomNo = sg.FirstOrDefault().Cust != null ? sg.FirstOrDefault().Cust.Room : "",
        //            //    Name = sg.FirstOrDefault().Cust != null ? sg.FirstOrDefault().Cust.FirstName + " " + sg.FirstOrDefault().Cust.LastName : "",
        //            //    Id = sg.FirstOrDefault().Cust.Id != null ? sg.FirstOrDefault().Cust.Id.ToString() : "",
        //            //    ProfileNo = sg.FirstOrDefault().Cust.ProfileNo != null ? sg.FirstOrDefault().Cust.ProfileNo.ToString() : "",
        //            //    ReservationId = sg.FirstOrDefault().Cust.ReservationId != null ? sg.FirstOrDefault().Cust.ReservationId.ToString() : ""
        //            //} : null
        //        ,
        //        Table = sg.FirstOrDefault().TableCode,
        //        AccountObj = sg.FirstOrDefault().AccountObj,

        //        PosInfoDetailId = sg.Key.PosInfoDetailId,
        //        PosInfoDescription = sg.FirstOrDefault().PosInfo.Description,
        //        PosInfoId = sg.FirstOrDefault().PosInfo.Id,
        //        Department = sg.FirstOrDefault().Department,
        //        Counter = sg.Key.Counter,
        //        Account = sg.FirstOrDefault().OrderDetail.TransactionId != null ? sg.FirstOrDefault().Account : "",
        //        AccountId = sg.FirstOrDefault().OrderDetail.TransactionId != null ? sg.FirstOrDefault().AccountId : -1,
        //        Day = sg.FirstOrDefault().CreationTS,
        //        PosInfoDetail = new
        //        {
        //            Id = sg.FirstOrDefault().PosInfoDetail.Id,
        //            Counter = sg.FirstOrDefault().PosInfoDetail.Counter,
        //            IsInvoice = sg.FirstOrDefault().PosInfoDetail.IsInvoice,
        //            CreateTransaction = sg.FirstOrDefault().PosInfoDetail.CreateTransaction,
        //            IsCancel = sg.FirstOrDefault().PosInfoDetail.IsCancel,
        //            FiscalType = sg.FirstOrDefault().PosInfoDetail.FiscalType,
        //            Description = sg.FirstOrDefault().PosInfoDetail.Description,
        //            Abbreviation = sg.FirstOrDefault().PosInfoDetail.Abbreviation,
        //            GroupId = sg.FirstOrDefault().PosInfoDetail.GroupId
        //        },
        //        StaffId = sg.FirstOrDefault().StaffId,
        //        Staff = new
        //        {
        //            FirstName = sg.FirstOrDefault().Staff.FirstName,
        //            LastName = sg.FirstOrDefault().Staff.LastName
        //        },
        //        OrdersNo = sg.Select(ss => ss.OrderNo).Distinct(),
        //        OrdersId = sg.Select(ss => ss.OrderId).Distinct(),
        //        OrderDetail = sg.Select(ss => ss.OrderDetail).GroupBy(ggg => ggg.Id).Select(od => new
        //        {
        //            Id = od.FirstOrDefault().Id,
        //            Guid = od.FirstOrDefault().Guid,
        //            ProductId = od.FirstOrDefault().ProductId,
        //            Description = od.FirstOrDefault().Description,
        //            OrderId = od.FirstOrDefault().OrderId,
        //            OrderNo = od.FirstOrDefault().OrderNo,
        //            Status = od.FirstOrDefault().Status,
        //            PaidStatus = od.FirstOrDefault().PaidStatus,
        //            TransactionId = od.FirstOrDefault().TransactionId,
        //            //VatCode = ss.PricelistDetail.Vat.Code,
        //            // VatDesc = ss.PricelistDetail.Vat.Percentage,
        //            Qty = od.FirstOrDefault().Qty,
        //            SalesTypeId = od.FirstOrDefault().SalesTypeId,
        //            //Status = od.
        //            //SalesType = j.SalesType.Abbreviation,
        //            Price = od.FirstOrDefault().Price,
        //            Discount = od.FirstOrDefault().Discount,
        //            TotalAfterDiscount = od.FirstOrDefault().TotalAfterDiscount,
        //            OrderDetailIgredients = sg.Select(sel => sel.OrderDetail.OrderDetailIgredients).Where(odi => odi.Id != 0 && odi.OrderDetailId == od.FirstOrDefault().Id),
        //            OrderDetailInvoices = od.FirstOrDefault().OrderDetailInvoices,
        //            Total = (double)((double)od.FirstOrDefault().Total + (double)sg.Select(sel => sel.OrderDetail.OrderDetailIgredients).Where(odi => odi.Id != 0 && odi.OrderDetailId == od.FirstOrDefault().Id).Sum(sm => sm.Price))
        //        }
        //        ),
        //        Couver = sg.Max(m => m.OrderDetail.Couver),
        //        Discount = sg.Select(sss => sss.OrderDetail).GroupBy(ggg => ggg.Id).Select(n => n.FirstOrDefault()).Sum(od => (double?)((double?)od.Discount + (double?)sg.Select(sel => sel.OrderDetail.OrderDetailIgredients).Where(odi => odi.Id != 0 && odi.OrderDetailId == od.Id).Sum(sm => sm.Discount))),
        //        Total = sg.Select(sss => sss.OrderDetail).GroupBy(ggg => ggg.Id).Select(n => n.FirstOrDefault()).Sum(od => (double)((double)od.Total + (double)sg.Select(sel => sel.OrderDetail.OrderDetailIgredients).Where(odi => odi.Id != 0 && odi.OrderDetailId == od.Id).Sum(sm => sm.Price))),
        //        TotalAfterDiscount = sg.Select(sss => sss.OrderDetail).GroupBy(ggg => ggg.Id).Select(n => n.FirstOrDefault()).Sum(od => (double)((double)od.TotalAfterDiscount + (double)sg.Select(sel => sel.OrderDetail.OrderDetailIgredients).Where(odi => odi.Id != 0 && odi.OrderDetailId == od.Id).Sum(sm => sm.TotalAfterDiscount))),
        //        IsPrinted = sg.FirstOrDefault().IsPrinted,
        //        PdaModule = sg.FirstOrDefault().PdaModule
        //    });

        //    if (nopriced == true)
        //    {
        //        query = query.Where(w => w.OrderDetail.Any(a => a.PaidStatus == 0 && a.Status != 5));
        //    }
        //    if (nopaid == true)
        //    {
        //        query = query.Where(w => w.OrderDetail.Any(a => a.PaidStatus == 1 && a.Status != 5));
        //    }

        //    dynamic Total = new ExpandoObject();
        //    Total.Gross = query.Where(w => w.PosInfoDetail.CreateTransaction == true).Sum(s => s.Total);
        //    Total.CanceledAmount = query.Where(w => w.PosInfoDetail.IsCancel == true).Sum(s => s.Total);
        //    Total.CanceledCount = query.Where(w => w.PosInfoDetail.IsCancel == true).Count();
        //    Total.Count = query.Where(w => w.PosInfoDetail.CreateTransaction == true).Count();

        //    int count = query.Count();
        //    int totalpages = 1;
        //    if (count > 0)
        //    {
        //        double pages = (double)count / rows;
        //        totalpages = Convert.ToInt16(Math.Ceiling(pages));

        //        if (page > totalpages) page = totalpages;
        //        int start = rows * page - rows;
        //        query = query.OrderByDescending(o => o.Day).Skip(start).Take(rows);
        //    }



        //    return new { query, totalpages, page, Total };
        //}
        #endregion

        #region NEW RECEIPT VIEW PAGER With INVOICES
        public Object GetOrders(string pidids, string date, long posid, int page, int rows, int? searchNo, bool searchReceipt, bool nopriced, bool nopaid, bool notprinted, string tableno, bool old)
        {
            db.Configuration.LazyLoadingEnabled = true;
            List<long> posinfodetids = JsonConvert.DeserializeObject<List<long>>(pidids);//Ayta pleon einai InvoiceTypes

            //var invoices = db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == posid && posinfodetids.Contains(w.PosInfoDetailId.Value) && (w.IsDeleted ?? false) == false);
            //Anti na filtrarw me ta PosInfoDetail tha filtrarw me Invoice Types
            var invoices = db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == posid && posinfodetids.Contains(w.InvoiceTypes.Id) && (w.IsDeleted ?? false) == false);
            if (tableno != null)
            {
                invoices = invoices.Where(w => w.Table.Code == tableno);
            }
            if (searchNo != null)
            {
                if (searchReceipt == true)
                {
                    invoices = invoices.Where(w => w.Counter == searchNo);
                }
                else
                {
                    invoices = invoices.Where(w => w.OrderDetailInvoices.FirstOrDefault().OrderDetail.Order.OrderNo == searchNo);
                }
            }
            if (notprinted == true)
            {
                invoices = invoices.Where(w => (w.IsPrinted ?? false) == false);
            }

            dynamic Total = new ExpandoObject();
            Total.Gross = invoices.Where(w => w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Receipt || w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Timologio).Sum(s => s.Total);//PosInfoDetail.CreateTransaction == true  
            Total.CanceledAmount = invoices.Where(w => w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Void).Sum(s => s.Total);
            Total.CanceledCount = invoices.Where(w => w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Void).Count();
            Total.Count = invoices.Where(w => w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Receipt || w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Timologio).Count();
            Total.Discount = invoices.Where(w => w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Receipt || w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Timologio).Sum(s => s.Discount);




            var query = (from s in invoices
                         join guest in db.Guest on s.GuestId equals guest.Id into jguest
                         from jguestleft in jguest.DefaultIfEmpty()
                         join table in db.Table on s.TableId equals table.Id into jtable
                         from jtableleft in jtable.DefaultIfEmpty()
                         join jDep in db.Department on s.PosInfo.DepartmentId equals jDep.Id
                         join jPid in db.PosInfoDetail on s.PosInfoDetailId equals jPid.Id
                         select new
                         {
                             Id = s.Id,
                             Customer = jguestleft,
                             Table = jtableleft != null ? jtableleft.Code : null,
                             AccountObj = s.Transactions.Count > 0 ? s.Transactions.FirstOrDefault().Accounts : null,
                             Accounts = s.Transactions.Where(w => (w.IsDeleted ?? false) == false).Select(ss => new
                             {
                                 Id = ss.AccountId,
                                 Description = ss.Accounts.Description,
                                 Amount = ss.Amount,
                                 Guest = ss.Invoice_Guests_Trans.FirstOrDefault() != null ? ss.Invoice_Guests_Trans.FirstOrDefault().Guest : null
                             }),//.GroupBy(a => a.Id).Select(g => g.FirstOrDefault()),
                             Account = s.Transactions.Where(w => (w.IsDeleted ?? false) == false).Count() > 0 ? s.Transactions.Where(w => (w.IsDeleted ?? false) == false).FirstOrDefault().Accounts.Description : "",
                             AccountId = s.Transactions.Where(w => (w.IsDeleted ?? false) == false).Count() > 0 ? s.Transactions.Where(w => (w.IsDeleted ?? false) == false).FirstOrDefault().AccountId : null,
                             PosInfoDetailId = s.PosInfoDetailId,
                             PosInfoDescription = s.Description,
                             PosInfoId = s.PosInfoId,
                             Department = jDep.Description,
                             Counter = s.Counter,
                             Day = s.Day,
                             InvoiceTypeAbr = s.InvoiceTypes != null ? s.InvoiceTypes.Abbreviation : "",
                             PosInfoDetail = new
                             {
                                 Id = jPid.Id,
                                 Counter = jPid.Counter,
                                 IsInvoice = jPid.IsInvoice,
                                 CreateTransaction = jPid.CreateTransaction,
                                 IsCancel = jPid.IsCancel,
                                 FiscalType = jPid.FiscalType,
                                 Description = jPid.Description,
                                 Abbreviation = jPid.Abbreviation,
                                 GroupId = jPid.GroupId,
                                 InvoicesTypeId = jPid.InvoicesTypeId
                             },
                             StaffId = s.StaffId,
                             Staff = new
                             {
                                 FirstName = s.Staff.FirstName,
                                 LastName = s.Staff.LastName
                             },
                             //Orders = s.OrderDetailInvoices.Select(ss => ss.OrderDetail),
                             OrderDetail = from p in s.OrderDetailInvoices
                                           join j in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false) on p.OrderDetailId equals j.Id
                                           join jj in db.Product on j.ProductId equals jj.Id
                                           join jord in db.Order.Where(w => (w.IsDeleted ?? false) == false) on j.OrderId equals jord.Id
                                           select new
                                           {
                                               Id = j.Id,
                                               Guid = j.Guid,
                                               ProductId = j.ProductId,
                                               Description = jj.Description,
                                               OrderId = j.OrderId,
                                               OrderNo = jord.OrderNo,
                                               PaidStatus = j.PaidStatus,
                                               Status = j.Status,
                                               Couver = j.Couver,
                                               Qty = j.Qty,
                                               SalesTypeId = j.SalesTypeId,
                                               //SalesType = j.SalesType.Abbreviation,
                                               Price = j.Price,
                                               Total = j.TotalAfterDiscount + (j.OrderDetailIgredients.Count > 0 ? j.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount) : 0),
                                               Discount = j.Discount,
                                               TotalAfterDiscount = j.TotalAfterDiscount,
                                               TransactionId = j.TransactionId,
                                               OrderDetailIgredients = (from odi in j.OrderDetailIgredients
                                                                        join os in db.OrderDetailIgredients.Where(w => (w.IsDeleted ?? false) == false) on odi.OrderDetailId equals os.OrderDetailId into ff
                                                                        from jodi in ff.DefaultIfEmpty()
                                                                        join ing in db.Ingredients on jodi.IngredientId equals ing.Id into fff
                                                                        from jing in fff.DefaultIfEmpty()
                                                                        select new
                                                                        {
                                                                            Id = jodi != null ? jodi.Id : 0,
                                                                            OrderDetailId = jodi != null ? jodi.OrderDetailId : null,
                                                                            Qty = jodi != null ? jodi.Qty : null,
                                                                            IngredientId = jodi != null ? jodi.IngredientId : null,
                                                                            Description = jing.Description,
                                                                            Price = jodi != null ? jodi.Price : null,
                                                                            Units = jodi != null ? jodi.UnitId : null,
                                                                            Discount = jodi != null ? jodi.Discount : null,
                                                                            TotalAfterDiscount = jodi != null ? jodi.TotalAfterDiscount : null,
                                                                        }).Distinct(),
                                               OrderDetailInvoices = j.OrderDetailInvoices
                                           },
                             OrdersNo = (from p in s.OrderDetailInvoices
                                         join j in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false) on p.OrderDetailId equals j.Id
                                         join jord in db.Order.Where(w => (w.IsDeleted ?? false) == false) on j.OrderId equals jord.Id
                                         select jord.OrderNo).Distinct(),
                             Couver = s.Cover,
                             Discount = s.Discount,
                             Total = (s.Total != null ? s.Total : 0) + (s.Discount != null ? s.Discount : 0),
                             TotalAfterDiscount = s.Total != null ? s.Total : 0,
                             IsPrinted = s.IsPrinted,
                             PdaModule = s.PdaModule,
                             IsVoided = s.IsVoided,
                             IsVoidOfNotPaid = s.OrderDetailInvoices.All(ss => ss.OrderDetail.Status == 5 && ss.OrderDetail.PaidStatus < 1)
                         }).ToList();

            if (nopriced == true)
            {
                query = query.Where(w => w.OrderDetail.Any(a => a.PaidStatus == 0 && a.Status != 5)).ToList();
            }
            if (nopaid == true)
            {
                query = query.Where(w => w.OrderDetail.Any(a => a.PaidStatus == 1 && a.Status != 5)).ToList();
            }
            query = query.OrderByDescending(o => o.Day).ToList();

            int count = invoices.Count();
            int totalpages = 1;
            if (count > 0)
            {
                double pages = (double)count / rows;
                totalpages = Convert.ToInt16(Math.Ceiling(pages));

                if (page > totalpages) page = totalpages;
                int start = rows * page - rows;
                query = query.Skip(start).Take(rows).ToList();
            }

            db.Configuration.LazyLoadingEnabled = false;

            return new { query, totalpages, page, Total };
        }
        #endregion

        #region FASTER RECEIPT VIEW PAGER With INVOICES

        public IEnumerable<dynamic> GetAllOrders(long piid)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            var posdata = db.PosInfo.Where(f => f.Id == piid).FirstOrDefault();

            //Configuration.LazyLoadingEnabled = true;

            //.Select(s => s.Invoices).
            var allOdInvs = db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == piid && (w.IsDeleted ?? false) == false);
            //join qq in db.Staff on q.StaffId equals qq.Id
            //select new
            //{
            //    Id = q.Id,
            //    InvoiceTypeId = q.InvoiceTypeId,
            //    PosInfoId = q.PosInfoId,
            //    PosInfoDetailId = q.PosInfoDetailId,
            //    Description = q.Description,
            //    IsVoided = q.IsVoided ?? false,
            //    IsPrinted = q.IsPrinted ?? false,
            //    Total = q.Total,
            //    Discount = q.Discount,
            //    TableId = q.TableId,
            //    StaffId = q.StaffId,
            //    Counter = q.Counter,
            //    Day = q.Day,
            //    Cover = q.Cover,
            //    FirstName = qq.FirstName,
            //    LastName = qq.LastName,
            //    ImageUri = qq.ImageUri,
            //    Code = qq.Code

            //}).Distinct().ToList();

            var baseInfo = from q in db.OrderDetailInvoices
                           join qq in allOdInvs on q.InvoicesId equals qq.Id
                           select q;

            //  .Where(w => w.Invoices.EndOfDayId == null && w.Invoices.PosInfoId == piid && (w.Invoices.IsDeleted ?? false) == false).Distinct();
            //var allOdVats = baseInfo.Select(s => s.OrderDetail).SelectMany(s => s.OrderDetailVatAnal).Distinct().Select(s => new
            //{
            //    VatRate = s.VatRate,
            //    VatAmount = s.VatAmount,
            //    OrderDetailId = s.OrderDetailId
            //}).ToList();
            var allOdingVats = baseInfo.Select(s => s.OrderDetail).SelectMany(sm => sm.OrderDetailIgredients).SelectMany(s => s.OrderDetailIgredientVatAnal).Distinct().Select(s => new
            {
                VatRate = s.VatRate,
                VatAmount = s.VatAmount,
                OrderDetailIgredientsId = s.OrderDetailIgredientsId
            }).ToList();
            var allOds = (from q in baseInfo.Select(s => s.OrderDetail).Distinct()
                          join qq in baseInfo.Select(s => s.OrderDetail).SelectMany(s => s.OrderDetailVatAnal).Distinct() on q.Id equals qq.OrderDetailId
                          select new
                          {
                              Id = q.Id,
                              Guid = q.Guid,
                              ProductId = q.ProductId,
                              OrderId = q.OrderId,
                              //  OrderNo = s.OrderNo,
                              PaidStatus = q.PaidStatus,
                              Status = q.Status,
                              Couver = q.Couver,
                              Qty = q.Qty,
                              SalesTypeId = q.SalesTypeId,
                              Price = q.Price,
                              Discount = q.Discount,
                              TotalAfterDiscount = q.TotalAfterDiscount,
                              VatRate = qq.VatRate,
                              VatAmount = qq.VatAmount,
                              OrderDetailId = qq.OrderDetailId
                              //TransactionId = jOrdDet.TransactionId,
                              //  //	                    //hereme
                              //     VatDesc = ss.FirstOrDefault().VatDesc,

                          }).Distinct().ToList();


            var allTransactions = (from q in baseInfo.Select(s => s.Invoices).SelectMany(sm => sm.Transactions).Distinct().Where(w => w.AccountId != null)
                                   join qq in db.Accounts on q.AccountId equals qq.Id
                                   select new
                                   {
                                       Id = q.Id,
                                       TransactionType = q.TransactionType,
                                       AccountId = q.AccountId,
                                       InvoicesId = q.InvoicesId,
                                       Amount = q.Amount,
                                       AccountDescription = qq.Description
                                   }).Distinct().ToList();
            var allAccounts = baseInfo.Select(s => s.Invoices).SelectMany(sm => sm.Transactions).Distinct().Select(s => s.Accounts).Select(s => new
            {
                Id = s.Id,
                Description = s.Description,

            }).Distinct().ToList();
            // var allIgt = baseInfo.Select(s => s.Invoices).SelectMany(sm => sm.Transactions).Select(ss => ss.Invoice_Guests_Trans).Distinct().ToList();
            // var allrooms = baseInfo.Select(s => s.Invoices).SelectMany(sm => sm.Transactions).SelectMany(ss => ss.Invoice_Guests_Trans).Select(s => s.Guest).ToList();
            var allTable = baseInfo.Select(s => s.Invoices).Select(sm => sm.Table).Where(w => w.Code != null).Select(s => new
            {
                Id = s.Id,
                Code = s.Code
            }).Distinct().ToList();
            var deps = db.Department.Where(w => w.Id == posdata.DepartmentId).Distinct().FirstOrDefault();
            var allinvTypes = db.InvoiceTypes.Select(s => new
            {
                Id = s.Id,
                Abbreviation = s.Abbreviation,
                Type = s.Type,
            }).ToList();
            var allposInfoDetails = baseInfo.Select(s => s.PosInfoDetail).AsNoTracking().Distinct().ToList();
            var allOrders = baseInfo.Select(s => s.OrderDetail).Select(ss => ss.Order).Distinct().Select(s => new
            {
                Id = s.Id,
                OrderNo = s.OrderNo
            }).Distinct().ToList();
            var allSalesTypes = db.SalesType.AsNoTracking().ToList();
            var allprods = baseInfo.Select(s => s.OrderDetail).Select(s => s.Product).Distinct().Select(s => new
            {
                Id = s.Id,
                Code = s.Code,
                Description = s.SalesDescription
            }).ToList();
            var allOdis = baseInfo.Distinct().ToList();
            // db.Configuration.LazyLoadingEnabled = false;
            // db.Configuration.AutoDetectChangesEnabled = true;
            var receipts = from q in allOdInvs
                           let l1 = allTransactions.Where(w => w.InvoicesId == q.Id)
                           from trans in l1.DefaultIfEmpty()
                           let l2 = allTable.Where(w => w.Id == q.TableId)
                           from t in l2.DefaultIfEmpty()

                           let l4 = allAccounts.Where(w => w.Id == (trans != null ? trans.AccountId : -1))
                           from acc in l4.DefaultIfEmpty()
                           let l5 = allinvTypes.Where(w => w.Id == q.InvoiceTypeId)
                           from it in l5
                           let l6 = allposInfoDetails.Where(w => w.Id == q.PosInfoDetailId)
                           from pid in l5.DefaultIfEmpty()
                           let l7 = baseInfo.Where(w => w.InvoicesId == q.Id)
                           from odi in l7
                           let l8 = allOds.Where(w => w.Id == odi.OrderDetailId)
                           from od in l8
                           let l9 = allOrders.Where(w => w.Id == od.OrderId)
                           from o in l9
                               //    let l10 = allOdVats.Where(w => w.OrderDetailId == od.Id)
                               //    from odvat in l10
                               // let l11 = allSalesTypes.Where(w=>w.Id == od.SalesTypeId) from st in l11
                           let l12 = allprods.Where(w => w.Id == od.ProductId)
                           from p in l12.DefaultIfEmpty()
                           select new
                           {
                               Id = q.Id,
                               TableId = q.TableId,
                               TableCode = t != null ? t.Code : "",
                               //AccountObj
                               //Account
                               AccountId = trans != null ? trans.AccountId : 0,
                               TransactionId = trans != null ? trans.Id : 0,
                               AccountDescription = acc != null ? acc.Description : "",
                               TransAmount = trans != null ? trans.Amount : 0,
                               //Guest = ss.Key.TransactionId != null ? ss.FirstOrDefault().Invoice_Trans_Guest.Id != 0 ? ss.FirstOrDefault().Invoice_Trans_Guest.Guest : null : null
                               PosInfoDetailId = q.PosInfoDetailId,
                               PosInfoDescription = posdata.Description,
                               PosInfoId = q.PosInfoId,
                               Department = deps != null ? deps.Description : "",
                               Counter = q.Counter,
                               Day = q.Day,
                               InvoiceTypeAbr = it.Abbreviation,
                               PosInfoDetail = pid,
                               StaffId = q.StaffId,
                               //Staff = s,
                               OrderDetailId = od.Id,
                               //Description = od.Description,
                               OrderDetailGuid = od.Guid,
                               ProductId = od.ProductId,
                               OrderId = od.OrderId,
                               //OrderNo
                               PaidStatus = od.PaidStatus,
                               Status = od.Status,
                               Couver = od.Couver,
                               Qty = od.Qty,
                               Price = od.Price,
                               TotalAfterDiscount = od.TotalAfterDiscount,
                               OrderDetailDiscount = od.Discount,
                               OrderDetailDescription = p.Description,
                               //  OrderDetailInvoice = odi,
                               Cover = q.Cover,
                               Discount = q.Discount,
                               InvoiceTotal = q.Total,
                               IsPrinted = q.IsPrinted,
                               IsVoided = q.IsVoided,
                               OrderNo = o.OrderNo,
                               VatDesc = od.VatRate,
                               SalesTypeId = od.SalesTypeId,
                               IsVoidOfNotPaid = false
                           };
            //receipts.Dump();
            //var rc = receipts.Count();

            var group1 = receipts.GroupBy(g => g.Id).Select(s => new
            {
                Id = s.FirstOrDefault().Id,
                //Customer = s.FirstOrDefault().Invoice_Trans_Guest != null ? s.FirstOrDefault().Invoice_Trans_Guest.Guest : null,
                Table = new
                {
                    Id = s.FirstOrDefault().TableId,
                    Code = s.FirstOrDefault().TableCode,
                },
                //   AccountObj = s.FirstOrDefault().AccountObj.Id != 0 ? s.FirstOrDefault().AccountObj : null,
                Accounts = s.GroupBy(g => new { g.TransactionId, g.AccountId }).ToList().Select(ss => new
                {
                    Id = ss.FirstOrDefault().AccountId,
                    TransactionId = ss.FirstOrDefault().TransactionId,
                    Description = ss.FirstOrDefault().AccountDescription,
                    Amount = ss.FirstOrDefault().TransAmount,
                    //	                    Guest = ss.Key.TransactionId != null ? ss.FirstOrDefault().Invoice_Trans_Guest.Id != 0 ? ss.FirstOrDefault().Invoice_Trans_Guest.Guest : null : null
                }).Where(ww => ww.Id != 0),
                Account = s.FirstOrDefault().AccountDescription,
                AccountId = s.FirstOrDefault().AccountId,
                PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                PosInfoDescription = s.FirstOrDefault().PosInfoDescription,
                PosInfoId = s.FirstOrDefault().PosInfoId,
                Department = s.FirstOrDefault().Department,
                Counter = s.FirstOrDefault().Counter,
                Day = s.FirstOrDefault().Day,
                InvoiceTypeAbr = s.FirstOrDefault().InvoiceTypeAbr,
                PosInfoDetail = s.FirstOrDefault().PosInfoDetail,
                StaffId = s.FirstOrDefault().StaffId,
                // Staff = s.FirstOrDefault().Staff,
                //OrderDetail = s.Select(ss=>ss.OrderDetail).Distinct(),
                OrderDetail = s.GroupBy(gg => gg.OrderDetailId).Select(ss => new
                {
                    Id = ss.Key,
                    Description = ss.FirstOrDefault().OrderDetailDescription,
                    Guid = ss.FirstOrDefault().OrderDetailGuid,
                    ProductId = ss.FirstOrDefault().ProductId,
                    OrderId = ss.FirstOrDefault().OrderId,
                    OrderNo = ss.FirstOrDefault().OrderNo,
                    PaidStatus = ss.FirstOrDefault().PaidStatus,
                    Status = ss.FirstOrDefault().Status,
                    Couver = ss.FirstOrDefault().Couver,
                    Qty = ss.FirstOrDefault().Qty,
                    SalesTypeId = ss.FirstOrDefault().SalesTypeId,
                    Price = ss.FirstOrDefault().Price,
                    Total = ss.FirstOrDefault().Price * (Decimal?)ss.FirstOrDefault().Qty,
                    Discount = ss.FirstOrDefault().OrderDetailDiscount,
                    TotalAfterDiscount = ss.FirstOrDefault().TotalAfterDiscount,
                    //TransactionId = jOrdDet.TransactionId,
                    //	                    //hereme
                    VatDesc = ss.FirstOrDefault().VatDesc,
                    //     OrderDetailInvoices = ss.Select(sss => sss.OrderDetailInvoice),
                    //	                    OrderDetailIgredients = ss.Where(ww => ww.OrderDetailIgredients.Id != 0).Select(sss => sss.OrderDetailIgredients).Distinct()
                }),
                OrdersNo = s.Select(ss => ss.OrderNo).Distinct(),
                Couver = s.FirstOrDefault().Couver,
                Discount = s.FirstOrDefault().Discount,
                Total = (s.FirstOrDefault().InvoiceTotal != null ? s.FirstOrDefault().InvoiceTotal : 0) + (s.FirstOrDefault().Discount != null ? s.FirstOrDefault().Discount : 0),
                TotalAfterDiscount = s.FirstOrDefault().InvoiceTotal != null ? s.FirstOrDefault().InvoiceTotal : 0,
                IsPrinted = s.FirstOrDefault().IsPrinted,
                //    PdaModule = s.FirstOrDefault().PdaModule.Id != 0 ? s.FirstOrDefault().PdaModule : null,
                IsVoided = s.FirstOrDefault().IsVoided,
                IsVoidOfNotPaid = s.FirstOrDefault().IsVoidOfNotPaid
            }
             );
            //         group1.Count();
            return group1;
        }



        public Object GetOrders(string pidids, string date, long posid, int page, int rows, int? searchNo, bool searchReceipt, bool nopriced, bool nopaid, bool notprinted, string tableno)
        {
            // var a = GetAllOrders(posid);

            db.Configuration.LazyLoadingEnabled = true;
            db.Configuration.ProxyCreationEnabled = true;
            List<long> posinfodetids = JsonConvert.DeserializeObject<List<long>>(pidids);//Ayta pleon einai InvoiceTypes

            //var invoices = db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == posid && posinfodetids.Contains(w.PosInfoDetailId.Value) && (w.IsDeleted ?? false) == false);
            //Anti na filtrarw me ta PosInfoDetail tha filtrarw me Invoice Types
            var invoices = db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == posid && posinfodetids.Contains(w.InvoiceTypes.Id) && (w.IsDeleted ?? false) == false);
            if (tableno != null)
            {
                invoices = invoices.Where(w => w.Table.Code == tableno);
            }
            if (searchNo != null)
            {
                if (searchReceipt == true)
                {
                    invoices = invoices.Where(w => w.Counter == searchNo);
                }
                else
                {
                    invoices = invoices.Where(w => w.OrderDetailInvoices.FirstOrDefault().OrderDetail.Order.OrderNo == searchNo);
                }
            }
            if (notprinted == true)
            {
                invoices = invoices.Where(w => (w.IsPrinted ?? false) == false);
            }


            dynamic Total = new ExpandoObject();
            Total.Gross = invoices.Where(w => w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Receipt || w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Timologio).Sum(s => s.Total);//PosInfoDetail.CreateTransaction == true  
            Total.CanceledAmount = invoices.Where(w => w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Void).Sum(s => s.Total);
            Total.CanceledCount = invoices.Where(w => w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Void).Count();
            Total.Count = invoices.Where(w => w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Receipt || w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Timologio).Count();
            Total.Discount = invoices.Where(w => w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Receipt || w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Timologio).Sum(s => s.Discount);



            var invCount = invoices.Count();

            invoices = invoices.OrderByDescending(o => o.Id).Skip((page * rows) - rows).Take(rows);

            var query1 = (from s in invoices
                          join guest in db.Guest on s.GuestId equals guest.Id into jguest
                          from jguestleft in jguest.DefaultIfEmpty()
                          join table in db.Table on s.TableId equals table.Id into jtable
                          from jtableleft in jtable.DefaultIfEmpty()
                          join jDep in db.Department on s.PosInfo.DepartmentId equals jDep.Id
                          join jPid in db.PosInfoDetail on s.PosInfoDetailId equals jPid.Id
                          join jTrans in db.Transactions.Where(w => (w.IsDeleted ?? false) == false) on s.Id equals jTrans.InvoicesId into Trans
                          from jTransLeft in Trans.DefaultIfEmpty()
                          join jOrdDetInv in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false) on s.Id equals jOrdDetInv.InvoicesId into OrdInv
                          from jOrdDetInvLeft in OrdInv.DefaultIfEmpty()
                          join jOrdDet in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false) on jOrdDetInvLeft.OrderDetailId equals jOrdDet.Id
                          join jProd in db.Product on jOrdDet.ProductId equals jProd.Id
                          join jOrd in db.Order.Where(w => (w.IsDeleted ?? false) == false) on jOrdDet.OrderId equals jOrd.Id
                          join jOrdDetIng in db.OrderDetailIgredients.Where(w => (w.IsDeleted ?? false) == false) on jOrdDet.Id equals jOrdDetIng.OrderDetailId into ff
                          from jOrdDetIngLeft in ff.DefaultIfEmpty()
                          join ing in db.Ingredients on jOrdDetIngLeft.IngredientId equals ing.Id into fff
                          from jIngLeft in fff.DefaultIfEmpty()
                          join jtrg in db.Invoice_Guests_Trans on jTransLeft.Id equals jtrg.TransactionId into jtrgg
                          from jTranGuest in jtrgg.DefaultIfEmpty()
                          select new
                          {
                              Id = s.Id,
                              Customer = jguestleft,
                              Table = jtableleft != null ? jtableleft.Code : null,
                              //AccountObj = s.Transactions.Count > 0 ? s.Transactions.FirstOrDefault().Account : null,
                              AccountObj = new
                              {
                                  Id = jTransLeft != null ? jTransLeft.Accounts.Id : 0,
                                  Description = jTransLeft != null ? jTransLeft.Accounts.Description : null,
                                  Type = jTransLeft != null ? jTransLeft.Accounts.Type : null,
                                  SendsTransfer = jTransLeft != null ? jTransLeft.Accounts.SendsTransfer : null,
                              },
                              Transaction = new
                              {
                                  Id = jTransLeft != null ? jTransLeft.Id : 0,
                                  Amount = jTransLeft != null ? jTransLeft.Amount : null,
                              },
                              TransactionId = jTransLeft != null ? (long?)jTransLeft.Id : null,
                              Invoice_Trans_Guest = new
                              {
                                  Id = jTranGuest != null ? jTranGuest.Id : 0,
                                  Guest = jTranGuest != null ? jTranGuest.Guest : null,
                              },

                              //Account = s.Transactions.Where(w => (w.IsDeleted ?? false) == false).Count() > 0 ? s.Transactions.Where(w => (w.IsDeleted ?? false) == false).FirstOrDefault().Account.Description : "",
                              Account = jTransLeft != null ? jTransLeft.Accounts.Description : "",
                              //AccountId = s.Transactions.Where(w => (w.IsDeleted ?? false) == false).Count() > 0 ? s.Transactions.Where(w => (w.IsDeleted ?? false) == false).FirstOrDefault().AccountId : null,
                              AccountId = jTransLeft != null ? jTransLeft.AccountId : null,
                              PosInfoDetailId = s.PosInfoDetailId,
                              PosInfoDescription = s.Description,
                              PosInfoId = s.PosInfoId,
                              Department = jDep.Description,
                              Counter = s.Counter,
                              Day = s.Day,
                              InvoiceTypeAbr = s.InvoiceTypes != null ? s.InvoiceTypes.Abbreviation : "",
                              PosInfoDetail = new
                              {
                                  Id = jPid.Id,
                                  Counter = jPid.Counter,
                                  IsInvoice = jPid.IsInvoice,
                                  CreateTransaction = jPid.CreateTransaction,
                                  IsCancel = jPid.IsCancel,
                                  FiscalType = jPid.FiscalType,
                                  Description = jPid.Description,
                                  Abbreviation = jPid.Abbreviation,
                                  GroupId = jPid.GroupId,
                                  InvoicesTypeId = jPid.InvoicesTypeId
                              },
                              StaffId = s.StaffId,
                              Staff = new
                              {
                                  FirstName = s.Staff.FirstName,
                                  LastName = s.Staff.LastName
                              },

                              OrderDetail = new
                              {
                                  Id = jOrdDet.Id,
                                  Guid = jOrdDet.Guid,
                                  ProductId = jOrdDet.ProductId,
                                  Description = jProd.Description,
                                  OrderId = jOrdDet.OrderId,
                                  OrderNo = jOrd.OrderNo,
                                  PaidStatus = jOrdDet.PaidStatus,
                                  Status = jOrdDet.Status,
                                  Couver = jOrdDet.Couver,
                                  Qty = jOrdDet.Qty,
                                  SalesTypeId = jOrdDet.SalesTypeId,
                                  Price = jOrdDet.Price,
                                  Total = jOrdDet.TotalAfterDiscount + (jOrdDet.OrderDetailIgredients.Count > 0 ? jOrdDet.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount) : 0),
                                  Discount = jOrdDet.Discount,
                                  TotalAfterDiscount = jOrdDet.TotalAfterDiscount,
                                  TransactionId = jOrdDet.TransactionId,
                                  VatDesc = jOrdDet.OrderDetailVatAnal.FirstOrDefault().VatRate,
                                  VatCode = db.Vat.Where(w => w.Id == jOrdDet.OrderDetailVatAnal.FirstOrDefault().VatId).FirstOrDefault().Code,
                                  OrderDetailInvoices = jOrdDet.OrderDetailInvoices.Select(ss => new
                                  {
                                      Id = ss.Id,
                                      OrderDetailId = ss.OrderDetailId,
                                      StaffId = ss.StaffId,
                                      PosInfoDetailId = ss.PosInfoDetailId,
                                      Counter = ss.Counter,
                                      IsPrinted = ss.IsPrinted,
                                      InvoicesId = ss.InvoicesId,
                                  })
                              },
                              OrderDetailIgredients = new
                              {
                                  Id = jOrdDetIngLeft != null ? jOrdDetIngLeft.Id : 0,
                                  OrderDetailId = jOrdDetIngLeft != null ? jOrdDetIngLeft.OrderDetailId : null,
                                  Qty = jOrdDetIngLeft != null ? jOrdDetIngLeft.Qty : null,
                                  IngredientId = jOrdDetIngLeft != null ? jOrdDetIngLeft.IngredientId : null,
                                  Description = jOrdDetIngLeft != null ? jIngLeft.Description : null,
                                  Price = jOrdDetIngLeft != null ? jOrdDetIngLeft.Price : null,
                                  Units = jOrdDetIngLeft != null ? jOrdDetIngLeft.UnitId : null,
                                  Discount = jOrdDetIngLeft != null ? jOrdDetIngLeft.Discount : null,
                                  TotalAfterDiscount = jOrdDetIngLeft != null ? jOrdDetIngLeft.TotalAfterDiscount : null,
                                  VatDesc = jOrdDetIngLeft != null ? jOrdDetIngLeft.OrderDetailIgredientVatAnal.FirstOrDefault().VatRate:0,
                                  VatCode = jOrdDetIngLeft != null ? db.Vat.Where(w => w.Id == jOrdDetIngLeft.OrderDetailIgredientVatAnal.FirstOrDefault().VatId).FirstOrDefault().Code:0
                              },
                              OrdersNo = jOrd.OrderNo,
                              //                             OrdersNo = (from p in s.OrderDetailInvoices
                              //                                         join j in OrderDetail.Where(w=>(w.IsDeleted ?? false) == false) on p.OrderDetailId equals j.Id
                              //                                         join jord in Order.Where(w=>(w.IsDeleted ?? false) == false) on j.OrderId equals jord.Id
                              //                                         select jord.OrderNo).Distinct(),
                              Couver = s.Cover,
                              Discount = s.Discount,
                              Total = s.Total != null ? s.Total : 0,
                              TotalAfterDiscount = (s.Total != null ? s.Total : 0) - (s.Discount != null ? s.Discount : 0),
                              IsPrinted = s.IsPrinted,
                              PdaModule = new
                              {
                                  Id = s.PdaModule != null ? s.PdaModule.Id : 0,
                                  Description = s.PdaModule != null ? s.PdaModule.Description : null,
                                  Code = s.PdaModule != null ? s.PdaModule.Code : null,
                                  PosInfoId = s.PdaModule != null ? s.PdaModule.PosInfoId : null,
                              },
                              IsVoided = s.IsVoided,
                              IsVoidOfNotPaid = s.OrderDetailInvoices.All(ss => ss.OrderDetail.Status == 5 && ss.OrderDetail.PaidStatus < 1)
                          });

            if (nopriced == true)
            {
                query1 = query1.Where(w => w.OrderDetail.PaidStatus == 0 && w.OrderDetail.Status != 5);//.ToList();
                // query = query.Where(w => w.OrderDetail.Any(a => a.PaidStatus == 0 && a.Status != 5)).ToList();
            }
            if (nopaid == true)
            {
                query1 = query1.Where(w => w.OrderDetail.PaidStatus == 1 && w.OrderDetail.Status != 5);//.ToList();
                //query = query.Where(w => w.OrderDetail.Any(a => a.PaidStatus == 1 && a.Status != 5)).ToList();
            }

            //Groupings

            var group1 = query1.ToList().GroupBy(g => g.Id).Select(s => new
            {
                Id = s.FirstOrDefault().Id,
                Customer = s.FirstOrDefault().Invoice_Trans_Guest != null ? s.FirstOrDefault().Invoice_Trans_Guest.Guest : null,
                Table = s.FirstOrDefault().Table,
                AccountObj = s.FirstOrDefault().AccountObj.Id != 0 ? s.FirstOrDefault().AccountObj : null,
                //Accounts = s.GroupBy(g => g.AccountId).Select(ss => new
                //{
                //    Id = ss.Key != null ? ss.Key : 0,
                //    Description = ss.Key != null ? ss.FirstOrDefault().AccountObj.Description : "",
                //    //Amount = ss.Amount,
                //    //Guest = ss.TransactionInvoice_Guests_Trans.FirstOrDefault() != null ? ss.TransactionInvoice_Guests_Trans.FirstOrDefault().Guest : null
                //}).Where(ww => ww.Id != 0),
                Accounts = s.GroupBy(g => new { g.TransactionId, g.AccountId }).ToList().Select(ss => new
                {
                    Id = ss.Key.AccountId != null ? ss.Key.AccountId : 0,
                    TransactionId = ss.Key.TransactionId != null ? ss.Key.TransactionId : 0,
                    Description = ss.Key.TransactionId != null ? ss.FirstOrDefault().Account : "",
                    Amount = ss.Key.TransactionId != null ? ss.FirstOrDefault().Transaction.Id != 0 ? ss.FirstOrDefault().Transaction.Amount : 0 : 0,
                    Guest = ss.Key.TransactionId != null ? ss.FirstOrDefault().Invoice_Trans_Guest.Id != 0 ? ss.FirstOrDefault().Invoice_Trans_Guest.Guest : null : null
                    // Amount = ss.Key.TransactionId != null ? ss.FirstOrDefault().Transaction.Amount : 0,
                    // Guest = ss.Key.TransactionId != null ? ss.FirstOrDefault().Transaction.Invoice_Guests_Trans.FirstOrDefault() != null ? ss.FirstOrDefault().Transaction.Invoice_Guests_Trans.FirstOrDefault().Guest : null : null
                }).Where(ww => ww.Id != 0),
                Account = s.FirstOrDefault().Account,
                AccountId = s.FirstOrDefault().AccountId,
                PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                PosInfoDescription = s.FirstOrDefault().PosInfoDescription,
                PosInfoId = s.FirstOrDefault().PosInfoId,
                Department = s.FirstOrDefault().Department,
                Counter = s.FirstOrDefault().Counter,
                Day = s.FirstOrDefault().Day,
                InvoiceTypeAbr = s.FirstOrDefault().InvoiceTypeAbr,
                PosInfoDetail = s.FirstOrDefault().PosInfoDetail,
                StaffId = s.FirstOrDefault().StaffId,
                Staff = s.FirstOrDefault().Staff,
                //OrderDetail = s.Select(ss=>ss.OrderDetail).Distinct(),
                OrderDetail = s.GroupBy(gg => gg.OrderDetail.Id).Select(ss => new
                {
                    Id = ss.Key,
                    Description = ss.FirstOrDefault().OrderDetail.Description,
                    Guid = ss.FirstOrDefault().OrderDetail.Guid,
                    ProductId = ss.FirstOrDefault().OrderDetail.ProductId,
                    OrderId = ss.FirstOrDefault().OrderDetail.OrderId,
                    OrderNo = ss.FirstOrDefault().OrderDetail.OrderNo,
                    PaidStatus = ss.FirstOrDefault().OrderDetail.PaidStatus,
                    Status = ss.FirstOrDefault().OrderDetail.Status,
                    Couver = ss.FirstOrDefault().OrderDetail.Couver,
                    Qty = ss.FirstOrDefault().OrderDetail.Qty,
                    SalesTypeId = ss.FirstOrDefault().OrderDetail.SalesTypeId,
                    Price = ss.FirstOrDefault().OrderDetail.Price,
                    Total = ss.FirstOrDefault().OrderDetail.Total,
                    Discount = ss.FirstOrDefault().OrderDetail.Discount,
                    TotalAfterDiscount = ss.FirstOrDefault().OrderDetail.TotalAfterDiscount,
                    //TransactionId = jOrdDet.TransactionId,
                    //hereme
                    VatDesc = ss.FirstOrDefault().OrderDetail.VatDesc,
                    OrderDetailInvoices = ss.FirstOrDefault().OrderDetail.OrderDetailInvoices,
                    OrderDetailIgredients = ss.Where(ww => ww.OrderDetailIgredients.Id != 0).Select(sss => sss.OrderDetailIgredients).Distinct()
                }),
                OrdersNo = s.Select(ss => ss.OrdersNo).Distinct(),
                Couver = s.FirstOrDefault().Couver,
                Discount = s.FirstOrDefault().Discount,
                Total = (s.FirstOrDefault().Total != null ? s.FirstOrDefault().Total : 0) + (s.FirstOrDefault().Discount != null ? s.FirstOrDefault().Discount : 0),
                TotalAfterDiscount = s.FirstOrDefault().Total != null ? s.FirstOrDefault().Total : 0,
                IsPrinted = s.FirstOrDefault().IsPrinted,
                PdaModule = s.FirstOrDefault().PdaModule.Id != 0 ? s.FirstOrDefault().PdaModule : null,
                IsVoided = s.FirstOrDefault().IsVoided,
                IsVoidOfNotPaid = s.FirstOrDefault().IsVoidOfNotPaid
            });


            group1 = group1.OrderByDescending(o => o.Day).ToList();

            int count = invCount;//invoices.Count();
            int totalpages = 1;
            if (count > 0)
            {
                double pages = (double)count / rows;
                totalpages = Convert.ToInt16(Math.Ceiling(pages));

                if (page > totalpages) page = totalpages;
                int start = rows * page - rows;
                //group1 = group1.ToList();//.Skip(start).Take(rows).ToList();
            }

            db.Configuration.LazyLoadingEnabled = false;
            var query = group1.Take(10);
            return new { query, totalpages, page, Total };
        }
        #endregion

        public Object GetOrder(long id, bool forResend, bool n)
        {
            var query = db.Order.Include(i => i.EndOfDay).Include(i => i.Transactions).Include("OrderDetail").Include("OrderDetail.Table").Include("OrderDetail.OrderDetailInvoices")
                .Where(w => w.Id == id).AsNoTracking().Select(s => new //Order()
                {
                    Id = s.Id,
                    Day = s.Day,
                    EndOfDayId = s.EndOfDayId,
                    Staff = new
                    {
                        Id = s.Staff.Id,
                        FirstName = s.Staff.FirstName,
                        LastName = s.Staff.LastName,
                        ImageUri = s.Staff.ImageUri,
                        Code = s.Staff.Code
                    },
                    StaffId = s.StaffId,
                    OrderNo = s.OrderNo,
                    ReceiptNo = s.ReceiptNo,
                    OrderStatus = s.OrderStatus.Select(ss => new
                    {
                        Id = ss.Id,
                        Status = ss.Status,
                        TimeChanged = ss.TimeChanged,
                        StaffId = ss.StaffId
                    }),
                    PosId = s.PosId,
                    PosInfo = new
                    {
                        Code = s.PosInfo.Code,
                        Description = s.PosInfo.Description,
                        Id = s.PosInfo.Id
                    },
                    Total = s.Total,
                    TotalBeforeDiscount = s.TotalBeforeDiscount,
                    FODay = s.EndOfDay.FODay,
                    AccountId = s.Transactions.FirstOrDefault() != null ? s.Transactions.FirstOrDefault().AccountId : null,
                    PreparationTime = s.OrderDetail.Max(m => m.PreparationTime),
                    SalesType = s.OrderDetail.Select(ss => ss.SalesTypeId).Distinct().Count() == 1 ? s.OrderDetail.FirstOrDefault().SalesType.Abbreviation : null,
                    OrderDetail = s.OrderDetail.Select(ss => new
                    {
                        Id = ss.Id,
                        ProductId = ss.ProductId,
                        Description = ss.Product.Description,
                        VatCode = ss.PricelistDetail.Vat.Code,
                        VatDesc = ss.PricelistDetail.Vat.Percentage,
                        Qty = ss.Qty,
                        SalesTypeId = ss.SalesTypeId,
                        SalesType = ss.SalesType.Abbreviation,
                        Price = ss.Price,
                        TableId = ss.TableId,
                        TableNo = ss.Table.Code,
                        OrderDetailIgredients = ss.OrderDetailIgredients.Select(sss => new
                        {
                            Id = sss.Id,
                            Qty = sss.Qty,
                            IngredientId = sss.IngredientId,
                            Description = sss.Ingredients.Description,
                            Price = sss.Price,
                            Units = sss.UnitId,
                            VatCode = sss.PricelistDetail.Vat.Code,
                            VatDesc = sss.PricelistDetail.Vat.Percentage
                        })
                    }),
                    PaymentType = s.Transactions.FirstOrDefault() != null ? s.Transactions.FirstOrDefault().Accounts != null ? s.Transactions.FirstOrDefault().Accounts.Description : SqlFunctions.StringConvert((double)s.Transactions.FirstOrDefault().AccountId, 10, 0).Trim() : null,
                }).SingleOrDefault();
            return query;
        }

        #region RECEIPT REPRINT
        public Object GetOrder(bool forResend, long posInfoDetailId, long receiptNo)
        {
            db.Configuration.LazyLoadingEnabled = true;
            //var posInfoDetailId = 98;
            //var receiptNo = 71;
            // var guestData = db.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().CustomerId != null;
            //OrderDetail.Where(w=>w.OrderDetailInvoices.Any(a=>a.Counter == 59 && a.PosInfoDetailId == 58)).Dump();
            //var qry = db.OrderDetail.Where(w => w.OrderDetailInvoices.Any(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId));
            var query = db.OrderDetail.Include("OrderDetailIgredients.OrderDetailIgredientVatAnal").Include("OrderDetailIgredients.Ingredients").Where(w => w.Order.EndOfDay == null && w.OrderDetailInvoices.Any(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId)).ToList().Select(s => new
            {
                Id = s.Id,
                FiscalType = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.FiscalType,
                InvoiceIndex = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.InvoiceId,
                Guest = s.Guest,
                TableNo = s.Table != null ? s.Table.Code : "",
                Waiter = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().Staff.LastName,//+ ' ' + s.OrderDetailInvoices.FirstOrDefault().Staff.FirstName,
                WaiterNo = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().StaffId,
                Pos = "POS-" + s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.PosInfo.Id,
                PosDescr = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.PosInfo.Description,
                DepartmentDesc = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.PosInfo.Department.Description,
                Department = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.PosInfo.DepartmentId,
                //  CustomerName = "Πελάτης Λιανικής",
                //  CustomerAddress ="",
                //  CustomerDeliveryAddress ="",
                //  CustomerPhone = "",
                //  CustomerComments = "",
                //  CustomerAfm = "",
                //  CustomerDoy = "",
                //  CustomerJob = "",
                // RegNo = "",
                //  SumOfLunches = "",
                //  SumofConsumedLunches = "",
                //  GuestTerm = "",
                //  Adults = 0,
                ////  Kids = 0,
                InvoiceType = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.InvoiceId,//InvoiceTYpe right??
                //TotalVat = s.OrderDetailVatAnal.Sum(sm => sm.Gross),
                //TotalVat1 = s.OrderDetailVatAnal.Where(w => w.VatId == 1).Sum(sm => sm.Gross),
                //TotalVat2 = s.OrderDetailVatAnal.Where(w => w.VatId == 2).Sum(sm => sm.Gross),
                //TotalVat3 = s.OrderDetailVatAnal.Where(w => w.VatId == 3).Sum(sm => sm.Gross),
                //TotalVat4 = s.OrderDetailVatAnal.Where(w => w.VatId == 4).Sum(sm => sm.Gross),
                //TotalVat5 = s.OrderDetailVatAnal.Where(w => w.VatId == 5).Sum(sm => sm.Gross),
                TotalDiscount = 0,//s.Discount,
                Bonus = 0,
                PriceList = s.PricelistDetail.PricelistId,
                ReceiptNo = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().Counter,
                Change = 0,
                PaidAmount = s.Transactions != null ? s.Transactions.Amount : 0,
                IsVoid = s.Status == 5 && s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.IsCancel != null ? s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.IsCancel : false,
                DetailsId = s.Id,

                PrintKitchen = false,
                KitchenId = s.Product.KitchenId,
                PaymentType = s.Transactions != null ? s.Transactions.Accounts.Description : "",
                ReceiptTypeDescription = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.Description,
                DepartmentTypeDescription = s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.PosInfo.Department.Description,
                Price = s.Price,
                IsChangeItem = s.Price < 0 || s.Status == 5,
                ItemCode = s.ProductId,
                ItemDescr = s.Price < 0 ? ("ΑΛΛΑΓΗ " + s.Product.Description) : s.Product.Description,
                ItemQty = s.Qty,
                ItemPrice = s.Price,
                ItemVatRate = db.Vat.Find(s.OrderDetailVatAnal.FirstOrDefault().VatId) != null ? db.Vat.Find(s.OrderDetailVatAnal.FirstOrDefault().VatId).Code : s.OrderDetailVatAnal.FirstOrDefault().VatId,
                ItemGross = (double)s.Price * (double)s.Qty,//s.TotalAfterDiscount,//s.Status == 5 ? s.TotalAfterDiscount * -1 : 
                ItemDiscount = s.Discount == null ? 0 : s.Discount.Value,
                SalesTypeDesc = s.SalesType.Abbreviation,
                Extras = s.OrderDetailIgredients != null ? s.OrderDetailIgredients : new List<OrderDetailIgredients>(),
                ItemVatValue = s.OrderDetailVatAnal.FirstOrDefault().VatAmount,
                ItemVatDesc = s.OrderDetailVatAnal.FirstOrDefault().VatRate,
                ItemTotal = (double)s.Price * (double)s.Qty,//s.TotalAfterDiscount,
                OrderId = s.OrderId,
                OrderNo = s.Order.OrderNo,
                Couver = s.Couver,
                Status = s.Status,
                Guid = s.Guid,
                IsNegativePrice = s.Status == 5
                && s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.IsInvoice == false
                && s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.CreateTransaction == false
                && s.OrderDetailInvoices.Where(a => a.Counter == receiptNo && a.PosInfoDetailId == posInfoDetailId).FirstOrDefault().PosInfoDetail.IsCancel == false,
                Transactions = s.Transactions
            }).ToList();
            var queryGrouped = query.GroupBy(g => g.ReceiptNo).Select(s => new
            {
                OrderId = string.Join("-", s.Select(f => f.OrderId).Distinct()),
                OrderNo = string.Join(",", s.Select(f => f.OrderNo).Distinct()),
                FiscalType = s.FirstOrDefault().FiscalType,
                InvoiceIndex = s.FirstOrDefault().InvoiceIndex,
                TableNo = s.FirstOrDefault().TableNo,
                RoomNo = s.FirstOrDefault().Guest == null ? "" : s.FirstOrDefault().Guest.Room,
                Waiter = s.FirstOrDefault().Waiter,
                WaiterNo = s.FirstOrDefault().WaiterNo,
                Pos = s.FirstOrDefault().Pos,
                PosDescr = s.FirstOrDefault().PosDescr,
                DepartmentDesc = s.FirstOrDefault().DepartmentDesc,
                Department = s.FirstOrDefault().Department,
                CustomerName = s.FirstOrDefault().Guest == null ? "ΠΕΛΑΤΗΣ ΛΙΑΝΙΚΗΣ" : s.FirstOrDefault().Guest.LastName,// + " " + s.FirstOrDefault().Guest.FirstName,
                CustomerAddress = s.FirstOrDefault().Guest == null ? "" : s.FirstOrDefault().Guest.Address,
                CustomerDeliveryAddress = "",
                CustomerPhone = "",
                CustomerComments = "",
                CustomerAfm = "",
                CustomerDoy = "",
                CustomerJob = "",
                RegNo = s.FirstOrDefault().Guest == null ? 0 : s.FirstOrDefault().Guest.ReservationId,
                SumOfLunches = 0,
                SumofConsumedLunches = 0,
                GuestTerm = s.FirstOrDefault().Guest == null ? "" : s.FirstOrDefault().Guest.BoardCode,
                Adults = 0,
                Kids = 0,
                InvoiceType = s.FirstOrDefault().InvoiceType,
                Total = (decimal)s.Sum(ss => ss.ItemGross) - (decimal)s.Sum(ss => ss.ItemDiscount),
                ReceiptTypeDescription = s.FirstOrDefault().ReceiptTypeDescription,
                DepartmentTypeDescription = s.FirstOrDefault().DepartmentTypeDescription,
                Couver = s.FirstOrDefault().Couver,
                PrintKitchen = false,
                //TotalVat1 = s.Sum(sm => sm.TotalVat1),
                //TotalVat2 = s.Sum(sm => sm.TotalVat2),
                //TotalVat3 = s.Sum(sm => sm.TotalVat3),
                //TotalVat4 = s.Sum(sm => sm.TotalVat4),
                //TotalVat5 = s.Sum(sm => sm.TotalVat5),
                TotalDiscount = s.Sum(sm => sm.TotalDiscount),
                Bonus = s.FirstOrDefault().Bonus,
                PriceList = s.FirstOrDefault().PriceList,
                ReceiptNo = s.FirstOrDefault().ReceiptNo,
                Change = s.FirstOrDefault().Change,
                PaidAmount = s.FirstOrDefault().Transactions != null ? s.Select(ss => ss.Transactions).Distinct().Sum(sum => sum.Gross) != null ? Math.Round(s.Select(ss => ss.Transactions).Distinct().Sum(sum => sum.Gross).Value, 2) : 0 : 0,
                IsVoid = s.FirstOrDefault().IsVoid,
                DetailsId = string.Join("+", s.Select(f => f.Guid).Distinct()) + "," + posInfoDetailId.ToString(),
                //Guest = s.FirstOrDefault().Guest,
                Details = s.Select(ss => new
                {
                    //Id = ss.Id;
                    //od.AA = d.AA;
                    ItemCustomRemark = "",
                    ItemVatRate = ss.ItemVatRate,
                    ItemVatValue = ss.ItemVatValue,
                    ItemVatDesc = ss.ItemVatDesc,
                    ItemDiscount = Math.Round(ss.ItemDiscount, 2),
                    ItemNet = 0,
                    ItemGross = !ss.IsNegativePrice ? ss.ItemGross : ss.ItemGross * -1,
                    ItemTotal = !ss.IsNegativePrice ? ss.ItemTotal : ss.ItemTotal * -1,
                    ItemPosition = 0,
                    ItemSort = 0,
                    ItemRegion = "",
                    RegionPosition = 0,
                    ItemBarcode = 0,
                    ItemDescr = ss.ItemDescr,
                    IsVoid = ss.IsVoid,
                    IsChangeItem = ss.IsChangeItem,
                    KitchenId = ss.KitchenId,
                    ItemPrice = ss.Price,
                    ItemCode = ss.ItemCode,
                    ItemQty = ss.ItemQty,
                    SalesTypeDesc = ss.SalesTypeDesc,
                    Extras = ss.Extras.GroupBy(g => g.OrderDetailId).Select(sss => new
                    {
                        ItemCode = sss.FirstOrDefault().IngredientId,
                        ItemDescr = sss.FirstOrDefault().Qty > 0 ? sss.FirstOrDefault().Ingredients as Ingredients != null ? sss.FirstOrDefault().Ingredients.Description : "ΧΩΡΙΣ" + sss.FirstOrDefault().Ingredients.Description : "",
                        IsChangeItem = sss.FirstOrDefault().Price < 0,
                        ItemGross = !ss.IsNegativePrice ? sss.FirstOrDefault().TotalAfterDiscount : sss.FirstOrDefault().TotalAfterDiscount * -1,
                        ItemPrice = sss.FirstOrDefault().Price,
                        ItemDiscount = sss.FirstOrDefault().Discount != null ? Math.Round(sss.FirstOrDefault().Discount.Value, 2) : 0,
                        ItemQty = sss.FirstOrDefault().Qty,
                        ItemVatRate = db.Vat.Find(sss.FirstOrDefault().OrderDetailIgredientVatAnal.FirstOrDefault().VatId) != null ? db.Vat.Find(sss.FirstOrDefault().OrderDetailIgredientVatAnal.FirstOrDefault().VatId).Code : sss.FirstOrDefault().OrderDetailIgredientVatAnal.FirstOrDefault().VatId,
                        ItemVatDesc = sss.FirstOrDefault().OrderDetailIgredientVatAnal.FirstOrDefault().VatRate,
                    })
                })
            });

            db.Configuration.LazyLoadingEnabled = false;


            return queryGrouped.SingleOrDefault();
        }
        #endregion

        #region RECEIPT REPRINT WITH INVOICES
        public Object GetOrder(bool forResend, long invoiceId)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            var query = from q in db.OrderDetailInvoices.Where(w => w.InvoicesId == invoiceId)
                        let l = db.Invoices.Where(w => w.Id == q.InvoicesId)
                        from inv in l
                        let l1 = db.OrderDetail.Where(w => w.Id == q.OrderDetailId)
                        from od in l1
                        let l2 = db.OrderDetailVatAnal.Where(w => w.OrderDetailId == q.OrderDetailId)
                        from odva in l2
                        let l3 = db.Vat.Where(w => w.Id == odva.VatId)
                        from vatOd in l3
                        let l4 = db.Product.Where(w => w.Id == od.ProductId)
                        from p in l4
                        let l5 = db.SalesType.Where(w => w.Id == od.SalesTypeId)
                        from st in l5
                        let l6 = db.OrderDetailIgredients.Where(w => w.OrderDetailId == q.OrderDetailId)
                        from oding in l6.DefaultIfEmpty()
                        let l7 = db.Ingredients.Where(w => w.Id == oding.IngredientId)
                        from ing in l7.DefaultIfEmpty()
                        let l8 = db.OrderDetailIgredientVatAnal.Where(w => w.OrderDetailIgredientsId == q.OrderDetailId)
                        from odingva in l2.DefaultIfEmpty()
                        let l9 = db.Vat.Where(w => w.Id == odingva.VatId)
                        from ingvatOd in l3.DefaultIfEmpty()
                        let l10 = db.Transactions.Where(w => w.InvoicesId == q.InvoicesId)
                        from trans in l10.DefaultIfEmpty()
                        let l11 = db.Accounts.Where(w => w.Id == trans.AccountId)
                        from acc in l11.DefaultIfEmpty()
                        let l12 = db.Invoice_Guests_Trans.Where(w => w.TransactionId == trans.Id)
                        from igt in l12.DefaultIfEmpty()
                        let l13 = db.Guest.Where(w => w.Id == igt.GuestId)
                        from g in l13.DefaultIfEmpty()
                        let l14 = db.PosInfo.Where(w => w.Id == inv.PosInfoId)
                        from pi in l14
                        let l15 = db.PosInfoDetail.Where(w => w.Id == inv.PosInfoDetailId)
                        from pid in l15
                        let l16 = db.Department.Where(w => w.Id == pi.DepartmentId)
                        from d in l16.DefaultIfEmpty()
                        let l17 = db.Table.Where(w => w.Id == inv.TableId)
                        from t in l17.DefaultIfEmpty()
                        let l18 = db.Order.Where(w => w.Id == od.OrderId)
                        from o in l18
                        let l19 = db.Staff.Where(w => w.Id == inv.StaffId)
                        from s in l19
                        select new
                        {
                            InvoiceId = q.InvoicesId,
                            PosInfoDetailId = q.PosInfoDetailId,
                            //Details
                            OrderDetailId = od.Id,
                            ItemCustomRemark = "",
                            ItemVatRate = vatOd.Code,
                            ItemVatValue = odva.VatAmount,
                            ItemVatDesc = odva.VatRate,
                            ItemDiscount = od.Discount,//Math.Round(od.Discount != null ? od.Discount.Value : 0, 2, MidpointRounding.AwayFromZero),
                            ItemNet = 0,
                            Guid = od.Guid,
                            //ItemGross = od.TotalAfterDiscount >= 0 ? ((decimal?)od.Qty * od.Price) : ((decimal?)od.Qty * od.Price) * -1,
                            Total = od.TotalAfterDiscount,// >= 0 ? ((decimal?)od.Qty * od.Price) : ((decimal?)od.Qty * od.Price) * -1,
                            ItemPosition = 0,
                            ItemSort = 0,
                            ItemRegion = "",
                            RegionPosition = 0,
                            ItemBarcode = 0,
                            ItemDescr = p.Description,
                            IsChangeItem = od.TotalAfterDiscount < 0 || od.Status == 5,
                            KitchenId = od.KitchenId,
                            //ItemPrice = od.TotalAfterDiscount >= 0 ? od.Price : od.Price * -1,
                            ItemCode = od.ProductId,
                            ItemQty = od.Qty,
                            SalesTypeDesc = st.Description,
                            //Extras
                            IngItemCode = oding != null ? oding.IngredientId : null,
                            //IngItemDescr = oding != null && oding.Qty > 0 ? ing != null ? ing.Description : "ΧΩΡΙΣ" + ing.Description : "",
                            IngDescription = ing != null ? ing.Description : "",
                            IngIsChangeItem = oding != null ? oding.Price < 0 : false,
                            IngItemGross = oding.TotalAfterDiscount,
                            IngItemPrice = oding != null ? oding.Price : null,// >= 0 ? oding.Qty > 1 ? (decimal)(oding.Price / (decimal)oding.Qty) : oding.Price : (oding.Qty > 1 ? (decimal)(oding.Price / (decimal)oding.Qty) : oding.Price) * -1 : null,
                            IngItemDiscount = oding != null ? oding.Discount : 0,
                            IngItemQty = oding != null ? oding.Qty : null,
                            IngItemVatRate = ingvatOd != null ? ingvatOd.Code : null,
                            IngItemVatDesc = odingva != null ? odingva.VatRate : null,
                            //Payments
                            //AccountDescription = acc != null?acc.Description:"",
                            AccountId = acc != null ? acc.Id : -1,
                            AccountType = acc != null ? acc.Type : null,
                            Amount = trans != null ? trans.Amount : null,
                            AccountDescription = acc != null && g != null ? string.Concat("Room:", g.Room) : acc != null ? acc.Description : "",
                            //Invoice
                            OrderId = o.Id,//string.Join("-", o.Select(ss => ss.Id).Distinct()),
                            OrderNo = o.OrderNo,//string.Join(",", o.Select(ss => ss.OrderNo).Distinct()),
                            FiscalType = pi.FiscalType,
                            InvoiceIndex = pid.InvoiceId,
                            TableNo = t != null ? t.Code : null,
                            RoomNo = "",
                            Waiter = s.LastName,
                            WaiterNo = s.Code,
                            Pos = inv.PosInfoId,
                            PosDescr = pi.Description,
                            DepartmentDesc = d.Description,
                            //Department = s.PosInfo.Department,
                            CustomerName = "",//s.Invoice_Guests_Trans.FirstOrDefault() == null ? "ΠΕΛΑΤΗΣ ΛΙΑΝΙΚΗΣ" : s.Invoice_Guests_Trans.FirstOrDefault().Guest.LastName,
                            CustomerAddress = g != null ? g.Address : "",
                            CustomerDeliveryAddress = "",
                            CustomerPhone = "",
                            CustomerComments = "",
                            CustomerAfm = "",
                            CustomerDoy = "",
                            CustomerJob = "",
                            RegNo = g != null ? g.ReservationId : null,
                            SumOfLunches = 0,
                            SumofConsumedLunches = 0,
                            GuestTerm = g != null ? g.BoardCode : null,
                            Adults = 0,
                            Kids = 0,
                            InvoiceType = pid.InvoiceId,
                            InvTotal = inv.Total,
                            ReceiptTypeDescription = pid.Description,
                            DepartmentTypeDescription = d.Description,
                            Couver = inv.Cover,
                            PrintKitchen = false,
                            SalesTypeDescription = st != null ? st.Description : "",
                            //TotalVat1 = s.Sum(sm => sm.TotalVat1),
                            //TotalVat2 = s.Sum(sm => sm.TotalVat2),
                            //TotalVat3 = s.Sum(sm => sm.TotalVat3),
                            //TotalVat4 = s.Sum(sm => sm.TotalVat4),
                            //TotalVat5 = s.Sum(sm => sm.TotalVat5),
                            TotalDiscount = 0,//Only Discount on Items is shown //s.Discount,
                            Bonus = 0,
                            PriceList = 0,
                            ReceiptNo = inv.Counter,
                            Change = 0,
                            PaidAmount = inv.Total,
                            IsVoid = inv.IsVoided ?? false,


                        };
            query.ToList();
            var reprintReceipt = query.ToList().GroupBy(g => g.InvoiceId)
                                         .Select(s => new
                                         {
                                             InvoiceId = s.FirstOrDefault().InvoiceId,
                                             OrderId = string.Join("-", s.Select(ss => ss.OrderId).Distinct()),
                                             OrderNo = string.Join(",", s.Select(ss => ss.OrderNo).Distinct()),
                                             FiscalType = s.FirstOrDefault().FiscalType,
                                             InvoiceIndex = s.FirstOrDefault().InvoiceIndex,
                                             TableNo = s.FirstOrDefault().TableNo,
                                             RoomNo = s.FirstOrDefault().RoomNo,
                                             Waiter = s.FirstOrDefault().Waiter,
                                             WaiterNo = s.FirstOrDefault().WaiterNo,
                                             Pos = "POS-" + s.FirstOrDefault().Pos.ToString(),
                                             PosDescr = s.FirstOrDefault().PosDescr,
                                             DepartmentDesc = s.FirstOrDefault().DepartmentDesc,
                                             //Department = s.PosInfo.Department,
                                             CustomerName = s.FirstOrDefault().CustomerName,
                                             CustomerAddress = s.FirstOrDefault().CustomerAddress,
                                             CustomerDeliveryAddress = s.FirstOrDefault().CustomerDeliveryAddress,
                                             CustomerPhone = s.FirstOrDefault().CustomerPhone,
                                             CustomerComments = s.FirstOrDefault().CustomerComments,
                                             CustomerAfm = s.FirstOrDefault().CustomerAfm,
                                             CustomerDoy = s.FirstOrDefault().CustomerDoy,
                                             CustomerJob = s.FirstOrDefault().CustomerDoy,
                                             RegNo = s.FirstOrDefault().RegNo,
                                             SumOfLunches = s.FirstOrDefault().SumOfLunches,
                                             SumofConsumedLunches = s.FirstOrDefault().SumofConsumedLunches,
                                             GuestTerm = s.FirstOrDefault().GuestTerm,
                                             Adults = s.FirstOrDefault().Adults,
                                             Kids = s.FirstOrDefault().Kids,
                                             InvoiceType = s.FirstOrDefault().InvoiceType,
                                             Total = s.FirstOrDefault().InvTotal,
                                             ReceiptTypeDescription = s.FirstOrDefault().ReceiptTypeDescription,
                                             DepartmentTypeDescription = s.FirstOrDefault().DepartmentTypeDescription,
                                             Couver = s.FirstOrDefault().Couver,
                                             PrintKitchen = s.FirstOrDefault().PrintKitchen,
                                             SalesTypeDescription = s.FirstOrDefault().SalesTypeDescription,
                                             //TotalVat1 = s.Sum(sm => sm.TotalVat1),
                                             //TotalVat2 = s.Sum(sm => sm.TotalVat2),
                                             //TotalVat3 = s.Sum(sm => sm.TotalVat3),
                                             //TotalVat4 = s.Sum(sm => sm.TotalVat4),
                                             //TotalVat5 = s.Sum(sm => sm.TotalVat5),
                                             TotalDiscount = s.FirstOrDefault().TotalDiscount,
                                             Bonus = s.FirstOrDefault().Bonus,
                                             PriceList = s.FirstOrDefault().PriceList,
                                             ReceiptNo = s.FirstOrDefault().ReceiptNo,
                                             Change = s.FirstOrDefault().Change,
                                             PaidAmount = s.FirstOrDefault().PaidAmount,
                                             IsVoid = s.FirstOrDefault().IsVoid,
                                             DetailsId = string.Join("+", s.Select(f => f.Guid).Distinct()) + "," + s.FirstOrDefault().PosInfoDetailId.ToString(),
                                             Details = s.Select(ss => new
                                             {
                                                 ItemCustomRemark = "",
                                                 ItemVatRate = ss.ItemVatRate,
                                                 ItemVatValue = Math.Round(ss.ItemVatValue.Value, 2, MidpointRounding.AwayFromZero),
                                                 ItemVatDesc = ss.ItemVatDesc,
                                                 ItemDiscount = ss.ItemDiscount,//!= null ? Math.Round(oding.Discount.Value, 2, MidpointRounding.AwayFromZero) : 0
                                                 ItemNet = 0,
                                                 ItemGross = ss.Total,
                                                 Total = ss.Total,
                                                 ItemPosition = 0,
                                                 ItemSort = 0,
                                                 ItemRegion = "",
                                                 RegionPosition = 0,
                                                 ItemBarcode = 0,
                                                 ItemDescr = ss.ItemDescr,
                                                 IsChangeItem = ss.IsChangeItem,
                                                 KitchenId = ss.KitchenId,
                                                 ItemPrice = Math.Round((Decimal)(ss.Total / (decimal?)ss.ItemQty), 2, MidpointRounding.AwayFromZero),
                                                 ItemCode = ss.ItemCode,
                                                 ItemQty = ss.ItemQty,
                                                 SalesTypeDesc = ss.SalesTypeDesc,
                                                 Extras = s.Where(w => w.OrderDetailId == ss.OrderDetailId && ss.IngItemCode != null)
                                                           .Select(sss => new
                                                           {
                                                               ItemCode = sss.IngItemCode,
                                                               ItemDescr = sss.IngItemQty > 0 ? sss.IngDescription : "ΧΩΡΙΣ" + sss.IngDescription,
                                                               IsChangeItem = sss.IngIsChangeItem,
                                                               ItemGross = sss.IngItemGross,
                                                               ItemPrice = sss.IngItemPrice,
                                                               ItemDiscount = sss.IngItemDiscount,
                                                               ItemQty = sss.IngItemQty,
                                                               ItemVatRate = sss.ItemVatRate,
                                                               ItemVatDesc = sss.ItemVatDesc,
                                                           }).Distinct()
                                             }).Distinct(),
                                             PaymentsList = s.Where(w => w.AccountId != -1).Select(sss => new
                                             {
                                                 AccountId = sss.AccountId,
                                                 Amount = sss.Amount,
                                                 AccountType = sss.AccountType,
                                                 Description = sss.AccountDescription

                                             }).Distinct()

                                         }
                );
            db.Configuration.AutoDetectChangesEnabled = true;

            return reprintReceipt.FirstOrDefault();
            #region to be deleted
            //db.Configuration.LazyLoadingEnabled = true;
            //var invoice = db.Invoices
            //    .Include(i => i.OrderDetailInvoices).Include(i => i.OrderDetailInvoices.Select(ii => ii.OrderDetail.Order))
            //    .Include(i => i.PosInfo.Department).Include(i => i.OrderDetailInvoices.Select(ii => ii.OrderDetail.OrderDetailVatAnal))
            //    .Include(i => i.OrderDetailInvoices.Select(ii => ii.OrderDetail.OrderDetailIgredients))
            //    .Include(i => i.OrderDetailInvoices.Select(ii => ii.OrderDetail.OrderDetailIgredients.Select(iii => iii.OrderDetailIgredientVatAnal)))
            //    .Include(i => i.OrderDetailInvoices.Select(ii => ii.OrderDetail.OrderDetailIgredients.Select(iii => iii.Ingredients)))
            //    .Include(i => i.Guest).Include(i => i.Table).Include(i => i.OrderDetailInvoices.Select(ii => ii.PosInfoDetail))
            //    .Include(i => i.OrderDetailInvoices.Select(ii => ii.OrderDetail.SalesType)).Include(i => i.Transactions.Select(ii => ii.Accounts)).Include(i => i.Staff)
            //    .Include(i => i.OrderDetailInvoices.Select(ii => ii.OrderDetail.Product))
            //    .Include(i => i.Transactions.Select(ii=> ii.Invoice_Guests_Trans.Select(iii=>iii.Guest)))
            //    .Where(w => w.Id == invoiceId).ToList();
            //var queryGrouped = from s in invoice
            //                   select new
            //                   {
            //                       OrderId = string.Join("-", s.OrderDetailInvoices.Select(ss => ss.OrderDetail.OrderId).Distinct()),
            //                       OrderNo = string.Join(",", s.OrderDetailInvoices.Select(ss => ss.OrderDetail.Order.OrderNo).Distinct()),
            //                       FiscalType = s.PosInfo.FiscalType,
            //                       InvoiceIndex = s.OrderDetailInvoices.FirstOrDefault().PosInfoDetail.InvoiceId,
            //                       TableNo = s.Table != null ? s.Table.Code : null,
            //                      // RoomNo = s.Guest != null ? s.Guest.Room : null,
            //                       RoomNo = s.Transactions.FirstOrDefault() != null && s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault() == null ? null : s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault().Guest.Room,
            //                       RoomDescription = s.Transactions.FirstOrDefault() != null && s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault() == null ? null : s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault().Guest.Room,
            //                       Waiter = s.Staff.LastName,
            //                       WaiterNo = s.Staff.Code,
            //                       Pos = "POS-" + s.PosInfoId.ToString(),
            //                       PosDescr = s.PosInfo.Description,
            //                       DepartmentDesc = s.PosInfo.Department.Description,
            //                       //Department = s.PosInfo.Department,
            //                       //s.Transactions.FirstOrDefault() != null &&   s.Transactions.FirstOrDefault().TransactionInvoice_Guests_Trans.FirstOrDefault() == null ? 0 : s.Transactions.FirstOrDefault().TransactionInvoice_Guests_Trans.FirstOrDefault().
            //                       CustomerName = s.Transactions.FirstOrDefault() != null && s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault() == null ? "ΠΕΛΑΤΗΣ ΛΙΑΝΙΚΗΣ" : s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault().Guest.LastName,
            //                       CustomerAddress = s.Transactions.FirstOrDefault() != null && s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault() == null ? "" : s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault().Guest.Address,
            //                       CustomerDeliveryAddress = "",
            //                       CustomerPhone = "",
            //                       CustomerComments = "",
            //                       CustomerAfm = "",
            //                       CustomerDoy = "",
            //                       CustomerJob = "",
            //                       RegNo = s.Transactions.FirstOrDefault() != null && s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault() == null ? 0 : s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault().Guest.ReservationId,
            //                       SumOfLunches = 0,
            //                       SumofConsumedLunches = 0,
            //                       GuestTerm = s.Transactions.FirstOrDefault() != null && s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault() == null ? "" : s.Transactions.FirstOrDefault().Invoice_Guests_Trans.FirstOrDefault().Guest.BoardCode,
            //                       Adults = 0,
            //                       Kids = 0,
            //                       InvoiceType = s.OrderDetailInvoices.FirstOrDefault().PosInfoDetail.InvoiceId,
            //                       Total = s.Total,//(decimal)s.Sum(ss => ss.ItemGross) - (decimal)s.Sum(ss => ss.ItemDiscount),
            //                       ReceiptTypeDescription = s.OrderDetailInvoices.FirstOrDefault().PosInfoDetail.Description,
            //                       DepartmentTypeDescription = s.PosInfo.Department.Description,
            //                       Couver = s.Cover,
            //                       PrintKitchen = false,
            //                       SalesTypeDescription = s.OrderDetailInvoices.FirstOrDefault().OrderDetail.SalesType.Description,
            //                       //TotalVat1 = s.Sum(sm => sm.TotalVat1),
            //                       //TotalVat2 = s.Sum(sm => sm.TotalVat2),
            //                       //TotalVat3 = s.Sum(sm => sm.TotalVat3),
            //                       //TotalVat4 = s.Sum(sm => sm.TotalVat4),
            //                       //TotalVat5 = s.Sum(sm => sm.TotalVat5),
            //                       TotalDiscount = 0,//Only Discount on Items is shown //s.Discount,
            //                       Bonus = 0,
            //                       PriceList = 0,
            //                       ReceiptNo = s.Counter,
            //                       Change = 0,
            //                       PaidAmount = s.Total,//s.FirstOrDefault().Transactions !=null ? s.Select(ss => ss.Transactions).Distinct().Sum(sum => sum.Gross) != null ? Math.Round(s.Select(ss => ss.Transactions).Distinct().Sum(sum => sum.Gross).Value, 2) : 0 : 0,
            //                       IsVoid = s.IsVoided != null ? s.IsVoided : false,
            //                       DetailsId = string.Join("+", s.OrderDetailInvoices.Select(f => f.OrderDetail.Guid).Distinct()) + "," + s.OrderDetailInvoices.FirstOrDefault().PosInfoDetailId.ToString(),
            //                       Details = (from q in s.OrderDetailInvoices
            //                                  join j in db.OrderDetailVatAnal on q.OrderDetailId equals j.OrderDetailId
            //                                  join v in db.Vat on j.VatId equals v.Id
            //                                  select new
            //                                  {
            //                                      ItemCustomRemark = "",
            //                                      ItemVatRate = v.Code,
            //                                      ItemVatValue = j.VatAmount,
            //                                      ItemVatDesc = j.VatRate,
            //                                      ItemDiscount = Math.Round(q.OrderDetail.Discount != null ? q.OrderDetail.Discount.Value : 0, 2, MidpointRounding.AwayFromZero),
            //                                      ItemNet = 0,
            //                                      ItemGross = q.OrderDetail.TotalAfterDiscount >= 0 ? ((decimal?)q.OrderDetail.Qty * q.OrderDetail.Price) : ((decimal?)q.OrderDetail.Qty * q.OrderDetail.Price) * -1,
            //                                      Total = q.OrderDetail.TotalAfterDiscount >= 0 ? ((decimal?)q.OrderDetail.Qty * q.OrderDetail.Price) : ((decimal?)q.OrderDetail.Qty * q.OrderDetail.Price) * -1,
            //                                      ItemPosition = 0,
            //                                      ItemSort = 0,
            //                                      ItemRegion = "",
            //                                      RegionPosition = 0,
            //                                      ItemBarcode = 0,
            //                                      ItemDescr = q.OrderDetail.Product.Description,
            //                                      IsChangeItem = q.OrderDetail.TotalAfterDiscount < 0 || q.OrderDetail.Status == 5,
            //                                      KitchenId = q.OrderDetail.KitchenId,
            //                                      ItemPrice = q.OrderDetail.TotalAfterDiscount >= 0 ? q.OrderDetail.Price : q.OrderDetail.Price * -1,
            //                                      ItemCode = q.OrderDetail.ProductId,
            //                                      ItemQty = q.OrderDetail.Qty,
            //                                      SalesTypeDesc = q.OrderDetail.SalesType.Description,
            //                                      Extras = (from sss in q.OrderDetail.OrderDetailIgredients
            //                                                join jj in db.OrderDetailIgredientVatAnal on sss.Id equals jj.OrderDetailIgredientsId
            //                                                join vat in db.Vat on jj.VatId equals vat.Id
            //                                                select new
            //                                       {
            //                                           ItemCode = sss.IngredientId,
            //                                           ItemDescr = sss.Qty > 0 ? sss.Ingredients as Ingredients != null ? sss.Ingredients.Description : "ΧΩΡΙΣ" + sss.Ingredients.Description : "",
            //                                           IsChangeItem = sss.Price < 0,
            //                                           ItemGross = sss.Price >= 0 ? sss.Price : sss.Price * -1,
            //                                           ItemPrice = sss.Price >= 0 ? sss.Qty > 1 ? (decimal)(sss.Price / (decimal)sss.Qty) : sss.Price : (sss.Qty > 1 ? (decimal)(sss.Price / (decimal)sss.Qty) : sss.Price) * -1,
            //                                           ItemDiscount = sss.Discount != null ? Math.Round(sss.Discount.Value, 2, MidpointRounding.AwayFromZero) : 0,
            //                                           ItemQty = sss.Qty,
            //                                           ItemVatRate = vat.Code,//db.Vat.Where(w => w.Id == sss.OrderDetailIgredientVatAnal.FirstOrDefault().VatId).FirstOrDefault() != null ? db.Vat.Where(w => w.Id == sss.OrderDetailIgredientVatAnal.FirstOrDefault().VatId).FirstOrDefault().Code : sss.OrderDetailIgredientVatAnal.FirstOrDefault().VatId,
            //                                           ItemVatDesc = sss.OrderDetailIgredientVatAnal.FirstOrDefault().VatRate,
            //                                       })
            //                                  }),
            //                       PaymentsList = s.Transactions.Select(ss => new
            //                       {
            //                           AccountId = ss.Accounts.Description,
            //                           Amount = ss.Amount,
            //                           AccountType = ss.Accounts.Type,
            //                           Description = ss.Accounts.Description,
            //                           Guest = new {
            //                               FirstName = ss.Invoice_Guests_Trans.FirstOrDefault() != null && ss.Invoice_Guests_Trans.FirstOrDefault().Guest != null 
            //                                                ? ss.Invoice_Guests_Trans.FirstOrDefault().Guest.FirstName : "",
            //                               LastName = ss.Invoice_Guests_Trans.FirstOrDefault() != null && ss.Invoice_Guests_Trans.FirstOrDefault().Guest != null 
            //                                                ? ss.Invoice_Guests_Trans.FirstOrDefault().Guest.LastName : "",
            //                               Room = ss.Invoice_Guests_Trans.FirstOrDefault() != null && ss.Invoice_Guests_Trans.FirstOrDefault().Guest  != null
            //                                                ? ss.Invoice_Guests_Trans.FirstOrDefault().Guest.Room : ""
            //                           }

            //                          // Description = ss.Accounts.Description + (ss.Invoice_Guests_Trans.FirstOrDefault() != null ? " Room : " + ss.Invoice_Guests_Trans.FirstOrDefault().Guest.Room : "")

            //                      //     Description = ss.Accounts.Description
            //                       })
            //                   };
            //db.Configuration.LazyLoadingEnabled = false;
            //return queryGrouped.SingleOrDefault();
            #endregion
        }
        #endregion

        public Object GetOrders(string date, long posid)
        {
            //DateTime dtime = Convert.ToDateTime(date);
            //DateTime dtimeend = dtime.AddDays(1);
            var query = db.Order.Include(i => i.EndOfDay).Include(i => i.Transactions).Include("OrderDetail").Where(w => w.EndOfDayId == null && w.PosId == posid).Select(s => new //Order()
            {
                Id = s.Id,
                Day = s.Day,
                EndOfDayId = s.EndOfDayId,
                Staff = s.Staff,
                StaffId = s.StaffId,
                OrderNo = s.OrderNo,
                ReceiptNo = s.ReceiptNo,
                OrderStatus = s.OrderStatus,
                PosId = s.PosId,
                PosInfo = s.PosInfo,
                Total = s.Total,
                FODay = s.EndOfDay.FODay,
                AccountId = s.Transactions.FirstOrDefault() != null ? s.Transactions.FirstOrDefault().AccountId : null,
                PreparationTime = s.OrderDetail.Max(m => m.PreparationTime),
                SalesType = s.OrderDetail.Select(ss => ss.SalesTypeId).Distinct().Count() == 1 ? s.OrderDetail.FirstOrDefault().SalesType.Abbreviation : null,
                OrderDetail = s.OrderDetail.Select(ss => new
                {
                    Id = ss.Id,
                    ProductId = ss.ProductId,
                    Description = ss.Product.Description,
                    VatCode = ss.PricelistDetail.Vat.Code,
                    VatDesc = ss.PricelistDetail.Vat.Percentage,
                    Qty = ss.Qty,
                    SalesTypeId = ss.SalesTypeId,
                    SalesType = ss.SalesType.Abbreviation,
                    Price = ss.Price,
                    OrderDetailIgredients = ss.OrderDetailIgredients.Select(sss => new
                    {
                        Id = sss.Id,
                        Qty = sss.Qty,
                        IngredientId = sss.IngredientId,
                        Description = sss.Ingredients.Description,
                        Price = sss.Price,
                        Units = sss.UnitId,
                        VatCode = sss.PricelistDetail.Vat.Code,
                        VatDesc = sss.PricelistDetail.Vat.Percentage
                    })
                })
            });
            return new { query };
        }

        #region Old KDS Query
        //modified for new kds
        public Object GetOrders(string date, string kdsids, string sltypeIds, string filter)
        {

            dynamic totals = new System.Dynamic.ExpandoObject();
            IEnumerable<long?> kdslist = JsonConvert.DeserializeObject<List<long?>>(kdsids);
            IEnumerable<long?> sltlist = JsonConvert.DeserializeObject<List<long?>>(sltypeIds);

            List<OrderDetail> q = db.OrderDetail.AsNoTracking().Include(i => i.Order).AsNoTracking().Include("Order.OrderStatus").AsNoTracking().Include("OrderDetailInvoices").AsNoTracking()
                .Include("Product").AsNoTracking().Include("SalesType").AsNoTracking().Include("OrderDetailIgredients").AsNoTracking().Include("OrderDetailIgredients.Ingredients").AsNoTracking()
                .Where(w => w.Order.EndOfDayId == null && w.Status > 0 && w.Price >= 0 && kdslist.Contains(w.KdsId) && sltlist.Contains(w.SalesTypeId) && (w.IsDeleted ?? false) == false).ToList();// && w.Price >= 0
            List<OrderDetail> queryfiltered = new List<OrderDetail>() { };
            foreach (OrderDetail item in q)
            {
                if (item.Order.ExtType == (int)ExternalSystemOrderEnum.Forkey && !String.IsNullOrEmpty(item.Order.ExtObj) )
                {
                    try
                    {
                        dynamic e = cjson.DynamicDeseriallize(item.Order.ExtObj);
                        List<OrderStatus> statuses = item.Order.OrderStatus.OrderByDescending(i => i.Status).ToList();
                        if (statuses!=null && statuses[0].Status >=(long?) Symposium.Models.Enums.OrderStatusEnum.Preparing)// (e != null && e.isPrinted != null && e.isPrinted == true)
                        {
                            queryfiltered.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }
                }
                else {
                    queryfiltered.Add(item);
                }
            }


            totals.pending = queryfiltered.Where(w => w.Status <= 2 && w.Status >= 1).GroupBy(g => g.OrderId).Count();
            totals.ready = queryfiltered.Where(w => w.Status == 3).GroupBy(g => g.OrderId).Count();
            totals.cancel = queryfiltered.Where(w => w.Status == 5).GroupBy(g => g.OrderId).Count();
            //int takevar = 200;
            switch (filter)
            {
                case "pending":
                    //takevar = 400;
                    queryfiltered = queryfiltered.Where(w => w.Status <= 2).ToList();//.Take(takevar).ToList();
                    break;
                case "ready":
                    //takevar = 50;
                    queryfiltered = queryfiltered.Where(w => w.Status == 3).OrderByDescending(o => o.Id).ToList();//.Take(takevar).ToList();
                    break;
                case "cancel":
                    //takevar = 50;
                    queryfiltered = queryfiltered.Where(w => w.Status == 5).OrderByDescending(o => o.Id).ToList();//.Take(takevar).ToList();
                    break;
                default:
                    //takevar = 50;
                    break;
            }


            var query1 = queryfiltered.GroupBy(g => g.OrderId).Select(s => new
            {
                OrderId = s.FirstOrDefault().OrderId,
                OrderDetailCount = s.Count()
            });

            var query = (from s in queryfiltered
                         select new
                         {
                             Guid = s.Guid,
                             OrderId = s.Order.Id,
                             Day = s.Order.Day,
                             EndOfDayId = s.Order.EndOfDayId,
                             OrderNo = s.Order.OrderNo,//s.OrderDetailInvoices.FirstOrDefault() != null ? s.OrderDetailInvoices.FirstOrDefault().Counter : s.Order.OrderNo,
                             ReceiptNo = s.Order.ReceiptNo,
                             ItemRemark = s.OrderDetailInvoices.FirstOrDefault() != null ? s.OrderDetailInvoices.FirstOrDefault().ItemRemark : null,//s.Order.OrderNo,//s.Order.OrderNo,
                             OrderStatus = s.Order.OrderStatus.Select(ss => new
                             {
                                 Id = ss.Id,
                                 Status = ss.Status,
                                 TimeChanged = ss.TimeChanged,
                                 OrderId = ss.OrderId,
                                 StaffId = ss.StaffId
                             }),
                             Status = s.Status,
                             StatusTS = s.StatusTS,
                             IsReady = s.Status == 3 ? true : false,
                             IsCancelled = s.Status == 5 ? true : false,// s.Order.OrderStatus.Select(ss => ss.Status).Contains(5) ? true : false,
                             PreparationTime = s.Order.OrderDetail.Max(m => m.PreparationTime),
                             PosId = s.Order.PosId,
                             Total = s.Order.Total,
                             Id = s.Id,
                             ProductId = s.ProductId,
                             Product = s.Product.Description,
                             Qty = s.Qty,
                             PendingQty = (s.Status == 3 || s.Status == 5) ? 0 : s.PendingQty,
                             KdsId = s.KdsId,
                             SalesTypeId = s.SalesTypeId,
                             SalesType = s.SalesType.Abbreviation,
                             OrderDetailIgredients = s.OrderDetailIgredients.Select(sss => new
                             {
                                 Id = sss.Id,
                                 Qty = sss.Qty,
                                 IngredientId = sss.IngredientId,
                                 Ingredient = sss.Ingredients.Description
                             }),
                             OrderDetailLength = query1.Where(w => w.OrderId == s.Order.Id).Count() > 0 ? query1.Where(w => w.OrderId == s.Order.Id).FirstOrDefault().OrderDetailCount : 0
                         }).Where(w => w.KdsId != null).ToList();
            //var xx = query.Where(w => w.KdsId != null).ToList();
           var result= new { query, totals };
            db.Dispose();
            query = null;
            totals = null;
            query1 = null;
            q = null;
            cjson = null;
            return result;
        }
        #endregion

        #region New KDS Query
        //public Object GetOrders(string date, string kdsids, string flat, string filter)
        //{

        //    dynamic totals = new System.Dynamic.ExpandoObject();
        //    IEnumerable<long?> kdslist = JsonConvert.DeserializeObject<List<long?>>(kdsids);
        //    var queryfiltered = db.OrderDetail.AsNoTracking().Include(i => i.Order).Include("Order.OrderStatus").Include("OrderDetailInvoices")
        //        .Include("Product").Include("SalesType").Include("OrderDetailIgredients").Include("OrderDetailIgredients.Ingredients")
        //        .Where(w => w.Order.EndOfDayId == null && kdslist.Contains(w.KdsId)).ToList();// && w.Price >= 0
        //    totals.pending = queryfiltered.Where(w => w.Status <= 2).GroupBy(g => g.OrderId).Count();
        //    totals.ready = queryfiltered.Where(w => w.Status == 3).GroupBy(g => g.OrderId).Count();
        //    totals.cancel = queryfiltered.Where(w => w.Status == 5).GroupBy(g => g.OrderId).Count();
        //    int takevar = 200;
        //    switch (filter)
        //    {
        //        case "pending":
        //            queryfiltered = queryfiltered.Where(w => w.Status <= 2).ToList();
        //            takevar = 400;
        //            break;
        //        case "ready":
        //            queryfiltered = queryfiltered.Where(w => w.Status == 3).ToList();
        //            takevar = 50;
        //            break;
        //        case "cancel":
        //            queryfiltered = queryfiltered.Where(w => w.Status == 5).ToList();
        //            takevar = 50;
        //            break;
        //        default:
        //            takevar = 50;
        //            break;
        //    }


        //    var query1 = queryfiltered.GroupBy(g => g.OrderId).Select(s => new
        //    {
        //        OrderId = s.FirstOrDefault().OrderId,
        //        OrderDetailCount = s.Count()
        //    });

        //    var q1 = (from s in queryfiltered
        //              join ord in db.Order on s.OrderId equals ord.Id
        //              join invo in db.OrderDetailInvoices on s.Id equals invo.OrderDetailId
        //              join prod in db.Product on s.ProductId equals prod.Id
        //              join os in db.OrderDetailIgredients on s.Id equals os.OrderDetailId into ff
        //              from jodi in ff.DefaultIfEmpty(new OrderDetailIgredients() {Id = 0 })
        //              join ing in db.Ingredients on jodi.IngredientId equals ing.Id into fff
        //              from jing in fff.DefaultIfEmpty()
        //              join ordst in db.OrderStatus on s.OrderId equals ordst.OrderId into ffff
        //              from jordstat in ffff.DefaultIfEmpty()
        //              join st in db.SalesType on s.SalesTypeId equals st.Id
        //              select new
        //              {
        //                  OrderId = ord.Id,//s.Order.Id,
        //                  Day = ord.Day,//s.Order.Day,
        //                  EndOfDayId = ord.EndOfDay,//s.Order.EndOfDayId,
        //                  OrderNo = invo.Counter,//.FirstOrDefault() != null ? invo.FirstOrDefault().Counter : ord.OrderNo,//s.Order.OrderNo,
        //                  OrderStatus = jordstat,
        //                  Status = s.Status,
        //                  StatusTS = s.StatusTS,
        //                  IsReady = s.Status == 3 ? true : false,
        //                  IsCancelled = s.Status == 5 ? true : false,
        //                  PreparationTime = ord.OrderDetail.Max(m => m.PreparationTime),//s.Order.OrderDetail.Max(m => m.PreparationTime),
        //                  PosId = ord.PosId,//s.Order.PosId,
        //                  Total = ord.Total,
        //                  Id = s.Id,
        //                  ProductId = s.ProductId,
        //                  Product = prod.Description,
        //                  Qty = s.Qty,
        //                  KdsId = s.KdsId,
        //                  SalesTypeId = s.SalesTypeId,
        //                  SalesType = st.Abbreviation,
        //                  //							 OrderDetailIgredients = jing,
        //                  OrderDetailIgredients = new
        //                  {
        //                      Id = jodi != null ? jodi.Id : 0,
        //                      OrderDetailId = jodi != null ? jodi.OrderDetailId : 0,
        //                      Qty = jodi != null ? jodi.Qty : null,
        //                      IngredientId = jodi != null ? jodi.IngredientId : null,
        //                      Ingredient = jing != null ? jing.Description : ""//jodi != null ? db.Ingredients.Where(w => w.Id == jodi.IngredientId).FirstOrDefault().Description : ""
        //                  },
        //                  OrderDetailLength = query1.Where(w => w.OrderId == s.Order.Id).Count() > 0 ? query1.Where(w => w.OrderId == s.Order.Id).FirstOrDefault().OrderDetailCount : 0
        //              });



        //    var query = q1.ToList().GroupBy(g => g.Id).Select(s => new
        //    {
        //        OrderId = s.FirstOrDefault().OrderId,
        //        Day = s.FirstOrDefault().Day,
        //        EndOfDayId = s.FirstOrDefault().EndOfDayId,//s.Order.EndOfDayId,
        //        OrderNo = s.FirstOrDefault().OrderNo,//.FirstOrDefault() != null ? invo.FirstOrDefault().Counter : ord.OrderNo,//s.Order.OrderNo,
        //        OrderStatus = s.Select(ss => new
        //        {
        //            Id = ss.OrderStatus.Id,
        //            Status = ss.OrderStatus.Status,
        //            TimeChanged = ss.OrderStatus.TimeChanged,
        //            OrderId = ss.OrderStatus.OrderId,
        //            StaffId = ss.OrderStatus.StaffId
        //        }),
        //        Status = s.FirstOrDefault().Status,
        //        StatusTS = s.FirstOrDefault().StatusTS,
        //        IsReady = s.FirstOrDefault().Status == 3 ? true : false,
        //        IsCancelled = s.FirstOrDefault().Status == 5 ? true : false,
        //        PreparationTime = s.FirstOrDefault().PreparationTime,//s.Order.OrderDetail.Max(m => m.PreparationTime),
        //        PosId = s.FirstOrDefault().PosId,//s.Order.PosId,
        //        Total = s.FirstOrDefault().Total,
        //        Id = s.FirstOrDefault().Id,
        //        ProductId = s.FirstOrDefault().ProductId,
        //        Product = s.FirstOrDefault().Product,
        //        Qty = s.FirstOrDefault().Qty,
        //        KdsId = s.FirstOrDefault().KdsId,
        //        SalesTypeId = s.FirstOrDefault().SalesTypeId,
        //        SalesType = s.FirstOrDefault().SalesType,
        //        OrderDetailIgredients = s.Select(sel => sel.OrderDetailIgredients).Where(odi => odi.Id != 0 && odi.OrderDetailId == s.FirstOrDefault().Id).Distinct(),
        //        OrderDetailLength = s.FirstOrDefault().OrderDetailLength
        //    }).Take(takevar);

        //    return new { query, totals };
        //}
        #endregion

        public Object GetOrders(string date, string kdsids, string flat)
        {
            IEnumerable<long?> kdslist = JsonConvert.DeserializeObject<List<long?>>(kdsids);

            var query1 = db.OrderDetail.Include(i => i.Order).Where(w => w.Order.EndOfDayId == null //&& (w.Status <= 2 || w.Status == 5)
                                    && kdslist.Contains(w.KdsId) && w.Price >= 0).GroupBy(g => g.OrderId).Select(s => new
                                    {
                                        OrderId = s.FirstOrDefault().OrderId,
                                        OrderDetailCount = s.Count()
                                    });

            var query = (from s in db.OrderDetail.Include(i => i.Order)
                         where s.Order.EndOfDayId == null// && (s.Status <= 2 || s.Status == 5)
                                    && kdslist.Contains(s.KdsId) && s.Price >= 0
                         select new
                         {
                             OrderId = s.Order.Id,
                             Day = s.Order.Day,
                             EndOfDayId = s.Order.EndOfDayId,
                             OrderNo = s.Order.OrderNo,
                             OrderStatus = s.Order.OrderStatus,
                             Status = s.Status,
                             StatusTS = s.StatusTS,
                             IsReady = s.Status == 3 ? true : false,
                             IsCancelled = s.Order.OrderStatus.Select(ss => ss.Status).Contains(5) ? true : false,
                             PreparationTime = s.Order.OrderDetail.Max(m => m.PreparationTime),
                             PosId = s.Order.PosId,
                             Total = s.Order.Total,
                             Id = s.Id,
                             ProductId = s.ProductId,
                             Product = s.Product.Description,
                             Qty = s.Qty,
                             KdsId = s.KdsId,
                             SalesTypeId = s.SalesTypeId,
                             SalesType = s.SalesType.Abbreviation,
                             OrderDetailIgredients = s.OrderDetailIgredients.Select(sss => new
                             {
                                 Id = sss.Id,
                                 Qty = sss.Qty,
                                 IngredientId = sss.IngredientId,
                                 Ingredient = sss.Ingredients.Description
                             }),
                             OrderDetailLength = query1.Where(w => w.OrderId == s.Order.Id).Count() > 0 ? query1.Where(w => w.OrderId == s.Order.Id).FirstOrDefault().OrderDetailCount : 0
                         });
          //  return new { query };
            var result = new { query };
            db.Dispose();
            query = null;
            query1 = null;
            return result;
        }

        public Object GetOrders(long orderid, string flat)
        {

            var query1 = db.OrderDetail.Where(w => w.OrderId == orderid).GroupBy(g => g.OrderId).Select(s => new
            {
                OrderId = s.FirstOrDefault().OrderId,
                OrderDetailCount = s.Count()
            });

            var query = (from s in db.OrderDetail
                         where s.OrderId == orderid
                         select new
                         {
                             OrderId = s.Order.Id,
                             Day = s.Order.Day,
                             EndOfDayId = s.Order.EndOfDayId,
                             OrderNo = s.Order.OrderNo,
                             OrderStatus = s.Order.OrderStatus,
                             Status = s.Status,
                             StatusTS = s.StatusTS,
                             PreparationTime = s.Order.OrderDetail.Max(m => m.PreparationTime),
                             PosId = s.Order.PosId,
                             Total = s.Order.Total,
                             Id = s.Id,
                             ProductId = s.ProductId,
                             Product = s.Product.Description,
                             Qty = s.Qty,
                             KdsId = s.KdsId,
                             SalesTypeId = s.SalesTypeId,
                             SalesType = s.SalesType.Abbreviation,
                             IsReady = s.Status == 3 ? true : false,
                             IsCancelled = s.Order.OrderStatus.Select(ss => ss.Status).Contains(5) ? true : false,
                             IsChangeItem = false,
                             OrderDetailIgredients = s.OrderDetailIgredients.Select(sss => new
                             {
                                 Id = sss.Id,
                                 Qty = sss.Qty,
                                 IngredientId = sss.IngredientId,
                                 Description = sss.Ingredients.Description
                             }),
                             OrderDetailLength = query1.Where(w => w.OrderId == s.Order.Id).Count() > 0 ? query1.Where(w => w.OrderId == s.Order.Id).FirstOrDefault().OrderDetailCount : 0
                         });
            return new { query };
        }

        public Object GetOrders(long id, bool forreceipt)
        {
            Order order = db.Order.Include(i => i.OrderDetail).Include(i => i.OrderDetail.Select(s => s.Table))
                .Include(i => i.OrderDetail.Select(s => s.Product)).Include(i => i.OrderDetail.Select(s => s.OrderDetailInvoices))
                .Include(i => i.OrderDetail.Select(s => s.OrderDetailIgredients))
                .Include(i => i.OrderDetail.Select(s => s.OrderDetailIgredients.Select(ss => ss.Ingredients)))
                .Include(i => i.PosInfo).Include(i => i.PosInfo.Department).Include(i => i.OrdersStaff).Include(i => i.OrdersStaff.Select(s => s.Staff))
                .Where(w => w.Id == id).FirstOrDefault();
            if (order == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            Receipt rec = new Receipt();
            rec.Id = order.Id;
            rec.Change = 0;
            var pid = db.PosInfoDetail.Find(order.OrderDetail.FirstOrDefault().OrderDetailInvoices.LastOrDefault().PosInfoDetailId);
            if (pid != null)
            {
                rec.InvoiceIndex = pid.InvoiceId;
            }
            rec.OrderNo = order.OrderNo;
            rec.PosDescr = order.PosInfo.Description;
            rec.PosId = order.PosId;
            rec.ReceiptNo = order.ReceiptNo;
            rec.TableNo = order.OrderDetail.FirstOrDefault().Table != null ? order.OrderDetail.FirstOrDefault().Table.Code : "";
            rec.Total = order.Total;
            rec.DepartmentTypeDescription = order.PosInfo.Department != null ? order.PosInfo.Department.Description : "";
            rec.ReceiptTypeDescription = pid != null ? pid.Description : "";
            var waiter = order.OrdersStaff.Where(w => w.Type == 1).FirstOrDefault();
            if (waiter != null)
            {
                rec.Waiter = waiter.Staff.FirstName;
                rec.WaiterNo = waiter.Staff.Id;
            }
            rec.DepartmentTypeDescription = order.PosInfo.Department != null ? order.PosInfo.Department.Description : "";
            rec.ReceiptTypeDescription = pid != null ? pid.Description : "";
            rec.OrderDetail = new List<ReceiptDetail>();
            foreach (var d in order.OrderDetail)
            {
                ReceiptDetail od = new ReceiptDetail();
                od.Id = d.Id;
                od.Guid = d.Guid;
                od.AA = d.AA;
                od.Description = d.Product.Description;
                od.IsChangeItem = d.Price < 0 ? true : false;
                od.KitchenCode = d.KitchenId != null ? db.Kitchen.Find(d.KitchenId) != null ? db.Kitchen.Find(d.KitchenId).Code : null : null;
                od.Price = d.Price;
                od.TotalAfterDiscount = d.TotalAfterDiscount;
                od.Discount = d.Discount != null ? Math.Round(d.Discount.Value, 2) : 0;
                od.ProductId = d.ProductId;
                od.Qty = d.Qty;
                od.OrderDetailIgredients = new List<ReceiptExtra>();
                var vat1 = db.PricelistDetail.Include("Vat").Where(w => w.Id == d.PriceListDetailId).FirstOrDefault();
                if (vat1 != null)
                {
                    od.VatCode = vat1.Vat.Code.ToString();
                    od.VatDesc = vat1.Vat.Percentage.ToString();
                }
                foreach (var e in d.OrderDetailIgredients)
                {
                    ReceiptExtra re = new ReceiptExtra();
                    re.Description = e.Ingredients.Description;
                    re.IngredientId = e.IngredientId;
                    re.Price = e.Price;
                    re.Qty = e.Qty;
                    re.TotalAfterDicount = e.TotalAfterDiscount;
                    var vat = db.PricelistDetail.Include("Vat").Where(w => w.Id == e.PriceListDetailId).FirstOrDefault();
                    if (vat != null)
                    {
                        re.VatCode = vat.Vat.Code.ToString();
                        re.VatDesc = vat.Vat.Percentage.ToString();
                    }
                    od.OrderDetailIgredients.Add(re);
                }
                rec.OrderDetail.Add(od);
            }
            return rec;
        }

        // GET api/Order/5
        public Order GetOrder(long id)
        {
            Order order = db.Order.Find(id);
            if (order == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return order;
        }

        // PUT api/Order/5
        public HttpResponseMessage PutOrder(long id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != order.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(order).State = EntityState.Modified;

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

        // PUT api/Order/5
        public HttpResponseMessage PutOrder(long id, long staffid, string forstaff) //ONLY STAFF UPDATE
        {
            Order order = db.Order.Where(w => w.Id == id).FirstOrDefault();
            if (order == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ModelState);
            }
            order.StaffId = staffid;

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, order);
        }

        public HttpResponseMessage PutOrder(string storeid, IEnumerable<TableCovers> coverAnalysis) //ONLY COVER UPDATE
        {
            foreach (var orderCover in coverAnalysis)
            {
                try
                {
                    Order order = db.Order.Where(w => w.Id == orderCover.OrderId).FirstOrDefault();
                    if (order == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ModelState);
                    }
                    order.Couver = orderCover.Cover;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                   // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.ExpectationFailed };
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT api/Order/5
        public Object GetOrders(long id, int receiptno, string forreceiptno) //ONLY ReceiptNo UPDATE
        {
            Order order = db.Order.Where(w => w.Id == id).FirstOrDefault();
            if (order == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ModelState);
            }
            order.ReceiptNo = receiptno;

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, order);
        }


        #region Post Order
        //public HttpResponseMessage PostOrder(Order order)
        //{

        //    PosInfo pi = db.PosInfo.Include("PosInfoDetail").Where(w => w.Id == order.PosId).FirstOrDefault();
        //    pi.ReceiptCount += 1;
        //    order.OrderNo = pi.ReceiptCount;

        //    PosInfoDetail piDet = pi.PosInfoDetail.Where(w => w.Id == order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().PosInfoDetailId).FirstOrDefault();

        //    var piDetGroup = pi.PosInfoDetail.Where(w => w.GroupId != null && w.GroupId == piDet.GroupId);
        //    long newCounter = piDet.Counter != null ? (piDet.Counter.Value + 1) : 1;
        //    if (order != null && order.OrderDetail != null && order.OrderDetail.FirstOrDefault() != null
        //        && order.OrderDetail.FirstOrDefault().OrderDetailInvoices != null && order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault() != null
        //        && order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().Counter != null)
        //    {
        //        newCounter = order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().Counter.Value;  //PARE COUNTER APO TON CLIENT
        //    }
        //    piDet.Counter = newCounter;
        //    foreach (var i in piDetGroup)
        //    {
        //        i.Counter = newCounter;
        //    }

        //    if (order.Day == null)
        //        order.Day = DateTime.Now;

        //    order.OrderDetail = new Economic().EpimerismosEkptwshs(order.Discount, order.TotalBeforeDiscount, order.OrderDetail);
        //    //Table Status
        //    if (order.OrderDetail.FirstOrDefault().TableId != null)
        //    {
        //        var table = db.Table.Find(order.OrderDetail.FirstOrDefault().TableId);
        //        if (table != null)
        //        {
        //            table.Status = 1;//GEMATO
        //        }
        //    }
        //    foreach (var item in order.OrderDetail)
        //    {
        //        item.OrderDetailInvoices.FirstOrDefault().Counter = piDet.Counter;
        //        item.StatusTS = DateTime.Now;
        //        //
        //        OrderDetailVatAnal anal = new OrderDetailVatAnal();
        //        PricelistDetail prdet = db.PricelistDetail.Find(item.PriceListDetailId);
        //        Vat vat = db.PricelistDetail.Include("Vat").Where(w => w.Id == item.PriceListDetailId).FirstOrDefault().Vat;
        //        item.Vat = vat;
        //        Tax tax = db.PricelistDetail.Include("Tax").Where(w => w.Id == item.PriceListDetailId).FirstOrDefault().Tax;
        //        item.Tax = tax;
        //        decimal tempprice = item.Price != null ? item.Price.Value : 0;//prdet.Price != null ? prdet.Price.Value : 0;
        //        tempprice = item.Qty != null && item.Qty > 0 ? (decimal)(item.Qty.Value) * tempprice : tempprice;
        //        tempprice = item.Discount != null ? tempprice - (decimal)item.Discount.Value : tempprice;
        //        var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, tempprice) : tempprice;
        //        var tempgross = tempprice;
        //        var tempvat = (decimal)(tempprice - tempnetbyvat);

        //        var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
        //        var temptax = (decimal)(tempnetbyvat - tempnetbytax);

        //        anal.Gross = tempgross;
        //        anal.Net = (decimal)(tempprice - tempvat - temptax);
        //        anal.TaxAmount = temptax;
        //        anal.VatAmount = tempvat;
        //        anal.VatRate = vat != null ? vat.Percentage : 0;
        //        anal.VatId = vat != null ? (long?)vat.Id : null;
        //        anal.TaxId = tax != null ? (long?)tax.Id : null;
        //        item.OrderDetailVatAnal.Add(anal);
        //        //
        //        foreach (var odi in item.OrderDetailIgredients)
        //        {
        //            OrderDetailIgredientVatAnal analIngr = new OrderDetailIgredientVatAnal();
        //            //PricelistDetail prdet2 = db.PricelistDetail.Find(odi.PriceListDetailId);
        //            Vat vat2 = db.PricelistDetail.Include("Vat").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault().Vat;
        //            Tax tax2 = db.PricelistDetail.Include("Tax").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault().Tax;
        //            decimal tempprice2 = odi.Price != null ? odi.Price.Value : 0;//prdet2.Price != null ? prdet2.Price.Value : 0;
        //            tempprice2 = odi.Discount != null ? tempprice2 - (decimal)odi.Discount.Value : tempprice2;
        //            //tempprice2 = odi.Qty != null && odi.Qty > 0 ? (decimal)(odi.Qty.Value) * tempprice2 : tempprice2; Pairnw etoimh thn timh apo to pos
        //            var tempnetbyvat2 = vat2 != null && vat2.Percentage != null ? DeVat(vat2.Percentage.Value, tempprice2) : tempprice2;
        //            var tempvat2 = (decimal)(tempprice2 - tempnetbyvat2);

        //            var tempnetbytax2 = tax2 != null && tax2.Percentage != null ? DeVat(tax2.Percentage.Value, tempnetbyvat2) : tempnetbyvat2;
        //            var temptax2 = (decimal)(tempnetbyvat2 - tempnetbytax2);
        //            var tempgross2 = tempprice2;
        //            analIngr.Gross = tempgross2;
        //            analIngr.Net = (decimal)(tempprice2 - tempvat2 - temptax2);
        //            analIngr.TaxAmount = temptax2;
        //            analIngr.VatAmount = tempvat2;
        //            analIngr.VatRate = vat2 != null ? vat2.Percentage : 0;
        //            analIngr.VatId = vat2 != null ? (long?)vat2.Id : null;
        //            analIngr.TaxId = tax2 != null ? (long?)tax2.Id : null;
        //            odi.OrderDetailIgredientVatAnal.Add(analIngr);
        //        }
        //    }


        //    foreach (Transactions t in order.Transactions.ToList())
        //    {
        //        Transactions tr = new Economic().SetEconomicNumbers(t, order, db, order.OrderDetail.Select(s => s.Id).ToList());
        //        t.Gross = tr.Gross;
        //        t.Net = tr.Net;
        //        t.Tax = tr.Tax;
        //        t.Vat = tr.Vat;
        //        t.Day = order.Day;

        //        t.OrderDetail = new List<OrderDetail>();
        //        t.OrderDetail = order.OrderDetail;
        //    }
        //    order.OrderStatus.FirstOrDefault().TimeChanged = DateTime.Now;
        //    order.Staff = null;
        //    if (ModelState.IsValid)
        //    {
        //        db.Order.Add(order);
        //        db.SaveChanges();

        //        //RETURN ORDER ONLY WITH REQUIRED PROPERTIES
        //        Order ordToReturn = new Order();
        //        ordToReturn.OrderNo = order.OrderNo;
        //        ordToReturn.Id = order.Id;
        //        ordToReturn.PosInfoDetailId = piDet.Id;
        //        ordToReturn.ReceiptNo = (int?)piDet.Counter;
        //        foreach (var o in order.OrderDetail)
        //        {
        //            OrderDetail ordDetToReturn = new OrderDetail();
        //            ordDetToReturn.OrderId = order.Id;
        //            ordDetToReturn.Id = o.Id;
        //            ordDetToReturn.AA = o.AA;
        //            ordDetToReturn.ProductId = o.ProductId;
        //            ordDetToReturn.Guid = o.Guid;
        //            ordToReturn.OrderDetail.Add(ordDetToReturn);
        //            if (order.Transactions.Count > 0)
        //            {
        //                o.TransactionId = order.Transactions.FirstOrDefault().Id;

        //            }
        //        }
        //        var hotel = db.HotelInfo.FirstOrDefault();
        //        //   List<PmsTranferModel> toSend = new List<PmsTranferModel>();
        //        //   List<TransferObject> tranfers = new List<TransferObject>();
        //        if (order.Transactions.Count > 0 && hotel != null)
        //        {
        //            var trans = order.Transactions.ToList().FirstOrDefault().AccountId;
        //            var account = db.Accounts.Where(f => trans == f.Id).FirstOrDefault();
        //            if (account != null && account.SendsTransfer == true)
        //            {
        //                var query = (from f in order.OrderDetail
        //                             join st in db.SalesType on f.SalesTypeId equals st.Id
        //                             join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
        //                             from ls in loj.DefaultIfEmpty()
        //                             select new
        //                             {
        //                                 Id = f.Id,
        //                                 SalesTypeId = st.Id,
        //                                 Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (double)(((double)f.TotalAfterDiscount) + (double)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (double)f.TotalAfterDiscount,
        //                                 PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
        //                                 PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
        //                                 CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId

        //                             }).Distinct().GroupBy(g => g.PmsDepartmentId).Select(s => new
        //                             {
        //                                 PmsDepartmentId = s.Key,
        //                                 PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
        //                                 Total = s.Sum(sm => sm.Total),
        //                                 CustomerId = s.FirstOrDefault().CustomerId
        //                             });

        //                var IsCreditCard = false;
        //                var roomOfCC = "";
        //                if (account.Type == 4)
        //                {
        //                    IsCreditCard = true;
        //                    roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == pi.Id).FirstOrDefault() != null ?
        //                        db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == pi.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
        //                }


        //                int counter = 0;
        //                List<TransferObject> objTosendList = new List<TransferObject>();

        //                string storeid = HttpContext.Current.Request.Params["storeid"];
        //                foreach (var g in query)
        //                {
        //                    TransferToPms tpms = new TransferToPms();

        //                    tpms.Description = "Rec: " + newCounter + ", Pos: " + pi.Id + ", " + pi.Description;
        //                    tpms.PmsDepartmentId = g.PmsDepartmentId;
        //                    tpms.PosInfoDetailId = piDet.Id;
        //                    tpms.ProfileId = !IsCreditCard ? g.CustomerId : null;
        //                    tpms.ProfileName = !IsCreditCard ? order.CustomerName : account.Description;
        //                    tpms.ReceiptNo = newCounter.ToString();
        //                    tpms.RegNo = !IsCreditCard ? order.RegNo : "0";
        //                    tpms.RoomDescription = !IsCreditCard ? order.RoomDescription : roomOfCC;
        //                    tpms.RoomId = !IsCreditCard ? order.RoomId : "";
        //                    tpms.SendToPMS = true;
        //                    tpms.TransactionId = order.Transactions.FirstOrDefault().Id;
        //                    tpms.TransferType = 0;//Xrewstiko
        //                    tpms.Total = (decimal)g.Total;
        //                    tpms.SendToPmsTS = DateTime.Now;
        //                    var identifier = Guid.NewGuid();
        //                    tpms.TransferIdentifier = identifier;
        //                    tpms.PmsDepartmentDescription = g.PmsDepDescription;
        //                    db.TransferToPms.Add(tpms);
        //                    TransferObject to = new TransferObject();
        //                    to.HotelId = (int)hotel.Id;
        //                    to.amount = (decimal)tpms.Total;
        //                    int PmsDepartmentId = 0;
        //                    var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
        //                    //
        //                    to.TransferIdentifier = tpms.TransferIdentifier;
        //                    //
        //                    to.departmentId = PmsDepartmentId;
        //                    to.description = tpms.Description;
        //                    to.profileName = tpms.ProfileName;
        //                    int resid = 0;
        //                    var toint = int.TryParse(tpms.RegNo, out resid);
        //                    to.resId = resid;
        //                    to.HotelUri = hotel.HotelUri;
        //                    to.TransferIdentifier = identifier;
        //                    to.pmsDepartmentDescription = g.PmsDepDescription;
        //                    if (IsCreditCard)
        //                    {
        //                        to.RoomName = roomOfCC;
        //                    }
        //                    //toSend.Add(new PmsTranferModel()
        //                    //{
        //                    //    amount =to.amount, 
        //                    //    departmentId = to.departmentId ,
        //                    //    description =to.description,
        //                    //    profileName = to.profileName,
        //                    //    resId = to.resId
        //                    //});
        //                    //tranfers.Add(to);

        //                    string res = "";



        //                    // 1
        //                    //  toSend.Add(to);
        //                    //   for (int i = 0; i < 10; i++)

        //                    counter++;
        //                    if (to.amount != 0)
        //                        objTosendList.Add(to);

        //                    // log That there Is no pmsDepartmentId

        //                }




        //                db.SaveChanges();
        //                foreach (var to in objTosendList)
        //                {
        //                    SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
        //                    sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
        //                }
        //                //  var sa = JsonConvert.SerializeObject(toSend);

        //            }
        //        }
        //        // db.SaveChanges();


        //        foreach (var o in order.OrderStatus)
        //        {
        //            OrderStatus ordStaToReturn = new OrderStatus();
        //            ordStaToReturn.Id = o.Id;
        //            ordStaToReturn.OrderId = order.Id;
        //            ordStaToReturn.StaffId = o.StaffId;
        //            ordStaToReturn.Status = o.Status;
        //            ordStaToReturn.TimeChanged = o.TimeChanged;
        //            ordToReturn.OrderStatus.Add(ordStaToReturn);
        //        }
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, ordToReturn);
        //        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = order.Id }));
        //        return response;
        //    }
        //    else
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //}
        #endregion

        #region Post Order With Invoices
        //public HttpResponseMessage PostOrder(Order order)
        //{

        //    PosInfo pi = db.PosInfo.Include("PosInfoDetail").Where(w => w.Id == order.PosId).FirstOrDefault();
        //    pi.ReceiptCount += 1;
        //    order.OrderNo = pi.ReceiptCount;

        //    PosInfoDetail piDet = pi.PosInfoDetail.Where(w => w.Id == order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().PosInfoDetailId).FirstOrDefault();

        //    var piDetGroup = pi.PosInfoDetail.Where(w => w.GroupId != null && w.GroupId == piDet.GroupId);
        //    long newCounter = piDet.Counter != null ? (piDet.Counter.Value + 1) : 1;
        //    if (order != null && order.OrderDetail != null && order.OrderDetail.FirstOrDefault() != null
        //        && order.OrderDetail.FirstOrDefault().OrderDetailInvoices != null && order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault() != null
        //        && order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().Counter != null)
        //    {
        //        newCounter = order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().Counter.Value;  //PARE COUNTER APO TON CLIENT
        //    }
        //    piDet.Counter = newCounter;
        //    foreach (var i in piDetGroup)
        //    {
        //        i.Counter = newCounter;
        //    }

        //    if (order.Day == null)
        //        order.Day = DateTime.Now;


        //    Invoices invoice = new Invoices();

        //    order.OrderDetail = new Economic().EpimerismosEkptwshs(order.Discount, order.TotalBeforeDiscount, order.OrderDetail);
        //    //Table Status
        //    if (order.OrderDetail.FirstOrDefault().TableId != null)
        //    {
        //        var table = db.Table.Find(order.OrderDetail.FirstOrDefault().TableId);
        //        if (table != null)
        //        {
        //            table.Status = 1;//GEMATO
        //        }
        //    }
        //    foreach (var item in order.OrderDetail)
        //    {
        //        foreach (var inv in item.OrderDetailInvoices)
        //        {
        //            invoice.OrderDetailInvoices.Add(inv);
        //        }
        //        item.OrderDetailInvoices.FirstOrDefault().Counter = piDet.Counter;
        //        item.StatusTS = DateTime.Now;
        //        //
        //        OrderDetailVatAnal anal = new OrderDetailVatAnal();
        //        PricelistDetail prdet = db.PricelistDetail.Find(item.PriceListDetailId);
        //        Vat vat = db.PricelistDetail.Include("Vat").Where(w => w.Id == item.PriceListDetailId).FirstOrDefault().Vat;
        //        item.Vat = vat;
        //        Tax tax = db.PricelistDetail.Include("Tax").Where(w => w.Id == item.PriceListDetailId).FirstOrDefault().Tax;
        //        item.Tax = tax;
        //        decimal tempprice = item.Price != null ? item.Price.Value : 0;//prdet.Price != null ? prdet.Price.Value : 0;
        //        tempprice = item.Qty != null && item.Qty > 0 ? (decimal)(item.Qty.Value) * tempprice : tempprice;
        //        tempprice = item.Discount != null ? tempprice - (decimal)item.Discount.Value : tempprice;
        //        var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, tempprice) : tempprice;
        //        var tempgross = tempprice;
        //        var tempvat = (decimal)(tempprice - tempnetbyvat);

        //        var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
        //        var temptax = (decimal)(tempnetbyvat - tempnetbytax);

        //        anal.Gross = tempgross;
        //        anal.Net = (decimal)(tempprice - tempvat - temptax);
        //        anal.TaxAmount = temptax;
        //        anal.VatAmount = tempvat;
        //        anal.VatRate = vat != null ? vat.Percentage : 0;
        //        anal.VatId = vat != null ? (long?)vat.Id : null;
        //        anal.TaxId = tax != null ? (long?)tax.Id : null;
        //        item.OrderDetailVatAnal.Add(anal);
        //        //
        //        foreach (var odi in item.OrderDetailIgredients)
        //        {
        //            OrderDetailIgredientVatAnal analIngr = new OrderDetailIgredientVatAnal();
        //            //PricelistDetail prdet2 = db.PricelistDetail.Find(odi.PriceListDetailId);
        //            Vat vat2 = db.PricelistDetail.Include("Vat").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault().Vat;
        //            Tax tax2 = db.PricelistDetail.Include("Tax").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault().Tax;
        //            decimal tempprice2 = odi.Price != null ? odi.Price.Value : 0;//prdet2.Price != null ? prdet2.Price.Value : 0;
        //            tempprice2 = odi.Discount != null ? tempprice2 - (decimal)odi.Discount.Value : tempprice2;
        //            //tempprice2 = odi.Qty != null && odi.Qty > 0 ? (decimal)(odi.Qty.Value) * tempprice2 : tempprice2; Pairnw etoimh thn timh apo to pos
        //            var tempnetbyvat2 = vat2 != null && vat2.Percentage != null ? DeVat(vat2.Percentage.Value, tempprice2) : tempprice2;
        //            var tempvat2 = (decimal)(tempprice2 - tempnetbyvat2);

        //            var tempnetbytax2 = tax2 != null && tax2.Percentage != null ? DeVat(tax2.Percentage.Value, tempnetbyvat2) : tempnetbyvat2;
        //            var temptax2 = (decimal)(tempnetbyvat2 - tempnetbytax2);
        //            var tempgross2 = tempprice2;
        //            analIngr.Gross = tempgross2;
        //            analIngr.Net = (decimal)(tempprice2 - tempvat2 - temptax2);
        //            analIngr.TaxAmount = temptax2;
        //            analIngr.VatAmount = tempvat2;
        //            analIngr.VatRate = vat2 != null ? vat2.Percentage : 0;
        //            analIngr.VatId = vat2 != null ? (long?)vat2.Id : null;
        //            analIngr.TaxId = tax2 != null ? (long?)tax2.Id : null;
        //            odi.OrderDetailIgredientVatAnal.Add(analIngr);
        //        }
        //    }

        //    invoice.ClientPosId = order.ClientPosId;
        //    invoice.Counter = (int)newCounter;
        //    invoice.Cover = order.OrderDetail.FirstOrDefault().Couver;
        //    invoice.Day = order.Day;
        //    invoice.Description = piDet.Description;
        //    invoice.Discount = order.OrderDetail.Sum(s => s.Discount) + order.OrderDetail.Sum(s => s.OrderDetailIgredients.Sum(ss => ss.Discount));
        //    invoice.GuestId = order.OrderDetail.FirstOrDefault().GuestId;
        //    invoice.InvoiceTypeId = piDet.InvoicesTypeId;
        //    invoice.Net = order.OrderDetail.Sum(s => s.OrderDetailVatAnal.Sum(ss => ss.Net) + s.OrderDetailIgredients.Sum(ss => ss.OrderDetailIgredientVatAnal.Sum(sss => sss.Net)));
        //    invoice.PdaModuleId = order.PdaModuleId;
        //    invoice.PosInfoDetailId = piDet.Id;
        //    invoice.PosInfoId = pi.Id;
        //    invoice.StaffId = order.StaffId;
        //    invoice.TableId = order.OrderDetail.FirstOrDefault().TableId;
        //    invoice.Tax = order.OrderDetail.Sum(s => s.OrderDetailVatAnal.Sum(ss => ss.TaxAmount) + s.OrderDetailIgredients.Sum(ss => ss.OrderDetailIgredientVatAnal.Sum(sss => sss.TaxAmount)));
        //    invoice.Total = order.OrderDetail.Sum(s => s.OrderDetailVatAnal.Sum(ss => ss.Gross) + s.OrderDetailIgredients.Sum(ss => ss.OrderDetailIgredientVatAnal.Sum(sss => sss.Gross)));
        //    invoice.Vat = order.OrderDetail.Sum(s => s.OrderDetailVatAnal.Sum(ss => ss.VatAmount) + s.OrderDetailIgredients.Sum(ss => ss.OrderDetailIgredientVatAnal.Sum(sss => sss.VatAmount)));


        //    foreach (Transactions t in order.Transactions.ToList())
        //    {
        //        Transactions tr = new Economic().SetEconomicNumbers(t, order, db, order.OrderDetail.Select(s => s.Id).ToList());
        //        t.Gross = tr.Gross;
        //        t.Net = tr.Net;
        //        t.Tax = tr.Tax;
        //        t.Vat = tr.Vat;
        //        t.Day = order.Day;

        //        t.OrderDetail = new List<OrderDetail>();
        //        t.OrderDetail = order.OrderDetail;
        //        invoice.Transactions.Add(t);
        //    }
        //    //foreach (Transactions t in order.Transactions.ToList())
        //    //{
        //    //    Transactions tr = new Economic().SetEconomicNumbers(t, order, db, order.OrderDetail.Select(s => s.Id).ToList());
        //    //    t.Gross = null;
        //    //    t.Net = null;
        //    //    t.Tax = null;
        //    //    t.Vat = null;
        //    //    t.Day = order.Day;

        //    //    //if (transactionsCount == 0)
        //    //    //{
        //    //    //    t.OrderDetail = new List<OrderDetail>();
        //    //    //    t.OrderDetail = order.OrderDetail;
        //    //    //}
        //    //    invoice.Transactions.Add(t);
        //    //    transactionsCount++;
        //    //}
        //    order.OrderStatus.FirstOrDefault().TimeChanged = DateTime.Now;
        //    order.Staff = null;
        //    if (ModelState.IsValid)
        //    {
        //        db.Invoices.Add(invoice);
        //        db.Order.Add(order);
        //        db.SaveChanges();

        //        //RETURN ORDER ONLY WITH REQUIRED PROPERTIES
        //        Order ordToReturn = new Order();
        //        ordToReturn.OrderNo = order.OrderNo;
        //        ordToReturn.Id = order.Id;
        //        ordToReturn.PosInfoDetailId = piDet.Id;
        //        ordToReturn.ReceiptNo = (int?)piDet.Counter;
        //        foreach (var o in order.OrderDetail)
        //        {
        //            OrderDetail ordDetToReturn = new OrderDetail();
        //            ordDetToReturn.OrderId = order.Id;
        //            ordDetToReturn.Id = o.Id;
        //            ordDetToReturn.AA = o.AA;
        //            ordDetToReturn.ProductId = o.ProductId;
        //            ordDetToReturn.Guid = o.Guid;
        //            ordDetToReturn.OrderDetailInvoices = new List<OrderDetailInvoices>();
        //            foreach (var inv in o.OrderDetailInvoices)
        //            {
        //                OrderDetailInvoices oinv = new OrderDetailInvoices();
        //                oinv.InvoicesId = inv.InvoicesId;
        //                ordDetToReturn.OrderDetailInvoices.Add(oinv);
        //            }
        //            ordToReturn.OrderDetail.Add(ordDetToReturn);
        //            if (order.Transactions.Count > 0)
        //            {
        //                o.TransactionId = order.Transactions.FirstOrDefault().Id;

        //            }
        //        }
        //        var hotel = db.HotelInfo.FirstOrDefault();
        //        //   List<PmsTranferModel> toSend = new List<PmsTranferModel>();
        //        //   List<TransferObject> tranfers = new List<TransferObject>();
        //        if (order.Transactions.Count > 0 && hotel != null)
        //        {
        //            var trans = order.Transactions.ToList().FirstOrDefault().AccountId;
        //            var account = db.Accounts.Where(f => trans == f.Id).FirstOrDefault();
        //            if (account != null && account.SendsTransfer == true)
        //            {
        //                var query = (from f in order.OrderDetail
        //                             join st in db.SalesType on f.SalesTypeId equals st.Id
        //                             join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
        //                             from ls in loj.DefaultIfEmpty()
        //                             select new
        //                             {
        //                                 Id = f.Id,
        //                                 SalesTypeId = st.Id,
        //                                 Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (double)(((double)f.TotalAfterDiscount) + (double)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (double)f.TotalAfterDiscount,
        //                                 PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
        //                                 PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
        //                                 CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId,
        //                                 VatPercentage = ls != null ? ls.VatPercentage : null

        //                             }).Distinct().GroupBy(g => g.PmsDepartmentId).Select(s => new
        //                             {
        //                                 PmsDepartmentId = s.Key,
        //                                 PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
        //                                 Total = s.Sum(sm => sm.Total),
        //                                 CustomerId = s.FirstOrDefault().CustomerId,
        //                                 VatPercentage = s.FirstOrDefault().VatPercentage
        //                             });

        //                var IsCreditCard = false;
        //                var roomOfCC = "";
        //                if (account.Type == 4)
        //                {
        //                    IsCreditCard = true;
        //                    roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == pi.Id).FirstOrDefault() != null ?
        //                        db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == pi.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
        //                }


        //                int counter = 0;
        //                List<TransferObject> objTosendList = new List<TransferObject>();

        //                string storeid = HttpContext.Current.Request.Params["storeid"];
        //                foreach (var g in query)
        //                {
        //                    TransferToPms tpms = new TransferToPms();

        //                    tpms.Description = "Rec: " + newCounter + ", Pos: " + pi.Id + ", " + pi.Description;
        //                    tpms.PmsDepartmentId = g.PmsDepartmentId;
        //                    tpms.PosInfoDetailId = piDet.Id;
        //                    tpms.ProfileId = !IsCreditCard ? g.CustomerId : null;
        //                    tpms.ProfileName = !IsCreditCard ? order.CustomerName : account.Description;
        //                    tpms.ReceiptNo = newCounter.ToString();
        //                    tpms.RegNo = !IsCreditCard ? order.RegNo : "0";
        //                    tpms.RoomDescription = !IsCreditCard ? order.RoomDescription : roomOfCC;
        //                    tpms.RoomId = !IsCreditCard ? order.RoomId : "";
        //                    tpms.SendToPMS = true;
        //                    tpms.TransactionId = order.Transactions.FirstOrDefault().Id;
        //                    tpms.TransferType = 0;//Xrewstiko
        //                    tpms.Total = (decimal)g.Total;
        //                    tpms.SendToPmsTS = DateTime.Now;
        //                    var identifier = Guid.NewGuid();
        //                    tpms.TransferIdentifier = identifier;
        //                    tpms.PmsDepartmentDescription = g.PmsDepDescription;
        //                    db.TransferToPms.Add(tpms);
        //                    TransferObject to = new TransferObject();
        //                    to.HotelId = (int)hotel.Id;
        //                    to.amount = (decimal)tpms.Total;
        //                    int PmsDepartmentId = 0;
        //                    var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
        //                    //
        //                    to.TransferIdentifier = tpms.TransferIdentifier;
        //                    //
        //                    to.departmentId = PmsDepartmentId;
        //                    to.description = tpms.Description;
        //                    to.profileName = tpms.ProfileName;
        //                    int resid = 0;
        //                    var toint = int.TryParse(tpms.RegNo, out resid);
        //                    to.resId = resid;
        //                    to.HotelUri = hotel.HotelUri;
        //                    to.TransferIdentifier = identifier;
        //                    to.pmsDepartmentDescription = g.PmsDepDescription;
        //                    to.RoomName = tpms.RoomDescription;
        //                    to.VatPercentage = g.VatPercentage;
        //                    if (IsCreditCard)
        //                    {
        //                        to.RoomName = roomOfCC;
        //                    }
        //                    //toSend.Add(new PmsTranferModel()
        //                    //{
        //                    //    amount =to.amount, 
        //                    //    departmentId = to.departmentId ,
        //                    //    description =to.description,
        //                    //    profileName = to.profileName,
        //                    //    resId = to.resId
        //                    //});
        //                    //tranfers.Add(to);

        //                    string res = "";



        //                    // 1
        //                    //  toSend.Add(to);
        //                    //   for (int i = 0; i < 10; i++)

        //                    counter++;
        //                    if (to.amount != 0)
        //                        objTosendList.Add(to);

        //                    // log That there Is no pmsDepartmentId

        //                }




        //                db.SaveChanges();
        //                foreach (var to in objTosendList)
        //                {
        //                    SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
        //                    sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
        //                }
        //                //  var sa = JsonConvert.SerializeObject(toSend);

        //            }
        //        }
        //        // db.SaveChanges();


        //        foreach (var o in order.OrderStatus)
        //        {
        //            OrderStatus ordStaToReturn = new OrderStatus();
        //            ordStaToReturn.Id = o.Id;
        //            ordStaToReturn.OrderId = order.Id;
        //            ordStaToReturn.StaffId = o.StaffId;
        //            ordStaToReturn.Status = o.Status;
        //            ordStaToReturn.TimeChanged = o.TimeChanged;
        //            ordToReturn.OrderStatus.Add(ordStaToReturn);
        //        }
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, ordToReturn);
        //        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = order.Id }));
        //        return response;
        //    }
        //    else
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //}
        #endregion

        #region Post Order With Invoices and split accounts
        public HttpResponseMessage PostOrder(Order order)
        {
            //CHECK IF ALLREADY EXISTS
            var curGuids = order.OrderDetail.Select(s => s.Guid);
            var hasSameGuids = db.OrderDetail.Where(w => w.Order.EndOfDayId == null && w.Order.PosId == order.PosId).Where(w => curGuids.Contains(w.Guid));
            if (hasSameGuids.Count() > 0)
            {
                order.Id = 0;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, order);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = 0 }));
                return response;
            }

            //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            PosInfo pi = db.PosInfo.Include("PosInfoDetail").Where(w => w.Id == order.PosId).FirstOrDefault();
            pi.ReceiptCount += 1;
            order.OrderNo = pi.ReceiptCount;

            PosInfoDetail piDet = pi.PosInfoDetail.Where(w => w.Id == order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().PosInfoDetailId).FirstOrDefault();

            var piDetGroup = pi.PosInfoDetail.Where(w => w.GroupId != null && w.GroupId == piDet.GroupId);
            long newCounter = piDet.Counter != null ? (piDet.Counter.Value + 1) : 1;
            Invoices invoice = new Invoices();

            if (order != null && order.OrderDetail != null && order.OrderDetail.FirstOrDefault() != null
                && order.OrderDetail.FirstOrDefault().OrderDetailInvoices != null && order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault() != null
                && order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().Counter != null)
            {
                newCounter = order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().Counter.Value;  //PARE COUNTER APO TON CLIENT
                if (order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().Invoices != null)
                {
                    invoice = order.OrderDetail.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().Invoices;
                }
            }
            piDet.Counter = newCounter;
            foreach (var i in piDetGroup)
            {
                i.Counter = newCounter;
            }

            if (order.Day == null)
                order.Day = DateTime.Now;


            //invoice.DiscountRemark = order.DiscountRemark;
            order.OrderDetail = new Economic().EpimerismosEkptwshs(order.Discount, order.TotalBeforeDiscount, order.OrderDetail);
            //Table Status
            if (order.OrderDetail.FirstOrDefault().TableId != null)
            {
                var table = db.Table.Find(order.OrderDetail.FirstOrDefault().TableId);
                if (table != null)
                {
                    table.Status = 1;//GEMATO
                }
            }
            foreach (var item in order.OrderDetail)
            {
                foreach (var inv in item.OrderDetailInvoices)
                {
                    inv.Invoices = null;
                    invoice.OrderDetailInvoices.Add(inv);
                }
                item.OrderDetailInvoices.FirstOrDefault().Counter = piDet.Counter;
                item.StatusTS = DateTime.Now;
                //
                OrderDetailVatAnal anal = new OrderDetailVatAnal();
                PricelistDetail prdet = db.PricelistDetail.Find(item.PriceListDetailId);
                Vat vat = db.PricelistDetail.Include("Vat").Where(w => w.Id == item.PriceListDetailId).FirstOrDefault().Vat;
                item.Vat = vat;
                Tax tax = db.PricelistDetail.Include("Tax").Where(w => w.Id == item.PriceListDetailId).FirstOrDefault().Tax;
                item.Tax = tax;
                decimal tempprice = item.TotalAfterDiscount ?? 0;// item.Price != null ? item.Price.Value : 0;//prdet.Price != null ? prdet.Price.Value : 0;
              //  tempprice = item.Qty != null && item.Qty > 0 ? (decimal)(item.Qty.Value) * tempprice : tempprice;
               // tempprice = item.Discount != null ? tempprice - (decimal)item.Discount.Value : tempprice;
                var tempnetbyvat = vat != null && vat.Percentage != null ? DeVat(vat.Percentage.Value, tempprice) : tempprice;
                var tempgross = tempprice;
                var tempvat = (decimal)(tempprice - tempnetbyvat);

                var tempnetbytax = tax != null && tax.Percentage != null ? DeVat(tax.Percentage.Value, tempnetbyvat) : tempnetbyvat;
                var temptax = (decimal)(tempnetbyvat - tempnetbytax);

                anal.Gross = tempgross;
                anal.Net = (decimal)(tempprice - tempvat - temptax);
                anal.TaxAmount = temptax;
                anal.VatAmount = tempvat;
                anal.VatRate = vat != null ? vat.Percentage : 0;
                anal.VatId = vat != null ? (long?)vat.Id : null;
                anal.TaxId = tax != null ? (long?)tax.Id : null;
                item.OrderDetailVatAnal.Add(anal);
                //
                foreach (var odi in item.OrderDetailIgredients)
                {
                    OrderDetailIgredientVatAnal analIngr = new OrderDetailIgredientVatAnal();
                    //PricelistDetail prdet2 = db.PricelistDetail.Find(odi.PriceListDetailId);
                    Vat vat2 = db.PricelistDetail.Include("Vat").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault().Vat;
                    Tax tax2 = db.PricelistDetail.Include("Tax").Where(w => w.Id == odi.PriceListDetailId).FirstOrDefault().Tax;
                    decimal tempprice2 = odi.TotalAfterDiscount ?? 0;
                    //odi.Price != null ? odi.Price.Value : 0;//prdet2.Price != null ? prdet2.Price.Value : 0;
                   // tempprice2 = odi.Discount != null ? tempprice2 - (decimal)odi.Discount.Value : tempprice2;
                    //tempprice2 = odi.Qty != null && odi.Qty > 0 ? (decimal)(odi.Qty.Value) * tempprice2 : tempprice2; Pairnw etoimh thn timh apo to pos
                    var tempnetbyvat2 = vat2 != null && vat2.Percentage != null ? DeVat(vat2.Percentage.Value, tempprice2) : tempprice2;
                    var tempvat2 = (decimal)(tempprice2 - tempnetbyvat2);

                    var tempnetbytax2 = tax2 != null && tax2.Percentage != null ? DeVat(tax2.Percentage.Value, tempnetbyvat2) : tempnetbyvat2;
                    var temptax2 = (decimal)(tempnetbyvat2 - tempnetbytax2);
                    var tempgross2 = tempprice2;
                    analIngr.Gross = tempgross2;
                    analIngr.Net = (decimal)(tempprice2 - tempvat2 - temptax2);
                    analIngr.TaxAmount = temptax2;
                    analIngr.VatAmount = tempvat2;
                    analIngr.VatRate = vat2 != null ? vat2.Percentage : 0;
                    analIngr.VatId = vat2 != null ? (long?)vat2.Id : null;
                    analIngr.TaxId = tax2 != null ? (long?)tax2.Id : null;
                    odi.OrderDetailIgredientVatAnal.Add(analIngr);
                }
            }

            invoice.ClientPosId = order.ClientPosId;
            invoice.Counter = (int)newCounter;
            invoice.Cover = order.OrderDetail.FirstOrDefault().Couver;
            invoice.Day = order.Day;
            invoice.Description = piDet.Description;
            invoice.Discount = order.OrderDetail.Sum(s => s.Discount) + order.OrderDetail.Sum(s => s.OrderDetailIgredients.Sum(ss => ss.Discount));
            invoice.GuestId = order.OrderDetail.FirstOrDefault().GuestId;
            invoice.InvoiceTypeId = piDet.InvoicesTypeId;
            invoice.Net = order.OrderDetail.Sum(s => s.OrderDetailVatAnal.Sum(ss => ss.Net) + s.OrderDetailIgredients.Sum(ss => ss.OrderDetailIgredientVatAnal.Sum(sss => sss.Net)));
            invoice.PdaModuleId = order.PdaModuleId;
            invoice.PosInfoDetailId = piDet.Id;
            invoice.PosInfoId = pi.Id;
            invoice.StaffId = order.StaffId;
            invoice.TableId = order.OrderDetail.FirstOrDefault().TableId;
            invoice.Tax = order.OrderDetail.Sum(s => s.OrderDetailVatAnal.Sum(ss => ss.TaxAmount) + s.OrderDetailIgredients.Sum(ss => ss.OrderDetailIgredientVatAnal.Sum(sss => sss.TaxAmount)));
            invoice.Total = order.OrderDetail.Sum(s => s.OrderDetailVatAnal.Sum(ss => ss.Gross) + s.OrderDetailIgredients.Sum(ss => ss.OrderDetailIgredientVatAnal.Sum(sss => sss.Gross)));
            invoice.Vat = order.OrderDetail.Sum(s => s.OrderDetailVatAnal.Sum(ss => ss.VatAmount) + s.OrderDetailIgredients.Sum(ss => ss.OrderDetailIgredientVatAnal.Sum(sss => sss.VatAmount)));
            invoice.DiscountRemark = order.DiscountRemark;
            //added for the IsPrintedStatus of Waterpark barcode module invoice 
            invoice.IsPrinted = order.OrderDetail.SelectMany(s => s.OrderDetailInvoices).OrderByDescending(o => o.Id).FirstOrDefault().IsPrinted;


            try {
                InvoiceShippingDetails invoiceShippingDetails = new InvoiceShippingDetails();

                invoiceShippingDetails.ShippingAddressId = order.ShippingAddressId;
                invoiceShippingDetails.ShippingAddress = order.ShippingAddress;
                invoiceShippingDetails.ShippingCity = order.ShippingCity;
                invoiceShippingDetails.ShippingZipCode = order.ShippingZipCode;
                invoiceShippingDetails.BillingAddressId = order.BillingAddressId;
                invoiceShippingDetails.BillingAddress = order.BillingAddress;
                invoiceShippingDetails.BillingCity = order.BillingCity;
                invoiceShippingDetails.BillingZipCode = order.BillingZipCode;
                invoiceShippingDetails.Floor = order.Floor;
                invoiceShippingDetails.CustomerRemarks = order.CustomerRemarks;
                invoiceShippingDetails.StoreRemarks = order.StoreRemarks;
                invoiceShippingDetails.Longtitude = order.Longtitude;
                invoiceShippingDetails.Latitude = order.Latitude;
                invoiceShippingDetails.CustomerName = order.CustomerName;
                invoiceShippingDetails.CustomerID = order.CustomerID;
                invoiceShippingDetails.Phone = order.Phone;

                invoice.InvoiceShippingDetails.Add(invoiceShippingDetails);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                //      Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            List<AccountsObj> accountsList = new List<AccountsObj>();

            foreach (Transactions t in order.Transactions.ToList())
            {
                //Transactions tr = new Economic().SetEconomicNumbers(t, order, db, order.OrderDetail.Select(s => s.Id).ToList());
                t.Gross = null;
                t.Net = null;
                t.Tax = null;
                t.Vat = null;
                t.Day = order.Day;
                invoice.Transactions.Add(t);

                AccountsObj acc = new AccountsObj();
                acc.Account = db.Accounts.Find(t.AccountId);
                acc.AccountId = t.AccountId != null ? t.AccountId.Value : 0;
                acc.Amount = t.Amount != null ? t.Amount.Value : 0;
                if (t.Invoice_Guests_Trans != null && t.Invoice_Guests_Trans.Count > 0)
                {
                    acc.GuestId = t.Invoice_Guests_Trans.FirstOrDefault().GuestId;
                    invoice.Invoice_Guests_Trans.Add(t.Invoice_Guests_Trans.FirstOrDefault());
                }
                else
                {
                    acc.GuestId = invoice.GuestId;
                }
                accountsList.Add(acc);
            }
            order.OrderStatus.FirstOrDefault().TimeChanged = DateTime.Now;
            order.Staff = null;
            if (ModelState.IsValid)
            {
                db.Invoices.Add(invoice);
                db.Order.Add(order);
                db.SaveChanges();

                //RETURN ORDER ONLY WITH REQUIRED PROPERTIES
                Order ordToReturn = new Order();
                ordToReturn.OrderNo = order.OrderNo;
                ordToReturn.Id = order.Id;
                ordToReturn.PosInfoDetailId = piDet.Id;
                ordToReturn.ReceiptNo = (int?)piDet.Counter;
                foreach (var o in order.OrderDetail)
                {
                    OrderDetail ordDetToReturn = new OrderDetail();
                    ordDetToReturn.OrderId = order.Id;
                    ordDetToReturn.Id = o.Id;
                    ordDetToReturn.AA = o.AA;
                    ordDetToReturn.ProductId = o.ProductId;
                    ordDetToReturn.Guid = o.Guid;
                    ordDetToReturn.OrderDetailInvoices = new List<OrderDetailInvoices>();
                    foreach (var inv in o.OrderDetailInvoices)
                    {
                        OrderDetailInvoices oinv = new OrderDetailInvoices();
                        oinv.InvoicesId = inv.InvoicesId;
                        ordDetToReturn.OrderDetailInvoices.Add(oinv);
                    }
                    ordToReturn.OrderDetail.Add(ordDetToReturn);
                    if (order.Transactions.Count > 0)
                    {
                        o.TransactionId = order.Transactions.FirstOrDefault().Id;

                    }
                }
                var hotel = db.HotelInfo.FirstOrDefault();
                //   List<PmsTranferModel> toSend = new List<PmsTranferModel>();
                //   List<TransferObject> tranfers = new List<TransferObject>();
                if (order.Transactions.Count > 0 && hotel != null)
                {
                    //var trans = order.Transactions.ToList().FirstOrDefault().AccountId;
                    //var account = db.Accounts.Where(f => trans == f.Id).FirstOrDefault();
                    foreach (var ac in accountsList)
                    {
                        var account = ac.Account;
                        if (account != null)//&& account.SendsTransfer == true (tha chekaristei otan einai na stalei)
                        {
                            var deps = order.PosInfo.DepartmentId;
                            var pril = order.OrderDetail.Select(s => s.PricelistDetail.PricelistId).Distinct();
                            var prods = order.OrderDetail.Select(s => s.ProductId);
                            var query = (from f in order.OrderDetail
                                         join st in db.SalesType on f.SalesTypeId equals st.Id
                                         join tm in db.TransferMappings.Where(w => deps == w.PosDepartmentId && pril.Contains(w.PriceListId) && prods.Contains(w.ProductId)).ToList()
                                         on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                         from ls in loj.DefaultIfEmpty()
                                         select new
                                         {
                                             Id = f.Id,
                                             SalesTypeId = st.Id,
                                             Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (decimal)(((decimal)f.TotalAfterDiscount) + (decimal)(f.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))) : (decimal)f.TotalAfterDiscount,
                                             OrderDetail = f,
                                             PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                             PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                             //CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId

                                         }).Distinct().GroupBy(g => g.PmsDepartmentId).Select(s => new
                                         {
                                             PmsDepartmentId = s.Key,
                                             PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                             Total = s.Sum(sm => sm.Total),
                                             OrderDetails = s.Select(ss => ss.OrderDetail),
                                             //CustomerId = s.FirstOrDefault().CustomerId
                                         });


                            long guestid = ac.GuestId != null ? ac.GuestId.Value : -1;//tr.OrderDetail.FirstOrDefault().GuestId != null ? tr.OrderDetail.FirstOrDefault().GuestId.Value : -1;



                            Guest curcustomer = db.Guest.Where(w => w.Id == guestid).FirstOrDefault();

                            var IsNotSendingTransfer = account.SendsTransfer == false;
                            var IsCreditCard = false;
                            var roomOfCC = "";
                            if (account.Type == 4 || IsNotSendingTransfer == true)
                            {
                                IsCreditCard = true;
                                roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == pi.Id).FirstOrDefault() != null ?
                                    db.EODAccountToPmsTransfer.Where(w => w.AccountId == account.Id && w.PosInfoId == pi.Id).FirstOrDefault().PmsRoom.ToString() : "-1";
                            }



                            int counter = 0;
                            List<TransferObject> objTosendList = new List<TransferObject>();

                            string storeid = HttpContext.Current.Request.Params["storeid"];

                            //EpimerismosPerDepartment
                            decimal TotalAmounts = invoice.Total.Value;
                            decimal totalDiscount = TotalAmounts - ac.Amount;
                            decimal percentageEpim = TotalAmounts == 0 ? TotalAmounts : 1 - (decimal)(ac.Amount / TotalAmounts);
                            decimal totalEpim = 0;
                            decimal remainingDiscount = totalDiscount;
                            decimal ctr = 1;
                            List<dynamic> query2 = new List<dynamic>();
                            foreach (var f in query)
                            {
                                if (ctr < query.Count())
                                {
                                    decimal subtotal = f.Total;
                                    decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                                    totalEpim += subtotal - percSub;
                                    query2.Add(new
                                    {
                                        PmsDepartmentId = f.PmsDepartmentId,
                                        Total = subtotal - percSub
                                    });
                                    remainingDiscount = remainingDiscount - percSub;
                                }
                                else
                                {
                                    decimal subtotal = f.Total;
                                    query2.Add(new
                                    {
                                        PmsDepartmentId = f.PmsDepartmentId,
                                        Total = subtotal - remainingDiscount
                                    });
                                    totalEpim += subtotal - remainingDiscount;
                                }
                                ctr++;
                            }
                            //
                            var query3 = from q in query
                                         join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                                         select new
                                         {
                                             PmsDepartmentId = q.PmsDepartmentId,
                                             PmsDepDescription = q.PmsDepDescription,
                                             Total = j.Total,
                                             OrderDetails = q.OrderDetails,
                                             //CustomerId = q.CustomerId
                                         };
                            //

                            var departmentDescritpion = db.Department.FirstOrDefault(f => f.Id == pi.DepartmentId);
                            string depstr = departmentDescritpion != null ? departmentDescritpion.Description : pi.Description;

                            foreach (var g in query3)
                            {
                                decimal total = g.Total;//new Economic().EpimerisiAccountTotal(g.OrderDetails, g.Total);
                                TransferToPms tpms = new TransferToPms();

                                tpms.Description = "Rec: " + newCounter + ", Pos: " + pi.Id + ", " + depstr;
                                tpms.PmsDepartmentId = g.PmsDepartmentId;
                                tpms.PosInfoDetailId = piDet.Id;
                                tpms.ProfileId = !IsCreditCard && !IsNotSendingTransfer ? curcustomer.ProfileNo.ToString() : null;//g.CustomerId
                                tpms.ProfileName = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "" : account.Description;
                                tpms.ReceiptNo = newCounter.ToString();
                                tpms.RegNo = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.ReservationId.ToString() : "" : "0";
                                tpms.RoomDescription = !IsCreditCard && !IsNotSendingTransfer ? curcustomer != null ? curcustomer.Room : "" : roomOfCC;
                                tpms.RoomId = !IsCreditCard && !IsNotSendingTransfer ? curcustomer.RoomId.ToString() : "";
                                tpms.SendToPMS = !IsNotSendingTransfer;
                                tpms.TransactionId = order.Transactions.Where(w => w.AccountId == ac.AccountId).FirstOrDefault() != null ? order.Transactions.Where(w => w.AccountId == ac.AccountId).FirstOrDefault().Id : -1;
                                tpms.TransferType = 0;//Xrewstiko
                                tpms.Total = (decimal)g.Total;
                                tpms.SendToPmsTS = DateTime.Now;
                                var identifier = Guid.NewGuid();
                                tpms.TransferIdentifier = identifier;
                                tpms.PmsDepartmentDescription = g.PmsDepDescription;
                                //Set Status Flag (0: Cash, 1: Not Cash)
                                tpms.Status = IsNotSendingTransfer ? (short)0 : (short)1;
                                tpms.PosInfoId = pi.Id;
                                db.TransferToPms.Add(tpms);

                                TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, IsCreditCard, roomOfCC, tpms, identifier);
                                to.pmsDepartmentDescription = g.PmsDepDescription;
                                counter++;
                                if (to.amount != 0 && ac.Account.SendsTransfer == true)
                                    objTosendList.Add(to);

                                // log That there Is no pmsDepartmentId

                            }
                            db.SaveChanges();
                            foreach (var to in objTosendList)
                            {
                                SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                //sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                            }
                        }
                    }
                }
                foreach (var o in order.OrderStatus)
                {
                    OrderStatus ordStaToReturn = new OrderStatus();
                    ordStaToReturn.Id = o.Id;
                    ordStaToReturn.OrderId = order.Id;
                    ordStaToReturn.StaffId = o.StaffId;
                    ordStaToReturn.Status = o.Status;
                    ordStaToReturn.TimeChanged = o.TimeChanged;
                    ordToReturn.OrderStatus.Add(ordStaToReturn);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, ordToReturn);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = order.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        #endregion

        private delegate TranferResultModel SendTransfer(TransferObject tpms, string storeid);


        // get response from pms and update table tranferpms
        private void SendTransferCallback(IAsyncResult result)
        {
            // db = new PosEntities(false);
            SendTransfer del = (SendTransfer)result.AsyncState;

            TranferResultModel res = (TranferResultModel)del.EndInvoke(result);

            Guid storeid;

            if (Guid.TryParse(res.StoreId, out storeid))
            {

                using (var ctx = new PosEntities(false, storeid))
                {
                    var originalTransfer = ctx.TransferToPms.FirstOrDefault(f => f.TransferIdentifier == res.TransferObj.TransferIdentifier);

                    if (originalTransfer != null)
                    {
                        //originalTransfer.SendToPmsTS = DateTime.Now;
                        originalTransfer.ErrorMessage = res.TransferErrorMessage;
                        originalTransfer.PmsResponseId = res.TransferResponseId;
                        //   originalTransfer.PmsDepartmentDescription = res.TransferObj.pmsDepartmentDescription;

                    }

                    ctx.SaveChanges();
                }
            }

        }


        //private void CreatePMSTransfer(TransferToPms tpms)
        //{
        //    string url = "";
        //    string result = "";
        //    using (var w = new WebClient())
        //    {
        //        w.Encoding = System.Text.Encoding.UTF8;
        //        var json_data = string.Empty;
        //        // attempt to download JSON data as a string
        //        try
        //        {
        //            json_data = w.DownloadString(url);
        //        }
        //        catch (Exception) { }
        //        // if string with JSON data is not empty, deserialize it to class and return its instance 
        //        result = json_data;
        //    }
        //    //return result;
        //}

        // DELETE api/Order/5
        public HttpResponseMessage DeleteOrder(long id)
        {
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Order.Remove(order);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, order);
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

        private static decimal DeVat(decimal perc, decimal tempnetbyvat)
        {
            return (decimal)(tempnetbyvat / (decimal)(1 + (decimal)(perc / 100)));
        }
    }
}