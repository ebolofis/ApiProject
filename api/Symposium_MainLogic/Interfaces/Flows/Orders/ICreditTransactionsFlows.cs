using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface ICreditTransactionsFlows
    {
        /// <summary>
        /// Add new Credit Transaction to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewCreditTransaction(DBInfoModel Store, CreditTransactionsModel item);

        /// <summary>
        /// Add new Credit Transaction to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        long AddNewCreditTransaction(IDbConnection db, CreditTransactionsModel item, IDbTransaction dbTransact, out string Error);
    }
}
