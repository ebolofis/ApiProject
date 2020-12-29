using Symposium.WebApi.DataAccess.Interfaces.DT.Delivery;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Delivery;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using Symposium.Models.Models.Delivery;
using Symposium_DTOs.PosModel_Info;
using System.Linq;
using System.Data;
using Symposium.Helpers;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Symposium.Helpers.Hubs;
using Symposium.WebApi.DataAccess;
using System.Web;
using log4net;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System.Data.SqlClient;
using Symposium.Models.Enums;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using System.Threading;
using AutoMapper;
using System.Globalization;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections;

namespace Symposium.WebApi.MainLogic.Tasks.Delivery
{
    public class DeliveryRoutingTasks : IDeliveryRoutingTasks
    {
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IDeliveryRoutingDT DeliveryRoutingDT;
        IDeliveryOrdersDT genDeliveryOrdersDT;
        DeliveryRoutingHubParticipants drHubParticipants;
        IUsersToDatabasesXML usersToDatabases;
        IStaffDT genStaffDT;
        DeliveryRoutingRoutesList pendingRoutes;

        private readonly object lockKey = new object();

        public DeliveryRoutingTasks(
            IDeliveryRoutingDT DeliveryRoutingDT,
            IDeliveryOrdersDT genDeliveryOrdersDT,
            DeliveryRoutingHubParticipants drHubParticipants,
            IUsersToDatabasesXML usersToDatabases, 
            IStaffDT genStaffDT,
            DeliveryRoutingRoutesList pendingRoutes)
        {
            this.DeliveryRoutingDT = DeliveryRoutingDT;
            this.genDeliveryOrdersDT = genDeliveryOrdersDT;
            this.drHubParticipants = drHubParticipants;
            this.usersToDatabases = usersToDatabases;
            this.genStaffDT = genStaffDT;
            this.pendingRoutes = pendingRoutes;
        }

        /// <summary>
        /// task that searches for unassigned routes after api restart and attempts auto assignment
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="hub">IHubContext</param>
        public void restartAsyncAssignRouteToStaff(DBInfoModel DBInfo, IHubContext hub = null)
        {
            AutoMapperConfig.RegisterMappings();

            bool drAutoAssignRouting = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drAutoAssignRouting");

            if (drAutoAssignRouting)
            {
                List<DeliveryRoutingModel> unassignedRoutes = getUnassignedRoutes(DBInfo);

                foreach (DeliveryRoutingModel dr in unassignedRoutes)
                {
                    asyncAssignRouteToStaff(DBInfo, dr.Id, hub);
                }
            }
        }

        /// <summary>
        /// get routes that have assigne status: searchingStaff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        public List<DeliveryRoutingModel> getUnassignedRoutes(DBInfoModel DBInfo)
        {
            return DeliveryRoutingDT.getRoutesByAssignStatus(DBInfo, (int)DeliveryRoutingAssignStatusEnum.searchingStaff);
        }

        /// <summary>
        /// get routes with specified status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="assignStatus">int</param>
        /// <param name="staffId">long</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        public List<DeliveryRoutingModel> getStaffRoutesByAssignStatus(DBInfoModel DBInfo, int assignStatus, long staffId)
        {
            return DeliveryRoutingDT.getStaffRoutesByAssignStatus(DBInfo, assignStatus, staffId);
        }

        /// <summary>
        /// check if route exists in database
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <returns>bool</returns>
        public bool checkRouteExists(DBInfoModel DBInfo, long deliveryRoutingId)
        {
            DeliveryRoutingModel res = getRoute(DBInfo, deliveryRoutingId);

            return res != null ? true : false;
        }

