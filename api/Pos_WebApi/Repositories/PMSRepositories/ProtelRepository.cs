using Pos_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using PMSConnectionLib;
using log4net;
using System.Linq;
using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;
using Symposium.Plugins;
using Symposium.Helpers;
using Symposium.WebApi.MainLogic.Flows.Hotelizer;
using Symposium.Models.Models.Hotelizer;

namespace Pos_WebApi.Repositories.PMSRepositories
{
    public class ProtelRepository : IPMS_Base
    {
        private string server { get; set; }
        private string userName { get; set; }
        private string password { get; set; }
        private string databasename { get; set; }
        private short? allHotels { get; set; }
        private string hotelType { get; set; }

        public ProtelRepository(string serverName, string dbUser, string dbPassword, string dbName, short? allH, string hType)
        {
            databasename = dbName;
            allHotels = allH;
            hotelType = hType;
            server = serverName;
            userName = dbUser;
            password = dbPassword;
        }

        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public bool CheckProcedure()
        {
            bool res = false;
            var constring = new SqlConnectionStringBuilder();
            constring.InitialCatalog = databasename; //"Protel";
            constring.DataSource = server; //"192.168.15.31";
            constring.UserID = userName;//"proteluser";
            constring.Password = password;//"protel915930";

            SqlConnection sqlConnection1 = new SqlConnection(constring.ConnectionString);

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "SELECT top 100 * FROM " + constring.UserID + ".leist";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;

            sqlConnection1.Open();

            reader = cmd.ExecuteReader();
            // Data is accessible through the DataReader object here.

            sqlConnection1.Close();
            return false;
        }


        public void CreateProcedure()
        {

        }

