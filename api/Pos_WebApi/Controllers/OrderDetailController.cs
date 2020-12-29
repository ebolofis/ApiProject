using Pos_WebApi.Helpers;
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
using Pos_WebApi.Models;
using Microsoft.AspNet.SignalR;
using log4net;
using Symposium.Models.Enums;
using Symposium.Helpers;

namespace Pos_WebApi.Controllers
{
    public class OrderDetailController : ApiController
    {
        //private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
        private PosEntities db;
        private InvoiceRepository repository;
        public OrderDetailController()
        {
            db = new PosEntities(false);
            repository = new InvoiceRepository(db);
        }

        [Route("api/PutOrderDetail/{storeid}/{kdsIdupdate}")]
        [HttpPost]
        //public HttpResponseMessage PutOrderDetail(string storeid, OrderDetail kdsOrderDetail) {
        public HttpResponseMessage PutOrderDetail(string storeid, long kdsIdupdate, IEnumerable<OrderDetail> kdsOrderDetails)
        {
            //IEnumerable<OrderDetail> list = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<OrderDetail>>(kdsOrderDetails);
            db = new PosEntities(false, Guid.Parse(storeid));
            repository = new InvoiceRepository(db);
            bool communicationBetweenKdsAndDelivery = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "CommunicationBetweenKdsAndDelivery");
            try
            {
                DateTime newdt = DateTime.Now;
                var odsres = repository.KdsUpdateOrderDetails(kdsOrderDetails, newdt);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }
                hub.Clients.Group(storeid).kdsMessage(storeid, newdt);
                bool sendSignalRToDispatcherFlag = false;
                List<OrderDetail> tmpOrderDetailList = new List<OrderDetail>();
                tmpOrderDetailList = kdsOrderDetails.AsEnumerable().ToList();
                var ordId = tmpOrderDetailList[0].Id;
                long orderId = (long)db.OrderDetail.Where(w => w.Id == ordId).Select(s => s.OrderId).FirstOrDefault();
                IEnumerable<OrderDetail> ordList = db.OrderDetail.Where(w => w.OrderId == orderId).AsEnumerable().Select(s => s).ToList();
                foreach (OrderDetail ord in ordList)
                {
                    sendSignalRToDispatcherFlag = true;
                    if (ord.Status == 1 && ord.KdsId != null)
                    {
                        sendSignalRToDispatcherFlag = false;
                        break;
                    }
                }
                if (sendSignalRToDispatcherFlag == true)
                {
                    if (communicationBetweenKdsAndDelivery == true)
                    {
                        List<long> KdsOrdersIdList = new List<long>();
                        KdsOrdersIdList = KdsOrderIdListHelper.addKdsOrderIdsToList(orderId);
                        logger.Info("KDS Send Message To Dispatcher for OrderId = " + orderId);
                        hub.Clients.Group(storeid).kdsMessageToDispatcher(storeid, KdsOrdersIdList);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        // GET api/OrderDetail
        public IEnumerable<OrderDetail> GetOrderDetail(string storeid)
        {
            return db.OrderDetail.AsEnumerable();
        }



        // GET api/OrderDetail/5
        public OrderDetail GetOrderDetail(long id, string storeid)
        {
            OrderDetail OrderDetail = db.OrderDetail.Find(id);
            if (OrderDetail == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return OrderDetail;
        }

        // PUT api/OrderDetail/5
        public HttpResponseMessage PutOrderDetail(long id, string storeid, OrderDetail OrderDetail)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != OrderDetail.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(OrderDetail).State = EntityState.Modified;

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

        public string GetOrderDetail(string orderdetails, bool updaterange)
        {
            IEnumerable<OrderDetailUpdateObj> list = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<OrderDetailUpdateObj>>(orderdetails);
            List<OrderDetail> ordetlist = new List<OrderDetail>();
            List<OrderDetail> ordetPaidlist = new List<OrderDetail>();
            decimal amount = 0;
            long updateReceiptNo = -1;
            foreach (var o in list)
            {
                if (o.IsOffline == null || o.IsOffline == false)
                {
                    var ord = db.OrderDetail.Where(w => w.Id == o.Id).FirstOrDefault();
                    if (ord != null)
                    {
                        ord.Status = o.Status;
                        ord.StatusTS = DateTime.Now;
                        if (o.PaidStatus != null)
                            ord.PaidStatus = o.PaidStatus;
                        if (o.Status == 7)//Exoflisi
                        {
                            ord.PaidStatus = 2;//Paid
                            amount += ord.Price != null ? ord.Price.Value : 0;
                            ordetPaidlist.Add(ord);
                        }
                        if (o.Status == 5 && o.PaidStatus == 2)
                        {
                            ordetPaidlist.Add(ord);
                        }
                        ordetlist.Add(ord);
                    }
                }
                else if (o.IsOffline == true && o.AA != null && o.PosId != null && o.OrderNo != null)
                {
                    var ord = db.Order.Include("OrderDetail").Where(w => w.OrderNo == o.OrderNo && w.PosId == o.PosId && w.EndOfDayId == null).FirstOrDefault();
                    if (ord != null)
                    {
                        OrderDetail ordet = new OrderDetail();
                        ordet = ord.OrderDetail.Skip(o.AA.Value).FirstOrDefault();
                        ordet.Status = o.Status;
                        ordet.StatusTS = DateTime.Now;
                        //if (o.Status == 7)//Exoflisi
                        //{
                        //    ordet.PaidStatus = 2;//Paid
                        //}
                        ordetlist.Add(ordet);
                    }
                }
                if (o.OrderDetailInvoices != null && o.OrderDetailInvoices.Count > 0)
                {
                    foreach (var i in o.OrderDetailInvoices)
                    {
                        db.OrderDetailInvoices.Add(i);
                        updateReceiptNo = i.PosInfoDetailId != null ? i.PosInfoDetailId.Value : -1;
                    }
                }
            }

            long newCounter = -1;
            var pid = db.PosInfoDetail.FirstOrDefault();
            if (updateReceiptNo > -1)
            {
                pid = db.PosInfoDetail.Where(w => w.Id == updateReceiptNo).FirstOrDefault();
                newCounter = pid.Counter != null ? (pid.Counter.Value + 1) : 1;
                pid.Counter = newCounter;
                var posinfo = db.PosInfo.Include("PosInfoDetail").Where(w => w.Id == pid.PosInfoId).FirstOrDefault();
                if (posinfo != null)
                {
                    var piDetGroup = posinfo.PosInfoDetail.Where(w => w.GroupId != null && w.GroupId == pid.GroupId);
                    foreach (var i in piDetGroup)
                    {
                        i.Counter = newCounter;
                    }
                }
            }

            var group = ordetlist.GroupBy(g => g.OrderId).Select(s => new
            {
                OrderId = s.Key,
                OrderDetailStatus = s.FirstOrDefault().Status
            });

            foreach (var o in group)
            {
                var order = db.Order.Include("OrderDetail").Include("OrderDetail.OrderDetailIgredients").Include("OrderDetail.OrderDetailInvoices")
                    .Include("OrderDetail.OrderDetailInvoices.PosInfoDetail").Include("OrderStatus")
                    .Include(i => i.PosInfo).SingleOrDefault(s => s.Id == o.OrderId);
                var gr = order.OrderDetail.GroupBy(g => new { g.PaidStatus, g.Status }).Select(s => new
                {
                    PaidStatus = s.Key.PaidStatus,
                    Status = s.Key.Status,
                    Count = s.Count(),
                    PaidOrderDetails = s.Where(w => ordetPaidlist.Contains(w)).AsEnumerable(),
                    OrderDetails = s
                });
                foreach (var g in gr)
                {
                    if (g.Status != 5 && g.PaidStatus == 2 && order.OrderStatus.Where(w => w.Status == 7).Count() == 0)//Paid && AN DEN YPARXEI HDH STATUS EXOFLISIS
                    {
                        Transactions tr = new Transactions();
                        tr.AccountId = list.FirstOrDefault().AccountId != null ? list.FirstOrDefault().AccountId : order.PosInfo.AccountId;
                        tr.PosInfoId = order.PosInfo.Id;
                        //tr.Amount = g.PaidOrderDetails.Sum(sm => sm.Price);
                        tr.Day = DateTime.Now;
                        tr.DepartmentId = order.PosInfo.DepartmentId;
                        tr.Description = "Εξόφληση";
                        if (g.PaidOrderDetails.Count() < order.OrderDetail.Count)
                        {
                            tr.ExtDescription = "Μερική Εξόφληση " + g.PaidOrderDetails.Count() + "/" + order.OrderDetail.Count;
                        }
                        tr.InOut = 0;
                        tr.OrderId = order.Id;
                        tr.StaffId = list.FirstOrDefault().StaffId;
                        tr.TransactionType = 3;
                        Transactions t = new Economic().SetEconomicNumbers(tr, order, db, g.PaidOrderDetails.Select(s => s.Id).ToList());
                        tr.Gross = t.Gross;
                        tr.Amount = t.Gross;
                        tr.Net = t.Net;
                        tr.Tax = t.Tax;
                        tr.Vat = t.Vat;
                        db.Transactions.Add(tr);
                        db.SaveChanges();
                        foreach (var paidDet in g.PaidOrderDetails)
                        {
                            var orderdetailinvoices = paidDet.OrderDetailInvoices.Where(w => w.PosInfoDetail.CreateTransaction == false && w.PosInfoDetail.IsInvoice == true);
                            if (orderdetailinvoices.Count() > 0)
                            {
                                foreach (var orinv in orderdetailinvoices)
                                {
                                    orinv.StaffId = list.FirstOrDefault().StaffId;
                                }
                            }
                            paidDet.TransactionId = tr.Id;
                        }
                        var hotel = db.HotelInfo.FirstOrDefault();
                        var account = db.Accounts.Find(tr.AccountId);
                        if (account != null && account.SendsTransfer == true && hotel!=null)
                        {
                            var query = (from f in order.OrderDetail
                                         join st in db.SalesType on f.SalesTypeId equals st.Id
                                         join tm in db.TransferMappings on new { Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                         from ls in loj.DefaultIfEmpty()
                                         select new
                                         {
                                             Id = f.Id,
                                             SalesTypeId = st.Id,
                                             Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (double)(((double)f.Price * f.Qty) + (double)(f.OrderDetailIgredients.Sum(sum => sum.Price))) : (double)f.Price * f.Qty,
                                             PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                             PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                             CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId
                                         }).GroupBy(ag => ag.PmsDepartmentId).Select(s => new
                                         {
                                             PmsDepartmentId = s.Key,
                                             PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                             Total = s.Sum(sm => sm.Total),
                                             CustomerId = s.FirstOrDefault().CustomerId
                                         });
                            Customers curcustomer = list.FirstOrDefault().Customer;


                            List<TransferObject> objTosendList = new List<TransferObject>();
                            string storeid = HttpContext.Current.Request.Params["storeid"];
                            foreach (var acg in query)
                            {
                                TransferToPms tpms = new TransferToPms();
                                tpms.Description = "Rec: " + newCounter + ", Pos: " + order.PosInfo.Id + ", PosName: " + order.PosInfo.Description;
                                tpms.PmsDepartmentId = acg.PmsDepartmentId;
                                tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
                                tpms.ProfileId = acg.CustomerId;
                                tpms.ProfileName = curcustomer != null ? curcustomer.FirstName + " "+curcustomer.LastName : "";
                                tpms.ReceiptNo = newCounter.ToString();
                                tpms.RegNo = curcustomer != null ? curcustomer.ReservationCode : "";
                                tpms.RoomDescription = curcustomer != null ? curcustomer.Room : "";
                                tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                                tpms.SendToPMS = true;
                                tpms.TransactionId = tr.Id;
                                tpms.TransferType = 0;//Xrewstiko
                                tpms.PmsDepartmentDescription = acg.PmsDepDescription;
                                tpms.Total = (decimal)acg.Total;
                                var identifier = Guid.NewGuid();
                                tpms.TransferIdentifier = identifier;
                                db.TransferToPms.Add(tpms);
                                TransferObject to = CreateHelperObjects.CreateTransferObject(hotel, false, "", tpms, tpms.TransferIdentifier.Value);
                               // TransferObject to = new TransferObject();
                                //
                               // to.TransferIdentifier = tpms.TransferIdentifier;
                                //
                               // to.HotelId = (int)hotel.Id;
                              //  to.amount = (decimal)tpms.Total;
                             //   int PmsDepartmentId = 0;
                             //   var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
                             //   to.departmentId = PmsDepartmentId;
                             //   to.description = tpms.Description;
                            //    to.profileName = tpms.ProfileName;
                            //    int resid = 0;
                            //    var toint = int.TryParse(tpms.RegNo, out resid);
                            //    to.TransferIdentifier = identifier;
                           //     to.resId = resid;

                            //    to.HotelUri = hotel.HotelUri;

                                // 3
                                
                               // SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                               // sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                            }

                            db.SaveChanges();
                            foreach (var to in objTosendList)
                            {
                                SendTransfer sendserv = new SendTransfer(CreateTransfer.CreatePMSTransfer);
                                //sendserv.BeginInvoke(to, storeid, new AsyncCallback(SendTransferCallback), sendserv);
                            }
                        }
                    }
                    if (g.Status == 5 && g.PaidStatus == 2)//Einai Akyrwseis kai einai exoflimena
                    {
                        Transactions tr = new Transactions();
                        tr.AccountId = list.FirstOrDefault().AccountId != null ? list.FirstOrDefault().AccountId : order.PosInfo.AccountId;
                        tr.PosInfoId = order.PosInfo.Id;
                        //tr.Amount = g.PaidOrderDetails.Sum(sm => sm.Price) * -1;
                        tr.Day = DateTime.Now;
                        tr.DepartmentId = order.PosInfo.DepartmentId;
                        tr.Description = "Ακύρωση";
                        if (g.PaidOrderDetails.Count() < order.OrderDetail.Count)
                        {
                            tr.ExtDescription = "Μερική Ακύρωση " + g.PaidOrderDetails.Count() + "/" + order.OrderDetail.Count;
                        }
                        tr.InOut = 1;//Εκροη
                        tr.OrderId = order.Id;
                        tr.StaffId = list.FirstOrDefault().StaffId;
                        tr.TransactionType = (short)TransactionTypesEnum.Cancel;
                        Transactions t = new Economic().SetEconomicNumbers(tr, order, db, g.PaidOrderDetails.Select(s => s.Id).ToList());
                        //tr.TransactionType = (short)TransactionType.Cancel;
                        tr.Gross = t.Gross;
                        tr.Amount = t.Gross;
                        tr.Net = t.Net;
                        tr.Tax = t.Tax;
                        tr.Vat = t.Vat;
                        db.Transactions.Add(tr);
                        db.SaveChanges();
                        var account = db.Accounts.Find(tr.AccountId);
                        if (account != null && account.SendsTransfer == true)
                        {
                            var query = (from f in order.OrderDetail
                                         join st in db.SalesType on f.SalesTypeId equals st.Id
                                         join tm in db.TransferMappings on new { SalesType = st.Id, Product = f.ProductId, Pricelist = f.PricelistDetail.PricelistId, PosDepartmentId = f.Order.PosInfo.DepartmentId } equals new { SalesType = tm.SalesTypeId.Value, Product = tm.ProductId, Pricelist = tm.PriceListId, PosDepartmentId = tm.PosDepartmentId } into loj
                                         from ls in loj.DefaultIfEmpty()
                                         select new
                                         {
                                             Id = f.Id,
                                             SalesTypeId = st.Id,
                                             Total = f.OrderDetailIgredients != null && f.OrderDetailIgredients.Count > 0 ? (double)(((double)f.Price * f.Qty) + (double)(f.OrderDetailIgredients.Sum(sum => sum.Price))) : (double)f.Price * f.Qty,
                                             PmsDepartmentId = ls != null ? ls.PmsDepartmentId : null,
                                             PmsDepDescription = ls != null ? ls.PmsDepDescription : null,
                                             CustomerId = f.OrderDetailInvoices.FirstOrDefault().CustomerId
                                         }).GroupBy(ag => ag.PmsDepartmentId).Select(s => new
                                         {
                                             PmsDepartmentId = s.Key,
                                             PmsDepDescription = s.FirstOrDefault().PmsDepDescription,
                                             Total = s.Sum(sm => sm.Total),
                                             CustomerId = s.FirstOrDefault().CustomerId
                                         });
                            Customers curcustomer = list.FirstOrDefault().Customer;
                            foreach (var acg in query)
                            {
                                TransferToPms tpms = new TransferToPms();
                                tpms.Description = "Rec: " + newCounter + ", Pos: " + order.PosInfo.Id;
                                tpms.PmsDepartmentId = acg.PmsDepartmentId;
                                tpms.PosInfoDetailId = updateReceiptNo > -1 ? pid.Id : -1;
                                tpms.ProfileId = list.FirstOrDefault().OrderDetailInvoices != null && list.FirstOrDefault().OrderDetailInvoices.Count > 0 ? list.FirstOrDefault().OrderDetailInvoices.FirstOrDefault().CustomerId : null;//acg.CustomerId;
                                tpms.ProfileName = curcustomer != null ? curcustomer.FirstName + " " + curcustomer.LastName : "";
                                tpms.ReceiptNo = newCounter.ToString();
                                tpms.RegNo = curcustomer != null ? curcustomer.ReservationId.ToString() : "";
                                tpms.RoomDescription = curcustomer != null ? curcustomer.Room : "";
                                tpms.RoomId = curcustomer != null ? curcustomer.RoomId.ToString() : "";
                                tpms.SendToPMS = true;
                                tpms.TransactionId = tr.Id;
                                tpms.TransferType = 0;//Xrewstiko
                                tpms.Total = (decimal)acg.Total * -1;
                                db.TransferToPms.Add(tpms);
                            }
                        }
                    }
                    if (g.Count == order.OrderDetail.Count) // OLA TA OrderDetail THS PARAGGELIAS EINAI STO GROUPAKI
                    {
                        //Add New Order Status
                        OrderStatus os = new OrderStatus();
                        os.OrderId = order.Id;
                        if (g.Status == 3 || g.Status == 2 || g.Status == 5)
                        {
                            os.Status = g.Status;
                        }
                        else
                        {
                            if (g.PaidStatus == 2)//paid
                            {
                                os.Status = 7;//"Εξοφλημένη";
                                if (order.OrderStatus.Where(w => w.Status == 8).Count() == 0)//AN EINAI EXOFLISI KAI DEN EXEI TIMOLOGITHEI
                                {
                                    OrderStatus os8 = new OrderStatus(); //PROSTHESE STATUS TIMOLOGISHS (8)
                                    os8.OrderId = order.Id;
                                    os8.Status = 8;
                                    os8.StaffId = list.FirstOrDefault().StaffId;
                                    os8.TimeChanged = DateTime.Now;
                                    db.OrderStatus.Add(os8);
                                }
                            }
                            else if (g.PaidStatus == 1)//invoiced
                            {
                                os.Status = 8;// "Τιμολογημένη";
                            }
                            else
                            {
                                os.Status = g.Status;
                            }
                        }
                        os.TimeChanged = DateTime.Now;
                        os.StaffId = list.FirstOrDefault().StaffId;
                        if (order.OrderStatus.Where(s => s.Status == os.Status).Count() == 0)//VALE TO NEO OrserStatus MONO AN DEN YPARXEI HDH
                        {
                            db.OrderStatus.Add(os);
                        }
                    }
                }
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return ex.ToString();
            }

            return "{\"result\":\"OK\"}";
        }

      

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
                        originalTransfer.SendToPmsTS = DateTime.Now;
                        originalTransfer.ErrorMessage = res.TransferErrorMessage;
                        originalTransfer.PmsResponseId = res.TransferResponseId;

                    }

                    ctx.SaveChanges();
                }
            }

        }



        // POST api/OrderDetail
        public HttpResponseMessage PostOrderDetail(OrderDetail OrderDetail, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.OrderDetail.Add(OrderDetail);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, OrderDetail);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = OrderDetail.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }


        // DELETE api/OrderDetail/5
        public HttpResponseMessage DeleteOrderDetail(long id, string storeid)
        {
            OrderDetail OrderDetail = db.OrderDetail.Find(id);
            if (OrderDetail == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.OrderDetail.Remove(OrderDetail);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, OrderDetail);
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

     

        public class OrderDetailUpdateObj
        {
            public long Id { get; set; }
            public int? AA { get; set; }
            public bool? IsOffline { get; set; }
            public long? PosId { get; set; }
            public int? OrderNo { get; set; }
            public byte Status { get; set; }
            public DateTime StatusTS { get; set; }
            public long? StaffId { get; set; }
            public byte? PaidStatus { get; set; }
            public ICollection<OrderDetailInvoices> OrderDetailInvoices { get; set; }
            public long? AccountId { get; set; }
            public Customers Customer { get; set; }
        }
    }
}