using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    /// <summary>
    /// Internal Main Logic class that handles Accounts
    /// </summary>
    public  class AccountTasks: IAccountTasks
    {

        IAccountsDT accountsDB;  // part of DataAccess Layer

        public AccountTasks(IAccountsDT accountsDB)
        {
            this.accountsDB = accountsDB;
        }

        /// <summary>
        /// Return the list of active accounts
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<AccountModel> GetActiveAccounts(DBInfoModel Store)
        {
          return  accountsDB.GetActiveModels(Store);
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<AccountSched_Model> model)
        {
            List<AccountSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<AccountSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

            UpsertListResultModel ups = accountsDB.InformTablesFromDAServer(Store, Upserts);
            UpsertListResultModel del = accountsDB.DeleteRecordsSendedFromDAServer(Store, Deleted);

            ups.TotalDeleted += del.TotalDeleted;
            ups.TotalFailed += del.TotalFailed;
            ups.TotalInserted += del.TotalInserted;
            ups.TotalRecords += del.TotalRecords;
            ups.TotalSucceded += del.TotalSucceded;
            ups.TotalUpdated += del.TotalUpdated;
            ups.TotalUpdated += del.TotalUpdated;
            if (ups.Results != null && ups.Results.Count > 0)
                ups.Results.Union(del.Results);
            else
                ups.Results.AddRange(del.Results);

            return ups;
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel Store, AccountModel item)
        {
            return accountsDB.UpdateModel(Store, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<AccountModel> item)
        {
            return accountsDB.UpdateModelList(Store, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, AccountModel item)
        {
            return accountsDB.InsertModel(Store, item);
        }

        /// <summary>
        /// Get's First Account Record using Field Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public AccountModel GetAccountByType(DBInfoModel Store, Int16 Type)
        {
            return accountsDB.GetAccountByType(Store, Type);
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
            return accountsDB.GetEodAccountAndAccount(Store, PosInfoId, AccountId);
        }
    }
}
