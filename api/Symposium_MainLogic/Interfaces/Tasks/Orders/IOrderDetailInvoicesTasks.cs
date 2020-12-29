using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks {
    public interface IOrderDetailInvoicesTasks
    {
        /// <summary>
        /// Return's a List Of Order Detail Invoices and 
        /// a list Of Order Detail included exrtas
        /// </summary>
        /// <param name="receiptDetails"></param>
        /// <param name="invoice"></param>
        /// <param name="orderType"></param>
        /// <param name="Abbrev"></param>
        /// <returns></returns>
        List<OrderDetailWithExtrasModel> CreateListOfOrderDetailInvoiceFromReceipt(ICollection<ReceiptDetails> receiptDetails,
            InvoiceModel invoice, ModifyOrderDetailsEnum orderType, string Abbrev, bool isPrinted, int? Counter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewOrderDetailInvoice(DBInfoModel Store, OrderDetailInvoicesModel item);

        /// <summary>
        /// Return's a list of orderdetails, erdetdetailingredients and orderdetailextras
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Order"></param>
        /// <param name="orderDet"></param>
        /// <param name="extras"></param>
        /// <param name="orderType"></param>
        /// <param name="GuestId"></param>
        /// <param name="IsDA"></param>
        /// <param name="SalesType"></param>
        /// <param name="TableId"></param>
        /// <param name="StaffId"></param>
        /// <param name="Error"></param>
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <returns></returns>
        List<OrderDetailWithExtrasModel> ConvertDA_OrderDetailsToOrderDetails(DBInfoModel Store, DA_OrderModel Order,
            List<DA_OrderDetails> orderDet, List<DA_OrderDetailsExtrasModel> extras,
            ModifyOrderDetailsEnum orderType, long? GuestId, bool IsDA, long? SalesType,
            long? TableId, DeliveryCustomerModel customer, long? StaffId, out string Error, IDbConnection db = null, IDbTransaction dbTransact = null);

        /// <summary>
        /// Gets order detail invoices of selected invoice based on InvoicesId
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice to get order detail invoices from </param>
        /// <returns> List of order detail invoices. See: <seealso cref="Symposium.Models.Models.OrderDetailInvoicesModel"/> </returns>
        List<OrderDetailInvoicesModel> GetOrderDetailInvoicesOfSelectedInvoice(DBInfoModel Store, long InvoiceId);

        /// <summary>
        /// Determines table tabel of order according to customer's email and already inserted table labels
        /// </summary>
        /// <param name="tableLabels"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        string DetermineTableLabel(long? tableId, List<string> tableLabels, DeliveryCustomerModel customer);

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

        ///// <summary>
        ///// Return's a list of orderdetails, erdetdetailingredients and orderdetailextras
        ///// Uses Transaction
        ///// </summary>
        ///// <param name="db"></param>
        ///// <param name="dbTransact"></param>
        ///// <param name="Order"></param>
        ///// <param name="orderDet"></param>
        ///// <param name="extras"></param>
        ///// <param name="orderType"></param>
        ///// <param name="GuestId"></param>
        ///// <param name="IsDA"></param>
        ///// <param name="SalesType"></param>
        ///// <param name="TableId"></param>
        ///// <param name="StaffId"></param>
        ///// <param name="Error"></param>
        ///// <returns></returns>
        //List<OrderDetailWithExtrasModel> ConvertDA_OrderDetailsToOrderDetails(IDbConnection db, IDbTransaction dbTransact,
        //    DA_OrderModel Order, List<DA_OrderDetails> orderDet, List<DA_OrderDetailsExtrasModel> extras,
        //    ModifyOrderDetailsEnum orderType, long? GuestId, bool IsDA, long? SalesType,
        //    long? TableId, long? StaffId, out string Error);
    }
}
