using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Symposium.Models.Models.Orders;
using Symposium.WebApi.MainLogic.Flows;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IOrderFlows
    {
        List<ResultsAfterDA_OrderActionsModel> InsertDeliveryOrders(DBInfoModel dbInfo, OrderFromDAToClientForWebCallModel model, 
            string StatusCanCanceld, bool CreateDP, out SignalRAfterInvoiceModel signal);

        // <summary>
        /// Check Client Store Status and Order Type Status.
        /// Return true if Store can accept order type status
        /// </summary>
        /// <param name="StoreStatus"></param>
        /// <param name="OrderType"></param>
        /// <returns></returns>
        bool CheckClientStoreOrderStatus(DBInfoModel dbInfo, long StoreId, OrderTypeStatus OrderType);

        /// <summary>
        /// Add's new order to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewOrder(DBInfoModel dbInfo, OrderModel item);

        /// <summary>
        /// Insert an order to db with all 
        /// association tables such as OrderDetail, invoices, ....
        /// Return's a model with result
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="order"></param>
        /// <param name="PrinterType"></param>
        /// <param name="ExtEcrName"></param>
        /// <param name="DA_OrderId"></param>
        /// <param name="OrderStatus"></param>
        /// <param name="dbExists"></param>
        /// <param name="dbTransactExist"></param>
        /// <returns></returns>
        ResultsAfterDA_OrderActionsModel UpsertNewOrder(DBInfoModel dbInfo, FullOrderWithTablesModel order, PrintTypeEnum PrinterType,
            string ExtEcrName, long? DA_OrderId = 0, OrderStatusEnum OrderStatus = 0, IDbConnection dbExists = null, IDbTransaction dbTransactExist = null);

        /// <summary>
        /// Return's InvoiceIs using OrderId and Tables OrderDetail and OrderDetailInvoices
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        long GetInvoiceIdByOrderId(DBInfoModel dbInfo, long OrderId);

        /// <summary>
        /// Return's all is delayed orders to send to kitchen if EstTakeoutDate if 10 min less 
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        List<OrderModel> GetDelayedOrders(DBInfoModel dbInfo);

        /// <summary>
        /// Return's a Order Model using Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        OrderModel GetOrderById(DBInfoModel dbInfo, long OrderId);

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
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        List<long> GetInvoiceIdsForOrderId(DBInfoModel dbInfo, long OrderId);

        /// <summary>
        /// Returns 1 for Cancel state, 0 for Delete state for an Order Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        int CanCancelOrDeleteState(DBInfoModel dbInfo, long OrderId);

        /// <summary>
        /// Returns 1 for Cancel state, 0 for Delete state for an Order Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        int CanCancelOrDeleteState(DBInfoModel dbInfo, long DAOrderId, ExternalSystemOrderEnum ExtType);

        /// <summary>
        /// Return's an Order with all associated tables such as OrderDetail, OrderIngredients, OrderDetailInvoices......
        /// The result is an FullOrderWithTablesModel same to Post new Order Model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        FullOrderWithTablesModel GetFullOrderModel(DBInfoModel dbInfo, long? OrderId, long? DAOrderId, ExternalSystemOrderEnum ExtType);

        /// <summary>
        /// Invoke plugin for card payment
        /// </summary>
        /// <param name="OrderCardPayment"></param>
        /// <param name=""></param>
        /// <returns></returns>
        bool OrderCardPayment(PluginCardPaymentModel model, DBInfoModel dbInfo);

        /// <summary>
        /// Return's a transfer mapping model based on product category, pos department and pricelist id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosDepartmentId"></param>
        /// <param name="ProductCategoryId"></param>
        /// <param name="PriceListId"></param>
        /// <param name="HotelId"></param>
        /// <returns></returns>
        TransferMappingsModel GetTransferMappingForNewTransaction(DBInfoModel Store, long PosDepartmentId, long ProductCategoryId, long PriceListId, int HotelId);

        /// <summary>
        /// Update OrderDetail set status for a specific Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        bool SetStatusToOrderDetails(DBInfoModel Store, long OrderId, OrderStatusEnum Status);
    }
}
