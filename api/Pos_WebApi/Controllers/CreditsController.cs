using log4net;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using Pos_WebApi.Models.DTOModels;
using Pos_WebApi.Repositories;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class CreditsController : ApiController
    {
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
        public CreditAccountsRepository svc;

        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CreditsController()
        {
            svc = new CreditAccountsRepository(db);
        }
        [Route("api/{storeId}/Credits")]
        [HttpPost]
        public HttpResponseMessage GetCreditAccount(string storeId, string code, long? creditaccountid)
        {
            PosEntities db = new PosEntities(false, Guid.Parse(storeId));
            svc = new CreditAccountsRepository(db);
            CreditAccountsDTO res;
            try
            {
                res = svc.GetAccount(code, creditaccountid);

                //    if (svc svc.SaveChanges() == 0)
                //        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
                //}
                //catch (DbUpdateConcurrencyException ex)
                //{
                //    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error CreditsController |" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [Route("api/{storeId}/AddCredit")]
        [HttpPost]
        public HttpResponseMessage AddCredit(string storeId, CreditTransactionDTO model)
        {
            PosEntities db = new PosEntities(false, Guid.Parse(storeId));
            svc = new CreditAccountsRepository(db);
            CreditAccountsDTO res = null;
            try
            {
                var inv = svc.AddCredit(model);
                hub.Clients.Group(storeId).newReceipt(storeId + "|" + inv.ExtecrName, inv.InvoiceId, true, false, PrintType.PrintWhole, null, false);
                res = svc.GetAccount(model.Code, null);

                //if (svc.SaveChanges() == 0)
                //    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error CreditsController |" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [Route("api/{storeId}/RemoveCredit")]
        [HttpPost]
        public HttpResponseMessage RemoveCredit(string storeId, CreditTransactionDTO model)
        {
            PosEntities db = new PosEntities(false, Guid.Parse(storeId));
            svc = new CreditAccountsRepository(db);
            CreditAccountsDTO res = null;
            try
            {
                var inv = svc.ReturnCredit(model);
                hub.Clients.Group(storeId).newReceipt(storeId + "|" + inv.ExtecrName, inv.InvoiceId, true, false, PrintType.PrintWhole, null, false);
                res = svc.GetAccount(model.Code, null);

                //if (svc.SaveChanges() == 0)
                //    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error CreditsController |" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        public class removeCredAccModel
        {
            public long CreditAccountId { get; set; }
            public decimal Amount { get; set; }
            public long StaffId { get; set; }
            public long PosInfoId { get; set; }
            public string Code { get; set; }
            public Nullable<System.DateTime> CreationTS { get; set; }
            public Nullable<long> DepartmentId { get; set; }
            public Nullable<long> AccountId { get; set; }
            public Boolean createInvoice { get; set; }    
            public IEnumerable<Receipts> Receipts { get; set; }
    }

        /// <summary>
        /// Removes all Credits with or without receipt
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [Route("api/{storeId}/RemoveAllCredit")]
        [HttpPost]
        public HttpResponseMessage RemoveAllCredit(string storeId, List<removeCredAccModel> rem)
        {


            PosEntities db = new PosEntities(false, Guid.Parse(storeId));
            svc = new CreditAccountsRepository(db);

            List<long> res = new List<long>();
            try
            {
                foreach (var item in rem)
                {
                    if (item.createInvoice == false)
                    {
                        var query = db.CreditAccounts.Find(item.CreditAccountId);
                        query.DeactivateTS = DateTime.Now;
                        if (ModelState.IsValid)
                        {
                            db.Entry(query).State = System.Data.Entity.EntityState.Modified;
                        }
                        //Update Table CreditTransactions
                        var InsertCredTrans = new CreditTransactions();
                        InsertCredTrans.Amount = item.Amount;
                        InsertCredTrans.CreationTS = DateTime.Now;//item.CreationTS;
                        InsertCredTrans.CreditAccountId = item.CreditAccountId;
                        InsertCredTrans.CreditCodeId = db.CreditCodes.Where(z => z.Code == item.Code).Select(x => x.Id).FirstOrDefault();
                        InsertCredTrans.EndOfDayId = null;//db.EndOfDay.Where(z => z.PosInfoId == item.PosInfoId).Select(x => x.Id).FirstOrDefault();
                        InsertCredTrans.Type = (byte)CreditTransactionType.ReturnAllCredits;
                        InsertCredTrans.StaffId = item.StaffId;
                        InsertCredTrans.Description = Symposium.Resources.Messages.AMOUNTRETURN;
                        InsertCredTrans.PosInfoId = item.PosInfoId;
                        InsertCredTrans.InvoiceId = (db.Invoices.Select(x => x.Id).FirstOrDefault()) + 1;
                        InsertCredTrans.TransactionId = (db.Transactions.Select(x => x.Id).FirstOrDefault()) + 1;
                        db.CreditTransactions.Add(InsertCredTrans);
                        //Update Table Transactions
                        var insertTrans = new Transactions();
                        insertTrans.Day = DateTime.Now;
                        insertTrans.PosInfoId = item.PosInfoId;
                        insertTrans.StaffId = item.StaffId;
                        insertTrans.OrderId = null;
                        insertTrans.TransactionType = 7;
                        insertTrans.Amount = item.Amount;
                        insertTrans.DepartmentId = item.DepartmentId;
                        insertTrans.Description = Symposium.Resources.Messages.REMAININGAMOUNTRETURN;
                        insertTrans.AccountId = item.AccountId;
                        insertTrans.InOut = 1;
                        insertTrans.Gross = null;
                        insertTrans.Net = null;
                        insertTrans.Vat = null;
                        insertTrans.Tax = null;
                        insertTrans.EndOfDayId = InsertCredTrans.EndOfDayId;
                        insertTrans.ExtDescription = null;
                        insertTrans.InvoicesId = InsertCredTrans.InvoiceId;
                        insertTrans.IsDeleted = null;
                        db.Transactions.Add(insertTrans);

                        db.SaveChanges();
                    }
                    else
                    {
                        var query = db.CreditAccounts.Find(item.CreditAccountId);
                        query.DeactivateTS = DateTime.Now;
                        if (ModelState.IsValid)
                        {
                            db.Entry(query).State = System.Data.Entity.EntityState.Modified;
                        }
                        //Update Table CreditTransactions
                        var InsertCredTrans = new CreditTransactions();
                        var repo = new CreditAccountsRepository(db);
                        InsertCredTrans.Amount = item.Amount;
                        InsertCredTrans.CreationTS = DateTime.Now;//item.CreationTS;
                        InsertCredTrans.CreditAccountId = item.CreditAccountId;
                        InsertCredTrans.CreditCodeId = db.CreditCodes.Where(z => z.Code == item.Code).Select(x => x.Id).FirstOrDefault();
                        InsertCredTrans.EndOfDayId = null;//db.EndOfDay.Where(z => z.PosInfoId == item.PosInfoId).Select(x => x.Id).FirstOrDefault();
                        InsertCredTrans.Type = (byte)CreditTransactionType.ReturnAllCredits;
                        InsertCredTrans.StaffId = item.StaffId;
                        InsertCredTrans.Description = Symposium.Resources.Messages.AMOUNTRETURN;
                        InsertCredTrans.PosInfoId = item.PosInfoId;
                        InsertCredTrans.InvoiceId = (db.Invoices.Select(x => x.Id).FirstOrDefault()) + 1;
                        InsertCredTrans.TransactionId = (db.Transactions.Select(x => x.Id).FirstOrDefault()) + 1;
                        db.CreditTransactions.Add(InsertCredTrans);
                        //Update Table Transactions
                        var insertTrans = new Transactions();
                        insertTrans.Day = DateTime.Now;
                        insertTrans.PosInfoId = item.PosInfoId;
                        insertTrans.StaffId = item.StaffId;
                        insertTrans.OrderId = null;
                        insertTrans.TransactionType = 7;
                        insertTrans.Amount = item.Amount;
                        insertTrans.DepartmentId = item.DepartmentId;
                        insertTrans.Description = Symposium.Resources.Messages.REMAININGAMOUNTRETURN;
                        insertTrans.AccountId = item.AccountId;
                        insertTrans.InOut = 1;
                        insertTrans.Gross = null;
                        insertTrans.Net = null;
                        insertTrans.Vat = null;
                        insertTrans.Tax = null;
                        insertTrans.EndOfDayId = InsertCredTrans.EndOfDayId;
                        insertTrans.ExtDescription = null;
                        insertTrans.InvoicesId = InsertCredTrans.InvoiceId;
                        insertTrans.IsDeleted = null;
                        db.Transactions.Add(insertTrans);

                        db.SaveChanges();
                        var crDTO = new CreditTransactionDTO();
                        crDTO.CreditAccountId = InsertCredTrans.CreditAccountId??0;
                        crDTO.CreditAccountDescription = InsertCredTrans.Description;
                        crDTO.AccountId = insertTrans.AccountId??-1;
                        crDTO.StaffId = InsertCredTrans.StaffId??-1;
                        crDTO.PosInfoId = InsertCredTrans.PosInfoId??-1;
                        crDTO.Amount = insertTrans.Amount??0;
                        crDTO.Code = item.Code;
                        crDTO.CreditCodeId = InsertCredTrans.CreditCodeId ??0;
                        crDTO.CreditAccountId = item.CreditAccountId;
                        crDTO.Balance = InsertCredTrans.Amount;
                        repo.AddCredit(crDTO);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error CreditsController |" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);

            //List<CreditAccountsDTO> res = null;
            //CreditTransactionDTO model = new CreditTransactionDTO();
            //try
            //{
            //    string str = svc.GetAllCred();
            //    res = svc.CreateCreditAcc(str);

            //    foreach (CreditAccountsDTO item in res)
            //    {
            //        foreach (CreditAccountTransactions element in item.CreditAccountTransactions)
            //        {
            //            //public long CreditAccountId { get; set; }
            //            //public string CreditAccountDescription { get; set; }
            //            //public long CreditCodeId { get; set; }
            //            //public long StaffId { get; set; }
            //            //public long PosInfoId { get; set; }
            //            //public string Code { get; set; }
            //            //public decimal? Balance { get; set; }

            //            model.AccountId = element.AccountId ?? 0;
            //            model.Amount = element.Amount ?? 0;
            //            model.CreditAccountId = item.CreditAccountId ?? 0;
            //            model.CreditAccountDescription = element.CreditAccountTransactionsDescription;
            //            model.CreditCodeId = item.CreditCodesId;
            //            model.Code = item.CreditCodesCode;
            //            model.Balance = item.Balance;
            //            model.StaffId = staffId;
            //            model.PosInfoId = posInfoId;

            //            var inv = svc.ReturnCredit(model);
            //            hub.Clients.Group(storeId).newReceipt(storeId + "|" + inv.ExtecrName, inv.InvoiceId, true, false);
            //            res.Add(svc.GetAccount(model.Code, null));
            //        }
            //        //var inv = svc.ReturnCredit(item.CreditAccountTransactions);
            //        //hub.Clients.Group(storeId).newReceipt(storeId + "|" + inv.ExtecrName, inv.InvoiceId, true, false);
            //        // res = svc.GetAccount(item.CreditCodesCode, null);
            //    }

            //}
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);

            //}
            //catch (Exception ex)
            //{
            //    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error CreditsController |" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            //}

            //return Request.CreateResponse(HttpStatusCode.OK, res);


        }

        [Route("api/{storeId}/DeactivateAccount")]
        [HttpPost]
        public HttpResponseMessage DeactivateAccount(string storeId, CreditTransactionDTO model)
        {
            PosEntities db = new PosEntities(false, Guid.Parse(storeId));
            svc = new CreditAccountsRepository(db);
            CreditAccountsDTO res = null;
            try
            {
                var inv = svc.ReturnCredit(model, true);
                hub.Clients.Group(storeId).newReceipt(storeId + "|" + inv.ExtecrName, inv.InvoiceId, true, false, PrintType.PrintWhole, null, false);
                res = svc.GetAccount(model.Code, null);

                //if (svc.SaveChanges() == 0)
                //    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error CreditsController |" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Gets all open barcodes with return balance
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="code"></param>
        /// <param name="creditaccountid"></param>
        /// <returns></returns>
        [Route("api/{storeId}/GetAllCredits")]
        [HttpGet]
        public HttpResponseMessage GetAllCreditsAccount(string storeId)
        {
            PosEntities db = new PosEntities(false, Guid.Parse(storeId));
            svc = new CreditAccountsRepository(db);
            string res;
            try
            {
                //res = svc.GetAccount("1", 18L);

                res = svc.GetAllCred();
                //    if (svc svc.SaveChanges() == 0)
                //        return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, Symposium.Resources.Errors.FAILED);
                //}
                //catch (DbUpdateConcurrencyException ex)
                //{
                //    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
               // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Error GetAllCredits |" + ex.InnerException == null ? ex.InnerException.Message : ex.Message));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();

            base.Dispose(disposing);
        }
    }


}
