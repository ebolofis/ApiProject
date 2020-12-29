using log4net;
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

namespace Pos_WebApi.Controllers
{
    public class OrderDetailInvoicesController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/OrderDetailInvoices
        public IEnumerable<OrderDetailInvoices> GetOrderDetailInvoices(string storeid)
        {
            return db.OrderDetailInvoices.Take(10).AsEnumerable();
        }

        

        // GET api/OrderDetailInvoices/5
        public OrderDetailInvoices GetOrderDetailInvoices(long id, string storeid)
        {
            OrderDetailInvoices OrderDetailInvoices = db.OrderDetailInvoices.Find(id);
            if (OrderDetailInvoices == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return OrderDetailInvoices;
        }

        [System.Web.Http.HttpPost]
        public Object PostOrders(string ids, int receiptno, long posInfoDetailId) //ONLY CounterNo UPDATE
        {
            //List<long> listId = Newtonsoft.Json.JsonConvert.DeserializeObject<List<long>>(ids);
            //IEnumerable<OrderDetailInvoices> orderInvoices = db.OrderDetailInvoices.Where(w => listId.Contains(w.OrderDetailId.Value) && w.PosInfoDetailId == posInfoDetailId);
            List<Guid> listId = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Guid>>(ids);
            IEnumerable<OrderDetail> orderDetails = db.OrderDetail.Where(w => listId.Contains(w.Guid.Value));
            var list = orderDetails.Select(s=>s.Id).ToList();
            IEnumerable<OrderDetailInvoices> orderInvoices = db.OrderDetailInvoices.Where(w => list.Contains(w.OrderDetailId.Value) && w.PosInfoDetailId == posInfoDetailId);
            if (ids == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ModelState);
            }
            List<long> invlist = new List<long>();
            foreach (var i in orderInvoices)
            {
                i.Counter = receiptno;
                i.IsPrinted = true;
                db.Entry(i).State = EntityState.Modified;
                if (i.InvoicesId != null && invlist.Contains(i.InvoicesId.Value) == false)
                {
                    invlist.Add(i.InvoicesId.Value);
                }
            }
            foreach (var i in invlist)
            {
                var invoice = db.Invoices.Find(i);
                if (invoice != null)
                {
                    invoice.Counter = receiptno;
                    invoice.IsPrinted = true;
                    db.Entry(invoice).State = EntityState.Modified;
                }
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, orderInvoices);
        }

        public Object GetOrders(string ids,bool forstaff, long staffid) //ONLY CounterNo UPDATE
        {
            List<long> listId = Newtonsoft.Json.JsonConvert.DeserializeObject<List<long>>(ids);
            IEnumerable<OrderDetailInvoices> orderInvoices = db.OrderDetailInvoices.Where(w => listId.Contains(w.Id));
            if (ids == null || orderInvoices == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ModelState);
            }
            foreach (var i in orderInvoices)
            {
                i.StaffId = staffid;
                db.Entry(i).State = EntityState.Modified;
            }


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, orderInvoices);
        }

        // PUT api/OrderDetailInvoices/5
        public HttpResponseMessage PutOrderDetailInvoices(long id, string storeid, OrderDetailInvoices OrderDetailInvoices)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != OrderDetailInvoices.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(OrderDetailInvoices).State = EntityState.Modified;

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

        public Object GetOrderDetailInvoices(string orderdetailsinvoices, bool updaterange)
        {
            IEnumerable<OrderDetailInvoices> list = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<OrderDetailInvoices>>(orderdetailsinvoices);
            if (list != null)
            {
                foreach (var o in list)
                {
                    db.OrderDetailInvoices.Add(o);
                }
                long posinfodetid = list.FirstOrDefault().PosInfoDetailId.Value;
                long counter = list.FirstOrDefault().Counter.Value;
                var pid = db.PosInfoDetail.Where(w => w.Id == posinfodetid).SingleOrDefault();
                if (pid != null)
                {
                    pid.Counter = counter;
                }
                db.SaveChanges();
            }
            return new { list };
        }

        // POST api/OrderDetailInvoices
        public HttpResponseMessage PostOrderDetailInvoices(OrderDetailInvoices OrderDetailInvoices, string storeid)
        {
            if (ModelState.IsValid)
            {

                db.OrderDetailInvoices.Add(OrderDetailInvoices);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, OrderDetailInvoices);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = OrderDetailInvoices.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/OrderDetailInvoices/5
        public HttpResponseMessage DeleteOrderDetailInvoices(long id, string storeid)
        {
            OrderDetailInvoices OrderDetailInvoices = db.OrderDetailInvoices.Find(id);
            if (OrderDetailInvoices == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.OrderDetailInvoices.Remove(OrderDetailInvoices);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, OrderDetailInvoices);
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