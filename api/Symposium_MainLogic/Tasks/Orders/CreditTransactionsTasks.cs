using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class CreditTransactionsTasks : ICreditTransactionsTasks
    {
        ICreditTransactionDT dt;

        public CreditTransactionsTasks(ICreditTransactionDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Add new Credit Transaction to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewCreditTransaction(DBInfoModel Store, CreditTransactionsModel item)
        {
            return dt.AddNewCreditTransaction(Store, item);
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
            return dt.AddNewCreditTransaction(db, item, dbTransact, out Error);
        }

        /// <summary>
        /// Return's a List of Credit Transactions for Invoiceand Transaction Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesIds"></param>
        /// <param name="TransactionsIds"></param>
        /// <returns></returns>
        public List<CreditTransactionsModel> GetCreditTransactionsByInvoiceIdAndTransactionId(DBInfoModel Store, List<long> InvoicesIds, List<long> TransactionsIds)
        {
            List<CreditTransactionsModel> ret = new List<CreditTransactionsModel>();
            foreach (long item in InvoicesIds)
            {
                List<CreditTransactionsModel> tmp = dt.GetListModelByInvoiceId(Store, item);
                foreach (CreditTransactionsModel crTr in tmp)
                {
                    var fld = TransactionsIds.Find(f => f == crTr.TransactionId);
                    if (fld != null && fld > 0)
                        ret.Add(crTr);
                }
            }

            return ret;
        }

        /// <summary>
        /// Return's a List of records for specific Ijvoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public List<CreditTransactionsModel> GetListModelByInvoiceId(DBInfoModel Store, long InvoiceId)
        {
            return dt.GetListModelByInvoiceId(Store, InvoiceId);
        }
    }
}
