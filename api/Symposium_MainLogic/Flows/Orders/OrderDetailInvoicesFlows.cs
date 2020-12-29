using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
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
    class OrderDetailInvoicesFlows : IOrderDetailInvoicesFlows
    {
        IOrderDetailInvoicesTasks task;

        public OrderDetailInvoicesFlows(IOrderDetailInvoicesTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewOrderDetailInvoice(DBInfoModel dbInfo, OrderDetailInvoicesModel item)
        {
            return task.AddNewOrderDetailInvoice(dbInfo, item);
        }

        /// <summary>
        /// Return's a List Of Order Detail Invoices and 
        /// a list Of Order Detail included exrtas
        /// </summary>
        /// <param name="receiptDetails"></param>
        /// <param name="invoice"></param>
        /// <param name="orderType"></param>
        /// <param name="Abbrev"></param>
        /// <returns></returns>
        public List<OrderDetailWithExtrasModel> CreateListOfOrderDetailInvoiceFromReceipt(ICollection<ReceiptDetails> receiptDetails,
            InvoiceModel invoice, ModifyOrderDetailsEnum orderType, string Abbrev, bool isPrinted, int? Counter)
        {
            return task.CreateListOfOrderDetailInvoiceFromReceipt(receiptDetails, invoice, orderType, Abbrev, isPrinted, Counter);
        }

        /// <summary>
        /// Return's a list of orderdetails, erdetdetailingredients and orderdetailextras
        /// </summary>
        /// <param name="dbInfo"></param>
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
        public List<OrderDetailWithExtrasModel> ConvertDA_OrderDetailsToOrderDetails(DBInfoModel dbInfo, DA_OrderModel Order,
            List<DA_OrderDetails> orderDet, List<DA_OrderDetailsExtrasModel> extras,
            ModifyOrderDetailsEnum orderType, long? GuestId, bool IsDA, long? SalesType,
            long? TableId, DeliveryCustomerModel customer, long? StaffId, out string Error, IDbConnection db = null, IDbTransaction dbTransact = null)
        {
            return task.ConvertDA_OrderDetailsToOrderDetails(dbInfo, Order, orderDet, extras, orderType, GuestId, IsDA, SalesType, TableId, customer, StaffId, out Error, db, dbTransact);
        }

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
        //public List<OrderDetailWithExtrasModel> ConvertDA_OrderDetailsToOrderDetails(IDbConnection db, IDbTransaction dbTransact,
        //    DA_OrderModel Order, List<DA_OrderDetails> orderDet, List<DA_OrderDetailsExtrasModel> extras,
        //    ModifyOrderDetailsEnum orderType, long? GuestId, bool IsDA, long? SalesType,
        //    long? TableId, long? StaffId, out string Error)
        //{
        //    return task.ConvertDA_OrderDetailsToOrderDetails(db, dbTransact, Order, orderDet, extras, orderType, GuestId, IsDA, SalesType, TableId, StaffId, out Error);
        //}

        /// <summary>
        /// Gets order detail invoices of selected invoice based on InvoicesId
        /// </summary>
        /// <param name="dbInfo"> Store </param>
        /// <param name="InvoiceId"> Invoice to get order detail invoices from </param>
        /// <returns> List of order detail invoices. See: <seealso cref="Symposium.Models.Models.OrderDetailInvoicesModel"/> </returns>
        public List<OrderDetailInvoicesModel> GetOrderDetailInvoicesOfSelectedInvoice(DBInfoModel dbInfo, long InvoiceId)
        {
            return task.GetOrderDetailInvoicesOfSelectedInvoice(dbInfo, InvoiceId);
        }

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
        public OrderDetailInvoicesModel GetOrderDtailByOrderDetailId(DBInfoModel Store, long OrderDetailId, long ProductIngredId, long? InvoiceId,
            long? PosInfoId, long? PosInfoDetailId, bool IsExtra, out string Error)
        {
            return task.GetOrderDtailByOrderDetailId(Store, OrderDetailId, ProductIngredId, InvoiceId, PosInfoId, PosInfoDetailId, IsExtra, out Error);
        }
    }
}
