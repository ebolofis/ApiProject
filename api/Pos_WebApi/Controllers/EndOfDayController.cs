//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web;
//using System.Web.Http;
//using System.Data.Common;
//using Pos_WebApi.Helpers;
//using Pos_WebApi.Models;
//using log4net;

//namespace Pos_WebApi.Controllers
//{
//    public class EndOfDayController : ApiController
//    {
//        private PosEntities db = new PosEntities(false);
//        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

//        // GET api/EndOfDay
//        public IEnumerable<EndOfDay> GetEndOfDay(string storeid)
//        {

//            return db.EndOfDay.AsEnumerable();
//        }

//        // GET api/EndOfDay/5
//        public EndOfDay GetEndOfDay(long id, string storeid)
//        {
//            EndOfDay EndOfDay = db.EndOfDay.Find(id);
//            if (EndOfDay == null)
//            {
//                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
//            }

//            return EndOfDay;
//        }

//        // PUT api/EndOfDay/5
//        public HttpResponseMessage PutEndOfDay(long id, string storeid, EndOfDay EndOfDay)
//        {
//            if (!ModelState.IsValid)
//            {
//                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
//            }

//            if (id != EndOfDay.Id)
//            {
//                return Request.CreateResponse(HttpStatusCode.BadRequest);
//            }

//            db.Entry(EndOfDay).State = EntityState.Modified;

//            try
//            {
//                db.SaveChanges();
//            }
//            catch (DbUpdateConcurrencyException ex)
//            {
//                logger.Error(ex.ToString());
//                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
//            }

//            return Request.CreateResponse(HttpStatusCode.OK, EndOfDay);
//        }



//        // POST api/EndOfDay
//        #region old PostEndOfDDay Before Invoice Transactions
//        //public HttpResponseMessage PostEndOfDay(EndOfDay EndOfDay)
//        //{
//        //    if (ModelState.IsValid)
//        //    {
//        //        //using (var tx = db.BeginTransaction())
//        //        //{


//        //        PosInfo pos = db.PosInfo.Where(w => w.Id == EndOfDay.PosInfoId).FirstOrDefault();
//        //        //pos.FODay = pos.FODay != null ? pos.FODay.Value.AddDays(1) : DateTime.Today;
//        //        pos.IsOpen = false;
//        //        pos.CloseId = EndOfDay.CloseId;

//        //        EndOfDay.FODay = pos.FODay;
//        //        db.EndOfDay.Add(EndOfDay);

//        //        db.SaveChanges();
//        //        decimal? totalGross = 0.0m;
//        //        decimal? totalNet = 0.0m;
//        //        decimal? totalVatAmount = 0.0m;
//        //        decimal? totalTaxAmount = 0.0m;
//        //        decimal? totalDiscount = 0.0m;
//        //        var invoices = db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == EndOfDay.PosInfoId);
//        //        foreach (var t in invoices)
//        //        {
//        //            t.EndOfDayId = EndOfDay.Id;
//        //            db.Entry(t).State = EntityState.Modified;
//        //        }
//        //        var transactions = db.Transactions.Where(w => w.EndOfDayId == null && w.PosInfoId == EndOfDay.PosInfoId);
//        //        foreach (var t in transactions)
//        //        {
//        //            t.EndOfDayId = EndOfDay.Id;
//        //            db.Entry(t).State = EntityState.Modified;
//        //        }
//        //        var orders = db.Order.Where(w => w.EndOfDayId == null && w.PosId == EndOfDay.PosInfoId);
//        //        foreach (var o in orders)
//        //        {
//        //            o.EndOfDayId = EndOfDay.Id;
//        //            db.Entry(o).State = EntityState.Modified;
//        //        }
//        //        var vatAnals = db.OrderDetailVatAnal.Include("OrderDetail.Transactions")
//        //                                            .Include("OrderDetail.Order")
//        //                                            .Include("OrderDetail.Order.Transactions")
//        //                                            .Where(w => w.OrderDetail.Order.EndOfDayId == null && w.OrderDetail.TransactionId != null
//        //                                                            && w.OrderDetail.Order.PosId == pos.Id)
//        //                         .GroupBy(g => new { g.VatId, g.OrderDetail.Status, g.OrderDetail.Transactions.AccountId })
//        //                         .Select(s => new
//        //                         {
//        //                             VatId = s.Key.VatId,
//        //                             Gross = s.Sum(sm => sm != null ? sm.Gross : 0),
//        //                             //VatAmount = s.Sum(s1 => s1!=null ? s1.VatAmount :0),
//        //                             VatAmount = s.Sum(sm => sm != null ? sm.Gross : 0) - s.Sum(sm => sm != null ? sm.Net : 0),
//        //                             Net = s.Sum(sm => sm != null ? sm.Net : 0),
//        //                             Discount = s.Sum(sm => sm != null ? sm.OrderDetail.Discount : 0),
//        //                             VatRate = s.FirstOrDefault() != null ? s.FirstOrDefault().VatRate : null,
//        //                             TaxId = s.FirstOrDefault() != null ? s.FirstOrDefault().TaxId : null,
//        //                             TaxAmount = s.FirstOrDefault() != null ? s.FirstOrDefault().TaxAmount : 0,
//        //                             AccountId = s.Key.AccountId,//s.FirstOrDefault().OrderDetail.Order.Transactions.FirstOrDefault().AccountId,
//        //                             Status = s.FirstOrDefault().OrderDetail.Status
//        //                         }).ToList();


