using Dapper;
using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT.TableReservations
{
    public class ReservationsDT : IReservationsDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public ReservationsDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns True or False, True in case that Reservation is in Restaurants Trading Hours else False
        /// </summary>
        /// <returns></returns>
        public bool IsRestaurantOpen(DBInfoModel Store)
        {
            bool res = false;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                TimeSpan timeFrom = db.Query<TimeSpan>("SELECT timeFrom FROM TR_TradingHours").FirstOrDefault();
                TimeSpan timeTo = db.Query<TimeSpan>("SELECT timeTo FROM TR_TradingHours").FirstOrDefault();
                TimeSpan now = DateTime.Now.TimeOfDay;

                if(timeFrom <= timeTo)
                {
                    if ( now >= timeFrom && now <= timeTo)
                    {
                        res = true;
                    }
                    else
                    {
                        res = false;
                        throw new Exception(Symposium.Resources.Errors.OUTOFTRADINGHOURS);
                    }
                }
                else
                {
                    if(now >= timeFrom || now <= timeTo)
                    {
                        res = true;
                    }
                    else
                    {
                        res = false;
                        throw new Exception(Symposium.Resources.Errors.OUTOFTRADINGHOURS);
                    }
                }  
            }

            return res;
        }

        /// <summary>
        /// Returns the List of Reservations
        /// </summary>
        /// <returns></returns>
        public ReservationsListModel GetReservations(DBInfoModel Store)
        {
            // get the results
            ReservationsListModel reservationsList = new ReservationsListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<ReservationsModel> resDetails = db.Query<ReservationsModel>("SELECT * FROM TR_Reservations AS tr").ToList();
                reservationsList.ReservationsModelList = resDetails;
            }

            return reservationsList;
        }

        /// <summary>
        /// Returns details for a specific Reservation 
        /// </summary>
        /// <param name="Id">ReservationID</param>
        /// <returns></returns>
        public ReservationsModel GetReservationById(DBInfoModel Store, long Id)
        {
            // get the results
            ReservationsModel reservationsDetails = new ReservationsModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                ReservationsModel reservDetails = db.Query<ReservationsModel>("SELECT * FROM TR_Reservations AS tr WHERE tr.Id =@ID", new { ID = Id }).FirstOrDefault();
                reservationsDetails = reservDetails;
            }

            return reservationsDetails;
        }

        /// <summary>
        /// Return the List of Available Restaurants Dates And Time
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public AvailabilityListModel GetAvailability(DBInfoModel Store, string TotProfiles, string TotRooms, int Paxes, string language)
        {
            // get the results
            AvailabilityListModel availabilityDetails = new AvailabilityListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string availabilityQuery = @"EXEC TR_GetAvailableRestaurantsDatesAndTime @TotProfiles =@totProfiles, @TotRooms =@totRooms, 
                                                                                         @Paxes =@paxes, @dur =@Dur, 
                                                                                         @sLang =@setLang, @RestId = -1 , @ActiveDate = ''";
                string durationQuery = "SELECT tc.PreviewDays FROM TR_Config AS tc";
                int duration = db.Query<int>(durationQuery).FirstOrDefault();
                List<AvailabilityModel> availabilityList = db.Query<AvailabilityModel>(availabilityQuery, new
                {
                    totProfiles = TotProfiles,
                    totRooms = TotRooms,
                    paxes = Paxes,
                    Dur = duration,
                    setLang = language
                }).ToList();

                availabilityDetails.AvailabilityModelList = availabilityList;
            }

            return availabilityDetails;
        }

        /// <summary>
        /// Return the List of Available Restaurants Dates And MaxTime
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public AvailabilityListModelMaxTime GetMaxTimeAvailability(DBInfoModel Store, string TotProfiles, string TotRooms, int Paxes, string language)
        {
            // get the results
            AvailabilityListModelMaxTime availabilityDetails = new AvailabilityListModelMaxTime();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string availabilityQuery = @"EXEC TR_GetAvailableRestaurantsDatesAndTime @TotProfiles =@totProfiles, @TotRooms =@totRooms, 
                                                                                         @Paxes =@paxes, @dur =@Dur, 
                                                                                         @sLang =@setLang, @RestId = 0 , @ActiveDate = ''";
                string durationQuery = "SELECT tc.PreviewDays FROM TR_Config AS tc";
                int duration = db.Query<int>(durationQuery).FirstOrDefault();
                List<AvailabilityModelMaxTime> availabilityList = db.Query<AvailabilityModelMaxTime>(availabilityQuery, new
                {
                    totProfiles = TotProfiles,
                    totRooms = TotRooms,
                    paxes = Paxes,
                    Dur = duration,
                    setLang = language
                }).ToList();

                availabilityDetails.AvailabilityModelListMaxTime = availabilityList;
            }

            return availabilityDetails;
        }

        /// <summary>
        /// Data Access object filters reservations from to date and in range of ids provided or all
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ReservationsAndCustomersListModel GetFilteredReservations(DBInfoModel Store, ReservationFilter filter)
        {

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string q = @"Select * from TR_Reservations where ReservationDate >= @fromDate and ReservationDate <= @toDate ";
                string qid = " ( Select Id from TR_Reservations where ReservationDate >= @fromDate and ReservationDate <= @toDate ";
                if (filter.Restaurants.Count > 0)
                {
                    q += " and RestId in @resIds ";
                    qid += " and RestId in @resIds ";
                }
                q += " order by ReservationDate , ReservationTime ";
                qid += " ) ";

                List<ReservationsModel> res = db.Query<ReservationsModel>(q, new { fromDate = filter.FromDate.Date, toDate = filter.ToDate.Date, resIds = filter.Restaurants }).ToList();

                string getReservationCustomersQuery = @"OPEN SYMMETRIC KEY KeyKhwp
                                                           DECRYPTION BY CERTIFICATE WHPMAIN
                                                           with PASSWORD  = 'pH!t8bb928DGvbD2439h0Ty';
                                                                select id
                                                                    , protelid
                                                                    ,CONVERT(varchar, DecryptByKey(Protelname)) as ProtelName
                                                                    ,CONVERT(varchar, DecryptByKey(reservationname)) as ReservationName
                                                                    ,roomnum
                                                                    ,cast(DecryptByKey(Email) as varchar) AS Email
                                                                    ,reservationid
                                                                    ,hotelid
                                                                from TR_ReservationCustomers
                                                                where ReservationId in 
                                                          ";
                getReservationCustomersQuery += qid;
                List<ReservationCustomersModel> resCustomersInfo = db.Query<ReservationCustomersModel>(
                        getReservationCustomersQuery, new { fromDate = filter.FromDate.Date, toDate = filter.ToDate.Date, resIds = filter.Restaurants }
                        ).ToList();

                return new ReservationsAndCustomersListModel() { ReservationsModelList = res, ReservationsCustomerModelList = resCustomersInfo };
            }
        }

        /// <summary>
        /// Return the List of Available Times for a specific Restaurant
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <param name="RestId"></param>
        /// <param name="ActiveDate"></param>
        /// <returns></returns>
        public RestaurantAvailabilityListModel GetRestaurantAvailability(DBInfoModel Store, string TotProfiles, string TotRooms, int Paxes, string language, long RestId, DateTime ActiveDate)
        {
            // get the results
            RestaurantAvailabilityListModel restaurantavailabilityDetails = new RestaurantAvailabilityListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string restaurantAvailabilityQuery = @"EXEC TR_GetAvailableRestaurantsDatesAndTime @TotProfiles =@totProfiles, @TotRooms =@totRooms,
                                                                                                   @Paxes =@paxes, @dur =@Dur, 
                                                                                                   @sLang =@setLang, @RestId =@restId , @ActiveDate =@activeDate";
                string durationQuery = "SELECT tc.PreviewDays FROM TR_Config AS tc";
                int duration = db.Query<int>(durationQuery).FirstOrDefault();
                List<RestaurantAvailabilityModel> restaurantAvailabilityList = db.Query<RestaurantAvailabilityModel>(restaurantAvailabilityQuery, new
                {
                    totProfiles = TotProfiles,
                    totRooms = TotRooms,
                    paxes = Paxes,
                    Dur = duration,
                    setLang = language,
                    restId = RestId,
                    activeDate = ActiveDate
                }).ToList();
                restaurantavailabilityDetails.RestaurantAvailabilityModelList = restaurantAvailabilityList;
            }

            return restaurantavailabilityDetails;
        }


        /// <summary>
        /// Update a Reservation
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ReservationsModel UpdateReservation(DBInfoModel Store, ReservationsModel Model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"UPDATE TR_Reservations SET RestId=@RestId, CapacityId=@CapacityId, Couver=@Couver , ReservationDate=@ReservationDate ,
                                       ReservationTime=@ReservationTime, CreateDate=@CreateDate, [Status]=@Status WHERE Id=@ID";

                db.Query(updateQuery, new
                {
                    ID = Model.Id,
                    RestId = Model.RestId,
                    CapacityId = Model.CapacityId,
                    Couver = Model.Couver,
                    ReservationDate = Model.ReservationDate,
                    ReservationTime = Model.ReservationTime,
                    CreateDate = Model.CreateDate,
                    Status = Model.Status
                });

                return Model;
            }
        }

        /// <summary>
        /// Delete a Reservation
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteReservation(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM TR_Reservations WHERE Id=@ID";
                db.Query(deleteQuery, new { ID = Id });
                res = Id;
                return res;
            }
        }

        /// <summary>
        /// Deleting old Reservations from DB
        /// </summary>
        /// <returns></returns>
        public bool DeleteOldReservations(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        //Delete Customers Informations
                        DeleteReservationCustomers(Store);

                        //Delete Reservations Informations
                        DeleteReservations(Store);
                    }
                    catch
                    {
                        throw new Exception("Old Reservations can not Deleted...!");
                    }
                    // Commit transaction
                    scope.Complete();
                }
            }
            return true;
        }


        #region "Private Members"

        private void DeleteReservationCustomers(DBInfoModel Store)
        {
            List<long> reservationIds = new List<long>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string reservationIdsQuery = @"SELECT tr.Id FROM TR_Reservations AS tr WHERE tr.ReservationDate < GETDATE()";
                reservationIds = db.Query<long>(reservationIdsQuery).ToList();
                foreach (long resId in reservationIds)
                {
                    string deleteReservationCustQuery = @"DELETE FROM TR_ReservationCustomers WHERE ReservationId =@resID";
                    db.Query(deleteReservationCustQuery, new { resID = resId });
                }
            }
        }

        private void DeleteReservations(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteReservationsQuery = @"DELETE FROM TR_Reservations WHERE ReservationDate < GETDATE()";
                db.Query(deleteReservationsQuery);
            }
        }
        #endregion
    }
}
