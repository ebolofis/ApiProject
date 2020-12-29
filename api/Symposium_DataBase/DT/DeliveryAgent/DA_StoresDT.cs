using Dapper;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Symposium.Models.Enums;
using System.Transactions;
using System.Web.Configuration;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_StoresDT : IDA_StoresDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<DA_StoresDTO> daStoreDao;

        public DA_StoresDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<DA_StoresDTO> daStoreDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.daStoreDao = daStoreDao;
        }

        /// <summary>
        /// Get a List of Stores
        /// </summary>
        /// <returns>DA_StoreModel</returns>
        public List<DA_StoreModel> GetStores(DBInfoModel dbInfo)
        {

            List<DA_StoreModel> storeModel = new List<DA_StoreModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetStores = @"SELECT * FROM DA_Stores AS ds";
                storeModel = db.Query<DA_StoreModel>(sqlGetStores).ToList();
            }

            return storeModel;
        }

        /// <summary>
        /// Get a List of Stores With Latitude and Longtitude
        /// </summary>
        /// <returns>DA_StoreInfoModel</returns>
        public List<DA_StoreInfoModel> GetStoresPosition(DBInfoModel dbInfo)
        {
            List<DA_StoreInfoModel> storeInfoModel = new List<DA_StoreInfoModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetStores = @"SELECT ds.*, da.Latitude, da.Longtitude
                                        FROM DA_Stores AS ds
                                        INNER JOIN DA_Addresses AS da ON ds.AddressId = da.Id";
                storeInfoModel = db.Query<DA_StoreInfoModel>(sqlGetStores).ToList();
            }

            return storeInfoModel;
        }

        /// <summary>
        /// return DA_StoreInfoModel based an StoreId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DA_StoreInfoModel GetStoreById(DBInfoModel dbInfo, long Id)
        {
            DA_StoreInfoModel storeModel = new DA_StoreInfoModel();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetStore = @"SELECT ds.*, da.Latitude, da.Longtitude
                                        FROM DA_Stores AS ds
                                        LEFT OUTER JOIN DA_Addresses AS da ON ds.AddressId = da.Id
                                        WHERE ds.Id = @storeId AND da.AddressType = 2";

                storeModel = db.QueryFirstOrDefault<DA_StoreInfoModel>(sqlGetStore, new { storeId = Id });
            }

            return storeModel;
        }

        /// <summary>
        /// Return Store Id based on Store Code. If no Code found then return 0.
        /// </summary>
        /// <param name="dbInfo">dbInfo</param>
        /// <param name="Code">Store Code</param>
        /// <returns></returns>
        public long? GetStoreIdFromCode(DBInfoModel dbInfo, string Code)
        {
            if (String.IsNullOrWhiteSpace(Code)) return 0;
            DA_StoreModel storeModel;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                storeModel =  db.QueryFirstOrDefault<DA_StoreModel>("SELECT * FROM [DA_Stores] where Code=@Code",new { Code=Code});
            }
            if (storeModel != null)
                return storeModel.Id;
            else
                return 0;
        }

        /// <summary>
        /// Update DA_Store Set Notes to NUll
        /// <param name="StoreId"></param>
        /// </summary>
        public long UpdateDaStoreNotes(DBInfoModel dbInfo, long StoreId)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string SqlData = @"UPDATE DA_Stores SET Notes = NULL WHERE Id = @storeId";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Query<long>(SqlData, new { storeId = StoreId }).FirstOrDefault();
                res = 1;
            }
            return res;
        }

        /// <summary>
        /// insert new DA Store. Return new Id
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="StoreModel">DA_StoreModel</param>
        /// <returns></returns>
        public long Insert(DBInfoModel dbInfo, DA_StoreModel StoreModel)
        {
            DA_StoresDTO dto = AutoMapper.Mapper.Map<DA_StoresDTO>(StoreModel);
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                return daStoreDao.Insert(db, dto);
            }
        }


        /// <summary>
        /// update a DA Store. Return number of rows affected
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="StoreModel">DA_StoreModel</param>
        /// <returns></returns>
        public long Update(DBInfoModel dbInfo, DA_StoreModel StoreModel)
        {
            DA_StoresDTO dto = AutoMapper.Mapper.Map<DA_StoresDTO>(StoreModel);
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                return daStoreDao.Update(db, dto);
            }
        }

        /// <summary>
        /// delete a DA Store. Return number of rows affected. 
        /// Also deletes from DA_ShortageProds, DA_ScheduleTaskes, DA_PriceListAssoc, DA_Addresses
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="Id">DA_Store.Id</param>
        /// <returns></returns>
        public long Delete(DBInfoModel dbInfo, long Id)
        {
            int count;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    //1. delete all DA_ShortageProds for the specific store
                    db.Execute("delete DA_ShortageProds where StoreId=@StoreId", new { StoreId = Id });
                    //2. delete all DA_ScheduleTaskes for the specific store
                    db.Execute("delete DA_ScheduleTaskes where StoreId=@StoreId", new { StoreId = Id });
                    //3. delete all DA_PriceListAssoc for the specific store
                    db.Execute("delete DA_PriceListAssoc where StoreId=@StoreId", new { StoreId = Id });
                    //4. delete all DA_Addresses for the specific store
                    db.Execute("delete DA_Addresses where OwnerId=@StoreId and AddressType=2", new { StoreId = Id });
                    //5. delete Store
                    count = daStoreDao.Delete(db, Id);
                    //6. commit transaction
                    scope.Complete();
                }
            }
            return count;
        }


public   long BODelete(DBInfoModel dbInfo, long Id)
        {
            int count;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                    count = daStoreDao.Delete(db, Id);
            }
            return count;
        }


        /// <summary>
        /// Update Store's DeliveryTime, TakeOutTime, StoreStatus
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="daStoreId">DA_Stores.Id</param>
        /// <param name="deliveryTime">deliveryTime (min)</param>
        /// <param name="takeOutTime">takeOutTime (min)</param>
        /// <param name="storeStatus">storeStatus</param>
        public void UpdateTimesStatus(DBInfoModel dbInfo, long daStoreId, int deliveryTime, int takeOutTime, DAStoreStatusEnum storeStatus)
        {

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "update DA_Stores set DeliveryTime=@DeliveryTime, TakeOutTime=@TakeOutTime,StoreStatus=@StoreStatus where Id=@id";
                db.Execute(sql, new { DeliveryTime = deliveryTime, TakeOutTime = takeOutTime, StoreStatus = storeStatus, Id = daStoreId });
            }
        }
    }
}
