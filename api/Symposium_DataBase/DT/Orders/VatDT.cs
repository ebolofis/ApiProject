using AutoMapper;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.DAOs;
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


namespace Symposium.WebApi.DataAccess.DT {
    public class VatDT : IVatDT
    {
        string connectionString;
        IGenericDAO<VatDTO> vatDao;
        IUsersToDatabasesXML usersToDatabases;

        public VatDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<VatDTO> _vatDao) {
            this.vatDao = _vatDao;
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Return the list of vats registered Accounts
        /// </summary>
        /// <param name="Store">Store</param>
        /// <returns>a list of AccountModel</returns>
        public List<VatModel> GetVatModels(Models.Models.DBInfoModel Store) {
            //1. Return all vat Objects 
            List<VatModel> vatsDto = new List<VatModel>();//DTO
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString)) {
                //vatsDto = vatDao.Select(db, "where IsDeleted=0 or IsDeleted is  null", null);
            }

            //2. AccountsDto -> AccountModel
            return AutoMapper.Mapper.Map<List<VatModel>>(vatsDto);

        }

        /// <summary>
        /// Retrun's a list with all vats Included deleted
        /// If One of them is deleted and on da is not then we have to update to not deleted
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<VatModel> GetAllVats(DBInfoModel Store, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            if (dbTran != null)
            {
                string sql = "SELECT * FROM Vat";
                return Mapper.Map<List<VatModel>>(vatDao.Select(dbTran, sql, null, dbTransact));
            }
            else
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM Vat";
                    return Mapper.Map<List<VatModel>>(vatDao.Select(db, sql, null, dbTransact));
                }
            }
        }


        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<VatSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = vatDao.Upsert(db, Mapper.Map<List<VatDTO>>(model));
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
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<VatSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (VatSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    item.Id = GetIdByDAIs(Store, item.TableId ?? 0) ?? 0;
                }

                results = vatDao.DeleteOrSetIsDeletedList(db, Mapper.Map<List<VatDTO>>(model));
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
        public int UpdateModel(DBInfoModel Store, VatModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = vatDao.Update(db, Mapper.Map<VatDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<VatModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = vatDao.UpdateList(db, Mapper.Map<List<VatDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, VatModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = vatDao.Insert(db, Mapper.Map<VatDTO>(item));
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
                VatDTO tmp = vatDao.SelectFirst(db, "WHERE DAId = @DAId", new { DAId = dAId });
                if (tmp == null)
                    return null;
                else
                    return tmp.Id;
            }
        }
    }
}
