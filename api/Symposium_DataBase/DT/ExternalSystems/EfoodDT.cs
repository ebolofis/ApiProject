using Dapper;
using Symposium.Models.Models;
using Symposium.Models.Models.ExternalSystems.Efood;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.ExternalSystems;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.ExternalSystems
{
   public class EfoodDT: IEfoodDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<EFoodBucketDTO> effodDao;

        public EfoodDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<EFoodBucketDTO> effodDao)
        {
            this.usersToDatabases = usersToDatabases;
            this.effodDao = effodDao;
        }

        /// <summary>
        /// search an item from efood bucket based on efood order id
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="id">efood order id</param>
        /// <returns>EFoodBucketModel</returns>
        public ExtDeliveryBucketModel GetOrder(DBInfoModel Store, string id)
        {
            ExtDeliveryBucketModel item = null;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                item = db.QueryFirstOrDefault<ExtDeliveryBucketModel>("select * from EFoodBucket where Id=@Id", new { Id= id });
            }

            return item;
        }

        /// <summary>
        /// return all bucket from DB (only IsDeleted=false)
        /// </summary>
        /// <param name="Store">db</param>
        /// <returns></returns>
        public List<ExtDeliveryBucketModel> GetOrders(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<ExtDeliveryBucketModel>("select * from EFoodBucket where isDeleted=0").ToList();
            }
        }

        /// <summary>
        /// returns size of bucket
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public int GetBucketSize(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<int>("select count(*) from EFoodBucket where isDeleted=0").FirstOrDefault();
            }
        }


        /// <summary>
        /// upsert order into bucket
        /// </summary>
        /// <param name="Store">db</param>
        /// <returns></returns>
        public void UpsertOrder(DBInfoModel Store, ExtDeliveryBucketModel model)
        {
            EFoodBucketDTO modelDto = AutoMapper.Mapper.Map<EFoodBucketDTO>(model);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int c = effodDao.RecordCount(db, "where Id=@Id",new {Id=model.Id });
                if(c==0)
                   effodDao.Insert<string>(db, modelDto);
                else
                    effodDao.Update(db, modelDto);
            }
        }

        /// <summary>
        /// delete order from bucket
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">efood Order Id</param>
        /// <returns></returns>
        public void DeleteOrder(DBInfoModel Store, string Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                effodDao.Delete(db, Id);
            }
        }

        /// <summary>
        /// delete old orders marked as deleted from bucket
        /// </summary>
        /// <param name="Store">db</param>
        /// <returns></returns>
        public void DeleteOldOrder(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.ExecuteAsync("delete [EFoodBucket] where  IsDeleted=1  and CreateDate < DATEADD(dd,-2,GETDATE())");
              //  effodDao.Execute(db, "delete [EFoodBucket] where  IsDeleted=1  and CreateDate < DATEADD(dd,-2,GETDATE())");
            }
        }


        /// <summary>
        /// mark a bucket order as deleted
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">efood Order Id</param>
        /// <returns></returns>
        public void MarkDeleted(DBInfoModel Store, string Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                effodDao.Execute(db, "update [EFoodBucket] set IsDeleted=1  where Id=@Id", new { Id = Id });
            }
        }

    }
}
