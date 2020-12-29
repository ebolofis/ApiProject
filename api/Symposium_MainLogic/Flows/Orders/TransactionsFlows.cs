using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class TransactionsFlows : ITransactionsFlows
    {
        ITransactionsTasks task;

        public TransactionsFlows(ITransactionsTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// add's new transaction to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewTransaction(DBInfoModel dbInfo, TransactionsModel item)
        {
            return task.AddNewTransaction(dbInfo, item);
        }

        /// <summary>
        /// Return's PMS Room For TransferToPms for Cash, CC, etc....
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="AccountId"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        public PmsRoomModel GetPmsRoomForCashForTransferToPMS(DBInfoModel dbInfo, long AccountId, long PosInfoId)
        {
            return task.GetPmsRoomForCashForTransferToPMS(dbInfo, AccountId, PosInfoId);
        }
    }
}
