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
    public class Invoice_Guest_TransFlows : IInvoice_Guest_TransFlows
    {
        IInvoice_Guest_TransTasks task;

        public Invoice_Guest_TransFlows(IInvoice_Guest_TransTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// Add new record to Invoice Guest Trnsaction
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewInvoiceGuestTransaction(DBInfoModel dbInfo, Invoice_Guests_TransModel item)
        {
            return task.AddNewInvoiceGuestTransaction(dbInfo, item);
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
            return task.DeleteInvoiceGuestTransaction(db, dbTransaction, item);
        }

        /// <summary>
        /// Return Invoice_Guests_Trans record using invoice id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public Invoice_Guests_TransModel GetInvoiceGuestByInvoiceId(DBInfoModel Store, long InvoiceId)
        {
            return task.GetInvoiceGuestByInvoiceId(Store, InvoiceId);
        }
    }
}