//        //        var vatIngAnals = db.OrderDetailIgredientVatAnal.Include("OrderDetailIgredients")
//        //                                                        .Include("OrderDetailIgredients.OrderDetail.Transactions")
//        //                                                        .Include("OrderDetailIgredients.OrderDetail.Order")
//        //                                                        .Include("OrderDetailIgredients.OrderDetail.Order.Transactions")
//        //                                                        .Where(w => w.OrderDetailIgredients.OrderDetail.Order.EndOfDayId == null && w.OrderDetailIgredients.OrderDetail.TransactionId != null
//        //                                                                            && w.OrderDetailIgredients.OrderDetail.Order.PosId == pos.Id)
//        //                                          .GroupBy(g => new { g.VatId, g.OrderDetailIgredients.OrderDetail.Status, g.OrderDetailIgredients.OrderDetail.Transactions.AccountId })
//        //                                          .Select(s => new
//        //                                          {
//        //                                              VatId = s.Key.VatId,
//        //                                              Gross = s.Sum(sm => sm.Gross),
//        //                                              //VatAmount = s.Sum(s1 => s1!=null ? s1.VatAmount :0),
//        //                                              VatAmount = s.Sum(sm => sm != null ? sm.Gross : 0) - s.Sum(sm => sm != null ? sm.Net : 0),
//        //                                              Net = s.Sum(sm => sm != null ? sm.Net : 0),
//        //                                              Discount = s.Sum(sm => sm != null ? sm.OrderDetailIgredients.OrderDetail.Discount : 0),
//        //                                              VatRate = s.FirstOrDefault() != null ? s.FirstOrDefault().VatRate : null,
//        //                                              TaxId = s.FirstOrDefault() != null ? s.FirstOrDefault().TaxId : null,
//        //                                              TaxAmount = s.FirstOrDefault().TaxAmount != null ? s.FirstOrDefault().TaxAmount : 0,
//        //                                              AccountId = s.Key.AccountId,//s.FirstOrDefault().OrderDetailIgredients.OrderDetail.Order.Transactions.FirstOrDefault().AccountId,
//        //                                              Status = s.FirstOrDefault().OrderDetailIgredients.OrderDetail.Status
//        //                                          }).ToList();

//        //        var united = vatAnals.Union(vatIngAnals);

//        //        //throw new Exception("asdas");


//        //        var unitedGroupedByVatId = united.Where(w => w.Status != 5).GroupBy(g => g.VatId)
//        //                                          .Select(s => new
//        //                                          {
//        //                                              VatId = s.Key,
//        //                                              Gross = s.Sum(sm => sm.Gross),
//        //                                              VatAmount = s.Sum(sm => sm.Gross) - s.Sum(sm => sm.Net != null ? sm.Net : 0),
//        //                                              Net = s.Sum(sm => sm.Net != null ? sm.Net : 0),
//        //                                              Discount = s.Sum(sm => sm.Discount != null ? sm.Discount : 0),
//        //                                              EndOfdayId = EndOfDay.Id,
//        //                                              VatRate = s.FirstOrDefault().VatRate,
//        //                                              TaxId = s.FirstOrDefault().TaxId,
//        //                                              TaxAmount = s.Sum(sm => sm.TaxAmount),
//        //                                              AccountId = s.FirstOrDefault().AccountId,
//        //                                              Status = s.FirstOrDefault().Status
//        //                                          });

//        //        var unitedGroupedByAccount = united.Where(w => w.Status != 5).GroupBy(g => new { g.AccountId })
//        //                                          .Select(s => new
//        //                                          {
//        //                                              AccountId = s.Key.AccountId,
//        //                                              EndOfdayId = EndOfDay.Id,
//        //                                              Gross = s.Sum(sm => sm.Gross),
//        //                                              //    Status = s.Key.Status

//        //                                          });

//        //        var unitedGroupedByAccountVoidsOnly = united.Where(w => w.Status == 5).GroupBy(g => new { g.AccountId })
//        //                                              .Select(s => new
//        //                                              {
//        //                                                  AccountId = s.Key.AccountId,
//        //                                                  EndOfdayId = EndOfDay.Id,
//        //                                                  Gross = s.Sum(sm => sm.Gross),
//        //                                                  Net = s.Sum(sm => sm.Net),
//        //                                                  VatAmount = s.Sum(sm => sm.VatAmount),
//        //                                                  TaxAmount = s.Sum(sm => sm.TaxAmount),
//        //                                                  Discount = s.Sum(sm => sm.Discount)
//        //                                                  //      Status = s.Key.Status

