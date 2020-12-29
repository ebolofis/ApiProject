using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IAccountFlows
    {
        /// <summary>
        /// Return's list of Accounts after upsert, using as search field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<AccountSched_Model> model);

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel dbInfo, AccountModel item);

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModelList(DBInfoModel dbInfo, List<AccountModel> item);

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel dbInfo, AccountModel item);

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
