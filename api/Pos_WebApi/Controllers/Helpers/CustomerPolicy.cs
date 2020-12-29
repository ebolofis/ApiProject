using log4net;
using Newtonsoft.Json;
using Pos_WebApi.Controllers.Helpers;
using Pos_WebApi.Models;
using Pos_WebApi.Repositories.PMSRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Pos_WebApi.Helpers
{
    public interface ICustomerProvider
    {
        Object GetCustomers(long Id, string name, string room, int page, int pagesize);
    }

    public class HotelInfoCustomers : ICustomerProvider
    {
        PosEntities dbContext;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public HotelInfoCustomers(PosEntities db)
        {
            dbContext = db;
        }

        private IEnumerable<Customers> GetCustomersFromService(string hotelUri, int? hotelId, long Id, string name, string room, int? page = 0, int? pagesize = 12)
        {
            var hi = dbContext.HotelInfo.FirstOrDefault(f => f.HotelId == hotelId);
            if (hi == null)
            {
                throw new Exception(string.Format("Hotel with id {0} not fount on HotelInfo", hotelId));
            }
            //var gettime1 = DateTime.Now;
            var customers = new List<Customers>();

            var protelRep = new ProtelRepository(hi.ServerName,hi.DBUserName,hi.DBPassword,hi.DBName,hi.allHotels,hi.HotelType);
            //string url = hotelUri + "/Public/GetReservationInfo?";
            //string roomurl = "";
            //if (!String.IsNullOrWhiteSpace(name))
            //{
            //    roomurl = "&room=" + name;
            //}
            //if (!String.IsNullOrWhiteSpace(room))
            //{
            //    roomurl = "&room=" + room;
            //}
            //url += roomurl + "&hotelid=" + hotelId + "&pageNo=" + page + "&pagesize=" + pagesize;
            //using (var w = new WebClient())
            //{
            //    w.Encoding = System.Text.Encoding.UTF8;
            //    var json_data = string.Empty;
            //    // attempt to download JSON data as a string
            //    try
            //    {
            //        json_data = w.DownloadString(url);
            //        customers = !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<List<Customers>>(json_data) : new List<Customers>();
            //    }
            //    catch (Exception ex)
            //    {
            //        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("GetCustomersFromService error | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));

            //    }
            //    // if string with JSON data is not empty, deserialize it to class and return its instance 

            //}

            customers = protelRep.GetReservations(hotelId??1,name,room,page,pagesize).ToList();

            return customers;
        }

        public IEnumerable<Customers> GetCustomersFromGuest(long Id, string name, string room, int page, int pagesize)
        {
            var query = dbContext.Guest.Select(s => new Customers
            {
                Id = s.Id,
                LastName = s.LastName,
                FirstName = s.FirstName,
                Room = s.Room,
                RoomId = s.RoomId,
                Address = s.Address,
                Adults = s.Adults,
                arrivalDT = s.arrivalDT,
                departureDT = s.departureDT,
                Arrival = s.Arrival,
                Departure = s.Departure,
                Benefits = s.Benefits,
                BoardCode = s.BoardCode,
                BoardName = s.BoardName,
                Children = s.Children,
                Email = s.Email,
                Note1 = s.Note1,
                Note2 = s.Note2,
                ProfileNo = s.ProfileNo,
                City = s.City,
                ConfirmationCode = s.ConfirmationCode,
                PostalCode = s.PostalCode,
                Country = s.Country,
                Telephone = s.Telephone,
                VIP = s.VIP,
                Type = s.Type,
                ReservationId = s.ReservationId,
                NoPos = 0,
                GuestFuture = ""
            });

            if (!string.IsNullOrEmpty(room))
            {
                query = query.Where(w => w.Room.ToLower().StartsWith(room));
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(w => w.FirstName.ToLower().StartsWith(name) || w.LastName.ToLower().StartsWith(name));
            }
            return query;
        }

        public Object GetCustomers(long Id, string name, string room, int page, int pagesize)
        {
            var pageno = page - 1;

            HotelInfo hotel = dbContext.HotelInfo.Where(w => w.StoreId == Id).FirstOrDefault();
            IEnumerable<Customers> custList;

            if (hotel == null)
            {
                return new
                {
                    Data = new List<Customers>(),
                    TotalPages = 0,
                    PageSize = 0,
                };
            }
            int? totalRecs = 0;
            if (hotel.Type == 10)
            {
                //Changed 4th argument from pageno to page to get the right results
                custList = GetCustomersFromGuest(Id, name, room, page, pagesize);
                totalRecs = custList.Count();
            }
            else
            {
                //Changed 6th argument from pageno to page to get the right results
                custList = GetCustomersFromService(hotel.HotelUri, hotel.HotelId, Id, name, room, page, pagesize);
              if (custList.Count()>0)  totalRecs = custList.FirstOrDefault().TotalRecs;
                if (totalRecs == 0)
                    totalRecs = custList.Count();
            }
            if (custList == null)
            {
                return new
                {
                    Data = new List<Customers>(),
                    TotalPages = 0,
                    PageSize = 0,
                };
            }

            SetCustomersIsGuestFuture(custList);

            // Uncomment this line with the new version of HotelInfo
            var pages = custList.Count() > 0 ? Math.Ceiling((decimal)(totalRecs / pagesize)) : 1;
            //var pages = Math.Ceiling((decimal)(custList.Count() / pagesize));

            if (pages == 0)
                pageno = 0;

            // Uncomment this line with the new version of HotelInfo
            //custList = custList.OrderByDescending(o => o.Room);
            if (hotel.Type == 10)
            {
                pageno++;
                custList = custList.OrderByDescending(o => o.Room).Skip(pageno * pagesize).Take(pagesize);
            }
            else
            {
                custList = custList.OrderByDescending(o => o.Room);

            }


            var consumedGrouped = dbContext.MealConsumption.Where(w => w.EndOfDayId == null).GroupBy(g => new { g.GuestId, g.ReservationId })
                                                 .Select(s => new
                                                 {
                                                     //             GuestId = s.FirstOrDefault().GuestId,
                                                     ReservationId = s.FirstOrDefault().ReservationId,
                                                     ConsumedMeals = s.Sum(sm => sm.ConsumedMeals),
                                                     ConsumedMealsChild = s.Sum(sm => sm.ConsumedMealsChild)
                                                 }).ToList();

            var alloweds = from qq in custList
                           join q in dbContext.AllowedMealsPerBoard on qq.BoardCode equals q.BoardId
                           select new
                           {
                               ReservationId = qq.ReservationId,
                               BoardId = qq.BoardCode,
                               AllowdPriceList = q.PriceListId,
                               AllowedMeals = q.AllowedMeals ?? 0,
                               AllowedMealsChild = q.AllowedMealsChild ?? 0,
                               AllowedDiscountAmount = q.AllowedDiscountAmount ?? 0,
                               AllowedDiscountAmountChild = q.AllowedDiscountAmountChild ?? 0,
                           };

            var tempguest = dbContext.Guest.Select(s => new
            {
                ProfileNo = s.ProfileNo,
                ReservationId = s.ReservationId,
                Id = s.Id
            }).OrderBy(o => o.Id).GroupBy(g => new { g.ProfileNo, g.ReservationId }).Select(ss => new
            {
                ProfileNo = ss.Key.ProfileNo,
                ReservationId = ss.Key.ReservationId,
                Id = ss.FirstOrDefault().Id
            });

            var query = from qq in custList.OrderBy(o => o.Room).ToList()
                        join q in tempguest on new { ProfileNo = qq.ProfileNo, ReservationId = qq.ReservationId } equals new { ProfileNo = q.ProfileNo, ReservationId = q.ReservationId } into f
                        from c in f.DefaultIfEmpty()
                        join qqq in consumedGrouped on qq.ReservationId equals qqq.ReservationId into ff
                        from cg in ff.DefaultIfEmpty()
                        join qqqq in alloweds on qq.ReservationId equals qqqq.ReservationId into fff
                        from allds in fff.DefaultIfEmpty()
                        select new
                        {
                            Id = c != null ? c.Id : 0,
                            Arrival = qq.arrivalDT ?? DateTime.Now,
                            ConfirmationCode = qq.ConfirmationCode,
                            Departure = qq.departureDT ?? DateTime.Now,
                            Password = qq.Password,
                            ProfileNo = qq.ProfileNo,
                            ReservationCode = qq.ReservationCode,
                            ReservationId = qq.ReservationId,
                            Room = qq.Room,
                            RoomId = qq.RoomId,
                            Address = qq.Address,
                            Adults = qq.Adults,
                            Benefits = qq.Benefits,
                            Birthday = qq.Birthday,
                            BoardCode = qq.BoardCode,
                            BoardName = qq.BoardName,
                            Children = qq.Children,
                            City = qq.City,
                            // ConfirmationCode = qq.ConfirmationCode,
                            Country = qq.Country,
                            //   departureDT = qq.departureDT,
                            //   arrivalDT = qq.arrivalDT,
                            Email = qq.Email,
                            FirstName = qq.FirstName ?? "",
                            IsSharer = qq.IsSharer,
                            LastName = qq.LastName ?? "",
                            Member = qq.Member,
                            NationalityCode = qq.NationalityCode,
                            Note1 = StringHelpers.RemoveSpecialCharacters(qq.Note1),
                            Note2 = StringHelpers.RemoveSpecialCharacters(qq.Note2),
                            NoPos = qq.NoPos,
                            PostalCode = qq.PostalCode,
                            Telephone = qq.Telephone,
                            Title = qq.Title,
                            Type = qq.Type,
                            VIP = qq.VIP,
                            AllowdPriceList = allds != null ? allds.AllowdPriceList : 0,
                            AllowdDiscount = allds != null ? allds.AllowedDiscountAmount : 0,
                            AllowdDiscountChild = allds != null ? allds.AllowedDiscountAmountChild : 0,
                            AllowedAdultMeals = allds != null ? cg != null ? (allds.AllowedMeals * qq.Adults) - cg.ConsumedMeals : (allds.AllowedMeals * qq.Adults) : 0,
                            AllowedChildMeals = allds != null ? cg != null ? (allds.AllowedMealsChild * qq.Children) - cg.ConsumedMealsChild : (allds.AllowedMealsChild * qq.Children) : 0,
                            ConsumedMeals = cg != null ? cg.ConsumedMeals + cg.ConsumedMealsChild : 0,

                            //Απαραίτητη πληροφορία loyalty του πελάτη
                            classId = qq.ClassId,
                            ClassName = qq.ClassName,
                            AvailablePoints = qq.AvailablePoints,
                            fnbdiscount = qq.fnbdiscount,
                            ratebuy = qq.ratebuy,
                            TravelAgent = qq.TravelAgent,
                            Company = qq.Company,
                            IsGuestFuture = qq.IsGuestFuture
                        };
            var finalobj = new
            {
                Data = query.Distinct().ToList(),
                TotalPages = pages,
                PageSize = pagesize,
            };


            return finalobj;
        }

        public void SetCustomersIsGuestFuture(IEnumerable<Customers> searchedCustomers)
        {
            List<GuestFuture> guestFutures = dbContext.GuestFuture.ToList();
            foreach (Customers c in searchedCustomers)
            {
                var found = false;
                string[] cFutures = c.GuestFuture.Split(',');
                foreach (GuestFuture gf in guestFutures)
                {
                    foreach (string cf in cFutures)
                    {
                        if (cf.Equals(gf.Description))
                        {
                            found = true;
                        }
                        if (found)
                        {
                            break;
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                }
                c.IsGuestFuture = found;
            }
            return;
        }

    }

    public class NoProvider : ICustomerProvider
    {
        PosEntities dbContext;

        public NoProvider(PosEntities db)
        {
            dbContext = db;
        }

        public Object GetCustomers(long Id, string name, string room, int page, int pagesize)
        {

            IEnumerable<Guest> custList = dbContext.Guest;
            if (!String.IsNullOrEmpty(name))
                custList = custList.Where(w => w.LastName.StartsWith(name));
            if (!String.IsNullOrEmpty(name))
                custList = custList.Where(w => w.Room.StartsWith(room));
            var pageno = page - 1;
            var pages = Math.Ceiling((decimal)(custList.Count() / pagesize));

            if (pages == 0)
                pageno = 0;


            custList = custList.OrderByDescending(o => o.Room).Skip(pageno).Take(pagesize);



            var consumedGrouped = dbContext.MealConsumption.Where(w => w.EndOfDayId == null).GroupBy(g => new { g.GuestId, g.ReservationId })
                                              .Select(s => new
                                              {
                                                  //             GuestId = s.FirstOrDefault().GuestId,
                                                  ReservationId = s.FirstOrDefault().ReservationId,
                                                  ConsumedMeals = s.Sum(sm => sm.ConsumedMeals),
                                                  ConsumedMealsChild = s.Sum(sm => sm.ConsumedMealsChild)
                                              }).ToList();

            var alloweds = from q in dbContext.AllowedMealsPerBoard
                           join qq in custList on q.BoardId equals qq.BoardCode
                           select new
                           {
                               ReservationId = qq.ReservationId,
                               BoardId = qq.BoardCode,
                               AllowdPriceList = q.PriceListId,
                               AllowedMeals = q.AllowedMeals ?? 0,
                               AllowedMealsChild = q.AllowedMealsChild ?? 0,
                               AllowedDiscountAmount = q.AllowedDiscountAmount ?? 0,
                               AllowedDiscountAmountChild = q.AllowedDiscountAmountChild ?? 0,
                           };

            var query = from qq in custList
                        join qqq in consumedGrouped on qq.ReservationId equals qqq.ReservationId into ff
                        from cg in ff.DefaultIfEmpty()
                        join qqqq in alloweds on qq.ReservationId equals qqqq.ReservationId into fff
                        from allds in fff.DefaultIfEmpty()
                        select new
                        {
                            Id = qq.Id,
                            Arrival = qq.Arrival,
                            ConfirmationCode = qq.ConfirmationCode,
                            Departure = qq.Departure,
                            Password = qq.Password,
                            ProfileNo = qq.ProfileNo,
                            ReservationCode = qq.ReservationCode,
                            ReservationId = qq.ReservationId,
                            Room = qq.Room,
                            RoomId = qq.RoomId,
                            Address = qq.Address,
                            Adults = qq.Adults,
                            Benefits = qq.Benefits,
                            Birthday = qq.Birthday,
                            BoardCode = qq.BoardCode,
                            BoardName = qq.BoardName,
                            Children = qq.Children,
                            City = qq.City,
                            // ConfirmationCode = qq.ConfirmationCode,
                            Country = qq.Country,
                            //   departureDT = qq.departureDT,
                            //   arrivalDT = qq.arrivalDT,
                            Email = qq.Email,
                            FirstName = qq.FirstName,
                            IsSharer = qq.IsSharer,
                            LastName = qq.LastName,
                            Member = qq.Member,
                            NationalityCode = qq.NationalityCode,
                            Note1 = StringHelpers.RemoveSpecialCharacters(qq.Note1),
                            Note2 = StringHelpers.RemoveSpecialCharacters(qq.Note2),
                            //Note1 = qq.Note1,
                            //Note2 = qq.Note2,
                            PostalCode = qq.PostalCode,
                            Telephone = qq.Telephone,
                            Title = qq.Title,
                            Type = qq.Type,
                            VIP = qq.VIP,
                            AllowdPriceList = allds != null ? allds.AllowdPriceList : 0,
                            AllowdDiscount = allds != null ? allds.AllowedDiscountAmount : 0,
                            AllowdDiscountChild = allds != null ? allds.AllowedDiscountAmountChild : 0,
                            AllowedAdultMeals = allds != null ? cg != null ? (allds.AllowedMeals * qq.Adults) - cg.ConsumedMeals : (allds.AllowedMeals * qq.Adults) : 0,
                            AllowedChildMeals = allds != null ? cg != null ? (allds.AllowedMealsChild * qq.Children) - cg.ConsumedMealsChild : (allds.AllowedMealsChild * qq.Children) : 0,
                            ConsumedMeals = cg != null ? cg.ConsumedMeals + cg.ConsumedMealsChild : 0,
                        };
            var finalobj = new
            {
                Data = query.Distinct().ToList(),
                TotalPages = pages,
                PageSize = pagesize,
            };
            return finalobj;
        }
    }

    public class DeliveryCustomers : ICustomerProvider
    {
        PosEntities dbContext;
        private readonly AuthenticationHeaderValue ahv = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                                            System.Text.UTF8Encoding.UTF8.GetBytes(string.Format("{0}:{1}", "3", "3"))));
        private readonly HttpClient _client = new HttpClient();
        private string _BaseUri = "";
        private byte _hotelType;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DeliveryCustomers(PosEntities db)
        {
            dbContext = db;
            HotelInfo hotel = dbContext.HotelInfo.FirstOrDefault();
            _BaseUri = hotel.HotelUri;
            _hotelType = hotel.Type ?? 0;
        }

        public object GetCustomers(long Id, string name, string room, int page, int pagesize)
        {
            var session = GetSessionId(room, name);
            var flts = JsonConvert.DeserializeObject<CustomerCardRoot>(session);
            var customers = from q in flts.items
                            select new Customers
                            {
                                FirstName = q.firstName,
                                LastName = q.companyName == null ? q.lastName : q.companyName,
                                Address = q.addresses.FirstOrDefault().cityAddress != null ? q.addresses.FirstOrDefault().cityAddress : "",
                                City = q.addresses.FirstOrDefault().city,
                                ProfileNo = Convert.ToInt32(q.customerId),
                                Country = q.addresses.FirstOrDefault().country,
                                Email = q.email,
                                ConfirmationCode = q.taxNumber,
                                Member = q.taxServiceBranch,
                                Room = q.profession,
                                PostalCode = q.zipCode,
                                Telephone = q.addresses.FirstOrDefault().phonesInAddress.FirstOrDefault().number
                            };


            return customers;
        }


        public Object GetCustomers(long Id, string sessionId, string deliveryFilters)
        {


            return null;
        }

        //public async Task<IEnumerable<dynamic>> GetCustomer()
        //{

        //    HttpResponseMessage response = await _client.WithHeader("Accept", "application/json").GetAsync(_BaseUri);
        //    var json = await response.Content.ReadAsStringAsync();
        //    return JsonConvert.DeserializeObject<IEnumerable<dynamic>>(json);
        //}
        public async Task<IEnumerable<dynamic>> GetCustomerStore()
        {
            HttpResponseMessage response = await _client.GetAsync(_BaseUri);
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<dynamic>>(json);
        }

        /// <summary>
        /// Login to telephone Center based on HotelUri of Hotel Info loaded
        /// </summary>
        /// <param name="room"> Room Here Represents remote_SEARCHFIELD </param>
        /// <param name="name"> Name Here Represents remote_SEARCHVALUE </param>
        /// <returns></returns>
        public string GetSessionId(string room, string name)
        {
            //Construct Connection dependencies to Remote Agent
            string url = _BaseUri;
            var userName = "Agent01"; string userPassword = "111111";
            var _serverApp = "";
            var passwordHash = MD5Helper.CalculateMD5Hash(userPassword);
            var authData = userName + ":" + passwordHash;
            var base64str = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));

            //Apply Header AUTH 4 delivery Agent
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, _BaseUri + @"/sessions");
            requestMessage.Headers.Add("client-app", _serverApp);
            requestMessage.Headers.Add("Authorization", "Basic QWdlbnQwMTo5NmU3OTIxODk2NWViNzJjOTJhNTQ5ZGQ1YTMzMDExMg==");
            //"Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authData)));
            HttpResponseMessage resp = _client.SendAsync(requestMessage).Result;
            var sessionid = resp.Content.ReadAsStringAsync().Result.Replace('"', ' ');

            var searchfield = "taxNumber";
            var serachvalue = room;
            if (!String.IsNullOrEmpty(name))
            {
                searchfield = "lastName";
                serachvalue = name;
            }

            requestMessage = new HttpRequestMessage(HttpMethod.Get, (_BaseUri + @"/customers/search%2Fby%2F/" + searchfield + "?&pageSize=5&searchValue=" + serachvalue));
            requestMessage.Headers.Add("session-id", sessionid);
            resp = _client.SendAsync(requestMessage).Result;
            var json = resp.Content.ReadAsStringAsync().Result;
            return json;

        }

        public long UpdateCustomer(Customers model)
        {
            try
            {
                var guest = dbContext.Guest.Where(w => w.ProfileNo == model.ProfileNo).OrderBy(o => o.Id).FirstOrDefault();
                var guestid = (long)0;

                if (guest != null)
                {
                    guestid = guest.Id;

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

                    dbContext.Entry(guest).State = EntityState.Modified;
                    dbContext.SaveChanges();

                }
                else
                {
                    Guest g = new Guest();
                    g.FirstName = model.FirstName;
                    g.LastName = model.LastName;
                    g.ProfileNo = model.ProfileNo;
                    g.Note1 = model.Note1;
                    g.Note2 = model.Note2;
                    g.Room = model.Room;
                    g.RoomId = model.RoomId;
                    g.arrivalDT = model.arrivalDT;
                    g.Arrival = model.Arrival;
                    g.departureDT = model.departureDT;
                    g.Departure = model.Departure;
                    g.Country = model.Country;
                    g.City = model.City;
                    g.ConfirmationCode = model.ConfirmationCode;
                    g.Email = model.Email;
                    g.Children = model.Children;
                    g.BoardCode = model.BoardCode;
                    g.BoardName = model.BoardName;
                    g.birthdayDT = model.birthdayDT;
                    g.Adults = model.Adults;
                    g.Address = model.Address;
                    g.ReservationId = model.ReservationId;
                    dbContext.Guest.Add(g);
                    dbContext.SaveChanges();
                    guestid = g.Id;
                }


                return guestid;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return 0;
            }

        }
    }

    public class PMSProvider : ICustomerProvider
    {
        PosEntities dbContext;

        public PMSProvider(PosEntities db)
        {
            dbContext = db;
        }

        /// <summary>
        /// Get customers and MealConsumptions
        /// </summary>
        /// <param name="Id">HotelId</param>
        /// <param name="name"></param>
        /// <param name="room"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public Object GetCustomers(long Id, string name, string room, int page, int pagesize)
        {
            var pageno = page - 1;
            var hi = dbContext.HotelInfo.FirstOrDefault(f => f.HotelId == Id);
            if (hi == null)
            {
                throw new Exception(string.Format("Hotel with id {0} not fount on HotelInfo", Id));
            }
            ProtelRepository pr = new ProtelRepository(hi.ServerName, hi.DBUserName, hi.DBPassword,hi.DBName,hi.allHotels,hi.HotelType);
            //   var test = pr.CheckProcedure();
            //if ()
            //var test = pr.GetReservations(hi.MPEHotel.Value, name, room, page, pagesize);
            IEnumerable<Customers> custList = pr.GetReservations((int)Id, name, room, page, pagesize);

            if (hi == null || custList == null || custList.Count() == 0)
            {
                return new
                {
                    Data = new List<Customers>(),
                    TotalPages = 0,
                    PageSize = 0,
                };
            }
            int? totalRecs = totalRecs = custList.FirstOrDefault().TotalRecs;
            if (totalRecs == 0)
                totalRecs = custList.Count();
            if (custList == null)
            {
                return new
                {
                    Data = new List<Customers>(),
                    TotalPages = 0,
                    PageSize = 0,
                };
            }

            SetCustomersIsGuestFuture(custList);

            var pages = custList.Count() > 0 ? Math.Ceiling((decimal)(totalRecs / pagesize)) : 1;

            if (pages == 0)
                pageno = 0;

            custList = custList.OrderByDescending(o => o.Room);

            var consumedGrouped = dbContext.MealConsumption.Where(w => w.EndOfDayId == null).GroupBy(g => new { g.GuestId, g.ReservationId })
                                                 .Select(s => new
                                                 {
                                                     //             GuestId = s.FirstOrDefault().GuestId,
                                                     ReservationId = s.FirstOrDefault().ReservationId,
                                                     ConsumedMeals = s.Sum(sm => sm.ConsumedMeals),
                                                     ConsumedMealsChild = s.Sum(sm => sm.ConsumedMealsChild)
                                                 }).ToList();

            var alloweds = from qq in custList
                           join q in dbContext.AllowedMealsPerBoard on qq.BoardCode equals q.BoardId
                           select new
                           {
                               ReservationId = qq.ReservationId,
                               BoardId = qq.BoardCode,
                               AllowdPriceList = q.PriceListId,
                               AllowedMeals = q.AllowedMeals ?? 0,
                               AllowedMealsChild = q.AllowedMealsChild ?? 0,
                               AllowedDiscountAmount = q.AllowedDiscountAmount ?? 0,
                               AllowedDiscountAmountChild = q.AllowedDiscountAmountChild ?? 0,
                           };

            var tempguest = dbContext.Guest.Select(s => new
            {
                ProfileNo = s.ProfileNo,
                ReservationId = s.ReservationId,
                Id = s.Id
            }).OrderBy(o => o.Id).GroupBy(g => new { g.ProfileNo, g.ReservationId }).Select(ss => new
            {
                ProfileNo = ss.Key.ProfileNo,
                ReservationId = ss.Key.ReservationId,
                Id = ss.FirstOrDefault().Id
            });

            var query = from qq in custList.OrderBy(o => o.Room).ToList()
                        join q in tempguest on new { ProfileNo = qq.ProfileNo, ReservationId = qq.ReservationId } equals new { ProfileNo = q.ProfileNo, ReservationId = q.ReservationId } into f
                        from c in f.DefaultIfEmpty()
                        join qqq in consumedGrouped on qq.ReservationId equals qqq.ReservationId into ff
                        from cg in ff.DefaultIfEmpty()
                        join qqqq in alloweds on qq.ReservationId equals qqqq.ReservationId into fff
                        from allds in fff.DefaultIfEmpty()
                        select new
                        {
                            Id = c != null ? c.Id : 0,
                            Arrival = qq.arrivalDT,
                            ConfirmationCode = qq.ConfirmationCode,
                            Departure = qq.departureDT,
                            Password = qq.Password,
                            ProfileNo = qq.ProfileNo,
                            ReservationCode = qq.ReservationCode,
                            ReservationId = qq.ReservationId,
                            Room = qq.Room,
                            RoomId = qq.RoomId,
                            Address = qq.Address,
                            Adults = qq.Adults,
                            Benefits = qq.Benefits,
                            Birthday = qq.Birthday,
                            BoardCode = qq.BoardCode,
                            BoardName = qq.BoardName,
                            Children = qq.Children,
                            City = qq.City,
                            // ConfirmationCode = qq.ConfirmationCode,
                            Country = qq.Country,
                            //   departureDT = qq.departureDT,
                            //   arrivalDT = qq.arrivalDT,
                            Email = qq.Email,
                            FirstName = qq.FirstName ?? "",
                            IsSharer = qq.IsSharer,
                            LastName = qq.LastName ?? "",
                            Member = qq.Member,
                            NationalityCode = qq.NationalityCode,
                            Note1 = StringHelpers.RemoveSpecialCharacters(qq.Note1),
                            Note2 = StringHelpers.RemoveSpecialCharacters(qq.Note2),
                            NoPos = qq.NoPos,
                            //Note1 = qq.Note1,
                            //Note2 = qq.Note2,
                            PostalCode = qq.PostalCode,
                            Telephone = qq.Telephone,
                            Title = qq.Title,
                            Type = qq.Type,
                            VIP = qq.VIP,
                            AllowdPriceList = allds != null ? allds.AllowdPriceList : 0,
                            AllowdDiscount = allds != null ? allds.AllowedDiscountAmount : 0,
                            AllowdDiscountChild = allds != null ? allds.AllowedDiscountAmountChild : 0,
                            AllowedAdultMeals = allds != null ? cg != null ? (allds.AllowedMeals * qq.Adults) - cg.ConsumedMeals : (allds.AllowedMeals * qq.Adults) : 0,
                            AllowedChildMeals = allds != null ? cg != null ? (allds.AllowedMealsChild * qq.Children) - cg.ConsumedMealsChild : (allds.AllowedMealsChild * qq.Children) : 0,
                            ConsumedMeals = cg != null ? cg.ConsumedMeals + cg.ConsumedMealsChild : 0,

                            //Απαραίτητη πληροφορία loyalty του πελάτη
                            classId = qq.ClassId,
                            ClassName = qq.ClassName,
                            AvailablePoints = qq.AvailablePoints,
                            fnbdiscount = qq.fnbdiscount,
                            ratebuy = qq.ratebuy,
                            TravelAgent = qq.TravelAgent,
                            Company = qq.Company,
                            IsGuestFuture = qq.IsGuestFuture
                        };
            var finalobj = new
            {
                Data = query.Distinct().ToList(),
                TotalPages = pages,
                PageSize = pagesize,
            };


            return finalobj;
        }

        public void SetCustomersIsGuestFuture(IEnumerable<Customers> searchedCustomers)
        {
            List<GuestFuture> guestFutures = dbContext.GuestFuture.ToList();
            foreach (Customers c in searchedCustomers) {
                var found = false;
                string[] cFutures = c.GuestFuture.Split(',');
                foreach (GuestFuture gf in guestFutures)
                {
                    foreach(string cf in cFutures)
                    {
                        if (cf.Equals(gf.Description))
                        {
                            found = true;
                        }
                        if (found)
                        {
                            break;
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                }
                c.IsGuestFuture = found;
            }
            return;
        }

    }

    public class CustomerCardRoot
    {
        public IEnumerable<CustomerCardModel> items { get; set; }
    }
    public class CustomerCardModel
    {
        public string firstName { get; set; }
        //[XmlPath=]
        public string lastName { get; set; }
        public string companyName { get; set; }
        public IEnumerable<CustomerCardAddressModel> addresses { get; set; }
        public string customerId { get; set; }
        public string email { get; set; }
        public string taxNumber { get; set; }
        public string taxServiceBranch { get; set; }
        public string profession { get; set; }
        public string zipCode { get; set; }



    }
    public class CustomerCardAddressModel
    {
        public string cityAddress { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string streetName { get; set; }
        public string streetNumber { get; set; }
        public IEnumerable<CustomerCardPhoneModel> phonesInAddress { get; set; }

    }
    public class CustomerCardPhoneModel
    {
        public string number { get; set; }
        public string phoneType { get; set; }
    }
}