        /// <summary>
        /// check if staff is connected to hub list
        /// </summary>
        /// <param name="staffId">long</param>
        /// <returns>bool</returns>
        public bool checkStaffAvailable(DBInfoModel DBInfo, long staffId, string[] rejectedStaffNames = null)
        {
            //return drHubParticipants.IdExists(staffId);
            StaffDTO data = Mapper.Map<StaffDTO>(genStaffDT.GetStaffById(DBInfo, staffId));

            if (data == null || data.isAssignedToRoute == true ||  data.CurrentOrderStatus == true)
            {
                return false;
            }
            else if (rejectedStaffNames != null)
            {
                string staffFullName = data.LastName + ' ' + data.FirstName;

                if (rejectedStaffNames.Contains(staffFullName))
                {
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// attempt to auto assign route to staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="returnId">long</param>
        /// <param name="hub">IHubContext</param>
        public void asyncAssignRouteToStaff(DBInfoModel DBInfo, long deliveryRouteId, IHubContext hub)
        {
            logger.Info("initiate assignment for route " + deliveryRouteId);

            Task.Run(() => 
            {
                try
                {
                    pendingRoutes.pendingRoutes.Add(deliveryRouteId, deliveryRouteId);
                    //logger.Info("### added to pending routes dr " + deliveryRouteId + " : " + JsonConvert.SerializeObject(pendingRoutes.pendingRoutes));
                    StaffDeliveryModel staff = null;

                    bool staffAvailable = false;

                    bool routeExists = true;

                    int retries = 0;

                    int maxRetries = Convert.ToInt32(MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drAutoAssignRoutingMaxRetries"));

                    int retryDelay = Convert.ToInt32(MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drAutoAssignRoutingDelay"));

                    List<DeliveryRoutingModel> routes = getRoutes(DBInfo, deliveryRouteId);

                    Hashtable staffRetries = new Hashtable();

                    while (retries <= maxRetries && staff == null && pendingRoutes.pendingRoutes.ContainsKey(deliveryRouteId))
                    {
                        routeExists = checkRouteExists(DBInfo, deliveryRouteId);

                        if (!routeExists)
                        {
                            logger.Error("Auto assign staff to route failed. Route does not exist anymore");

                            return;
                        }

                        lock (lockKey)
                        {

                            List<StaffDeliveryModel> staffs = DeliveryRoutingDT.getActiveStaffList(DBInfo);
                            
                            if (staffs != null && staffs.Count > 0)
                            {
                                staff = staffs[0];

                                string staffFullName = staff.LastName + ' ' + staff.FirstName;

                                string[] rejectedStaffNames = null;

                                if (routes != null && routes.Count > 0)
                                {
                                    if (!string.IsNullOrWhiteSpace(routes[0].RejectedNames))
                                    {
                                        rejectedStaffNames = routes[0].RejectedNames.Split(';').ToArray();
                                    }

                                    int staffTriesCount = staffRetries.Contains(staffFullName) ? Convert.ToInt32(staffRetries[staffFullName]) + 1 : 1;

                                    if (staffTriesCount >= (maxRetries < 8 ? maxRetries : 8))
                                    {
                                        rejectedStaffNames = null;
                                    }

                                    if (staffRetries.Contains(staffFullName))
                                        staffRetries[staffFullName] = staffTriesCount;
                                    else
                                        staffRetries.Add(staffFullName, staffTriesCount);
                                }

                                staffAvailable = checkStaffAvailable(DBInfo, staff.StaffId, rejectedStaffNames);

                                if (!staffAvailable)
                                {
                                    logger.Info("Auto assign staff to route " + deliveryRouteId + ", attempt " + (retries + 1).ToString() + " failed. Staff " + staff.StaffId + " not available");
                                }
                                else
                                {                                    
                                    if (routes != null && routes.Count > 0 && deliveryRouteId == pendingRoutes.pendingRoutes.Keys[0])
                                    {
                                        string connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                                        using (IDbConnection db = new SqlConnection(connectionString))
                                        {
                                            db.Open();

                                            using (IDbTransaction transact = db.BeginTransaction(IsolationLevel.ReadCommitted))
                                            {
                                                bool res = DeliveryRoutingDT.updateRouteAssignStatus(DBInfo, deliveryRouteId, (int)DeliveryRoutingAssignStatusEnum.pendingResponse, db, transact);

                                                if (!res)
                                                {
                                                    logger.Error("Failed to update route " + deliveryRouteId + " assign status");

                                                    return;
                                                }

                                                res = updateRouteStaff(DBInfo, deliveryRouteId, staff.StaffId, db, transact);

                                                if (!res)
                                                {
                                                    logger.Error("Failed to update route " + deliveryRouteId + " staff");

                                                    return;
                                                }

                                                bool useMobile = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drActivateMobileApp");

                                                res = DeliveryRoutingDT.updateStaffIsAssignedToRoute(DBInfo, staff.StaffId, true, db, transact, useMobile ? (int?)null : 4);

                                                long resUpd = genDeliveryOrdersDT.UpdateStaffStatusTran(DBInfo, staff.StaffId, true, db, transact);

                                                if (resUpd != 0)
                                                {
                                                    logger.Error("Failed to update status for staff " + staff.StaffId);

                                                    return;
                                                }

                                                transact.Commit();

                                                if (pendingRoutes.pendingRoutes.ContainsKey(deliveryRouteId))
                                                {
                                                    pendingRoutes.pendingRoutes.Remove(deliveryRouteId);
                                                }

                                                logger.Info("Auto assign staff to route success. Route " + routes[0].Id + " has been assigned to staff " + staff.StaffId);
                                            }
                                        }
                                        
                                        break;
                                    }
                                }

                                staff = null;
                            }
                            else
                            {
                                logger.Info("Auto assign staff to route " + deliveryRouteId + ", attempt " + (retries + 1).ToString() + " failed. No available staff");
                            }
                        }

                        Thread.Sleep(retryDelay * 1000);

                        retries++;
                    }

                    if (staff == null)
                    {
                        logger.Error("Auto assign staff to route failed. No available staff");

                        bool res = DeliveryRoutingDT.updateRouteAssignStatus(DBInfo, deliveryRouteId, (int)DeliveryRoutingAssignStatusEnum.autoAssignStaffFailure);

                        if (res && routes != null && routes.Count > 0)
                        {
                            hub.Clients.Group("DeliveryPos").DeliveryManFailure(routes[0].Id, routes[0].Status);
                        }
                    }
                    else
                    {
                        //logger.Info("### staff not null. route " + deliveryRouteId + " : " + JsonConvert.SerializeObject(staff));
                        string drExternalApiUrl = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drExternalApiUrlAssign");

                        if (!string.IsNullOrEmpty(drExternalApiUrl))
                        {
                            long? modifiedDeliveryRouteStatus = null;

                            long? modifiedDeliveryRouteId = null;

                            sendRouteToExternalApi(DBInfo, Convert.ToInt64(deliveryRouteId), drExternalApiUrl, out modifiedDeliveryRouteId, out modifiedDeliveryRouteStatus, null);

                            if (modifiedDeliveryRouteId != null)
                            {
                                hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(modifiedDeliveryRouteId, modifiedDeliveryRouteStatus);
                            }
                        }

                        List<DeliveryRoutingModel> updatedRoute = getRoutes(DBInfo, deliveryRouteId);

                        if (updatedRoute != null && updatedRoute.Count > 0)
                        {
                            hub.Clients.Group("DeliveryPos").DeliveryManAssignPos(updatedRoute[0]);

                            DeliveryRoutingExtModel model = new DeliveryRoutingExtModel();

                            model.route = updatedRoute[0];

                            model.orderNos = getRouteOrderIds(DBInfo, deliveryRouteId).ConvertAll(x => Convert.ToString(x));

                            string mobileClientConn = drHubParticipants.GetSessionId(staff.StaffId);

                            if (!string.IsNullOrEmpty(mobileClientConn))
                            {
                                hub.Clients.Client(mobileClientConn).DeliveryManAssign(staff.StaffId, model);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    logger.Error("Failed to assign staff to route: " + ex.ToString());
                }
                finally
                {
                    //logger.Info("### remove route " + deliveryRouteId + " : " + JsonConvert.SerializeObject(pendingRoutes.pendingRoutes));
                    if (pendingRoutes.pendingRoutes.ContainsKey(deliveryRouteId))
                    {
                        pendingRoutes.pendingRoutes.Remove(deliveryRouteId);
                        //logger.Info("### route removed " + deliveryRouteId + " : " + JsonConvert.SerializeObject(pendingRoutes.pendingRoutes));
                    }
                }

                return;
            });
        }

        /// <summary>
        /// release staff from route, set isonroad = 0, isassignedtoroute = 0, datetime = now
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staffId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool releaseStaff(DBInfoModel DBInfo, long staffId, IDbConnection db = null, IDbTransaction transact = null)
        {
            return DeliveryRoutingDT.releaseStaff(DBInfo, staffId, db, transact);
        }

        /// <summary>
        /// update staff record, isAssignedToRoute field with provided value
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staffId">long</param>
        /// <param name="assigned">bool</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool updateStaffIsAssignedToRoute(DBInfoModel DBInfo, long staffId, bool assigned, IDbConnection db = null, IDbTransaction transact = null, int? routeStatus = null)
        {
            return DeliveryRoutingDT.updateStaffIsAssignedToRoute(DBInfo, staffId, assigned, db, transact, routeStatus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DBInfo"></param>
        /// <param name="deliveryRoutingId"></param>
        /// <param name="staffId"></param>
        /// <param name="db"></param>
        /// <param name="transact"></param>
        /// <returns></returns>
        public bool sendCancelRouteToExternalApp(DBInfoModel DBInfo, long deliveryRoutingId, long? staffId, IDbConnection db = null, IDbTransaction transact = null)
        {
            string drExternalApiUrl = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drExternalApiUrlUnassign");

            if (!string.IsNullOrEmpty(drExternalApiUrl))
            {
                List<DeliveryRoutingModel> routes = getRoutes(DBInfo, deliveryRoutingId, null, db, transact);

                DeliveryRoutingModel data = new DeliveryRoutingModel();

                HttpContent content;

                string json;

                if (routes != null && routes.Count > 0)
                {
                    data = routes[0];
                }

                Routing3ReleaseModel postModel = new Routing3ReleaseModel();

                postModel.Id = deliveryRoutingId;

                postModel.StaffId = staffId != null ? Convert.ToInt64(staffId) : data != null ? Convert.ToInt64(data.StaffId) : 0;

                postModel.StoreId = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drStoreId");

                json = JsonConvert.SerializeObject(postModel);

                content = new StringContent(json, Encoding.UTF8, "application/json");

                string auth = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drExternalApiAuthorization");

                if (string.IsNullOrEmpty(auth))
                {
                    logger.Error("Error retrieving authorization for external api from api configuration (key drExternalApiAuthorization).");
                }

                string authorizationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));

                HttpClient client = new HttpClient();

                try
                {
                    client.BaseAddress = new Uri(drExternalApiUrl);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }

                if (data != null && data.Id != 0)
                {
                    data.Failure3th = false;
                }

                client.DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

                request.Content = content;

                request.Headers.Add("Authorization", "Basic " + authorizationString);

                logger.Info("Sending unassign to external api for route " + deliveryRoutingId);

                var t = PostURI(client, request);

                if (!t.Result.IsSuccessStatusCode)
                {
                    logger.Error("Send unassign to external api failed for route " + deliveryRoutingId);

                    logger.Error("Send route model: " + json);

                    logger.Error("External api url: " + drExternalApiUrl);

                    logger.Error(t.Result);

                    if (data != null && data.Id != 0)
                    {
                        data.Failure3th = true;

                        bool resUpd = DeliveryRoutingDT.updateRoute(DBInfo, data, db, transact);

                        if (!resUpd)
                        {
                            logger.Error("Failed to update Failure3th field for route " + deliveryRoutingId);
                        }
                    }

                    return false;
                }
                else
                {
                    logger.Info("Send route info to external api success for route " + deliveryRoutingId);
                }
            }
            else
            {
                logger.Error("Error retrieving external api url for unassignment from api configuration (key drExternalApiUrlUnassign).");

                return false;
            }

            return true;
        }

        /// <summary>
        /// post request to external api
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="request">HttpRequestMessage</param>
        /// <returns>Task<HttpResponseMessage></returns>
        private Task<HttpResponseMessage> PostURI(HttpClient client, HttpRequestMessage request)
        {
            return Task.Run(() =>
            {
                return client.SendAsync(request).Result;
            });
        }

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
        public bool addOrdersToRoute(DBInfoModel DBInfo, List<long> orderIds, long? deliveryRoutingId, out List<long> modifiedDeliveryRouteIds, out List<long> deletedDeliveryRouteIds, out List<long> assignDeliveryRouteIds)
        {
            return DeliveryRoutingDT.addOrdersToRoute(DBInfo, orderIds, deliveryRoutingId, out modifiedDeliveryRouteIds, out deletedDeliveryRouteIds, out assignDeliveryRouteIds);
        }

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
        public bool removeOrdersFromRoute(DBInfoModel DBInfo, List<long> orderIds, long? deliveryRoutingId, out List<long> modifiedDeliveryRouteIds, out List<long> deletedDeliveryRouteIds, out List<long> assignDeliveryRouteIds)
        {
            List<DeliveryRoutingModel> routes = getRoutes(DBInfo, deliveryRoutingId);

            long? staffId = null;  

            if (routes != null && routes.Count > 0)
            {
                staffId = routes[0].StaffId;
            }

            bool res = DeliveryRoutingDT.removeOrdersFromRoute(DBInfo, orderIds, deliveryRoutingId, out modifiedDeliveryRouteIds, out deletedDeliveryRouteIds, out assignDeliveryRouteIds);

            if (deletedDeliveryRouteIds != null && deletedDeliveryRouteIds.Count > 0)
            {
                res = sendCancelRouteToExternalApp(DBInfo, deletedDeliveryRouteIds[0], staffId, null, null);
            }

            return res;
        }

        /// <summary>
        /// get all routes if parameters null, get specific route if id provided, get routes of specific status if status provided
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long?</param>
        /// <param name="status">int?</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        public List<DeliveryRoutingModel> getRoutes(DBInfoModel DBInfo, long? deliveryRoutingId = null, int? status = null, IDbConnection db = null, IDbTransaction transact = null)
        {
            return DeliveryRoutingDT.getRoutes(DBInfo, deliveryRoutingId, status, db, transact);
        }

        /// <summary>
        /// get route by id provided
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <returns>DeliveryRoutingModel</returns>
        public DeliveryRoutingModel getRoute(DBInfoModel DBInfo, long deliveryRoutingId)
        {
            return DeliveryRoutingDT.getRoute(DBInfo, deliveryRoutingId);
        }

        /// <summary>
        /// get routes by ids provided
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="routeIds">List<long></param>
        /// <returns>List<DeliveryRoutingModel></returns>
        public List<DeliveryRoutingModel> getRoutes(DBInfoModel DBInfo, List<long> routeIds)
        {
            return DeliveryRoutingDT.getRoutes(DBInfo, routeIds);
        }

        /// <summary>
        /// delete specific route
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool deleteRoute(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db = null, IDbTransaction transact = null)
        {
            return DeliveryRoutingDT.deleteRoute(DBInfo, deliveryRoutingId, db, transact);
        }

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
        public bool updateRouteStatus(DBInfoModel DBInfo, long deliveryRoutingId, int status, IDbConnection db, IDbTransaction transact, int? assignStatus = null, bool? complete = false)
        {
            logger.Info("Setting route status for route " + deliveryRoutingId + " to " + status);

            bool res = DeliveryRoutingDT.updateRouteStatus(DBInfo, deliveryRoutingId, status, db, transact, assignStatus, complete);

            if (res)
            {
                DeliveryRoutingOrdersModel model = DeliveryRoutingDT.getRouteOrders(DBInfo, deliveryRoutingId, db, transact);

                if (model == null)
                {
                    transact.Rollback();

                    return false;
                }

                //List<long> orderIds = model.OrdersList.Select(x => x.Id).OfType<long>().ToList();

                //res = DeliveryRoutingDT.updateOrdersStatus(DBInfo, orderIds, status, db, transact);

                //if (res)
                //{
                //    return true;
                //}

                return true;
            }

            transact.Rollback();

            return false;
        }

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
        public bool updateRouteAssignStatus(DBInfoModel DBInfo, long deliveryRoutingId, int assignStatus)
        {
            bool res = DeliveryRoutingDT.updateRouteAssignStatus(DBInfo, deliveryRoutingId, assignStatus);

            if (res)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// modify provided orders status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderIds">List<long></param>
        /// <param name="status">int</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool updateOrdersStatus(DBInfoModel DBInfo, List<long> orderIds, int status, IDbConnection db = null, IDbTransaction transact = null)
        {
            return true; // DeliveryRoutingDT.updateOrdersStatus(DBInfo, orderIds, status, db, transact);
        }

        /// <summary>
        /// get specified route list of order ids
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>List<long></returns>
        public List<long> getRouteOrderIds(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db = null, IDbTransaction transact = null)
        {
            DeliveryRoutingOrdersModel orders = DeliveryRoutingDT.getRouteOrders(DBInfo, deliveryRoutingId, db, transact);

            if (orders == null)
            {
                return null;
            }

            List<long> res = orders.OrdersList.Select(x => x.Id).OfType<long>().ToList();

            return res;
        }

        public long? getOrderRouteId(DBInfoModel DBInfo, long orderid)
        {
            return DeliveryRoutingDT.getOrderRouteId(DBInfo, orderid);
        }

        public List<DeliveryRoutingLastRouteInfoDetails> getRouteOrdersDetails(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db = null, IDbTransaction transact = null)
        {
            List<DeliveryRoutingLastRouteInfoDetails> res = new List<DeliveryRoutingLastRouteInfoDetails>();

            DeliveryRoutingOrdersModel orders = DeliveryRoutingDT.getRouteOrders(DBInfo, deliveryRoutingId, db, transact);
            
            if (orders != null)
            {
                foreach (OrderModel o in orders.OrdersList)
                {
                    try
                    {
                        DeliveryRoutingLastRouteInfoDetails order = new DeliveryRoutingLastRouteInfoDetails();
                    
                        order.orderId = Convert.ToString(o.Id);

                        order.orderNo = Convert.ToString(o.OrderNo);

                        Dictionary<string, string> orderModel = DeliveryRoutingDT.GetOrderExtElements(DBInfo, Convert.ToInt64(o.Id), new List<string>() { "bell", "EstBillingDate" });

                        DateTime date;

                        if (orderModel.ContainsKey("EstBillingDate"))
                        {
                            DateTime.TryParse(orderModel["EstBillingDate"], out date);
                        }
                        else
                        {
                            date = DateTime.MinValue;
                        }

                        order.deliveryTime = date; //orderModel.ContainsKey("EstBillingDate") ? DateTime.ParseExact(orderModel["EstBillingDate"].Replace('T',' '), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) : (DateTime?)null;

                        InvoiceShippingDetailsExtModel cstm = DeliveryRoutingDT.GetOrderCustomer(DBInfo, deliveryRoutingId, Convert.ToInt64(o.Id), db);

                        if (cstm != null)
                        {
                            cstm.bell = orderModel.ContainsKey("bell") ? orderModel["bell"] : "";
                        }

                        order.customerInfo = cstm;

                        res.Add(order);
                    }
                    catch(Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// get route and associated orders data
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>DeliveryRoutingOrdersModel</returns>
        public DeliveryRoutingOrdersModel getRouteOrders(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db, IDbTransaction transact)
        {
            return DeliveryRoutingDT.getRouteOrders(DBInfo, deliveryRoutingId, db, transact);
        }

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
        public long? upsertRouteOrders(DBInfoModel DBInfo, long? deliveryRoutingId, int orderIdCount, IDbConnection db, IDbTransaction transact, out List<long> deletedDeliveryRouteIds, int? status = null, long? staffId = null)
        {
            return DeliveryRoutingDT.upsertRouteOrders(DBInfo, deliveryRoutingId, orderIdCount, db, transact, out deletedDeliveryRouteIds, status, staffId);
        }

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
        public bool updateRouteStaff(DBInfoModel DBInfo, long deliveryRoutingId, long? staffId = null, IDbConnection db = null, IDbTransaction transact = null, long? staffUsername = null)
        {
            return DeliveryRoutingDT.updateRouteStaff(DBInfo, deliveryRoutingId, staffId, db, transact, staffUsername);
        }

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
        public bool updateOrdersDeliveryRoutingId(DBInfoModel DBInfo, List<long> orderIds, long? deliveryRoutingId, IDbConnection db, IDbTransaction transact, out List<long> modifiedDeliveryRouteIds)
        {
            return DeliveryRoutingDT.updateOrdersDeliveryRoutingId(DBInfo, orderIds, deliveryRoutingId, db, transact, out modifiedDeliveryRouteIds);
        }

        /// <summary>
        /// add staff name to delivery routing rejected field after reject
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="staffId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool addStaffToRejected(DBInfoModel DBInfo, long deliveryRoutingId, long staffId, IDbConnection db = null, IDbTransaction transact = null)
        {
            return DeliveryRoutingDT.addStaffToRejected(DBInfo, deliveryRoutingId, staffId, db, transact);
        }

        /// <summary>
        /// send data to external api. DeliveryRoutingChangeStaffModel when route staff is modified, Routing3Model when route is created
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="drExternalApiUrl">string</param>
        /// <param name="modifiedDeliveryRouteId">out long?</param>
        /// <param name="modifiedDeliveryRouteStatus">long?</param>
        /// <param name="oldStaffId">long?</param>
        public void sendRouteToExternalApi(DBInfoModel DBInfo, long deliveryRoutingId, string drExternalApiUrl, out long? modifiedDeliveryRouteId, out long? modifiedDeliveryRouteStatus, long? oldStaffId = null)
        {
            DeliveryRoutingDT.sendRouteToExternalApi(DBInfo, deliveryRoutingId, drExternalApiUrl, out modifiedDeliveryRouteId, out modifiedDeliveryRouteStatus, oldStaffId);
        }

        /// <summary>
        /// check provided orders for appropriate status when creating route or modifing staff 
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderIds">List<long></param>
        /// <returns>bool</returns>
        public bool checkValidOrderStatus(DBInfoModel DBInfo, List<long> orderIds)
        {
            return DeliveryRoutingDT.checkValidOrderStatus(DBInfo, orderIds);
        }

        /// <summary>
        /// check if staff id is required and provided
        /// </summary>
        /// <param name="staffId">long?</param>
        /// <returns>bool</returns>
        public bool checkValidStaffId(long? staffId)
        {
            bool drAutoAssignRouting = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drAutoAssignRouting");

            if (!drAutoAssignRouting && staffId == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// get appropriate route status on creation depending on configuration and staff provided
        /// </summary>
        /// <param name="staffId">long?</param>
        /// <returns>int</returns>
        public int getAppropriateStatus(long? staffId)
        {
            bool drActivateMobileApp = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drActivateMobileApp");

            if (drActivateMobileApp || (!drActivateMobileApp && staffId == null))
            {
                return (int)OrderStatusEnum.Ready;
            }

            return (int)OrderStatusEnum.Onroad;
        }

        /// <summary>
        /// task to move delivery routing records to hist table at end of day
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        public void moveDeliveryRoutingToHist(DBInfoModel DBInfo)
        {
            DeliveryRoutingDT.moveDeliveryRoutingToHist(DBInfo);
        }

        /// <summary>
        /// return last route for specified staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <returns>DeliveryRoutingExtModel</returns>
        public List<DeliveryRoutingLastRouteInfo> getLastStaffRoute(DBInfoModel DBInfo, IDbConnection db, string staff)
        {
            List<DeliveryRoutingLastRouteInfo> res = new List<DeliveryRoutingLastRouteInfo>();

            res = DeliveryRoutingDT.getLastStaffRoute(db, staff);

            foreach(DeliveryRoutingLastRouteInfo route in res)
            { 
                route.ordersDetails = getRouteOrdersDetails(DBInfo, route.route.Id, db, null);
            }

            return res;
        }

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
        public bool addStaffToActiveStaffList(DBInfoModel DBInfo, IDbConnection db, IDbTransaction transact, long? staffId = null, string staffUsername = null, string position = "last")
        {
            return DeliveryRoutingDT.addStaffToActiveStaffList(DBInfo, db, transact, staffId, staffUsername, position);
        }

        /// <summary>
        /// delivery routing staff login
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="login">DeliveryRoutingStaffCredentialsModel</param>
        /// <returns>long?</returns>
        public long? staffLogin(DBInfoModel DBInfo, DeliveryRoutingStaffCredentialsModel login)
        {
            return DeliveryRoutingDT.staffLogin(DBInfo, login);
        }
    }
}
