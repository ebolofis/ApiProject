using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNet.SignalR;
using Symposium.Models.Models;
using Symposium.Models.Models.Delivery;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.Delivery
{
    public interface IDeliveryRoutingTasks
    {
        /// <summary>
        /// task that searches for unassigned routes after api restart and attempts auto assignment
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="hub">IHubContext</param>
        void restartAsyncAssignRouteToStaff(DBInfoModel DBInfo, IHubContext hub = null);

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
        /// change route assign status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="assignStatus">int</param>
        /// <returns>bool</returns>
        bool updateRouteAssignStatus(DBInfoModel DBInfo, long deliveryRoutingId, int assignStatus);

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
        /// release staff from route, set isonroad = 0, isassignedtoroute = 0, datetime = now
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staffId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        bool releaseStaff(DBInfoModel DBInfo, long staffId, IDbConnection db = null, IDbTransaction transact = null);

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
        /// check if staff id is required and provided
        /// </summary>
        /// <param name="staffId">long?</param>
        /// <returns>bool</returns>
        bool checkValidStaffId(long? staffId);

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
        long? upsertRouteOrders(DBInfoModel DBInfo, long? deliveryRoutingId, int orderIdCount, IDbConnection db, IDbTransaction transact, out List<long> deletedDeliveryRouteIds, int? status = null, long? staffId = null);

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
        /// get appropriate route status on creation depending on configuration and staff provided
        /// </summary>
        /// <param name="staffId">long?</param>
        /// <returns>int</returns>
        int getAppropriateStatus(long? staffId);

        /// <summary>
        /// get routes with specified status assigned to provided staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="assignStatus">int</param>
        /// <param name="staffId">long</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        List<DeliveryRoutingModel> getStaffRoutesByAssignStatus(DBInfoModel DBInfo, int assignStatus, long staffId);

        /// <summary>
        /// get route and associated orders data
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>DeliveryRoutingOrdersModel</returns>
        DeliveryRoutingOrdersModel getRouteOrders(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db, IDbTransaction transact);

        /// <summary>
        /// get specified route list of order ids
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>List<long></returns>
        List<long> getRouteOrderIds(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db = null, IDbTransaction transact = null);

        List<DeliveryRoutingLastRouteInfoDetails> getRouteOrdersDetails(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db = null, IDbTransaction transact = null);

        /// <summary>
        /// return last route for specified staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <returns>DeliveryRoutingExtModel</returns>
        List<DeliveryRoutingLastRouteInfo> getLastStaffRoute(DBInfoModel DBInfo, IDbConnection db, string staff);

        /// <summary>
        /// task to move delivery routing records to hist table at end of day
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        void moveDeliveryRoutingToHist(DBInfoModel DBInfo);

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

        bool sendCancelRouteToExternalApp(DBInfoModel DBInfo, long deliveryRoutingId, long? staffId, IDbConnection db = null, IDbTransaction transact = null);

        /// <summary>
        /// delivery routing staff login
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="login">DeliveryRoutingStaffCredentialsModel</param>
        /// <returns>long?</returns>
        long? staffLogin(DBInfoModel DBInfo, DeliveryRoutingStaffCredentialsModel login);

        /// <summary>
        /// attempt to auto assign route to staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="returnId">long</param>
        /// <param name="hub">IHubContext</param>
        void asyncAssignRouteToStaff(DBInfoModel DBInfo, long returnId, IHubContext hub);
        long? getOrderRouteId(DBInfoModel DBInfo, long orderid);
    }
}
