using Microsoft.AspNet.SignalR;
using Pos_WebApi.Helpers;
using Symposium.Helpers;
using Symposium.Helpers.Hubs;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.Delivery;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace Pos_WebApi.Controllers.V3.Delivery
{
    public class DeliveryRoutingV3Controller : BasicV3Controller
    {
        System.Web.Http.Dependencies.IDependencyResolver autofac;

        IDeliveryRoutingFlows genDeliveryRoutingFlows;

        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        DeliveryRoutingHubParticipants drHubParticipants;

        public DeliveryRoutingV3Controller(IDeliveryRoutingFlows genDeliveryRoutingFlows)
        {
            this.genDeliveryRoutingFlows = genDeliveryRoutingFlows;
            var config = System.Web.Http.GlobalConfiguration.Configuration;
            System.Web.Http.Dependencies.IDependencyResolver autofac;
            autofac = config.DependencyResolver;
            this.drHubParticipants = (DeliveryRoutingHubParticipants)autofac.GetService(typeof(DeliveryRoutingHubParticipants));
        }

        // test call to be used as third party api route
        [HttpPost, Route("api/v3/dr/Delivery/Routing/routeData")]
        public HttpResponseMessage routeData(Routing3Model data)
        {
            //System.Threading.Thread.Sleep(35000);
            logger.Info( data != null ? "route updated: id=" + data.Id + " staff=" + data.StaffId : "empty model");
            //return Request.CreateResponse(HttpStatusCode.InternalServerError, 0);
            return Request.CreateResponse(HttpStatusCode.OK, 0);
        }

        /// <summary>
        /// called from pos to create new delivery route with specific orders assigned and optional staff assignment
        /// </summary>
        /// <param name="data">DeliveryRoutingNewModel</param>
        /// <returns>long?</returns>
        [HttpPost, Route("api/v3/Delivery/Routing/createRoute")]
        public HttpResponseMessage createRoute(DeliveryRoutingNewModel data)
        {            
            List<long> modifiedDeliveryRouteIds = new List<long>();

            List<long> deletedDeliveryRouteIds = new List<long>();

            long? res = genDeliveryRoutingFlows.createRoute(DBInfo, data, out modifiedDeliveryRouteIds, out deletedDeliveryRouteIds, hub);

            bool drActivateMobileApp = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drActivateMobileApp");

            if (res != null)
            {
                if (deletedDeliveryRouteIds != null)
                {
                    // SignalR DeliveryRoutingDelete - send deleted routes to Pos and related staff mobile client
                    signalDeletedDeliveryRouteIds(deletedDeliveryRouteIds);
                }

                if (modifiedDeliveryRouteIds != null)
                {
                    // SignalR DeliveryRoutingChange - send modified routes to Pos
                    signalModifiedDeliveryRouteIds(modifiedDeliveryRouteIds);

                    if (drActivateMobileApp)
                    { 
                        // SignalR DeliveryManAssign - send delivery route assignment to related staff mobile client
                        signalDeliveryManAssignList(DBInfo, modifiedDeliveryRouteIds);
                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// assign orders to route. create route if not provided, update existing
        /// </summary>
        /// <param name="data">DeliveryRoutingOrdersListModel</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost, Route("api/v3/Delivery/Routing/addOrdersToRoute")]
        public HttpResponseMessage addOrdersToRoute(DeliveryRoutingOrdersListModel data)
        {
            List<long> modifiedDeliveryRouteIds = new List<long>();

            List<long> deletedDeliveryRouteIds = new List<long>();

            List<long> assignDeliveryRouteIds = new List<long>();

            bool res = genDeliveryRoutingFlows.addOrdersToRoute(DBInfo, data.orderIds, data.deliveryRoutingId, out modifiedDeliveryRouteIds, out deletedDeliveryRouteIds, out assignDeliveryRouteIds);

            if (res)
            {
                List<DeliveryRoutingModel> route = genDeliveryRoutingFlows.getRoutes(DBInfo, data.deliveryRoutingId);

                bool drActivateMobileApp = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drActivateMobileApp");

                if (deletedDeliveryRouteIds != null)
                {
                    // SignalR DeliveryRoutingDelete - send deleted routes to Pos and related staff mobile client
                    signalDeletedDeliveryRouteIds(deletedDeliveryRouteIds);
                }

                if (route != null && route.Count > 0)
                {
                    // SignalR DeliveryRoutingChange - send modified routes to Pos
                    hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(route[0].Id, route[0].Status);

                    if (drActivateMobileApp && route[0].AssignStatus == (int)DeliveryRoutingAssignStatusEnum.pendingResponse)
                    {
                        // SignalR DeliveryManAssign - send delivery route assignment to related staff mobile client
                        signalDeliveryManAssign(DBInfo, route[0]);
                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// unassign orders from route
        /// </summary>
        /// <param name="data">DeliveryRoutingOrdersListModel</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost, Route("api/v3/Delivery/Routing/removeOrdersFromRoute")]
        public HttpResponseMessage removeOrdersFromRoute(DeliveryRoutingOrdersListModel data)
        {
            List<long> modifiedDeliveryRouteIds = new List<long>();

            List<long> deletedDeliveryRouteIds = new List<long>();

            List<long> assignDeliveryRouteIds = new List<long>();

            bool res = genDeliveryRoutingFlows.removeOrdersFromRoute(DBInfo, data.orderIds, data.deliveryRoutingId, out modifiedDeliveryRouteIds, out deletedDeliveryRouteIds, out assignDeliveryRouteIds);

            if (res)
            {
                List<DeliveryRoutingModel> route = genDeliveryRoutingFlows.getRoutes(DBInfo, data.deliveryRoutingId);

                bool drActivateMobileApp = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drActivateMobileApp");

                if (deletedDeliveryRouteIds != null && deletedDeliveryRouteIds.Count > 0)
                {
                    // SignalR DeliveryRoutingDelete - send deleted routes to Pos and related staff mobile client
                    signalDeletedDeliveryRouteIds(deletedDeliveryRouteIds);
                }

                if (route != null && route.Count > 0)
                {
                    // SignalR DeliveryRoutingChange - send modified routes to Pos
                    hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(route[0].Id, route[0].Status);

                    if (drActivateMobileApp && route[0].AssignStatus == (int)DeliveryRoutingAssignStatusEnum.pendingResponse)
                    {
                        // SignalR DeliveryManAssign - send delivery route assignment to related staff mobile client
                        signalDeliveryManAssign(DBInfo, route[0]);
                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// get all routes if parameters null, get specific route if id provided, get routes of specific status if status provided
        /// </summary>
        /// <param name="deliveryRoutingId">long?</param>
        /// <param name="status">int?</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        [HttpGet, Route("api/v3/Delivery/Routing/getRoutes/{deliveryRoutingId}/{status}")]
        public HttpResponseMessage getRoutes(long? deliveryRoutingId = null, int? status = null)
        {
            List<DeliveryRoutingModel> res = genDeliveryRoutingFlows.getRoutes(DBInfo, deliveryRoutingId, status);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// get routes by ids provided
        /// </summary>
        /// <param name="model">DeliveryRoutingIdsListModel</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        [HttpPost, Route("api/v3/Delivery/Routing/getRoutes")]
        public HttpResponseMessage getRoutes(DeliveryRoutingIdsListModel model)
        {
            List<DeliveryRoutingModel> res = new List<DeliveryRoutingModel>();
            if(model != null)
            {
                res = genDeliveryRoutingFlows.getRoutes(DBInfo, model.routeIds);

                res = res.OrderBy(r => r.Id).ToList();
            }
            
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// change provided orders route, to another route if provided or to no route if not
        /// </summary>
        /// <param name="data">DeliveryRoutingMoveOrdersModel</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost, Route("api/v3/Delivery/Routing/changeOrdersRoute")]
        public HttpResponseMessage changeOrdersRoute(DeliveryRoutingMoveOrdersModel data)
        {
            bool resAdd = false;

            List<long> modifiedDeliveryRouteIds = new List<long>();

            List<long> deletedDeliveryRouteIds  = new List<long>();

            List<long> assignDeliveryRouteIds = new List<long>();

            bool drActivateMobileApp = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drActivateMobileApp");

            if (data.deliveryRoutingTargetId != null)
            {
                resAdd = genDeliveryRoutingFlows.addOrdersToRoute(DBInfo, data.orderIds, data.deliveryRoutingTargetId, out modifiedDeliveryRouteIds, out deletedDeliveryRouteIds, out assignDeliveryRouteIds);

                if (resAdd)
                {
                    List<DeliveryRoutingModel> targetRoute = genDeliveryRoutingFlows.getRoutes(DBInfo, data.deliveryRoutingTargetId);

                    if (deletedDeliveryRouteIds != null)
                    {
                        // SignalR DeliveryRoutingDelete - send deleted routes to Pos and related staff mobile client
                        signalDeletedDeliveryRouteIds(deletedDeliveryRouteIds);
                    }

                    if (targetRoute != null && targetRoute.Count > 0)
                    {
                        // SignalR DeliveryRoutingChange - send modified routes to Pos
                        hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(targetRoute[0].Id, targetRoute[0].Status);

                        if (drActivateMobileApp && targetRoute[0].AssignStatus == (int)DeliveryRoutingAssignStatusEnum.pendingResponse)
                        {
                            // SignalR DeliveryManAssign - send delivery route assignment to related staff mobile client
                            signalDeliveryManAssign(DBInfo, targetRoute[0]);
                        }
                    }
                }
            }

            deletedDeliveryRouteIds = null;

            bool resRemove = genDeliveryRoutingFlows.removeOrdersFromRoute(DBInfo, data.orderIds, data.deliveryRoutingSourceId, out modifiedDeliveryRouteIds, out deletedDeliveryRouteIds, out assignDeliveryRouteIds);

            if (resRemove)
            {
                List<DeliveryRoutingModel> sourceRoute = genDeliveryRoutingFlows.getRoutes(DBInfo, data.deliveryRoutingSourceId);

                if (deletedDeliveryRouteIds != null)
                {
                    // SignalR DeliveryRoutingDelete - send deleted routes to Pos and related staff mobile client
                    signalDeletedDeliveryRouteIds(deletedDeliveryRouteIds);
                }

                if (sourceRoute != null && sourceRoute.Count > 0)
                {
                    // SignalR DeliveryRoutingChange - send modified routes to Pos
                    hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(sourceRoute[0].Id, sourceRoute[0].Status);

                    if (drActivateMobileApp && sourceRoute[0].AssignStatus == (int)DeliveryRoutingAssignStatusEnum.pendingResponse)
                    {
                        // SignalR DeliveryManAssign - send delivery route assignment to related staff mobile client
                        signalDeliveryManAssign(DBInfo, sourceRoute[0]);
                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, resAdd && resRemove);
        }

        /// <summary>
        /// delete specific route
        /// </summary>
        /// <param name="deliveryRoutingId">long</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet, Route("api/v3/Delivery/Routing/deleteRoute/{deliveryRoutingId}")]
        public HttpResponseMessage deleteRoute(long deliveryRoutingId)
        {
            bool res = genDeliveryRoutingFlows.deleteRoute(DBInfo, deliveryRoutingId);

            if (res)
            {
                // SignalR DeliveryRoutingDelete - send deleted routes to Pos and related staff mobile client
                signalDeletedDeliveryRouteIds(new List<long>() { deliveryRoutingId });
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// change route, and associated orders, status to provided status
        /// </summary>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="status">int</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet, Route("api/v3/Delivery/Routing/updateRouteStatus/{deliveryRoutingId}/{status}")]
        public HttpResponseMessage updateRouteStatus(long deliveryRoutingId, int status)
        {
            bool res = genDeliveryRoutingFlows.updateRouteStatus(DBInfo, deliveryRoutingId, status);

            if (res)
            {
                // SignalR DeliveryRoutingChange - send modified routes to Pos
                hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(deliveryRoutingId, status);
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// change route, and associated orders, status to provided status
        /// </summary>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="assignStatus">int</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet, Route("api/v3/Delivery/Routing/updateRouteAssignStatus/{deliveryRoutingId}/{assignStatus}")]
        public HttpResponseMessage updateRouteAssignStatus(long deliveryRoutingId, int assignStatus)
        {
            logger.Info("Setting assign status for route " + deliveryRoutingId + " to " + assignStatus);

            bool res = genDeliveryRoutingFlows.updateRouteAssignStatus(DBInfo, deliveryRoutingId, assignStatus);
            
            bool drActivateMobileApp = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drActivateMobileApp");

            if (drActivateMobileApp)
            {
                List<DeliveryRoutingModel> route = genDeliveryRoutingFlows.getRoutes(DBInfo, deliveryRoutingId);

                if (route != null && route.Count > 0)
                {
                    // SignalR DeliveryManAssign - send delivery route assignment to related staff mobile client
                    signalDeliveryManAssign(DBInfo, route[0]);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// delivery routing staff login
        /// </summary>
        /// <param name="login">DeliveryRoutingStaffCredentialsModel</param>
        /// <returns>long?</returns>
        [HttpPost, Route("api/v3/dr/Delivery/Routing/staffLogin")]
        [AllowAnonymous]
        public HttpResponseMessage staffLogin(DeliveryRoutingStaffCredentialsModel login)
        {
            HttpContext httpContext = HttpContext.Current;

            string staffId = GetStaffIdFromAuthorization(httpContext.Request.Headers["Authorization"]);

            GetDefaultGuid();

            long? res = genDeliveryRoutingFlows.staffLogin(DBInfo, login);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// change staff assigned to route
        /// </summary>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="newStaffId">long</param>
        /// <param name="oldStaffId">long?</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet, Route("api/v3/Delivery/Routing/changeRouteStaff/{deliveryRoutingId}/{newStaffId}/{oldStaffId}")]
        public HttpResponseMessage changeRouteStaff(long deliveryRoutingId, long newStaffId, long? oldStaffId)
        {
            long? modifiedDeliveryRouteId = null;

            bool res = genDeliveryRoutingFlows.changeRouteStaff(DBInfo, deliveryRoutingId, newStaffId, oldStaffId, out modifiedDeliveryRouteId, hub);

            if (res)
            {
                List<DeliveryRoutingModel> route = genDeliveryRoutingFlows.getRoutes(DBInfo, deliveryRoutingId);

                if (route != null && route.Count > 0)
                {
                    if (modifiedDeliveryRouteId != null)
                    {
                        // SignalR DeliveryRoutingChange - send modified routes to Pos
                        hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(modifiedDeliveryRouteId, route[0].Status);
                    }

                    bool drActivateMobileApp = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drActivateMobileApp");

                    if (drActivateMobileApp)
                    {
                        // SignalR DeliveryManAssign - send delivery route assignment to related staff mobile client
                        signalDeliveryManAssign(DBInfo, route[0]);

                        if (oldStaffId != null)
                        {
                            string mobileClientConn = drHubParticipants.GetSessionId(Convert.ToInt64(oldStaffId));

                            if (!string.IsNullOrWhiteSpace(mobileClientConn))
                            {
                                // SignalR DeliveryManDismiss - send delivery route unassignment to related staff mobile client
                                hub.Clients.Client(mobileClientConn).DeliveryManDismiss(oldStaffId, deliveryRoutingId);
                            }
                        }
                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// accept call for delivery routing assignment from mobile client
        /// </summary>
        /// <param name="data">DeliveryRoutingIdModel</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost, Route("api/v3/dr/Delivery/Routing/acceptRoute")]
        [Route("api/v3/Delivery/Routing/acceptRoute")]
        public HttpResponseMessage acceptRoute(DeliveryRoutingIdModel data)
        {
            HttpContext httpContext = HttpContext.Current;

            logger.Info("Received accept route for id " + data.Id);

            string staff = GetStaffIdFromAuthorization(httpContext.Request.Headers["Authorization"]);

            bool res = genDeliveryRoutingFlows.acceptRoute(DBInfo, staff, data.Id, hub);

            if (res)
            {
                List<DeliveryRoutingModel> route = genDeliveryRoutingFlows.getRoutes(DBInfo, data.Id);

                if (route != null && route.Count > 0)
                {
                    if (route[0].AcceptDate != null && route[0].AssignDate != null)
                    {
                        route[0].acceptTimeOffset = (int)Convert.ToDateTime(route[0].AcceptDate).Subtract(Convert.ToDateTime(route[0].AssignDate)).TotalMinutes;
                    }

                    // SignalR DeliveryManAssignPos - send accepted route to Pos
                    hub.Clients.Group("DeliveryPos").DeliveryManAssignPos(route[0]);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// reject call for delivery routing assignment from mobile client
        /// </summary>
        /// <param name="data">DeliveryRoutingIdModel</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost, Route("api/v3/dr/Delivery/Routing/rejectRoute")]
        [Route("api/v3/Delivery/Routing/rejectRoute")]
        public HttpResponseMessage rejectRoute(DeliveryRoutingIdModel data)
        {
            HttpContext httpContext = HttpContext.Current;

            string staff = null;

            bool res = false;

            if (httpContext.Request.FilePath.Contains("/dr/"))
            {
                logger.Info("Received reject route for id " + data.Id);

                staff = GetStaffIdFromAuthorization(httpContext.Request.Headers["Authorization"]);
            }

            res = genDeliveryRoutingFlows.rejectRoute(DBInfo, staff, data.Id, hub);

            if (res)
            {
                // SignalR DeliveryManReject - send rejected route to Pos
                hub.Clients.Group("DeliveryPos").DeliveryManReject(data.Id);
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// complete call for delivery routing assignment from mobile client
        /// </summary>
        /// <param name="data">DeliveryRoutingIdModel</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost, Route("api/v3/dr/Delivery/Routing/completeRoute")]
        [Route("api/v3/Delivery/Routing/completeRoute")]
        public HttpResponseMessage completeRoute(DeliveryRoutingIdModel data)
        {
            HttpContext httpContext = HttpContext.Current;

            string staff = null;

            bool res = false;

            try
            {
                if (httpContext.Request.FilePath.Contains("/dr/"))
                {
                    logger.Info("Received complete route for id " + data.Id);

                    staff = GetStaffIdFromAuthorization(httpContext.Request.Headers["Authorization"]);
                }

                res = genDeliveryRoutingFlows.completeRoute(DBInfo, staff, data.Id);

                if (res && httpContext.Request.FilePath.Contains("/dr/"))
                {
                    List<DeliveryRoutingModel> routes = genDeliveryRoutingFlows.getRoutes(DBInfo, data.Id);

                    if (routes != null && routes.Count > 0)
                    {
                        DeliveryRoutingModel route = routes[0];

                        logger.Info("Signaling DeliveryManReturned for route " + data.Id);

                        // SignalR DeliveryManReturned - send complete route to Pos
                        hub.Clients.Group("DeliveryPos").DeliveryManReturned(route); // data.Id);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error("Failed to complete route: " + ex.ToString());
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// clock-in staff
        /// </summary>
        /// <returns>ok</returns>
        [HttpGet, Route("api/v3/dr/Delivery/Routing/ClockIn")]
        public HttpResponseMessage ClockIn()
        {
            string staff = GetStaffIdFromAuthorization(HttpContext.Current.Request.Headers["Authorization"]);

            genDeliveryRoutingFlows.ClockIn(DBInfo, staff);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// clock-out staff
        /// </summary>
        /// <returns>ok</returns>
        [HttpGet, Route("api/v3/dr/Delivery/Routing/ClockOut")]
        public HttpResponseMessage ClockOut()
        {
            string staff = GetStaffIdFromAuthorization(HttpContext.Current.Request.Headers["Authorization"]);
            
            genDeliveryRoutingFlows.ClockOut(DBInfo, staff);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// return last route for specified staff
        /// </summary>
        /// <returns>DeliveryRoutingExtModel</returns>
        [HttpGet, Route("api/v3/dr/Delivery/Routing/getLastStaffRoute")]
        public HttpResponseMessage getLastStaffRoute()
        {
            HttpContext httpContext = HttpContext.Current;

            string staff = GetStaffIdFromAuthorization(httpContext.Request.Headers["Authorization"]);

            StaffRoutes res = genDeliveryRoutingFlows.getLastStaffRoute(DBInfo, staff);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// get staff if from calls 'Authorization' header
        /// </summary>
        /// <param name="authHeader">string</param>
        /// <returns>string</returns>
        private string GetStaffIdFromAuthorization(string authHeader)
        {
            string staffId = null;

            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();

                Encoding encoding = Encoding.GetEncoding("iso-8859-1");

                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');

                staffId = usernamePassword.Substring(0, seperatorIndex);
            }

            return staffId;
        }

        /// <summary>
        /// send signalr messages for deleted routes
        /// </summary>
        /// <param name="ids">List<long></param>
        private void signalDeletedDeliveryRouteIds(List<long> ids)
        {
            bool drActivateMobileApp = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drActivateMobileApp");

            foreach (long deliveryRouteId in ids)
            {
                if (drActivateMobileApp)
                {
                    List<DeliveryRoutingModel> targetRoute = genDeliveryRoutingFlows.getRoutes(DBInfo, deliveryRouteId);

                    if (targetRoute != null && targetRoute.Count > 0)
                    {
                        string mobileClientConn = drHubParticipants.GetSessionId(Convert.ToInt64(targetRoute[0].StaffId));

                        if (!string.IsNullOrWhiteSpace(mobileClientConn))
                        { 
                            hub.Clients.Client(mobileClientConn).DeliveryRoutingDelete(targetRoute[0].StaffId, deliveryRouteId);
                        }
                        else
                        {
                            logger.Debug("session for staff " + targetRoute[0].StaffId + " not found");
                        }
                    }
                }

                hub.Clients.Group("DeliveryPos").DeliveryRoutingDelete(deliveryRouteId);
            }
        }

        /// <summary>
        /// send signalr messages for modified routes
        /// </summary>
        /// <param name="ids">List<long></param>
        private void signalModifiedDeliveryRouteIds(List<long> ids)
        {
            List<DeliveryRoutingModel> routes = genDeliveryRoutingFlows.getRoutes(DBInfo, ids);

            foreach (DeliveryRoutingModel route in routes)
            {
                hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(route.Id, route.Status);
            }
        }

        /// <summary>
        /// send signalr messages for accepted route assignment
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="data">DeliveryRoutingModel</param>
        private void signalDeliveryManAssign(DBInfoModel DBInfo, DeliveryRoutingModel data)
        {
            if (data != null && data.StaffId != null)
            { 
                try
                {
                    DeliveryRoutingExtModel model = new DeliveryRoutingExtModel();

                    model.route = data;

                    List<long> ids = genDeliveryRoutingFlows.getRouteOrderIds(DBInfo, data.Id);

                    model.orderNos = ids.ConvertAll(x => Convert.ToString(x));

                    string mobileClientConn = drHubParticipants.GetSessionId(Convert.ToInt64(data.StaffId));

                    if (!string.IsNullOrWhiteSpace(mobileClientConn))
                    {
                        hub.Clients.Client(mobileClientConn).DeliveryManAssign(data.StaffId, model);
                    }
                    else
                    {
                        logger.Debug("session for staff " + StaffId + " not found");
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Failed to send signalR message DeliveryManAssign to mobile client");

                    logger.Warn(ex.ToString());
                }
            }
        }

        /// <summary>
        /// send signalr messages for list of accepted route assignments
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="data">List<long></param>
        private void signalDeliveryManAssignList(DBInfoModel DBInfo, List<long> data)
        {
            foreach (long id in data)
            {
                List<DeliveryRoutingModel> route = genDeliveryRoutingFlows.getRoutes(DBInfo, id);

                if (route != null && route.Count > 0)
                {
                    signalDeliveryManAssign(DBInfo, route[0]);
                }
            }
        }

        private void GetDefaultGuid()
        {
            var config = System.Web.Http.GlobalConfiguration.Configuration;

            autofac = config.DependencyResolver;

            string deliveryRoutingDefaultGuid = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drDefaultGuid");

            IStoreIdsPropertiesHelper stores = (IStoreIdsPropertiesHelper)autofac.GetService(typeof(IStoreIdsPropertiesHelper));
            
            DBInfo = stores.GetStoreById(new Guid(deliveryRoutingDefaultGuid));
        }
    }
}