using log4net;
using Microsoft.AspNet.SignalR;
using Pos_WebApi.Helpers;
using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalDelivery;
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
    public class OrderStatusController : ApiController
    {
        protected string _storeid;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
       // private PosEntities db;
        IForkeyFlows forkyflow;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static object lockObj = 0;


        public OrderStatusController(IForkeyFlows _forkyflow)
        {
          //  db = new PosEntities(false);
            forkyflow = _forkyflow;
        }
        // GET api/OrderStatus
        public IEnumerable<OrderStatus> GetOrderStatus()
        {
            using (PosEntities db = new PosEntities(false))
            {
                var orderstatus = db.OrderStatus.Include(o => o.Order).Include(o => o.Staff);
                return orderstatus.AsEnumerable();
            }
           
        }

        // GET api/OrderStatus/5
        public OrderStatus GetOrderStatus(long id)
        {
            using (PosEntities db = new PosEntities(false))
            {
                OrderStatus orderstatus = db.OrderStatus.Find(id);
                if (orderstatus == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }
                return orderstatus;
            }
          

            
        }

        // PUT api/OrderStatus/5
        public HttpResponseMessage PutOrderStatus(long id, OrderStatus orderstatus)
        {
            using (PosEntities db = new PosEntities(false))
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                if (id != orderstatus.Id)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                db.Entry(orderstatus).State = EntityState.Modified;

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
        [Route("api/PostOrderStatus/{storeid}/{orderid}/{extType}")]
        [HttpPost]
        public HttpResponseMessage PostOrderStatus(string storeid, long orderid, OrderStatus orderstatus, int? extType)
        {
            using (PosEntities db = new PosEntities(false, Guid.Parse(storeid)))
            {
                if (ModelState.IsValid)
                {
                    extords rets = new extords { data = orderstatus, alsoMsg = false, hitposOrderId = string.Empty };
                    List<long?> del_sel_SalesTypes;
                   
                    try
                    {
                        if (orderstatus.Status == (long)OrderStatusEnum.Ready)
                        {
                            logger.Info("Dispatcher Send Message To KDS for OrderId = " + orderid);
                            hub.Clients.Group(storeid).dispatcherMessageToKds(storeid, orderid);
                            //Send SignalR Signal to Update Kds UI
                            hub.Clients.Group(storeid).kdsSignalToUpdateUI();
                        }
                        lock (lockObj)
                        {

                            var latestsStatus = db.OrderStatus.Where(w => w.OrderId == orderstatus.OrderId).OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().Status;

                            var Dtype = db.HotelInfo.FirstOrDefault().Type;
                            if (Dtype == (int)HotelInfoCustomerPolicyEnum.Delivery)
                            {
                                long rval = (long)OrderStatusEnum.Ready;
                                OrderStatus ordS = db.OrderStatus.Where(qq => qq.OrderId == orderstatus.OrderId && qq.Status == rval).FirstOrDefault();
                                var flag = (ordS == null && orderstatus.Status >= rval && orderstatus.Status != (long)OrderStatusEnum.Canceled) ? true : false;
                                if (flag == true)
                                {
                                    string temp = db.Order.Where(qq => qq.Id == orderstatus.OrderId).FirstOrDefault().ExtKey;
                                    rets.alsoMsg = true;
                                    rets.hitposOrderId = temp;
                                }
                            }
                            DateTime newdt = DateTime.Now;
                            if (latestsStatus == 0 && orderstatus.Status > 0)
                            {
                                List<OrderDetail> ods = db.OrderDetail.Where(w => w.OrderId == orderstatus.OrderId).ToList();
                                foreach (OrderDetail item in ods)
                                {
                                    item.Status = 1;
                                    item.StatusTS = newdt;
                                    db.Entry(item).State = EntityState.Modified;
                                }
                                del_sel_SalesTypes = ods.Select(q => q.SalesTypeId).Distinct().ToList(); //select from order detail their sales types for filter
                            }
                            else
                            {
                                try
                                {
                                    //try collect salesTypes by order Id  for delivery filter entity
                                    del_sel_SalesTypes = db.OrderDetail.Where(od => od.OrderId == orderstatus.OrderId).Select(q => q.SalesTypeId).Distinct().ToList();
                                }
                                catch (Exception ex) { logger.Error(ex.ToString()); del_sel_SalesTypes = new List<long?> { }; }
                            }

                            try
                            {
                                //force this time to be server time and latest as orders for delivery view are sorted by Timechanged
                                orderstatus.TimeChanged = newdt;
                                if (orderstatus.Status != latestsStatus)
                                {
                                    db.OrderStatus.Add(orderstatus);
                                    db.SaveChanges();
                                }
                            }
                            catch (DbUpdateConcurrencyException ex)
                            {
                                logger.Error(ex.ToString());
                                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                            }

                        }
                        // You can call DeliveryForkey Patch 
                        //must not be null, pass parameter from POS
                        long pret;
                        if (extType == 3)
                            pret = forkyflow.PatchOrderFromStatus(storeid, orderid, 3);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                    }
                    hub.Clients.Group(storeid).deliveryMessage(storeid, orderstatus.Status, del_sel_SalesTypes);
                    HttpResponseMessage response = Request.CreateResponse<extords>(HttpStatusCode.Created, rets);
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

            }
           
        }
        private partial class extords
        {
            public OrderStatus data { get; set; }
            public Nullable<bool> alsoMsg { get; set; }
            public string hitposOrderId { get; set; }

        }

        // POST api/OrderStatus
        public HttpResponseMessage PostOrderStatus(OrderStatus orderstatus)
        {
            if (ModelState.IsValid)
            {
                using (PosEntities db = new PosEntities(false))
                {

                    try
                    {
                        var latestsStatus = db.OrderStatus.Where(w => w.OrderId == orderstatus.OrderId).OrderByDescending(mx => mx.TimeChanged).FirstOrDefault().Status;
                        if (latestsStatus == 0 && orderstatus.Status > 0)
                        {
                            List<OrderDetail> ods = db.OrderDetail.Where(w => w.OrderId == orderstatus.OrderId).ToList();
                            DateTime newdt = DateTime.Now;
                            foreach (OrderDetail item in ods)
                            {
                                item.Status = 1;
                                item.StatusTS = newdt;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            //blah 
                        }
                        if (orderstatus.Status != latestsStatus)
                        {
                            db.OrderStatus.Add(orderstatus);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex) { logger.Error(ex.ToString()); }

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, orderstatus);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = orderstatus.Id }));
                    return response;
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        
               
        }

        // DELETE api/OrderStatus/5
        public HttpResponseMessage DeleteOrderStatus(long id)
        {
            using (PosEntities db = new PosEntities(false))
            {
                OrderStatus orderstatus = db.OrderStatus.Find(id);
                if (orderstatus == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                db.OrderStatus.Remove(orderstatus);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.Error(ex.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                return Request.CreateResponse(HttpStatusCode.OK, orderstatus);

            }
           
        }

        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {
          //  db.Dispose();
            base.Dispose(disposing);
        }
    }
}