using Dapper;
using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.Models.Models.Hotel;
using Symposium.Models.Models.MealBoards;
using Symposium.WebApi.DataAccess.Interfaces.DT.Hotel;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Symposium.Models.Models.Helper;
using PMSConnectionLib;
using Symposium.WebApi.DataAccess.Interfaces.DT.CashedObjects;
using Symposium_DTOs.PosModel_Info;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Newtonsoft.Json;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.Helpers;
using Symposium.WebApi.DataAccess.DT.CashedObjects;
using System.Reflection;
using System.Linq.Expressions;
using System.Globalization;

namespace Symposium.WebApi.DataAccess.DT.Hotel
{
    public class HotelDT : IHotelDT
    {
        string connectionString;
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IUsersToDatabasesXML usersToDatabases;
        IHotelMacrosDT hotelMacrosDT;
        IGenericDAO<HotelMacrosDTO> genMacro;
        IGenericDAO<HotelMacroTimezoneDTO> genTimezone;
        IGenericDAO<HotelCustomerDataConfigDTO> genCustomerDataConfig;
        IGenericDAO<HotelMacros_HistDTO> genMacroHist;
        IHotelMacroTimezoneDT timezoneDT;
        private readonly object hotelMacroTimezoneDT;


        //  IHotelMacroTimezoneDT hotelMacroTimezoneDT;

        public HotelDT(IUsersToDatabasesXML usersToDatabases, IHotelMacrosDT hotelMacrosDT, IHotelMacroTimezoneDT timezoneDT, IGenericDAO<HotelMacrosDTO> genMacro, IGenericDAO<HotelMacrosDTO> genTimezone, IGenericDAO<HotelCustomerDataConfigDTO> genCustomerDataConfig, IGenericDAO<HotelMacros_HistDTO> genMacroHist)
        {
            this.usersToDatabases = usersToDatabases;
            this.hotelMacrosDT = hotelMacrosDT;
            this.timezoneDT = timezoneDT;
            this.genMacro = genMacro;
            this.genCustomerDataConfig = genCustomerDataConfig;
            this.genMacroHist = genMacroHist;
        }

        public List<ParamModel> GetParams(DBInfoModel DBInfo)
        {

            List<ParamModel> model = new List<ParamModel>();
            CustomerModel cstmModel = new CustomerModel();
            Type myType = (typeof(CustomerModel));

            MethodInfo[] info = myType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (MethodInfo methodInfo in info)
            {
                if (methodInfo.Name.Contains("get"))
                {
                    string tempname = methodInfo.ReturnType.Name;

                    ParamModel temp = new ParamModel();
                    temp.FieldType = tempname.Replace(",", "");
                    int i = model.Count() + 1;
                    temp.Id = i;
                    temp.Description = methodInfo.Name.Substring(4);
                    model.Add(temp);

                }
            }
            return model;
        }

        public void DeleteObsoleteMacros(DBInfoModel DBInfo)
        {
            List<MacroModel> macros = hotelMacrosDT.CashedDT.Select(DBInfo).Where(m => m.ActiveTo != null && m.ActiveTo < DateTime.Now.AddDays(-420)).ToList();

            foreach (MacroModel macro in macros)
            {
                bool result = InsertMacroToHist(DBInfo, macro);

                if (result)
                {
                    hotelMacrosDT.CashedDT.Delete(DBInfo, macro.Id);
                }
            }
        }

        public bool InsertMacroToHist(DBInfoModel DBInfo, MacroModel macro)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Guid macroId = macro.Id;

                string sql = @"SELECT * FROM HotelMacros WHERE Id=@macroId";

                HotelMacros_HistDTO dbMacro = db.Query<HotelMacros_HistDTO>(sql, new { macroId = macroId }).FirstOrDefault();

                try
                {
                    genMacroHist.Insert<Guid>(db, dbMacro);

                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());

