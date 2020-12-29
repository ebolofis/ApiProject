using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT {

    /// <summary>
    /// Class that handles data related to Invoices
    /// </summary>
    public interface IInvoicesDT
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
        /// Returns invoice with selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice </param>
        /// <returns> Invoice model. See: <seealso cref="Symposium.Models.Models.InvoiceModel"/> </returns>
        InvoiceModel GetSingleInvoice(DBInfoModel Store, long InvoiceId);

        /// <summary>
        /// Cancels receipt
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <param name="PosInfoId"> Pos </param>
        /// <param name="StaffId"> Staff </param>
        /// <param name="newInvoiceId"> New Invoice </param>
        /// <returns> Id of new invoice inserted </returns>
        int cancelReceipt(DBInfoModel Store, long InvoiceId, long PosInfoId, long StaffId, bool CancelOrder, out long NewInvoiceId);

        long GetTicketCount(DBInfoModel store, long posInfo);

        /// <summary>
        /// Return's Inovice model using HasCode
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="HashCode"></param>
        /// <returns></returns>
        InvoiceModel GetInvoiceByHashCode(DBInfoModel Store, string HashCode);

        /// <summary>
        /// Add's new Invoice to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewInvoice(DBInfoModel Store, InvoiceModel item);

        /// <summary>
        /// Add's new Invoice to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        long AddNewInvoice(IDbConnection db, InvoiceModel item, IDbTransaction dbTransact, out string Error);

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
        /// Update's the field IsPrinted for table Invoices and OrderDetailInvoices for Specific invoie Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoieId"></param>
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

        InvoiceModel GetSingleInvoice(IDbConnection db, IDbTransaction dbTransact, long InvoiceId);
    }
}
