using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IOrderTask
    {
        /// <summary>
        /// Add's new order to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewOrder(DBInfoModel Store, OrderModel item);

        /// <summary>
        /// Return's InvoiceIs using OrderId and Tables OrderDetail and OrderDetailInvoices
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        long GetInvoiceIdByOrderId(DBInfoModel Store, long OrderId);

        /// <summary>
        /// Return's all isdelayed orders to send to kintchen if EstTakeoutDate if 10 min less 
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        List<OrderModel> GetDelayedOrders(DBInfoModel Store);

        /// <summary>
        /// Delete's all records for specific Order (OrderDetail,OrderDetailIgredients,OrderDetailInvoices)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <param name="OrderId"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        bool DeleteOrderItemsForUpdate(IDbConnection db, IDbTransaction dbTransact, long OrderId, ref List<long> InvoiceId, out string Error);

        /// <summary>
        /// Return's a list of invoice ids for specific order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        List<long> GetInvoiceIdsForOrderId(DBInfoModel Store, long OrderId);

        /// <summary>
        /// Return's a Order Model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        OrderModel GetOrderById(DBInfoModel Store, long OrderId);

        /// <summary>
        /// Returns 1 for Cancel state, 0 for Delete state for an Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        int CanCancelOrDeleteState(DBInfoModel Store, long OrderId);

        /// <summary>
        /// Returns 1 for Cancel state, 0 for Delete state for an Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        int CanCancelOrDeleteState(DBInfoModel Store, long DAOrderId, ExternalSystemOrderEnum ExtType);

        /// <summary>
        /// Return's an Order with all associated tables such as OrderDetail, OrderIngredients, OrderDetailInvoices......
        /// The result is an FullOrderWithTablesModel same to Post new Order Model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        FullOrderWithTablesModel GetFullOrderModel(DBInfoModel Store, long? OrderId, long? DAOrderId, ExternalSystemOrderEnum ExtType);

        /// <summary>
        /// Check if order is already canceled
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        bool CheckIfOrderIsCanceled(DBInfoModel Store, long? OrderId, long? DAOrderId, ExternalSystemOrderEnum ExtType, out string Error);


        /// <summary>
        /// Upsert's a Full Order Model Using Transaction.
        /// Roolback if not succeeded.
        /// Inglude's all table 
        ///     Order
        ///     OrderDetail
        ///     OrderDetailIngredients
        ///     Invoices
        ///     OrderDetailInvoices
        ///     InvoiceShippingDetail
        ///     Transaction
        ///     Invoice_Guest_Trans
        ///     CreditTransaction
        ///     TransferToPms
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Model"></param>
        /// <param name="StoreOrderId"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        bool UpSertFullOrderModel(DBInfoModel Store, FullOrderWithTablesModel Model, out long StoreOrderId, out long OrderNo, out string Error, IDbConnection dbExists = null, IDbTransaction dbTransactExist = null);


        /// <summary>
        /// Return's a Order Model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        OrderModel GetOrderByDAId(DBInfoModel Store, long DAOrderId, ExternalSystemOrderEnum ExtType);

        /// <summary>
        /// Creates model for signalr messages for delivery orders (used for dine in orders at time)
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        SignalRAfterInvoiceModel CreateSignalForDeliveryOrder(DA_OrderModel order);
    }
}
