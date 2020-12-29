using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
   public interface IAccountsDT
    {


        /// <summary>
        /// Return the list of Active Accounts
        /// </summary>
        /// <param name="Store">Store</param>
        /// <returns>a list of AccountModel</returns>
        List<AccountModel> GetActiveModels(Models.Models.DBInfoModel Store);

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<AccountSched_Model> model);

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel Store, AccountModel item);

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModelList(DBInfoModel Store, List<AccountModel> item);

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel Store, AccountModel item);

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<AccountSched_Model> model);

        /// <summary>
        /// Get's First Account Record using Field Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        AccountModel GetAccountByType(DBInfoModel Store, Int16 Type);

        /// <summary>
        /// Returns a record with two models. EODAccountToPmsTransfer and Accounts
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        EODAccountAndAccountModel GetEodAccountAndAccount(DBInfoModel Store, long PosInfoId, long AccountId);
    }
}
