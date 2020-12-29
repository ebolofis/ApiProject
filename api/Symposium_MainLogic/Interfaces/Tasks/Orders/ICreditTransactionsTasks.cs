using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface ICreditTransactionsTasks
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

        /// <summary>
        /// Return's a List of records for specific Ijvoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        List<CreditTransactionsModel> GetListModelByInvoiceId(DBInfoModel Store, long InvoiceId);

        /// <summary>
        /// Return's a List of Credit Transactions for Invoiceand Transaction Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesIds"></param>
        /// <param name="TransactionsIds"></param>
        /// <returns></returns>
        List<CreditTransactionsModel> GetCreditTransactionsByInvoiceIdAndTransactionId(DBInfoModel Store, List<long> InvoicesIds, List<long> TransactionsIds);
    }
}
