using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks {
    /// <summary>
    /// Returns all type of invoice models and collections based on flows procedural tasks
    /// </summary>
    public interface IInvoiceTasks
    {
        /// <summary>
        /// create aade qr code image, based on provided url and linked to provided invoiceid
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="InvoiceId">long</param>
        /// <param name="url">string</param>
        /// <returns>long?</returns>
        long? CreateInvoiceQR(DBInfoModel DBInfo, long InvoiceId, string url);

        /// <summary>
        /// Add's new Invoice to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewInvoice(DBInfoModel Store, InvoiceModel item);

        /// <summary>
        /// Return's all invoices using OrderId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        List<InvoiceModel> GetInvoicesByOrderId(IDbConnection db, long OrderId, IDbTransaction dbTransact = null);

        /// <summary>
        /// Return's all invoices using OrderId without transaction
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        List<InvoiceModel> GetInvoicesByOrderId(DBInfoModel Store, long OrderId);

        /// <summary>
        /// Return's all invoices using List Of Invoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        List<InvoiceModel> GetInvoicesByIds(DBInfoModel Store, List<long> InvoicesId);

        /// <summary>
        /// Get invoice with selected id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        InvoiceModel GetInvoiceById(DBInfoModel Store, long InvoicesId);

        /// <summary>
        /// Update's the field IsPrinted for table Invoices and OrderDetailInvoices for Specific invoie Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="IsPrinted"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        bool UpdateInvoicePrinted(DBInfoModel Store, long InvoiceId, bool IsPrinted, out string Error);

        /// <summary>
        /// Get Invoice from Old Invoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        InvoiceWithTablesModel GetInvoiceFromOldId(DBInfoModel Store, long InvoiceId);

        /// <summary>
        /// Get Invoice from Old Invoice Id using transaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        InvoiceWithTablesModel GetInvoiceFromOldId(IDbConnection db, IDbTransaction dbTransaction, long InvoiceId);
    }
}
