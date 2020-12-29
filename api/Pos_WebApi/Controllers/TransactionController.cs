using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class TransactionController : ApiController
    {
      //  private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/Transaction
        public Object GetTransactions(long posid, long? staffid, string foday)
        {
            try
            {

                        using (PosEntities db = new PosEntities(false))
                    {
                        //DateTime day = Convert.ToDateTime(foday);
                        //DateTime dayto = day.AddDays(1);
                        var transactionTypesRaw = (TransactionTypesEnum[])Enum.GetValues(typeof(TransactionTypesEnum));
                        var accounts = db.Accounts.ToList();
                    var TransactionTypes = db.TransactionTypes.ToList();
                        if (staffid != null)
                        {
                            var transactions = db.Transactions.Include("Order").Include("Order.OrderStatus").Where(w => w.EndOfDayId == null && w.StaffId == staffid && w.PosInfoId == posid && (w.IsDeleted ?? false) == false).GroupBy(g => g.TransactionType)
                                .Select(s => new
                                {
                                    Id = s.FirstOrDefault().Id,
                                    Amount = s.Sum(ss => ss.Amount),
                                    Count = s.Count(),
                                    TransactionType = s.Key,
                                    InOut = s.FirstOrDefault().InOut,
                                    Date = s.FirstOrDefault().Day,
                                    Gross = s.Where(ww => ww.Order.OrderStatus.Where(www => www.Status == 5).Count() == 0).Sum(ss => ss.Gross),
                                    Net = s.Where(ww => ww.Order.OrderStatus.Where(www => www.Status == 5).Count() == 0).Sum(ss => ss.Net),
                                    Vat = s.Where(ww => ww.Order.OrderStatus.Where(www => www.Status == 5).Count() == 0).Sum(ss => ss.Vat),
                                    Tax = s.Where(ww => ww.Order.OrderStatus.Where(www => www.Status == 5).Count() == 0).Sum(ss => ss.Tax),
                                    Cancelled = s.FirstOrDefault().Order.OrderStatus.Where(ww => ww.Status == 5).Count() > 0 ? true : false
                                }).ToList();
                            return new { transactions, TransactionTypes, accounts };
                            //return new { transactions, db.TransactionTypes };
                        }
                        else
                        {
                        var transactions = db.Transactions.Include("Order").Include("Order.OrderStatus").Where(w => w.EndOfDayId == null && w.PosInfoId == posid && (w.IsDeleted ?? false) == false).GroupBy(g => g.TransactionType)
                            .Select(s => new
                                {
                                    Id = s.FirstOrDefault().Id,
                                    Amount = s.Sum(ss => ss.Amount),
                                    Count = s.Count(),
                                    TransactionType = s.Key,
                                    InOut = s.FirstOrDefault().InOut,
                                    Date = s.FirstOrDefault().Day,
                                    Gross = s.Where(ww => ww.Order.OrderStatus.Where(www => www.Status == 5).Count() == 0).Sum(ss => ss.Gross),
                                    Net = s.Where(ww => ww.Order.OrderStatus.Where(www => www.Status == 5).Count() == 0).Sum(ss => ss.Net),
                                    Vat = s.Where(ww => ww.Order.OrderStatus.Where(www => www.Status == 5).Count() == 0).Sum(ss => ss.Vat),
                                    Tax = s.Where(ww => ww.Order.OrderStatus.Where(www => www.Status == 5).Count() == 0).Sum(ss => ss.Tax),
                                    Cancelled = s.FirstOrDefault().Order.OrderStatus.Where(ww => ww.Status == 5).Count() > 0 ? true : false
                                }).ToList();
                            return new { transactions, TransactionTypes, accounts };
                            //return new { transactions, db.TransactionTypes };
                        }

                    }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Task.Run(() => GC.Collect());
            }

        }

        // GET api/Transaction/5
        public Object GetTransactions(long posid, string foday, bool byposid)
        {
            try
            {
                    using (PosEntities db = new PosEntities(false))
                {
                    //DateTime fodayDT = Convert.ToDateTime(foday);
                    //DateTime dayto = fodayDT.AddDays(1);
                    var transactions = db.Transactions.Include(i => i.Order).Where(w => w.PosInfoId == posid && w.EndOfDayId == null && (w.IsDeleted ?? false) == false).AsNoTracking().AsEnumerable();
                    if (transactions == null)
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                    }
                    var group = transactions.Where(w => w.StaffId != null).GroupBy(g => g.StaffId).Select(s => new
                    {
                        Id = s.Key,
                        Code = db.Staff.Find(s.Key).Code,
                        FirstName = db.Staff.Find(s.Key).FirstName,
                        ImageUri = db.Staff.Find(s.Key).ImageUri,
                        LastName = db.Staff.Find(s.Key).LastName,
                        Transactions = s.Select(ss => new
                        {
                            Description = ss.Description,
                            Day = ss.Day,
                            Amount = ss.Amount,
                            OrderNo = ss.Order != null ? ss.Order.OrderNo : null
                        }).AsEnumerable()
                    });

                    var flat = transactions;

                    int itemscount = 0;
                    foreach (var i in db.Transactions.Include("Order").Include("Order.OrderDetail").Where(w => w.PosInfoId == posid && w.EndOfDayId == null && (w.IsDeleted ?? false) == false))
                    {
                        if (i.Order != null)
                            itemscount += i.Order.OrderDetail.Count;
                    }

                    return new { group, flat, itemscount };

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Task.Run(() => GC.Collect());
            }

        }

        #region End of day View
        //public Object GetTransactions(long posid, string foday, bool byposid, bool n)
        //{
        //    var orders = db.Order.Include(i => i.OrderDetail).Include(i => i.OrderDetail.Select(s=>s.OrderDetailIgredients))
        //        .Where(w => w.EndOfDayId == null && w.PosId == posid && !w.OrderStatus.Any(a => a.Status == 5)).ToList();
        //    var unpricedOrders = orders.Where(w => w.OrderDetail.Any(a => a.PaidStatus < 1  && a.Status != 5)).Select(s => new
        //    {
        //        Id = s.Id,
        //        OrderNo = s.OrderNo,
        //        StaffId = s.StaffId,
        //        PosId = s.PosId,
        //        OrderTotal = s.Total,
        //        TotalDiscount = (s.Discount != null ? s.Discount : 0) + s.OrderDetail.Where(ww => ww.PaidStatus < 1 && ww.Status != 5).Sum(sm => (sm.Discount != null ? sm.Discount : 0) + sm.OrderDetailIgredients.Sum(sum => (sum.Discount != null ? sum.Discount : 0))),
        //        UnPricedTotal = s.OrderDetail.Where(ww => ww.PaidStatus < 1 && ww.Status != 5).Sum(sm => (decimal)((decimal)sm.TotalAfterDiscount + sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))),
        //        UnPricedCount = s.OrderDetail.Where(ww => ww.PaidStatus < 1 && ww.Status != 5).Sum(sm => sm.Qty),
        //        //PricedTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 1).Sum(sm => sm.Price),
        //        //PricedCount = s.OrderDetail.Where(ww => ww.PaidStatus == 1).Sum(sm => sm.Qty),
        //        //PaidTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 2).Sum(sm => sm.Price),
        //        //PaidCount = s.OrderDetail.Where(ww => ww.PaidStatus == 2).Sum(sm => sm.Qty)
        //    });

        //    var unpaidOrders = orders.Where(w => w.OrderDetail.Any(a => a.PaidStatus == 1 && a.Status != 5)).Select(s => new
        //    {
        //        Id = s.Id,
        //        OrderNo = s.OrderNo,
        //        StaffId = s.StaffId,
        //        PosId = s.PosId,
        //        Total = s.Total,
        //        //UnPricedTotal = s.OrderDetail.Where(ww => ww.PaidStatus < 1).Sum(sm => sm.Price),
        //        //UnPricedCount = s.OrderDetail.Where(ww => ww.PaidStatus < 1).Count(),
        //        TotalDiscount = (s.Discount != null ? s.Discount : 0) + s.OrderDetail.Where(ww => ww.PaidStatus < 2 && ww.Status != 5).Sum(sm => (sm.Discount != null ? sm.Discount : 0) + sm.OrderDetailIgredients.Sum(sum => (sum.Discount != null ? sum.Discount : 0))),
        //        UnPaidTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 1 && ww.Status != 5).Sum(sm => (decimal)((decimal)sm.TotalAfterDiscount + sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))),
        //        UnPaidCount = s.OrderDetail.Where(ww => ww.PaidStatus == 1 && ww.Status != 5).Sum(sm => sm.Qty),
        //        //PricedTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 1).Sum(sm => sm.Price),
        //        //PricedCount = s.OrderDetail.Where(ww => ww.PaidStatus == 1).Count(),
        //        //PaidTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 2).Sum(sm => sm.Price),
        //        //PaidCount = s.OrderDetail.Where(ww => ww.PaidStatus == 2).Count()
        //    });

        //    var paidOrders = orders.Where(w => w.OrderDetail.Any(a => a.PaidStatus == 2 && a.Status != 5)).Select(s => new
        //    {
        //        Id = s.Id,
        //        OrderNo = s.OrderNo,
        //        StaffId = s.StaffId,
        //        PosId = s.PosId,
        //        Total = s.Total,
        //        //UnPricedTotal = s.OrderDetail.Where(ww => ww.PaidStatus < 1).Sum(sm => sm.Price),
        //        //UnPricedCount = s.OrderDetail.Where(ww => ww.PaidStatus < 1).Count(),
        //        //PricedTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 1).Sum(sm => sm.Price),
        //        //PricedCount = s.OrderDetail.Where(ww => ww.PaidStatus == 1).Count(),
        //        TotalDiscount = s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5).Sum(sm => (sm.Discount != null ? sm.Discount : 0) + sm.OrderDetailIgredients.Sum(sum => (sum.Discount != null ? sum.Discount : 0))),//(s.Discount != null ? s.Discount : 0 ) +
        //        PaidTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5).Sum(sm => (decimal)((decimal)sm.TotalAfterDiscount + sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))),
        //        //s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5).Sum(sm => (decimal)(((decimal)sm.Price * (decimal)sm.Qty) + sm.OrderDetailIgredients.Sum(sum => (sum.Price != null ? sum.Price : 0) - (sum.Discount != null ? sum.Discount : 0)) - (sm.Discount != null ? sm.Discount : 0))) - (s.Discount != null ? (decimal?)s.Discount : 0),
        //        PaidCount = s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5).Sum(sm => sm.Qty)
        //    });

        //    //var canceledOrders = db.Order.Where(w => w.EndOfDayId == null && w.PosId == posid && w.OrderStatus.Any(a=>a.Status==5)).Select(s => new
        //    //{
        //    //    Id = s.Id,
        //    //    OrderNo = s.OrderNo,
        //    //    StaffId = s.StaffId,
        //    //    PosId = s.PosId,
        //    //    OrderTotal = s.Total,
        //    //    //UnPricedTotal = s.OrderDetail.Where(ww => ww.PaidStatus < 1).Sum(sm => (double?)((double)sm.Price * (double)sm.Qty) + (double)sm.OrderDetailIgredients.Sum(smsm => smsm.Price)),
        //    //    //PricedTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 1).Sum(sm => (double?)((double)sm.Price * (double)sm.Qty) + (double)sm.OrderDetailIgredients.Sum(sum => sum.Price)),
        //    //   // PaidTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 2).Sum(sm => (double?)((double)sm.Price * (double)sm.Qty) + (double)sm.OrderDetailIgredients.Sum(sum => sum.Price))
        //    //});

        //    //var canceledOrderDetails = db.Order.Where(w => w.EndOfDayId == null && w.PosId == posid && w.OrderDetail.Any(a => a.Status == 5)).Select(s => new
        //    //{
        //    //    Id = s.Id,
        //    //    OrderNo = s.OrderNo,
        //    //    StaffId = s.StaffId,
        //    //    PosId = s.PosId,
        //    //    OrderTotal = s.Total,
        //    //    CancelTotal = s.OrderDetail.Where(ww => ww.Status == 5).Sum(sm => (double?)((double)sm.TotalAfterDiscount) + (double)sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount)),
        //    //    CancelCount = s.OrderDetail.Where(ww => ww.Status == 5).Sum(sm => sm.Qty)
        //    //});

        //    var canceledInvoices = db.OrderDetailInvoices.Include("OrderDetail.Order").Include("PosInfoDetail").Where(w => w.OrderDetail.Order.EndOfDayId == null && w.PosInfoDetail.PosInfoId == posid &&
        //    w.PosInfoDetail.IsInvoice == true &&
        //     w.OrderDetail.Status == 5).Select(s => new
        //     {
        //         Id = s.Id,
        //         OrderNo = s.OrderDetail.Order.OrderNo,
        //         StaffId = s.StaffId,
        //         PosId = s.PosInfoDetail.PosInfoId,
        //         OrderTotal = (decimal?)s.OrderDetail.TotalAfterDiscount + (decimal?)(s.OrderDetail.OrderDetailIgredients.Count() > 0 ? s.OrderDetail.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount) : 0),
        //         GroupId = s.PosInfoDetail.GroupId,
        //         Counter = s.Counter
        //         //CancelTotal = s.OrderDetail.Where(ww => ww.Status == 5).Sum(sm => (double?)((double)sm.Price * (double)sm.Qty) + (double)sm.OrderDetailIgredients.Sum(sum => sum.Price)-(double?)sm.Discount) - (double?)s.Discount,
        //         // CancelCount = s.OrderDetail.Where(ww => ww.Status == 5).Sum(sm => sm.Qty)
        //     });

        //    var canceledInvoicesfinal = canceledInvoices.GroupBy(g => new { g.GroupId, g.Counter }).Select(s => new
        //    {
        //        OrderTotal = s.Sum(sm => sm.OrderTotal),
        //        CancelItemsCount = s.Count()
        //    });

        //    var returnedOrders = orders.Where(w => w.OrderDetail.Any(a => a.Price < 0)).Select(s => new
        //    {
        //        Id = s.Id,
        //        OrderNo = s.OrderNo,
        //        StaffId = s.StaffId,
        //        PosId = s.PosId,
        //        OrderTotal = s.Total,
        //        ReturnTotal = s.OrderDetail.Where(ww => ww.Price < 0).Sum(sm => (double?)((double)sm.TotalAfterDiscount) + (double)sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount)),
        //        ReturnCount = s.OrderDetail.Where(ww => ww.Price < 0).Sum(sm => sm.Qty)
        //    });

        //    dynamic Totals = new ExpandoObject();
        //    dynamic Paid = new ExpandoObject();
        //    dynamic UnPaid = new ExpandoObject();
        //    dynamic UnPriced = new ExpandoObject();
        //    dynamic Canceled = new ExpandoObject();
        //    dynamic Returned = new ExpandoObject();
        //    Paid.Total = (decimal?)paidOrders.Sum(s => s.PaidTotal) != null ? paidOrders.Sum(s => s.PaidTotal) : 0;
        //    Paid.Count = paidOrders.Count();
        //    Paid.TotalItems = paidOrders.Count() > 0 ? paidOrders.Sum(s => s.PaidCount):0;
        //    Paid.TotalDiscount = (decimal?)paidOrders.Sum(s => s.TotalDiscount);
        //    Paid.Problem = null;
        //    Totals.Paid = Paid;

        //    UnPaid.Total = (decimal?)unpaidOrders.Sum(s => s.UnPaidTotal) != null ? unpaidOrders.Sum(s => s.UnPaidTotal) : 0;
        //    UnPaid.Count = unpaidOrders.Count();
        //    UnPaid.TotalItems = unpaidOrders.Count() > 0 ? unpaidOrders.Sum(s => s.UnPaidCount) : 0;
        //    UnPaid.TotalDiscount = (decimal?)unpaidOrders.Sum(s => s.TotalDiscount);
        //    UnPaid.Problem = null;
        //    Totals.UnPaid = UnPaid;

        //    UnPriced.Total = (decimal?)unpricedOrders.Sum(s => s.UnPricedTotal) != null ? unpricedOrders.Sum(s => s.UnPricedTotal) : 0;
        //    UnPriced.Count = unpricedOrders.Count();
        //    UnPriced.TotalItems = unpricedOrders.Count() > 0 ? unpricedOrders.Sum(s => s.UnPricedCount) : 0;
        //    UnPriced.TotalDiscount = (decimal?)unpricedOrders.Sum(s => s.TotalDiscount);
        //    UnPriced.Problem = null;
        //    Totals.UnPriced = UnPriced;

        //    Canceled.Total = canceledInvoicesfinal.Sum(s => s.OrderTotal) != null ? canceledInvoicesfinal.Sum(s => s.OrderTotal) : 0;
        //    Canceled.Count = canceledInvoicesfinal.Count();
        //    Canceled.TotalItems = canceledInvoicesfinal.Count() > 0 ? canceledInvoicesfinal.Sum(s => s.CancelItemsCount) : 0;
        //    //Canceled.TotalDiscount = canceledOrderDetails.Sum(s => s.TotalDiscount);
        //    Canceled.Problem = null;
        //    Totals.Canceled = Canceled;

        //    Returned.Total = returnedOrders.Count() > 0 ? returnedOrders.Sum(s => s.ReturnTotal) : 0;
        //    Returned.Count = returnedOrders.Count();
        //    Returned.TotalItems = returnedOrders.Count() > 0 ? returnedOrders.Sum(s => s.ReturnCount) : 0;
        //    //Returned.TotalDiscount = returnedOrders.Sum(s => s.TotalDiscount);
        //    Returned.Problem = null;
        //    Totals.Returned = Returned;

        //    Totals.TotalPrice = (decimal?)((decimal?)Totals.Paid.Total + (decimal?)Totals.UnPaid.Total + (decimal?)Totals.UnPriced.Total);
        //    Totals.TotalCount = (int?)(Totals.Paid.Count + Totals.UnPaid.Count + Totals.UnPriced.Count);
        //    Totals.TotalItemsSum = (int?)(Totals.Paid.TotalItems + Totals.UnPaid.TotalItems + Totals.UnPriced.TotalItems);
        //    Totals.TotalDiscount = (decimal?)((decimal?)Totals.Paid.TotalDiscount + (decimal?)Totals.UnPaid.TotalDiscount + (decimal?)Totals.UnPriced.TotalDiscount);

        //    var orderdetailsinvoices = db.OrderDetailInvoices.Where(w => w.OrderDetail.Order.EndOfDayId == null
        //        && w.PosInfoDetail.IsInvoice == true && w.PosInfoDetail.PosInfoId == posid && w.OrderDetail.Status != 5);
        //    Totals.ReceiptCount = orderdetailsinvoices.GroupBy(g => new { g.PosInfoDetailId, g.Counter }).Select(s => new
        //        {
        //            PosInfoDetailId = s.Key.PosInfoDetailId,
        //            Counter = s.Key.Counter,
        //            Count = s.Count()
        //        }).Distinct().Count();

        //    var transactions = db.Transactions.Include(i => i.Order).Include(i => i.Accounts).Include(e => e.OrderDetail.Select(f => f.OrderDetailInvoices)).Where(w => w.PosInfoId == posid && w.EndOfDayId == null).AsNoTracking().AsEnumerable();
        //    Totals.Gross = transactions.Where(w => w.TransactionType == (int)TransactionType.Sale || w.TransactionType == (int)TransactionType.Cancel).Sum(g => g.Gross);
        //    Totals.Net = transactions.Where(w => w.TransactionType == (int)TransactionType.Sale || w.TransactionType == (int)TransactionType.Cancel).Sum(g => g.Net);
        //    Totals.Vat = transactions.Where(w => w.TransactionType == (int)TransactionType.Sale || w.TransactionType == (int)TransactionType.Cancel).Sum(g => g.Vat);
        //    Totals.Tax = transactions.Where(w => w.TransactionType == (int)TransactionType.Sale || w.TransactionType == (int)TransactionType.Cancel).Sum(g => g.Tax);

        //    var acc = transactions.Where(w => w.TransactionType == (int)TransactionType.Sale || w.TransactionType == (int)TransactionType.Cancel).GroupBy(g => g.AccountId).Select(s => new //.Where(w=>w.TransactionType == (int)TransactionType.Sale)
        //    {
        //        AccountId = s.Key,
        //        Description = s.FirstOrDefault().Accounts != null ? s.FirstOrDefault().Accounts.Description: "",
        //        TotalAmount = s.Sum(sum => sum.Gross)
        //    });
        //    Totals.Accounts = acc;

        //    var count = transactions.Where(w => w.TransactionType == (short)TransactionType.Sale);
        //    var cancel = transactions.Where(w => w.TransactionType == (short)TransactionType.Cancel);
        //    decimal? summary = count.Sum(s => s.Gross) + cancel.Sum(s => s.Gross);
        //    Totals.AverageTicketTotal = (count.Count() - cancel.Count()) != 0 ? (decimal)summary / (count.Count() - cancel.Count()) : 0;

        //    //GIA NA MPOREI NA KLEISEI
        //    var tra = transactions.Where(w => w.TransactionType == 3);
        //    var traCancel = transactions.Where(w => w.TransactionType == 4).Sum(s => s.Gross);
        //    traCancel = traCancel == null ? 0 : traCancel;
        //    var traTotal = tra.Sum(s => s.Gross) == null ? 0 : tra.Sum(s => s.Gross);
        //    traTotal = traTotal + traCancel;

        //    var TotalPaid = (decimal?)paidOrders.Sum(s => s.PaidTotal);
        //    TotalPaid = TotalPaid == null ? 0 : TotalPaid;

        //    Totals.BalanceOk = Math.Round(TotalPaid.Value,2) == Math.Round(traTotal.Value,2);

        //    if (transactions == null)
        //    {
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        //    }



        //    var group = transactions.Where(w => w.StaffId != null).GroupBy(g => g.StaffId).Select(s => new
        //    {
        //        Id = s.Key,
        //        Code = db.Staff.Find(s.Key).Code,
        //        FirstName = db.Staff.Find(s.Key).FirstName,
        //        ImageUri = db.Staff.Find(s.Key).ImageUri,
        //        LastName = db.Staff.Find(s.Key).LastName,
        //        Transactions = s.GroupBy(gg => gg.TransactionType).Select(ss => new
        //        {
        //            Description = ss.FirstOrDefault().Description,
        //            Amount = ss.Sum(sum => sum.Amount),
        //            Count = ss.Count(),
        //            TransType = ss.Key,
        //            TranAnalysis = ss.GroupBy(f => new { AccId = f.AccountId, AccDesc = f.Accounts.Description }).Select(ff => new
        //            {
        //                AccountDescr = ff.Key.AccDesc,
        //                AccId = ff.Key.AccId,
        //                Amount = ff.Sum(f => f.Gross),
        //                TicketsCount = ff.Select(f => f.OrderDetail.Select(fff => fff.OrderDetailInvoices.Where(g => g.OrderDetail.Status != 5))).Count(),//ff.GroupBy(fff => fff.ord).Count(),
        //                //Voids = ff.Select(f => f.OrderDetail.Select(fff => fff.OrderDetailInvoices.Where(g => g.OrderDetail.Status == 5)
        //                //    .GroupBy(e => new { invgroup = e.PosInfoDetail.GroupId, invposinfid = e.PosInfoDetailId, invcounter = e.Counter  })))


        //                //  Voids = ff.Select(o => o.Order.OrderDetail.Where(g => g.Status == 5).Where(gg => gg.))
        //                //Void = ff.Select(o => o.OrderDetail.Select(oo => oo.OrderDetailInvoices
        //                //    .Where(gg => gg.OrderDetail.Status == 5 && gg.PosInfoDetail.IsInvoice == true)
        //                //    .Select(q => new
        //                //    {
        //                //        InvoiceGroup = q.PosInfoDetail.GroupId,
        //                //        InvoiceAbbreviation = q.PosInfoDetail.Abbreviation,
        //                //        InvoiceCounter = q.Counter
        //                //    })))

        //            })
        //        }).AsEnumerable(),
        //        Balance = s.Sum(sum => sum.Amount),
        //        TotalTransactions = s.Count()
        //    });

        //    var flat = transactions;

        //    Totals.ItemsCount = orderdetailsinvoices.Sum(s=>s.OrderDetail.Qty) != null ? orderdetailsinvoices.Sum(s=>s.OrderDetail.Qty) :0 ;
        //    //int itemscount = 0;
        //    //foreach (var i in db.Transactions.Include("Order").Include("Order.OrderDetail").Where(w => w.PosInfoId == posid && w.EndOfDayId == null))
        //    //{
        //    //    if (i.Order != null)
        //    //        itemscount += i.Order.OrderDetail.Count;
        //    //}

        //    return new { group, Totals };
        //}
        #endregion

        #region End of day View with Invoices
        public Object GetTransactionsNew(long posid, string foday, bool byposid, bool n)
        {
            try
            {
                using (PosEntities db = new PosEntities(false))
            {
                db.Configuration.LazyLoadingEnabled = true;
                db.Configuration.ProxyCreationEnabled = true;

                var validInvoiceTypesIds = db.InvoiceTypes.Where(w => w.Code != "2" && w.Code != "3").Select(w => w.Id).ToList();
                var dpInvTypes = db.InvoiceTypes.Where(w => w.Code == "2").Select(w => w.Id);

                //dp.Dump();
                var getInvoicesFromDetails = (from q in db.OrderDetailInvoices.Where(w => (w.IsDeleted ?? false) == false)
                                              join qq in db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == posid) on q.InvoicesId equals qq.Id
                                              join qqq in db.OrderDetail.Where(w => (w.IsDeleted ?? false) == false) on q.OrderDetailId equals qqq.Id
                                              select new
                                              {
                                                  InvoicesId = q.InvoicesId,
                                                  OrderDetailId = q.OrderDetailId,
                                                  Total = qqq.TotalAfterDiscount + (qqq.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) ?? 0),
                                                  Status = qqq.Status,
                                                  PaidStatus = qqq.PaidStatus,
                                                  InvoiceTypeId = qq.InvoiceTypeId,
                                                  Qty = qqq.Qty
                                              }).Where(w => w.Status != 5 && w.PaidStatus < 1).ToList().GroupBy(g => g.OrderDetailId).Select(s => new
                                              {
                                                  InvoicesId = s.FirstOrDefault().InvoicesId,
                                                  Total = s.FirstOrDefault().Total,
                                                  //HasDP = s.Any(a => a.InvoiceTypeId == 2),
                                                  HasDP = dpInvTypes.Contains(s.FirstOrDefault().InvoiceTypeId.Value),
                                                  InvCount = s.Count(),
                                                  Qty = s.FirstOrDefault().Qty

                                              }).Where(w => w.HasDP).GroupBy(g => g.InvoicesId).Select(ss => new
                                              {
                                                  InvoicesId = ss.FirstOrDefault().InvoicesId,
                                                  Total = ss.Sum(sm => sm.Total),
                                                  Qty = ss.Sum(sm => sm.Qty)
                                              }).ToList();
                //getInvoicesFromDetails.Dump();

                //var invtrans = (from q in db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == posid && (w.IsDeleted ?? false == false))
                //                join qq in db.Transactions on q.Id equals qq.InvoicesId
                //                select new
                //                {
                //                    InvoicesId = q.Id,
                //                    Amount = qq.Amount
                //                }).ToList().GroupBy(g => g.InvoicesId).Select(ss => new
                //                {
                //                    Id = ss.FirstOrDefault().InvoicesId,
                //                    Amount = ss.Sum(sm => sm.Amount)
                //                });

                //var invs = (from s in db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == posid && (w.IsDeleted ?? false == false))//.ToList()
                //            join qq in validInvoiceTypesIds on s.InvoiceTypeId equals qq
                //         //   join qqq in invtrans on s.Id equals qqq.Id into f
                //        //    from it in f.DefaultIfEmpty()
                //            select new
                //            {
                //                InvoicesId = s.Id,
                //                PosInfoId = s.PosInfoId,
                //                Discount = s.Discount,
                //                Total = s.Total,
                //                IsPaid = s.Transactions.Count() > 0,
                //                IsVoided = s.IsVoided ?? false,
                //                InvoiceTypeId = s.InvoiceTypeId,
                //                IsPrinted = s.IsPrinted ?? false,
                //                //ItemsCount = s.SelectMany(sm=>sm.OrderDetailInvoices).
                //                PaidTotal =  s.Transactions.Sum(sm => sm.Amount) ?? 0
                //             //   PaidTotal = it.Amount
                //            }).ToList();


                var validInvoices = db.Invoices.ToList().Where(w => w.EndOfDayId == null
                                                       && w.PosInfoId == posid
                                                       && (w.IsDeleted ?? false) == false
                                                      //   && (w.IsPrinted ?? false) == true
                                                      ).Select(s => s.Id).Distinct();
                var itemsCount = (from q in db.OrderDetailInvoices
                                  join qq in db.OrderDetail on q.OrderDetailId equals qq.Id
                                  join qqq in validInvoices on q.InvoicesId equals qqq
                                  select new
                                  {
                                      InvoiceId = q.InvoicesId,
                                      ItemsCount = qq.Qty

                                  }).GroupBy(g => g.InvoiceId).Select(s => new
                                  {
                                      InvoiceId = s.Key,
                                      ItemsCount = s.Sum(sm => sm.ItemsCount)
                                  }).ToList();

                var invsTemp = (from s in db.Invoices.Where(w => w.EndOfDayId == null
                                                              && w.PosInfoId == posid
                                                              && (w.IsDeleted ?? false) == false
                                                      //        && (w.IsPrinted ?? false) == true)
                                                      )
                                join qq in validInvoiceTypesIds on s.InvoiceTypeId equals qq
                                join qqq in itemsCount on s.Id equals qqq.InvoiceId
                                select new
                                {
                                    InvoicesId = s.Id,
                                    PosInfoId = s.PosInfoId,
                                    Covers = s.Cover,
                                    Discount = s.Discount,
                                    Total = s.Total,
                                    IsPaid = s.Transactions.Count() > 0,
                                    IsVoided = s.IsVoided ?? false,
                                    InvoiceTypeId = s.InvoiceTypeId,
                                    IsPrinted = s.IsPrinted ?? false,
                                    ItemsCount = qqq.ItemsCount,
                                    PaidTotal = 0//s.Transactions.Count() == 0? 0:s.Transactions.Sum(sm=>sm.Amount)
                                }).ToList();

                var invs = (from q in invsTemp
                            join qq in db.Transactions.Where(w => w.EndOfDayId == null && w.PosInfoId == posid) on q.InvoicesId equals qq.InvoicesId into f
                            from it in f.DefaultIfEmpty()
                            select new
                            {
                                InvoicesId = q.InvoicesId,
                                PosInfoId = q.PosInfoId,
                                Discount = q.Discount,
                                Covers = q.Covers,
                                Total = q.Total,
                                IsPaid = q.IsPaid,
                                IsVoided = q.IsVoided,
                                InvoiceTypeId = q.InvoiceTypeId,
                                IsPrinted = q.IsPrinted,
                                TransAmount = it != null ? Math.Round((Decimal)it.Amount, 3, MidpointRounding.AwayFromZero) : 0,
                                ItemsCount = q.ItemsCount
                            }).ToList().GroupBy(g => g.InvoicesId).Select(ss => new
                            {
                                InvoicesId = ss.FirstOrDefault().InvoicesId,
                                PosInfoId = ss.FirstOrDefault().PosInfoId,
                                Covers = ss.FirstOrDefault().Covers,
                                Discount = ss.FirstOrDefault().Discount,
                                Total = ss.FirstOrDefault().Total,
                                IsPaid = ss.FirstOrDefault().IsPaid,
                                IsVoided = ss.FirstOrDefault().IsVoided,
                                InvoiceTypeId = ss.FirstOrDefault().InvoiceTypeId,
                                IsPrinted = ss.FirstOrDefault().IsPrinted,
                                // PaidTotal = ss.Sum(sm => sm.TransAmount) > ss.FirstOrDefault().Total ? ss.FirstOrDefault().Total : ss.Sum(sm => sm.TransAmount),
                                PaidTotal = ss.Sum(sm => Math.Round((Decimal)sm.TransAmount, 2, MidpointRounding.AwayFromZero)) > Math.Round((Decimal)ss.FirstOrDefault().Total, 2, MidpointRounding.AwayFromZero) ? Math.Round((Decimal)ss.FirstOrDefault().Total, 2, MidpointRounding.AwayFromZero) : ss.Sum(sm => Math.Round((Decimal)sm.TransAmount, 2, MidpointRounding.AwayFromZero)),
                                ItemsCount = ss.FirstOrDefault().ItemsCount
                            });

                var paidinvs = invs.Where(w => w.IsPaid && w.IsVoided == false && w.IsPrinted).ToList();
                var voidinvs = invs.Where(w => w.IsPaid && w.IsVoided == true && w.IsPrinted).ToList();
                var unpaidinvs = invs.Where(w => (Math.Abs((decimal)(w.Total - w.PaidTotal)) > 1) && w.IsPrinted);
                //invs.Dump();
                //unpaidinvs.Dump();
                //return;
                //var unpriced  = invs.Where(w=>w.


                var validTransInvoices = db.Invoices.Where(w => w.EndOfDayId == null
                                                             && w.PosInfoId == posid
                                                             && (w.IsDeleted ?? false) == false
                                                             && (w.IsVoided ?? false) == false
                                                             && (w.IsPrinted ?? false) == true
                                                              ).Select(s => s.Id).Distinct().ToList();
                var validTransitemsCount = (from q in db.OrderDetailInvoices
                                            join qq in db.OrderDetail on q.OrderDetailId equals qq.Id
                                            join qqq in validTransInvoices on q.InvoicesId equals qqq
                                            select new
                                            {
                                                InvoiceId = q.InvoicesId,
                                                ItemsCount = qq.PaidStatus == 1 ? qq.Qty : 0,
                                                CancelCount = qq.Status == 5 ? qq.Qty : 0

                                            }).GroupBy(g => g.InvoiceId).Select(s => new
                                            {
                                                InvoiceId = s.Key,
                                                ItemsCount = s.Sum(sm => sm.ItemsCount),
                                                CancelCount = s.Sum(sm => sm.CancelCount)
                                            }).ToList();

                //            var trans = db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == posid && (w.IsDeleted ?? false == false) && (w.IsVoided ?? false) == false).SelectMany(sm => sm.Transactions).Select(s => new

                var trans = (from s in db.Invoices.Where(w => w.EndOfDayId == null
                                                  // && w.PosInfoId == posid
                                                  //  && (w.IsDeleted ?? false) == false
                                                  //  && (w.IsVoided ?? false) == false
                                                  // && (w.IsPrinted ?? false) == true
                                                  ).SelectMany(sm => sm.Transactions)
                             join qq in validTransitemsCount on s.InvoicesId equals qq.InvoiceId
                             select new
                             {


                                 AccountId = s.AccountId,
                                 AccountDescription = s.Accounts.Description,
                                 Amount = s.TransactionType == 4 ? Math.Round((decimal)s.Amount, 3) /*s.Amount */ * -1 : Math.Round((decimal)s.Amount, 3),//s.Amount,
                                 AccountType = s.Accounts.Type,
                                 TransactionType = s.TransactionType,
                                 ItemsCount = qq.ItemsCount,
                                 CancelCount = qq.CancelCount,
                                 IsDeleted = s.IsDeleted
                             }).Where(w => (w.IsDeleted ?? false) == false).ToList().GroupBy(g => new { g.AccountId, g.TransactionType }).Select(ss => new
                             {
                                 TransactionType = ss.FirstOrDefault().TransactionType,
                                 AccountId = ss.FirstOrDefault().AccountId,
                                 Description = ss.FirstOrDefault().AccountDescription,
                                 AccountType = ss.FirstOrDefault().AccountType,
                                 TotalAmount = ss.Sum(sm => Math.Round((decimal)sm.Amount, 2)),
                                 ItemsCount = ss.FirstOrDefault().ItemsCount,
                                 CancelCount = ss.FirstOrDefault().CancelCount
                             });



                var orderDetailIds = db.Invoices.Where(w => w.EndOfDayId == null
                                                         && w.PosInfoId == posid
                                                         && (w.IsDeleted ?? false) == false
                                                         && (w.IsVoided ?? false) == false
                                                         && (w.IsPrinted ?? false) == true
                                                         && validInvoiceTypesIds.Contains(w.InvoiceTypeId.Value))
                                                .SelectMany(sm => sm.OrderDetailInvoices).Where(w => w.OrderDetail.Status != 5 && w.OrderDetail.PaidStatus == 2)
                                                .Select(s => s.OrderDetailId).Distinct().ToList();
                /*
                            var odVats = (from q in db.OrderDetailVatAnal
                                          join qq in orderDetailIds on q.OrderDetailId equals qq
                                          select new
                                          {
                                              Id = q.Id,
                                              VatId = q.VatId,
                                              VatRate = q.VatRate,
                                              VatAmount = q.VatAmount,
                                              Net = q.Net,
                                              Gross = q.Gross,
                                              Tax = q.TaxAmount
                                          });

                            var orderDetailIngIds = from q in db.OrderDetailIgredients
                                                    join qq in orderDetailIds on q.OrderDetailId equals qq
                                                    select new
                                                    {
                                                        Id = q.Id
                                                    };

                            var odingVats = (from q in db.OrderDetailIgredientVatAnal
                                             join qq in orderDetailIngIds on q.OrderDetailIgredientsId equals qq.Id
                                             select new
                                             {
                                                 Id = q.Id,
                                                 VatId = q.VatId,
                                                 VatRate = q.VatRate,
                                                 VatAmount = q.VatAmount,
                                                 Net = q.Net,
                                                 Gross = q.Gross,
                                                 Tax = q.TaxAmount
                                             });

                            //	odVats.Sum(sm=>sm.Gross).Dump();
                */
                var sumOds = db.OrderDetail.Where(w => w.Order.EndOfDayId == null && w.Order.PosId == posid && (w.IsDeleted == null || w.IsDeleted == false) && w.Status != 5).SelectMany(s => s.OrderDetailVatAnal).GroupBy(g => g.VatId).Select(s => new {
                    VatId = s.Key,
                    Gross = s.Sum(sm => sm.Gross),
                    VatAmount = s.Sum(sm => sm.VatAmount),
                    Tax = s.Sum(sm => sm.TaxAmount)
                }).ToList();
                var sumOdis = db.OrderDetail.Where(w => w.Order.EndOfDayId == null && w.Order.PosId == posid && (w.IsDeleted == null || w.IsDeleted == false) && w.Status != 5).SelectMany(ss => ss.OrderDetailIgredients).SelectMany(sm => sm.OrderDetailIgredientVatAnal).GroupBy(g => g.VatId).Select(s => new {
                    VatId = s.Key,
                    Gross = s.Sum(sm => sm.Gross),
                    VatAmount = s.Sum(sm => sm.VatAmount),
                    Tax = s.Sum(sm => sm.TaxAmount)
                }).ToList();

                var vatsTemp = sumOds.Union(sumOdis).GroupBy(g => g.VatId).Select(s => new
                {
                    VatId = s.Key,
                    Gross = s.Sum(sm => sm.Gross),
                    VatAmount = s.Sum(sm => sm.VatAmount),
                    Tax = s.Sum(sm => sm.Tax)
                }).ToList();

                var vats = from q in vatsTemp
                           join v in db.Vat.ToList() on q.VatId equals v.Id
                           select new
                           {
                               VatId = q.VatId,
                               VatRate = v.Percentage,
                               VatAmount = q.VatAmount,
                               Net = q.Gross - q.VatAmount - q.Tax,
                               Gross = q.Gross,
                               Tax = q.Tax
                           };

                //var vats = odVats.Union(odingVats).ToList()
                //                .GroupBy(g => g.VatId).Select(s => new
                //                {
                //                    VatId = s.FirstOrDefault().VatId,
                //                    VatRate = s.FirstOrDefault().VatRate,
                //                    VatAmount = s.Sum(sm => sm.VatAmount),
                //                    Net = s.Sum(sm => sm.Net),
                //                    Gross = s.Sum(sm => sm.Gross),
                //                    Tax = s.Sum(sm => sm.Tax)
                //                });
                //odVats.Dump();
                //odingVats.Dump();
                decimal? lockerPrice = 1;

                var totalLockers = db.Transactions.Where(w => w.EndOfDayId == null
                                                         && w.PosInfoId == posid
                                                         && (w.TransactionType == (int)TransactionTypesEnum.OpenLocker
                                                         || w.TransactionType == (int)TransactionTypesEnum.CloseLocker));
                decimal? openLocker = totalLockers.Where(w => w.TransactionType == (int)TransactionTypesEnum.OpenLocker).Sum(sm => sm.Amount);
                decimal? paidLocker = (totalLockers.Where(w => w.TransactionType == (int)TransactionTypesEnum.CloseLocker).Sum(sm => sm.Amount) * -1);

                var rlp = db.RegionLockerProduct.FirstOrDefault();
                if (rlp != null)
                    lockerPrice = rlp.Price;

                var barcodes = db.Transactions.Where(w => w.PosInfoId == posid
                                                      && w.EndOfDayId == null
                                                      && w.TransactionType == (int)TransactionTypesEnum.CreditCode
                                                      && (w.IsDeleted ?? false) == false
                                                      ).Sum(sm => sm.Amount) ?? 0;



                var prods = db.Invoices.AsNoTracking()
                                       .Include("OrderDetailInvoices.OrderDetail")
                                       .Where(w => w.EndOfDayId == null && w.PosInfoId == posid && (w.IsDeleted ?? false) == false && (w.IsVoided ?? false) == false)
                                       .SelectMany(w => w.OrderDetailInvoices).Select(s => new
                                       {
                                           ProductId = s.OrderDetail.ProductId,
                                           Qty = s.OrderDetail.Qty,
                                           Total = s.OrderDetail.TotalAfterDiscount
                                       }).ToList();

                var productsForEODStats = (from q in db.ProductsForEOD
                                           join qq in prods on q.ProductId equals qq.ProductId
                                           select new
                                           {
                                               ProductId = q.ProductId,
                                               Description = q.Description,
                                               Qty = qq.Qty,
                                               Total = qq.Total
                                           }).GroupBy(g => g.ProductId).Select(s => new
                                           {
                                               ProductId = s.FirstOrDefault().ProductId,
                                               Description = s.FirstOrDefault().Description,
                                               Qty = s.Sum(sm => sm.Qty),
                                               Total = s.Sum(sm => sm.Total)
                                           }).ToList();

                var SumOfItemsCount = Math.Round((double)(paidinvs.Sum(s => s.ItemsCount) + unpaidinvs.Sum(s => s.ItemsCount) + getInvoicesFromDetails.Sum(sm => sm.Qty)), 3);
                var Totals = new
                {
                    Paid = new
                    {
                        Total = paidinvs.Where(w => w.IsVoided == false).Sum(sm => sm.PaidTotal),
                        Count = paidinvs.Where(w => w.IsVoided == false).Count(),
                        TotalItems = Math.Round((double)paidinvs.Sum(s => s.ItemsCount), 3),
                        TotalDiscount = paidinvs.Where(w => w.IsVoided == false).Sum(sm => sm.Discount),
                        Problem = (Int64?)null,
                        TotalsPaid = 0,
                        Invs = paidinvs.Where(w => w.IsVoided == false),
                    },
                    UnPaid = new
                    {
                        //Correcting the rounding Issue in the end of day
                        Total = (unpaidinvs.Sum(sm => Math.Round(sm.Total.Value, 2, MidpointRounding.AwayFromZero) - Math.Round(sm.PaidTotal, 2, MidpointRounding.AwayFromZero))),
                        Count = unpaidinvs.Count(),
                        TotalItems = Math.Round((double)unpaidinvs.Sum(s => s.ItemsCount), 3),
                        TotalDiscount = unpaidinvs.Sum(sm => sm.Discount),
                        Problem = (Int64?)null,
                        Invs = unpaidinvs,

                    },
                    UnPriced = new
                    {
                        Total = getInvoicesFromDetails.Sum(sm => sm.Total),
                        Count = getInvoicesFromDetails.Count(),
                        TotalItems = Math.Round((double)getInvoicesFromDetails.Sum(sm => sm.Qty), 3),
                        TotalDiscount = (decimal?)unpaidinvs.Sum(s => s.Discount),
                        Problem = (Int64?)null,
                        Invs = getInvoicesFromDetails
                    },
                    Canceled = new
                    {
                        Total = voidinvs.Sum(sm => sm.Total),
                        Count = voidinvs.Count(),
                        TotalItems = voidinvs.Sum(s => s.ItemsCount),
                        Problem = (Int64?)null,
                    },
                    Returned = new
                    {
                        Total = 0,
                        Count = 0,
                        TotalItems = 0
                    },
                    TotalPrice = paidinvs.Where(w => w.IsVoided == false).Sum(sm => sm.PaidTotal)
                                     + unpaidinvs.Sum(sm => sm.Total - sm.PaidTotal)
                                     + getInvoicesFromDetails.Sum(sm => sm.Total),
                    TotalCount = invs.Where(w => w.IsVoided == false && w.IsPrinted).Count()
                               + getInvoicesFromDetails.Count(),
                    TotalItemsSum = SumOfItemsCount,
                    TotalDiscount = invs.Sum(s => s.Discount),

                    NotPrintedCount = invs.Where(f => f.IsPrinted == false).Count(),
                    NotPrintedTotal = invs.Where(f => f.IsPrinted == false).Sum(sm => sm.Total),
                    ZFiscalTotal = invs.Sum(sm => sm.Total),
                    ReceiptCount = invs.Count() + getInvoicesFromDetails.Count(),
                    Accounts = trans.Where(w => w.TransactionType == 3 && w.AccountType != (int)TransactionTypesEnum.CreditCode).Select(sa => new
                    {

                        AccountId = sa.AccountId,
                        Description = sa.Description,
                        TotalAmount = sa.TotalAmount

                    }),
                    Barcodes = barcodes,
                    //trans.Where(w => w.TransactionType == 3 && w.AccountType == (int)TransactionType.CreditCode).Sum(sm => sm.TotalAmount),
                    Lockers = new
                    {
                        HasLockers = rlp != null,
                        TotalLockers = openLocker / lockerPrice,
                        TotalLockersAmount = openLocker,
                        Paidlockers = (paidLocker / lockerPrice) ?? 0,
                        PaidlockersAmount = paidLocker,
                        OccLockers = (openLocker / lockerPrice) - (paidLocker / lockerPrice),
                        OccLockersAmount = openLocker - paidLocker
                    },
                    Gross = Math.Round(vats.Sum(sm => sm.Gross ?? 0), 2),
                    Net = vats.Sum(sm => sm.Gross - sm.VatAmount - sm.Tax),
                    Gross2 = vats.Sum(sm => sm.Gross),
                    Tax = vats.Sum(sm => sm.Tax),
                    Vat = vats.Sum(sm => sm.VatAmount),
                    ItemsCount = SumOfItemsCount + voidinvs.Sum(s => s.ItemsCount),
                    AverageTicketTotal = invs.Count() > 0 ? vats.Sum(sm => sm.Gross) / (decimal)(invs.Count() + getInvoicesFromDetails.Count()) : 0,
                    BalanceOk = true,
                    ProductsForEODStats = productsForEODStats,
                    Covers = invs.Distinct().Sum(sm => sm.Covers)


                };

                StatisticFilters filters = new StatisticFilters() { PosList = new List<long?>() { posid } };
                var group = ReportsHelper.GetWaiterReportNew(JsonConvert.SerializeObject(filters), db);
                db.Configuration.LazyLoadingEnabled = false;

                return new { group, Totals };

            }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Task.Run(() => GC.Collect());
            }
        }


        public Object GetTransactions(long posid, string foday, bool byposid, bool n, bool nes)
        {
            using (PosEntities db = new PosEntities(false))
            {

                db.Configuration.LazyLoadingEnabled = true;
                var orders = db.Order
                    .Where(w => w.EndOfDayId == null && w.PosId == posid && !w.OrderStatus.Any(a => a.Status == 5) && ((w.IsDeleted ?? false) == false)).ToList();
                var unpricedOrders = orders.Where(w => w.OrderDetail.Any(a => a.PaidStatus < 1 && a.Status != 5)).Select(s => new
                {
                    Id = s.Id,
                    OrderNo = s.OrderNo,
                    StaffId = s.StaffId,
                    PosId = s.PosId,
                    OrderTotal = s.Total,
                    TotalDiscount = (s.Discount != null ? s.Discount : 0) + s.OrderDetail.Where(ww => ww.PaidStatus < 1 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (sm.Discount != null ? sm.Discount : 0) + sm.OrderDetailIgredients.Sum(sum => (sum.Discount != null ? sum.Discount : 0))),
                    UnPricedTotal = s.OrderDetail.Where(ww => ww.PaidStatus < 1 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (decimal)((decimal)sm.TotalAfterDiscount + sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))),
                    UnPricedCount = s.OrderDetail.Where(ww => ww.PaidStatus < 1 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => sm.Qty),

                });

                var unpaidOrders = orders.Where(w => w.OrderDetail.Any(a => a.PaidStatus == 1 && a.Status != 5)).Select(s => new
                {
                    Id = s.Id,
                    OrderNo = s.OrderNo,
                    StaffId = s.StaffId,
                    PosId = s.PosId,
                    Total = s.Total,
                    TotalDiscount = (s.Discount != null ? s.Discount : 0) + s.OrderDetail.Where(ww => ww.PaidStatus < 2 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (sm.Discount != null ? sm.Discount : 0) + sm.OrderDetailIgredients.Sum(sum => (sum.Discount != null ? sum.Discount : 0))),
                    UnPaidTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 1 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (decimal)((decimal)sm.TotalAfterDiscount + sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))),
                    UnPaidCount = s.OrderDetail.Where(ww => ww.PaidStatus == 1 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => sm.Qty),
                });

                var paidOrders = orders.Where(w => w.OrderDetail.Any(a => a.PaidStatus == 2 && a.Status != 5)).Select(s => new
                {
                    Id = s.Id,
                    OrderNo = s.OrderNo,
                    StaffId = s.StaffId,
                    PosId = s.PosId,
                    Total = s.Total,
                    TotalDiscount = s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (sm.Discount != null ? sm.Discount : 0) + sm.OrderDetailIgredients.Sum(sum => (sum.Discount != null ? sum.Discount : 0))),//(s.Discount != null ? s.Discount : 0 ) +
                    PaidTotal = s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (decimal)((decimal)sm.TotalAfterDiscount + sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount))),
                    PaidCount = s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => sm.Qty),
                    PaidTotalGross = s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (decimal)((decimal)sm.OrderDetailVatAnal.Sum(smm => smm.Gross) + (decimal)(sm.OrderDetailIgredients.Count > 0 ? (decimal)sm.OrderDetailIgredients.Sum(sum => sum.OrderDetailIgredientVatAnal.Sum(smm => smm.Gross)) : 0))),
                    PaidTotalNet = s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (decimal)((decimal)sm.OrderDetailVatAnal.Sum(smm => smm.Net) + sm.OrderDetailIgredients.Sum(sum => sum.OrderDetailIgredientVatAnal.Sum(smm => smm.Net)))),
                    PaidTotalVat = s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (decimal)((decimal)sm.OrderDetailVatAnal.Sum(smm => smm.VatAmount) + sm.OrderDetailIgredients.Sum(sum => sum.OrderDetailIgredientVatAnal.Sum(smm => smm.VatAmount)))),
                    PaidTotalTax = s.OrderDetail.Where(ww => ww.PaidStatus == 2 && ww.Status != 5 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (decimal)((decimal)sm.OrderDetailVatAnal.Sum(smm => smm.TaxAmount) + sm.OrderDetailIgredients.Sum(sum => sum.OrderDetailIgredientVatAnal.Sum(smm => smm.TaxAmount))))
                });


                var canceledInvoices = db.OrderDetailInvoices.Include("OrderDetail.Order").Include("PosInfoDetail").Where(w => w.OrderDetail.Order.EndOfDayId == null && w.PosInfoDetail.PosInfoId == posid &&
                w.PosInfoDetail.IsInvoice == true &&
                 w.OrderDetail.Status == 5 && ((w.IsDeleted ?? false) == false)).Select(s => new
                 {
                     Id = s.Id,
                     OrderNo = s.OrderDetail.Order.OrderNo,
                     StaffId = s.StaffId,
                     PosId = s.PosInfoDetail.PosInfoId,
                     OrderTotal = (decimal?)s.OrderDetail.TotalAfterDiscount + (decimal?)(s.OrderDetail.OrderDetailIgredients.Count() > 0 ? s.OrderDetail.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount) : 0),
                     GroupId = s.PosInfoDetail.GroupId,
                     Counter = s.Counter
                 }).ToList();

                var canceledInvoicesfinal = canceledInvoices.GroupBy(g => new { g.GroupId, g.Counter }).Select(s => new
                {
                    OrderTotal = s.Sum(sm => sm.OrderTotal),
                    CancelItemsCount = s.Count()
                });

                var returnedOrders = orders.Where(w => w.OrderDetail.Any(a => a.Price < 0)).Select(s => new
                {
                    Id = s.Id,
                    OrderNo = s.OrderNo,
                    StaffId = s.StaffId,
                    PosId = s.PosId,
                    OrderTotal = s.Total,
                    ReturnTotal = s.OrderDetail.Where(ww => ww.Price < 0 && ((ww.IsDeleted ?? false) == false)).Sum(sm => (double?)((double)sm.TotalAfterDiscount) + (double)sm.OrderDetailIgredients.Sum(sum => sum.TotalAfterDiscount)),
                    ReturnCount = s.OrderDetail.Where(ww => ww.Price < 0 && ((ww.IsDeleted ?? false) == false)).Sum(sm => sm.Qty)
                });


                dynamic Totals = new ExpandoObject();
                dynamic Paid = new ExpandoObject();
                dynamic UnPaid = new ExpandoObject();
                dynamic UnPriced = new ExpandoObject();
                dynamic Canceled = new ExpandoObject();
                dynamic Returned = new ExpandoObject();
                Paid.Total = (decimal?)paidOrders.Sum(s => s.PaidTotal) != null ? paidOrders.Sum(s => s.PaidTotal) : 0;
                Paid.Count = paidOrders.Count();
                Paid.TotalItems = paidOrders.Count() > 0 ? paidOrders.Sum(s => s.PaidCount) : 0;
                Paid.TotalDiscount = (decimal?)paidOrders.Sum(s => s.TotalDiscount);
                Paid.Problem = null;
                Totals.Paid = Paid;

                UnPaid.Total = (decimal?)unpaidOrders.Sum(s => s.UnPaidTotal) != null ? unpaidOrders.Sum(s => s.UnPaidTotal) : 0;
                UnPaid.Count = unpaidOrders.Count();
                UnPaid.TotalItems = unpaidOrders.Count() > 0 ? unpaidOrders.Sum(s => s.UnPaidCount) : 0;
                UnPaid.TotalDiscount = (decimal?)unpaidOrders.Sum(s => s.TotalDiscount);
                UnPaid.Problem = null;
                Totals.UnPaid = UnPaid;

                UnPriced.Total = (decimal?)unpricedOrders.Sum(s => s.UnPricedTotal) != null ? unpricedOrders.Sum(s => s.UnPricedTotal) : 0;
                UnPriced.Count = unpricedOrders.Count();
                UnPriced.TotalItems = unpricedOrders.Count() > 0 ? unpricedOrders.Sum(s => s.UnPricedCount) : 0;
                UnPriced.TotalDiscount = (decimal?)unpricedOrders.Sum(s => s.TotalDiscount);
                UnPriced.Problem = null;
                Totals.UnPriced = UnPriced;

                Canceled.Total = canceledInvoicesfinal.Sum(s => s.OrderTotal) != null ? canceledInvoicesfinal.Sum(s => s.OrderTotal) : 0;
                Canceled.Count = canceledInvoicesfinal.Count();
                Canceled.TotalItems = canceledInvoicesfinal.Count() > 0 ? canceledInvoicesfinal.Sum(s => s.CancelItemsCount) : 0;
                //Canceled.TotalDiscount = canceledOrderDetails.Sum(s => s.TotalDiscount);
                Canceled.Problem = null;
                Totals.Canceled = Canceled;

                Returned.Total = returnedOrders.Count() > 0 ? returnedOrders.Sum(s => s.ReturnTotal) : 0;
                Returned.Count = returnedOrders.Count();
                Returned.TotalItems = returnedOrders.Count() > 0 ? returnedOrders.Sum(s => s.ReturnCount) : 0;
                //Returned.TotalDiscount = returnedOrders.Sum(s => s.TotalDiscount);
                Returned.Problem = null;
                Totals.Returned = Returned;

                Totals.TotalPrice = (decimal?)((decimal?)Totals.Paid.Total + (decimal?)Totals.UnPaid.Total + (decimal?)Totals.UnPriced.Total);
                Totals.TotalCount = (int?)(Totals.Paid.Count + Totals.UnPaid.Count + Totals.UnPriced.Count);
                Totals.TotalItemsSum = (int?)(Totals.Paid.TotalItems + Totals.UnPaid.TotalItems + Totals.UnPriced.TotalItems);
                Totals.TotalDiscount = (decimal?)((decimal?)Totals.Paid.TotalDiscount + (decimal?)Totals.UnPaid.TotalDiscount + (decimal?)Totals.UnPriced.TotalDiscount);

                // New Not Printed Invoices
                var InvoicesNotPrinted = db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == posid && ((w.IsDeleted ?? false) == false && ((w.IsPrinted ?? false) == false) && w.InvoiceTypes.Type != (int)InvoiceTypesEnum.Order)).ToList();
                Totals.NotPrintedCount = InvoicesNotPrinted.Count();
                Totals.NotPrintedTotal = InvoicesNotPrinted.Sum(s => s.Total);

                //Ypologismeno apo ta orderDetails
                //Totals.ZFiscalTotal = (decimal?)((decimal?)Totals.Paid.Total + (decimal?)Totals.UnPaid.Total + (decimal?)Totals.Canceled.Total);

                //Ypologismeno apo ta Invoices
                Totals.ZFiscalTotal = db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == posid && ((w.IsDeleted ?? false) == false && ((w.IsPrinted ?? false) == true) && w.InvoiceTypes.Type != (int)InvoiceTypesEnum.Order && w.InvoiceTypes.Type != (int)InvoiceTypesEnum.Void)).Sum(sm => sm.Total);
                Totals.ZFiscalTotal = Totals.ZFiscalTotal == null ? 0 : Totals.ZFiscalTotal;
                var orderdetailsinvoices = db.OrderDetailInvoices.Where(w => w.OrderDetail.Order.EndOfDayId == null
                    && w.PosInfoDetail.IsInvoice == true && w.PosInfoDetail.PosInfoId == posid && w.OrderDetail.Status != 5 && ((w.IsDeleted ?? false) == false)).ToList();
                Totals.ReceiptCount = orderdetailsinvoices.GroupBy(g => new { g.PosInfoDetailId, g.Counter }).Select(s => new
                {
                    PosInfoDetailId = s.Key.PosInfoDetailId,
                    Counter = s.Key.Counter,
                    Count = s.Count()
                }).Distinct().Count();

                var transactions = db.Transactions.Include(i => i.Order).Include(i => i.Accounts).Include(e => e.OrderDetail.Select(f => f.OrderDetailInvoices))
                    .Where(w => w.PosInfoId == posid && w.EndOfDayId == null && ((w.IsDeleted ?? false) == false)).AsNoTracking().AsEnumerable();

                #region New Total Calculations
                db.Configuration.LazyLoadingEnabled = true;
                var vatAnals = db.OrderDetailVatAnal.Include("OrderDetail.Order")
                                                        .Where(w => w.OrderDetail.Order.EndOfDayId == null && w.OrderDetail.Order.PosId == posid && ((w.IsDeleted ?? false) == false) && w.OrderDetail.PaidStatus == 2)
                                     .GroupBy(g => new { g.VatId, g.OrderDetail.Status }).ToList()
                                     .Select(s => new
                                     {
                                         VatId = s.Key.VatId,
                                         Gross = s.Sum(sm => sm != null ? sm.Gross : 0),
                                     //VatAmount = s.Sum(s1 => s1!=null ? s1.VatAmount :0),
                                     VatAmount = Math.Round((decimal)(s.Sum(sm => sm != null ? sm.Gross : 0) - s.Sum(sm => sm != null ? sm.Net : 0) - s.Sum(sm => sm != null ? sm.TaxAmount : 0)), 2, MidpointRounding.AwayFromZero),
                                         Net = Math.Round((decimal)(s.Sum(sm => sm != null ? sm.Net : 0)), 2, MidpointRounding.AwayFromZero),
                                         Discount = s.Sum(sm => sm != null ? sm.OrderDetail.Discount : 0),
                                         VatRate = s.FirstOrDefault() != null ? s.FirstOrDefault().VatRate : null,
                                         TaxId = s.FirstOrDefault() != null ? s.FirstOrDefault().TaxId : null,
                                         TaxAmount = s.FirstOrDefault() != null ? s.FirstOrDefault().TaxAmount : 0,
                                         Status = s.FirstOrDefault().OrderDetail.Status
                                     }).ToList();


                var vatIngAnals = db.OrderDetailIgredientVatAnal.Include("OrderDetailIgredients")
                                                                .Include("OrderDetailIgredients.OrderDetail.Order")
                                                                .Where(w => w.OrderDetailIgredients.OrderDetail.Order.EndOfDayId == null
                                                                                    && w.OrderDetailIgredients.OrderDetail.Order.PosId == posid && ((w.IsDeleted ?? false) == false) && w.OrderDetailIgredients.OrderDetail.PaidStatus == 2)
                                                  .GroupBy(g => new { g.VatId, g.OrderDetailIgredients.OrderDetail.Status }).ToList()
                                                  .Select(s => new
                                                  {
                                                      VatId = s.Key.VatId,
                                                      Gross = s.Sum(sm => sm.Gross),
                                                  //VatAmount = s.Sum(s1 => s1!=null ? s1.VatAmount :0),
                                                  VatAmount = Math.Round((decimal)(s.Sum(sm => sm != null ? sm.Gross : 0) - s.Sum(sm => sm != null ? sm.Net : 0) - s.Sum(sm => sm != null ? sm.TaxAmount : 0)), 2),
                                                      Net = Math.Round((decimal)(s.Sum(sm => sm != null ? sm.Net : 0)), 2, MidpointRounding.AwayFromZero),
                                                      Discount = s.Sum(sm => sm != null ? sm.OrderDetailIgredients.OrderDetail.Discount : 0),
                                                      VatRate = s.FirstOrDefault() != null ? s.FirstOrDefault().VatRate : null,
                                                      TaxId = s.FirstOrDefault() != null ? s.FirstOrDefault().TaxId : null,
                                                      TaxAmount = s.FirstOrDefault().TaxAmount != null ? s.FirstOrDefault().TaxAmount : 0,
                                                      Status = s.FirstOrDefault().OrderDetailIgredients.OrderDetail.Status
                                                  }).ToList();

                var united = vatAnals.Union(vatIngAnals);

                //throw new Exception("asdas");
                //TODO: Create Group per invoice Types temp solution
                List<Int32?> invoicetypesList = new List<Int32?>() { 1, 4, 5, 7 };
                List<Int32?> voidInvoicetypesList = new List<Int32?>() { 3 };




                var unitedGroupedByVatId = united.Where(w => w.Status != 5).GroupBy(g => g.VatId).ToList()
                                                  .Select(s => new
                                                  {
                                                      VatId = s.Key,
                                                      Gross = s.Sum(sm => sm.Gross),
                                                      VatAmount = Math.Round((decimal)(s.Sum(sm => sm.Gross) - s.Sum(sm => sm.Net != null ? sm.Net : 0) - s.Sum(sm => sm != null ? sm.TaxAmount : 0)), 2, MidpointRounding.AwayFromZero),
                                                      Net = Math.Round((decimal)(s.Sum(sm => sm.Net != null ? sm.Net : 0)), 2, MidpointRounding.AwayFromZero),
                                                      Discount = s.Sum(sm => sm.Discount != null ? sm.Discount : 0),
                                                      VatRate = s.FirstOrDefault().VatRate,
                                                      TaxId = s.FirstOrDefault().TaxId,
                                                      TaxAmount = s.Sum(sm => sm.TaxAmount),
                                                      Status = s.FirstOrDefault().Status
                                                  });

                db.Configuration.LazyLoadingEnabled = false;
                #endregion


                Totals.Gross = paidOrders.Sum(s => s.PaidTotalGross);
                Totals.Gross2 = unitedGroupedByVatId.Sum(s => s.Gross);//
                Totals.Net = unitedGroupedByVatId.Sum(s => s.Net);//paidOrders.Sum(s => s.PaidTotalNet);
                Totals.Tax = unitedGroupedByVatId.Sum(s => s.TaxAmount);//paidOrders.Sum(s => s.PaidTotalTax);
                Totals.Vat = (decimal)Totals.Gross - (decimal)Totals.Net - (decimal)Totals.Tax;//paidOrders.Sum(s => s.PaidTotalVat);

                var acc = transactions.Where(w => w.TransactionType == (int)TransactionTypesEnum.Sale || w.TransactionType == (int)TransactionTypesEnum.Cancel).GroupBy(g => g.AccountId).Select(s => new //.Where(w=>w.TransactionType == (int)TransactionType.Sale)
                {
                    AccountId = s.Key,
                    Description = s.FirstOrDefault().Accounts != null ? s.FirstOrDefault().Accounts.Description : "",
                    TotalAmount = s.Sum(sum => sum.Amount)
                });
                Totals.Accounts = acc;
                Totals.Barcodes = transactions.Where(w => w.TransactionType == (int)TransactionTypesEnum.CreditCode && w.InOut == 0).Sum(s => s.Amount);
                var count = transactions.Where(w => w.TransactionType == (short)TransactionTypesEnum.Sale);
                var cancel = transactions.Where(w => w.TransactionType == (short)TransactionTypesEnum.Cancel);
                decimal? summary = count.Sum(s => s.Amount) + cancel.Sum(s => s.Amount);
                Totals.AverageTicketTotal = (count.Count() - cancel.Count()) != 0 ? (decimal)summary / (count.Count() - cancel.Count()) : 0;

                //GIA NA MPOREI NA KLEISEI
                var tra = transactions.Where(w => w.TransactionType == 3);
                var traCancel = transactions.Where(w => w.TransactionType == 4).Sum(s => s.Amount);
                traCancel = traCancel == null ? 0 : traCancel;
                var traTotal = tra.Sum(s => s.Amount) == null ? 0 : tra.Sum(s => s.Amount);
                traTotal = traTotal + traCancel;

                var TotalPaid = (decimal?)paidOrders.Sum(s => s.PaidTotal);
                TotalPaid = TotalPaid == null ? 0 : TotalPaid;

                Totals.BalanceOk = Math.Round(TotalPaid.Value, 2) == Math.Round(traTotal.Value, 2);

                if (transactions == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                //var invoices = db.Invoices.Include("Transactions").Include("InvoiceTypes").Where(w => w.EndOfDayId == null && w.PosInfoId == posid && (w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Receipt || w.InvoiceTypes.Type == (int)InvoiceTypesEnum.Coplimentary)
                //    && (w.IsVoided ?? false) == false && (w.IsDeleted ?? false) == false && w.Transactions.Count == 0);


                //var group = transactions.Where(w => w.StaffId != null).ToList().GroupBy(g => g.StaffId).Select(s => new
                //{
                //    Id = s.Key,
                //    Code = db.Staff.Find(s.Key).Code,
                //    FirstName = db.Staff.Find(s.Key).FirstName,
                //    ImageUri = db.Staff.Find(s.Key).ImageUri,
                //    LastName = db.Staff.Find(s.Key).LastName,
                //    Transactions = s.GroupBy(gg => gg.TransactionType).Select(ss => new
                //    {
                //        Description = ss.FirstOrDefault().Description,
                //        Amount = ss.Sum(sum => sum.Amount),
                //        Count = ss.Count(),
                //        TransType = ss.Key,
                //        TranAnalysis = ss.GroupBy(f => new { AccId = f.AccountId, AccDesc = f.Accounts.Description }).Select(ff => new
                //        {
                //            AccountDescr = ff.Key.AccDesc,
                //            AccId = ff.Key.AccId,
                //            Amount = ff.Sum(f => f.Amount),
                //            TicketsCount = ff.Select(f => f.OrderDetail.Select(fff => fff.OrderDetailInvoices.Where(g => g.OrderDetail.Status != 5))).Count(),//ff.GroupBy(fff => fff.ord).Count(),
                //            //Voids = ff.Select(f => f.OrderDetail.Select(fff => fff.OrderDetailInvoices.Where(g => g.OrderDetail.Status == 5)
                //            //    .GroupBy(e => new { invgroup = e.PosInfoDetail.GroupId, invposinfid = e.PosInfoDetailId, invcounter = e.Counter  })))


                //            //  Voids = ff.Select(o => o.Order.OrderDetail.Where(g => g.Status == 5).Where(gg => gg.))
                //            //Void = ff.Select(o => o.OrderDetail.Select(oo => oo.OrderDetailInvoices
                //            //    .Where(gg => gg.OrderDetail.Status == 5 && gg.PosInfoDetail.IsInvoice == true)
                //            //    .Select(q => new
                //            //    {
                //            //        InvoiceGroup = q.PosInfoDetail.GroupId,
                //            //        InvoiceAbbreviation = q.PosInfoDetail.Abbreviation,
                //            //        InvoiceCounter = q.Counter
                //            //    })))

                //        })
                //    }).AsEnumerable(),
                //    Balance = s.Sum(sum => sum.Amount),
                //    TotalTransactions = s.Count(),
                //    UnPaidInvoices = new
                //    {
                //        Count = invoices.Where(ww => ww.StaffId == s.Key).Count(),
                //        Amount = invoices.Where(ww => ww.StaffId == s.Key).Sum(sum => sum.Total)
                //    }
                //});


                StatisticFilters filters = new StatisticFilters() { PosList = new List<long?>() { posid } };
                var group = ReportsHelper.GetWaiterReportNew(JsonConvert.SerializeObject(filters), db);

                var flat = transactions;

                Totals.ItemsCount = orderdetailsinvoices.Sum(s => s.OrderDetail.Qty) != null ? orderdetailsinvoices.Sum(s => s.OrderDetail.Qty) : 0;
                //int itemscount = 0;
                //foreach (var i in db.Transactions.Include("Order").Include("Order.OrderDetail").Where(w => w.PosInfoId == posid && w.EndOfDayId == null))
                //{
                //    if (i.Order != null)
                //        itemscount += i.Order.OrderDetail.Count;
                //}
                db.Configuration.LazyLoadingEnabled = false;
                return new { group, Totals };
            }
          
        }
        #endregion

        #region Change Payment Account
        //[Obsolete]
        //public Object GetTransactions(string ordids, long accountId, long prevaccountId, string newcustId,
        //    string prevcustId, string newcustregno, string prevcustregno, bool update, long posid)
        //{
        //    List<long> ids = JsonConvert.DeserializeObject<List<long>>(ordids);
        //    //var trans = db.Transactions.Include(ff => ff.PosInfo.PosInfoDetail).Include(f => f.Order).Include(f => f.OrderDetail)
        //    //    .Include(ff => ff.OrderDetail.Select(f => f.OrderDetailInvoices)).Where(f => ids.Contains(f.Id));//.FirstOrDefault();
        //    var orderdetails = db.OrderDetail.Include(i => i.OrderDetailInvoices).Include(i => i.OrderDetailInvoices.Select(s => s.PosInfoDetail)).Include(f => f.Order).Include(i => i.Order.PosInfo).Where(w => ids.Contains(w.Id)).ToList();
        //    // var trans = db.Transactions.Where(f => ids.Contains(f.Id));

        //    List<TransferObject> objectsForPms = new List<TransferObject>();

        //    foreach (var od in orderdetails)
        //    {
        //        if (od != null)
        //        {
        //            Transactions tra = db.Transactions.Where(w => w.OrderDetail.Any(s => s.TransactionId == od.TransactionId)).FirstOrDefault();
        //            if (tra != null)
        //            {
        //                tra.AccountId = accountId;
        //            }
        //        }
        //    }
        //    //od.AccountId = accountId;
        //    // db.Entry(tr).State = EntityState.Modified;
        //    //db.SaveChanges();

        //    var newaccount = db.Accounts.FirstOrDefault(f => f.Id == accountId);
        //    var oldaccount = db.Accounts.FirstOrDefault(f => f.Id == prevaccountId);
        //    var newmakesTransfer = newaccount.SendsTransfer != null ? (bool)newaccount.SendsTransfer : false;
        //    var oldaccistran = oldaccount.SendsTransfer != null ? (bool)oldaccount.SendsTransfer : false;

        //    // is cash to creditcard etc. sends no transfer
        //    if (newmakesTransfer == false && oldaccistran == false)
        //    {
        //        //continue;
        //    }
        //    if (newmakesTransfer == false && oldaccistran == true) // old makes charge new does not
        //    {
        //        ////////////////// OLD CUSTOMER NEGATIVE TRANSACTION ////////
        //        #region


        //        var oldguestid = long.Parse(prevcustId);
        //        var oldguestRegNo = int.Parse(prevcustregno);
        //        var oldguest = db.Guest.Where(f => f.Id == oldguestid && f.ReservationId == oldguestRegNo).FirstOrDefault();

        //        foreach (var item in orderdetails)
        //        {
        //            var ss = from dd in item.OrderDetailInvoices
        //                     join s in db.PosInfoDetail on dd.PosInfoDetailId equals s.Id
        //                     select new { odinv = dd, isinv = s.IsInvoice };

        //            item.GuestId = null;
        //            foreach (var item1 in ss.Where(f => f.isinv == true))
        //            {
        //                item1.odinv.CustomerId = "";
        //            }
        //        }

        //        var hotel = db.HotelInfo.FirstOrDefault();
        //        var query = (from f in orderdetails
        //                     //  join e in db.PosInfo on f.OrderId equals e.Id
        //                     //  join pr in db.PricelistDetail on f.PriceListDetailId equals pr.PricelistId
        //                     //  join st in db.SalesType on f.SalesTypeId equals st.Id
        //                     //  join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
        //                     //equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
        //                     //  from ls in loj.DefaultIfEmpty()
        //                     join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
        //                     join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
        //                     join st in db.SalesType on f.SalesTypeId equals st.Id
        //                     join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
        //                   equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
        //                     from ls in loj.DefaultIfEmpty()
        //                     select new
        //                     {
        //                         Id = f.Id,
        //                         SalesTypeId = st.Id,
        //                         Total = f.TotalAfterDiscount + (f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
        //                         PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
        //                         PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
        //                         // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
        //                         ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
        //                         PosId = f.Order.PosId,
        //                         PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
        //                         TransactionId = f.TransactionId,
        //                         PosName = f.Order.PosInfo.Description
        //                         //  ReceiptNo = ls.

        //                     }).Distinct()
        //                     .GroupBy(g => g.PmsDepartmentId).Select(s => new
        //                     {
        //                         PmsDepartmentId = s.Key,
        //                         PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
        //                         Total = s.Sum(sm => sm.Total),
        //                         //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
        //                         // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
        //                         Guest = oldguest,
        //                         ReceiptNo = s.FirstOrDefault().ReceiptNo,
        //                         PosId = s.FirstOrDefault().PosId,
        //                         PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
        //                         TransactionId = s.FirstOrDefault().TransactionId,
        //                         PosName = s.FirstOrDefault().PosName

        //                     });

        //        var IsCreditCard = false;
        //        var roomOfCC = "";
        //        if (oldaccount.Type == 4)
        //        {
        //            IsCreditCard = true;
        //            roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
        //                db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
        //        }
        //        foreach (var g in query)
        //        {
        //            TransferToPms tpms = new TransferToPms();

        //            tpms.Description = "Rec: " + g.ReceiptNo + ", Pos: " + g.PosId + ", " + g.PosName;
        //            tpms.PmsDepartmentId = g.PmsDepartmentId;
        //            tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
        //            tpms.ProfileId = !IsCreditCard ? g.Guest.ProfileNo.ToString() : null;
        //            tpms.ProfileName = !IsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : oldaccount.Description;
        //            tpms.ReceiptNo = g.ReceiptNo.ToString();
        //            tpms.RegNo = !IsCreditCard ? g.Guest.ReservationId.ToString() : "0";
        //            tpms.RoomDescription = !IsCreditCard ? g.Guest.Room : roomOfCC;
        //            tpms.RoomId = !IsCreditCard ? g.Guest.RoomId.ToString() : null;
        //            tpms.SendToPMS = true;
        //            tpms.SendToPmsTS = DateTime.Now;
        //            tpms.TransactionId = g.TransactionId;
        //            tpms.TransferType = 0;//Xrewstiko
        //            tpms.Total = (decimal)g.Total * (-1);  //negative
        //            tpms.TransferIdentifier = Guid.NewGuid();
        //            tpms.PmsDepartmentDescription = g.PmsDepDescription;
        //            tpms.PosInfoId = g.PosId;
        //            db.TransferToPms.Add(tpms);


        //            TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, IsCreditCard, roomOfCC, tpms, Guid.NewGuid());
        //            to.pmsDepartmentDescription = g.PmsDepDescription;
        //            if (IsCreditCard)
        //            {
        //                to.RoomName = roomOfCC;
        //            }
        //            string res = "";

        //            //string storeid = HttpContext.Current.Request.Params["storeid"];

        //            // send only non zero transactions
        //            if (to.amount != 0)
        //                objectsForPms.Add(to);

        //            // 1
        //            //  SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
        //            // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
        //        }

        //        #endregion
        //    }
        //    else if (newmakesTransfer == true && oldaccistran == false) // new makes transaction
        //    {
        //        ///////////// MAKE TRANSACTION //////////////////////
        //        #region

        //        var newCusId = long.Parse(newcustId);
        //        var newCustResId = int.Parse(newcustregno);
        //        var guest = db.Guest.Where(f => f.Id == newCusId && f.ReservationId == newCustResId).FirstOrDefault();
        //        foreach (var item in orderdetails)
        //        {

        //            var ss = from dd in item.OrderDetailInvoices
        //                     join s in db.PosInfoDetail on dd.PosInfoDetailId equals s.Id
        //                     select new { odinv = dd, isinv = s.IsInvoice };

        //            if (guest != null)
        //            {
        //                item.GuestId = guest.Id;
        //            }
        //            foreach (var item1 in ss.Where(f => f.isinv == true))
        //            {
        //                item1.odinv.CustomerId = newcustId;
        //            }
        //        }


        //        var hotel = db.HotelInfo.FirstOrDefault();
        //        var query = (from f in orderdetails
        //                     //  join e in db.PosInfo on f.OrderId equals e.Id
        //                     //  join pr in db.PricelistDetail on f.PriceListDetailId equals pr.PricelistId
        //                     //  join st in db.SalesType on f.SalesTypeId equals st.Id
        //                     //  join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
        //                     //equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
        //                     //  from ls in loj.DefaultIfEmpty()
        //                     join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
        //                     join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
        //                     join st in db.SalesType on f.SalesTypeId equals st.Id
        //                     join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
        //                   equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
        //                     from ls in loj.DefaultIfEmpty()
        //                     select new
        //                     {
        //                         Id = f.Id,
        //                         SalesTypeId = st.Id,
        //                         Total = f.TotalAfterDiscount + (f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
        //                         PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
        //                         PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
        //                         // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
        //                         ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
        //                         PosId = f.Order.PosId,
        //                         PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
        //                         TransactionId = f.TransactionId,
        //                         PosName = f.Order.PosInfo.Description
        //                         //  ReceiptNo = ls.

        //                     }).Distinct()
        //                     .GroupBy(g => g.PmsDepartmentId).Select(s => new
        //                     {
        //                         PmsDepartmentId = s.Key,
        //                         PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
        //                         Total = s.Sum(sm => sm.Total),
        //                         //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
        //                         // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
        //                         Guest = guest,
        //                         ReceiptNo = s.FirstOrDefault().ReceiptNo,
        //                         PosId = s.FirstOrDefault().PosId,
        //                         PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
        //                         TransactionId = s.FirstOrDefault().TransactionId,
        //                         PosName = s.FirstOrDefault().PosName

        //                     });
        //        var IsCreditCard = false;
        //        var roomOfCC = "";
        //        if (newaccount.Type == 4)
        //        {
        //            IsCreditCard = true;
        //            roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
        //                db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
        //        }
        //        foreach (var g in query)
        //        {
        //            TransferToPms tpms = new TransferToPms();

        //            tpms.Description = "Rec: " + g.ReceiptNo + " Pos: " + g.PosId + ", " + g.PosName;
        //            tpms.PmsDepartmentId = g.PmsDepartmentId;
        //            tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
        //            tpms.ProfileId = !IsCreditCard ? g.Guest.ProfileNo.ToString() : null;
        //            tpms.ProfileName = !IsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : newaccount.Description;
        //            tpms.ReceiptNo = g.ReceiptNo.ToString();
        //            tpms.RegNo = !IsCreditCard ? g.Guest.ReservationId.ToString() : "0";
        //            tpms.RoomDescription = !IsCreditCard ? g.Guest.Room : roomOfCC;
        //            tpms.RoomId = !IsCreditCard ? g.Guest.RoomId.ToString() : null;
        //            tpms.SendToPMS = true;
        //            tpms.SendToPmsTS = DateTime.Now;
        //            tpms.TransactionId = g.TransactionId;
        //            tpms.TransferType = 0;//Xrewstiko
        //            tpms.Total = (decimal)g.Total;
        //            tpms.TransferIdentifier = Guid.NewGuid();
        //            tpms.PmsDepartmentDescription = g.PmsDepDescription;
        //            tpms.PosInfoId = g.PosId;
        //            db.TransferToPms.Add(tpms);


        //            TransferObject to = new TransferObject();
        //            to.HotelId = (int)hotel.Id;
        //            to.amount = (decimal)tpms.Total;
        //            int PmsDepartmentId = 0;
        //            var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
        //            //
        //            to.TransferIdentifier = tpms.TransferIdentifier;
        //            //
        //            to.departmentId = PmsDepartmentId;
        //            to.description = tpms.Description;
        //            to.profileName = tpms.ProfileName;
        //            int resid = 0;
        //            var toint = int.TryParse(tpms.RegNo, out resid);
        //            to.resId = resid;
        //            to.HotelUri = hotel.HotelUri;
        //            to.pmsDepartmentDescription = g.PmsDepDescription;
        //            if (IsCreditCard)
        //            {
        //                to.RoomName = roomOfCC;
        //            }
        //            string res = "";

        //            // string storeid = HttpContext.Current.Request.Params["storeid"];

        //            // send only non zero transactions
        //            if (to.amount != 0)
        //                objectsForPms.Add(to);

        //            // 1
        //            //  SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
        //            // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);

        //        }
        //        #endregion
        //        // }

        //    }
        //    else if (newmakesTransfer == true && oldaccistran == true) // from customer to customer
        //    {
        //        var hotel = db.HotelInfo.FirstOrDefault();

        //        ////////////////// OLD CUSTOMER NEGATIVE TRANSACTION ////////
        //        #region

        //        var oldguestid = long.Parse(prevcustId);
        //        var oldGuestRegNo = int.Parse(prevcustregno);
        //        var oldguest = db.Guest.Where(f => f.Id == oldguestid && f.ReservationId == oldGuestRegNo).FirstOrDefault();

        //        var query = (from f in orderdetails
        //                     join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
        //                     join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
        //                     join st in db.SalesType on f.SalesTypeId equals st.Id
        //                     join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
        //                   equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
        //                     from ls in loj.DefaultIfEmpty()
        //                     select new
        //                     {
        //                         Id = f.Id,
        //                         SalesTypeId = st.Id,
        //                         Total = f.TotalAfterDiscount + (f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
        //                         PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
        //                         PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
        //                         // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
        //                         ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
        //                         PosId = f.Order.PosId,
        //                         PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
        //                         TransactionId = f.TransactionId,
        //                         PosName = f.Order.PosInfo.Description
        //                         //  ReceiptNo = ls.

        //                     }).Distinct()
        //                     .GroupBy(g => g.PmsDepartmentId).Select(s => new
        //                     {
        //                         PmsDepartmentId = s.Key,
        //                         PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
        //                         Total = s.Sum(sm => sm.Total),
        //                         //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
        //                         // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
        //                         Guest = oldguest,
        //                         ReceiptNo = s.FirstOrDefault().ReceiptNo,
        //                         PosId = s.FirstOrDefault().PosId,
        //                         PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
        //                         TransactionId = s.FirstOrDefault().TransactionId,
        //                         PosName = s.FirstOrDefault().PosName,
        //                     });
        //        var oldIsCreditCard = false;
        //        var oldroomOfCC = "";
        //        if (oldaccount.Type == 4)
        //        {
        //            oldIsCreditCard = true;
        //            oldroomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
        //                db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
        //        }
        //        foreach (var g in query)
        //        {
        //            TransferToPms tpms = new TransferToPms();

        //            tpms.Description = "Rec: " + g.ReceiptNo + ", Pos: " + g.PosId + ", " + g.PosName;
        //            tpms.PmsDepartmentId = g.PmsDepartmentId;
        //            tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
        //            tpms.ProfileId = !oldIsCreditCard ? g.Guest.ProfileNo.ToString() : null;
        //            tpms.ProfileName = !oldIsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : oldaccount.Description;
        //            tpms.ReceiptNo = g.ReceiptNo.ToString();
        //            tpms.RegNo = !oldIsCreditCard ? g.Guest.ReservationId.ToString() : "0";
        //            tpms.RoomDescription = !oldIsCreditCard ? g.Guest.Room : oldroomOfCC;
        //            tpms.SendToPmsTS = DateTime.Now;
        //            tpms.RoomId = !oldIsCreditCard ? g.Guest.RoomId.ToString() : null;
        //            tpms.SendToPMS = true;
        //            tpms.TransactionId = g.TransactionId;
        //            tpms.TransferType = 0;//Xrewstiko
        //            tpms.Total = (decimal)g.Total * (-1);  //negative
        //            tpms.TransferIdentifier = Guid.NewGuid();
        //            tpms.PmsDepartmentDescription = g.PmsDepDescription;
        //            tpms.PosInfoId = g.PosId;
        //            db.TransferToPms.Add(tpms);


        //            TransferObject to = new TransferObject();
        //            to.HotelId = (int)hotel.Id;
        //            to.amount = (decimal)tpms.Total;
        //            int PmsDepartmentId = 0;
        //            var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
        //            //
        //            to.TransferIdentifier = tpms.TransferIdentifier;
        //            //
        //            to.departmentId = PmsDepartmentId;
        //            to.description = tpms.Description;
        //            to.profileName = tpms.ProfileName;
        //            int resid = 0;
        //            var toint = int.TryParse(tpms.RegNo, out resid);
        //            to.resId = resid;
        //            to.HotelUri = hotel.HotelUri;
        //            to.pmsDepartmentDescription = g.PmsDepDescription;
        //            if (oldIsCreditCard)
        //            {
        //                to.RoomName = oldroomOfCC;
        //            }
        //            string res = "";

        //            // string storeid = HttpContext.Current.Request.Params["storeid"];

        //            // send only non zero transactions
        //            if (to.amount != 0)
        //                objectsForPms.Add(to);

        //            // 1
        //            //  SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
        //            //  sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);

        //        }
        //        #endregion

        //        /////////// NEW CUSTOMER TRANSFER //////////////
        //        #region

        //        var newCusId = long.Parse(newcustId);
        //        var newCustRegNo = int.Parse(newcustregno);
        //        var newguest = db.Guest.Where(f => f.Id == newCusId && f.ReservationId == newCustRegNo).FirstOrDefault();
        //        foreach (var item in orderdetails)
        //        {
        //            var ss = from dd in item.OrderDetailInvoices
        //                     join s in db.PosInfoDetail on dd.PosInfoDetailId equals s.Id
        //                     select new { odinv = dd, isinv = s.IsInvoice };
        //            if (newguest != null)
        //            {
        //                item.GuestId = newguest.Id;
        //            }
        //            foreach (var item1 in ss.Where(f => f.isinv == true))
        //            {
        //                item1.odinv.CustomerId = newcustId;
        //            }
        //        }

        //        var query1 = (from f in orderdetails
        //                      join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
        //                      join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
        //                      join st in db.SalesType on f.SalesTypeId equals st.Id
        //                      join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
        //                    equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
        //                      from ls in loj.DefaultIfEmpty()
        //                      select new
        //                      {
        //                          Id = f.Id,
        //                          SalesTypeId = st.Id,
        //                          Total = f.TotalAfterDiscount + (f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
        //                          PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
        //                          PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
        //                          // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
        //                          ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
        //                          PosId = f.Order.PosId,
        //                          PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
        //                          TransactionId = f.TransactionId,
        //                          PosName = f.Order.PosInfo.Description,
        //                          //  ReceiptNo = ls.

        //                      }).Distinct()
        //                           .GroupBy(g => g.PmsDepartmentId).Select(s => new
        //                           {
        //                               PmsDepartmentId = s.Key,
        //                               PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
        //                               Total = s.Sum(sm => sm.Total),
        //                               //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
        //                               // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
        //                               Guest = newguest,
        //                               ReceiptNo = s.FirstOrDefault().ReceiptNo,
        //                               PosId = s.FirstOrDefault().PosId,
        //                               PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
        //                               TransactionId = s.FirstOrDefault().TransactionId,
        //                               PosName = s.FirstOrDefault().PosName

        //                           });
        //        var newIsCreditCard = false;
        //        var newroomOfCC = "";
        //        if (newaccount.Type == 4)
        //        {
        //            newIsCreditCard = true;
        //            newroomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
        //                db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
        //        }
        //        foreach (var g in query1)
        //        {
        //            TransferToPms tpms = new TransferToPms();

        //            tpms.Description = "Rec: " + g.ReceiptNo + ", Pos: " + g.PosId + ", " + g.PosName;
        //            tpms.PmsDepartmentId = g.PmsDepartmentId;
        //            tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
        //            tpms.ProfileId = !newIsCreditCard ? g.Guest.ProfileNo.ToString() : null;
        //            tpms.ProfileName = !newIsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : newaccount.Description;
        //            tpms.ReceiptNo = g.ReceiptNo.ToString();
        //            tpms.RegNo = !newIsCreditCard ? g.Guest.ReservationId.ToString() : "0";
        //            tpms.RoomDescription = !newIsCreditCard ? g.Guest.Room : newroomOfCC;
        //            tpms.RoomId = !newIsCreditCard ? g.Guest.RoomId.ToString() : null;
        //            tpms.SendToPmsTS = DateTime.Now;
        //            tpms.SendToPMS = true;
        //            tpms.TransactionId = g.TransactionId;
        //            tpms.TransferType = 0;//Xrewstiko
        //            tpms.Total = (decimal)g.Total;
        //            tpms.TransferIdentifier = Guid.NewGuid();
        //            tpms.PmsDepartmentDescription = g.PmsDepDescription;
        //            tpms.PosInfoId = g.PosId;
        //            db.TransferToPms.Add(tpms);


        //            TransferObject to = new TransferObject();
        //            to.HotelId = (int)hotel.Id;
        //            to.amount = (decimal)tpms.Total;
        //            int PmsDepartmentId = 0;
        //            var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
        //            //
        //            to.TransferIdentifier = tpms.TransferIdentifier;
        //            //
        //            to.departmentId = PmsDepartmentId;
        //            to.description = tpms.Description;
        //            to.profileName = tpms.ProfileName;
        //            int resid = 0;
        //            var toint = int.TryParse(tpms.RegNo, out resid);
        //            to.resId = resid;
        //            to.HotelUri = hotel.HotelUri;
        //            to.pmsDepartmentDescription = g.PmsDepDescription;
        //            if (newIsCreditCard)
        //            {
        //                to.RoomName = newroomOfCC;
        //            }
        //            string res = "";


        //            // send only non zero transactions
        //            if (to.amount != 0)
        //                objectsForPms.Add(to);

        //            // 1
        //            // SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
        //            // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);

        //        }
        //        #endregion

        //    }

        //    //    }
        //    //}

        //    db.SaveChanges();

        //    string storeid = HttpContext.Current.Request.Params["storeid"];
        //    // MAKE ALL TRANSACTIONS
        //    foreach (var to in objectsForPms)
        //    {
        //        SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
        //        sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
        //    }


        //    return new object();
        //}
        #endregion

        #region Change Payment Account Split Accounts
        public Object GetTransactions(string ordids, long accountId, long prevaccountId, string newcustId,
            string prevcustId, string newcustregno, string prevcustregno, bool update, long posid, long invoiceId, long transactionId)
        {

            using (PosEntities db = new PosEntities(false))
            {


                List<long> ids = JsonConvert.DeserializeObject<List<long>>(ordids);
                //var trans = db.Transactions.Include(ff => ff.PosInfo.PosInfoDetail).Include(f => f.Order).Include(f => f.OrderDetail)
                //    .Include(ff => ff.OrderDetail.Select(f => f.OrderDetailInvoices)).Where(f => ids.Contains(f.Id));//.FirstOrDefault();
                var orderdetails = db.OrderDetail.Include(i => i.OrderDetailInvoices).Include(i => i.OrderDetailInvoices.Select(s => s.PosInfoDetail))
                    .Include(f => f.Order).Include(i => i.Order.PosInfo).Where(w => ids.Contains(w.Id)).ToList();
                // var trans = db.Transactions.Where(f => ids.Contains(f.Id));

                List<TransferObject> objectsForPms = new List<TransferObject>();
                var departmentDescritpion = db.PosInfo.Include("Department").FirstOrDefault(f => f.Id == posid);
                string depstr = departmentDescritpion.Department != null ? departmentDescritpion.Department.Description : departmentDescritpion.Description;


                decimal CurAmount = 0;
                var newaccount = db.Accounts.FirstOrDefault(f => f.Id == accountId);
                var oldaccount = db.Accounts.FirstOrDefault(f => f.Id == prevaccountId);
                var newmakesTransfer = newaccount.SendsTransfer != null ? (bool)newaccount.SendsTransfer : false;
                var oldaccistran = oldaccount.SendsTransfer != null ? (bool)oldaccount.SendsTransfer : false;
                var invoice = db.Invoices.Include(i => i.Transactions.Select(ii => ii.Invoice_Guests_Trans)).Include(i => i.Transactions.Select(ii => ii.TransferToPms))
                    .Where(w => w.Id == invoiceId).Include(i => i.Invoice_Guests_Trans).FirstOrDefault();
                Transactions curTransactiontoChange = invoice.Transactions.Where(w => w.Id == transactionId).FirstOrDefault();//invoice.Transactions.Where(w => w.AccountId == prevaccountId).FirstOrDefault();
                List<Transactions> latestTransactions = invoice.Transactions.Where(w => w.Id == transactionId).ToList();//invoice.Transactions.Where(w => w.AccountId == prevaccountId && (w.IsDeleted ?? false) == false).ToList();
                if (invoice != null)
                {
                    foreach (var t in latestTransactions)
                    {
                        if (t.Invoice_Guests_Trans.FirstOrDefault() != null && t.Invoice_Guests_Trans.FirstOrDefault().GuestId != null)
                        {
                            //An exei polla xrewstika vres poio tha allaxei kai allaxe mono ayto
                            if (t.Invoice_Guests_Trans.FirstOrDefault().GuestId == long.Parse(prevcustId))
                            {
                                t.AccountId = accountId;
                                CurAmount = t.Amount != null ? t.Amount.Value : 0;
                                ////An apo xrewstiko paei se mh xrewstiko vgale to GuestId
                                //if (newmakesTransfer == false || newaccount.Type == (int)AccountType.CreditCard)
                                //{
                                //    t.Invoice_Guests_Trans.FirstOrDefault().GuestId = null;
                                //}
                                curTransactiontoChange = t;
                                //break;
                            }
                            //An apo xrewstiko paei se mh xrewstiko vgale to GuestId
                            if (newmakesTransfer == false || newaccount.Type == (int)AccountType.CreditCard || newaccount.Type == (int)AccountType.TicketCompliment)
                            {
                                t.Invoice_Guests_Trans.FirstOrDefault().GuestId = null;
                            }
                        }
                        t.AccountId = accountId;
                        CurAmount = t.Amount != null ? t.Amount.Value : 0;
                    }
                }
                decimal TotalAmounts = latestTransactions.Where(w => (w.IsDeleted ?? false) == false).Sum(s => s.Amount) != null ? latestTransactions.Where(w => (w.IsDeleted ?? false) == false).Sum(s => s.Amount).Value : 0;


                // is cash to creditcard etc. sends no transfer
                if (newmakesTransfer == false && oldaccistran == false)
                {
                    //continue;
                }
                if (newmakesTransfer == false && oldaccistran == true) // old makes charge new does not
                {
                    #region OLD CUSTOMER NEGATIVE TRANSACTION



                    var oldguestid = long.Parse(prevcustId);
                    var oldguestRegNo = int.Parse(prevcustregno);
                    var oldguest = db.Guest.Where(f => f.Id == oldguestid && f.ReservationId == oldguestRegNo).FirstOrDefault();

                    foreach (var item in orderdetails)
                    {
                        var ss = from dd in item.OrderDetailInvoices
                                 join s in db.PosInfoDetail on dd.PosInfoDetailId equals s.Id
                                 select new { odinv = dd, isinv = s.IsInvoice };

                        item.GuestId = null;
                        foreach (var item1 in ss.Where(f => f.isinv == true))
                        {
                            item1.odinv.CustomerId = "";
                        }
                    }

                    var hotel = db.HotelInfo.FirstOrDefault();
                    //var query = (from f in orderdetails
                    //             join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
                    //             join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
                    //             join st in db.SalesType on f.SalesTypeId equals st.Id
                    //             join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
                    //           equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                    //             from ls in loj.DefaultIfEmpty()
                    //             select new
                    //             {
                    //                 Id = f.Id,
                    //                 SalesTypeId = st.Id,
                    //                 Total = (decimal)f.TotalAfterDiscount + (decimal)(f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
                    //                 OrderDetail = f,
                    //                 PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                    //                 PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                    //                 // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
                    //                 ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
                    //                 PosId = f.Order.PosId,
                    //                 PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
                    //                 // TransactionId = f.TransactionId,
                    //                 PosName = f.Order.PosInfo.Description
                    //                 //  ReceiptNo = ls.

                    //             }).Distinct()
                    //             .GroupBy(g => g.PmsDepartmentId).Select(s => new
                    //             {
                    //                 PmsDepartmentId = s.Key,
                    //                 PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                    //                 Total = s.Sum(sm => sm.Total),
                    //                 OrderDetails = s.Select(ss => ss.OrderDetail),
                    //                 //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
                    //                 // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
                    //                 Guest = oldguest,
                    //                 ReceiptNo = s.FirstOrDefault().ReceiptNo,
                    //                 PosId = s.FirstOrDefault().PosId,
                    //                 PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                    //                 // TransactionId = s.FirstOrDefault().TransactionId,
                    //                 PosName = s.FirstOrDefault().PosName

                    //             });

                    var NewIsNotSendingTransfer = true;
                    var IsCreditCard = false;
                    var roomOfCC = "";
                    var roomofCash = "";
                    if (oldaccount.Type == 4)
                    {
                        IsCreditCard = true;
                        roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
                            db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                    }
                    if (IsCreditCard == false)//Einai xrewstiko alla oxi karta
                    {
                        if (curTransactiontoChange.Invoice_Guests_Trans.Count > 0)
                        {//Afairesh syndeshs Guest
                         //curTransactiontoChange.Invoice_Guests_Trans.FirstOrDefault().GuestId = null;
                            List<Invoice_Guests_Trans> todelete = new List<Invoice_Guests_Trans>();
                            foreach (var i in curTransactiontoChange.Invoice_Guests_Trans)
                            {
                                todelete.Add(i);
                            }
                            foreach (var i in todelete)
                            {
                                db.Invoice_Guests_Trans.Remove(i);
                            }
                        }
                    }
                    if (NewIsNotSendingTransfer)
                    {
                        roomofCash = db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
                            db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                    }
                    //EpimerismosPerDepartment
                    //decimal totalDiscount = TotalAmounts - CurAmount;
                    //decimal percentageEpim = 1 - (decimal)(CurAmount / TotalAmounts);
                    //decimal totalEpim = 0;
                    //decimal remainingDiscount = totalDiscount;
                    //decimal ctr = 1;
                    //List<dynamic> query2 = new List<dynamic>();
                    //query = query.OrderBy(o => o.Total);
                    //foreach (var f in query)
                    //{
                    //    if (ctr < query.Count())
                    //    {
                    //        decimal subtotal = f.Total;
                    //        decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                    //        totalEpim += subtotal - percSub;
                    //        query2.Add(new
                    //        {
                    //            PmsDepartmentId = f.PmsDepartmentId,
                    //            Total = subtotal - percSub
                    //        });
                    //        remainingDiscount = remainingDiscount - percSub;
                    //    }
                    //    else
                    //    {
                    //        decimal subtotal = f.Total;
                    //        query2.Add(new
                    //        {
                    //            PmsDepartmentId = f.PmsDepartmentId,
                    //            Total = subtotal - remainingDiscount
                    //        });
                    //        totalEpim += subtotal - remainingDiscount;
                    //    }
                    //    ctr++;
                    //}
                    ////
                    //var query3 = from q in query
                    //             join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                    //             select new
                    //             {
                    //                 PmsDepartmentId = q.PmsDepartmentId,
                    //                 PmsDepDescription = q.PmsDepDescription,
                    //                 Total = j.Total,
                    //                 OrderDetails = q.OrderDetails,
                    //                 ReceiptNo = q.ReceiptNo,
                    //                 Guest = q.Guest,
                    //                 PosId = q.PosId,
                    //                 PosInfoDetailId = q.PosInfoDetailId,
                    //                 PosName = q.PosName
                    //             };
                    //
                    ///Check if OK//oldguest = !IsCreditCard && oldguest != null ? oldguest : null;


                    IEnumerable<dynamic> query3 = EpimerismosDepartments(orderdetails, TotalAmounts, CurAmount, oldguest);
                    foreach (var g in query3)
                    {
                        decimal total = g.Total;//new Economic().EpimerisiAccountTotal(g.OrderDetails, (decimal)g.Total);
                        TransferToPms tpms = new TransferToPms();

                        tpms.Description = "Rec: " + g.ReceiptNo + ", Pos: " + g.PosId + ", " + depstr;// g.PosName;
                        tpms.PmsDepartmentId = g.PmsDepartmentId;
                        tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
                        tpms.ProfileId = !IsCreditCard ? g.Guest.ProfileNo.ToString() : null;
                        tpms.ProfileName = !IsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : oldaccount.Description;
                        tpms.ReceiptNo = g.ReceiptNo.ToString();
                        tpms.RegNo = !IsCreditCard ? g.Guest.ReservationId.ToString() : "0";
                        tpms.RoomDescription = !IsCreditCard ? g.Guest.Room : roomOfCC;
                        tpms.RoomId = !IsCreditCard ? g.Guest.RoomId.ToString() : null;
                        tpms.SendToPMS = true;
                        tpms.SendToPmsTS = DateTime.Now;
                        tpms.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                        tpms.TransferType = 0;//Xrewstiko
                        tpms.Total = (decimal)total * (-1);  //negative
                        tpms.TransferIdentifier = Guid.NewGuid();
                        tpms.PmsDepartmentDescription = g.PmsDepDescription;
                        //Set Status Flag (0: Cash, 1: Not Cash)
                        tpms.Status = 1;
                        tpms.PosInfoId = g.PosId;

                        db.TransferToPms.Add(tpms);

                        //FTIAKSE KAI MIA GIA TA CASH
                        TransferToPms tpmsCash = new TransferToPms();

                        tpmsCash.Description = "Rec: " + g.ReceiptNo + ", Pos: " + g.PosId + ", " + depstr;//g.PosName;
                        tpmsCash.PmsDepartmentId = g.PmsDepartmentId;
                        tpmsCash.PosInfoDetailId = g.PosInfoDetailId;//.Id;
                        tpmsCash.ProfileId = null;
                        tpmsCash.ProfileName = newaccount.Description;
                        tpmsCash.ReceiptNo = g.ReceiptNo.ToString();
                        tpmsCash.RegNo = "0";
                        tpmsCash.RoomDescription = roomofCash;
                        tpmsCash.RoomId = null;
                        tpmsCash.SendToPMS = false;
                        tpmsCash.SendToPmsTS = DateTime.Now;
                        tpmsCash.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                        tpmsCash.TransferType = 0;//Xrewstiko
                        tpmsCash.Total = (decimal)total;  //positive
                        tpmsCash.TransferIdentifier = Guid.NewGuid();
                        tpmsCash.PmsDepartmentDescription = g.PmsDepDescription;
                        //Set Status Flag (0: Cash, 1: Not Cash)
                        tpmsCash.Status = 0;
                        tpmsCash.PosInfoId = g.PosId;
                        db.TransferToPms.Add(tpmsCash);


                        TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, IsCreditCard, roomOfCC, tpms, tpmsCash.TransferIdentifier.Value);
                        to.pmsDepartmentDescription = g.PmsDepDescription;

                        //string storeid = HttpContext.Current.Request.Params["storeid"];

                        // send only non zero transactions
                        if (to.amount != 0)
                            objectsForPms.Add(to);

                        // 1
                        //  SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                        // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                    }

                    #endregion
                }
                else if (newmakesTransfer == true && oldaccistran == false) // new makes transaction
                {


                    #region MAKE TRANSACTION


                    //CHANGE TRANSFERS TO PMS THAT IS NOT TRANSFERED (CASH) 
                    foreach (var tr in latestTransactions)
                    {
                        foreach (var transfer in tr.TransferToPms)
                        {
                            transfer.Status = (short)3; //DO NOT SEND at the end of day
                        }
                    }

                    var newCusId = long.Parse(newcustId);
                    var newCustResId = int.Parse(newcustregno);
                    var guest = db.Guest.Where(f => f.Id == newCusId && f.ReservationId == newCustResId).FirstOrDefault();
                    foreach (var item in orderdetails)
                    {

                        var ss = from dd in item.OrderDetailInvoices
                                 join s in db.PosInfoDetail on dd.PosInfoDetailId equals s.Id
                                 select new { odinv = dd, isinv = s.IsInvoice };

                        if (guest != null)
                        {
                            item.GuestId = guest.Id;
                        }
                        foreach (var item1 in ss.Where(f => f.isinv == true))
                        {
                            item1.odinv.CustomerId = newcustId;
                        }
                    }


                    var hotel = db.HotelInfo.FirstOrDefault();
                    //var query = (from f in orderdetails
                    //             join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
                    //             join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
                    //             join st in db.SalesType on f.SalesTypeId equals st.Id
                    //             join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
                    //           equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                    //             from ls in loj.DefaultIfEmpty()
                    //             select new
                    //             {
                    //                 Id = f.Id,
                    //                 SalesTypeId = st.Id,
                    //                 Total = (decimal)f.TotalAfterDiscount + (decimal)(f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
                    //                 OrderDetail = f,
                    //                 PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                    //                 PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                    //                 // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
                    //                 ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
                    //                 PosId = f.Order.PosId,
                    //                 PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
                    //                 //TransactionId = f.TransactionId,
                    //                 PosName = f.Order.PosInfo.Description
                    //                 //  ReceiptNo = ls.

                    //             }).Distinct()
                    //             .GroupBy(g => g.PmsDepartmentId).Select(s => new
                    //             {
                    //                 PmsDepartmentId = s.Key,
                    //                 PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                    //                 Total = s.Sum(sm => sm.Total),
                    //                 OrderDetails = s.Select(ss => ss.OrderDetail),
                    //                 //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
                    //                 // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
                    //                 Guest = guest,
                    //                 ReceiptNo = s.FirstOrDefault().ReceiptNo,
                    //                 PosId = s.FirstOrDefault().PosId,
                    //                 PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                    //                 //TransactionId = s.FirstOrDefault().TransactionId,
                    //                 PosName = s.FirstOrDefault().PosName

                    //             });
                    var IsCreditCard = false;
                    var roomOfCC = "";
                    if (newaccount.Type == 4)
                    {
                        IsCreditCard = true;
                        roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
                            db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                    }
                    if (IsCreditCard == false)
                    {
                        if (curTransactiontoChange.Invoice_Guests_Trans.Count > 0)
                        {
                            curTransactiontoChange.Invoice_Guests_Trans.FirstOrDefault().GuestId = guest.Id;
                        }
                        else
                        {
                            Invoice_Guests_Trans assoc = new Invoice_Guests_Trans();
                            assoc.GuestId = guest.Id;
                            assoc.InvoiceId = curTransactiontoChange.InvoicesId;
                            assoc.TransactionId = curTransactiontoChange.Id;
                            db.Invoice_Guests_Trans.Add(assoc);
                        }
                    }

                    ////EpimerismosPerDepartment
                    //decimal totalDiscount = TotalAmounts - CurAmount;
                    //decimal percentageEpim = 1 - (decimal)(CurAmount / TotalAmounts);
                    //decimal totalEpim = 0;
                    //decimal remainingDiscount = totalDiscount;
                    //decimal ctr = 1;
                    //List<dynamic> query2 = new List<dynamic>();
                    //query = query.OrderBy(o => o.Total);
                    //foreach (var f in query)
                    //{
                    //    if (ctr < query.Count())
                    //    {
                    //        decimal subtotal = f.Total;
                    //        decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                    //        totalEpim += subtotal - percSub;
                    //        query2.Add(new
                    //        {
                    //            PmsDepartmentId = f.PmsDepartmentId,
                    //            Total = subtotal - percSub
                    //        });
                    //        remainingDiscount = remainingDiscount - percSub;
                    //    }
                    //    else
                    //    {
                    //        decimal subtotal = f.Total;
                    //        query2.Add(new
                    //        {
                    //            PmsDepartmentId = f.PmsDepartmentId,
                    //            Total = subtotal - remainingDiscount
                    //        });
                    //        totalEpim += subtotal - remainingDiscount;
                    //    }
                    //    ctr++;
                    //}
                    //


                    //var query3 = from q in query
                    //             join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                    //             select new
                    //             {
                    //                 PmsDepartmentId = q.PmsDepartmentId,
                    //                 PmsDepDescription = q.PmsDepDescription,
                    //                 Total = j.Total,
                    //                 OrderDetails = q.OrderDetails,
                    //                 ReceiptNo = q.ReceiptNo,
                    //                 Guest = q.Guest,
                    //                 PosId = q.PosId,
                    //                 PosInfoDetailId = q.PosInfoDetailId,
                    //                 PosName = q.PosName
                    //             };
                    //


                    IEnumerable<dynamic> query3 = EpimerismosDepartments(orderdetails, TotalAmounts, CurAmount, guest);
                    foreach (var g in query3)
                    {
                        decimal total = g.Total;//new Economic().EpimerisiAccountTotal(g.OrderDetails, g.Total);
                        TransferToPms tpms = new TransferToPms();

                        tpms.Description = "Rec: " + g.ReceiptNo + " Pos: " + g.PosId + ", " + depstr;//g.PosName;
                        tpms.PmsDepartmentId = g.PmsDepartmentId;
                        tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
                        tpms.ProfileId = !IsCreditCard ? g.Guest.ProfileNo.ToString() : null;
                        tpms.ProfileName = !IsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : newaccount.Description;
                        tpms.ReceiptNo = g.ReceiptNo.ToString();
                        tpms.RegNo = !IsCreditCard ? g.Guest.ReservationId.ToString() : "0";
                        tpms.RoomDescription = !IsCreditCard ? g.Guest.Room : roomOfCC;
                        tpms.RoomId = !IsCreditCard ? g.Guest.RoomId.ToString() : null;
                        tpms.SendToPMS = true;
                        tpms.SendToPmsTS = DateTime.Now;
                        tpms.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                        tpms.TransferType = 0;//Xrewstiko
                        tpms.Total = total;//(decimal)g.Total;
                        tpms.TransferIdentifier = Guid.NewGuid();
                        tpms.PmsDepartmentDescription = g.PmsDepDescription;
                        tpms.PosInfoId = g.PosId;
                        db.TransferToPms.Add(tpms);

                        TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, IsCreditCard, roomOfCC, tpms, tpms.TransferIdentifier.Value);
                        to.pmsDepartmentDescription = g.PmsDepDescription;


                        // string storeid = HttpContext.Current.Request.Params["storeid"];

                        // send only non zero transactions
                        if (to.amount != 0)
                            objectsForPms.Add(to);

                        // 1
                        //  SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                        // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);

                    }
                    #endregion

                }
                else if (newmakesTransfer == true && oldaccistran == true) // from customer to customer
                {
                    var hotel = db.HotelInfo.FirstOrDefault();


                    #region OLD CUSTOMER NEGATIVE TRANSACTION

                    var oldguestid = long.Parse(prevcustId);
                    var oldGuestRegNo = int.Parse(prevcustregno);
                    var oldguest = db.Guest.Where(f => f.Id == oldguestid && f.ReservationId == oldGuestRegNo).FirstOrDefault();

                    //var query = (from f in orderdetails
                    //             join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
                    //             join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
                    //             join st in db.SalesType on f.SalesTypeId equals st.Id
                    //             join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
                    //           equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                    //             from ls in loj.DefaultIfEmpty()
                    //             select new
                    //             {
                    //                 Id = f.Id,
                    //                 SalesTypeId = st.Id,
                    //                 Total = (decimal)f.TotalAfterDiscount + (decimal)(f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
                    //                 OrderDetail = f,
                    //                 PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                    //                 PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                    //                 // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
                    //                 ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
                    //                 PosId = f.Order.PosId,
                    //                 PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
                    //                 //TransactionId = f.TransactionId,
                    //                 PosName = f.Order.PosInfo.Description
                    //                 //  ReceiptNo = ls.

                    //             }).Distinct()
                    //             .GroupBy(g => g.PmsDepartmentId).Select(s => new
                    //             {
                    //                 PmsDepartmentId = s.Key,
                    //                 PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                    //                 Total = s.Sum(sm => sm.Total),
                    //                 OrderDetails = s.Select(ss => ss.OrderDetail),
                    //                 //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
                    //                 // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
                    //                 Guest = oldguest,
                    //                 ReceiptNo = s.FirstOrDefault().ReceiptNo,
                    //                 PosId = s.FirstOrDefault().PosId,
                    //                 PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                    //                 //TransactionId = s.FirstOrDefault().TransactionId,
                    //                 PosName = s.FirstOrDefault().PosName,
                    //             });
                    var oldIsCreditCard = false;
                    var oldroomOfCC = "";
                    if (oldaccount.Type == 4)
                    {
                        oldIsCreditCard = true;
                        oldroomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
                            db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                    }
                    if (oldIsCreditCard == false)//Einai xrewstiko alla oxi karta
                    {
                        if (curTransactiontoChange.Invoice_Guests_Trans.Count > 0)
                        {
                            curTransactiontoChange.Invoice_Guests_Trans.FirstOrDefault().GuestId = null;
                        }
                    }
                    ////EpimerismosPerDepartment
                    //decimal totalDiscount = TotalAmounts - CurAmount;
                    //decimal percentageEpim = 1 - (decimal)(CurAmount / TotalAmounts);
                    //decimal totalEpim = 0;
                    //decimal remainingDiscount = totalDiscount;
                    //decimal ctr = 1;
                    //List<dynamic> query2 = new List<dynamic>();
                    //query = query.OrderBy(o => o.Total);
                    //foreach (var f in query)
                    //{
                    //    if (ctr < query.Count())
                    //    {
                    //        decimal subtotal = f.Total;
                    //        decimal percSub = Math.Round((decimal)(subtotal * percentageEpim), 2);
                    //        totalEpim += subtotal - percSub;
                    //        query2.Add(new
                    //        {
                    //            PmsDepartmentId = f.PmsDepartmentId,
                    //            Total = subtotal - percSub
                    //        });
                    //        remainingDiscount = remainingDiscount - percSub;
                    //    }
                    //    else
                    //    {
                    //        decimal subtotal = f.Total;
                    //        query2.Add(new
                    //        {
                    //            PmsDepartmentId = f.PmsDepartmentId,
                    //            Total = subtotal - remainingDiscount
                    //        });
                    //        totalEpim += subtotal - remainingDiscount;
                    //    }
                    //    ctr++;
                    //}
                    ////
                    //var query3 = from q in query
                    //             join j in query2 on q.PmsDepartmentId equals j.PmsDepartmentId
                    //             select new
                    //             {
                    //                 PmsDepartmentId = q.PmsDepartmentId,
                    //                 PmsDepDescription = q.PmsDepDescription,
                    //                 Total = j.Total,
                    //                 OrderDetails = q.OrderDetails,
                    //                 ReceiptNo = q.ReceiptNo,
                    //                 Guest = q.Guest,
                    //                 PosId = q.PosId,
                    //                 PosInfoDetailId = q.PosInfoDetailId,
                    //                 PosName = q.PosName
                    //             };
                    //
                    IEnumerable<dynamic> query3 = EpimerismosDepartments(orderdetails, TotalAmounts, CurAmount, oldguest);
                    foreach (var g in query3)
                    {
                        decimal total = g.Total;//new Economic().EpimerisiAccountTotal(g.OrderDetails, g.Total);
                        TransferToPms tpms = new TransferToPms();

                        tpms.Description = "Rec: " + g.ReceiptNo + ", Pos: " + g.PosId + ", " + depstr;//g.PosName;
                        tpms.PmsDepartmentId = g.PmsDepartmentId;
                        tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
                        tpms.ProfileId = !oldIsCreditCard ? g.Guest.ProfileNo.ToString() : null;
                        tpms.ProfileName = !oldIsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : oldaccount.Description;
                        tpms.ReceiptNo = g.ReceiptNo.ToString();
                        tpms.RegNo = !oldIsCreditCard ? g.Guest.ReservationId.ToString() : "0";
                        tpms.RoomDescription = !oldIsCreditCard ? g.Guest.Room : oldroomOfCC;
                        tpms.SendToPmsTS = DateTime.Now;
                        tpms.RoomId = !oldIsCreditCard ? g.Guest.RoomId.ToString() : null;
                        tpms.SendToPMS = true;
                        tpms.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                        tpms.TransferType = 0;//Xrewstiko
                        tpms.Total = (decimal)total * (-1);  //negative
                        tpms.TransferIdentifier = Guid.NewGuid();
                        tpms.PmsDepartmentDescription = g.PmsDepDescription;
                        tpms.PosInfoId = g.PosId;
                        db.TransferToPms.Add(tpms);

                        TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, oldIsCreditCard, oldroomOfCC, tpms, tpms.TransferIdentifier.Value);
                        to.pmsDepartmentDescription = g.PmsDepDescription;

                        // string storeid = HttpContext.Current.Request.Params["storeid"];

                        // send only non zero transactions
                        if (to.amount != 0)
                            objectsForPms.Add(to);

                        // 1
                        //  SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                        //  sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);

                    }
                    #endregion


                    #region NEW CUSTOMER TRANSFER

                    var newCusId = long.Parse(newcustId);
                    var newCustRegNo = int.Parse(newcustregno);
                    var newguest = db.Guest.Where(f => f.Id == newCusId && f.ReservationId == newCustRegNo).FirstOrDefault();
                    foreach (var item in orderdetails)
                    {
                        var ss = from dd in item.OrderDetailInvoices
                                 join s in db.PosInfoDetail on dd.PosInfoDetailId equals s.Id
                                 select new { odinv = dd, isinv = s.IsInvoice };
                        if (newguest != null)
                        {
                            item.GuestId = newguest.Id;
                        }
                        foreach (var item1 in ss.Where(f => f.isinv == true))
                        {
                            item1.odinv.CustomerId = newcustId;
                        }
                    }

                    //var query1 = (from f in orderdetails
                    //              join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
                    //              join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
                    //              join st in db.SalesType on f.SalesTypeId equals st.Id
                    //              join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
                    //            equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                    //              from ls in loj.DefaultIfEmpty()
                    //              select new
                    //              {
                    //                  Id = f.Id,
                    //                  SalesTypeId = st.Id,
                    //                  Total = (decimal)f.TotalAfterDiscount + (decimal)(f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
                    //                  PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                    //                  PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                    //                  OrderDetail = f,
                    //                  // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
                    //                  ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
                    //                  PosId = f.Order.PosId,
                    //                  PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
                    //                  //TransactionId = f.TransactionId,
                    //                  PosName = f.Order.PosInfo.Description,
                    //                  //  ReceiptNo = ls.

                    //              }).Distinct()
                    //                   .GroupBy(g => g.PmsDepartmentId).Select(s => new
                    //                   {
                    //                       PmsDepartmentId = s.Key,
                    //                       PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                    //                       Total = s.Sum(sm => sm.Total),
                    //                       OrderDetails = s.Select(ss => ss.OrderDetail),
                    //                       //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
                    //                       // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
                    //                       Guest = newguest,
                    //                       ReceiptNo = s.FirstOrDefault().ReceiptNo,
                    //                       PosId = s.FirstOrDefault().PosId,
                    //                       PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                    //                       //TransactionId = s.FirstOrDefault().TransactionId,
                    //                       PosName = s.FirstOrDefault().PosName

                    //                   });
                    var newIsCreditCard = false;
                    var newroomOfCC = "";
                    if (newaccount.Type == 4)
                    {
                        newIsCreditCard = true;
                        newroomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
                            db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                    }
                    if (newIsCreditCard == false)//Einai xrewstiko alla den einai karta
                    {
                        if (curTransactiontoChange.Invoice_Guests_Trans.Count > 0)
                        {
                            curTransactiontoChange.Invoice_Guests_Trans.FirstOrDefault().GuestId = newguest.Id;
                        }
                        else
                        {
                            Invoice_Guests_Trans assoc = new Invoice_Guests_Trans();
                            assoc.GuestId = newguest.Id;
                            assoc.InvoiceId = curTransactiontoChange.InvoicesId;
                            assoc.TransactionId = curTransactiontoChange.Id;
                            db.Invoice_Guests_Trans.Add(assoc);
                        }
                    }
                    ////EpimerismosPerDepartment
                    //decimal totalDiscount1 = TotalAmounts - CurAmount;
                    //decimal percentageEpim1 = 1 - (decimal)(CurAmount / TotalAmounts);
                    //decimal totalEpim1 = 0;
                    //decimal remainingDiscount1 = totalDiscount1;
                    //decimal ctr1 = 1;
                    //List<dynamic> query12 = new List<dynamic>();
                    //query1 = query1.OrderBy(o => o.Total);
                    //foreach (var f in query1)
                    //{
                    //    if (ctr1 < query1.Count())
                    //    {
                    //        decimal subtotal = f.Total;
                    //        decimal percSub = Math.Round((decimal)(subtotal * percentageEpim1), 2);
                    //        totalEpim1 += subtotal - percSub;
                    //        query12.Add(new
                    //        {
                    //            PmsDepartmentId = f.PmsDepartmentId,
                    //            Total = subtotal - percSub
                    //        });
                    //        remainingDiscount1 = remainingDiscount1 - percSub;
                    //    }
                    //    else
                    //    {
                    //        decimal subtotal = f.Total;
                    //        query12.Add(new
                    //        {
                    //            PmsDepartmentId = f.PmsDepartmentId,
                    //            Total = subtotal - remainingDiscount1
                    //        });
                    //        totalEpim1 += subtotal - remainingDiscount1;
                    //    }
                    //    ctr1++;
                    //}
                    ////
                    //var query13 = from q in query1
                    //              join j in query12 on q.PmsDepartmentId equals j.PmsDepartmentId
                    //              select new
                    //              {
                    //                  PmsDepartmentId = q.PmsDepartmentId,
                    //                  PmsDepDescription = q.PmsDepDescription,
                    //                  Total = j.Total,
                    //                  OrderDetails = q.OrderDetails,
                    //                  ReceiptNo = q.ReceiptNo,
                    //                  Guest = q.Guest,
                    //                  PosId = q.PosId,
                    //                  PosInfoDetailId = q.PosInfoDetailId,
                    //                  PosName = q.PosName
                    //              };
                    //
                    IEnumerable<dynamic> query13 = EpimerismosDepartments(orderdetails, TotalAmounts, CurAmount, newguest);
                    foreach (var g in query13)
                    {
                        decimal total = g.Total;//new Economic().EpimerisiAccountTotal(g.OrderDetails, g.Total);
                        TransferToPms tpms = new TransferToPms();

                        tpms.Description = "Rec: " + g.ReceiptNo + ", Pos: " + g.PosId + ", " + depstr;// g.PosName;
                        tpms.PmsDepartmentId = g.PmsDepartmentId;
                        tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
                        tpms.ProfileId = !newIsCreditCard ? g.Guest.ProfileNo.ToString() : null;
                        tpms.ProfileName = !newIsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : newaccount.Description;
                        tpms.ReceiptNo = g.ReceiptNo.ToString();
                        tpms.RegNo = !newIsCreditCard ? g.Guest.ReservationId.ToString() : "0";
                        tpms.RoomDescription = !newIsCreditCard ? g.Guest.Room : newroomOfCC;
                        tpms.RoomId = !newIsCreditCard ? g.Guest.RoomId.ToString() : null;
                        tpms.SendToPmsTS = DateTime.Now;
                        tpms.SendToPMS = true;
                        tpms.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                        tpms.TransferType = 0;//Xrewstiko
                        tpms.Total = total;//(decimal)g.Total;
                        tpms.TransferIdentifier = Guid.NewGuid();
                        tpms.PmsDepartmentDescription = g.PmsDepDescription;
                        tpms.PosInfoId = g.PosId;
                        db.TransferToPms.Add(tpms);

                        TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, newIsCreditCard, newroomOfCC, tpms, tpms.TransferIdentifier.Value);
                        to.pmsDepartmentDescription = g.PmsDepDescription;
                        // send only non zero transactions
                        if (to.amount != 0)
                            objectsForPms.Add(to);

                        // 1
                        // SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                        // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);

                    }
                    #endregion

                }

                db.SaveChanges();

                string storeid = HttpContext.Current.Request.Params["storeid"];
                // MAKE ALL TRANSACTIONS
                foreach (var to in objectsForPms)
                {
                    SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                    //sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                }


                return new object();

            }

        }
        #endregion

        #region Change Payment Type To Split Payment
        public Object GetTransactions(string ordids, long accountId, long prevaccountId, string newcustId,
            string prevcustId, string newcustregno, string prevcustregno, bool update, long posid, long invoiceId, long transactionId, string splitAccounts)
        {

            List<AccountsObj> accList = new List<AccountsObj>();
            if (String.IsNullOrEmpty(splitAccounts) == false)
            {
                accList = JsonConvert.DeserializeObject<List<AccountsObj>>(splitAccounts);
            }

            List<long> ids = JsonConvert.DeserializeObject<List<long>>(ordids);
            //var trans = db.Transactions.Include(ff => ff.PosInfo.PosInfoDetail).Include(f => f.Order).Include(f => f.OrderDetail)
            //    .Include(ff => ff.OrderDetail.Select(f => f.OrderDetailInvoices)).Where(f => ids.Contains(f.Id));//.FirstOrDefault();
            using (PosEntities db = new PosEntities(false))
            {
                var orderdetails = db.OrderDetail.Include(i => i.OrderDetailInvoices).Include(i => i.OrderDetailInvoices.Select(s => s.PosInfoDetail)).Include(f => f.Order).Include(i => i.Order.PosInfo).Where(w => ids.Contains(w.Id)).ToList();
                // var trans = db.Transactions.Where(f => ids.Contains(f.Id));

                List<TransferObject> objectsForPms = new List<TransferObject>();


                decimal CurAmount = 0;
                //var newaccount = db.Accounts.FirstOrDefault(f => f.Id == acc.AccountId);
                var oldaccount = db.Accounts.FirstOrDefault(f => f.Id == prevaccountId);
                //var newmakesTransfer = newaccount.SendsTransfer != null ? (bool)newaccount.SendsTransfer : false;
                var oldaccistran = oldaccount.SendsTransfer != null ? (bool)oldaccount.SendsTransfer : false;
                var invoice = db.Invoices.Include(i => i.Transactions.Select(ii => ii.TransferToPms)).Where(w => w.Id == invoiceId).Include(i => i.Invoice_Guests_Trans).FirstOrDefault();
                IEnumerable<Transactions> curTransactiontoChange = invoice.Transactions.Where(w => (w.IsDeleted ?? false) == false).AsEnumerable();
                List<Transactions> newTransactions = new List<Transactions>();
                if (invoice != null)
                {
                    //curTransactiontoChange = invoice.Transactions.Where(w => w.AccountId == prevaccountId).FirstOrDefault();

                    foreach (var acc in accList)
                    {
                        //Etoimase ta nea Transactions
                        Transactions newtr = new Transactions();
                        newtr.AccountId = acc.AccountId;
                        newtr.Amount = acc.Amount;
                        newtr.Day = DateTime.Now;
                        newtr.DepartmentId = curTransactiontoChange.FirstOrDefault().DepartmentId;
                        newtr.Description = curTransactiontoChange.FirstOrDefault().Description;
                        newtr.ExtDescription = curTransactiontoChange.FirstOrDefault().ExtDescription + " ~ Changed to Splited";
                        newtr.InOut = curTransactiontoChange.FirstOrDefault().InOut;
                        newtr.Invoice_Guests_Trans = new List<Invoice_Guests_Trans>();
                        newtr.InvoicesId = invoice.Id;
                        newtr.OrderId = curTransactiontoChange.FirstOrDefault().OrderId;
                        newtr.PosInfoId = curTransactiontoChange.FirstOrDefault().PosInfoId;
                        newtr.StaffId = curTransactiontoChange.FirstOrDefault().StaffId;
                        newtr.TransactionType = curTransactiontoChange.FirstOrDefault().TransactionType;
                        if (acc.GuestId != null)
                        {
                            Invoice_Guests_Trans assoc = new Invoice_Guests_Trans();
                            assoc.GuestId = acc.GuestId;
                            assoc.InvoiceId = invoice.Id;
                            newtr.Invoice_Guests_Trans.Add(assoc);
                            db.Invoice_Guests_Trans.Add(assoc);
                        }
                        //foreach (var t in invoice.Transactions.Where(w => w.AccountId == prevaccountId))
                        //{
                        //    if (t.Invoice_Guests_Trans.FirstOrDefault() != null && t.Invoice_Guests_Trans.FirstOrDefault().GuestId != null)
                        //    {
                        //        //An exei polla xrewstika vres poio tha allaxei kai allaxe mono ayto
                        //        if (t.Invoice_Guests_Trans.FirstOrDefault().GuestId == long.Parse(prevcustId))
                        //        {
                        //            t.AccountId = accountId;
                        //            CurAmount = t.Amount != null ? t.Amount.Value : 0;
                        //            //An apo xrewstiko paei se mh xrewstiko vgale to GuestId
                        //            if (newmakesTransfer == false || newaccount.Type == (int)AccountType.CreditCard)
                        //            {
                        //                t.Invoice_Guests_Trans.FirstOrDefault().GuestId = null;
                        //            }
                        //            curTransactiontoChange = t;
                        //            break;
                        //        }
                        //    }
                        //    t.AccountId = accountId;
                        //    CurAmount = t.Amount != null ? t.Amount.Value : 0;
                        //}
                        var newaccount = db.Accounts.FirstOrDefault(f => f.Id == acc.AccountId);
                        var newmakesTransfer = newaccount.SendsTransfer != null ? (bool)newaccount.SendsTransfer : false;
                        decimal TotalAmounts = invoice.Total.Value;//invoice.Transactions.Where(w => (w.IsDeleted ?? false) == false).Sum(s => s.Amount) != null ? invoice.Transactions.Where(w => (w.IsDeleted ?? false) == false).Sum(s => s.Amount).Value : 0;


                        // is cash to creditcard etc. sends no transfer
                        if (newmakesTransfer == false && oldaccistran == false)
                        {

                            //Put Cash TransferToPms
                            InsertTransferToPms(posid, orderdetails, invoice, acc, newtr, newaccount, TotalAmounts);

                            //IEnumerable<dynamic> query3 = EpimerismosDepartments(orderdetails,TotalAmounts,acc.Amount,null);
                            //var roomOfCC = "";
                            //roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == acc.AccountId && w.PosInfoId == posid).FirstOrDefault() != null ?
                            //        db.EODAccountToPmsTransfer.Where(w => w.AccountId == acc.AccountId && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                            //foreach (var g in query3)
                            //{
                            //    //Make new with correct amount (CASH)
                            //    TransferToPms tpms = new TransferToPms();

                            //    tpms.Description = "Rec: " + invoice.Counter + " Pos: " + invoice.PosInfoId + ", " + invoice.PosInfo.Description;
                            //    tpms.PmsDepartmentId = g.PmsDepartmentId;
                            //    tpms.PosInfoDetailId = invoice.PosInfoDetailId;//.Id;
                            //    tpms.ProfileId = null;
                            //    tpms.ProfileName = newaccount.Description;
                            //    tpms.ReceiptNo = invoice.Counter.ToString();//g.ReceiptNo.ToString();
                            //    tpms.RegNo = "0";
                            //    tpms.RoomDescription = roomOfCC;
                            //    tpms.RoomId = null;
                            //    tpms.SendToPMS = false;
                            //    tpms.SendToPmsTS = DateTime.Now;
                            //    //Set Status Flag (0: Cash, 1: Not Cash)
                            //    tpms.Status = (short)0;
                            //    //tpms.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                            //    tpms.TransferType = 0;//Xrewstiko
                            //    tpms.Total = (decimal)g.Total;
                            //    tpms.TransferIdentifier = Guid.NewGuid();
                            //    tpms.PmsDepartmentDescription = g.PmsDepDescription;
                            //    tpms.PosInfoId = invoice.PosInfoId;
                            //    db.TransferToPms.Add(tpms);

                            //    newtr.TransferToPms.Add(tpms);
                            //}
                        }

                        if (newmakesTransfer == false && oldaccistran == true) // old makes charge new does not
                        {
                            //Not Used here. Tha paei eksw apo to foreach gia na ginei mia fora
                            #region OLD CUSTOMER NEGATIVE TRANSACTION
                            /*
                            var oldguestid = long.Parse(prevcustId);
                            var oldguestRegNo = int.Parse(prevcustregno);
                            var oldguest = db.Guest.Where(f => f.Id == oldguestid && f.ReservationId == oldguestRegNo).FirstOrDefault();

                            foreach (var item in orderdetails)
                            {
                                var ss = from dd in item.OrderDetailInvoices
                                         join s in db.PosInfoDetail on dd.PosInfoDetailId equals s.Id
                                         select new { odinv = dd, isinv = s.IsInvoice };

                                item.GuestId = null;
                                foreach (var item1 in ss.Where(f => f.isinv == true))
                                {
                                    item1.odinv.CustomerId = "";
                                }
                            }

                            var hotel = db.HotelInfo.FirstOrDefault();
                            var query = (from f in orderdetails
                                         join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
                                         join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
                                         join st in db.SalesType on f.SalesTypeId equals st.Id
                                         join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
                                       equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                         from ls in loj.DefaultIfEmpty()
                                         select new
                                         {
                                             Id = f.Id,
                                             SalesTypeId = st.Id,
                                             Total = (decimal)f.TotalAfterDiscount + (decimal)(f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
                                             OrderDetail = f,
                                             PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                             PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                             // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
                                             ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
                                             PosId = f.Order.PosId,
                                             PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
                                             // TransactionId = f.TransactionId,
                                             PosName = f.Order.PosInfo.Description
                                             //  ReceiptNo = ls.

                                         }).Distinct()
                                         .GroupBy(g => g.PmsDepartmentId).Select(s => new
                                         {
                                             PmsDepartmentId = s.Key,
                                             PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                             Total = s.Sum(sm => sm.Total),
                                             OrderDetails = s.Select(ss => ss.OrderDetail),
                                             //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
                                             // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
                                             Guest = oldguest,
                                             ReceiptNo = s.FirstOrDefault().ReceiptNo,
                                             PosId = s.FirstOrDefault().PosId,
                                             PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                                             // TransactionId = s.FirstOrDefault().TransactionId,
                                             PosName = s.FirstOrDefault().PosName

                                         });

                            var IsCreditCard = false;
                            var roomOfCC = "";
                            if (oldaccount.Type == 4)
                            {
                                IsCreditCard = true;
                                roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
                                    db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                            }
                            if (IsCreditCard == false)//Einai xrewstiko alla oxi karta
                            {
                                if (curTransactiontoChange.Invoice_Guests_Trans.Count > 0)
                                {//Afairesh syndeshs Guest
                                    curTransactiontoChange.Invoice_Guests_Trans.FirstOrDefault().GuestId = null;
                                }
                            }
                            //EpimerismosPerDepartment
                            decimal totalDiscount = TotalAmounts - CurAmount;
                            decimal percentageEpim = 1 - (decimal)(CurAmount / TotalAmounts);
                            decimal totalEpim = 0;
                            decimal remainingDiscount = totalDiscount;
                            decimal ctr = 1;
                            List<dynamic> query2 = new List<dynamic>();
                            query = query.OrderBy(o => o.Total);
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
                                             ReceiptNo = q.ReceiptNo,
                                             Guest = q.Guest,
                                             PosId = q.PosId,
                                             PosInfoDetailId = q.PosInfoDetailId,
                                             PosName = q.PosName
                                         };
                            //

                            foreach (var g in query3)
                            {
                                decimal total = g.Total;//new Economic().EpimerisiAccountTotal(g.OrderDetails, (decimal)g.Total);
                                TransferToPms tpms = new TransferToPms();

                                tpms.Description = "Rec: " + g.ReceiptNo + ", Pos: " + g.PosId + ", " + g.PosName;
                                tpms.PmsDepartmentId = g.PmsDepartmentId;
                                tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
                                tpms.ProfileId = !IsCreditCard ? g.Guest.ProfileNo.ToString() : null;
                                tpms.ProfileName = !IsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : oldaccount.Description;
                                tpms.ReceiptNo = g.ReceiptNo.ToString();
                                tpms.RegNo = !IsCreditCard ? g.Guest.ReservationId.ToString() : "0";
                                tpms.RoomDescription = !IsCreditCard ? g.Guest.Room : roomOfCC;
                                tpms.RoomId = !IsCreditCard ? g.Guest.RoomId.ToString() : null;
                                tpms.SendToPMS = true;
                                tpms.SendToPmsTS = DateTime.Now;
                                tpms.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                                tpms.TransferType = 0;//Xrewstiko
                                tpms.Total = (decimal)total * (-1);  //negative
                                tpms.TransferIdentifier = Guid.NewGuid();
                                tpms.PmsDepartmentDescription = g.PmsDepDescription;

                                db.TransferToPms.Add(tpms);


                                TransferObject to = new TransferObject();
                                to.HotelId = (int)hotel.Id;
                                to.amount = (decimal)tpms.Total;
                                int PmsDepartmentId = 0;
                                var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
                                //
                                to.TransferIdentifier = tpms.TransferIdentifier;
                                //
                                to.departmentId = PmsDepartmentId;
                                to.description = tpms.Description;
                                to.profileName = tpms.ProfileName;
                                int resid = 0;
                                var toint = int.TryParse(tpms.RegNo, out resid);
                                to.resId = resid;
                                to.HotelUri = hotel.HotelUri;
                                to.pmsDepartmentDescription = g.PmsDepDescription;
                                if (IsCreditCard)
                                {
                                    to.RoomName = roomOfCC;
                                }
                                string res = "";

                                //string storeid = HttpContext.Current.Request.Params["storeid"];

                                // send only non zero transactions
                                if (to.amount != 0)
                                    objectsForPms.Add(to);

                                // 1
                                //  SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                            }
                            */
                            #endregion

                            //Put Cash TransferToPms
                            InsertTransferToPms(posid, orderdetails, invoice, acc, newtr, newaccount, TotalAmounts);
                        }
                        else if (newmakesTransfer == true && oldaccistran == false) // new makes transaction
                        {

                            #region MAKE TRANSACTION

                            //var newCusId = long.Parse(newcustId);
                            //var newCustResId = int.Parse(newcustregno);
                            if (acc.GuestId != null)
                            {
                                var guest1 = db.Guest.Find(acc.GuestId);//db.Guest.Where(f => f.Id == newCusId && f.ReservationId == newCustResId).FirstOrDefault();
                                foreach (var item in orderdetails)
                                {

                                    var ss = from dd in item.OrderDetailInvoices
                                             join s in db.PosInfoDetail on dd.PosInfoDetailId equals s.Id
                                             select new { odinv = dd, isinv = s.IsInvoice };

                                    if (guest1 != null)
                                    {
                                        item.GuestId = guest1.Id;
                                    }
                                    foreach (var item1 in ss.Where(f => f.isinv == true))
                                    {
                                        item1.odinv.CustomerId = newcustId;
                                    }
                                }
                            }

                            var hotel = db.HotelInfo.FirstOrDefault();
                            /*
                            var query = (from f in orderdetails
                                         join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
                                         join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
                                         join st in db.SalesType on f.SalesTypeId equals st.Id
                                         join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
                                       equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                         from ls in loj.DefaultIfEmpty()
                                         select new
                                         {
                                             Id = f.Id,
                                             SalesTypeId = st.Id,
                                             Total = (decimal)f.TotalAfterDiscount + (decimal)(f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
                                             OrderDetail = f,
                                             PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                             PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                             // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
                                             ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
                                             PosId = f.Order.PosId,
                                             PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
                                             //TransactionId = f.TransactionId,
                                             PosName = f.Order.PosInfo.Description
                                             //  ReceiptNo = ls.

                                         }).Distinct()
                                         .GroupBy(g => g.PmsDepartmentId).Select(s => new
                                         {
                                             PmsDepartmentId = s.Key,
                                             PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                             Total = s.Sum(sm => sm.Total),
                                             OrderDetails = s.Select(ss => ss.OrderDetail),
                                             //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
                                             // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
                                             //Guest = guest,
                                             ReceiptNo = s.FirstOrDefault().ReceiptNo,
                                             PosId = s.FirstOrDefault().PosId,
                                             PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                                             //TransactionId = s.FirstOrDefault().TransactionId,
                                             PosName = s.FirstOrDefault().PosName

                                         });
                             * */
                            var IsCreditCard = false;
                            var roomOfCC = "";
                            if (newaccount.Type == 4)
                            {
                                IsCreditCard = true;
                                roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
                                    db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                            }

                            var guest = !IsCreditCard && acc.GuestId != null ? db.Guest.Find(acc.GuestId) : null;
                            //if (IsCreditCard == false)
                            //{
                            //    if (curTransactiontoChange.Invoice_Guests_Trans.Count > 0)
                            //    {
                            //        curTransactiontoChange.Invoice_Guests_Trans.FirstOrDefault().GuestId = guest.Id;
                            //    }
                            //    else
                            //    {
                            //        Invoice_Guests_Trans assoc = new Invoice_Guests_Trans();
                            //        assoc.GuestId = guest.Id;
                            //        assoc.InvoiceId = curTransactiontoChange.InvoicesId;
                            //        assoc.TransactionId = curTransactiontoChange.Id;
                            //        db.Invoice_Guests_Trans.Add(assoc);
                            //    }
                            //}

                            /*
                            //EpimerismosPerDepartment
                            CurAmount = acc.Amount;
                            decimal totalDiscount = TotalAmounts - CurAmount;
                            decimal percentageEpim = 1 - (decimal)(CurAmount / TotalAmounts);
                            decimal totalEpim = 0;
                            decimal remainingDiscount = totalDiscount;
                            decimal ctr = 1;
                            List<dynamic> query2 = new List<dynamic>();
                            query = query.OrderBy(o => o.Total);
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
                                             ReceiptNo = q.ReceiptNo,
                                             Guest = guest,//q.Guest,
                                             PosId = q.PosId,
                                             PosInfoDetailId = q.PosInfoDetailId,
                                             PosName = q.PosName
                                         };
                            //
                             * */
                            IEnumerable<dynamic> query3 = EpimerismosDepartments(orderdetails, TotalAmounts, acc.Amount, guest);
                            foreach (var g in query3)
                            {
                                decimal total = g.Total;//new Economic().EpimerisiAccountTotal(g.OrderDetails, g.Total);
                                TransferToPms tpms = new TransferToPms();

                                tpms.Description = "Rec: " + g.ReceiptNo + " Pos: " + g.PosId + ", " + g.PosName;
                                tpms.PmsDepartmentId = g.PmsDepartmentId;
                                tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
                                tpms.ProfileId = !IsCreditCard ? g.Guest.ProfileNo.ToString() : null;
                                tpms.ProfileName = !IsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : newaccount.Description;
                                tpms.ReceiptNo = g.ReceiptNo.ToString();
                                tpms.RegNo = !IsCreditCard ? g.Guest.ReservationId.ToString() : "0";
                                tpms.RoomDescription = !IsCreditCard ? g.Guest.Room : roomOfCC;
                                tpms.RoomId = !IsCreditCard ? g.Guest.RoomId.ToString() : null;
                                tpms.SendToPMS = true;
                                tpms.SendToPmsTS = DateTime.Now;
                                //tpms.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                                tpms.TransferType = 0;//Xrewstiko
                                tpms.Total = total;//(decimal)g.Total;
                                tpms.TransferIdentifier = Guid.NewGuid();
                                tpms.PmsDepartmentDescription = g.PmsDepDescription;
                                tpms.PosInfoId = g.PosId;
                                db.TransferToPms.Add(tpms);

                                newtr.TransferToPms.Add(tpms);

                                TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, IsCreditCard, roomOfCC, tpms, tpms.TransferIdentifier.Value);
                                to.pmsDepartmentDescription = g.PmsDepDescription;

                                // string storeid = HttpContext.Current.Request.Params["storeid"];

                                // send only non zero transactions
                                if (to.amount != 0)
                                    objectsForPms.Add(to);

                                // 1
                                //  SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);

                            }
                            #endregion

                        }
                        else if (newmakesTransfer == true && oldaccistran == true) // from customer to customer
                        {
                            var hotel = db.HotelInfo.FirstOrDefault();

                            //Not Used here. Tha paei eksw apo to foreach gia na ginei mia fora
                            #region OLD CUSTOMER NEGATIVE TRANSACTION
                            /*
                            var oldguestid = long.Parse(prevcustId);
                            var oldGuestRegNo = int.Parse(prevcustregno);
                            var oldguest = db.Guest.Where(f => f.Id == oldguestid && f.ReservationId == oldGuestRegNo).FirstOrDefault();

                            var query = (from f in orderdetails
                                         join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
                                         join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
                                         join st in db.SalesType on f.SalesTypeId equals st.Id
                                         join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
                                       equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                         from ls in loj.DefaultIfEmpty()
                                         select new
                                         {
                                             Id = f.Id,
                                             SalesTypeId = st.Id,
                                             Total = (decimal)f.TotalAfterDiscount + (decimal)(f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
                                             OrderDetail = f,
                                             PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                             PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                             // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
                                             ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
                                             PosId = f.Order.PosId,
                                             PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
                                             //TransactionId = f.TransactionId,
                                             PosName = f.Order.PosInfo.Description
                                             //  ReceiptNo = ls.

                                         }).Distinct()
                                         .GroupBy(g => g.PmsDepartmentId).Select(s => new
                                         {
                                             PmsDepartmentId = s.Key,
                                             PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                             Total = s.Sum(sm => sm.Total),
                                             OrderDetails = s.Select(ss => ss.OrderDetail),
                                             //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
                                             // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
                                             Guest = oldguest,
                                             ReceiptNo = s.FirstOrDefault().ReceiptNo,
                                             PosId = s.FirstOrDefault().PosId,
                                             PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                                             //TransactionId = s.FirstOrDefault().TransactionId,
                                             PosName = s.FirstOrDefault().PosName,
                                         });
                            var oldIsCreditCard = false;
                            var oldroomOfCC = "";
                            if (oldaccount.Type == 4)
                            {
                                oldIsCreditCard = true;
                                oldroomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
                                    db.EODAccountToPmsTransfer.Where(w => w.AccountId == oldaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                            }
                            if (oldIsCreditCard == false)//Einai xrewstiko alla oxi karta
                            {
                                if (curTransactiontoChange.Invoice_Guests_Trans.Count > 0)
                                {
                                    curTransactiontoChange.Invoice_Guests_Trans.FirstOrDefault().GuestId = null;
                                }
                            }
                            //EpimerismosPerDepartment
                            decimal totalDiscount = TotalAmounts - CurAmount;
                            decimal percentageEpim = 1 - (decimal)(CurAmount / TotalAmounts);
                            decimal totalEpim = 0;
                            decimal remainingDiscount = totalDiscount;
                            decimal ctr = 1;
                            List<dynamic> query2 = new List<dynamic>();
                            query = query.OrderBy(o => o.Total);
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
                                             ReceiptNo = q.ReceiptNo,
                                             Guest = q.Guest,
                                             PosId = q.PosId,
                                             PosInfoDetailId = q.PosInfoDetailId,
                                             PosName = q.PosName
                                         };
                            //
                            foreach (var g in query3)
                            {
                                decimal total = g.Total;//new Economic().EpimerisiAccountTotal(g.OrderDetails, g.Total);
                                TransferToPms tpms = new TransferToPms();

                                tpms.Description = "Rec: " + g.ReceiptNo + ", Pos: " + g.PosId + ", " + g.PosName;
                                tpms.PmsDepartmentId = g.PmsDepartmentId;
                                tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
                                tpms.ProfileId = !oldIsCreditCard ? g.Guest.ProfileNo.ToString() : null;
                                tpms.ProfileName = !oldIsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : oldaccount.Description;
                                tpms.ReceiptNo = g.ReceiptNo.ToString();
                                tpms.RegNo = !oldIsCreditCard ? g.Guest.ReservationId.ToString() : "0";
                                tpms.RoomDescription = !oldIsCreditCard ? g.Guest.Room : oldroomOfCC;
                                tpms.SendToPmsTS = DateTime.Now;
                                tpms.RoomId = !oldIsCreditCard ? g.Guest.RoomId.ToString() : null;
                                tpms.SendToPMS = true;
                                tpms.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                                tpms.TransferType = 0;//Xrewstiko
                                tpms.Total = (decimal)total * (-1);  //negative
                                tpms.TransferIdentifier = Guid.NewGuid();
                                tpms.PmsDepartmentDescription = g.PmsDepDescription;

                                db.TransferToPms.Add(tpms);


                                TransferObject to = new TransferObject();
                                to.HotelId = (int)hotel.Id;
                                to.amount = (decimal)tpms.Total;
                                int PmsDepartmentId = 0;
                                var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
                                //
                                to.TransferIdentifier = tpms.TransferIdentifier;
                                //
                                to.departmentId = PmsDepartmentId;
                                to.description = tpms.Description;
                                to.profileName = tpms.ProfileName;
                                int resid = 0;
                                var toint = int.TryParse(tpms.RegNo, out resid);
                                to.resId = resid;
                                to.HotelUri = hotel.HotelUri;
                                to.pmsDepartmentDescription = g.PmsDepDescription;
                                if (oldIsCreditCard)
                                {
                                    to.RoomName = oldroomOfCC;
                                }
                                string res = "";

                                // string storeid = HttpContext.Current.Request.Params["storeid"];

                                // send only non zero transactions
                                if (to.amount != 0)
                                    objectsForPms.Add(to);

                                // 1
                                //  SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                //  sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);

                            }
                            */
                            #endregion


                            #region NEW CUSTOMER TRANSFER

                            //var newCusId = long.Parse(newcustId);
                            //var newCustRegNo = int.Parse(newcustregno);
                            if (acc.GuestId != null)
                            {
                                var newguest1 = db.Guest.Find(acc.GuestId);
                                foreach (var item in orderdetails)
                                {
                                    var ss = from dd in item.OrderDetailInvoices
                                             join s in db.PosInfoDetail on dd.PosInfoDetailId equals s.Id
                                             select new { odinv = dd, isinv = s.IsInvoice };
                                    if (newguest1 != null)
                                    {
                                        item.GuestId = newguest1.Id;
                                    }
                                    foreach (var item1 in ss.Where(f => f.isinv == true))
                                    {
                                        item1.odinv.CustomerId = newcustId;
                                    }
                                }
                            }
                            /*
                            var query1 = (from f in orderdetails
                                          join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
                                          join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
                                          join st in db.SalesType on f.SalesTypeId equals st.Id
                                          join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
                                        equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                          from ls in loj.DefaultIfEmpty()
                                          select new
                                          {
                                              Id = f.Id,
                                              SalesTypeId = st.Id,
                                              Total = (decimal)f.TotalAfterDiscount + (decimal)(f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
                                              PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                              PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                              OrderDetail = f,
                                              // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
                                              ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
                                              PosId = f.Order.PosId,
                                              PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
                                              //TransactionId = f.TransactionId,
                                              PosName = f.Order.PosInfo.Description,
                                              //  ReceiptNo = ls.

                                          }).Distinct()
                                               .GroupBy(g => g.PmsDepartmentId).Select(s => new
                                               {
                                                   PmsDepartmentId = s.Key,
                                                   PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                                   Total = s.Sum(sm => sm.Total),
                                                   OrderDetails = s.Select(ss => ss.OrderDetail),
                                                   //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
                                                   // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
                                                   //Guest = newguest,
                                                   ReceiptNo = s.FirstOrDefault().ReceiptNo,
                                                   PosId = s.FirstOrDefault().PosId,
                                                   PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                                                   //TransactionId = s.FirstOrDefault().TransactionId,
                                                   PosName = s.FirstOrDefault().PosName

                                               });
                             * */
                            var newIsCreditCard = false;
                            var newroomOfCC = "";
                            if (newaccount.Type == 4)
                            {
                                newIsCreditCard = true;
                                newroomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault() != null ?
                                    db.EODAccountToPmsTransfer.Where(w => w.AccountId == newaccount.Id && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                            }
                            var newguest = !newIsCreditCard && acc.GuestId != null ? db.Guest.Find(acc.GuestId) : null;
                            //if (newIsCreditCard == false)//Einai xrewstiko alla den einai karta
                            //{
                            //    if (curTransactiontoChange.Invoice_Guests_Trans.Count > 0)
                            //    {
                            //        curTransactiontoChange.Invoice_Guests_Trans.FirstOrDefault().GuestId = newguest.Id;
                            //    }
                            //    else
                            //    {
                            //        Invoice_Guests_Trans assoc = new Invoice_Guests_Trans();
                            //        assoc.GuestId = newguest.Id;
                            //        assoc.InvoiceId = curTransactiontoChange.InvoicesId;
                            //        assoc.TransactionId = curTransactiontoChange.Id;
                            //        db.Invoice_Guests_Trans.Add(assoc);
                            //    }
                            //}
                            //EpimerismosPerDepartment
                            /*
                            CurAmount = acc.Amount;
                            decimal totalDiscount1 = TotalAmounts - CurAmount;
                            decimal percentageEpim1 = 1 - (decimal)(CurAmount / TotalAmounts);
                            decimal totalEpim1 = 0;
                            decimal remainingDiscount1 = totalDiscount1;
                            decimal ctr1 = 1;
                            List<dynamic> query12 = new List<dynamic>();
                            query1 = query1.OrderBy(o => o.Total);
                            foreach (var f in query1)
                            {
                                if (ctr1 < query1.Count())
                                {
                                    decimal subtotal = f.Total;
                                    decimal percSub = Math.Round((decimal)(subtotal * percentageEpim1), 2);
                                    totalEpim1 += subtotal - percSub;
                                    query12.Add(new
                                    {
                                        PmsDepartmentId = f.PmsDepartmentId,
                                        Total = subtotal - percSub
                                    });
                                    remainingDiscount1 = remainingDiscount1 - percSub;
                                }
                                else
                                {
                                    decimal subtotal = f.Total;
                                    query12.Add(new
                                    {
                                        PmsDepartmentId = f.PmsDepartmentId,
                                        Total = subtotal - remainingDiscount1
                                    });
                                    totalEpim1 += subtotal - remainingDiscount1;
                                }
                                ctr1++;
                            }
                            //
                            var query13 = from q in query1
                                          join j in query12 on q.PmsDepartmentId equals j.PmsDepartmentId
                                          select new
                                          {
                                              PmsDepartmentId = q.PmsDepartmentId,
                                              PmsDepDescription = q.PmsDepDescription,
                                              Total = j.Total,
                                              OrderDetails = q.OrderDetails,
                                              ReceiptNo = q.ReceiptNo,
                                              Guest = newguest,//q.Guest,
                                              PosId = q.PosId,
                                              PosInfoDetailId = q.PosInfoDetailId,
                                              PosName = q.PosName
                                          };
                            //
                             * */
                            IEnumerable<dynamic> query13 = EpimerismosDepartments(orderdetails, TotalAmounts, acc.Amount, newguest);
                            foreach (var g in query13)
                            {
                                decimal total = g.Total;//new Economic().EpimerisiAccountTotal(g.OrderDetails, g.Total);
                                TransferToPms tpms = new TransferToPms();

                                tpms.Description = "Rec: " + g.ReceiptNo + ", Pos: " + g.PosId + ", " + g.PosName;
                                tpms.PmsDepartmentId = g.PmsDepartmentId;
                                tpms.PosInfoDetailId = g.PosInfoDetailId;//.Id;
                                tpms.ProfileId = !newIsCreditCard ? g.Guest.ProfileNo.ToString() : null;
                                tpms.ProfileName = !newIsCreditCard ? g.Guest.FirstName + " " + g.Guest.LastName : newaccount.Description;
                                tpms.ReceiptNo = g.ReceiptNo.ToString();
                                tpms.RegNo = !newIsCreditCard ? g.Guest.ReservationId.ToString() : "0";
                                tpms.RoomDescription = !newIsCreditCard ? g.Guest.Room : newroomOfCC;
                                tpms.RoomId = !newIsCreditCard ? g.Guest.RoomId.ToString() : null;
                                tpms.SendToPmsTS = DateTime.Now;
                                tpms.SendToPMS = true;
                                //tpms.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                                tpms.TransferType = 0;//Xrewstiko
                                tpms.Total = total;//(decimal)g.Total;
                                tpms.TransferIdentifier = Guid.NewGuid();
                                tpms.PmsDepartmentDescription = g.PmsDepDescription;
                                tpms.PosInfoId = g.PosId;
                                db.TransferToPms.Add(tpms);

                                newtr.TransferToPms.Add(tpms);

                                TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, newIsCreditCard, newroomOfCC, tpms, tpms.TransferIdentifier.Value);
                                to.pmsDepartmentDescription = g.PmsDepDescription;

                                // send only non zero transactions
                                if (to.amount != 0)
                                    objectsForPms.Add(to);

                                // 1
                                // SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);

                            }
                            #endregion

                        }

                        db.SaveChanges();

                        string storeid = HttpContext.Current.Request.Params["storeid"];
                        // MAKE ALL TRANSACTIONS
                        foreach (var to in objectsForPms)
                        {
                            SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                            //sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                        }
                        newTransactions.Add(newtr);
                    }
                    foreach (var tr in curTransactiontoChange)
                    {
                        tr.IsDeleted = true;
                        if (tr.TransferToPms.Count > 0)//oldaccistran == true
                        {
                            var hotel = db.HotelInfo.FirstOrDefault();
                            List<TransferObject> objTosendList = new List<TransferObject>();
                            List<TransferToPms> transferNegative = tr.TransferToPms.ToList();
                            foreach (var otpms in transferNegative)
                            {
                                if (otpms.Status != 0 && otpms.Status != 3)//Den einai Cash h akyrwmena Cash
                                {
                                    TransferToPms tpms = new TransferToPms(); // newCounter
                                    tpms.Description = otpms.Description;
                                    tpms.PmsDepartmentId = otpms.PmsDepartmentId;
                                    tpms.PosInfoDetailId = otpms.PosInfoDetailId;
                                    tpms.ProfileId = otpms.ProfileId;
                                    tpms.ProfileName = otpms.ProfileName;
                                    tpms.ReceiptNo = otpms.ReceiptNo;
                                    tpms.RegNo = otpms.RegNo;
                                    tpms.RoomDescription = otpms.RoomDescription;
                                    tpms.RoomId = otpms.RoomId;
                                    tpms.SendToPMS = true;
                                    tpms.PmsDepartmentDescription = otpms.PmsDepartmentDescription;
                                    tpms.TransactionId = otpms.TransactionId;
                                    tpms.TransferType = 0;//Xrewstiko
                                    tpms.Total = (decimal)otpms.Total * -1;
                                    tpms.SendToPmsTS = DateTime.Now;
                                    var identifier = Guid.NewGuid();
                                    tpms.TransferIdentifier = identifier;
                                    tpms.PosInfoId = otpms.PosInfoId;
                                    db.TransferToPms.Add(tpms);

                                    TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, false, "", tpms, tpms.TransferIdentifier.Value);
                                    to.RoomName = tpms.RoomDescription;

                                    if (to.amount != 0)
                                        objTosendList.Add(to);
                                }
                                else
                                {
                                    //Akyrwse ta metrita kai xanavalta me to swsto Amount
                                    //Set Status Flag (0: Send Cash, !0: Do Not Send Cash)
                                    otpms.Status = (short)3; //DO NOT SEND at the end of day
                                }
                            }

                            string storeid = HttpContext.Current.Request.Params["storeid"];
                            foreach (var to in objTosendList)
                            {
                                SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                //sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                            }
                        }
                    }
                    foreach (var t in newTransactions)
                    {
                        db.Transactions.Add(t);
                    }
                }
                db.SaveChanges();
                return new object();

            }

           

        }

        private void InsertTransferToPms(long posid, List<OrderDetail> orderdetails, Invoices invoice, AccountsObj acc, Transactions newtr, Accounts newaccount, decimal TotalAmounts)
        {
            using (PosEntities db = new PosEntities(false))
            {
                IEnumerable<dynamic> query3 = EpimerismosDepartments(orderdetails, TotalAmounts, acc.Amount, null);
                var roomOfCC = "";
                roomOfCC = db.EODAccountToPmsTransfer.Where(w => w.AccountId == acc.AccountId && w.PosInfoId == posid).FirstOrDefault() != null ?
                        db.EODAccountToPmsTransfer.Where(w => w.AccountId == acc.AccountId && w.PosInfoId == posid).FirstOrDefault().PmsRoom.ToString() : "-1";
                foreach (var g in query3)
                {
                    //Make new with correct amount (CASH)
                    TransferToPms tpms = new TransferToPms();

                    // tpms.Description = "Rec: " + invoice.Counter + " Pos: " + invoice.PosInfoId + ", " + invoice.PosInfo.Description;
                    tpms.Description = "R:" + invoice.Counter + " P:" + invoice.PosInfoId + " O:" + orderdetails.FirstOrDefault().OrderId + ", " + invoice.PosInfo.Description;
                    tpms.PmsDepartmentId = g.PmsDepartmentId;
                    tpms.PosInfoDetailId = invoice.PosInfoDetailId;//.Id;
                    tpms.ProfileId = null;
                    tpms.ProfileName = newaccount.Description;
                    tpms.ReceiptNo = invoice.Counter.ToString();//g.ReceiptNo.ToString();
                    tpms.RegNo = "0";
                    tpms.RoomDescription = roomOfCC;
                    tpms.RoomId = null;
                    tpms.SendToPMS = false;
                    tpms.SendToPmsTS = DateTime.Now;
                    //Set Status Flag (0: Cash, 1: Not Cash)
                    tpms.Status = (short)0;
                    //tpms.TransactionId = curTransactiontoChange.Id;//g.TransactionId;
                    tpms.TransferType = 0;//Xrewstiko
                    tpms.Total = (decimal)g.Total;
                    tpms.TransferIdentifier = Guid.NewGuid();
                    tpms.PmsDepartmentDescription = g.PmsDepDescription;
                    tpms.PosInfoId = invoice.PosInfoId;
                    db.TransferToPms.Add(tpms);

                    newtr.TransferToPms.Add(tpms);
                }

            }
          
        }

        private IEnumerable<dynamic> EpimerismosDepartments(IEnumerable<OrderDetail> orderdetails, decimal TotalAmounts, decimal CurAmount, Guest guest)
        {
            using (PosEntities db = new PosEntities(false))
            {
                var query = (from f in orderdetails
                             join e in db.PosInfo on f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetail.PosInfoId equals e.Id
                             join pr in db.PricelistDetail on f.PricelistDetail.PricelistId equals pr.PricelistId
                             join st in db.SalesType on f.SalesTypeId equals st.Id
                             join tm in db.TransferMappings.AsNoTracking() on new { Product = f.ProductId, Pricelist = pr.PricelistId, PosDepartmentId = e.DepartmentId }//Order.PosInfo.DepartmentId }
                           equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                             from ls in loj.DefaultIfEmpty()
                             select new
                             {
                                 Id = f.Id,
                                 SalesTypeId = st.Id,
                                 Total = (decimal)f.TotalAfterDiscount + (decimal)(f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count() > 0 ? f.OrderDetailIgredients.Sum(sm => sm.TotalAfterDiscount) : 0),
                                 OrderDetail = f,
                                 PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                 PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                 // Guest = f.Guest, // f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().CustomerId,
                                 ReceiptNo = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().Counter,
                                 PosId = f.Order.PosId,
                                 PosInfoDetailId = f.OrderDetailInvoices.Where(w => w.PosInfoDetail.IsInvoice == true).FirstOrDefault().PosInfoDetailId,
                                 //TransactionId = f.TransactionId,
                                 PosName = f.Order.PosInfo.Description
                                 //  ReceiptNo = ls.

                             }).Distinct().ToList()
                                   .GroupBy(g => g.PmsDepartmentId).Select(s => new
                                   {
                                       PmsDepartmentId = s.Key,
                                       PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                       Total = s.Sum(sm => sm.Total),
                                       OrderDetails = s.Select(ss => ss.OrderDetail),
                                         //    CustomerId = s.FirstOrDefault().Guest.CustomerId, 
                                         // Guest = s.FirstOrDefault().Guest, //db.Guest.ToList().Where(x => x.Id.ToString() == s.FirstOrDefault().Guest).FirstOrDefault(),
                                         //Guest = guest,
                                         ReceiptNo = s.FirstOrDefault().ReceiptNo,
                                       PosId = s.FirstOrDefault().PosId,
                                       PosInfoDetailId = s.FirstOrDefault().PosInfoDetailId,
                                         //TransactionId = s.FirstOrDefault().TransactionId,
                                         PosName = s.FirstOrDefault().PosName

                                   });


                decimal totalDiscount = TotalAmounts - CurAmount;
                decimal percentageEpim = 1 - (decimal)(CurAmount / TotalAmounts);
                decimal totalEpim = 0;
                decimal remainingDiscount = totalDiscount;
                decimal ctr = 1;
                List<dynamic> query2 = new List<dynamic>();
                query = query.OrderBy(o => o.Total);
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
                                 ReceiptNo = q.ReceiptNo,
                                 Guest = guest,//q.Guest,
                                 PosId = q.PosId,
                                 PosInfoDetailId = q.PosInfoDetailId,
                                 PosName = q.PosName
                             };
                return query3;

            }

          
        }
        #endregion

        [HttpGet]
        public object ManuallyCreatePmsTransfer(int resId, string profileName, string description,
                int departmentId, decimal amount, string roomName, int rowId)
        {
            using (PosEntities db = new PosEntities(false))
            {
                var hotel = db.HotelInfo.FirstOrDefault();
                var savedTransfer = db.TransferToPms.Where(f => f.Id == rowId).FirstOrDefault();

                if (savedTransfer != null)
                {
                    var depidstr = departmentId.ToString();
                    var newPmsDep = db.TransferMappings.Where(f => f.PmsDepartmentId == depidstr).FirstOrDefault();
                    if (newPmsDep == null) // department not found
                        return 0;

                    string storeid = HttpContext.Current.Request.Params["storeid"];

                    savedTransfer.Description = description;
                    savedTransfer.Total = amount;
                    savedTransfer.ProfileName = profileName;
                    savedTransfer.PmsDepartmentId = departmentId.ToString();
                    savedTransfer.PmsDepartmentDescription = newPmsDep.PmsDepDescription;


                    //manual
                    TransferObject to = new TransferObject();
                    to.HotelId = (int)hotel.HotelId;
                    to.amount = (decimal)amount;
                    to.departmentId = departmentId;
                    to.description = description;
                    to.pmsDepartmentDescription = savedTransfer.PmsDepartmentDescription;
                    to.profileName = profileName;
                    to.resId = resId;
                    to.HotelUri = hotel.HotelUri;
                    to.TransferIdentifier = savedTransfer.TransferIdentifier;
                    // to.pmsDepartmentDescription = g.PmsDepDescription;

                    db.SaveChanges();

                    SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                    //sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);

                    return 1;
                }
                else
                {
                    return 0;
                }

            }
          
        }

        private delegate TranferResultModel SendTransfer(Pos_WebApi.Models.TransferObject tpms, string storeid);


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
                        //  originalTransfer.SendToPmsTS = DateTime.Now;
                        originalTransfer.ErrorMessage = res.TransferErrorMessage;
                        originalTransfer.PmsResponseId = res.TransferResponseId;
                        //   originalTransfer.PmsDepartmentDescription = res.TransferObj.pmsDepartmentDescription;

                    }

                    ctx.SaveChanges();
                }
            }

        }




        // test updates transfer to pms record with result ///

        public void GetTransactions(string storeid, TranferResultModel res)
        {
            using (PosEntities db = new PosEntities(false))
            {
                if (res != null && res.TransferObj != null)
                {
                    try
                    {
                        var originalTransfer = db.TransferToPms.FirstOrDefault(f => f.TransferIdentifier == res.TransferObj.TransferIdentifier);

                        if (originalTransfer != null)
                        {
                            originalTransfer.SendToPmsTS = DateTime.Now;
                            originalTransfer.ErrorMessage = res.TransferErrorMessage;
                            originalTransfer.PmsResponseId = res.TransferResponseId;
                            // db.TransferToPms.Attach(originalTransfer);
                            // db.TransferToPms.Attach(originalTransfer);
                            db.SaveChanges();
                        }  //}
                    }
                    finally
                    {
                        // dbs = null;
                        // objectContext.Connection.Close();
                    }
                }

            }

           
        }


        //BACKOFFICE get Receipt edit LOKUPS
        public object GetReceiptLookups(bool forBackoffice, bool lookups)
        {
            using (PosEntities db = new PosEntities(false))
            {
                var posDepartments = db.Department.Select(f => new { Id = f.Id, Description = f.Description }).OrderBy(d => d.Description).AsEnumerable();

                var invoiceTypes = db.PosInfoDetail.Where(f => f.InvoiceId != null).Select(f => new { InvId = f.InvoiceId, Abbr = f.Abbreviation, Description = f.Description, PosId = f.PosInfoId, PosDescription = f.PosInfo != null ? f.PosInfo.Description : "No name" }).Distinct()
                    .OrderBy(d => d.Description).AsEnumerable();

                return this.Request.CreateResponse(HttpStatusCode.OK, new { PosDepartments = posDepartments, InvoiceTypes = invoiceTypes });

            }
          
        }

        ////////////////////////



        // GET api/Transaction/5
        public Transactions GetTransactions(long id)
        {
            using (PosEntities db = new PosEntities(false))
            {
                Transactions transactions = db.Transactions.Find(id);
                if (transactions == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                return transactions;

            }
           
        }

        // PUT api/Transaction/5
        public HttpResponseMessage PutTransactions(long id, Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != transactions.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            using (PosEntities db = new PosEntities(false))
            {
                db.Entry(transactions).State = EntityState.Modified;

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
           
        }

        // POST api/Transaction
        public HttpResponseMessage PostTransactions(Transactions transactions)
        {
            if (ModelState.IsValid)
            {
                //FILL AMOUNTS
                using (PosEntities db = new PosEntities(false))
                {
                    try
                    {
                        transactions = new Economic().SetEconomicNumbers(transactions, null, db, null);

                        if (transactions.TransactionType == (int)TransactionTypesEnum.Cancel)
                        {
                            try
                            {
                                var firstTransaction = db.Transactions.Where(w => w.OrderId == transactions.OrderId).FirstOrDefault();
                                if (firstTransaction != null)
                                {
                                    transactions.Gross = firstTransaction.Gross * -1;
                                    transactions.Net = firstTransaction.Net * -1;
                                    transactions.Tax = firstTransaction.Tax * -1;
                                    transactions.Vat = firstTransaction.Vat * -1;
                                    db.Transactions.Add(transactions);
                                }
                            }
                            catch (Exception exx)
                            {
                                logger.Error(exx.ToString());
                                Request.CreateErrorResponse(HttpStatusCode.NotFound, exx);
                            }
                        }
                        else
                        {
                            if (transactions.TransactionType == (int)TransactionTypesEnum.Antilogismos)
                            {
                                transactions.Gross = transactions.Amount;
                                //TODO 
                                //transactions.Net = ;
                                //transactions.Tax = firstTransaction.Tax * -1;
                                //transactions.Vat = firstTransaction.Vat * -1;
                            }
                            if (transactions.TransactionType != (int)TransactionTypesEnum.Sale && transactions.TransactionType != (int)TransactionTypesEnum.Antilogismos)
                            {
                                transactions.Gross = transactions.Amount;
                            }
                            db.Transactions.Add(transactions);
                        }

                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                    }
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, transactions);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = transactions.Id }));
                    return response;

                }
               
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }



        // DELETE api/Transaction/5
        public HttpResponseMessage DeleteTransactions(long id)
        {
            using (PosEntities db = new PosEntities(false))
            {
                Transactions transactions = db.Transactions.Find(id);
                if (transactions == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                db.Transactions.Remove(transactions);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                return Request.CreateResponse(HttpStatusCode.OK, transactions);

            }
          
        }

        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
           // db.Dispose();
            base.Dispose(disposing);
        }
    }
}