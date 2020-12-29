using Dapper;
using log4net;
using PMSConnectionLib;
using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT
{
    public class GdprDT : IGdprDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GdprDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// GDPR For WaterPark Customers(Update table OnlineRegistration).
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public bool UpdateOnlineRegistration(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"UPDATE OnlineRegistration SET
	                                   FirtName = BarCode,
	                                   LastName = BarCode,
	                                   Mobile = BarCode,
	                                   Email = BarCode,
                                       isAnonymized = 1
                                       WHERE [Status] = 2 AND Gdpr = 0 AND isAnonymized = 0 ";

                db.Query(updateQuery);

                return true;
            }
        }

        // Getting the Selected HotelInfo
        public HotelsInfoModel GetHotelInfo(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                HotelsInfoModel hotelI = new HotelsInfoModel();
                string selectHotelQuery = @"SELECT * FROM HotelInfo AS hi";
                string selectPosQuery = @"SELECT * FROM PosInfo AS pi1";
                HotelsInfoModel hi = db.Query<HotelsInfoModel>(selectHotelQuery).FirstOrDefault();
                PosInfoModel pi = db.Query<PosInfoModel>(selectPosQuery).FirstOrDefault();
                if (hi.Type == 4)
                {
                    hotelI = hi;
                    logger.Info("Select HotelId : " + hotelI.HotelId);

                }
                else
                {
                    if (pi.DefaultHotelId != null)
                    {
                        string selectHotelQuery1 = @"SELECT * FROM HotelInfo AS hi WHERE HotelId = @defaultHotelId";
                        hotelI = db.Query<HotelsInfoModel>(selectHotelQuery1, new { defaultHotelId = pi.DefaultHotelId }).FirstOrDefault();
                        logger.Info("Select HotelId : " + hotelI.HotelId);
                    }
                    else
                    {
                        hotelI = hi;
                        logger.Info("Select HotelId : " + hotelI.HotelId);
                    }
                }
                return hotelI;
            }
        }

        //GDPR For Pms Customers(Update table Quest).
        public bool UpdateGuest(DBInfoModel Store, HotelsInfoModel hi)
        {
            int exists;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    // Set up connection and Getting CustomersIds from selected pms
                    PMSConnection pmsconnActions = new PMSConnection();
                    string connStr = "server=" + hi.ServerName + ";user id=" + hi.DBUserName +
                                                                           ";password=" + StringCipher.Decrypt(hi.DBPassword) + ";database=" + hi.DBName + ";";
                    pmsconnActions.initConn(connStr);

                    //Check if Table Exists
                    exists = pmsconnActions.CheckIfTableExists();
                    if (exists == 1)
                    {
                        List<long> CustomersId = pmsconnActions.GetCustomersIds(hi.DBUserName);
                        string input = "";
                        input = String.Join(",", CustomersId);
                        if (CustomersId.Count > 0)
                        {
                            // Update Guest for the selected customers
                            string updateQuery = @"UPDATE Guest SET 
                                            FirstName = ProfileNo, 
                                            LastName = ProfileNo, 
                                            [Address] = ProfileNo, 
                                            Email = ProfileNo, 
                                            Telephone = ProfileNo
                                       WHERE ProfileNo IN (" + input + @")";

                            db.Query(updateQuery);

                            // Delete those Costomers from PMS Database
                            pmsconnActions.DeleteCustomersProfile(hi.DBUserName, input);

                            // Commit transaction
                            scope.Complete();
                        }
                        else
                        {
                            logger.Info("Protel Table hit_DeleteProfileFromPos has No Profiles to Delete!");
                        }
                    }
                    else
                    {
                        logger.Info("Protel Table hit_DeleteProfileFromPos Does not Exist!");
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// GDPR Delivery Entities
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public bool UpdateDeliveryEntities(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //Customers that has GDPR_Returner == true

                //Filter those that has Invoice active
                //Filter those that has Invoice on InvoiceType == 7 and IsPaid == true

                //And for These customer ids 
                //Clear Data for InvoiceShippingDetails that has InvoicesId

            }
            return true;
        }
    }
}