                    return false;
                }
            }
        }

        public void HandleHotelCustomerDataConfig(DBInfoModel DBInfo, List<Hotel__CustomerDataConfigModel> listmodel)
        {
            string deletequery = "Delete from  HotelCustomerDataConfig";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(deletequery);


                foreach (Hotel__CustomerDataConfigModel row in listmodel)
                {
                    InsertCustomerDataConfig(DBInfo, row);
                }
            }



        }
        public List<VipModel> GetVip(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<VipModel> model = new List<VipModel>();

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            string vipModel = "select codenr,bezeich from " + hotelInfo.DBUserName + ".vipcode order by bezeich ";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                connectionString = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) + ";database=" + hotelInfo.DBName + ";";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<VipModel>(vipModel).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("VipModel Error : " + e.ToString());
                    }
                }
                return model;
            }
        }

        public List<BoardModel> GetBoards(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<BoardModel> model = new List<BoardModel>();

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            string boardModel = "select  distinct(short) as shrt from " + hotelInfo.DBUserName + ".splittab order by short";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                connectionString = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) + ";database=" + hotelInfo.DBName + ";";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<BoardModel>(boardModel).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("BoardModel Error : " + e.ToString());
                    }
                }
            }


            return model;
        }
        public List<NationalityCodeModel> GetNationalityCode(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<NationalityCodeModel> model = new List<NationalityCodeModel>();

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            string nationalityCodeQuery = "select codenr,abkuerz,land from " + hotelInfo.DBUserName + ".natcode";
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                connectionString = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) + ";database=" + hotelInfo.DBName + ";";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<NationalityCodeModel>(nationalityCodeQuery).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetBookedRoomTypeModel Error : " + e.ToString());
                    }
                }
            }

            return model;
        }
        public List<BookedRoomTypeModel> GetBookedRoomType(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<BookedRoomTypeModel> model = new List<BookedRoomTypeModel>();
            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            string bookedRoomTypeQuery = "select orgkatnr as id,kat as description from " + hotelInfo.DBUserName + ".buch inner join " + hotelInfo.DBUserName + ".kat on kat.katnr=buch.katnr where mpehotel IN (0, @mpehotel)";
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                connectionString = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) + ";database=" + hotelInfo.DBName + ";";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<BookedRoomTypeModel>(bookedRoomTypeQuery, new { mpehotel = mpehotel }).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetBookedRoomTypeModel Error : " + e.ToString());
                    }
                }
            }

            return model;
        }

        public HotelsInfoModel GetMpehotelQuery(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            HotelsInfoModel hotelinfo = new HotelsInfoModel();

            string query = "SELECT * FROM HotelInfo WHERE id=@hotelInfoId  AND MPEHotel=@mpehotel";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                hotelinfo = db.Query<HotelsInfoModel>(query, new { hotelInfoId = hotelInfoId, MPEHotel = mpehotel }).FirstOrDefault();

                if (hotelinfo == null || hotelinfo.MPEHotel == null)
                {
                    query = "SELECT TOP 1 * FROM HotelInfo WHERE MPEHotel=@mpehotel";

                    hotelinfo = db.Query<HotelsInfoModel>(query, new { MPEHotel = mpehotel }).FirstOrDefault();

                    if (hotelinfo == null || hotelinfo.MPEHotel == null)
                    {
                        return null;
                    }
                }
            }

            return hotelinfo;
        }

        public List<RoomTypeModel> GetRoomType(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<RoomTypeModel> model = new List<RoomTypeModel>();

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            string roomTypeQuery = "select katnr,kat,bez from " + hotelInfo.DBUserName + ".kat where zimmer=1";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                connectionString = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) + ";database=" + hotelInfo.DBName + ";";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<RoomTypeModel>(roomTypeQuery).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetProtelSourceList Error : " + e.ToString());
                    }
                }
            }

            return model;
        }

        public List<SourceModel> GetSource(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<SourceModel> model = new List<SourceModel>();

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            string sourceQuery = "select nr,bezeich from " + hotelInfo.DBUserName + ".source";
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                connectionString = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) + ";database=" + hotelInfo.DBName + ";";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<SourceModel>(sourceQuery, new { mpehotel = mpehotel }).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetProtelSourceList Error : " + e.ToString());
                    }
                }
            }
            return model;

        }

        public Dictionary<int, string> GetHotelInfo(DBInfoModel DBInfo)
        {
            Dictionary<int, string> model = new Dictionary<int, string>();

            string hotelInfoQuery = "select id, hotelname from HotelInfo WHERE type = 4";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                model = db.Query(hotelInfoQuery).ToDictionary(row => (int)row.id, row => (string)row.hotelname);
            }

            return model;
        }

        public List<ProtelHotelModel> GetHotels(DBInfoModel DBInfo, int hotelInfoId)
        {
            List<ProtelHotelModel> model = new List<ProtelHotelModel>();

            string hotelInfoQuery = "select * from HotelInfo where id=@id";


            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                HotelsInfoModel hotelinfo = db.Query<HotelsInfoModel>(hotelInfoQuery, new { id = hotelInfoId }).FirstOrDefault();

                connectionString = "server=" + hotelinfo.ServerName + ";user id=" + hotelinfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelinfo.DBPassword) + ";database=" + hotelinfo.DBName + ";";

                string qry = "" +
                    "SELECT l.mpehotel, l.short hotelName, d.pdate " +
                    "FROM " + hotelinfo.DBUserName + ".lizenz AS l " +
                    "INNER JOIN " + hotelinfo.DBUserName + ".datum AS d ON d.mpehotel = l.mpehotel " +
                    "WHERE l.mpehq <> 1 ";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<ProtelHotelModel>(qry).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetProtelHotelList Error : " + e.ToString());
                    }
                }
            }

            return model;
        }


        public List<RoomModel> GetProtelRoomNo(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<RoomModel> model = new List<RoomModel>();

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            string roomQuery = "select zinr, ziname,katnr,k.kat,bez from " + hotelInfo.DBUserName + ".kat as k inner join " + hotelInfo.DBUserName + ".zimmer as z on z.kat=k.katnr and k.zimmer = 1 where z.mpehotel IN (0, @mpehotel) order by z.ziname";
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                connectionString = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) + ";database=" + hotelInfo.DBName + ";";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<RoomModel>(roomQuery, new { mpehotel = mpehotel }).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetProtelRoomList Error : " + e.ToString());
                    }
                }
            }

            return model;
        }

        public List<GroupModel> GetGroups(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<GroupModel> model = new List<GroupModel>();
            string hotelInfoQuery = "select * from HotelInfo where MPEHotel=@mpehotel";
            //select kdnr,name1, case when typ=1 then 'Company' when typ=2 then 'Group' when typ=3 then 'Travel Agent' else 'Source' end typkind from kunden where typ in(1,2,3,4) order by name1


            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                HotelsInfoModel hotelinfo = db.Query<HotelsInfoModel>(hotelInfoQuery, new { mpehotel = mpehotel }).FirstOrDefault();

                string groupQuery = "select kdnr, name1 from " + hotelinfo.DBUserName + ".kunden where typ =2 order by name1";

                connectionString = "server=" + hotelinfo.ServerName + ";user id=" + hotelinfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelinfo.DBPassword) + ";database=" + hotelinfo.DBName + ";";
                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<GroupModel>(groupQuery).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetGroups Error : " + e.ToString());
                    }
                }
            }

            return model;
        }

        public List<ProductLookupHelper> GetFilteredProduct(DBInfoModel DBInfo, string name)
        {
            List<ProductLookupHelper> model = new List<ProductLookupHelper>();

            string filteredProducts = "select Id,Description from Product where Description like '%" + name + "%' order by Description";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    model = db.Query<ProductLookupHelper>(filteredProducts).ToList();
                }
            }
            catch (Exception e)
            {
                logger.Error("GetFilteredProduct Error : " + e.ToString());
            }

            return model;
        }

        public List<GroupModel> GetFilteredProtelGroupList(DBInfoModel DBInfo, int mpehotel, string name)
        {
            List<GroupModel> model = new List<GroupModel>();
            string hotelInfoQuery = "select * from HotelInfo where MPEHotel=@mpehotel";


            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                HotelsInfoModel hotelinfo = db.Query<HotelsInfoModel>(hotelInfoQuery, new { mpehotel = mpehotel }).FirstOrDefault();

                string groupQuery = "select kdnr, name1 from " + hotelinfo.DBUserName + ".kunden where typ=2 and  name1 like '%" + name + "%' order by name1";

                connectionString = "server=" + hotelinfo.ServerName + ";user id=" + hotelinfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelinfo.DBPassword) + ";database=" + hotelinfo.DBName + ";";
                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<GroupModel>(groupQuery).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetProtelTravelAgentList Error : " + e.ToString());
                    }
                }
            }

            return model;
        }


        public List<TravelAgentModel> GetFilteredProtelTravelAgentList(DBInfoModel DBInfo, int mpehotel, string name)
        {
            List<TravelAgentModel> model = new List<TravelAgentModel>();
            string hotelInfoQuery = "select * from HotelInfo where MPEHotel=@mpehotel";

            //select kdnr,name1, case when typ=1 then 'Company' when typ=2 then 'Group' when typ=3 then 'Travel Agent' else 'Source' end typkind from kunden where typ in(1,2,3,4) order by name1

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                HotelsInfoModel hotelinfo = db.Query<HotelsInfoModel>(hotelInfoQuery, new { mpehotel = mpehotel }).FirstOrDefault();

                string travelAgentQuery = "select kdnr, name1 from " + hotelinfo.DBUserName + ".kunden where typ=3 and  name1 like '%" + name + "%' order by name1";

                connectionString = "server=" + hotelinfo.ServerName + ";user id=" + hotelinfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelinfo.DBPassword) + ";database=" + hotelinfo.DBName + ";";
                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<TravelAgentModel>(travelAgentQuery).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetProtelTravelAgentList Error : " + e.ToString());
                    }
                }
            }

            return model;
        }
        public List<TravelAgentModel> GetProtelTravelAgentList(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<TravelAgentModel> model = new List<TravelAgentModel>();
            string hotelInfoQuery = "select * from HotelInfo where MPEHotel=@mpehotel";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                HotelsInfoModel hotelinfo = db.Query<HotelsInfoModel>(hotelInfoQuery, new { mpehotel = mpehotel }).FirstOrDefault();

                string travelAgentQuery = "select kdnr, name1 from " + hotelinfo.DBUserName + ".kunden where typ =3 order by name1";

                connectionString = "server=" + hotelinfo.ServerName + ";user id=" + hotelinfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelinfo.DBPassword) + ";database=" + hotelinfo.DBName + ";";
                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<TravelAgentModel>(travelAgentQuery).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetProtelTravelAgentList Error : " + e.ToString());
                    }
                }
            }

            return model;
        }

        public List<HotelPricelistModel> GetHotelPricelists(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<HotelPricelistModel> model = new List<HotelPricelistModel>();

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            string query = @"
                SELECT DISTINCT pt.ptypnr AS id, pt.ptyp +' ('+ p.longtext+')    RT : '+
                    CASE WHEN ISNULL(k1.kat,'') <> '' THEN k1.kat+' - ' ELSE '' END +
                    CASE WHEN ISNULL(k2.kat,'') <> '' THEN k2.kat+' - ' ELSE '' END +
                    CASE WHEN ISNULL(k3.kat,'') <> '' THEN k3.kat+' - ' ELSE '' END +
                    CASE WHEN ISNULL(k4.kat,'') <> '' THEN k4.kat+' - ' ELSE '' END +
                    CASE WHEN ISNULL(k5.kat,'') <> '' THEN k5.kat ELSE '' END AS [description]
                FROM " + hotelInfo.DBName + @"." + hotelInfo.DBUserName + @".ptypgrp AS p
                INNER JOIN " + hotelInfo.DBName + @"." + hotelInfo.DBUserName + @".ptyp AS pt ON pt.grp = p.ptgnr
                LEFT OUTER JOIN " + hotelInfo.DBName + @"." + hotelInfo.DBUserName + @".kat AS k1 ON k1.katnr = pt.kat
                LEFT OUTER JOIN " + hotelInfo.DBName + @"." + hotelInfo.DBUserName + @".kat AS k2 ON k2.katnr = pt.kat2
                LEFT OUTER JOIN " + hotelInfo.DBName + @"." + hotelInfo.DBUserName + @".kat AS k3 ON k3.katnr = pt.kat3
                LEFT OUTER JOIN " + hotelInfo.DBName + @"." + hotelInfo.DBUserName + @".kat AS k4 ON k4.katnr = pt.kat4
                LEFT OUTER JOIN " + hotelInfo.DBName + @"." + hotelInfo.DBUserName + @".kat AS k5 ON k5.katnr = pt.kat5
                WHERE mpehotel IN (0,@mpehotel)
            ";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                connectionString = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) + ";database=" + hotelInfo.DBName + ";";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<HotelPricelistModel>(query, new { mpehotel = mpehotel }).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetHotelPricelists Error : " + e.ToString());
                    }
                }
            }

            return model;
        }

        public List<MembershipModel> GetMemberships(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<MembershipModel> model = new List<MembershipModel>();

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            string query = @"SELECT nr AS id, short_text AS [description] FROM " + hotelInfo.DBUserName + @".prgname";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                connectionString = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) + ";database=" + hotelInfo.DBName + ";";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<MembershipModel>(query, new { mpehotel = mpehotel }).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetMemberships Error : " + e.ToString());
                    }
                }
            }

            return model;
        }

        public List<SubmembershipModel> GetSubmemberships(DBInfoModel DBInfo, int hotelInfoId, int mpehotel)
        {
            List<SubmembershipModel> model = new List<SubmembershipModel>();

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            string query = @"SELECT nr AS id, ref_to_reward_program AS membershipId, short_text AS [description] FROM " + hotelInfo.DBUserName + @".loylvl";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                connectionString = "server=" + hotelInfo.ServerName + ";user id=" + hotelInfo.DBUserName +
                                                       ";password=" + StringCipher.Decrypt(hotelInfo.DBPassword) + ";database=" + hotelInfo.DBName + ";";

                using (IDbConnection db2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        model = db2.Query<SubmembershipModel>(query, new { mpehotel = mpehotel }).ToList();
                    }
                    catch (Exception e)
                    {
                        logger.Error("GetSubmemberships Error : " + e.ToString());
                    }
                }
            }

            return null;
        }

        public List<CustomerModel> GetReservationsTimePeriod(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId, DateTime dateFrom, DateTime dateTo)
        {
            var mpehotel = helper.HotelId;

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            List<CustomerModel> results = new List<CustomerModel>();
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var dbName = hotelInfo.DBName;
                var dbPassword = hotelInfo.DBPassword;
                var dbUserName = hotelInfo.DBUserName;
                var url = hotelInfo.HotelUri;
                var servername = hotelInfo.ServerName;

                try
                {
                    var searchBy = string.Empty;
                    var tempRoom = !string.IsNullOrEmpty(helper.Room) ? helper.Room : "";
                    var tempName = !string.IsNullOrEmpty(helper.Name) ? helper.Name : "";
                    var constring = new SqlConnectionStringBuilder();
                    constring.InitialCatalog = dbName; //"Protel";
                    constring.DataSource = servername;
                    constring.UserID = dbUserName;
                    constring.Password = StringCipher.Decrypt(dbPassword);

                    string command = "";
                    short? allH = 0; //Not All Hotels

                    var defaultmpeHotel = mpehotel;
                    if (!tempRoom.Equals(""))
                    {
                        searchBy = tempRoom;
                    }
                    else if (!tempName.Equals(""))
                    {
                        searchBy = tempName;
                    }
                    else if (!tempName.Equals("") && !tempRoom.Equals(""))
                    {
                        searchBy = tempRoom;
                    }

                    DateTime dtFromConverted = Convert.ToDateTime(dateFrom);
                    DateTime dtToConverted = Convert.ToDateTime(dateTo);
                    string dtFrom = dtFromConverted.Year.ToString() + "-" + dtFromConverted.Month.ToString().PadLeft(2, '0') + "-" + dtFromConverted.Day.ToString().PadLeft(2, '0');
                    string dtTo = dtToConverted.Year.ToString() + "-" + dtToConverted.Month.ToString().PadLeft(2, '0') + "-" + dtToConverted.Day.ToString().PadLeft(2, '0');

                    command = "EXEC " + constring.UserID + ".GetReservationInfoPeriod '', '" + dtFrom + "' , '" + dtTo + "','" + searchBy + "', " + defaultmpeHotel.ToString() + ", " + helper.Page + "," + helper.Pagesize + "," + allH.ToString();

                    logger.Info("command: " + command);
                    string connStr = "server=" + servername + ";user id=" + dbUserName +
                                                       ";password=" + StringCipher.Decrypt(dbPassword) + ";database=" + dbName + ";";
                    logger.Info("connStr: " + connStr.Substring(0, 49) + "..." + ";database: " + dbName);
                    PMSConnection PMSCon = new PMSConnection();
                    PMSCon.initConn(connStr);

                    logger.Info("Reader Init");
                    SqlDataReader myReader = PMSCon.returnResult(command);
                    logger.Info("Getting Customers...");

                    while (myReader.Read())
                    {
                        CustomerModel cust = new CustomerModel();

                        cust.TotalRecs = !myReader.IsDBNull(0) == true ? myReader.GetInt32(0) : 0;
                        cust.ReservationCode = !myReader.IsDBNull(2) == true ? myReader.GetString(2) : "";
                        cust.ReservationId = !myReader.IsDBNull(3) == true ? myReader.GetInt32(3) : 0;
                        cust.RoomId = !myReader.IsDBNull(5) == true ? myReader.GetInt32(5) : 0;
                        cust.Room = !myReader.IsDBNull(6) == true ? myReader.GetString(6) : "";
                        if (myReader.IsDBNull(7)) continue;
                        cust.ArrivalDate = myReader.GetDateTime(7);
                        if (myReader.IsDBNull(8)) continue;
                        cust.DepartureDate = myReader.GetDateTime(8);
                        cust.ProfileNo = !myReader.IsDBNull(9) == true ? myReader.GetInt32(9) : 0;
                        cust.FirstName = !myReader.IsDBNull(10) == true ? myReader.GetString(10) : "";
                        cust.LastName = !myReader.IsDBNull(11) == true ? myReader.GetString(11) : "";
                        cust.Member = !myReader.IsDBNull(12) == true ? myReader.GetString(12) : "";
                        cust.Password = !myReader.IsDBNull(13) == true ? myReader.GetString(13) : "";
                        cust.Address = !myReader.IsDBNull(14) == true ? myReader.GetString(14) : "";
                        cust.City = !myReader.IsDBNull(15) == true ? myReader.GetString(15) : "";
                        cust.PostalCode = !myReader.IsDBNull(16) == true ? myReader.GetString(16) : "";
                        cust.Country = !myReader.IsDBNull(17) == true ? myReader.GetString(17) : "";
                        if (myReader.IsDBNull(18)) continue;
                        cust.BirthdayDate = myReader.GetDateTime(18);
                        cust.Email = !myReader.IsDBNull(19) == true ? myReader.GetString(19) : "";
                        cust.Telephone = !myReader.IsDBNull(20) == true ? myReader.GetString(20) : "";
                        cust.VIP = !myReader.IsDBNull(21) == true ? myReader.GetInt32(21).ToString() : "";
                        cust.Benefits = !myReader.IsDBNull(22) == true ? myReader.GetString(22) : "";
                        cust.NationalityId = !myReader.IsDBNull(23) == true ? myReader.GetInt32(23) : 0;
                        cust.Adults = !myReader.IsDBNull(24) == true ? myReader.GetInt32(24) : 0;
                        cust.Children = !myReader.IsDBNull(25) == true ? myReader.GetInt32(25) : 0;
                        cust.Note1 = !myReader.IsDBNull(26) == true ? myReader.GetString(26) : "";
                        cust.Note2 = !myReader.IsDBNull(27) == true ? myReader.GetString(27) : "";
                        cust.CustomerId = !myReader.IsDBNull(28) == true ? myReader.GetInt32(28).ToString() : "";
                        cust.IsSharer = !myReader.IsDBNull(29) == true ? myReader.GetBoolean(29) : false;
                        cust.BoardCode = !myReader.IsDBNull(30) == true ? myReader.GetString(30) : "";
                        cust.BoardName = !myReader.IsDBNull(31) == true ? myReader.GetString(31) : "";
                        cust.NoPos = !myReader.IsDBNull(32) == true ? myReader.GetInt32(32) : 0;
                        ////Απαραίτητη πληροφορία loyalty του πελάτη
                        cust.ClassId = !myReader.IsDBNull(33) == true ? myReader.GetInt32(33) : 0;
                        cust.ClassName = !myReader.IsDBNull(34) == true ? myReader.GetString(34) : "";
                        cust.AvailablePoints = !myReader.IsDBNull(35) == true ? myReader.GetInt32(35) : 0;
                        cust.FnbDiscount = !myReader.IsDBNull(36) == true ? myReader.GetInt32(36) : 0;
                        cust.Ratebuy = !myReader.IsDBNull(37) == true ? myReader.GetInt32(37) : 0;
                        cust.GuestFuture = !myReader.IsDBNull(38) == true ? myReader.GetString(38) : "";
                        cust.RoomTypeId = !myReader.IsDBNull(39) == true ? myReader.GetInt32(39) : 0;
                        cust.RoomType = !myReader.IsDBNull(40) == true ? myReader.GetString(40) : "";
                        cust.BookedRoomTypeId = !myReader.IsDBNull(41) == true ? myReader.GetInt32(41) : 0;
                        cust.BookedRoomType = !myReader.IsDBNull(42) == true ? myReader.GetString(42) : "";
                        cust.TravelAgentId = !myReader.IsDBNull(43) == true ? myReader.GetInt32(43) : 0;
                        cust.TravelAgent = !myReader.IsDBNull(44) == true ? myReader.GetString(44) : "";
                        cust.CompanyId = !myReader.IsDBNull(45) == true ? myReader.GetInt32(45) : 0;
                        cust.Company = !myReader.IsDBNull(46) == true ? myReader.GetString(46) : "";
                        cust.GroupId = !myReader.IsDBNull(47) == true ? myReader.GetInt32(47) : 0;
                        cust.GroupName = !myReader.IsDBNull(48) == true ? myReader.GetString(48) : "";
                        cust.SourceId = !myReader.IsDBNull(49) == true ? myReader.GetInt32(49) : 0;
                        cust.Source = !myReader.IsDBNull(50) == true ? myReader.GetString(50) : "";
                        cust.Mpehotel = !myReader.IsDBNull(51) == true ? myReader.GetInt32(51) : 0;
                        cust.ReservStatus = !myReader.IsDBNull(52) == true ? (CustomerModel.ReservStatusEnum)myReader.GetInt32(52) : 0;
                        cust.NationalityCode = !myReader.IsDBNull(53) == true ? myReader.GetString(53) : "";
                        cust.LoyaltyProgramId = !myReader.IsDBNull(54) == true ? myReader.GetInt32(54) : 0;
                        cust.LoyaltyLevelId = !myReader.IsDBNull(55) == true ? myReader.GetInt32(55) : 0;
                        cust.RateCodeId = !myReader.IsDBNull(56) == true ? myReader.GetInt32(56) : 0;
                        cust.RateCodeDescr = !myReader.IsDBNull(57) == true ? myReader.GetString(57) : "";
                        cust.CardType = !myReader.IsDBNull(58) == true ? myReader.GetString(58) : "";

                        results.Add(cust);
                    }
                    return results;
                }
                catch (Exception e)
                {
                    logger.Error("GetReservations Error : " + e.ToString());
                    return null;
                }
            }
        }

        /// <summary>
        /// Return a List of CustomerModel based on Reservation Confirmation Code
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="helper"></param>
        /// <param name="hotelInfoId"></param>
        /// <returns></returns>
        public List<CustomerModel> GetReservationByConfirmCode(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId)
        {
            var mpehotel = helper.HotelId;
            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            string confirmCode = helper.ReservationId;

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            List<CustomerModel> results = new List<CustomerModel>();
            var dbName = hotelInfo.DBName;
            var dbPassword = hotelInfo.DBPassword;
            var dbUserName = hotelInfo.DBUserName;
            var servername = hotelInfo.ServerName;
            var constring = new SqlConnectionStringBuilder();
            constring.InitialCatalog = dbName;//"Protel";
            constring.DataSource = servername;
            constring.UserID = dbUserName;
            constring.Password = StringCipher.Decrypt(dbPassword);
            string connStr = "server=" + servername + ";user id=" + dbUserName +
                                                      ";password=" + StringCipher.Decrypt(dbPassword) + ";database=" + dbName + ";";
            using (IDbConnection db = new SqlConnection(connStr))
            {
                try
                {
                    string command = "EXEC " + constring.UserID + ".GetReservationInfoPeriodAllReservs '" + confirmCode + "'";

                    logger.Info("command: " + command);
                    logger.Info("connStr: " + connStr.Substring(0, 49) + "..." + ";database: " + dbName);
                    PMSConnection PMSCon = new PMSConnection();
                    PMSCon.initConn(connStr);

                    logger.Info("Getting Customers...");
                    results = ((SqlConnection)db).Query<CustomerModel>(constring.UserID + ".GetReservationInfoPeriodAllReservs", new { confirmationCode = "" + confirmCode + ""}, commandType: CommandType.StoredProcedure).ToList();


                    logger.Info("Reservations retrieved: " + results.Count + ", checked out: " + results.Where(r => r.DepartureDate < DateTime.Now.Date).ToList().Count);

                    return results;
                }
                catch (Exception e)
                {
                    logger.Error("GetReservations Error : " + e.ToString());
                    return null;
                }
            }
        }

        public List<CustomerModel> GetReservationsNew(DBInfoModel DBInfo, PosReservationHelper helper, int hotelInfoId, DateTime? dt)
        {
            var mpehotel = helper.HotelId;


            string confirmCode = "";
            //1. Reservation Id encrypted with HashIds
            if (!string.IsNullOrWhiteSpace(helper.ReservationId))
            {
                HashIdsHelper hasHlp = new HashIdsHelper();
                int reservId = hasHlp.DecodeIds(helper.ReservationId);
                confirmCode = reservId.ToString();
            }

            HotelsInfoModel hotelInfo = GetMpehotelQuery(DBInfo, hotelInfoId, mpehotel);

            if (hotelInfo == null)
            {
                logger.Error("No hotelInfo for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            if (hotelInfo.DBUserName == null)
            {
                logger.Error("No hotelInfo user name for hotelid:" + hotelInfoId + ",mpehotel:" + mpehotel);

                return null;
            }

            List<CustomerModel> results = new List<CustomerModel>();
            var dbName = hotelInfo.DBName;
            var dbPassword = hotelInfo.DBPassword;
            var dbUserName = hotelInfo.DBUserName;
            var servername = hotelInfo.ServerName;
            var constring = new SqlConnectionStringBuilder();
            constring.InitialCatalog = dbName;//"Protel";
            constring.DataSource = servername;
            constring.UserID = dbUserName;
            constring.Password = StringCipher.Decrypt(dbPassword);
            string connStr = "server=" + servername + ";user id=" + dbUserName +
                                                      ";password=" + StringCipher.Decrypt(dbPassword) + ";database=" + dbName + ";";
            using (IDbConnection db = new SqlConnection(connStr))
            {
                try
                {
                    var searchBy = string.Empty;
                    var tempRoom = !string.IsNullOrEmpty(helper.Room) ? helper.Room : "";
                    var tempName = !string.IsNullOrEmpty(helper.Name) ? helper.Name : "";
                    
                    string command = "";
                    short? allH = 0; //Not All Hotels

                    var defaultmpeHotel = mpehotel;
                    if (!tempRoom.Equals(""))
                    {
                        searchBy = tempRoom;
                    }
                    else if (!tempName.Equals(""))
                    {
                        searchBy = tempName;
                    }
                    else if (!tempName.Equals("") && !tempRoom.Equals(""))
                    {
                        searchBy = tempRoom;
                    }

                    command = "EXEC " + constring.UserID + ".GetReservationInfo2 '" + confirmCode + "' , '" + searchBy + "'," + defaultmpeHotel.ToString() + "," + helper.Page + "," + helper.Pagesize + "," + allH.ToString();

                    logger.Info("command: " + command);
                    logger.Info("connStr: " + connStr.Substring(0, 49) + "..." + ";database: " + dbName);
                    PMSConnection PMSCon = new PMSConnection();
                    PMSCon.initConn(connStr);

                    logger.Info("Getting Customers...");
                    results = ((SqlConnection)db).Query<CustomerModel>(constring.UserID + ".GetReservationInfo2", new { confirmationCode = "" + confirmCode + "", room = "" + searchBy + "", mpeHotel = defaultmpeHotel.ToString(), pageNo = helper.Page, pageSize = helper.Pagesize, bAllHotels = allH.ToString() }, commandType: CommandType.StoredProcedure).ToList();

                    if (!string.IsNullOrEmpty(tempRoom))
                        logger.Info("Search for room: " + tempRoom);

                    logger.Info("Reservations retrieved: " + results.Count + ", checked out: " + results.Where(r => r.DepartureDate < DateTime.Now.Date).ToList().Count);

                    return results;
                }
                catch (Exception e)
                {
                    logger.Error("GetReservations Error : " + e.ToString());
                    return null;
                }
            }
        }

        /// <summary>
        /// Get remaining allowance and macro results if exist for selected customer
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="helper">PosReservationHelper</param>
        /// <param name="customer">CustomerModel</param>
        /// <returns>AllowanceModel</returns>
        public Dictionary<string, AllowanceModel> GetAllowance(DBInfoModel DBInfo, PosReservationHelper helper, CustomerModel customer, DateTime? dt)
        {
            if (dt == null)
            {
                dt = DateTime.Now;
            }

            DateTime processedDateTime = Convert.ToDateTime(dt);

            Dictionary<string, AllowanceModel> result = new Dictionary<string, AllowanceModel>();

            MacroTimezoneModel timezone = new MacroTimezoneModel();

            List<MacroTimezoneModel> timezones = new List<MacroTimezoneModel>();

            List<MacroModel> macros = new List<MacroModel>();

            List<Models.Models.Hotel.MealConsumptionModel> consumption = GetConsumption(DBInfo, processedDateTime, customer.ReservationId, customer.Room, helper.HotelId);

            timezones = timezoneDT.CashedDT.Select(DBInfo).ToList();

            timezone = GetActiveTimezone(processedDateTime, timezones);
            //timezones.Where(t => Convert.ToInt32(t.TimeFrom.ToString("HHmm")) <= Convert.ToInt32(processedDateTime.ToString("HHmm")) && Convert.ToInt32(t.TimeTo.ToString("HHmm")) >= Convert.ToInt32(processedDateTime.ToString("HHmm"))).FirstOrDefault();

            if (timezone == null)
            {
                return null;
            }

            macros = hotelMacrosDT.CashedDT.Select(DBInfo).OrderByDescending(m => m.Priority).ToList();

            HotelAllowanceMacroHelper allowanceHelper = new HotelAllowanceMacroHelper();

            result.Add("", allowanceHelper.GetMacroResults(DBInfo, helper, customer, timezone, timezones, macros, consumption));

            return result;
        }

        public Dictionary<string, AllowanceModel> GetMacroOverride(DBInfoModel DBInfo, PosReservationHelper helper, CustomerModel customer, DateTime? dt)
        {
            List<MacroTimezoneModel> timezones = timezoneDT.CashedDT.Select(DBInfo).ToList();

            List<MacroModel> macros = hotelMacrosDT.CashedDT.Select(DBInfo).OrderByDescending(m => m.Priority).ToList();

            HotelAllowanceMacroHelper allowanceHelper = new HotelAllowanceMacroHelper();

            Dictionary<string, AllowanceModel> result = allowanceHelper.GetMacroOverrideResult(DBInfo, helper, customer, timezones, macros, dt, null);

            return result;
        }

        public MacroTimezoneModel GetActiveTimezone(DateTime processedDateTime, List<MacroTimezoneModel> timezones)
        {
            int currentTime = Convert.ToInt32(processedDateTime.ToString("HHmm"));

            foreach (MacroTimezoneModel tz in timezones)
            {
                if (tz.TimeFrom > tz.TimeTo)
                {
                    DateTime toMidnight = new DateTime(1, 1, 1, 23, 59, 59);

                    DateTime fromMidnight = new DateTime(1, 1, 1, 0, 0, 0);

                    if ((currentTime >= Convert.ToInt32(fromMidnight.ToString("HHmm")) && currentTime <= Convert.ToInt32(tz.TimeTo.ToString("HHmm"))) ||    // now between 00:00 and timezoneTo
                        currentTime >= Convert.ToInt32(tz.TimeFrom.ToString("HHmm")) && currentTime <= Convert.ToInt32(toMidnight.ToString("HHmm")))        // now between timezoneFrom and 23:59
                        return tz;
                }
                else
                {
                    if (currentTime <= Convert.ToInt32(tz.TimeTo.ToString("HHmm")) && currentTime >= Convert.ToInt32(tz.TimeFrom.ToString("HHmm")))         // now between timezoneTo and timezoneFrom
                        return tz;
                }
            }

            return null;
        }

        public void GetMetadata()
        {
            //// TODO: GET PARAMETER FOR specific metadata.type / metadata.xkey    &   customerid(metadata.ref??)
            //int metadataType = 0;
            //string xkey = "";

            //string hotelInfoQuery = "SELECT * FROM HotelInfo WHERE MPEHotel = @mpehotel";
            //HotelsInfoModel hotelinfo = db.Query<HotelsInfoModel>(hotelInfoQuery, new { mpehotel = helper.HotelId }).FirstOrDefault();
            //var constring = new SqlConnectionStringBuilder();
            //constring.InitialCatalog = hotelinfo.DBName;
            //constring.DataSource = hotelinfo.ServerName;
            //constring.UserID = hotelinfo.DBUserName;
            //constring.Password = StringCipher.Decrypt(hotelinfo.DBPassword);
            //string connStr = "server=" + constring.DataSource + ";user id=" + constring.UserID + ";password=" + constring.Password + ";database=" + constring.InitialCatalog + ";";
            //string command = "SELECT * FROM metadata WHERE MPEHotel = " + helper.HotelId + " AND ref = " + customer.ProfileNo + " AND type = " + metadataType + " AND xkey = '" + xkey + "'";
            //PMSConnection PMSCon = new PMSConnection();
            //PMSCon.initConn(connStr);
            //SqlDataReader myReader = PMSCon.returnResult(command);
            //while (myReader.Read())
            //{
            //    MetadataModel met = new MetadataModel();
            //    met.iref = myReader.GetInt32(0);
            //    met.mpehotel = myReader.GetInt32(1);
            //    met.Ref = myReader.GetInt32(2);
            //    met.type = myReader.GetInt32(3);
            //    met.xkey = myReader.GetString(4);
            //    met.data = myReader.GetString(5);
            //    met.udata = myReader.GetString(6);
            //    met.ldata = myReader.GetInt32(7);
            //    met.ddata = myReader.GetDateTime(8);
            //    met.validfor = myReader.GetDateTime(9);
            //    metadata.Add(met);
            //}      
        }

        public List<Models.Models.Hotel.MealConsumptionModel> GetConsumption(DBInfoModel DBInfo, DateTime date, int reservationID, string room, int mpehotel)
        {
            List<Models.Models.Hotel.MealConsumptionModel> result = new List<Models.Models.Hotel.MealConsumptionModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            //TODO: ADD ROOM / mpehotel FILTER
            string queryConsumption = "" +
                "SELECT timezone AS timezoneCode, SUM(ConsumedMeals) AS adultConsumption, SUM(ConsumedMealsChild) AS childConsumption " +
                "FROM MealConsumption " +
                "WHERE ReservationId = @reservationID AND CAST(CONVERT(VARCHAR,ConsumedTS,120) AS DATE) = CAST(CONVERT(VARCHAR,@CurrDate,120) AS DATE) " +
                "GROUP BY timezone";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string dt = date.Year.ToString() + "-" + date.Month.ToString().PadLeft(2, '0') + "-" + date.Day.ToString().PadLeft(2, '0');

                result = db.Query<Models.Models.Hotel.MealConsumptionModel>(queryConsumption, new { reservationID = reservationID, CurrDate = dt }).ToList();
            }

            return result;
        }

        public List<MealConsumptionDetailModel> GetConsumptionDetails(DBInfoModel DBInfo, DateTime date, int reservationID, string room, int mpehotel)
        {
            List<MealConsumptionDetailModel> result = new List<MealConsumptionDetailModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            string queryConsumption = "" +
                "SELECT consumedts AS tmstamp, timezone AS timezoneCode, SUM(ConsumedMeals) AS adultConsumption, " +
                "SUM(ConsumedMealsChild) AS childConsumption, BoardCode, mc.DepartmentId, d.[Description] AS Department, PosInfoId, p.[Description] AS PosInfo " +
                "FROM MealConsumption mc " +
                "INNER JOIN Department d ON mc.DepartmentId = d.Id " +
                "INNER JOIN PosInfo p ON mc.PosInfoId = p.Id " +
                "WHERE ReservationId = @reservationID AND CAST (CONVERT(VARCHAR,ConsumedTS,120) AS DATE) = CAST(CONVERT(VARCHAR,@CurrDate,120) AS DATE) " +
                "GROUP BY consumedts, timezone, BoardCode, mc.DepartmentId, PosInfoId, d.[Description], p.[Description]";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string dt = date.Year.ToString() + "-" + date.Month.ToString().PadLeft(2, '0') + "-" + date.Day.ToString().PadLeft(2, '0');

                result = db.Query<MealConsumptionDetailModel>(queryConsumption, new { reservationID = reservationID, CurrDate = dt }).ToList();
            }

            return result;
        }

        public List<HotelAllowancesPerDay> GetHotelAllowances(DBInfoModel DBInfo, DateTime dateFrom, DateTime dateTo, int hotelInfoId, int mpehotel)
        {
            //checks if need to call Api Url for Hotelizer or other external systems and returns null because no data from external systems can be reached
            bool extApi = false;
            extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
            if (extApi)
                return null;


            List<HotelAllowancesPerDay> results = new List<HotelAllowancesPerDay>();

            List<CustomerModel> reservations = GetReservationsTimePeriod(DBInfo, new PosReservationHelper { HotelId = mpehotel, Page = 0, Pagesize = 10000 }, hotelInfoId, dateFrom, dateTo);

            if (reservations == null || reservations.Count < 1)
            {
                return null;
            }

            List<DateTime> days = DateTimeHelper.EachDay(dateFrom, dateTo).ToList();

            foreach (DateTime day in days)
            {
                HotelAllowancesPerDay dayResult = new HotelAllowancesPerDay();

                dayResult.date = day;

                dayResult.rooms = GetHotelAllowancesPerRoom(DBInfo, reservations.Where(r => r.ArrivalDate <= day && r.DepartureDate >= day).ToList(), day, hotelInfoId, mpehotel, dateTo);

                results.Add(dayResult);
            }

            return results;
        }

        public List<HotelAllowancesPerRoom> GetHotelAllowancesPerRoom(DBInfoModel DBInfo, List<CustomerModel> reservations, DateTime day, int hotelInfoId, int mpehotel, DateTime dt)
        {
            List<HotelAllowancesPerRoom> results = new List<HotelAllowancesPerRoom>();

            List<string> rooms = reservations.OrderBy(x => x.Room).Select(x => x.Room).Distinct().ToList();

            List<MacroModel> macros = GetMacros(DBInfo);

            //List<MacroTimezoneModel> timezones = timezoneDT.CashedDT.Select(DBInfo).ToList();

            //List<MacroModel> macros = hotelMacrosDT.CashedDT.Select(DBInfo).OrderBy(m => m.Priority).ToList();

            if (rooms == null || rooms.Count < 1)
            {
                return null;
            }

            foreach (string room in rooms)
            {
                HotelAllowancesPerRoom roomResult = new HotelAllowancesPerRoom();

                List<CustomerModel> roomCustomers = new List<CustomerModel>();

                roomCustomers = reservations.Where(r => r.Room == room).ToList();

                foreach (CustomerModel customer in roomCustomers)
                {
                    CustomerAllowanceModel customerAllowance = new CustomerAllowanceModel();

                    customerAllowance.customer = customer;

                    //customerAllowance.allowance = HotelAllowanceMacroHelper.CalculateAllowances(DBInfo, customer, timezones, macros, dt);

                    if (day <= DateTime.Now)
                    {
                        customerAllowance.consumption = GetConsumptionDetails(DBInfo, day, customer.ReservationId, room, mpehotel);
                    }
                    else
                    {
                        customerAllowance.consumption = null;
                    }

                    roomResult.customers.Add(customerAllowance);
                }

                roomResult.room = room;

                roomResult.hasOverride = CheckOverride(macros, roomCustomers, day);

                results.Add(roomResult);
            }

            return results;
        }

        public bool CheckOverride(List<MacroModel> macros, List<CustomerModel> customers, DateTime day)
        {
            int count = macros.Where(
            x => x.MacroRules != null && x.MacroRules.ReservationId != null && customers.Where(c => c.ReservationId == x.MacroRules.ReservationId).Any() &&
            (x.ActiveFrom == null && x.ActiveTo != null ? day < x.ActiveTo :
            x.ActiveFrom != null && x.ActiveTo == null ? day > x.ActiveFrom :
            x.ActiveFrom != null && x.ActiveTo != null ? x.ActiveFrom == x.ActiveTo && day == x.ActiveFrom : true ?
            day > x.ActiveFrom && day < x.ActiveTo : true)
            ).Count();

            return count > 0 ? true : false;
        }

        /// <summary>
        /// get pos guest id matching customer
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="customer">CustomerModel</param>
        /// <returns>int</returns>
        public int GetGuestId(DBInfoModel DBInfo, CustomerModel customer)
        {
            int GuestId;

            string query = @"
                SELECT id 
                FROM guest 
                WHERE 
	                ProfileNo = @profileNo AND 
	                reservationid = @reservationId";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                GuestId = db.Query<int>(query, new { profileNo = customer.ProfileNo, reservationId = customer.ReservationId }).FirstOrDefault();
            }

            return GuestId;
        }

        /// <summary>
        /// insert pos guest
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="customer">CustomerModel</param>
        /// <returns>int</returns>
        public int InsertGuest(DBInfoModel DBInfo, CustomerModel customer)
        {
            int? GuestId;

            Guest newGuest = new Guest();

            newGuest = MapGuestWithCustomer(customer);

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                GuestId = db.Insert(newGuest);
            }

            return GuestId == null ? 0 : Convert.ToInt32(GuestId);
        }

        /// <summary>
        /// update pos guest
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="customer">CustomerModel</param>
        public void UpdateGuest(DBInfoModel DBInfo, CustomerModel customer, int? id)
        {
            Guest newGuest = new Guest();

            newGuest = MapGuestWithCustomer(customer);

            newGuest.Id = (long)id;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Update(newGuest);
            }
        }

        /// <summary>
        /// map customer model data to guest model data
        /// </summary>
        /// <param name="customer">CustomerModel</param>
        /// <returns>GuestModel</returns>
        public Guest MapGuestWithCustomer(CustomerModel customer)
        {
            Guest newGuest = new Guest();

            newGuest.Address = customer.Address;
            newGuest.Adults = customer.Adults;
            newGuest.Arrival = Convert.ToString(customer.ArrivalDate);
            newGuest.arrivalDT = customer.ArrivalDate;
            newGuest.AvailablePoints = customer.AvailablePoints;
            newGuest.Benefits = customer.Benefits;
            newGuest.Birthday = Convert.ToString(customer.BirthdayDate);
            newGuest.birthdayDT = customer.BirthdayDate;
            newGuest.BoardCode = customer.BoardCode;
            newGuest.BoardName = customer.BoardName;
            newGuest.Children = customer.Children;
            newGuest.City = customer.City;
            newGuest.ClassId = customer.ClassId;
            newGuest.ClassName = customer.ClassName;
            newGuest.ConfirmationCode = null;
            newGuest.Country = customer.Country;
            newGuest.Departure = Convert.ToString(customer.DepartureDate);
            newGuest.departureDT = customer.DepartureDate;
            newGuest.Email = customer.Email;
            newGuest.FirstName = customer.FirstName;
            newGuest.fnbdiscount = customer.FnbDiscount;
            newGuest.HotelId = customer.Mpehotel;
            newGuest.IsSharer = customer.IsSharer;
            newGuest.LastName = customer.LastName;
            newGuest.Member = customer.Member;
            newGuest.NationalityCode = customer.NationalityCode;
            newGuest.Note1 = customer.Note1;
            newGuest.Note2 = customer.Note2;
            newGuest.Password = customer.Password;
            newGuest.PostalCode = customer.PostalCode;
            newGuest.ProfileNo = customer.ProfileNo;
            newGuest.ratebuy = customer.Ratebuy;
            newGuest.ReservationCode = customer.ReservationCode;
            newGuest.ReservationId = customer.ReservationId;
            newGuest.Room = customer.Room;
            newGuest.RoomId = customer.RoomId;
            newGuest.Telephone = customer.Telephone;
            newGuest.Title = null;
            newGuest.Type = null;
            newGuest.VIP = customer.VIP;

            return newGuest;
        }

        /// <summary>
        /// return customMessages that match customers parameters
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="customer">CustomerModel</param>
        /// <param name="messages">List<CustomMessageModel></param>
        /// <returns>List<CustomMessageModel></returns>
        public List<string> GetCustomerCustomMessages(DBInfoModel DBInfo, CustomerModel customer, List<CustomMessageModel> messages)
        {
            List<string> validMessages = new List<string>();

            foreach (CustomMessageModel message in messages.OrderByDescending(p => p.Priority).ToList())
            {
                object customerParamValue = customer.GetType().GetProperty(message.Param).GetValue(customer, null) ?? "";
                if (message.Value.ToString() == customerParamValue.ToString())
                {
                    if (message.Message.Contains("<value>"))
                    {
                        message.Message = message.Message.Replace("<value>", message.Value.ToString());
                    }

                    validMessages.Add(message.Message);
                }
            }

            return validMessages;
        }

        public List<MacroModel> GetMacros(DBInfoModel DBInfo)
        {
            List<MacroModel> model = new List<MacroModel>();

            List<MacroDBModel> dbData = new List<MacroDBModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    dbData = AutoMapper.Mapper.Map<List<MacroDBModel>>(genMacro.Select(db, null, null));
                }

                foreach (MacroDBModel row in dbData)
                {

                    model.Add(JsonConvert.DeserializeObject<MacroModel>(row.Model));
                }
            }
            catch (Exception e)
            {
                logger.Error("GetMacros Error : " + e.ToString());
            }

            return model;
        }

        public List<Hotel__CustomerDataConfigModel> GetCustomerDataConfig(DBInfoModel DBInfo)
        {
            List<Hotel__CustomerDataConfigModel> result = new List<Hotel__CustomerDataConfigModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            string query = "SELECT * FROM HotelCustomerDataConfig ORDER BY priority";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                result = db.Query<Hotel__CustomerDataConfigModel>(query).ToList();
            }

            return result;
        }

        public void InsertCustomerDataConfig(DBInfoModel DBInfo, Hotel__CustomerDataConfigModel data)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            HotelCustomerDataConfigDTO dat = AutoMapper.Mapper.Map<HotelCustomerDataConfigDTO>(data);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                genCustomerDataConfig.Insert(db, dat);
            }
        }

        public void UpdateCustomerDataConfig(DBInfoModel DBInfo, Hotel__CustomerDataConfigModel data)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            HotelCustomerDataConfigDTO dat = AutoMapper.Mapper.Map<HotelCustomerDataConfigDTO>(data);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                genCustomerDataConfig.Update(db, dat);
            }
        }

        public void DeleteCustomerDataConfigField(DBInfoModel DBInfo, long Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            string query = "DELETE FROM HotelCustomerDataConfig WHERE Id=@Id";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(query, new { Id = Id });
            }
        }

        /// <summary>
        /// get Dictionary of Guest ids and ProfileNo based on given ProfileNo
        /// </summary>
        /// <param name="guestIdsList">List<long></param>
        /// <returns></returns>
        public Dictionary<int, int> GetGuestIds(DBInfoModel DBInfo, GuestIdsList guestIdsList)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            List<guestResultModel> tmpGuestResultModel = new List<guestResultModel>();

            if (guestIdsList == null)
            {
                return result;
            }
            if (guestIdsList.guestIdsList.Count == 0)
            {
                return result;
            }

            string ids = String.Join(",", guestIdsList.guestIdsList);

            string SqlData = @"SELECT g.Id AS GuestId, g.ProfileNo FROM Guest AS g WHERE g.ProfileNo IN (" + ids + ")";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                tmpGuestResultModel = db.Query<guestResultModel>(SqlData).ToList();
            }

            foreach (guestResultModel row in tmpGuestResultModel)
            {
                if (result.ContainsKey(row.ProfileNo) == false)
                {
                    result.Add(row.ProfileNo, row.GuestId);
                }
            }

            return result;
        }

        public int GetGuestIdFromProfileAndRoom(DBInfoModel DBInfo, GuestGetIdModel guestGetIdModel)
        {
            int result = 0;

            if (guestGetIdModel == null)
            {
                return result;
            }

            string SqlData = @"SELECT g.Id AS GuestId, g.ProfileNo FROM Guest AS g WHERE g.ProfileNo =@profileNo AND g.RoomId =@roomId";

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                result = db.Query<int>(SqlData, new { profileNo = guestGetIdModel.ProfileNo, roomId = guestGetIdModel.RoomId }).FirstOrDefault();
            }

            return result;
        }
    }
}

