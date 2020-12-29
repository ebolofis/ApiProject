using log4net;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMSConnectionLib;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using Pos_WebApi.Models.FilterModels;
using Pos_WebApi.Repositories;
using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Goodys;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Pos_WebApi.Controllers
{
    [System.Web.Http.Authorize]
    public class InvoiceForDisplayController : ApiController
    {
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
        //  private PosEntities db;
        //  private InvoiceRepository repository;
        IGoodysFlow goodys;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public InvoiceForDisplayController()
        {
            //    db = new PosEntities(false);
            //   repository = new InvoiceRepository(db);
        }


        //public dynamic GetByFilters(string storeId, string filters)
        //{
        //    var flts = JsonConvert.DeserializeObject<ReceiptFilters>(filters);
        //    flts.EodId = 0;

        //    var results = repository.GetPagedReceiptsByPos(flts.predicate, flts.Page ?? 0, flts.PageSize ?? 10);
        //    return results;
        //}

        /// <summary>
        /// Get Receipts filtered by filters (added Day as filter)
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        [Route("api/{storeId}/InvoiceForDisplay")]
        [HttpPost]
        public dynamic GetByFilters(string storeId, ReceiptFiltersUI filters)
        {

            using (PosEntities db = new PosEntities(false, new Guid(storeId)))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {
                    try
                    {
                        db.Database.CommandTimeout = 300;
                        // repository = new InvoiceRepository(db);
                        var flts = JsonConvert.DeserializeObject<ReceiptFilters>(filters.filt);
                        flts.EodId = 0;
                        var results = repository.GetPagedReceiptsByPos(flts.predicate, flts.Page ?? 0, flts.PageSize ?? 10, flts);
                        return results;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        return null;
                    }
                }

            }
        }

        public class ReceiptFiltersUI
        {
            public int? Page { get; set; }
            public int? PageSize { get; set; }

            public string filt { get; set; }
        }

        // GET api/<controller>
        public dynamic Get(string storeid, long posid, int page, int rows)
        {
            using (PosEntities db = new PosEntities(false))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {

                    var results = repository.GetPagedReceiptsByPos(posid, page, rows, 0);
                    return results;
                }
            }



            //   var results = repository.GetInvoicesByPos(posid, page, rows);
            //if (results != null)
            //{

            //    System.Web.HttpContext.Current.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(results.paginationHeader));
            //    return results;
            //}

            //return null;//results;
        }

        [Route("api/{storeId}/InvoiceForDisplay")]
        [HttpGet]
        public bool HashCodeExists(string storeId, string str)
        {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {
                    return repository.HashCodeExists(str);
                }
            }

            //db = new PosEntities(false, Guid.Parse(storeId));
            //repository = new InvoiceRepository(db);
            //return repository.HashCodeExists(str);
        }

        [Route("api/{storeId}/InvoiceForDisplay/InvoiceIsPaid")]
        [HttpGet]
        public bool InvoiceIsPaid(string storeId, long invoiceId, decimal transAmount)
        {

            using (PosEntities db = new PosEntities(false, Guid.Parse(storeId)))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {
                    return repository.CheckIfPaid(invoiceId, transAmount);
                }
            }
            //db = new PosEntities(false, Guid.Parse(storeId));
            //repository = new InvoiceRepository(db);
            //return repository.CheckIfPaid(invoiceId, transAmount);
        }



        // GET api/<controller>/5
        /// <summary>
        /// Get an invoice by Id
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="id">invoiceId</param>
        /// <param name="forExtecr">true  if the request comes from ExtECT</param>
        /// <returns></returns>
        public object GetInvoiceById(string storeid, Int64 id, bool forExtecr = false)
        {

            using (PosEntities db = new PosEntities(false))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {
                    dynamic results = null;
                    if (!forExtecr)
                        results = repository.GetReceiptDetailsById(id);
                    else
                    {
                        results = repository.GetReceiptDetailsForExtecr(id);
                    }
                    return results;

                }
            }


        }

        /// <summary>
        /// Add new receipt (For Pda and Delivery Service)
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="receipts"></param>
        /// <returns></returns>
        // POST api/<controller>
        public HttpResponseMessage Post(string storeid, IEnumerable<Receipts> receipts)
        {
            using (PosEntities db = new PosEntities(false))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {

                    return repository.MainPostReceiptsAction(storeid, receipts);
                }
            }

        }

        /// <summary>
        /// Add new receipt (Reduce Receipt Time)
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="receipts"></param>
        /// <returns></returns>
        // POST api/<controller>
        [Route("api/InvoiceForDisplay/Post/{storeId}")]
        [HttpPost]
        public HttpResponseMessage Post(string storeid, Object Invoices)
        {
            string strJson = JsonConvert.SerializeObject(Invoices);
            string obj = JToken.Parse(strJson).ToString();
            using (PosEntities db = new PosEntities(false))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {
                    IEnumerable<Receipts> receipts = JsonConvert.DeserializeObject<IEnumerable<Receipts>>(obj,
                        new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    //goodys.UpdateGoodysApi(1234, db);
                    var result = repository.MainPostReceiptsAction(storeid, receipts);
                    return result;

                }
            }


        }

        //Loyalty insert into Actions Table 
        public bool InsertLoyaltyActions(Receipts receipt, HotelInfo hi)
        {
            using (PosEntities db = new PosEntities(false))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {
                    if (receipt.Points != 0)
                    {
                        Guest guest = new Guest();
                        foreach (ReceiptPayments value in receipt.ReceiptPayments)
                        {
                            guest = db.Guest.FirstOrDefault(g => g.Id == value.GuestId);
                        }
                        if (guest.ProfileNo != null)
                        {
                            logger.Info("Customer with id: " + guest.ProfileNo + " complete's points transaction in Action");
                            PMSConnection pmsconnActions = new PMSConnection();
                            string connStr = "server=" + hi.ServerName + ";user id=" + hi.DBUserName +
                                                                                   ";password=" + StringCipher.Decrypt(hi.DBPassword) + ";database=" + hi.DBName + ";";
                            pmsconnActions.initConn(connStr);

                            string notes = "";
                            int points = 0;
                            points = -receipt.Points;
                            string arrival = guest.Arrival.Substring(0, 10);
                            string daparture = guest.Departure.Substring(0, 10);
                            notes = "Reservation Id: " + guest.ReservationId + " Arrival : " + arrival + " Departure : " + daparture + " Voucher : " + guest.ReservationCode;
                            var actionDetails = pmsconnActions.InsertActions(hi.DBUserName, guest.ProfileNo, receipt.Day, 2, points, notes);
                        }
                        logger.Info("No customer found for add points into Action");
                    }
                    else
                    {
                        logger.Info("Points = 0");
                    }
                    return true;

                }
            }



        }


        // Loyalty choose the proper Hotel from HotelInfo 
        public HotelInfo hotelInfo(HotelInfo h)
        {
            HotelInfo hotelI = new HotelInfo();
            PosInfo pos = new PosInfo();

            using (PosEntities db = new PosEntities(false))
            {
                if (h.Type == 4)
                {
                    hotelI = db.HotelInfo.FirstOrDefault();
                    logger.Info("Select HotelId : " + hotelI.HotelId);

                }
                else
                {
                    if (pos.DefaultHotelId != null)
                    {
                        hotelI = db.HotelInfo.FirstOrDefault(x => x.HotelId == pos.DefaultHotelId);
                        logger.Info("Select HotelId : " + hotelI.HotelId);
                    }
                    else
                    {
                        hotelI = db.HotelInfo.FirstOrDefault();
                        logger.Info("Select HotelId : " + hotelI.HotelId);
                    }
                }
                return hotelI;
            }



        }



        [Route("api/{storeId}/{Id}/InvoiceForDisplay/ChangePaymentType")]
        [HttpPost]
        public HttpResponseMessage ChangePaymentTypePost(string storeid, Receipts receipt, long Id)
        {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeid)))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {
                    try
                    {
                        bool res = repository.ChangePaymentType(storeid, receipt, Id);
                        if (!res)
                            return new HttpResponseMessage { StatusCode = HttpStatusCode.MethodNotAllowed };
                        //GetConnections
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.Created };

                }
            }


            //db = new PosEntities(false, Guid.Parse(storeid));
            //repository = new InvoiceRepository(db);
            //try
            //{
            //    bool res = repository.ChangePaymentType(receipt, Id);
            //    if (!res)
            //        return new HttpResponseMessage { StatusCode = HttpStatusCode.MethodNotAllowed };
            //    //GetConnections
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex.ToString());
            //    // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            //}
            //return new HttpResponseMessage { StatusCode = HttpStatusCode.Created };
        }



        [Route("api/{storeId}/InvoiceForDisplay/MarkAsDeleted")]
        [HttpPost]
        public HttpResponseMessage MarkAsDeleted(string storeid, long receiptId)
        {

            using (PosEntities db = new PosEntities(false, Guid.Parse(storeid)))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {

                    try
                    {
                        var tblId = repository.MarkInvoiceAsDeleted(receiptId);
                        try
                        {
                            db.SaveChanges();
                            hub.Clients.Group(storeid).refreshTable(storeid, tblId);
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            logger.Error(ex.ToString());
                            //  Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                        }
                        //GetConnections
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
                }
            }

        }



        // PUT api/<controller>/5
        [HttpPut]
        public HttpResponseMessage Put(string storeid, Receipts r, long newStaffId)
        {
            if (r == null)//(!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            using (PosEntities db = new PosEntities(false))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {
                    repository.ChangeReceiptWaiter(r, newStaffId);
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



        }

        /// <summary>
        /// Is called from EXTecr
        /// Updates receipt with the correct printed status and receiptno
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="receiptId"></param>
        /// <param name="receiptNo"></param>
        /// <param name="isPrinted"></param>
        /// <returns></returns>
       // [Route("api/{storeId}/ReceiptToUpdate/{receiptId}/ReceiptNo/{receiptNo}/IsPrinted/{isPrinted}")]
        [HttpPut]
        public HttpResponseMessage ReceiptCountersPut(string storeid, long receiptId, long receiptNo, bool isPrinted)
        {
            using (PosEntities db = new PosEntities(false))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {
                    string extecrCode = null;
                    var result = repository.UpdateCountersFromFiscal(receiptId, receiptNo, isPrinted, extecrCode);
                    try
                    {
                        repository.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);

                }
            }

        }


        [HttpPut]
        public HttpResponseMessage ReceiptCountersPutWithCode(string storeid, long receiptId, long receiptNo, bool isPrinted, string extecrCode)
        {
            using (PosEntities db = new PosEntities(false))
            {
                using (InvoiceRepository repository = new InvoiceRepository(db))
                {
                    var result = repository.UpdateCountersFromFiscal(receiptId, receiptNo, isPrinted, extecrCode);
                    try
                    {
                        repository.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);

                }
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