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
using Pos_WebApi.Helpers;
using System.Text;
using System.Data.Entity.Core.Objects;
using log4net;
using Symposium.Models.Enums;

namespace Pos_WebApi.Controllers
{
    public class CreditTransactionsController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/CreditTransactions
        public IEnumerable<CreditTransactions> GetCreditTransactionss(string storeid)
        {
            return db.CreditTransactions.AsEnumerable();
        }

        // GET api/CreditTransactions/5
        public CreditTransactions GetCreditTransactions(long id, string storeid)
        {
            CreditTransactions CreditTransactions = db.CreditTransactions.Find(id);
            if (CreditTransactions == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return CreditTransactions;
        }

        public Object GetCreditTotals(string storeid, long posid, bool a)
        {
            var today = DateTime.Now.Date;
            var sold = db.CreditTransactions.Where(f => f.Type == (int)CreditTransactionType.AddCredit && f.EndOfDayId == null && EntityFunctions.TruncateTime(f.CreationTS) == today);
            var consumed = db.CreditTransactions.Where(f => f.Type == (int)CreditTransactionType.RemoveCredit  && f.EndOfDayId == null && EntityFunctions.TruncateTime(f.CreationTS) == today);
            var returned = db.CreditTransactions.Where(f => (f.Type == ((int)CreditTransactionType.ReturnCredit) || (f.Type == (int)CreditTransactionType.ReturnAllCredits)) && f.EndOfDayId == null && EntityFunctions.TruncateTime(f.CreationTS) == today);
            var unsused = (sold.Where(f => f.CreditAccounts.DeactivateTS == null).Sum(ff => ff.Amount) ?? 0)
                + (consumed.Where(f => f.CreditAccounts.DeactivateTS == null).Sum(ff => ff.Amount) ?? 0)
                + (returned.Where(f => f.CreditAccounts.DeactivateTS == null).Sum(ff => ff.Amount) ?? 0);

            var totals = new
            {
                SoldCount = sold.Select(f => f.CreditCodeId).Distinct().Count(),
                SoldTotal = sold.Sum(f => f.Amount) ?? 0,
                ConsumedCount = consumed.Select(f => f.CreditCodeId).Distinct().Count(),
                ConsumedTotal = consumed.Sum(f => f.Amount) * -1 ?? 0,
                ReturnedCount = returned.Select(f => f.CreditCodeId).Distinct().Count(),
                ReturnedTotal = returned.Sum(f => f.Amount) * -1 ?? 0,
                Balance = (sold.Sum(f => f.Amount) ?? 0) + ((consumed.Sum(f => f.Amount) ?? 0) + (returned.Sum(f => f.Amount) ?? 0)),
                LostTotal = (sold.Where(f => f.CreditAccounts.DeactivateTS != null).Sum(ff => ff.Amount) ?? 0)
                + (consumed.Where(f => f.CreditAccounts.DeactivateTS != null).Sum(ff => ff.Amount) ?? 0)
                + (returned.Where(f => f.CreditAccounts.DeactivateTS != null).Sum(ff => ff.Amount) ?? 0),
                UnUsed = unsused,
                NotClosedCount = db.CreditAccounts.Where(f => f.EndOfDayId == null && f.ActivateTS != null && f.DeactivateTS == null).Count(),
                StaffBalance = db.CreditTransactions.Where(f => f.CreditAccounts.ActivateTS != null && f.EndOfDayId == null && f.Type == (int)CreditTransactionType.AddCredit || f.Type == (int)CreditTransactionType.ReturnAllCredits).Sum(f => f.Amount)

            };

            //var perStaff = db.CreditTransactions.Where(f => f.EndOfDayId == null )
            //       .GroupBy(f => f.StaffId)
            //       .Select(s => new
            //       {
            //           StaffId = s.FirstOrDefault().StaffId,
            //           StaffBalance = (s.Where(w => w.Type == (int)CreditTransactionType.AddCredit).Sum(ww => ww.Amount) ?? 0) -
            //                         (
            //                           (s.Where(w => w.Type == (int)CreditTransactionType.RemoveCredit).Sum(ww => ww.Amount) ?? 0) +
            //                           ((s.Where(w => w.Type == (int)CreditTransactionType.ReturnAllCredits).Sum(ww => ww.Amount) ?? 0) * -1)
            //                         ),
            //          StaffTransAlalysis = s,
            //          StaffData = new
            //           {
            //               ImageUri = s.FirstOrDefault().Staff.ImageUri,
            //               Name = s.FirstOrDefault().Staff.LastName + " " + s.FirstOrDefault().Staff.FirstName
            //           }
            //       });

            // per departmenr -> per pos
            var perDep = db.CreditTransactions.Where(f => f.EndOfDayId == null && EntityFunctions.TruncateTime(f.CreationTS) == today)
                 .GroupBy(ff => ff.PosInfo.DepartmentId)
                 .Select(g => new
                 {
                     DepartmentId = g.Key,
                     DepartmentDescription = g.FirstOrDefault().PosInfo.Department.Description,
                     CreditSoldTotal = g.Where(r => r.Type == (int)CreditTransactionType.AddCredit).Sum(s => s.Amount) ?? 0,
                     CreditConsumedTotal = g.Where(r => r.Type == (int)CreditTransactionType.RemoveCredit).Sum(s => s.Amount) ?? 0,
                     CreditPerPos = g.Where(ss => ss.PosInfo.DepartmentId == g.Key).GroupBy(r => r.PosInfoId)
                                    .Select(e => new
                                    {
                                        PosInfoId = e.Key,
                                        PosInfoDescr = e.FirstOrDefault().PosInfo.Description,
                                        CreditSold = e.Where(d => d.Type == (int)CreditTransactionType.AddCredit).Sum(s => s.Amount) ?? 0,
                                        CreditConsumed = e.Where(d => d.Type == (int)CreditTransactionType.RemoveCredit).Sum(s => s.Amount) ?? 0
                                    })

                 });


            return new { Totals = totals, CreditsPerDep = perDep };

        }

        public Object GetTotalsPerPos(string storeid, DateTime? foday)
        {
            if (foday != null)
            {
                var eodDetails = (from q in db.EndOfDayDetail
                                  join qq in db.Vat on q.VatId equals qq.Id
                                  select new
                                  {
                                      EndOfDayId = q.EndOfdayId,
                                      EndOfDayDetailsId = q.Id,
                                      VatId = q.VatId,
                                      VatRate = q.VatRate,
                                      VatAmount = q.VatAmount,
                                      VatCode = qq.Code,
                                      TaxId = q.TaxId,
                                      TaxAmount = q.TaxAmount,
                                  }).GroupBy(g => g.EndOfDayId).Select(s => new
                                  {
                                      EndOfDayId = s.FirstOrDefault().EndOfDayId,
                                      EndOfDayDetails = s
                                  });
                var payDetails = (from q in db.EndOfDayPaymentAnalysis
                                  join qq in db.Accounts on q.AccountId equals qq.Id
                                  select new
                                  {
                                      EndOfDayId = q.EndOfDayId,
                                      EndOfDayPaymentAnalysisId = q.Id,
                                      AccountId = q.AccountId,
                                      AccountDescription = qq.Description,
                                      Total = q.Total
                                  }).GroupBy(g => g.EndOfDayId).Select(s => new
                                  {
                                      EndOfDayId = s.FirstOrDefault().EndOfDayId,
                                      EndOfDayDetails = s
                                  });
                //var voids = (

                //payDetails.Dump();
                var query = from q in db.EndOfDay.Where(f => f.FODay == foday)
                            join qq in eodDetails on q.Id equals qq.EndOfDayId
                            join qqq in payDetails on q.Id equals qqq.EndOfDayId
                            select new
                            {
                                Id = q.Id,
                                FODay = q.FODay,
                                DepartmentId = q.PosInfo.DepartmentId,
                                DepartmentDescription = q.PosInfo.Department.Description,
                                PosInfoId = q.PosInfoId,
                                PosInfoDescription = q.PosInfo.Description,
                                Gross = q.Gross,
                                Net = q.Net,
                                TicketsCount = q.TicketsCount,
                                TicketAverage = q.TicketAverage,
                                Discount = q.Discount,
                                CreditAccounts = q.CreditAccounts,
                                eodDetails = qq,
                                payDetails = qqq,
                                //      All = q
                            };

                var final = query.GroupBy(f => f.DepartmentId)
                 .Select(f => new
                 {
                     DepartmentDescription = f.FirstOrDefault().DepartmentDescription,
                     DepartmentId = f.Key,
                     TotalGross = f.Sum(s => s.Gross),
                     TotalNet = f.Sum(s => s.Net),
                     TotalDiscount = f.Sum(s => s.Discount),
                     TotalTicketsCount = f.Sum(s => s.TicketsCount),
                     TotalTicketAvg = f.Sum(s => s.TicketAverage) / (f.Count() == 0 ? 1 : f.Count()),
                     TotalsPerPos = f
                 });

                return final;
            }
            else
            {
                return null;
            }
        }


        public Object GetLostCredit(string storeid, bool sw)
        {
            var temp = db.CreditTransactions.Where(f => f.CreditAccounts.DeactivateTS != null && f.EndOfDayId == null)
             .GroupBy(g => g.CreditAccountId)
             .Select(s => new
             {
                 CreditAccountId = s.Key,
                 CreateTs = s.FirstOrDefault().CreationTS,
                 DeactivateTs = s.FirstOrDefault().CreditAccounts.DeactivateTS,
                 Balance = s.Sum(r => r.Amount),
                 Cards = s.Select(r => r.CreditCodes.Code),


             }).Where(f => f.Balance > 0);

            var LostAnalysis = new
            {
                TotalCount = temp.Count(),
                TotalSum = temp.Sum(f => f.Balance),
                Analysis = temp
            };

            return LostAnalysis;
        }

        public Object GetNotReturnedCredit(string storeid, bool sw, bool o)
        {
            var temp = db.CreditTransactions.Where(f => f.EndOfDayId == null)
             .GroupBy(g => g.CreditAccountId)
             .Select(s => new
             {
                 CreditAccountId = s.Key,
                 CreateTs = s.FirstOrDefault().CreationTS,
                 DeactivateTs = s.FirstOrDefault().CreditAccounts.DeactivateTS,
                 Balance = s.Sum(r => r.Amount),
                 Cards = s.Select(r => r.CreditCodes.Code),


             }).Where(f => f.Balance > 0);


            var LostAnalysis = new
            {
                TotalCount = temp.Count(),
                TotalSum = temp.Sum(f => f.Balance),
                Analysis = temp
            };


            return LostAnalysis;
        }


        public Object GetStaffBalance(string storeid, string ff)
        {
            //var StaffBalance = db.CreditTransactions.Where(f => f.CreditAccounts.ActivateTS != null && f.EndOfDayId == null && f.Type == (int)CreditTransactionType.AddCredit || f.Type == (int)CreditTransactionType.ReturnAllCredits)
            // .GroupBy(g => g.StaffId)
            // .Select(s => new
            // {
            //     StaffId = s.Key,
            //     StaffName = s.FirstOrDefault().Staff.LastName + " " + s.FirstOrDefault().Staff.FirstName,
            //     StaffImage = s.FirstOrDefault().Staff.ImageUri,
            //     Balance = s.Sum(r => r.Amount),
            //     //StaffTransactions =  s.FirstOrDefault().Staff.Transactions.Where(ww => ww.EndOfDayId == null).Select(t => new { Amount = t.Amount, AccountId = t.AccountId })
            //     PerAccount = s.GroupBy(f => f.CreditAccountId).Select(ss => new {
            //                      AccountId = ss.Key.Value,
            //                      Amount = ss.Sum(sm => sm.Amount)
            //                  }),



            // });//.Where(f => f.Balance > 0);

            var StaffBalance = //(from ct in db.CreditTransactions.Where(f => f.CreditAccounts.DeactivateTS != null && f.EndOfDayId == null && f.Type == 1 || f.Type == 2)
                //join tr in db.Transactions.Where(t => t.EndOfDayId == null && t.TransactionType == 7) on ct.StaffId equals tr.StaffId
                      (from tr in db.Transactions.Where(t => t.EndOfDayId == null && t.TransactionType == (int)TransactionTypesEnum.CreditCode)
                       join st in db.Staff on tr.StaffId equals st.Id
                       select new
                       {
                           StaffId = tr.StaffId,
                           StaffImage = st.ImageUri ?? "",
                           StaffName = st.LastName + " " + st.FirstName,
                           Trans = tr,
                           CTAmount = tr.Amount
                       })
            .GroupBy(g => g.StaffId).Select(s =>
             new
             {
                 StaffId = s.Key,
                 StaffName = s.FirstOrDefault().StaffName,
                 StaffImage = s.FirstOrDefault().StaffImage,
                 Balance = s.Sum(sm => sm.CTAmount),
                 PerAccount = s.Select(f => f.Trans).Distinct().GroupBy(f => f.AccountId).Select(r => new { AccountId = r.FirstOrDefault().AccountId, Amount = r.Sum(sm => sm.Amount) })
             });

            var StaffBalanceTotal = StaffBalance.Sum(sm => sm.Balance);

            return new { StaffBalance = StaffBalance, StaffBalanceTotal = StaffBalanceTotal };

        }

        public Object GetTransactionsByPos(string storeid, long posid)
        {
            var ws = db.CreditTransactions.Where(f => f.EndOfDayId == null && f.PosInfoId == posid).Select(ff =>
            new
            {
                Time = ff.CreationTS,
                Code = ff.CreditCodes.Code,
                Amount = ff.Amount,
                Pos = ff.PosInfo.Description,
                Staff = ff.Staff.LastName + " " + ff.Staff.FirstName,
                Type = ff.Type
            }).ToList().Where(w => w.Time.Value.Date == DateTime.Now.Date)
;

            var result = new
            {
                AddCredit = ws.Where(f => f.Type == 1).OrderBy(r => r.Time),
                RemoveCredit = ws.Where(f => f.Type == 0).OrderBy(r => r.Time),
                ReturnBalance = ws.Where(f => f.Type == 2).OrderBy(r => r.Time)
            };

            return result;
        }

        public Object GetInHouseAccounts(string storeid, bool w, string q)
        {
            var InHousCredits = db.CreditTransactions.Where(f => f.CreditAccounts.EndOfDayId == null && f.CreditAccounts.DeactivateTS == null)
                      .GroupBy(g => g.CreditAccountId)
                      .Select(a => new
                      {
                          AccountId = a.Key,
                          CreatedTS = a.FirstOrDefault().CreditAccounts.ActivateTS,
                          Cards = a.Select(o => o.CreditCodes.Code).Distinct(),
                          Balance = a.Sum(o => o.Amount) ?? 0
                      }).ToList().Where(ww => ww.CreatedTS.Value.Date == DateTime.Now.Date);
            return InHousCredits;
        }


        // PUT api/CreditTransactions/5
        public HttpResponseMessage PutCreditTransactions(long id, string storeid, CreditTransactions CreditTransactions)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != CreditTransactions.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(CreditTransactions).State = EntityState.Modified;

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

        // POST api/CreditTransactions
        public HttpResponseMessage PostCreditTransactions(CreditTransactions CreditTransactions, string storeid)
        {
            if (ModelState.IsValid)
            {
                if (CreditTransactions.Type == (int)CreditTransactionType.ReturnAllCredits)
                {
                    var creditAccount = db.CreditAccounts.Find(CreditTransactions.CreditAccountId);
                    if (creditAccount != null)
                    {
                        creditAccount.DeactivateTS = DateTime.Now;
                    }
                }
                string desc = "";
                if (CreditTransactions.Type == (int)CreditTransactionType.ReturnCredit || CreditTransactions.Type == (int)CreditTransactionType.AddCredit || CreditTransactions.Type == (int)CreditTransactionType.ReturnAllCredits)
                {

                    switch ((CreditTransactionType)CreditTransactions.Type)
                    {
                        case CreditTransactionType.AddCredit: desc = Symposium.Resources.Messages.ADDBARCODEAMOUNT;
                            break;
                        case CreditTransactionType.ReturnAllCredits: desc = Symposium.Resources.Messages.REMAININGAMOUNTRETURN;
                            break;
                        case CreditTransactionType.ReturnCredit: desc = Symposium.Resources.Messages.REMOVEBARCODEAMOUNT;
                            break;
                    }
                    bool isAdd = CreditTransactions.Type == (int)CreditTransactionType.AddCredit;


                    Invoices invoice = new Invoices();
                    invoice.Counter = db.Invoices.Where(w => w.InvoiceTypes.Type == 6).Count();
                    invoice.Day = DateTime.Now;
                    invoice.Description = desc;//isAdd ? "Add amount on Barcode Credit Account" : "Return remaining amount to Customer";
                    invoice.InvoiceTypeId = db.PredefinedCredits.FirstOrDefault() != null ? db.PredefinedCredits.FirstOrDefault().InvoiceTypeId : null;
                    invoice.PosInfoId = CreditTransactions.PosInfoId;
                    invoice.StaffId = CreditTransactions.StaffId;
                    invoice.Total = CreditTransactions.Amount;
                    Transactions tr = new Transactions();
                    var account = db.Accounts.Where(w => w.Type == 5).FirstOrDefault();
                    long? accountId = null;
                    if (account != null)
                    {
                        accountId = account.Id;
                    }
                    tr.AccountId = accountId;
                    tr.Amount = CreditTransactions.Amount;
                    tr.Day = DateTime.Now;
                    tr.DepartmentId = db.PosInfo.Find(CreditTransactions.PosInfoId) != null ? db.PosInfo.Find(CreditTransactions.PosInfoId).DepartmentId : null;
                    tr.Description = desc;//isAdd ? "Add amount on Barcode Credit Account" : "Return remaining amount to Customer";
                    tr.InOut = isAdd ? (short)0 : (short)1;
                    tr.PosInfoId = CreditTransactions.PosInfoId;
                    tr.StaffId = CreditTransactions.StaffId;
                    tr.TransactionType = (int)TransactionTypesEnum.CreditCode;
                    invoice.Transactions.Add(tr);
                    invoice.CreditTransactions.Add(CreditTransactions);
                    db.Invoices.Add(invoice);
                }
                db.CreditTransactions.Add(CreditTransactions);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, CreditTransactions);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = CreditTransactions.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/CreditTransactions/5
        public HttpResponseMessage DeleteCreditTransactions(long id, string storeid)
        {
            CreditTransactions CreditTransactions = db.CreditTransactions.Find(id);
            if (CreditTransactions == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.CreditTransactions.Remove(CreditTransactions);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, CreditTransactions);
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
    }
}