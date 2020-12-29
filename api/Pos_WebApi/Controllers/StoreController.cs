using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class StoreController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Επιστρέφει τη περιγραφή του καταστήματος (τυπικά επιστρέφει μία εγγραφή) 
        /// </summary>
        /// <remarks>GET api/Store</remarks>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public IEnumerable<Store> GetStores(string storeid)
        {
            return db.Store.AsEnumerable();
        }


        // GET api/Store/5
        public Store GetStore(long id, string storeid)
        {
            Store Store = db.Store.Find(id);
            if (Store == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return Store;
        }

        //CUSTOMERS
        public Object GetStore(long Id, string room, string name)
        {
            HotelInfo hotel = db.Store.Include(i => i.HotelInfo).Where(w => w.Id == Id).FirstOrDefault().HotelInfo.FirstOrDefault();
            if (hotel == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            var gettime1 = DateTime.Now;
            var customers = new List<Customers>();
            if (hotel.Type == 0)
            {
                string url = hotel.HotelUri + "/Public/GetReservationInfo?";
                string roomurl = "";
                if (!String.IsNullOrWhiteSpace(name))
                {
                    roomurl = "&room=" + name;
                }
                if (!String.IsNullOrWhiteSpace(room))
                {
                    roomurl = "&room=" + room;
                }
                url += roomurl + "&hotelid=" + hotel.HotelId;
                using (var w = new WebClient())
                {
                    w.Encoding = System.Text.Encoding.UTF8;
                    var json_data = string.Empty;
                    // attempt to download JSON data as a string
                    try
                    {
                        json_data = w.DownloadString(url);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }
                    // if string with JSON data is not empty, deserialize it to class and return its instance 
                    customers = !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<List<Customers>>(json_data) : new List<Customers>();
                }
            }
            var gettime2 = DateTime.Now;

            List<Guest> glist = new List<Guest>();

            int customersAdded = 0;
            int customersEddited = 0;
            int allCustomers = 0;
            foreach (var c in customers)
            {
                allCustomers++;
                if (c.Room == room || (room == null && name == null) ||( name != null && (c.FirstName.Contains(name) || c.LastName.Contains(name))) )
                {
                    var guest = db.Guest.Where(w => w.ProfileNo == c.ProfileNo && w.ReservationId == c.ReservationId && w.RoomId == c.RoomId
                         && w.FirstName == c.FirstName && w.LastName == c.LastName).FirstOrDefault();
                    Guest g = new Guest();
                    g.arrivalDT = c.arrivalDT; g.departureDT = c.departureDT; g.birthdayDT = c.birthdayDT; g.Room = c.Room;
                    g.RoomId = c.RoomId; g.Arrival = c.Arrival; g.Departure = c.Departure; g.ReservationCode = c.ReservationCode;
                    g.ProfileNo = c.ProfileNo; g.FirstName = c.FirstName; g.LastName = c.LastName; g.Member = c.Member;
                    g.Password = c.Password; g.Address = c.Address; g.City = c.City; g.PostalCode = c.PostalCode;

                    g.Country = c.Country; g.Birthday = c.Birthday; g.Email = c.Email; g.Telephone = c.Telephone;
                    g.VIP = c.VIP; g.Benefits = c.Benefits; g.NationalityCode = c.NationalityCode; g.ConfirmationCode = c.ConfirmationCode;
                    g.Type = c.Type; g.Title = c.Title; g.Adults = c.Adults; g.Children = c.Children;
                    g.BoardCode = c.BoardCode; g.BoardName = c.BoardName; g.Note1 = c.Note1; g.Note2 = c.Note2;
                    g.ReservationId = c.ReservationId; g.IsSharer = c.IsSharer;
                    if (guest == null)
                    {

                        db.Guest.Add(g);
                        customersAdded++;
                    }
                    else
                    {
                        if (guest.ReservationId != c.ReservationId || guest.RoomId != c.RoomId 
                                                                   || guest.BoardCode != c.BoardCode 
                                                                   || guest.Adults != c.Adults 
                                                                   || guest.VIP != c.VIP
                            ) 
                        {
                            customersEddited++;
                            guest.Arrival = g.Arrival;
                            guest.ConfirmationCode = g.ConfirmationCode;
                            guest.Departure = g.Departure;
                            guest.Password = g.Password;
                            guest.ProfileNo = g.ProfileNo;
                            guest.ReservationCode = g.ReservationCode;
                            guest.ReservationId = g.ReservationId;
                            guest.Room = g.Room;
                            guest.RoomId = g.RoomId;
                            guest.Address = g.Address;
                            guest.Adults = g.Adults;
                            guest.Benefits = g.Benefits;
                            guest.Birthday = g.Birthday;
                            guest.BoardCode = g.BoardCode;
                            guest.BoardName = g.BoardName;
                            guest.Children = g.Children;
                            guest.City = g.City;
                            guest.ConfirmationCode = g.ConfirmationCode;
                            guest.Country = g.Country;
                            guest.departureDT = g.departureDT;
                            guest.arrivalDT = g.arrivalDT;
                            guest.Email = g.Email;
                            guest.FirstName = g.FirstName;
                            guest.IsSharer = g.IsSharer;
                            guest.LastName = g.LastName;
                            guest.Member = g.Member;
                            guest.NationalityCode = g.NationalityCode;
                            guest.Note1 = g.Note1;
                            guest.Note2 = g.Note2;
                            guest.PostalCode = g.PostalCode;
                            guest.Telephone = g.Telephone;
                            guest.Title = g.Title;
                            guest.Type = g.Type;
                            guest.VIP = g.VIP;
                        }
                    }
                    glist.Add(guest);
                }



             
            }
            db.SaveChanges();
            var gettime3 = DateTime.Now;

            //glist = customers.Where(w=>w.Room == room || (room == null && name == null)).ToList<Guest>();
            //foreach (var c in customers)
            //{
            //    if (c.Room == room || (room == null && name == null))
            //    {
            //        var guest = db.Guest.Where(w => w.ProfileNo == c.ProfileNo && w.ReservationId == c.ReservationId &&
            //            w.RoomId == c.RoomId && w.FirstName == c.FirstName && w.LastName == c.LastName).FirstOrDefault();
            //        if (guest != null)
            //        {
            //            glist.Add(guest);
            //        }
            //    }
            //}
            //var query = db.Guest.Where(w => customers.Select(s => s.ProfileNo).Contains(w.ProfileNo) && customers.Select(s => s.ReservationId).Contains(w.ReservationId)).ToList();



            //For Testing Only
            if (hotel.Type == 10)
            {
                if (String.IsNullOrEmpty(room))
                    glist = db.Guest.ToList();
                else
                    glist = db.Guest.Where(w => w.Room.ToLower().StartsWith(room.ToLower())).ToList();
            }

            List<Int64> eodids = db.EndOfDay.Where(w => EntityFunctions.TruncateTime(w.FODay) == DateTime.Today.Date).Select(s => s.Id).ToList();
            eodids.Add(0);


            var consumedGrouped = db.MealConsumption.Where(w => eodids.Contains(w.EndOfDayId ?? 0)).GroupBy(g => new { g.GuestId, g.ReservationId })
                                                 .Select(s => new
                                                 {
                                                     //             GuestId = s.FirstOrDefault().GuestId,
                                                     ReservationId = s.FirstOrDefault().ReservationId,
                                                     ConsumedMeals = s.Sum(sm => sm.ConsumedMeals),
                                                     ConsumedMealsChild = s.Sum(sm => sm.ConsumedMealsChild)
                                                 }).ToList();
            var guestWithMeals = (from q in glist
                                  join qq in db.AllowedMealsPerBoard on q.BoardCode equals qq.BoardId into f
                                  from ampb in f.DefaultIfEmpty()
                                  join qqq in consumedGrouped on new { /*GuestId = q.Id,*/ ReservationNo = q.ReservationId }
                                                         equals new {/* GuestId = qqq.GuestId.Value,*/ ReservationNo = qqq.ReservationId }
                                                         into ff
                                  from cm in ff.DefaultIfEmpty()
                                  select new
                                  {
                                      Id = q.Id,
                                      Arrival = q.Arrival,
                                      ConfirmationCode = q.ConfirmationCode,
                                      Departure = q.Departure,
                                      Password = q.Password,
                                      ProfileNo = q.ProfileNo,
                                      ReservationCode = q.ReservationCode,
                                      ReservationId = q.ReservationId,
                                      Room = q.Room,
                                      RoomId = q.RoomId,
                                      Address = q.Address,
                                      Adults = q.Adults,
                                      Benefits = q.Benefits,
                                      Birthday = q.Birthday,
                                      BoardCode = q.BoardCode,
                                      BoardName = q.BoardName,
                                      Children = q.Children,
                                      City = q.City,
                                      // ConfirmationCode = q.ConfirmationCode,
                                      Country = q.Country,
                                      //   departureDT = q.departureDT,
                                      //   arrivalDT = q.arrivalDT,
                                      Email = q.Email,
                                      FirstName = q.FirstName,
                                      IsSharer = q.IsSharer,
                                      LastName = q.LastName,
                                      Member = q.Member,
                                      NationalityCode = q.NationalityCode,
                                      Note1 = q.Note1,
                                      Note2 = q.Note2,
                                      PostalCode = q.PostalCode,
                                      Telephone = q.Telephone,
                                      Title = q.Title,
                                      Type = q.Type,
                                      VIP = q.VIP,
                                      AllowdPriceList = ampb != null ? ampb.PriceListId : null,
                                      AllowdDiscount = ampb != null ? ampb.AllowedDiscountAmount : 0,
                                      AllowedDiscountAmountChild = ampb != null ? ampb.AllowedDiscountAmountChild : 0,
                                      AllowedAdultMeals = ampb != null ? (q.Adults * ampb.AllowedMeals) : 0,
                                      AllowedChildMeals = ampb != null ? (q.Children * ampb.AllowedMealsChild) : 0,
                                      ConsumedMeals = cm != null ? cm.ConsumedMeals : 0,
                                      ConsumedMealsChild = cm != null ? cm.ConsumedMealsChild : 0,

                                  }).Distinct().ToList().GroupBy(g => new { g.Id, g.ReservationId }).Select(s => new Customers
                                 {
                                     Id = s.FirstOrDefault().Id,
                                     Arrival = s.FirstOrDefault().Arrival,
                                     ConfirmationCode = s.FirstOrDefault().ConfirmationCode,
                                     Departure = s.FirstOrDefault().Departure,
                                     Password = s.FirstOrDefault().Password,
                                     ProfileNo = s.FirstOrDefault().ProfileNo,
                                     ReservationCode = s.FirstOrDefault().ReservationCode,
                                     ReservationId = s.FirstOrDefault().ReservationId,
                                     Room = s.FirstOrDefault().Room,
                                     RoomId = s.FirstOrDefault().RoomId,
                                     Address = s.FirstOrDefault().Address,
                                     Adults = s.FirstOrDefault().Adults,
                                     Benefits = s.FirstOrDefault().Benefits,
                                     Birthday = s.FirstOrDefault().Birthday,
                                     BoardCode = s.FirstOrDefault().BoardCode,
                                     BoardName = s.FirstOrDefault().BoardName,
                                     Children = s.FirstOrDefault().Children,
                                     City = s.FirstOrDefault().City,
                                     // ConfirmationCode = s.FirstOrDefault().ConfirmationCode,
                                     Country = s.FirstOrDefault().Country,
                                     //   departureDT = s.FirstOrDefault().departureDT,
                                     //   arrivalDT = s.FirstOrDefault().arrivalDT,
                                     Email = s.FirstOrDefault().Email,
                                     FirstName = s.FirstOrDefault().FirstName,
                                     IsSharer = s.FirstOrDefault().IsSharer,
                                     LastName = s.FirstOrDefault().LastName,
                                     Member = s.FirstOrDefault().Member,
                                     NationalityCode = s.FirstOrDefault().NationalityCode,
                                     Note1 = s.FirstOrDefault().Note1,
                                     Note2 = s.FirstOrDefault().Note2,
                                     PostalCode = s.FirstOrDefault().PostalCode,
                                     Telephone = s.FirstOrDefault().Telephone,
                                     Title = s.FirstOrDefault().Title,
                                     Type = s.FirstOrDefault().Type,
                                     VIP = s.FirstOrDefault().VIP,
                                     AllowdPriceList = s.FirstOrDefault().AllowdPriceList,
                                     AllowdDiscount = s.FirstOrDefault().AllowdDiscount,
                                     AllowdDiscountChild = s.FirstOrDefault().AllowedDiscountAmountChild,
                                     AllowedAdultMeals = s.FirstOrDefault().AllowedAdultMeals - s.Sum(sm => sm.ConsumedMeals),
                                     AllowedChildMeals = s.FirstOrDefault().AllowedChildMeals - s.Sum(sm => sm.ConsumedMealsChild),
                                     ConsumedMeals = s.Sum(sm => sm.ConsumedMeals + sm.ConsumedMealsChild),

                                 });
            var gettime4 = DateTime.Now;

            return guestWithMeals.OrderBy(o => o.Room).Distinct();
        }

        //public Object GetStore(long id, string room, string name, byte customerPolicy)
        //{
        //    switch ((CustomerPolicyEnum)customerPolicy)
        //    {
        //        case CustomerPolicyEnum.NoCustomers:
        //            NoProvider np = new NoProvider(db);
        //            return np.GetCustomers(id, name, room);
        //        case CustomerPolicyEnum.HotelInfo:
        //            HotelInfoCustomers hic = new HotelInfoCustomers(db);
        //            return hic.GetCustomers(id, name, room);
        //        case CustomerPolicyEnum.Other:
        //            break;
        //        case CustomerPolicyEnum.Delivery:
        //            break;
        //        default:
        //            return null;
        //    }
        //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //}


        // PUT api/Store/5
        public HttpResponseMessage PutStore(long id, string storeid, Store Store)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != Store.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(Store).State = EntityState.Modified;

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

        // POST api/Store
        public HttpResponseMessage PostStore(Store Store, string storeid)
        {
            if (ModelState.IsValid)
            {
                db.Store.Add(Store);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, Store);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = Store.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Store/5
        public HttpResponseMessage DeleteStore(long id, string storeid)
        {
            Store Store = db.Store.Find(id);
            if (Store == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Store.Remove(Store);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, Store);
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