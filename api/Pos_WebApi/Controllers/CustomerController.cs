using log4net;
using Pos_WebApi.Helpers;
using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class CustomerController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<Guest> Get()
        {
            return db.Guest.Take(21);
        }

        /// <summary>
        /// Return customers from various sources 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <param name="name"></param>
        /// <param name="customerPolicy"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public Object GetCustomer(long id, string room, string name, byte customerPolicy, int page, int pagesize)
        {
            logger.Info("GetCustomer - Id:" + id.ToString() + ", name:" + (name ?? "<null>") + ", room:" + (room ?? "<null>") + ", page:" + page.ToString() + ",pagesize:" + pagesize.ToString());
            switch ((CustomerPolicyEnum)customerPolicy)
            {
                case CustomerPolicyEnum.NoCustomers:
                    NoProvider np = new NoProvider(db);
                    logger.Info("CustomerPolicy: NoCustomers");
                    return np.GetCustomers(id, name, room, page, pagesize);
                case CustomerPolicyEnum.HotelInfo:
                    logger.Info("CustomerPolicy: HotelInfo");
                    HotelInfoCustomers hic = new HotelInfoCustomers(db);
                    return hic.GetCustomers(id, name, room, page, pagesize);
                case CustomerPolicyEnum.Other:
                case CustomerPolicyEnum.Delivery:
                    logger.Info("CustomerPolicy: Other, Delivery");
                    DeliveryCustomers dp = new DeliveryCustomers(db);
                    return dp.GetCustomers(id, name, room, page, pagesize);
                case CustomerPolicyEnum.PmsInterface:
                    logger.Info("CustomerPolicy: PmsInterface");
                    PMSProvider pm = new PMSProvider(db);
                    return pm.GetCustomers(id, name, room, page, pagesize);
                default:
                    logger.Info("CustomerPolicy: -");
                    return null;
            }
        }

        /// <summary>
        /// update customer info 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Store/5
        public HttpResponseMessage PutCustomer(long id, string storeid, Customers model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != model.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var hi = db.HotelInfo.FirstOrDefault();
            if (hi != null)
            {
                switch ((CustomerPolicyEnum)hi.Type)
                {
                    case CustomerPolicyEnum.NoCustomers:
                       // NoProvider np = new NoProvider(db);
                       // return np.GetCustomers(id, name, room, page, pagesize);
                    case CustomerPolicyEnum.HotelInfo:
                       // HotelInfoCustomers hic = new HotelInfoCustomers(db);
                       // return hic.GetCustomers(id, name, room, page, pagesize);
                    case CustomerPolicyEnum.Other:
                    case CustomerPolicyEnum.Delivery:
                        DeliveryCustomers dp = new DeliveryCustomers(db);
                        var resid = dp.UpdateCustomer(model);
                        if (resid != 0)
                        {
                            //   HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.Created, model);
                            // resp.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = resid }));

                            model.Id = resid;
                            return Request.CreateResponse(HttpStatusCode.OK, model);
                        }
                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
                }
            }

            var guest = db.Guest.Find(model.Id);
            if (guest != null)
            {
                guest.Id = model.Id;
                guest.FirstName = model.FirstName;
                guest.LastName = model.LastName;
                guest.ProfileNo = model.ProfileNo;
                guest.Note1 = model.Note1;
                guest.Note2 = model.Note2;
                guest.Room = model.Room;
                guest.RoomId = model.RoomId;
                guest.arrivalDT = model.arrivalDT;
                guest.Arrival = model.Arrival;
                guest.departureDT = model.departureDT;
                guest.Departure = model.Departure;
                guest.Country = model.Country;
                guest.City = model.City;
                guest.ConfirmationCode = model.ConfirmationCode;
                guest.Email = model.Email;
                guest.Children = model.Children;
                guest.BoardCode = model.BoardCode;
                guest.BoardName = model.BoardName;
                guest.birthdayDT = model.birthdayDT;
                guest.Adults = model.Adults;
                guest.Address = model.Address;
                guest.ReservationId = model.ReservationId;
                guest.HotelId = model.HotelId;
                guest.ClassId = model.ClassId;
                guest.ClassName = model.ClassName;
                guest.AvailablePoints = model.AvailablePoints;
                guest.fnbdiscount = model.fnbdiscount;
                guest.ratebuy = model.ratebuy;
            }
            //db.Entry(model).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, guest);
        //    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = guest.Id }));
            return Request.CreateResponse(HttpStatusCode.OK, guest);
        }


        /// <summary>
        /// update customer's Points 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeid"></param>
        /// <param name="points"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HttpResponseMessage PutCustomerPoints(long id, string storeid, int points, Customers model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != model.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var guest = db.Guest.Find(model.Id);
            if (guest != null)
            {
                guest.AvailablePoints = points;
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
            return Request.CreateResponse(HttpStatusCode.OK, guest);
        }

        // POST api/Store
        public HttpResponseMessage PostCustomer(Customers model, string customerid)
        {
            if (ModelState.IsValid)
            {
                Guest guest = new Guest();
                if (guest != null)
                {
                    guest.Id = model.Id;
                    guest.FirstName = model.FirstName;
                    guest.LastName = model.LastName;
                    guest.ProfileNo = model.ProfileNo;
                    guest.Note1 = model.Note1;
                    guest.Note2 = model.Note2;
                    guest.Room = model.Room;
                    guest.RoomId = model.RoomId;
                    guest.arrivalDT = model.arrivalDT;
                    guest.Arrival = model.Arrival;
                    guest.departureDT = model.departureDT;
                    guest.Departure = model.Departure;
                    guest.Country = model.Country;
                    guest.City = model.City;
                    guest.ConfirmationCode = model.ConfirmationCode;
                    guest.Email = model.Email;
                    guest.Children = model.Children;
                    guest.BoardCode = model.BoardCode;
                    guest.BoardName = model.BoardName;
                    guest.birthdayDT = model.birthdayDT;
                    guest.Adults = model.Adults;
                    guest.Address = model.Address;
                    guest.ReservationId = model.ReservationId;
                    guest.HotelId = model.HotelId;
                    guest.ClassId = model.ClassId;
                    guest.ClassName = model.ClassName;
                    guest.AvailablePoints = model.AvailablePoints;
                    guest.fnbdiscount = model.fnbdiscount;
                    guest.ratebuy = model.ratebuy;
                }


                db.Guest.Add(guest);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, guest);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = guest.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Store/5
        public HttpResponseMessage DeleteCustomer(long id, string customerid)
        {
            Guest model = db.Guest.Find(id);
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Guest.Remove(model);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, model);
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