//        //                                              });
//        //        foreach (var u in unitedGroupedByVatId)
//        //        {
//        //            EndOfDayDetail eoddet = new EndOfDayDetail();
//        //            eoddet.Id = 0;
//        //            eoddet.EndOfdayId = u.EndOfdayId;
//        //            eoddet.TaxAmount = u.TaxAmount;
//        //            eoddet.VatId = u.VatId;
//        //            eoddet.VatRate = u.VatRate;
//        //            eoddet.TaxId = u.TaxId;
//        //            eoddet.VatAmount = u.VatAmount;
//        //            eoddet.Gross = u.Gross;
//        //            eoddet.Net = u.Net;
//        //            eoddet.Discount = u.Discount;
//        //            db.EndOfDayDetail.Add(eoddet);
//        //            totalGross += u.Gross;
//        //            totalNet += u.Net;
//        //            totalVatAmount += u.VatAmount;
//        //            totalTaxAmount += u.TaxAmount;
//        //            totalDiscount += u.Discount;
//        //        }

//        //        //Canceled
//        //        foreach (var byAccount in unitedGroupedByAccountVoidsOnly)
//        //        {
//        //            //TODO: Enum for cancelation
//        //            EndOfDayVoidsAnalysis eodVoidAnal = new EndOfDayVoidsAnalysis();
//        //            eodVoidAnal.Id = 0;
//        //            eodVoidAnal.EndOfDayId = byAccount.EndOfdayId;
//        //            eodVoidAnal.AccountId = byAccount.AccountId;
//        //            eodVoidAnal.Total = byAccount.Gross;
//        //            db.EndOfDayVoidsAnalysis.Add(eodVoidAnal);
//        //        }
//        //        //Payments
//        //        foreach (var byAccount in unitedGroupedByAccount)
//        //        {
//        //            EndOfDayPaymentAnalysis eodAnal = new EndOfDayPaymentAnalysis();
//        //            eodAnal.Id = 0;
//        //            eodAnal.EndOfDayId = byAccount.EndOfdayId;
//        //            eodAnal.AccountId = byAccount.AccountId;
//        //            eodAnal.Total = byAccount.Gross;
//        //            db.EndOfDayPaymentAnalysis.Add(eodAnal);
//        //        }


//        //        db.SaveChanges();
//        //        //tx.Commit();
//        //        //}


//        //        // after EOD is saved ok send departments amounts to PMS
//        //        SendEodDataToPms(EndOfDay.Id);

//        //        //totalGross = totalGross - unitedGroupedByAccountVoidsOnly.Sum(s => s.Gross);
//        //        //totalNet = totalNet - unitedGroupedByAccountVoidsOnly.Sum(s => s.Net);
//        //        //totalVatAmount = totalVatAmount - unitedGroupedByAccountVoidsOnly.Sum(s => s.VatAmount);
//        //        //totalTaxAmount = totalTaxAmount - unitedGroupedByAccountVoidsOnly.Sum(s => s.TaxAmount);
//        //        //totalDiscount = totalDiscount - unitedGroupedByAccountVoidsOnly.Sum(s => s.Discount);

//        //        //EndOfDay.Gross = totalGross;
//        //        //EndOfDay.Net = totalNet;
//        //        //EndOfDay.Discount = totalDiscount;
//        //        //db.SaveChanges();

//        //        EndOfDay eod = new Pos_WebApi.EndOfDay();
//        //        eod.Id = EndOfDay.Id;
//        //        eod.CloseId = EndOfDay.CloseId;
//        //        eod.FODay = EndOfDay.FODay;
//        //        eod.Gross =  EndOfDay.Gross;
//        //        eod.ItemCount = EndOfDay.ItemCount;
//        //        eod.Net = EndOfDay.Net;
//        //        eod.PosInfoId = EndOfDay.PosInfoId;
//        //        eod.StaffId = EndOfDay.StaffId;
//        //        eod.TicketAverage = EndOfDay.TicketAverage;
//        //        eod.TicketsCount = EndOfDay.TicketsCount;

//        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, eod);
//        //        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = EndOfDay.Id }));
//        //        return response;
//        //    }
//        //    else
//        //    {
//        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
//        //    }
//        //}
//        #endregion

//        #region PostEndOfDay With Invoice Transactions
//        public HttpResponseMessage PostEndOfDay(EndOfDay EndOfDay)
//        {
//            db.Configuration.LazyLoadingEnabled = true;
        
