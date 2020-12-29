using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface ITransactionsFlows
    {
        /// <summary>
        /// add's new transaction to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewTransaction(DBInfoModel dbInfo, TransactionsModel item);

        /// <summary>
        /// Return's PMS Room For TransferToPms for Cash, CC, etc....
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="AccountId"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        PmsRoomModel GetPmsRoomForCashForTransferToPMS(DBInfoModel dbInfo, long AccountId, long PosInfoId);
    }
}
