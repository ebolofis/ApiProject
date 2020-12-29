using System;
using System.Collections.Generic;
using System.Data;
using Symposium.Models.Models;
using Symposium.Models.Models.Delivery;
using Symposium_DTOs.PosModel_Info;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Delivery
{
    public interface IDeliveryRoutingDT
    {
        /// <summary>
        /// assign orders to route. create route if not provided, update existing
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderIds">List<long></param>
        /// <param name="deliveryRoutingId"> long?</param>
        /// <param name="modifiedDeliveryRouteIds">out List<long></param>
        /// <param name="deletedDeliveryRouteIds">out List<long></param>
        /// <param name="assignDeliveryRouteIds">out List<long></param>
        /// <returns>bool</returns>
        bool addOrdersToRoute(DBInfoModel DBInfo, List<long> orderIds, long? deliveryRoutingId, out List<long> modifiedDeliveryRouteIds, out List<long> deletedDeliveryRouteIds, out List<long> assignDeliveryRouteIds);

        bool updateRoute(DBInfoModel DBInfo, DeliveryRoutingModel route, IDbConnection db = null, IDbTransaction transact = null);

        /// <summary>
        /// unassign orders from route
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderIds">List<long></param>
        /// <param name="deliveryRoutingId">long?</param>
        /// <param name="modifiedDeliveryRouteIds">out List<long></param>
        /// <param name="deletedDeliveryRouteIds">out List<long></param>
        /// <param name="assignDeliveryRouteIds">out List<long></param>
        /// <returns>bool</returns>
        bool removeOrdersFromRoute(DBInfoModel DBInfo, List<long> orderIds, long? deliveryRoutingId, out List<long> modifiedDeliveryRouteIds, out List<long> deletedDeliveryRouteIds, out List<long> assignDeliveryRouteIds);

        /// <summary>
        /// get all routes if parameters null, get specific route if id provided, get routes of specific status if status provided
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long?</param>
        /// <param name="status">int?</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        List<DeliveryRoutingModel> getRoutes(DBInfoModel DBInfo, long? deliveryRoutingId = null, int? status = null, IDbConnection db = null, IDbTransaction transact = null);

        /// <summary>
        /// get routes by ids provided
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="routeIds">List<long></param>
        /// <returns>List<DeliveryRoutingModel></returns>
        List<DeliveryRoutingModel> getRoutes(DBInfoModel DBInfo, List<long> routeIds);

        /// <summary>
        /// get route and associated orders data
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>DeliveryRoutingOrdersModel</returns>
        DeliveryRoutingOrdersModel getRouteOrders(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db = null, IDbTransaction transact = null);

        /// <summary>
        /// delete specific route
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        bool deleteRoute(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db = null, IDbTransaction transact = null);

        /// <summary>
        /// change route, and associated orders, status to provided status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="status">int</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <param name="assignStatus">int?</param>
        /// <param name="complete">bool?</param>
        /// <returns>bool</returns>
        bool updateRouteStatus(DBInfoModel DBInfo, long deliveryRoutingId, int status, IDbConnection db, IDbTransaction transact, int? assignStatus = null, bool? complete = false);
        
        /// <summary>
        /// update staff record, isAssignedToRoute field with provided value
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staffId">long</param>
        /// <param name="assigned">bool</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        bool updateStaffIsAssignedToRoute(DBInfoModel DBInfo, long staffId, bool assigned, IDbConnection db = null, IDbTransaction transact = null, int? routeStatus = null);

        /// <summary>
        /// get selected keys from orders.extobj json string
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderId">long?</param>
        /// <param name="keys"List<string>></param>
        /// <returns>Dictionary<string, string></returns>
        Dictionary<string, string> GetOrderExtElements(DBInfoModel DBInfo, long orderId, List<string> keys);

        /// <summary>
        /// modify specific route assign status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="assignStatus">int</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        bool updateRouteAssignStatus(DBInfoModel DBInfo, long deliveryRoutingId, int assignStatus, IDbConnection db = null, IDbTransaction transact = null);

        /// <summary>
        /// release staff from route, set isonroad = 0, isassignedtoroute = 0, datetime = now
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staffId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        bool releaseStaff(DBInfoModel DBInfo, long staffId, IDbConnection db = null, IDbTransaction transact = null);

        /// <summary>
        /// modify provided orders status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderIds">List<long></param>
        /// <param name="status">int</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        bool updateOrdersStatus(DBInfoModel DBInfo, List<long> orderIds, int status, IDbConnection db = null, IDbTransaction transact = null);

        /// <summary>
        /// update routing id reference to provided orders
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderIds">List<long></param>
        /// <param name="deliveryRoutingId">long?</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <param name="modifiedDeliveryRouteIds">out List<long></param>
        /// <returns>bool</returns>
        bool updateOrdersDeliveryRoutingId(DBInfoModel DBInfo, List<long> orderIds, long? deliveryRoutingId, IDbConnection db, IDbTransaction transact, out List<long> modifiedDeliveryRouteIds);

        /// <summary>
        /// send data to external api. DeliveryRoutingChangeStaffModel when route staff is modified, Routing3Model when route is created
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="drExternalApiUrl">string</param>
        /// <param name="modifiedDeliveryRouteId">out long?</param>
        /// <param name="modifiedDeliveryRouteStatus">long?</param>
        /// <param name="oldStaffId">long?</param>
        void sendRouteToExternalApi(DBInfoModel DBInfo, long deliveryRoutingId, string drExternalApiUrl, out long? modifiedDeliveryRouteId, out long? modifiedDeliveryRouteStatus, long? oldStaffId = null);

        /// <summary>
        /// check provided orders for appropriate status when creating route or modifing staff 
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderIds">List<long></param>
        /// <returns>bool</returns>
        bool checkValidOrderStatus(DBInfoModel DBInfo, List<long> orderIds);

        /// <summary>
        /// get routes with specified status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="assignStatus">int</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        List<DeliveryRoutingModel> getRoutesByAssignStatus(DBInfoModel DBInfo, int assignStatus);

        /// <summary>
        /// get routes with specified status assigned to provided staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="assignStatus">int</param>
        /// <param name="staffId">long</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        List<DeliveryRoutingModel> getStaffRoutesByAssignStatus(DBInfoModel DBInfo, int assignStatus, long staffId);

        /// <summary>
        /// get route by id provided
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <returns>DeliveryRoutingModel</returns>
        DeliveryRoutingModel getRoute(DBInfoModel DBInfo, long deliveryRoutingId);

        /// <summary>
        /// create route or update if exists
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long?</param>
        /// <param name="orderIdCount">int</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <param name="deletedDeliveryRouteIds">out List<long></param>
        /// <param name="status">int?</param>
        /// <param name="staffId">long?</param>
        /// <returns>long?</returns>
        long? upsertRouteOrders(DBInfoModel DBInfo, long? deliveryRoutingId, int orderOffset, IDbConnection db, IDbTransaction transact, out List<long> deletedDeliveryRouteIds, int? status = null, long? staffId = null);

        /// <summary>
        /// change staff assigned to route
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="staffId">long?</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <param name="staffUsername">long?</param>
        /// <returns>bool</returns>
        bool updateRouteStaff(DBInfoModel DBInfo, long deliveryRoutingId, long? staffId = null, IDbConnection db = null, IDbTransaction transact = null, long? staffUsername = null);

        /// <summary>
        /// add staff name to delivery routing rejected field after reject
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="staffId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        bool addStaffToRejected(DBInfoModel DBInfo, long deliveryRoutingId, long staffId, IDbConnection db = null, IDbTransaction transact = null);

        /// <summary>
        /// task to move delivery routing records to hist table at end of day
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        void moveDeliveryRoutingToHist(DBInfoModel DBInfo);

        /// <summary>
        /// return last route for specified staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <returns>DeliveryRoutingExtModel</returns>
        List<DeliveryRoutingLastRouteInfo> getLastStaffRoute(IDbConnection db, string staff);

        long? getOrderRouteId(DBInfoModel DBInfo, long orderid);

        InvoiceShippingDetailsExtModel GetOrderCustomer(DBInfoModel DBInfo, long deliveryRoutingId, long orderId, IDbConnection db);

        /// <summary>
        /// add staff to active staffs list at the position specified
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <param name="staffId">long?</param>
        /// <param name="staffUsername">string</param>
        /// <param name="position">string</param>
        /// <returns>bool</returns>
        bool addStaffToActiveStaffList(DBInfoModel DBInfo, IDbConnection db, IDbTransaction transact, long? staffId = null, string staffUsername = null, string position = "last");

        /// <summary>
        /// get list of active staff from database
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <returns>List<StaffDeliveryModel></returns>
        List<StaffDeliveryModel> getActiveStaffList(DBInfoModel DBInfo);

        /// <summary>
        /// delivery routing staff login
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="login">DeliveryRoutingStaffCredentialsModel</param>
        /// <returns>long?</returns>
        long? staffLogin(DBInfoModel DBInfo, DeliveryRoutingStaffCredentialsModel login);
    }
}
