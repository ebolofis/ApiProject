using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks {
    public class InvoiceTasks : IInvoiceTasks {

        IInvoicesDT invoiceDB;  // part of DataAccess Layer

        public InvoiceTasks(IInvoicesDT invoiceDB) {
            this.invoiceDB = invoiceDB;
        }

        /// <summary>
        /// create aade qr code image, based on provided url and linked to provided invoiceid
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="InvoiceId">long</param>
        /// <param name="url">string</param>
        /// <returns>long?</returns>
        public long? CreateInvoiceQR(DBInfoModel DBInfo, long InvoiceId, string url)
        {
            return invoiceDB.CreateInvoiceQR(DBInfo, InvoiceId, url);
        }

        /// <summary>
        /// Add's new Invoice to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewInvoice(DBInfoModel Store, InvoiceModel item)
        {
            return invoiceDB.AddNewInvoice(Store, item);
        }

        /// <summary>
        /// Return's all invoices using OrderId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<InvoiceModel> GetInvoicesByOrderId(IDbConnection db, long OrderId, IDbTransaction dbTransact = null)
        {
            return invoiceDB.GetInvoicesByOrderId(db, OrderId, dbTransact);
        }

        /// <summary>
        /// Return's all invoices using OrderId without transaction
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<InvoiceModel> GetInvoicesByOrderId(DBInfoModel Store, long OrderId)
        {
            return invoiceDB.GetInvoicesByOrderId(Store, OrderId);
        }

        /// <summary>
        /// Return's all invoices using List Of Invoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        public List<InvoiceModel> GetInvoicesByIds(DBInfoModel Store, List<long> InvoicesId)
        {
            return invoiceDB.GetInvoicesByIds(Store, InvoicesId);
        }

        /// <summary>
        /// Get invoice with selected id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        public InvoiceModel GetInvoiceById(DBInfoModel Store, long InvoicesId)
        {
            return invoiceDB.GetSingleInvoice(Store, InvoicesId);
        }

        /// <summary>
        /// Update's the field IsPrinted for table Invoices and OrderDetailInvoices for Specific invoie Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="IsPrinted"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool UpdateInvoicePrinted(DBInfoModel Store, long InvoiceId, bool IsPrinted, out string Error)
        {
            return invoiceDB.UpdateInvoicePrinted(Store, InvoiceId, IsPrinted, out Error);
        }

        /// <summary>
        /// Get Invoice from Old Invoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public InvoiceWithTablesModel GetInvoiceFromOldId(DBInfoModel Store, long InvoiceId)
        {
            return invoiceDB.GetInvoiceFromOldId(Store, InvoiceId);
        }

        /// <summary>
        /// Get Invoice from Old Invoice Id using transaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public InvoiceWithTablesModel GetInvoiceFromOldId(IDbConnection db, IDbTransaction dbTransaction, long InvoiceId)
        {
            return invoiceDB.GetInvoiceFromOldId(db, dbTransaction, InvoiceId);
        }
    }
}
