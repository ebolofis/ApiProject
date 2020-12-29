using AutoMapper;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class CategoriesDT : ICategoriesDT
    {
        IGenericDAO<CategoriesDTO> dt;
        IUsersToDatabasesXML usersToDatabases;
        string connectionString;

        public CategoriesDT(IGenericDAO<CategoriesDTO> dt, IUsersToDatabasesXML usersToDatabases)
        {
            this.dt = dt;
            this.usersToDatabases = usersToDatabases;
        }


        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<CategoriesSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = this.dt.Upsert(db, Mapper.Map<List<CategoriesDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<CategoriesSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (CategoriesSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    item.Id = GetIdByDAIs(Store, item.TableId ?? 0) ?? 0;
                }

                results = dt.DeleteOrSetIsDeletedList(db, Mapper.Map<List<CategoriesDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel Store, CategoriesModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Update(db, Mapper.Map<CategoriesDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<CategoriesModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.UpdateList(db, Mapper.Map<List<CategoriesDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, CategoriesModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = dt.Insert(db, Mapper.Map<CategoriesDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        public long? GetIdByDAIs(DBInfoModel Store, long dAId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                CategoriesDTO tmp = dt.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = dAId });
                if (tmp == null)
                    return null;
                else
                    return tmp.Id;
            }
        }

        /// <summary>
        /// Return's a Category model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public CategoriesModel GetModelById(DBInfoModel Store, long Id, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            if (dbTran != null)
                return AutoMapper.Mapper.Map<CategoriesModel>(dt.SelectFirst(dbTran, "WHERE Id = @Id", new { Id = Id }, dbTransact));
            else
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return AutoMapper.Mapper.Map<CategoriesModel>(dt.SelectFirst(db, "WHERE Id = @Id", new { Id = Id }));
                }
            }
        }
    }
}
