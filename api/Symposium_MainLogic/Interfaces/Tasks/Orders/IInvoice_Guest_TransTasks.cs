using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IInvoice_Guest_TransTasks
    {
        /// <summary>
        /// Add new record to Invoice Guest Trnsaction
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewInvoiceGuestTransaction(DBInfoModel Store, Invoice_Guests_TransModel item);

        /// <summary>
        /// Return's a list of invoice_guest_transactions for specific invoiceid and transactionid
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesIds"></param>
        /// <param name="TransactionsIds"></param>
        /// <returns></returns>
        List<Invoice_Guests_TransModel> GetInvoiceGuestByInvoiceIdAndTransactId(DBInfoModel Store, List<long> InvoicesIds, List<long> TransactionsIds);

        /// <summary>
        /// Delete's an Invoice_Guests_TransModel from db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int DeleteInvoiceGuestTransaction(IDbConnection db, IDbTransaction dbTransaction, Invoice_Guests_TransModel item);

        /// <summary>
        /// Return Invoice_Guests_Trans record using invoice id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        Invoice_Guests_TransModel GetInvoiceGuestByInvoiceId(DBInfoModel Store, long InvoiceId);
    }
}
