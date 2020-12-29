using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.WebGoodysOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_OrdersFlows
    {

        /// <summary>
        /// Return the status of an Order
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        OrderStatusEnum GetStatus(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Get All Orders
        /// </summary>
        /// <returns></returns>
        List<DA_OrderModelExt> GetAllOrders(DBInfoModel dbInfoint);

        /// <summary>
        /// Get Orders By Date
        /// </summary>
        /// <returns></returns>
        List<DA_OrderModelExt> GetOrdersByDate(DBInfoModel dbInfoint, string SelectedDate);

        /// <summary>
        /// Get Customer Recent Orders
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <param name="filter">define filter for returning orders</param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη</returns>
        List<DA_OrderModel> GetOrders(DBInfoModel dbInfo, long id, int top, GetOrdersFilterEnum filter = GetOrdersFilterEnum.All);

        /// <summary>
        /// Get A Specific Order
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Order + details + ShippingAddress</returns>
        DA_ExtOrderModel GetOrderById(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Search for Orders
        /// </summary>
        /// <param name="Model">Filter Model</param>
        /// <returns>Επιστρέφει τις παραγγελίες βάση κριτηρίων</returns>
        List<DA_OrderModel> SearchOrders(DBInfoModel dbInfo, DA_SearchOrdersModel Model);

        /// <summary>
        /// Add new Order 
        /// </summary>
        ///  <param name="dbInfo">db</param>
        /// <param name="Model">order to insert</param>
        /// <param name="CustomerId">CustomerId from Auth Header</param>
        /// <returns></returns>
        long InsertOrder(DBInfoModel dbInfo, DA_OrderModel Model, long CustomerId);

        /// <summary>
        /// Mεταβάλλει το DA_orders. StatusChange και εισάγει νέα εγγραφή στον DA_OrderStatus
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        long UpdateStatus(DBInfoModel dbInfo, long Id, OrderStatusEnum Status);

        /// <summary>
        /// Get Customer Recent Remarks
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη Remarks != null</returns>
        List<DA_OrderModel> GetRemarks(DBInfoModel dbInfo, long Id, int top);

        /// <summary>
        /// Get Order Status For Specific Order by OrderId
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        List<DA_OrderStatusModel> GetOrderStatusTimeChanges(DBInfoModel dbInfo, long OrderId);

        /// <summary>
        /// Update Customer Remarks
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long UpdateRemarks(DBInfoModel dbInfo, UpdateRemarksModel Model);

        /// <summary>
        /// Update an Order (from DA or WEB only)
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="CustomerId">CustomerId from Auth Header</param>
        /// <returns></returns>
        long UpdateOrder(DBInfoModel dbInfo, DA_OrderModel Model, long CustomerId);

        /// <summary>
        /// Ακύρωση παραγγελίας από όλους εκτός από το κατάστημα. 
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        long CancelOrder(DBInfoModel dbInfo, long Id, OrderStatusEnum[] cancelStasus);

        /// <summary>
        /// Ακύρωση παραγγελίας από το κατάστημα MONO.  
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        long StoreCancelOrder(DBInfoModel dbInfo, long Id, long StoreId, OrderStatusEnum[] cancelStasus);

        /// <summary>
        /// Επιλογή Των Order Status που επιτρέπεται η ακύρωση Παραγγελίας.
        /// </summary>
        /// <returns>List of Status</returns>
        List<int> StatusForCancel(DBInfoModel dbInfo, int[] cancelStasus);

        /// <summary>
        /// Update order with external payment id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns>order id</returns>
        long UpdateExternalPayment(DBInfoModel dbInfo, ExternalPaymentModel model);

        /// <summary>
        /// Post Web Goodys Orders model from ATCOM/OMNIREST
        /// </summary>
        /// <returns></returns>
        bool PostWebGoodysOrder(DBInfoModel dbInfo, WebGoodysOrdersModel Model);

        /// <summary>
        /// Returns an order from orderno
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderNo"></param>
        /// <returns></returns>
        DA_OrderModel GetOrderByOrderNo(DBInfoModel dbInfo, long DAOrderNo);

        /// <summary>
        /// Returns a list of order status for Goodys Omnirest and a list of open orders more than 24 hours to close them
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="completedOrders"></param>
        /// <returns></returns>
        List<DA_OrdersForGoodysOmnirestStatus> GetOrdersForGoodysOmnirest(DBInfoModel dbInfo, out List<DA_OrderStatusModel> completedOrders);

        /// <summary>
        /// Converts a list of Omnirest Orders status to da_order status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        List<DA_OrderStatusModel> ConvertOmniRestStatusToDA_OrderStatus(List<DA_OrdersForGoodysOmnirestStatus> model);
    }
}