//            if (ModelState.IsValid)
//            {
//                //Check if EndOfDay is already posted
//                //var eodExists = db.EndOfDay.Where(w => w.PosInfoId == EndOfDay.PosInfoId && w.FODay == EndOfDay.FODay && w.CloseId == EndOfDay.CloseId);
//                //if (eodExists.Count() > 0)
//                //{
//                //    foreach (var e in eodExists)
//                //    {
//                //        if (e.Invoices.Count > 0) // Has Invoices so we won't proceed
//                //        {
//                //            return null;
//                //        }
//                //    }
//                //}

//                PosInfo pos = db.PosInfo.Where(w => w.Id == EndOfDay.PosInfoId).FirstOrDefault();
//                //pos.FODay = pos.FODay != null ? pos.FODay.Value.AddDays(1) : DateTime.Today;
//                pos.IsOpen = false;
//                pos.CloseId = EndOfDay.CloseId;

//                EndOfDay.FODay = pos.FODay;
//                EndOfDay.dtDateTime = DateTime.Now;
//                db.EndOfDay.Add(EndOfDay);

//                db.SaveChanges();
//                decimal? totalGross = 0.0m;
//                decimal? totalNet = 0.0m;
//                decimal? totalVatAmount = 0.0m;
//                decimal? totalTaxAmount = 0.0m;
//                decimal? totalDiscount = 0.0m;
//                var invoices = db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == EndOfDay.PosInfoId);
//                foreach (var t in invoices)
//                {
//                    t.EndOfDayId = EndOfDay.Id;
//                    //   db.Entry(t).State = EntityState.Modified;
//                }
//                var transactions = db.Transactions.Where(w => w.EndOfDayId == null && w.PosInfoId == EndOfDay.PosInfoId);
//                foreach (var t in transactions)
//                {
//                    t.EndOfDayId = EndOfDay.Id;
//                    //      db.Entry(t).State = EntityState.Modified;
//                }
//                var orders = db.Order.Where(w => w.EndOfDayId == null && w.PosId == EndOfDay.PosInfoId);
//                foreach (var o in orders)
//                {
//                    o.EndOfDayId = EndOfDay.Id;
//                    //     db.Entry(o).State = EntityState.Modified;
//                }
//                var transfers = db.TransferToPms.Where(w => w.EndOfDayId == null && w.PosInfoId == EndOfDay.PosInfoId);
//                foreach (var o in transfers)
//                {
//                    o.EndOfDayId = EndOfDay.Id;
//                    //  db.Entry(o).State = EntityState.Modified;
//                }
//                var mealConsumption = db.MealConsumption.Where(w => w.EndOfDayId == null && w.PosInfoId == EndOfDay.PosInfoId);
//                foreach (var m in mealConsumption)
//                {
//                    m.EndOfDayId = EndOfDay.Id;
//                }
//                var vatAnals = db.OrderDetailVatAnal.Include("OrderDetail.Order")
//                                                    .Where(w => w.OrderDetail.Order.EndOfDayId == null && w.OrderDetail.Order.PosId == pos.Id && (w.IsDeleted ?? false) == false)
//                                 .ToList().GroupBy(g => new { g.VatId, g.OrderDetail.Status }).ToList()
//                                 .Select(s => new
//                                 {
//                                     VatId = s.Key.VatId,
//                                     Gross = s.Sum(sm => sm != null ? sm.Gross : 0),
//                                     //VatAmount = s.Sum(s1 => s1!=null ? s1.VatAmount :0),
//                                     VatAmount = Math.Round((decimal)(s.Sum(sm => sm != null ? sm.Gross : 0) - s.Sum(sm => sm != null ? sm.Net : 0) - s.Sum(sm => sm != null ? sm.TaxAmount : 0)), 2, MidpointRounding.AwayFromZero),
//                                     Net = Math.Round((decimal)(s.Sum(sm => sm != null ? sm.Net : 0)), 2, MidpointRounding.AwayFromZero),
//                                     Discount = s.Sum(sm => sm != null ? sm.OrderDetail.Discount : 0),
//                                     VatRate = s.FirstOrDefault() != null ? s.FirstOrDefault().VatRate : null,
//                                     TaxId = s.FirstOrDefault() != null ? s.FirstOrDefault().TaxId : null,
//                                     TaxAmount = s.FirstOrDefault() != null ? s.FirstOrDefault().TaxAmount : 0,
//                                     Status = s.FirstOrDefault().OrderDetail.Status
//                                 }).ToList();


