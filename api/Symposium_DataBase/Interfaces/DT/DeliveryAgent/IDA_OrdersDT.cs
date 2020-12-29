using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_OrdersDT
    {

        /// <summary>
        /// Return the status of an Order
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
         OrderStatusEnum GetStatus(DBInfoModel Store, long Id);

        /// <summary>
        /// Get All Orders
        /// </summary>
        /// <returns></returns>
        List<DA_OrderModelExt> GetAllOrders(DBInfoModel dbInfo);

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
        List<DA_OrderModel> GetOrders(DBInfoModel Store, long id, int top, GetOrdersFilterEnum filter = GetOrdersFilterEnum.All);

        /// <summary>
        /// Get A Specific Order based on ExtId1 (Efood order id). Return DA_Orders.Id. 
        /// If ExtId1 not found return 0;
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="ExtId1">Efood order id</param>
        /// <returns>Order id</returns>
         long GetOrderByExtId1(DBInfoModel Store, string ExtId1);

        /// <summary>
        /// Get A Specific Order
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Order + details + ShippingAddress</returns>
        DA_ExtOrderModel GetOrderById(DBInfoModel Store, long Id);

        /// <summary>
        /// Get A Specific Order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="orderId"></param>
        /// <returns>Order without details</returns>
        DA_OrderModel GetSingleOrderById(DBInfoModel Store, long orderId);

        /// <summary>
        /// Get a specific order with details
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="orderId"></param>
        /// <returns>Order with details</returns>
        DA_OrderModel GetOrderWithDetailsById(DBInfoModel Store, long orderId);

        /// <summary>
        /// Update A Specific Order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="order"></param>
        /// <returns>oder id</returns>
        long UpdateSingleOrder(DBInfoModel Store, DA_OrderModel order);

        /// <summary>
        /// Search for Orders
        /// </summary>
        /// <param name="Model">Filter Model</param>
        /// <returns>Επιστρέφει τις παραγγελίες βάση κριτηρίων</returns>
        List<DA_OrderModel> SearchOrders(DBInfoModel Store, DA_SearchOrdersModel Model);

        /// <summary>
        /// Add new Order 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long InsertOrder(DBInfoModel Store, DA_OrderModel Model);

        /// <summary>
        /// Mεταβάλλει το DA_orders. StatusChange και εισάγει νέα εγγραφή στον DA_OrderStatus
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        long UpdateStatus(DBInfoModel Store, long Id, OrderStatusEnum Status);

        /// <summary>
        /// Get Customer Recent Remarks
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη Remarks != null</returns>
        List<DA_OrderModel> GetRemarks(DBInfoModel Store, long Id, int top);

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
        long UpdateRemarks(DBInfoModel Store, UpdateRemarksModel Model);

        /// <summary>
        /// Update an Order 
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="HasChanges"></param>
        /// <returns></returns>
        long UpdateOrder(DBInfoModel Store, DA_OrderModel Model, bool HasChanges);

        /// <summary>
        /// Ακύρωση παραγγελίας από όλους εκτός από το κατάστημα. 
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        long CancelOrder(DBInfoModel Store, long Id, OrderStatusEnum[] cancelStasus, bool isSend = true);

        /// <summary>
        /// Ακύρωση παραγγελίας από το κατάστημα MONO.  
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        long StoreCancelOrder(DBInfoModel Store, long Id, long StoreId, OrderStatusEnum[] cancelStasus);

        /// <summary>
        /// return the number of orders in DB for a specific store  
        /// </summary>
        /// <param name="Store">connection string</param>
        /// <param name="StoreId">store id</param>
        /// <returns></returns>
        int GetStoreOrderNo(DBInfoModel Store, long StoreId);

        /// <summary>
        /// Delete's an DA_Order Record
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ErrorMess"></param>
        /// <returns></returns>
        bool DeleteOrders(DBInfoModel Store, long DAOrderId, out string ErrorMess);

        /// <summary>
        /// Update DAOrders Set Error Message
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ErrorMess"></param>
        void SetErrorMessageToDAOrder(DBInfoModel Store, long DAOrderId, string ErrorMess);

        /// <summary>
        /// Returns an order from orderno
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderNo"></param>
        /// <returns></returns>
        DA_OrderModel GetOrderByOrderNo(DBInfoModel Store, long DAOrderNo);

        /// <summary>
        /// Update's ExtId1 with Omnirest External system OrderNo 
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        bool UpdateDA_OrderExtId1(DBInfoModel dbInfo, long DA_OrderId, long OrderNo);

        /// <summary>
        /// Returns a list of ordera and statuses for Goodys Omnirest
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        List<DA_OrdersForGoodysOmnirestStatus> GetOrdersForGoodysOmnirest(DBInfoModel dbInfo, out string error);

        bool CheckDA_OpeningHours(DBInfoModel dbInfo, long StoreId, DateTime checkDate);
    }
}
