using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT {

    /// <summary>
    /// Class that handles data related to Order Detail Invoices
    /// </summary>
    public interface IOrderDetailInvoicesDT {

        /// <summary>
        /// Gets order detail invoices of selected invoice based on InvoicesId
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice to get order detail invoices from </param>
        /// <returns> List of order detail invoices. See: <seealso cref="Symposium.Models.Models.OrderDetailInvoicesModel"/> </returns>
        List<OrderDetailInvoicesModel> GetOrderDetailInvoicesOfSelectedInvoice(DBInfoModel Store, long InvoiceId);

        /// <summary>
        /// Add's new record to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewOrderDetailInvoice(DBInfoModel Store, OrderDetailInvoicesModel item);

        /// <summary>
        /// Add's new record to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        long AddNewOrderDetailInvoice(IDbConnection db, OrderDetailInvoicesModel item, IDbTransaction dbTransact, out string Error);

        /// <summary>
        /// Return's a record from OrderDetailInvoices using Parameters can use
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderDetailId"></param>
        /// <param name="ProductIngredId"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="IsExtra"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        OrderDetailInvoicesModel GetOrderDtailByOrderDetailId(DBInfoModel Store, long OrderDetailId, long ProductIngredId, long? InvoiceId,
            long? PosInfoId, long? PosInfoDetailId, bool IsExtra, out string Error);

        /// <summary>
        ///  Gets table labels of selected table for active orders
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="TableId"></param>
        /// <returns></returns>
        List<string> GetTableLabelsInTable(DBInfoModel Store, long? TableId);

    }
}