//                var vatIngAnals = db.OrderDetailIgredientVatAnal.Include("OrderDetailIgredients")
//                                                                .Include("OrderDetailIgredients.OrderDetail.Order")
//                                                                .Where(w => w.OrderDetailIgredients.OrderDetail.Order.EndOfDayId == null
//                                                                                    && w.OrderDetailIgredients.OrderDetail.Order.PosId == pos.Id && (w.IsDeleted ?? false) == false)
//                                                  .ToList().GroupBy(g => new { g.VatId, g.OrderDetailIgredients.OrderDetail.Status }).ToList()
//                                                  .Select(s => new
//                                                  {
//                                                      VatId = s.Key.VatId,
//                                                      Gross = s.Sum(sm => sm.Gross),
//                                                      //VatAmount = s.Sum(s1 => s1!=null ? s1.VatAmount :0),
//                                                      VatAmount = Math.Round((decimal)(s.Sum(sm => sm != null ? sm.Gross : 0) - s.Sum(sm => sm != null ? sm.Net : 0) - s.Sum(sm => sm != null ? sm.TaxAmount : 0)), 2),
//                                                      Net = Math.Round((decimal)(s.Sum(sm => sm != null ? sm.Net : 0)), 2, MidpointRounding.AwayFromZero),
//                                                      Discount = s.Sum(sm => sm != null ? sm.OrderDetailIgredients.OrderDetail.Discount : 0),
//                                                      VatRate = s.FirstOrDefault() != null ? s.FirstOrDefault().VatRate : null,
//                                                      TaxId = s.FirstOrDefault() != null ? s.FirstOrDefault().TaxId : null,
//                                                      TaxAmount = s.FirstOrDefault().TaxAmount != null ? s.FirstOrDefault().TaxAmount : 0,
//                                                      Status = s.FirstOrDefault().OrderDetailIgredients.OrderDetail.Status
//                                                  }).ToList();

//                var united = vatAnals.Union(vatIngAnals);

//                //throw new Exception("asdas");
//                //TODO: Create Group per invoice Types temp solution
//                List<Int32?> invoicetypesList = new List<Int32?>() { 1, 4, 5, 7 };
//                List<Int32?> voidInvoicetypesList = new List<Int32?>() { 3 };




//                var unitedGroupedByVatId = united.Where(w => w.Status != 5).GroupBy(g => g.VatId).ToList()
//                                                  .Select(s => new
//                                                  {
//                                                      VatId = s.Key,
//                                                      Gross = s.Sum(sm => sm.Gross),
//                                                      VatAmount = Math.Round((decimal)(s.Sum(sm => sm.Gross) - s.Sum(sm => sm.Net != null ? sm.Net : 0) - s.Sum(sm => sm != null ? sm.TaxAmount : 0)), 2, MidpointRounding.AwayFromZero),
//                                                      Net = Math.Round((decimal)(s.Sum(sm => sm.Net != null ? sm.Net : 0)), 2, MidpointRounding.AwayFromZero),
//                                                      Discount = s.Sum(sm => sm.Discount != null ? sm.Discount : 0),
//                                                      EndOfdayId = EndOfDay.Id,
//                                                      VatRate = s.FirstOrDefault().VatRate,
//                                                      TaxId = s.FirstOrDefault().TaxId,
//                                                      TaxAmount = s.Sum(sm => sm.TaxAmount),
//                                                      Status = s.FirstOrDefault().Status
//                                                  });

//                //Acount Summary
//                var byAccountId = (from q in db.Transactions.Where(w => (w.IsDeleted ?? false) == false)
//                                   join inv in db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == pos.Id && w.IsVoided == null && (w.IsDeleted ?? false) == false) on q.InvoicesId equals inv.Id
//                                   join it in db.InvoiceTypes.Where(w => invoicetypesList.Contains(w.Type)) on inv.InvoiceTypeId equals it.Id
//                                   select new
//                                   {
//                                       AccountId = q.AccountId,
//                                       Total = q.Amount
//                                   }).ToList().GroupBy(g => g.AccountId).Select(ss => new
//                                   {
//                                       EndOfdayId = EndOfDay.Id,
//                                       AccountId = ss.FirstOrDefault().AccountId,
//                                       Gross = ss.Sum(sm => sm.Total)
//                                   });


//                var byVoidAccountId = (from q in db.Transactions.Where(w => (w.IsDeleted ?? false) == false)
//                                       join inv in db.Invoices.Where(w => w.EndOfDayId == null && w.PosInfoId == pos.Id && w.IsVoided == null && (w.IsDeleted ?? false) == false) on q.InvoicesId equals inv.Id
//                                       join it in db.InvoiceTypes.Where(w => voidInvoicetypesList.Contains(w.Type)) on inv.InvoiceTypeId equals it.Id
//                                       select new
//                                       {
//                                           AccountId = q.AccountId,
//                                           Total = q.Amount * -1
//                                       }).ToList().GroupBy(g => g.AccountId).Select(ss => new
//                                       {
//                                           EndOfdayId = EndOfDay.Id,
//                                           AccountId = ss.FirstOrDefault().AccountId,
//                                           Gross = ss.Sum(sm => sm.Total)
//                                       });


