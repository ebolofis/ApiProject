using Dapper;
using PMSConnectionLib;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
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
    public class ReservationCustomersDT : IReservationCustomersDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public ReservationCustomersDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Returns the List of Reservation Customers
        /// </summary>
        /// <returns></returns>
        public ReservationCustomersListModel GetReservationCustomers(DBInfoModel Store)
        {
            // get the results
            ReservationCustomersListModel reservationCustomersList = new ReservationCustomersListModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string getCustomersQuery = @"OPEN SYMMETRIC KEY KeyKhwp
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
                                                    from TR_ReservationCustomers";
                List<ReservationCustomersModel> resCusDetails = db.Query<ReservationCustomersModel>(getCustomersQuery).ToList();
                reservationCustomersList.ReservationCustomersModelList = resCusDetails;
            }

            return reservationCustomersList;
        }

        /// <summary>
        /// Returns the Customer's Informations From Room Number
        /// </summary>
        /// <param name="Room">RoomNumber</param>
        /// <returns></returns>
        public CustomersInfo GetCustomersInfo(DBInfoModel Store, string Room, string reservationId = "")
        {
            string confirmCode = "";
            //1. Reservation id encrypted wuth HashId
            if (!string.IsNullOrWhiteSpace(reservationId))
            {
                HashIdsHelper hasHlp = new HashIdsHelper();
                int reservId = hasHlp.DecodeIds(reservationId);
                confirmCode = reservId.ToString();
            }

            // get the results
            CustomersInfo customersInfo = new CustomersInfo();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                long defaultHotelId = db.Query<long>("SELECT tc.DefaultHotelId FROM TR_Config AS tc").FirstOrDefault();
                HotelsInfoModel hi = db.Query<HotelsInfoModel>("SELECT * FROM HotelInfo AS hi WHERE HI.Id =@ID", new { ID = defaultHotelId }).FirstOrDefault();

                PMSConnection pmsconnCustomersInfo = new PMSConnection();
                string connStr = "server=" + hi.ServerName + ";user id=" + hi.DBUserName +
                                                                       ";password=" + StringCipher.Decrypt(hi.DBPassword) + ";database=" + hi.DBName + ";";
                pmsconnCustomersInfo.initConn(connStr);
                using (IDbConnection dbprotel = new SqlConnection(connStr))
                {
                    CustomersDetails cus = dbprotel.Query<CustomersDetails>("EXEC " + hi.DBUserName + ".GetReservationInfo2 @confirmationCode = '" + confirmCode + "', @room = '" + Room + "', @mpeHotel = " + hi.MPEHotel + ", @pageNo = -1, @pageSize = 100, @bAllHotels = 0").FirstOrDefault();
                    if (cus != null)
                    {
                        customersInfo.ProfileNo = cus.ProfileNo;
                        customersInfo.FirstName = cus.FirstName;
                        customersInfo.LastName = cus.LastName;
                        customersInfo.NumberOfPeople = cus.Adults + cus.Children;
                        customersInfo.Email = cus.Email;
                        customersInfo.Room = cus.Room;
                    }
                    else
                    {
                        throw new Exception("No Customer found for Room Number : " + Room + ".");
                    }
                }
            }
            return customersInfo;
        }

        /// <summary>
        /// Returns details for a specific Reservation Customer
        /// </summary>
        /// <param name="Id">ReservationCustomerID</param>
        /// <returns></returns>
        public ReservationCustomersModel GetReservationCustomerById(DBInfoModel Store, long Id)
        {
            // get the results
            ReservationCustomersModel reservationCustomerDetails = new ReservationCustomersModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string getReservationCustomerByIdQuery = @"OPEN SYMMETRIC KEY KeyKhwp
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
                                                                where Id=@ID
                                                          ";
                ReservationCustomersModel resCustomerDetails = db.Query<ReservationCustomersModel>(getReservationCustomerByIdQuery, new { ID = Id }).FirstOrDefault();
                reservationCustomerDetails = resCustomerDetails;
            }
            return reservationCustomerDetails;
        }

        /// <summary>
        /// Returns details for a specific Customers Reservation full model
        /// </summary>
        /// <param name="Id">ReservationCustomerID</param>
        /// <returns></returns>
        public ExtecrTableReservetionModel GetCustomersReservation(DBInfoModel Store, long Id)
        {
            // get the results
            ExtecrTableReservetionModel resCustomersDetails = new ExtecrTableReservetionModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
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
                                                                where ReservationId=@reservationId
                                                          ";

                string getReservationQuery = @"SELECT * FROM TR_Reservations AS tr WHERE tr.Id =@ID";

                //Get RestId From Reservations
                string reservationIdQuery = @"SELECT tr.RestId FROM TR_Reservations AS tr WHERE tr.Id =@reservID ";
                long resId = db.Query<long>(reservationIdQuery, new { reservID = Id }).FirstOrDefault();

                //Get Restaurant Name By Id
                string restaurantNameQuery = @"SELECT tr.NameEn FROM TR_Restaurants AS tr WHERE tr.Id=@ID";
                string restName = db.Query<string>(restaurantNameQuery, new { ID = resId }).FirstOrDefault();
                resCustomersDetails.RestaurantName = restName;

                List<ReservationCustomersModel> resCustomersInfo = db.Query<ReservationCustomersModel>(getReservationCustomersQuery, new { reservationId = Id }).ToList();
                resCustomersDetails.ReservationCustomers = resCustomersInfo;

                ReservationsModel resInfo = db.Query<ReservationsModel>(getReservationQuery, new { ID = Id }).FirstOrDefault();
                resCustomersDetails.Reservation = resInfo;
            }
            return resCustomersDetails;
        }

        /// <summary>
        /// Insert new Reservation Customer
        /// </summary>
        /// <returns></returns>
        public ExtendedReservetionModel insertReservationCustomer(DBInfoModel Store, ExtendedReservetionModel model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    //Insert Reservations Informations
                    insertReservations(Store, model);

                    //Insert Customers Informations
                    insertReservationCustomers(Store, model);

                    // Commit transaction
                    scope.Complete();
                }
                model.Reservation.Id = db.Query<long>("SELECT tr.Id FROM TR_Reservations AS tr ORDER BY tr.Id DESC").FirstOrDefault();
            }
            return model;
        }

        /// <summary>
        /// Update a Reservation Customer
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ReservationCustomersModel UpdateReservationCustomer(DBInfoModel Store, ReservationCustomersModel Model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"OPEN SYMMETRIC KEY KeyKhwp
                                       DECRYPTION BY CERTIFICATE WHPMAIN
                                       with PASSWORD  = 'pH!t8bb928DGvbD2439h0Ty';
                                       UPDATE TR_ReservationCustomers SET 
                                                   ProtelId=@ProtelId, 
                                                   ProtelName = EncryptByKey(Key_GUID('KeyKhwp'), '" + Model.ProtelName + @"'),
                                                   ReservationName = EncryptByKey(Key_GUID('KeyKhwp'),'" + Model.ReservationName + @"'), 
                                                   RoomNum=@RoomNum, 
                                                   Email = EncryptByKey(Key_GUID('KeyKhwp'), '" + Model.Email + @"'), 
                                                   ReservationId=@ReservationId,   
                                                   HotelId=@HotelId
                                       WHERE Id=@ID
                                       CLOSE SYMMETRIC KEY KeyKhwp; ";

                db.Query(updateQuery, new
                {
                    ID = Model.Id,
                    ProtelId = Model.ProtelId,
                    RoomNum = Model.RoomNum,
                    ReservationId = Model.ReservationId,
                    HotelId = Model.HotelId
                });

                return Model;
            }
        }

        /// <summary>
        /// Delete a Reservation Customer
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteReservationCustomer(DBInfoModel Store, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    string deleteResCusQuery = "DELETE FROM TR_ReservationCustomers WHERE ReservationId=@reservationId";
                    string deleteResQuery = "DELETE FROM TR_Reservations WHERE Id=@ID";
                    //delete from Reservation Customers
                    db.Query(deleteResCusQuery, new { reservationId = Id });
                    //delete from Reservations
                    db.Query(deleteResQuery, new { ID = Id });

                    scope.Complete();
                }
                res = Id;
                return res;
            }
        }


        #region "Private Members"
        private void insertReservationCustomers(DBInfoModel Store, ExtendedReservetionModel model)
        {
            List<string> emailTo = new List<string>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ReservationCustomersModel resCus in model.ReservationCustomers)
                {
                    long resID = 0;
                    int checkKey = 0;

                    string getReservationId = @"SELECT tr.Id FROM TR_Reservations AS tr ORDER BY tr.Id DESC";

                    string checkKeyQuery = "select COUNT(*) from sys.symmetric_keys where name ='KeyKhwp'";

                    string insertQuery = @"OPEN SYMMETRIC KEY KeyKhwp
                                       DECRYPTION BY CERTIFICATE WHPMAIN
                                       with PASSWORD  = 'pH!t8bb928DGvbD2439h0Ty';
                                       INSERT INTO TR_ReservationCustomers (ProtelId, ProtelName, ReservationName, RoomNum, Email, ReservationId, HotelId)
                                       VALUES ( @ProtelId, EncryptByKey(Key_GUID('KeyKhwp'), '" + resCus.ProtelName + @"'), EncryptByKey(Key_GUID('KeyKhwp'),'" + resCus.ReservationName + @"'),
                                                @RoomNum , EncryptByKey(Key_GUID('KeyKhwp'), '" + resCus.Email + @"'), @ReservationId ,@HotelId )
                                       CLOSE SYMMETRIC KEY KeyKhwp; ";


                    checkKey = db.Query<int>(checkKeyQuery).FirstOrDefault();
                    if (checkKey == 0)
                    {
                        throw new Exception("There is no Key in sql Database!");
                    }
                    else
                    {
                        resID = db.Query<long>(getReservationId).FirstOrDefault();
                        db.Query(insertQuery, new
                        {
                            ProtelId = resCus.ProtelId,
                            RoomNum = resCus.RoomNum,
                            ReservationId = resID,
                            HotelId = resCus.HotelId
                        });
                        emailTo.Add(resCus.Email);
                    }
                }
            }
        }

        private void insertReservations(DBInfoModel Store, ExtendedReservetionModel model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"INSERT INTO TR_Reservations(RestId, CapacityId, Couver, ReservationDate, ReservationTime, CreateDate, [Status],Description)
                                       VALUES ( @RestId, @CapacityId, @Couver , @ReservationDate, @ReservationTime, @CreateDate , @Status,@Description )";

                db.Query(insertQuery, new
                {
                    RestId = model.Reservation.RestId,
                    CapacityId = model.Reservation.CapacityId,
                    Couver = model.Reservation.Couver,
                    ReservationDate = model.Reservation.ReservationDate,
                    ReservationTime = model.Reservation.ReservationTime,
                    CreateDate = DateTime.Now,
                    Status = model.Reservation.Status,
                    Description = model.Reservation.Description
                });
            }
        }

        #endregion
    }
}
