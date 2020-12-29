using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class CreditCodesController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Object GetCreditCodes(string storeid, string barcode, long? creditaccountid)
        {
            if (creditaccountid == null)
            {
                var code = db.CreditCodes.Where(w => w.Code == barcode).FirstOrDefault();
                if (code != null)
                {
                   // var account = db.CreditAccounts.Include(i => i.CreditCodes).Include(i => i.CreditTransactions).Where(w => w.Id == code.CreditAccountId).AsNoTracking().FirstOrDefault();

                    var validInvs = db.CreditTransactions.Where(w => w.CreditAccountId == code.CreditAccountId && w.Type == 0).Select(s => s.InvoiceId);
                    var orders = from q in db.OrderDetailInvoices
                                 join qq in db.OrderDetail on q.OrderDetailId equals qq.Id
                                 join qqq in db.Invoices.Where(w => validInvs.Contains(w.Id)) on q.InvoicesId equals qqq.Id
                                 join p in db.Product on qq.ProductId equals p.Id
                                 select new
                                 {
                                     InvoiceId = q.InvoicesId,
                                     Invoice = qqq.Description,
                                     Counter = qqq.Counter,
                                     Product = p.Description,
                                     Price = qq.Price,
                                     Qty = qq.Qty,
                                     Total = qq.TotalAfterDiscount
                                 };

                    var final = ((from qqq in db.CreditCodes.Where(w => w.CreditAccountId == code.CreditAccountId)
                                  join ct in db.CreditTransactions on qqq.Id equals ct.CreditCodeId into ff
                                  from q in ff.DefaultIfEmpty()
                                  join qq in db.CreditAccounts on qqq.CreditAccountId equals qq.Id
                                  join o in orders on q.InvoiceId equals o.InvoiceId into f
                                  from ord in f.DefaultIfEmpty()
                                  join qqqq in db.PosInfo on q.PosInfoId equals qqqq.Id into fff
                                  from pi in fff.DefaultIfEmpty()
                                  select new
                                  {
                                      Id = qq.Id,
                                      Description = qq.Description,
                                      ActivateTS = qq.ActivateTS,
                                      DeactivateTS = qq.DeactivateTS,
                                      CreditTranactionsId = q != null ? (Int64?)q.Id : null,
                                      CreditTranactionsDescription = q != null ? q.Description : "",
                                      InvoiceId = q != null ? q.InvoiceId : null,
                                      CreditCodeId = q != null ? q.CreditCodeId : null,
                                      CreditAccountId = qqq.CreditAccountId,
                                      Amount = q != null ? q.Amount : 0,
                                      Type = q != null ? q.Type : null,
                                      StaffId = q != null ? q.StaffId : null,
                                      CreditTransactionDescription = q != null ? (string)q.Description : "",
                                      Product = ord.Product,
                                      Price = ord.Price,
                                      Qty = ord.Qty,
                                      ProductTotal = ord.Total,
                                      CreditCodes = qqq,
                                      PosInfoId = q != null ? q.PosInfoId : null,
                                      PosInfoDescription = pi != null ? pi.Description : ""

                                  }).ToList().GroupBy(g => g.InvoiceId).Select(s => new
                                  {
                                      Id = s.FirstOrDefault().CreditAccountId,
                                      Description = s.FirstOrDefault().Description,
                                      ActivateTS = s.FirstOrDefault().ActivateTS,
                                      DeactivateTS = s.FirstOrDefault().DeactivateTS,
                                      CreditTransactions = s.GroupBy(g => g.InvoiceId).Select(ss => new
                                      {
                                          Description = ss.FirstOrDefault().CreditTransactionDescription,
                                          CreditCodeId = ss.FirstOrDefault().CreditCodeId,
                                          CreditAccountId = ss.FirstOrDefault().CreditAccountId,
                                          Amount = ss.FirstOrDefault().Amount,
                                          Type = ss.FirstOrDefault().Type,
                                          StaffId = ss.FirstOrDefault().StaffId,
                                          PosInfoId = ss.FirstOrDefault().PosInfoId,
                                          PosInfoDescription = ss.FirstOrDefault().PosInfoDescription,
                                          InvoiceDetails = ss.Select(sss => new
                                          {
                                              Product = sss.Product,
                                              Price = sss.Price,
                                              Qty = sss.Qty,
                                              ProductTotal = sss.ProductTotal,
                                          }).Where(w => w.Product != null)

                                      }),
                                      CreditCodes = s.Select(sss => sss.CreditCodes)

                                  })).GroupBy(g => g.Id).Select(s => new
                                  {
                                      Id = s.FirstOrDefault().Id,
                                      Description = s.FirstOrDefault().Description,
                                      ActivateTS = s.FirstOrDefault().ActivateTS,
                                      DeactivateTS = s.FirstOrDefault().DeactivateTS,
                                      CreditTransactions = s.SelectMany(ss => ss.CreditTransactions),
                                      CreditCodes = s.SelectMany(ss => ss.CreditCodes).Distinct()
                                  });

                    return final.FirstOrDefault();
                }
                else
                {
                    CreditCodes cc = new CreditCodes();
                    cc.Code = barcode;
                    cc.Type = 1;//Den shmainei kati pros to parwn
                    CreditAccounts cacc = new CreditAccounts();
                    cacc.ActivateTS = DateTime.Now;
                    cacc.CreditCodes.Add(cc);
                    db.CreditAccounts.Add(cacc);
                    db.SaveChanges();
                    return cacc;
                }
            }
            else
            {
                dynamic Response = new ExpandoObject();
                var check = db.CreditAccounts.Include(i => i.CreditCodes).Where(w => w.Id == creditaccountid).AsNoTracking().FirstOrDefault();
                if (check == null)
                {                  
                    Response.Error = true;
                    Response.Message = Symposium.Resources.Messages.WRONGCREDITCARD;
                    return Response;
                }
                else
                {
                    var checkAllBarcodes = db.CreditCodes.Where(w => w.Code == barcode).FirstOrDefault();
                    if (checkAllBarcodes != null)
                    {
                        Response.Error = true;
                        Response.Message = Symposium.Resources.Messages.BARCODEGROUPED;
                        return Response;
                    }
                    if (check.DeactivateTS != null)
                    {
                        Response.Error = true;
                        Response.Message = string.Format(Symposium.Resources.Messages.ACCOUNTCLOSED, check.DeactivateTS.Value.ToShortTimeString());
                        return Response;
                    }
                    var bcc = check.CreditCodes.Where(w => w.Code == barcode).FirstOrDefault();
                    if (bcc != null)
                    {
                        Response.Error = true;
                        Response.Message = Symposium.Resources.Messages.BARCODEALREADYEXISTS;
                        return Response;
                    }
                    else
                    {
                        CreditCodes cc = new CreditCodes();
                        cc.Code = barcode;
                        cc.Type = 1;//Den shmainei kati pros to parwn
                        cc.CreditAccountId = check.Id;
                        db.CreditCodes.Add(cc);
                        db.SaveChanges();
                        check.CreditCodes.Add(cc);
                        return check;
                    }
                }
            }
        }

        public Object GetCreditCodes(string storeid, string barcode)
        {
            var code = db.CreditCodes.Where(w => w.Code == barcode).FirstOrDefault();
                if (code != null)
                {
                    var account = db.CreditAccounts.Include(i => i.CreditCodes).Include(i => i.CreditTransactions).Where(w => w.Id == code.CreditAccountId).AsNoTracking().FirstOrDefault();
                    return account;
                }
                else
                {
                    dynamic Response = new ExpandoObject();

                    Response.Error = true;
                Response.Message = Symposium.Resources.Messages.BARCODENOTREGISTERED;
                    return Response;
                }
        }

        // GET api/CreditCodes
        public IEnumerable<CreditCodes> GetCreditCodes(string storeid)
        {
            return db.CreditCodes.AsEnumerable();
        }

        // GET api/CreditCodes/5
        public CreditCodes GetCreditCodes(long id, string storeid)
        {
            CreditCodes CreditCodes = db.CreditCodes.Find(id);
            if (CreditCodes == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return CreditCodes;
        }

        // PUT api/CreditCodes/5
        public HttpResponseMessage PutCreditCodes(long id, string storeid, CreditCodes CreditCodes)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != CreditCodes.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(CreditCodes).State = EntityState.Modified;

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

        // POST api/CreditCodes
        public HttpResponseMessage PostCreditCodes(CreditCodes CreditCodes, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.CreditCodes.Add(CreditCodes);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, CreditCodes);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = CreditCodes.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/CreditCodes/5
        public HttpResponseMessage DeleteCreditCodes(long id, string storeid)
        {
            CreditCodes CreditCodes = db.CreditCodes.Find(id);
            if (CreditCodes == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.CreditCodes.Remove(CreditCodes);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, CreditCodes);
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