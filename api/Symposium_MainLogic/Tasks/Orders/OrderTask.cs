using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
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
    public class OrderTask : IOrderTask
    {
        IOrderDT dt;

        public OrderTask(IOrderDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Add's new order to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewOrder(DBInfoModel Store, OrderModel item)
        {
            return dt.AddNewOrder(Store, item);
        }

        /// <summary>
        /// Return's InvoiceIs using OrderId and Tables OrderDetail and OrderDetailInvoices
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public long GetInvoiceIdByOrderId(DBInfoModel Store, long OrderId)
        {
            return dt.GetInvoiceIdByOrderId(Store, OrderId);
        }

        /// <summary>
        /// Return's all isdelayed orders to send to kintchen if EstTakeoutDate if 10 min less 
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<OrderModel> GetDelayedOrders(DBInfoModel Store)
        {
            return dt.GetDelayedOrders(Store);
        }

        /// <summary>
        /// Return's a Order Model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public OrderModel GetOrderById(DBInfoModel Store, long OrderId)
        {
            return dt.GetOrderById(Store, OrderId);
        }

        /// <summary>
        /// Delete's all records for specific Order (OrderDetail,OrderDetailIgredients,OrderDetailInvoices)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <param name="OrderId"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool DeleteOrderItemsForUpdate(IDbConnection db, IDbTransaction dbTransact, long OrderId, ref List<long> InvoiceId, out string Error)
        {
            return dt.DeleteOrderItemsForUpdate(db, dbTransact, OrderId, ref InvoiceId, out Error);
        }

        /// <summary>
        /// Return's a list of invoice ids for specific order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<long> GetInvoiceIdsForOrderId(DBInfoModel Store, long OrderId)
        {
            return dt.GetInvoiceIdsForOrderId(Store, OrderId);
        }

        /// <summary>
        /// Returns 1 for Cancel state, 0 for Delete state for an Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public int CanCancelOrDeleteState(DBInfoModel Store, long OrderId)
        {
            return dt.CanCancelOrDeleteState(Store, OrderId);
        }

        /// <summary>
        /// Returns 1 for Cancel state, 0 for Delete state for an Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public int CanCancelOrDeleteState(DBInfoModel Store, long DAOrderId, ExternalSystemOrderEnum ExtType)
        {
            return dt.CanCancelOrDeleteState(Store, DAOrderId, ExtType);
        }

        /// <summary>
        /// Return's an Order with all associated tables such as OrderDetail, OrderIngredients, OrderDetailInvoices......
        /// The result is an FullOrderWithTablesModel same to Post new Order Model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public FullOrderWithTablesModel GetFullOrderModel(DBInfoModel Store, long? OrderId, long? DAOrderId, ExternalSystemOrderEnum ExtType)
        {
            return dt.GetFullOrderModel(Store, OrderId, DAOrderId, ExtType);
        }

        /// <summary>
        /// Check if order is already canceled
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool CheckIfOrderIsCanceled(DBInfoModel Store, long? OrderId, long? DAOrderId, ExternalSystemOrderEnum ExtType, out string Error)
        {
            return dt.CheckIfOrderIsCanceled(Store, OrderId, DAOrderId, ExtType, out Error);
        }

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
        public bool UpSertFullOrderModel(DBInfoModel Store, FullOrderWithTablesModel Model, out long StoreOrderId, out long OrderNo, 
            out string Error, IDbConnection dbExists = null, IDbTransaction dbTransactExist = null)
        {
            return dt.UpSertFullOrderModel(Store, Model, out StoreOrderId, out OrderNo, out Error, dbExists, dbTransactExist);
        }

        /// <summary>
        /// Return's a Order Model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public OrderModel GetOrderByDAId(DBInfoModel Store, long DAOrderId, ExternalSystemOrderEnum ExtType)
        {
            return dt.GetOrderByDAId(Store, DAOrderId, ExtType);
        }

        /// <summary>
        /// Creates model for signalr messages for delivery orders (used for dine in orders at time)
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public SignalRAfterInvoiceModel CreateSignalForDeliveryOrder(DA_OrderModel order)
        {
            SignalRAfterInvoiceModel signal = new SignalRAfterInvoiceModel();
            signal.TableId = order.TableId;
            // Add more info in case someone else wants to use it...
            return signal;
        }
    }
}
