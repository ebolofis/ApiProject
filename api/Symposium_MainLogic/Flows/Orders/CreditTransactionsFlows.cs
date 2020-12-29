using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class CreditTransactionsFlows : ICreditTransactionsFlows
    {
        ICreditTransactionsTasks task;

        public CreditTransactionsFlows(ICreditTransactionsTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// Add new Credit Transaction to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewCreditTransaction(DBInfoModel dbInfo, CreditTransactionsModel item)
        {
            return task.AddNewCreditTransaction(dbInfo, item);
        }

        /// <summary>
        /// Add new Credit Transaction to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public long AddNewCreditTransaction(IDbConnection db, CreditTransactionsModel item, IDbTransaction dbTransact, out string Error)
        {
            return task.AddNewCreditTransaction(db, item, dbTransact, out Error);
        }
    }
}
