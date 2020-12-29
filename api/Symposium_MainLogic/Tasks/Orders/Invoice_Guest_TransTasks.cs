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
    public class Invoice_Guest_TransTasks : IInvoice_Guest_TransTasks
    {
        IInvoice_Guests_TransDT dt;

        public Invoice_Guest_TransTasks(IInvoice_Guests_TransDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Add new record to Invoice Guest Trnsaction
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewInvoiceGuestTransaction(DBInfoModel Store, Invoice_Guests_TransModel item)
        {
            return dt.AddNewInvoiceGuestTransaction(Store, item);
        }

        /// <summary>
        /// Return's a list of invoice_guest_transactions for specific invoiceid and transactionid
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesIds"></param>
        /// <param name="TransactionsIds"></param>
        /// <returns></returns>
        public List<Invoice_Guests_TransModel> GetInvoiceGuestByInvoiceIdAndTransactId(DBInfoModel Store, List<long> InvoicesIds, List<long> TransactionsIds)
        {
            List<Invoice_Guests_TransModel> invGuest = new List<Invoice_Guests_TransModel>();
            foreach (long item in InvoicesIds)
            {
                Invoice_Guests_TransModel model = dt.GetInvoiceGuestByInvoiceId(Store, item);
                var fld = TransactionsIds.Find(f => f == model.TransactionId);
                if (fld != null && fld > 0)
                    invGuest.Add(model);
            }
            return invGuest;
        }

        /// <summary>
        /// Delete's an Invoice_Guests_TransModel from db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int DeleteInvoiceGuestTransaction(IDbConnection db, IDbTransaction dbTransaction, Invoice_Guests_TransModel item)
        {
            return dt.DeleteInvoiceGuestTransaction(db, dbTransaction, item);
        }

        /// <summary>
        /// Return Invoice_Guests_Trans record using invoice id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public Invoice_Guests_TransModel GetInvoiceGuestByInvoiceId(DBInfoModel Store, long InvoiceId)
        {
            return dt.GetInvoiceGuestByInvoiceId(Store, InvoiceId);
        }
    }
}