        public IEnumerable<PmsDepartmentModel> GetPMSDepartments(long hotelid)
        {

            //Hotelizer
            //checks if nedd to call Api Url for Hotelizer or other external systems
            bool extApi = false;
            extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
            if (extApi)
            {
                HotelizerFlows hotelizer = new HotelizerFlows();
                IEnumerable<HotelizerDepartmentModel> hotelizerDepart = hotelizer.GetHotelizerDepartments();
                List<PmsDepartmentModel> result = new List<PmsDepartmentModel>();
                foreach (HotelizerDepartmentModel item in hotelizerDepart)
                {
                    PmsDepartmentModel tmp = new PmsDepartmentModel();
                    tmp.DepartmentId = item.id;
                    tmp.Description = item.name;
                    tmp.Vat = 0;
                    result.Add(tmp);
                }
                return result;
            }

            var constring = new SqlConnectionStringBuilder();
            constring.InitialCatalog = databasename;//"Protel";
            constring.DataSource = server;
            constring.UserID = userName;
            constring.Password = StringCipher.Decrypt(password); ;

            OleDbConnection PubsConn = new OleDbConnection("Provider = SQLOLEDB;" + constring.ConnectionString);

            OleDbCommand resInfCMD = new OleDbCommand("GetDepartments", PubsConn);

            resInfCMD.CommandType = CommandType.StoredProcedure;

            OleDbParameter RetVal = resInfCMD.Parameters.Add
               ("RetVal", OleDbType.Integer); RetVal.Direction = ParameterDirection.ReturnValue;
            OleDbParameter mpeHotelValue = resInfCMD.Parameters.Add
               ("@mpeHotel", OleDbType.Integer);
            OleDbParameter isOnWebValue = resInfCMD.Parameters.Add
              ("@@IS_ON_WEB", OleDbType.Integer);

            isOnWebValue.Value = 1;
            mpeHotelValue.Value = hotelid;

            PubsConn.Open();

            OleDbDataReader myReader = resInfCMD.ExecuteReader();
            //var a = myReader..Cast<Customers>();
            var results = new List<PmsDepartmentModel>();
            while (myReader.Read())
            {
                try
                {
                    var cust = new PmsDepartmentModel()
                    {
                        DepartmentId = myReader.GetInt32(0),
                        Description = myReader.GetString(1),
                        Vat = Convert.ToDouble(myReader.GetValue(4), CultureInfo.InvariantCulture.NumberFormat)
                    };
                    results.Add(cust);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            myReader.Close();
            return results;
        }


        public IEnumerable<Customers> GetReservations(int hotelId, string name, string room, int? page = 0, int? pagesize = 12, string reservationId = "")
        {
            logger.Info("GetReservations - hotelId:" + hotelId.ToString() + ", name:" + (name ?? "<null>") + ", room:" + (room ?? "<null>") + ", page:" + (page ?? 0).ToString() + ",pagesize:" + pagesize.ToString());
            var results = new List<Customers>();
            try
            {
                string confirmCode = "";
                //1 Reservation id exists encrypted with hashIds...
                if (!string.IsNullOrWhiteSpace(reservationId))
                {
                    HashIdsHelper hasHlp = new HashIdsHelper();
                    int reservId = hasHlp.DecodeIds(reservationId);
                    confirmCode = reservId.ToString();
                }

                bool extApi = false;
                extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
                if (extApi)
                {
                    HotelizerFlows hotelizer = new HotelizerFlows();
                    results = hotelizer.GetRoomsAsCustomers(room, 0, page ?? 0, pagesize ?? 12);
                    return results;
                }


                var searchBy = string.Empty;
                var tempName = !string.IsNullOrEmpty(name) ? name : "";
                var tempRoom = !string.IsNullOrEmpty(room) ? room : "";
                var constring = new SqlConnectionStringBuilder();
                constring.InitialCatalog = databasename;//"Protel";
                constring.DataSource = server;
                constring.UserID = userName;
                constring.Password = StringCipher.Decrypt(password);

                string command = "";

                short? allH = allHotels;
                string sAllH;
                if (allH == 0)
                    sAllH = "False";
                else
                    sAllH = "True";

                switch (hotelType)
                {

                    case "PROTEL":
                        PosEntities db = new PosEntities(false);

                        var hi = db.HotelInfo.Where(w => w.HotelId == hotelId).FirstOrDefault();


                        short defaultmpeHotel = 1;
                        if (hi != null)
                        {
                            defaultmpeHotel = hi.MPEHotel ?? 1;
                        }
                        if (!tempRoom.Equals(""))
                        {
                            searchBy = room;
                        }
                        else if (!tempName.Equals(""))
                        {
                            searchBy = name;
                        }
                        else if (!tempName.Equals("") && !tempRoom.Equals(""))
                        {
                            searchBy = tempRoom;
                        }
                        command = "EXEC " + constring.UserID + ".GetReservationInfo2 '" + confirmCode + "' , '" + searchBy + "'," + defaultmpeHotel.ToString() + "," + page.ToString() + "," + pagesize.ToString() + "," + allH.ToString();
                        break;
                    case "ERMIS":

                        //command = "EXEC " + constring.UserID + ".GetReservationInfo '" + (!string.IsNullOrEmpty(name) ? name : "") + "' , '" + (!string.IsNullOrEmpty(room) ? room : "") + "'," + page.ToString() + "," + pagesize.ToString();
                        command = "EXEC " + constring.UserID + ".GetReservationInfo '" + "' , '" + (!string.IsNullOrEmpty(room) ? room : (!string.IsNullOrEmpty(name) ? name : "")) + "'," + page.ToString() + "," + pagesize.ToString();
                        break;
                }

                logger.Info("command: " + command);
                string connStr = "server=" + server + ";user id=" + userName +
                                                   ";password=" + StringCipher.Decrypt(password) + ";database=" + databasename + ";";
                logger.Info("connStr: " + connStr.Substring(0, 49) + "..." + ";database: " + databasename);
                PMSConnection PMSCon = new PMSConnection();
                PMSCon.initConn(connStr);

                logger.Info("Reader Init");
                SqlDataReader myReader = PMSCon.returnResult(command);
                logger.Info("Getting Customers...");
                while (myReader.Read())
                {

                    var cust = new Customers()
                    {
                        //TotalRecs = myReader.GetInt32(0),
                        //ReservationCode = myReader.GetString(2),
                        //ReservationId = myReader.GetInt32(3),

                        //RoomId = myReader.GetInt32(5),
                        //Room = myReader.GetString(6),
                        //arrivalDT = myReader.GetDateTime(7),
                        //departureDT = myReader.GetDateTime(8),
                        //ProfileNo = myReader.GetInt32(9),
                        //FirstName = myReader.GetString(10),
                        //LastName = myReader.GetString(11),
                        //Member = myReader.GetString(12),
                        //Password = myReader.GetString(13),
                        //Address = myReader.GetString(14),
                        //City = myReader.GetString(15),
                        //PostalCode = myReader.GetString(16),
                        //Country = myReader.GetString(17),
                        //birthdayDT = myReader.GetDateTime(18),
                        //Email = myReader.GetString(19),
                        //Telephone = myReader.GetString(20),
                        //VIP = myReader.GetInt32(21).ToString(),
                        //Benefits = myReader.GetString(22),
                        //NationalityCode = myReader.GetString(23),
                        //Adults = myReader.GetInt32(24),
                        //Children = myReader.GetInt32(25),
                        //Note1 = myReader.GetString(26),
                        //Note2 = myReader.GetString(27),
                        ////IsSharer = (myReader.GetInt32(29).,
                        //BoardCode = myReader.GetString(30),
                        //BoardName = myReader.GetString(31),
                        //NoPos = myReader.GetInt32(32),

                        ////Απαραίτητη πληροφορία loyalty του πελάτη
                        //ClassId = myReader.GetInt32(33),
                        //ClassName = myReader.GetString(34),
                        //AvailablePoints = myReader.GetInt32(35),
                        //fnbdiscount = myReader.GetInt32(36),
                        //ratebuy = myReader.GetInt32(37),
                        //TravelAgent = myReader.GetString(38),
                        //Company = myReader.GetString(39),
                        //GuestFuture = myReader.GetString(40)


                        TotalRecs = !myReader.IsDBNull(0) == true ? myReader.GetInt32(0) : 0,
                        ReservationCode = !myReader.IsDBNull(2) == true ? myReader.GetString(2) : "",
                        ReservationId2 = !myReader.IsDBNull(3) == true ? myReader.GetInt32(3) : 0,
                        ReservationId = !myReader.IsDBNull(4) == true ? myReader.GetInt32(4) : 0,
                        RoomId = !myReader.IsDBNull(5) == true ? myReader.GetInt32(5) : 0,
                        Room = !myReader.IsDBNull(6) == true ? myReader.GetString(6) : "",
                        //if (myReader.IsDBNull(7)) continue,
                        arrivalDT = myReader.GetDateTime(7),
                        //if (myReader.IsDBNull(8)) continue,
                        departureDT = myReader.GetDateTime(8),
                        ProfileNo = !myReader.IsDBNull(9) == true ? myReader.GetInt32(9) : 0,
                        FirstName = !myReader.IsDBNull(10) == true ? myReader.GetString(10) : "",
                        LastName = !myReader.IsDBNull(11) == true ? myReader.GetString(11) : "",
                        Member = !myReader.IsDBNull(12) == true ? myReader.GetString(12) : "",
                        Password = !myReader.IsDBNull(13) == true ? myReader.GetString(13) : "",
                        Address = !myReader.IsDBNull(14) == true ? myReader.GetString(14) : "",
                        City = !myReader.IsDBNull(15) == true ? myReader.GetString(15) : "",
                        PostalCode = !myReader.IsDBNull(16) == true ? myReader.GetString(16) : "",
                        Country = !myReader.IsDBNull(17) == true ? myReader.GetString(17) : "",
                        //if (myReader.IsDBNull(18)) continue,
                        birthdayDT = myReader.GetDateTime(18),
                        Email = !myReader.IsDBNull(19) == true ? myReader.GetString(19) : "",
                        Telephone = !myReader.IsDBNull(20) == true ? myReader.GetString(20) : "",
                        VIP = !myReader.IsDBNull(21) == true ? myReader.GetInt32(21).ToString() : "",
                        Benefits = !myReader.IsDBNull(22) == true ? myReader.GetString(22) : "",
                        NationalityCode = !myReader.IsDBNull(53) == true ? myReader.GetString(53) : "",
                        Adults = !myReader.IsDBNull(24) == true ? myReader.GetInt32(24) : 0,
                        Children = !myReader.IsDBNull(25) == true ? myReader.GetInt32(25) : 0,
                        Note1 = !myReader.IsDBNull(26) == true ? myReader.GetString(26) : "",
                        Note2 = !myReader.IsDBNull(27) == true ? myReader.GetString(27) : "",
                        //CustomerId = !myReader.IsDBNull(28) == true ? myReader.GetInt32(28).ToString() : "",
                        IsSharer = !myReader.IsDBNull(29) == true ? myReader.GetBoolean(29) : false,
                        BoardCode = !myReader.IsDBNull(30) == true ? myReader.GetString(30) : "",
                        BoardName = !myReader.IsDBNull(31) == true ? myReader.GetString(31) : "",
                        NoPos = !myReader.IsDBNull(32) == true ? myReader.GetInt32(32) : 0,

                        ////Απαραίτητη πληροφορία loyalty του πελάτη
                        ClassId = !myReader.IsDBNull(33) == true ? myReader.GetInt32(33) : 0,
                        ClassName = !myReader.IsDBNull(34) == true ? myReader.GetString(34) : "",
                        AvailablePoints = !myReader.IsDBNull(35) == true ? myReader.GetInt32(35) : 0,
                        fnbdiscount = !myReader.IsDBNull(36) == true ? myReader.GetInt32(36) : 0,
                        ratebuy = !myReader.IsDBNull(37) == true ? myReader.GetInt32(37) : 0,
                        GuestFuture = !myReader.IsDBNull(38) == true ? myReader.GetString(38) : "",
                        //RoomTypeId = !myReader.IsDBNull(39) == true ? myReader.GetInt32(39) : 0,
                        // RoomType = !myReader.IsDBNull(40) == true ? myReader.GetString(40) : "",
                        //BookedRoomTypeId = !myReader.IsDBNull(41) == true ? myReader.GetInt32(41) : 0,
                        //BookedRoomType = !myReader.IsDBNull(42) == true ? myReader.GetString(42) : "",
                        //TravelAgentId = !myReader.IsDBNull(43) == true ? myReader.GetInt32(43) : 0,
                        TravelAgent = !myReader.IsDBNull(44) == true ? myReader.GetString(44) : "",
                        //CompanyId = !myReader.IsDBNull(45) == true ? myReader.GetInt32(45) : 0,
                        Company = !myReader.IsDBNull(46) == true ? myReader.GetString(46) : "",
                        //GroupId = !myReader.IsDBNull(47) == true ? myReader.GetInt32(47) : 0,
                        //GroupName = !myReader.IsDBNull(48) == true ? myReader.GetString(48) : "",
                        //SourceId = !myReader.IsDBNull(49) == true ? myReader.GetInt32(49) : 0,
                        // Source = !myReader.IsDBNull(50) == true ? myReader.GetString(50) : "",
                        //Mpehotel = !myReader.IsDBNull(51) == true ? myReader.GetInt32(51) : 0,
                        ReservStatus = !myReader.IsDBNull(52) == true ? myReader.GetInt32(52) : 0,
                        //NationalityCode = !myReader.IsDBNull(53) == true ? myReader.GetString(53) : "",
                    };
                    results.Add(cust);
                    if (logger.IsInfoEnabled) logger.Info("customer from PMS : " + cust.ToString());
                }
                logger.Info(results.Count.ToString() + " customers from PMS.");
                myReader.Close();
                logger.Info("Reader Closed...");
                PMSCon.closeConnection();

            }
            catch (Exception ex_last_check)
            {
                logger.Error("GetReservations Error : " + ex_last_check.ToString());
                // var a = ex_last_check.Message;
                // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("GetReservations error | " + ex_last_check.InnerException != null ? ex_last_check.InnerException.Message : ex_last_check.Message));
                return null;
            }

            return results;
        }

        /// <summary>
        /// Search for customer by given vat number
        /// </summary>
        /// <param name="vat">string: vat number to search</param>
        /// <param name="dbInfo">DBInfoModel</param>
        /// <returns>IEnumerable<VATCustomerResultModel>: list of customers with specified vat</returns>
        public IEnumerable<VATCustomerResultModel> GetCustomersByAFM(string vat, DBInfoModel dbInfo)
        {
            //first search db for customer by vat
            IEnumerable<VATCustomerResultModel> result = GetCustomersByAFMFromDB(vat);

            //if no customer in db search taxis
            if (result == null || !result.Any())
            {
                result = GetCustomersByAFMFromTaxis(vat, dbInfo);
            }

            return result;
        }

        /// <summary>
        /// Search database for customer by given vat number
        /// </summary>
        /// <param name="afm">vat number to search</param>
        /// <returns>>IEnumerable<VATCustomerResultModel>: list of customers with specified vat</returns>
        public IEnumerable<VATCustomerResultModel> GetCustomersByAFMFromDB(string afm)
        {
            logger.Info("GetCustomersByAFMFromDB - AFM:" + (afm ?? "<null>"));
            var results = new List<VATCustomerResultModel>();

            try
            {
                var tempAFM = !string.IsNullOrEmpty(afm) ? afm : "";
                var constring = new SqlConnectionStringBuilder();
                constring.InitialCatalog = databasename;
                constring.DataSource = server;
                constring.UserID = userName;
                constring.Password = StringCipher.Decrypt(password);
                string command = "";
                switch (hotelType)
                {
                    case "PROTEL":
                        command = "EXEC " + constring.UserID + ".GetProfileByAFM '" + afm + "'";
                        break;
                    case "ERMIS":
                        command = "EXEC " + constring.UserID + ".GetProfileByAFM '" + afm + "'";
                        break;
                }

                string connStr = "server=" + server + ";user id=" + userName + ";password=" + StringCipher.Decrypt(password) + ";database=" + databasename + ";";

                PMSConnection PMSCon = new PMSConnection();
                PMSCon.initConn(connStr);

                SqlDataReader myReader = PMSCon.returnResult(command);

                while (myReader.Read())
                {
                    var cust = new VATCustomerResultModel()
                    {
                        Id = myReader.GetInt32(0),
                        LastName = myReader.GetString(1),
                        FirstName = myReader.GetString(2),
                        AFM = myReader.GetString(3),
                        DOY = myReader.GetString(4),
                        Address1 = myReader.GetString(5),
                        Address2 = myReader.GetString(6),
                        City = myReader.GetString(7),
                        PostCode = myReader.GetString(8),
                        NationalityId = myReader.GetInt32(9),
                        Nationality = myReader.GetString(10),
                        CountryId = myReader.GetString(11),
                        Country = myReader.GetString(12),
                        Telephone = myReader.GetString(13),
                        Mobile = myReader.GetString(14),
                        CompanyName = myReader.GetString(15),
                        Job = myReader.GetString(16),
                        VIP = myReader.GetInt32(17),
                        Code = myReader.GetString(18)
                    };
                    results.Add(cust);
                }
                logger.Info(results.Count.ToString() + " customers from PMS.");
                myReader.Close();
                PMSCon.closeConnection();
            }
            catch (Exception e)
            {
                logger.Error("GetCustomersByAFMFromDB Error : " + e.ToString());
                return null;
            }

            return results;
        }

        /// <summary>
        /// Search taxis for customer by given vat number
        /// </summary>
        /// <param name="vat">string: vat number to search</param>
        /// <param name="dbInfo">DBInfoModel</param>
        /// <returns>>IEnumerable<VATCustomerResultModel>: list of customers with specified vat</returns>
        public IEnumerable<VATCustomerResultModel> GetCustomersByAFMFromTaxis(string vat, DBInfoModel dbInfo)
        {
            PluginHelper pluginHelper = new PluginHelper();
            List<VATCustomerResultModel> results = new List<VATCustomerResultModel>();
            try
            {
                object ImplementedClassInstance = pluginHelper.InstanciatePlugin(typeof(TaxisSearchVAT));
                object[] InvokedMethodParameters = { vat, dbInfo, logger };
                results = pluginHelper.InvokePluginMethod<List<VATCustomerResultModel>>(ImplementedClassInstance, "InvokePluginTransaction", new[] { typeof(string), typeof(DBInfoModel), typeof(ILog) }, InvokedMethodParameters);
            }
            catch (Exception e)
            {
                logger.Error("Error calling TaxisSearchVAT plugin: " + e.ToString());
                return results;
            }
            return results;
        }

        public IEnumerable<PMSPaymentsModel> GetPMSPaymentTypes()
        {
            logger.Info("GetPMSPaymentTypes");
            var results = new List<PMSPaymentsModel>();

            try
            {
                var constring = new SqlConnectionStringBuilder();
                constring.InitialCatalog = databasename;
                constring.DataSource = server;
                constring.UserID = userName;
                constring.Password = StringCipher.Decrypt(password);
                string command = "";
                command = "EXEC " + constring.UserID + ".GetMethodOfPayments";
                logger.Info("command: " + command);
                string connStr = "server=" + server + ";user id=" + userName + ";password=" + StringCipher.Decrypt(password) + ";database=" + databasename + ";";
                logger.Info("connStr: " + connStr.Substring(0, 49) + "..." + ";database: " + databasename);
                PMSConnection PMSCon = new PMSConnection();
                PMSCon.initConn(connStr);
                logger.Info("Reader Init");
                SqlDataReader myReader = PMSCon.returnResult(command);
                logger.Info("Getting PMS Payment Types ...");
                while (myReader.Read())
                {
                    var payment = new PMSPaymentsModel()
                    {
                        Id = myReader.GetInt32(0),
                        Description = myReader.GetString(1)
                    };
                    results.Add(payment);
                    logger.Info("payment types from PMS : " + payment.ToString());
                }
                logger.Info(results.Count.ToString() + " payment types from PMS.");
                myReader.Close();
                logger.Info("Reader Closed...");
                PMSCon.closeConnection();
            }
            catch (Exception e)
            {
                logger.Error("GetPMSPaymentTypes Error : " + e.ToString());
                return new List<PMSPaymentsModel>();
            }

            return results;
        }

        public IEnumerable<PMSInvoiceTypes> GetPMSInvoiceTypes(short mpeHotel)
        {
            logger.Info("GetPMSInvoiceTypes");
            var results = new List<PMSInvoiceTypes>();

            try
            {
                var constring = new SqlConnectionStringBuilder();
                constring.InitialCatalog = databasename;
                constring.DataSource = server;
                constring.UserID = userName;
                constring.Password = StringCipher.Decrypt(password);
                string command = "";
                command = "EXEC " + constring.UserID + ".GetInvoiceTypes " + mpeHotel;
                logger.Info("command: " + command);
                string connStr = "server=" + server + ";user id=" + userName + ";password=" + StringCipher.Decrypt(password) + ";database=" + databasename + ";";
                logger.Info("connStr: " + connStr.Substring(0, 49) + "..." + ";database: " + databasename);
                PMSConnection PMSCon = new PMSConnection();
                PMSCon.initConn(connStr);
                logger.Info("Reader Init");
                SqlDataReader myReader = PMSCon.returnResult(command);
                logger.Info("Getting PMS Invoice Types ...");
                while (myReader.Read())
                {
                    var invoice = new PMSInvoiceTypes()
                    {
                        Id = myReader.GetInt32(0),
                        Description = myReader.GetString(1),
                        Abbreviation = myReader.GetString(2),
                    };
                    results.Add(invoice);
                    logger.Info("invoice types from PMS : " + invoice.ToString());
                }
                logger.Info(results.Count.ToString() + " invoice types from PMS.");
                myReader.Close();
                logger.Info("Reader Closed...");
                PMSCon.closeConnection();
            }
            catch (Exception e)
            {
                logger.Error("GetPMSInvoiceTypes Error : " + e.ToString());
                return null;
            }

            return results;
        }

    }
    public static class DataReaderExtension
    {
        public static IEnumerable<Object[]> AsEnumerable(this System.Data.IDataReader source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            while (source.Read())
            {
                Object[] row = new Object[source.FieldCount];
                source.GetValues(row);
                yield return row;
            }
        }
    }

}