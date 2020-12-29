using Dapper;
using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_OpeningHoursDT :IDA_OpeningHoursDT
    {
        IUsersToDatabasesXML usersToDatabases;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DA_OpeningHoursDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }
        public List<DA_OpeningHoursModel> GetHours(DBInfoModel Store)
        {
            List<DA_OpeningHoursModel> datalist = new List<DA_OpeningHoursModel>();
           string sql = @"select * from DA_OpeningHours";

           string  connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                datalist = db.Query<DA_OpeningHoursModel>(sql).ToList();
            }
            if(datalist.Count==0)
            {
                string dastoresql = @"Select * from DA_Stores";
                List<DA_StoreModel> stores = new List<DA_StoreModel>();
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    stores = db.Query<DA_StoreModel>(dastoresql).ToList();
                    FillEmptyDA_OpeningHours(Store, stores);
                    datalist = db.Query<DA_OpeningHoursModel>(sql).ToList();
                }
            }

            return datalist;
        }

        public void FillEmptyDA_OpeningHours(DBInfoModel Store, List<DA_StoreModel> stores)
        {
            string connectionString = usersToDatabases.ConfigureConnectionString(Store);
            foreach (DA_StoreModel model in stores)
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    for(int i=0;i<=6;i++)
                    {
                        long? StoreId = model.Id;
                        int Weekday = i;
                    string sql = @"INSERT INTO DA_OpeningHours(StoreId, Weekday, OpenHour, OpenMinute, CloseHour, CloseMinute)
                                            VALUES(@StoreId,@Weekday, 13,0,1,0)";
                        db.Execute(sql, new { StoreId = StoreId, Weekday = Weekday });
                    }
                }
            }


        }


        public void SaveForStore(DBInfoModel Store, List<DA_OpeningHoursModel> hourslist)
        {
            string sqldelete = @"delete from DA_OpeningHours where StoreId=@StoreId";
            long StoreId = hourslist[0].StoreId;

            string connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(sqldelete, new { StoreId = StoreId});

                string sqlinsert = @"INSERT INTO DA_OpeningHours(StoreId, Weekday, OpenHour, OpenMinute, CloseHour, CloseMinute)
                                            VALUES(@StoreId,@Weekday, @OpenHour,@OpenMinute,@CloseHour,@CloseMinute)";
                foreach(DA_OpeningHoursModel model in hourslist)
                { 
                db.Execute(sqlinsert, new { StoreId = model.StoreId, Weekday = model.Weekday, OpenHour=model.OpenHour, OpenMinute=model.OpenMinute,CloseHour=model.CloseHour,CloseMinute=model.CloseMinute});
                }
            }





            }


        public void SaveForAllStores(DBInfoModel Store, List<DA_OpeningHoursModel> hourslist)
        {
            string sqldelete = @"delete from DA_OpeningHours";
            long StoreId = hourslist[0].StoreId;

            string connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(sqldelete);
                string dastoresql = @"Select * from DA_Stores";
                List<DA_StoreModel> stores = new List<DA_StoreModel>();
                stores = db.Query<DA_StoreModel>(dastoresql).ToList();
                foreach(DA_StoreModel mod in stores)
                {
                    string sqlinsert = @"INSERT INTO DA_OpeningHours(StoreId, Weekday, OpenHour, OpenMinute, CloseHour, CloseMinute)
                                            VALUES(@StoreId,@Weekday, @OpenHour,@OpenMinute,@CloseHour,@CloseMinute)";
                       foreach(DA_OpeningHoursModel model in hourslist)
                    {
                        db.Execute(sqlinsert, new { StoreId = mod.Id, Weekday = model.Weekday, OpenHour = model.OpenHour, OpenMinute = model.OpenMinute, CloseHour = model.CloseHour, CloseMinute = model.CloseMinute });
                    }

                    }
                }

            }

        public bool CheckDA_OpeningHours(DBInfoModel Store, DateTime date, long StoreId)
        {
            DateTime OpenFrom=DateTime.Now;
            try
            {
                string connectionString = usersToDatabases.ConfigureConnectionString(Store);

                DayOfWeek weekday = date.DayOfWeek; int weekdayint = (int)weekday; int day = 0;
                string sql = @"select * from DA_OpeningHours where StoreId=" + StoreId + " and Weekday=" + weekdayint;

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                        DA_OpeningHoursModel openhours = db.Query<DA_OpeningHoursModel>(sql).FirstOrDefault();
                    if (openhours == null)
                    {
                        logger.Warn("The table DA_OpeningHours is not configured, please configure the aformentioned table if CheckDA_OpeningHours is true! ");
                        return false;
                    }

                    OpenFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, date.Day, openhours.OpenHour, openhours.OpenMinute, 0);

                        if (openhours.CloseHour <= openhours.OpenHour)
                            day = date.Day + 1;
                        else
                            day = date.Day;

                        DateTime OpenTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day, openhours.CloseHour, openhours.CloseMinute, 0);

                        if (OpenFrom <= date && date <= OpenTo)
                            return true;
                        else
                            throw new Symposium.Helpers.BusinessException($"Working hours from {openhours.OpenHour} - {openhours.CloseHour}");
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error while checking DA_OpeningHours : " + e);
                    return false;
                }
            
        }
       
    }
}
