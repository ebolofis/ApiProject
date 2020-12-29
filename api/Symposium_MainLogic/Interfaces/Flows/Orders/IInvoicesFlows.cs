using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{

    /// <summary>
    /// handles the invoice activities
    /// </summary>
    public interface IInvoicesFlows
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
        /// Cancels a receipt
        /// </summary>
        /// <param name="dbInfo"> Store </param>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <param name="PosInfoId"> POS </param>
        /// <param name="StaffId"> Staff </param>
        /// <returns> Data for signalR. See: <seealso cref="Symposium.Models.Models.SignalRAfterInvoiceModel"/> </returns>
        SignalRAfterInvoiceModel CancelReceipt(DBInfoModel dbInfo, long InvoiceId, long PosInfoId, long StaffId, bool CancelOrder);

        /// <summary>
        /// Cancels a receipt
        /// </summary>
        /// <param name="dbInfo"> Store </param>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <param name="PosInfoId"> POS </param>
        /// <param name="StaffId"> Staff </param>
        /// <returns> Data for signalR. See: <seealso cref="Symposium.Models.Models.SignalRAfterInvoiceModel"/> </returns>
        SignalRAfterInvoiceModel CancelReceipt(DBInfoModel dbInfo, long InvoiceId, long PosInfoId, long StaffId, bool CancelOrder, out string Error);


        /// <summary>
        /// Add's new Invoice to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewInvoice(DBInfoModel dbInfo, InvoiceModel item);

        /// <summary>
        /// Return's all invoices using OrderId
        /// </summary>
        /// <param name="dbInfo"></param>
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
        /// <param name="dbInfo"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        List<InvoiceModel> GetInvoicesByIds(DBInfoModel dbInfo, List<long> InvoicesId);

        /// <summary>
        /// Return's a list of InvoiceWithTablesModel for all Invoices associated to OrderId
        /// with all tables associated to each invoice
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        List<InvoiceWithTablesModel> GetOrderInvoices(DBInfoModel dbInfo, List<long> InvoicesId);

        /// <summary>
        /// Cancel's a ΔΠ.
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="StaffId"></param>
        /// <returns></returns>
        SignalRAfterInvoiceModel Cancel_DP_Receipt(DBInfoModel dbInfo, long InvoiceId, long PosInfoId, long StaffId, out string Error);

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