//                foreach (var u in unitedGroupedByVatId)
//                {
//                    EndOfDayDetail eoddet = new EndOfDayDetail();
//                    eoddet.Id = 0;
//                    eoddet.EndOfdayId = u.EndOfdayId;
//                    eoddet.TaxAmount = u.TaxAmount;
//                    eoddet.VatId = u.VatId;
//                    eoddet.VatRate = u.VatRate;
//                    eoddet.TaxId = u.TaxId;
//                    eoddet.VatAmount = u.VatAmount;
//                    eoddet.Gross = u.Gross;
//                    eoddet.Net = u.Net;
//                    eoddet.Discount = u.Discount;
//                    db.EndOfDayDetail.Add(eoddet);
//                    totalGross += u.Gross;
//                    totalNet += u.Net;
//                    totalVatAmount += u.VatAmount;
//                    totalTaxAmount += u.TaxAmount;
//                    totalDiscount += u.Discount;
//                }

//                //Canceled
//                foreach (var byAccount in byVoidAccountId)
//                {
//                    //TODO: Enum for cancelation
//                    EndOfDayVoidsAnalysis eodVoidAnal = new EndOfDayVoidsAnalysis();
//                    eodVoidAnal.Id = 0;
//                    eodVoidAnal.EndOfDayId = byAccount.EndOfdayId;
//                    eodVoidAnal.AccountId = byAccount.AccountId;
//                    eodVoidAnal.Total = byAccount.Gross;
//                    db.EndOfDayVoidsAnalysis.Add(eodVoidAnal);
//                }
//                //Payments
//                foreach (var byAccount in byAccountId)
//                {
//                    EndOfDayPaymentAnalysis eodAnal = new EndOfDayPaymentAnalysis();
//                    eodAnal.Id = 0;
//                    eodAnal.EndOfDayId = byAccount.EndOfdayId;
//                    eodAnal.AccountId = byAccount.AccountId;
//                    eodAnal.Total = byAccount.Gross;
//                    db.EndOfDayPaymentAnalysis.Add(eodAnal);
//                }


//                db.SaveChanges();
//                //tx.Commit();
//                //}


//                // after EOD is saved ok send departments amounts to PMS
//                try
//                {
//                    if (db.HotelInfo.Count() > 0)
//                        SendEodDataToPms(EndOfDay.Id);

//                }
//                catch (Exception ex)
//                {
//                    logger.Error(ex.ToString());
//                   // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
//                }
//                //totalGross = totalGross - unitedGroupedByAccountVoidsOnly.Sum(s => s.Gross);
//                //totalNet = totalNet - unitedGroupedByAccountVoidsOnly.Sum(s => s.Net);
//                //totalVatAmount = totalVatAmount - unitedGroupedByAccountVoidsOnly.Sum(s => s.VatAmount);
//                //totalTaxAmount = totalTaxAmount - unitedGroupedByAccountVoidsOnly.Sum(s => s.TaxAmount);
//                //totalDiscount = totalDiscount - unitedGroupedByAccountVoidsOnly.Sum(s => s.Discount);

//                //EndOfDay.Gross = totalGross;
//                //EndOfDay.Net = totalNet;
//                //EndOfDay.Discount = totalDiscount;
//                //db.SaveChanges();

//                EndOfDay eod = new Pos_WebApi.EndOfDay();
//                eod.Id = EndOfDay.Id;
//                eod.CloseId = EndOfDay.CloseId;
//                eod.FODay = EndOfDay.FODay;
//                eod.Gross = EndOfDay.Gross;
//                eod.ItemCount = EndOfDay.ItemCount;
//                eod.Net = EndOfDay.Net;
//                eod.PosInfoId = EndOfDay.PosInfoId;
//                eod.StaffId = EndOfDay.StaffId;
//                eod.TicketAverage = EndOfDay.TicketAverage;
//                eod.TicketsCount = EndOfDay.TicketsCount;
//                eod.dtDateTime = EndOfDay.dtDateTime;
//                db.Configuration.LazyLoadingEnabled = false;
//                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, eod);
//                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = EndOfDay.Id }));
//                return response;
//            }
//            else
//            {
//                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
//            }
//        }
//        #endregion


//        private void SendEodDataToPms(long eodId)
//        {


//            var posid = db.EndOfDay.Find(eodId).PosInfoId;

//            var pos = db.PosInfo.Where(w => w.Id == posid).FirstOrDefault();
//            db.Entry(pos).Reference(r => r.Department).Load();
//            var eodTransfersToPms = db.TransferToPms.Where(w => w.PosInfoId == posid && w.Status == 0 && w.EndOfDayId == eodId && w.Transactions.TransactionType == 3
//                && (w.Transactions.Invoices.IsVoided ?? false) == false && (w.Transactions.IsDeleted ?? false) == false).ToList();
//            foreach (var i in eodTransfersToPms)
//            {
//                i.SendToPMS = true;
//            }

