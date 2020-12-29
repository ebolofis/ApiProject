using AutoMapper;
using Dapper;
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

namespace Symposium.WebApi.DataAccess.DT
{
    public class AccountsDT: IAccountsDT
    {
        string connectionString;
        IGenericDAO<AccountDTO> accountDao;
        IUsersToDatabasesXML usersToDatabases;

      public AccountsDT(IUsersToDatabasesXML usersToDatabases,IGenericDAO<AccountDTO> accountDao)
        {
            this.accountDao = accountDao;
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Return the list of Active Accounts
        /// </summary>
        /// <param name="Store">Store</param>
        /// <returns>a list of AccountModel</returns>
        public List<AccountModel> GetActiveModels(Models.Models.DBInfoModel Store)
        {
            //1. Get Active Accounts from DB
            List<AccountDTO> accountsDto;//DTO
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                accountsDto= accountDao.Select(db, "where IsDeleted=0 or IsDeleted is  null",null);
            }

            //2. AccountsDto -> AccountModel
            return AutoMapper.Mapper.Map<List<AccountModel>>(accountsDto);

        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<AccountSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = accountDao.Upsert(db, Mapper.Map<List<AccountDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        public AccountDTO GetModelByDAId(DBInfoModel Store, long DAId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
                return accountDao.Select(db, "WHERE DAId = @DAId", new { DAId = DAId }).FirstOrDefault();
        }

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<AccountSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (AccountSched_Model item in model)
                {
                    item.DAId = item.TableId ?? 0;
                    AccountDTO tmp = GetModelByDAId(Store, item.TableId ?? 0);
                    if (tmp != null)
                        item.Id = tmp.Id;
                }

                results = accountDao.DeleteOrSetIsDeletedList(db,Mapper.Map<List<AccountDTO>>(model));
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
        public int UpdateModel(DBInfoModel Store, AccountModel item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = accountDao.Update(db, Mapper.Map<AccountDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<AccountModel> item)
        {
            int results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = accountDao.UpdateList(db, Mapper.Map<List<AccountDTO>>(item));
            }
            return results;
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, AccountModel item)
        {
            long results = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                results = accountDao.Insert(db, Mapper.Map<AccountDTO>(item));
            }
            return results;
        }

        /// <summary>
        /// Get's First Account Record using Field Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public AccountModel GetAccountByType(DBInfoModel Store, Int16 Type)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "SELECT * FROM Accounts WHERE Type = " + Type.ToString();
                return Mapper.Map<AccountModel>(db.Query<AccountDTO>(SQL).FirstOrDefault());
            }
        }

        /// <summary>
        /// Returns a record with two models. EODAccountToPmsTransfer and Accounts
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        public EODAccountAndAccountModel GetEodAccountAndAccount(DBInfoModel Store, long PosInfoId, long AccountId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "SELECT etpt.Id, etpt.PosInfoId, etpt.AccountId, etpt.PmsRoom, etpt.[Description], \n"
                           + "       etpt.[Status], etpt.PmsDepartmentId, etpt.PmsDepDescription, a.Id Account_Id, \n"
                           + "       a.[Description] Account_Descr, a.[Type], a.IsDefault, a.SendsTransfer, a.IsDeleted, \n"
                           + "       a.CardOnly  \n"
                           + "FROM EODAccountToPmsTransfer AS etpt \n"
                           + "INNER JOIN Accounts AS a ON a.Id = etpt.AccountId \n"
                           + "WHERE etpt.PosInfoId = @PosInfoId AND etpt.AccountId = @AccountId";
                return db.Query<EODAccountAndAccountModel>(SQL, new { PosInfoId = PosInfoId, AccountId = AccountId }).FirstOrDefault(); 
            }
        }
    }
}
