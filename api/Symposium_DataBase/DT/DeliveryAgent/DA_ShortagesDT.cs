using AutoMapper;
using Dapper;
using Symposium.Helpers.Classes;
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

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_ShortagesDT : IDA_ShortagesDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<DA_ShortageProdsDTO> daShortageDao;
        LocalConfigurationHelper configHlp;

        public DA_ShortagesDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<DA_ShortageProdsDTO> daShortageDao, LocalConfigurationHelper configHlp)
        {
            this.usersToDatabases = usersToDatabases;
            this.daShortageDao = daShortageDao;
            this.configHlp = configHlp;
        }

        /// <summary>
        /// Get a List of Shortages 
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <returns></returns>
        public List<DA_ShortagesExtModel> GetShortages(DBInfoModel dbInfo)
        {
            List<DA_ShortagesExtModel> shortagesModel = new List<DA_ShortagesExtModel>();
            configHlp.CheckDeliveryAgent();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetShortages = @"SELECT dsp.*, p.[Description] AS ProductDescr, s.[Title] AS StoreDescr
                                            FROM DA_ShortageProds AS dsp
                                            INNER JOIN Product AS p ON dsp.ProductId = p.Id
                                            INNER JOIN DA_Stores AS s ON dsp.StoreId = s.Id";
                shortagesModel = db.Query<DA_ShortagesExtModel>(sqlGetShortages).ToList();
            }

            return shortagesModel;
        }

        /// <summary>
        /// Get a List of Shortages for a store
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="StoreId">Store Id</param>
        /// <returns></returns>
        public List<DA_ShortagesExtModel> GetShortagesByStore(DBInfoModel dbInfo, long StoreId)
        {
            List<DA_ShortagesExtModel> shortagesModel = new List<DA_ShortagesExtModel>();
            configHlp.CheckDeliveryAgent();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetShortages = @"SELECT dsp.*, p.[Description] AS ProductDescr, p.Code AS ProductCode, s.Title AS StoreDescr
                                            FROM DA_ShortageProds AS dsp
                                            LEFT OUTER JOIN Product AS p ON dsp.ProductId = p.Id
											LEFT OUTER JOIN DA_Stores AS s ON dsp.StoreId = s.Id
											where dsp.StoreId=@Id ORDER BY dsp.Id DESC";
                shortagesModel = db.Query<DA_ShortagesExtModel>(sqlGetShortages,new { Id = StoreId }).ToList();
            }

            return shortagesModel;
        }

        /// <summary>
        /// Get Shortage by id
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="Id">DA_ShortageProds.Id</param>
        /// <returns></returns>
        public DA_ShortagesExtModel GetShortage(DBInfoModel dbInfo, int Id)
        {
            configHlp.CheckDeliveryAgent();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGetShortages = @"SELECT dsp.*, p.[Description] AS ProductDescr, s.[Description] AS StoreDescr
                                            FROM DA_ShortageProds AS dsp
                                            INNER JOIN Product AS p ON dsp.ProductId = p.Id
											INNER JOIN Store AS s ON dsp.StoreId = s.Id
											where dsp.Id=@Id";
               return db.QueryFirst<DA_ShortagesExtModel>(sqlGetShortages, new { Id = Id });
            }
        }

        /// <summary>
        /// Insert new Shortage 
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="model">DA_ShortageProdsModel to insert</param>
        /// <returns></returns>
        public long Insert(DBInfoModel dbInfo, DA_ShortageProdsModel model)
        {
            configHlp.CheckDeliveryAgent();
            DA_ShortageProdsDTO dto = Mapper.Map<DA_ShortageProdsDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //1. insert
               return daShortageDao.Insert(db,dto);

                //2. Αν η έλλειψη είναι μόνιμη τότε θα πρέπει να γραφεί και νέα εγγραφή στον DA_ScheduledTaskes για το προϊόν 
                //   με Action=0(insert)  ώστε το προϊόν στο κατάστημα να γίνει IsDeleted=true: --> Trigger
            }
        }

        public long Update(DBInfoModel dbInfo, DA_ShortageProdsModel model)
        {
            configHlp.CheckDeliveryAgent();
            DA_ShortageProdsDTO dto = Mapper.Map<DA_ShortageProdsDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //1. Update
                return daShortageDao.Update(db, dto);
            }

        }

        /// <summary>
        /// Delete a Shortage by id
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="Id">DA_ShortageProds.Id</param>
        /// <returns>return num of records affected</returns>
        public int Delete(DBInfoModel dbInfo, int Id)
        {
            configHlp.CheckDeliveryAgent();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //1. delete
               return daShortageDao.Delete(db, Id);

                //2. Αν η έλλειψη ήταν  μόνιμη τότε θα πρέπει να γραφεί και νέα εγγραφή στον DA_ScheduledTaskes για το προϊόν 
                //   με Action=2(delete) ώστε το προϊόν να προστεθεί  στο κατάστημα να γίνει IsDeleted=false: --> Trigger
            }
        }

        /// <summary>
        /// Delete all temporary Shortages for a store
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="Id">Store Id</param>
        /// <returns>return num of records affected</returns>
        public int DeleteTemp(DBInfoModel dbInfo, long StoreId)
        {
            configHlp.CheckDeliveryAgent();
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string sql = " where StoreId=@id and ShortType=0";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return daShortageDao.DeleteList(db, sql, new { id = StoreId });
            }
        }

    }
}