//            var eodTransfers = eodTransfersToPms.
//            GroupBy(g => new { g.PmsDepartmentId, g.RoomDescription }).Select(s => new
//            {
//                PosDepartmentId = pos.Department.Description,
//                AccountDescription = s.FirstOrDefault().ProfileName,
//                PmsDepartmentId = s.FirstOrDefault().PmsDepartmentId,
//                PmsDepDescription = s.FirstOrDefault().PmsDepartmentDescription,
//                AccountRoom = s.FirstOrDefault().RoomDescription,
//                PosId = posid,
//                PosName = pos.Description,
//                Total = s.Sum(sm => sm.Total)
//            });


//            var hotel = db.HotelInfo.FirstOrDefault();
//            string storeid = HttpContext.Current.Request.Params["storeid"];

//            List<TransferObject> objToSend = new List<TransferObject>();
//            foreach (var total in eodTransfers)
//            {
//                // Create insert to local transfertopms table
//                TransferToPms tpms = new TransferToPms();
//                tpms.Description = "Pos:" + total.PosId + " PosName: " + total.PosName + " Descr:" + total.AccountDescription + " Ημέρας  EOD:" + eodId;
//                tpms.PmsDepartmentId = total.PmsDepartmentId;
//                tpms.PmsDepartmentDescription = total.PmsDepDescription;
//                tpms.SendToPmsTS = DateTime.Now;
//                //  tpms.PosInfoDetailId = piDet.Id;
//                // tpms.ProfileId = g.CustomerId;
//                //  tpms.ProfileName = order.CustomerName;
//                //  tpms.ReceiptNo = newCounter.ToString();
//                tpms.RegNo = "0";//order.RegNo;
//                tpms.RoomDescription = total.AccountRoom.ToString();
//                //  tpms.RoomId = order.RoomId;
//                tpms.SendToPMS = true;
//                //  tpms.TransactionId = order.Transactions.FirstOrDefault().Id;
//                // tpms.TransferType = 0;//Xrewstiko
//                tpms.Total = (decimal)total.Total;
//                var identifier = Guid.NewGuid();
//                tpms.TransferIdentifier = identifier;

//                db.TransferToPms.Add(tpms);



//                // crate object to send to pms
//                TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, false, "", tpms, identifier);
//                to.profileName = "";
//                to.resId = 0;
//                to.TransferIdentifier = identifier;
//                to.RoomName = total.AccountRoom.ToString();

//                objToSend.Add(to);

//            }

//            db.SaveChanges();

//            foreach (var to in objToSend)
//            {
//                SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
//               // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
//            }

//        }

//        #region old Send Metrita
//        //private void SendEodDataToPms(long eodId)
//        //{
//        //    var flatEodTransfers = from q in db.OrderDetail.Include(f => f.Order).Include(f => f.Order.PosInfo).Where(w => w.Order.EndOfDayId == eodId && w.Transactions.Accounts.SendsTransfer == false && w.Status != 5)
//        //                           join qq in db.TransferMappings on new { PosDep = q.Order.PosInfo.DepartmentId, Pricelist = q.PricelistDetail.PricelistId, ProductId = q.ProductId } equals new { PosDep = qq.PosDepartmentId, Pricelist = qq.PriceListId, ProductId = qq.ProductId }
//        //                           select new
//        //                           {
//        //                               OrderId = q.OrderId,
//        //                               OrderDetailId = q.Id,
//        //                               PricelistId = q.PricelistDetail.PricelistId,
//        //                               AccountId = q.Transactions.AccountId,
//        //                               AccountDescription = q.Transactions.Accounts.Description,
//        //                               Total = q.TotalAfterDiscount,
//        //                               Price = q.Price,
//        //                               Qty = q.Qty,
//        //                               PosId = q.Order.PosInfo.Id,
//        //                               PosName = q.Order.PosInfo.Description,
//        //                               PosDepartmentId = q.Order.PosInfo.DepartmentId,
//        //                               PmsDepartmentId = qq.PmsDepartmentId,
//        //                               PmsDepDescription = qq.PmsDepDescription,
//        //                               VatId = q.PricelistDetail.VatId,
//        //                               AccountRoom = db.EODAccountToPmsTransfer.FirstOrDefault(f => f.AccountId == q.Transactions.AccountId && f.PosInfoId == q.Order.PosId).PmsRoom//"Default Room From Accounts"//qq.Accounts
//        //                           };

//        //    var eodTransfers = flatEodTransfers.GroupBy(g => new { g.PmsDepartmentId, g.AccountRoom, g.VatId })
//        //        .Select(s => new
//        //    {
//        //        PosDepartmentId = s.FirstOrDefault().PosDepartmentId,
//        //        AccountDescription = s.FirstOrDefault().AccountDescription,
//        //        PmsDepartmentId = s.FirstOrDefault().PmsDepartmentId,
//        //        PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
//        //        AccountRoom = s.FirstOrDefault().AccountRoom,
//        //        PosId = s.FirstOrDefault().PosId,
//        //        PosName = s.FirstOrDefault().PosName,
//        //        VatId = s.FirstOrDefault().VatId,
//        //        Total = s.Sum(sm => sm.Total),
//        //    });


