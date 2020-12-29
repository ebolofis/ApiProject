using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNet.SignalR;
using Symposium.Models.Models;
using Symposium.Models.Models.Delivery;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.Delivery
{
    public interface IDeliveryRoutingFlows
    {
        /// <summary>
        /// called from pos to create new delivery route with specific orders assigned and optional staff assignment
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="data">DeliveryRoutingNewModel</param>
        /// <param name="modifiedDeliveryRouteIds">out List<long></param>
        /// <param name="deletedDeliveryRouteIds">out List<long></param>
        /// <param name="hub">IHubContext</param>
        /// <returns>long?</returns>
        long? createRoute(DBInfoModel DBInfo, DeliveryRoutingNewModel data, out List<long> modifiedDeliveryRouteIds, out List<long> deletedDeliveryRouteIds, IHubContext hub);

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
        List<DeliveryRoutingModel> getRoutes(DBInfoModel DBInfo, long? deliveryRoutingId = null, int? status = null);

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
        /// <returns>bool</returns>
        bool updateRouteStatus(DBInfoModel DBInfo, long deliveryRoutingId, int status);

        /// <summary>
        /// change route assign status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="assignStatus">int</param>
        /// <returns>bool</returns>
        bool updateRouteAssignStatus(DBInfoModel DBInfo, long deliveryRoutingId, int assignStatus);

        /// <summary>
        /// delivery routing staff login
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="login">DeliveryRoutingStaffCredentialsModel</param>
        /// <returns>long?</returns>
        long? staffLogin(DBInfoModel DBInfo, DeliveryRoutingStaffCredentialsModel login);

        /// <summary>
        /// change staff assigned to route
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="newStaffId">long</param>
        /// <param name="oldStaffId">long?</param>
        /// <param name="modifiedDeliveryRouteId">out long?</param>
        /// <param name="hub">IHubContext</param>
        /// <returns>bool</returns>
        bool changeRouteStaff(DBInfoModel DBInfo, long deliveryRoutingId, long newStaffId, long? oldStaffId, out long? modifiedDeliveryRouteId, IHubContext hub);

        /// <summary>
        /// accept call for delivery routing assignment from mobile client
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="hub">IHubContext</param>
        /// <returns>bool</returns>
        bool acceptRoute(DBInfoModel DBInfo, string staff, long deliveryRoutingId, IHubContext hub);

        /// <summary>
        /// reject call for delivery routing assignment from mobile client
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="hub">IHubContext</param>
        /// <returns>bool</returns>
        bool rejectRoute(DBInfoModel DBInfo, string staff, long deliveryRoutingId, IHubContext hub);

        /// <summary>
        /// complete call for delivery routing assignment from mobile client
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <returns>bool</returns>
        bool completeRoute(DBInfoModel DBInfo, string staff, long deliveryRoutingId);

        /// <summary>
        /// return last route for specified staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <returns>DeliveryRoutingExtModel</returns>
        StaffRoutes getLastStaffRoute(DBInfoModel DBInfo, string staff);

        /// <summary>
        /// return specific route associated order ids
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <returns>List<long></returns>
        List<long> getRouteOrderIds(DBInfoModel DBInfo, long deliveryRoutingId);

        /// <summary>
        /// clock-in staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        void ClockIn(DBInfoModel DBInfo, string staff);

        /// <summary>
        /// clock-out staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        void ClockOut(DBInfoModel DBInfo, string staff);
    }
}
