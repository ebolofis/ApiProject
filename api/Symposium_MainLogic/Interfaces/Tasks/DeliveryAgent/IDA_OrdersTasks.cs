using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.WebGoodysOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_OrdersTasks
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
        List<DA_OrderModel> GetOrders(DBInfoModel dbInfo, long id, int top, GetOrdersFilterEnum filter = GetOrdersFilterEnum.All);

        /// <summary>
        /// Get A Specific Order
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Order + details + ShippingAddress</returns>
        DA_ExtOrderModel GetOrderById(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Get A Specific Order based on ExtId1 (Efood order id). Return DA_Orders.Id. 
        /// If ExtId1 not found return 0;
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="ExtId1">Efood order id</param>
        /// <returns>Order id</returns>
         long GetOrderByExtId1(DBInfoModel dbInfo, string ExtId1);

        /// <summary>
        /// Get a specific order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="orderId"></param>
        /// <returns>Order without details</returns>
        DA_OrderModel GetSingleOrderById(DBInfoModel dbInfo, long orderId);

        /// <summary>
        /// Update payment id and possibly status of order. Insert new order status in DA_OrderStatus
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="order"></param>
        /// <param name="model"></param>
        /// <returns>order id</returns>
        long UpdatePaymentIdAndStatus(DBInfoModel dbInfo, DA_OrderModel order, ExternalPaymentModel model);

        /// <summary>
        /// Search for Orders
        /// </summary>
        /// <param name="Model">Filter Model</param>
        /// <returns>Επιστρέφει τις παραγγελίες βάση κριτηρίων</returns>
        List<DA_OrderModel> SearchOrders(DBInfoModel dbInfo, DA_SearchOrdersModel Model);

        /// <summary>
        /// Add new Order 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long InsertOrder(DBInfoModel dbInfo, DA_OrderModel Model);

        /// <summary>
        /// check if prices are correct, otherwise throw exception.
        /// For items:
        ///       Total = Price * Qnt - Discount. (Extra's Price is not included into item's Total)    
        ///       NetAmount = Total/(1+RateVat/100)     
        ///       TotalVat = Total - NetAmount  
        ///  For Extras:
        ///       NetAmount = (item.Qnt * extra.Qnt * extra.Price)/(1+RateVat/100)  
        ///       TotalVat = (item.Qnt * extra.Qnt * extra.Price) - NetAmount
        /// </summary>
        /// <param name="Model">order model</param>
        /// <returns></returns>
        void CheckPrices(DA_OrderModel Model);


        /// <summary>
        /// Check shortages for the order. If a product is in shortage then throw exception.
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Model">Order</param>
        void CheckShoratges(DBInfoModel dbInfo, DA_OrderModel Model);

        /// <summary>
        /// various validations for a DA-order  
        /// </summary>
        /// <param name="Model">order model</param>
        /// <returns></returns>
        void OrderValidations(DBInfoModel dbInfo, DA_OrderModel Model);

        /// <summary>
        /// check if store has the proper status (if not throw exception),  and set the proper value to isDelay (χρονοκαθηστέρηση)
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Model">order model</param>
        void CheckStoreAvailabilityDelay(DBInfoModel dbInfo, DA_OrderModel Model);

        /// <summary>
        /// check if the correct price-lists are chosen
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Model">order model</param>
        void CheckPricelist(DBInfoModel dbInfo, DA_OrderModel Model);

        /// <summary>
        /// Check store and polygon validity
        /// </summary>
        /// <param name="Model">DA_OrderModel</param>
        void CheckStorePolygon(DBInfoModel dbInfo, DA_OrderModel Model);

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
        /// Update an Order 
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Model">order model</param>
        /// <param name="hasChanges">true if at least one item/extras have changed</param>
        /// <returns></returns>
        long UpdateOrder(DBInfoModel dbInfo, DA_OrderModel Model, bool hasChanges);

        /// <summary>
        /// We Check if we have changes in any of the items or extras of an order
        /// </summary>
        /// <param name="Model"></param>
        /// <returns>True or False</returns>
        bool CheckOrderItemsForChanges(DBInfoModel dbInfo, DA_OrderModel Model);

        /// <summary>
        /// Ακύρωση παραγγελίας από όλους εκτός από το κατάστημα. 
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        long CancelOrder(DBInfoModel dbInfo, long Id, OrderStatusEnum[] cancelStasus, bool isSend = true);

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
        /// return the number of orders in DB for a specific store  
        /// </summary>
        /// <param name="dbInfo">connection string</param>
        /// <param name="StoreId">store id</param>
        /// <returns></returns>
        int GetStoreOrderNo(DBInfoModel dbInfo, long StoreId);

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
        /// Post Web Goodys Orders model from ATCOM/OMNIREST
        /// </summary>
        /// <returns></returns>
        bool PostWebGoodysOrder(DBInfoModel dbInfo, WebGoodysOrdersModel Model);

        /// <summary>
        ///  At least one of Store Id and Store Code should have value. Otherwise throw exception.
        /// </summary>
        /// <param name="model"></param>
        void CheckStoreIdCode(DA_OrderModel model);


        /// <summary>
        /// find Ids for Product Codes and Extras Codes
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Model">DA_OrderModel</param>
        void MatchIdFromCode(DBInfoModel dbInfo, DA_OrderModel Model);

        /// <summary>
        /// Returns an order from orderno
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderNo"></param>
        /// <returns></returns>
        DA_OrderModel GetOrderByOrderNo(DBInfoModel dbInfo, long DAOrderNo);

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

        /// <summary>
        /// Fill missing extras codes in order
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="details"></param>
        void FillExtrasCodes(DBInfoModel dbInfo, List<DA_OrderDetails> details);


        bool CheckDA_OpeningHours(DBInfoModel dbInfo,long StoreId,DateTime checkDate);

        /// <summary>
        /// Sanitizes order properties
        /// </summary>
        /// <param name="order"></param>
        void SanitizeOrder(DA_OrderModel order);
    }
}