//        //    var hotel = db.HotelInfo.FirstOrDefault();
//        //    string storeid = HttpContext.Current.Request.Params["storeid"];

//        //    List<TransferObject> objToSend = new List<TransferObject>();
//        //    foreach (var total in eodTransfers)
//        //    {
//        //        // Create insert to local transfertopms table
//        //        TransferToPms tpms = new TransferToPms();
//        //        tpms.Description = "Pos:" + total.PosId +" PosName: "+total.PosName+ " Descr:" + total.AccountDescription + " Ημέρας VatRate:" + total.VatId + " EOD:" + eodId;
//        //        tpms.PmsDepartmentId = total.PmsDepartmentId;
//        //        tpms.PmsDepartmentDescription = total.PmsDepDescription;
//        //        tpms.SendToPmsTS = DateTime.Now;
//        //      //  tpms.PosInfoDetailId = piDet.Id;
//        //       // tpms.ProfileId = g.CustomerId;
//        //      //  tpms.ProfileName = order.CustomerName;
//        //      //  tpms.ReceiptNo = newCounter.ToString();
//        //        tpms.RegNo = "0";//order.RegNo;
//        //        tpms.RoomDescription = total.AccountRoom.ToString();
//        //      //  tpms.RoomId = order.RoomId;
//        //        tpms.SendToPMS = true;
//        //      //  tpms.TransactionId = order.Transactions.FirstOrDefault().Id;
//        //       // tpms.TransferType = 0;//Xrewstiko
//        //        tpms.Total = (decimal)total.Total;
//        //        var identifier = Guid.NewGuid();
//        //        tpms.TransferIdentifier = identifier;

//        //        db.TransferToPms.Add(tpms);



//        //        // crate object to send to pms
//        //        TransferObject to = new TransferObject();
//        //        to.HotelId = (int)hotel.Id;
//        //        to.amount = (decimal)total.Total;                
//        //        to.description = "Pos: " + total.PosId + " Descr:" + total.AccountDescription + " Ημέρας VatRate:" + total.VatId+ " PosName: "+ total.PosName ;
//        //        int PmsDepartmentId = 0;
//        //        var topmsint = int.TryParse(total.PmsDepartmentId, out PmsDepartmentId);
//        //        to.departmentId = PmsDepartmentId;
//        //        to.HotelUri = hotel.HotelUri;
//        //        to.profileName = "";
//        //        to.resId = 0;
//        //        to.TransferIdentifier = identifier;
//        //        to.RoomName = total.AccountRoom.ToString();

//        //        objToSend.Add(to);

//        //    }

//        //    db.SaveChanges();

//        //    foreach (var to in objToSend)
//        //    {
//        //         SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
//        //        sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
//        //    }

//        //}
//        #endregion

//        private delegate TranferResultModel SendTransfer(TransferObject tpms, string storeid);


//        // get response from pms and update table tranferpms
//        private void SendTransferCallback(IAsyncResult result)
//        {
//            // db = new PosEntities(false);
//            SendTransfer del = (SendTransfer)result.AsyncState;

//            TranferResultModel res = (TranferResultModel)del.EndInvoke(result);

//            Guid storeid;

//            if (Guid.TryParse(res.StoreId, out storeid))
//            {

//                using (var ctx = new PosEntities(false, storeid))
//                {
//                    var originalTransfer = ctx.TransferToPms.Where(f => f.TransferIdentifier == res.TransferObj.TransferIdentifier).FirstOrDefault();

//                    if (originalTransfer != null)
//                    {
//                        // originalTransfer.SendToPmsTS = DateTime.Now;
//                        originalTransfer.ErrorMessage = res.TransferErrorMessage;
//                        originalTransfer.PmsResponseId = res.TransferResponseId;
//                        // originalTransfer.PmsDepartmentDescription = res.TransferObj.pmsDepartmentDescription;

//                    }

//                    ctx.SaveChanges();
//                }
//            }

//        }





//        // DELETE api/EndOfDay/5
//        public HttpResponseMessage DeleteEndOfDay(long id, string storeid)
//        {
//            EndOfDay EndOfDay = db.EndOfDay.Find(id);
//            if (EndOfDay == null)
//            {
//                return Request.CreateResponse(HttpStatusCode.NotFound);
//            }

//            db.EndOfDay.Remove(EndOfDay);

//            try
//            {
//                db.SaveChanges();
//            }
//            catch (DbUpdateConcurrencyException ex)
//            {
//                logger.Error(ex.ToString());
//                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
//            }

//            return Request.CreateResponse(HttpStatusCode.OK, EndOfDay);
//        }

//        [AllowAnonymous]
//        public HttpResponseMessage Options()
//        {
//            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
//        }

//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//    }


//}