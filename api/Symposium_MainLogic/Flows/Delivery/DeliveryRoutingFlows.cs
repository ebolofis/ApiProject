using log4net;
using Microsoft.AspNet.SignalR;
using Symposium.Helpers;
using Symposium.Helpers.Hubs;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.Delivery;
using Symposium.Models.Models.Payroll;
using Symposium.WebApi.DataAccess.DT.Delivery;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Delivery;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Payroll;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Delivery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Delivery
{
    public class DeliveryRoutingFlows : IDeliveryRoutingFlows
    {
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IDeliveryRoutingTasks DeliveryRoutingTasks;
        IDeliveryOrdersDT genDeliveryOrdersDT;
        IUsersToDatabasesXML usersToDatabases;
        IPayrollFlows PayrollFlows;
        IStaffDT staffDT;
        DeliveryRoutingHubParticipants drHubParticipants;

        string connectionString;

        public DeliveryRoutingFlows(
            IDeliveryRoutingTasks DeliveryRoutingTasks,
            IDeliveryOrdersDT genDeliveryOrdersDT,
            IUsersToDatabasesXML usersToDatabases,
            IPayrollFlows PayrollFlows,
            IStaffDT staffDT,
            DeliveryRoutingHubParticipants drHubParticipants
            )
        {
            this.DeliveryRoutingTasks = DeliveryRoutingTasks;
            this.genDeliveryOrdersDT = genDeliveryOrdersDT;
            this.usersToDatabases = usersToDatabases;
            this.PayrollFlows = PayrollFlows;
            this.staffDT = staffDT;
            this.drHubParticipants = drHubParticipants;
        }

        /// <summary>
        /// called from pos to create new delivery route with specific orders assigned and optional staff assignment
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="data">DeliveryRoutingNewModel</param>
        /// <param name="modifiedDeliveryRouteIds">out List<long></param>
        /// <param name="deletedDeliveryRouteIds">out List<long></param>
        /// <param name="hub">IHubContext</param>
        /// <returns>long?</returns>
        public long? createRoute(DBInfoModel DBInfo, DeliveryRoutingNewModel data, out List<long> modifiedDeliveryRouteIds, out List<long> deletedDeliveryRouteIds, IHubContext hub)
        {
            long? returnId = null;
            
            long? modifiedDeliveryRouteId = null;

            modifiedDeliveryRouteIds = null;

            deletedDeliveryRouteIds = null;

            bool res = false;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                
                using (IDbTransaction transact = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    res = DeliveryRoutingTasks.checkValidOrderStatus(DBInfo, data.orderIds);

                    if (!res)
                    {
                        return null;
                    }

                    //res = DeliveryRoutingTasks.checkValidStaffId(data.staffId);

                    //if (!res)
                    //{
                    //    return null;
                    //}

                    long? drId = getOrderRouteId(DBInfo, data.orderIds);

                    if (drId == 0)
                    {
                        drId = null;
                    }

                    int status = DeliveryRoutingTasks.getAppropriateStatus(data.staffId);

                    returnId = DeliveryRoutingTasks.upsertRouteOrders(DBInfo, drId, data.orderIds.Count, db, transact, out deletedDeliveryRouteIds, status, data.staffId);

                    if (returnId == null)
                    {
                        transact.Rollback();

                        return null;
                    }

                    res = DeliveryRoutingTasks.updateOrdersDeliveryRoutingId(DBInfo, data.orderIds, returnId, db, transact, out modifiedDeliveryRouteIds);

                    if(!res)
                    {
                        transact.Rollback();

                        return null;
                    }

                    //res = DeliveryRoutingTasks.updateOrdersStatus(DBInfo, data.orderIds, status, db, transact);

                    //if (!res)
                    //{
                    //    transact.Rollback();

                    //    return null;
                    //}

                    if (data.staffId != null)
                    {
                        long resUpd = genDeliveryOrdersDT.UpdateStaffStatus(DBInfo, Convert.ToInt64(data.staffId), true, db, transact);

                        if (resUpd != 1)
                        {
                            logger.Error("Failed to update status for staff " + data.staffId);

                            transact.Rollback();

                            return null;
                        }

                        res = DeliveryRoutingTasks.updateStaffIsAssignedToRoute(DBInfo, Convert.ToInt64(data.staffId), true, db, transact);

                        if (!res)
                        {
                            return null;
                        }
                    }

                    transact.Commit();

                    System.Threading.Thread.Sleep(2000);

                    bool drAutoAssignRouting = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drAutoAssignRouting");

                    if (drAutoAssignRouting && data.staffId == null)
                    {
                        DeliveryRoutingTasks.asyncAssignRouteToStaff(DBInfo, Convert.ToInt64(returnId), hub);
                    }
                    else if (drAutoAssignRouting && data.staffId != null)
                    {
                        string drExternalApiUrl = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drExternalApiUrlAssign");

                        if (!string.IsNullOrEmpty(drExternalApiUrl))
                        {
                            long? modifiedDeliveryRouteStatus = null;

                            DeliveryRoutingTasks.sendRouteToExternalApi(DBInfo, Convert.ToInt64(returnId), drExternalApiUrl, out modifiedDeliveryRouteId, out modifiedDeliveryRouteStatus, null);

                            if (modifiedDeliveryRouteId != null)
                            {
                                hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(modifiedDeliveryRouteId, modifiedDeliveryRouteStatus);
                            }
                        }
                    }


                    return returnId;
                }
            }
        }

        public long? getOrderRouteId(DBInfoModel DBInfo, List<long> orderIds)
        {
            long? res = null;

            if (orderIds != null && orderIds.Count > 0)
            {
                long orderid = orderIds[0];
                
                res = DeliveryRoutingTasks.getOrderRouteId(DBInfo, orderid);
            }

            return res;
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
            return DeliveryRoutingTasks.addOrdersToRoute(DBInfo, orderIds, deliveryRoutingId, out modifiedDeliveryRouteIds, out deletedDeliveryRouteIds, out assignDeliveryRouteIds);
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
            return DeliveryRoutingTasks.removeOrdersFromRoute(DBInfo, orderIds, deliveryRoutingId, out modifiedDeliveryRouteIds, out deletedDeliveryRouteIds, out assignDeliveryRouteIds);
        }

        /// <summary>
        /// get all routes if parameters null, get specific route if id provided, get routes of specific status if status provided
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long?</param>
        /// <param name="status">int?</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        public List<DeliveryRoutingModel> getRoutes(DBInfoModel DBInfo, long? deliveryRoutingId = null, int? status = null)
        {
            return DeliveryRoutingTasks.getRoutes(DBInfo, deliveryRoutingId, status);
        }

        /// <summary>
        /// get routes by ids provided
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="routeIds">List<long></param>
        /// <returns>List<DeliveryRoutingModel></returns>
        public List<DeliveryRoutingModel> getRoutes(DBInfoModel DBInfo, List<long> routeIds)
        {
            return DeliveryRoutingTasks.getRoutes(DBInfo, routeIds);
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
            return DeliveryRoutingTasks.deleteRoute(DBInfo, deliveryRoutingId, db, transact);
        }

        /// <summary>
        /// change route, and associated orders, status to provided status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="status">int</param>
        /// <returns>bool</returns>
        public bool updateRouteStatus(DBInfoModel DBInfo, long deliveryRoutingId, int status)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                using (IDbTransaction transact = db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    bool res = DeliveryRoutingTasks.updateRouteStatus(DBInfo, deliveryRoutingId, status, db, transact);

                    if(res)
                    {
                        if (status == 3)
                        {
                            res = DeliveryRoutingTasks.sendCancelRouteToExternalApp(DBInfo, deliveryRoutingId, null, db, transact);
                        }

                        if (res)
                        {
                            transact.Commit();

                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// change route assign status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="assignStatus">int</param>
        /// <returns>bool</returns>
        public bool updateRouteAssignStatus(DBInfoModel DBInfo, long deliveryRoutingId, int assignStatus)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                bool res = DeliveryRoutingTasks.updateRouteAssignStatus(DBInfo, deliveryRoutingId, assignStatus);

                if (res)
                {
                    return true;
                }

                return false;
            }
        }



        /// <summary>
        /// return specific route associated order ids
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <returns>List<long></returns>
        public DeliveryRoutingOrdersModel getRouteOrders(DBInfoModel DBInfo, long deliveryRoutingId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                using (IDbTransaction transact = db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    return DeliveryRoutingTasks.getRouteOrders(DBInfo, deliveryRoutingId, db, transact);
                }
            }
        }

        /// <summary>
        /// accept call for delivery routing assignment from mobile client
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="hub">IHubContext</param>
        /// <returns>bool</returns>
        public bool acceptRoute(DBInfoModel DBInfo, string staff, long deliveryRoutingId, IHubContext hub)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                using (IDbTransaction transact = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    int orderStatus = (int)OrderStatusEnum.Onroad; 

                    int assignStatus = (int)DeliveryRoutingAssignStatusEnum.staffAccepted; 

                    bool res = DeliveryRoutingTasks.updateRouteStatus(DBInfo, deliveryRoutingId, orderStatus, db, transact, assignStatus);

                    if (res)
                    {
                        res = DeliveryRoutingTasks.updateRouteStaff(DBInfo, deliveryRoutingId, null, db, transact, Convert.ToInt64(staff));

                        if (res)
                        {
                            transact.Commit();

                            //string drExternalApiUrl = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drExternalApiUrl");

                            //if (!string.IsNullOrEmpty(drExternalApiUrl))
                            //{
                            //    long? modifiedDeliveryRouteStatus = null;

                            //    DeliveryRoutingTasks.sendRouteToExternalApi(DBInfo, Convert.ToInt64(deliveryRoutingId), drExternalApiUrl, out long? modifiedDeliveryRouteId, out modifiedDeliveryRouteStatus, null);

                            //    if (modifiedDeliveryRouteId != null)
                            //    {
                            //        hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(modifiedDeliveryRouteId, modifiedDeliveryRouteStatus);
                            //    }
                            //}
                        }
                    }

                    return res;
                }                
            }
        }

        /// <summary>
        /// reject call for delivery routing assignment from mobile client
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="hub">IHubContext</param>
        /// <returns>bool</returns>
        public bool rejectRoute(DBInfoModel DBInfo, string staff, long deliveryRoutingId, IHubContext hub)
        {
            bool res = false;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                using (IDbTransaction transact = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    int orderStatus = (int)OrderStatusEnum.Ready;
                    
                    int assignStatus = (int)DeliveryRoutingAssignStatusEnum.staffRejected;

                    res = DeliveryRoutingTasks.updateRouteStatus(DBInfo, deliveryRoutingId, orderStatus, db, transact, assignStatus);

                    if (res)
                    { 
                        DeliveryRoutingModel route = DeliveryRoutingTasks.getRoutes(DBInfo, deliveryRoutingId, null, db, transact).FirstOrDefault();

                        long? staffId = route.StaffId;

                        res = DeliveryRoutingTasks.updateRouteStaff(DBInfo, deliveryRoutingId, null, db, transact);

                        if(res)
                        {
                            res = DeliveryRoutingTasks.updateStaffIsAssignedToRoute(DBInfo, Convert.ToInt64(route.StaffId), false, db, transact, route.Status);

                            if (res)
                            {
                                res = DeliveryRoutingTasks.releaseStaff(DBInfo, Convert.ToInt64(staffId), db, transact);

                                if (res)
                                {
                                    res = DeliveryRoutingTasks.addStaffToRejected(DBInfo, deliveryRoutingId, Convert.ToInt64(staff), db, transact);

                                    if (res)
                                    {
                                        res = DeliveryRoutingTasks.addStaffToActiveStaffList(DBInfo, db, transact, null, staff, "last");

                                        if (res)
                                        {
                                            transact.Commit();

                                            System.Threading.Thread.Sleep(2000);

                                            bool drAutoAssignRouting = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drAutoAssignRouting");

                                            if (drAutoAssignRouting)
                                            {
                                                DeliveryRoutingTasks.asyncAssignRouteToStaff(DBInfo, Convert.ToInt64(deliveryRoutingId), hub);
                                            }

                                            return true;
                                        }
                                        else
                                        {
                                            if (transact != null)
                                            {
                                                transact.Rollback();

                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// complete call for delivery routing assignment from mobile client
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <returns>bool</returns>
        public bool completeRoute(DBInfoModel DBInfo, string staff, long deliveryRoutingId)
        {
            bool res = false;

            long? staffId = null;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                using (IDbTransaction transact = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    int orderStatus = (int)OrderStatusEnum.Complete;

                    DeliveryRoutingModel route = DeliveryRoutingTasks.getRoutes(DBInfo, deliveryRoutingId).FirstOrDefault();

                    if (route != null)
                    {
                        staffId = route.StaffId;

                        res = DeliveryRoutingTasks.updateRouteStatus(DBInfo, deliveryRoutingId, orderStatus, db, transact, null, true);

                        if (res)
                        {
                            res = DeliveryRoutingTasks.releaseStaff(DBInfo, Convert.ToInt64(staffId), db, transact);

                            if (res)
                            {
                                res = DeliveryRoutingTasks.addStaffToActiveStaffList(DBInfo, db, transact, staffId, staff, "last");

                                if (res)
                                {
                                    transact.Commit();

                                    return true;
                                }
                                else
                                {
                                    if(transact != null)
                                    {
                                        transact.Rollback();

                                        return false;
                                    }
                                }
                            }
                        }
                    }

                    return false;
                }
            }
        }

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
        public bool changeRouteStaff(DBInfoModel DBInfo, long deliveryRoutingId, long newStaffId, long? oldStaffId, out long? modifiedDeliveryRouteId, IHubContext hub)
        {
            bool res = false;

            modifiedDeliveryRouteId = null;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                using (IDbTransaction transact = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    List<long> orderIds = DeliveryRoutingTasks.getRouteOrderIds(DBInfo, deliveryRoutingId, db, transact);

                    res = DeliveryRoutingTasks.checkValidOrderStatus(DBInfo, orderIds);

                    if (res)
                    {
                        res = DeliveryRoutingTasks.updateRouteStaff(DBInfo, deliveryRoutingId, newStaffId, db, transact);

                        if (res)
                        {
                            if (oldStaffId != null && oldStaffId != newStaffId)
                            {
                                res = DeliveryRoutingTasks.releaseStaff(DBInfo, Convert.ToInt64(oldStaffId), db, transact);

                                if (res)
                                {
                                    res = DeliveryRoutingTasks.addStaffToActiveStaffList(DBInfo, db, transact, Convert.ToInt64(oldStaffId), null, "first");
                                }
                                else
                                {
                                    logger.Error("Delivery Routing: Failed to release staff " + oldStaffId);
                                }
                            }

                            transact.Commit();

                            string drExternalApiUrl = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drExternalApiUrlChangeStaff");

                            if (!string.IsNullOrEmpty(drExternalApiUrl))
                            {
                                long? modifiedDeliveryRouteStatus = null;

                                DeliveryRoutingTasks.sendRouteToExternalApi(DBInfo, deliveryRoutingId, drExternalApiUrl, out modifiedDeliveryRouteId, out modifiedDeliveryRouteStatus, Convert.ToInt64(oldStaffId));

                                if (modifiedDeliveryRouteId != null)
                                {
                                    hub.Clients.Group("DeliveryPos").DeliveryRoutingChange(modifiedDeliveryRouteId, modifiedDeliveryRouteStatus);
                                }
                            }

                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// delivery routing staff login
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="login">DeliveryRoutingStaffCredentialsModel</param>
        /// <returns>long?</returns>
        public long? staffLogin(DBInfoModel DBInfo, DeliveryRoutingStaffCredentialsModel login)
        {
            return DeliveryRoutingTasks.staffLogin(DBInfo, login);
        }

        /// <summary>
        /// return last route for specified staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <returns>DeliveryRoutingExtModel</returns>
        public StaffRoutes getLastStaffRoute(DBInfoModel DBInfo, string staff)
        {
            StaffRoutes resModel = new StaffRoutes();

            List<DeliveryRoutingLastRouteInfo> res = new List<DeliveryRoutingLastRouteInfo>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    resModel.staffRoutes = DeliveryRoutingTasks.getLastStaffRoute(DBInfo, db, staff);

                    resModel.StoreId = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drStoreId");
                }
                catch(Exception ex)
                {
                    logger.Error("Delivery Routing: Failed to get last staff route: " + ex.ToString());
                }

                //if (res != null && res.route != null)
                //{
                //    //List<long> orderIds = null;

                //    //orderIds = DeliveryRoutingTasks.getRouteOrdersDetails(DBInfo, res.route.Id, db); //getRouteOrderIds(DBInfo, res.route.Id, db);

                //    //if (orderIds != null)
                //    //{ 
                //    //    res.orderNos = orderIds.ConvertAll(x => Convert.ToString(x));
                //    //}
                //    //List<DeliveryRoutingLastRouteInfo> ordersDetails = null;

                //    res.ordersDetails = DeliveryRoutingTasks.getRouteOrdersDetails(DBInfo, res.route.Id, db); //getRouteOrderIds(DBInfo, res.route.Id, db);

                //}
            }

            return resModel;
        }

        /// <summary>
        /// return specific route associated order ids
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <returns>List<long></returns>
        public List<long> getRouteOrderIds(DBInfoModel DBInfo, long deliveryRoutingId)
        {
            return DeliveryRoutingTasks.getRouteOrderIds(DBInfo, deliveryRoutingId);
        }

        /// <summary>
        /// clock-out staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        public void ClockIn(DBInfoModel DBInfo, string staff)
        {
            PayrollNewModel payroll = GeneratePayrollModel(DBInfo, staff);

            PayrollFlows.InsertPayroll(DBInfo, payroll);
        }

        /// <summary>
        /// clock-out staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        public void ClockOut(DBInfoModel DBInfo, string staff)
        {
            StaffModels staffInfo = staffDT.GetStaffByCode(DBInfo, staff);

            if (staffInfo != null)
            {
                PayrollNewModel payroll = PayrollFlows.GetTopRowByType(DBInfo, staffInfo.Id);
                       
                if (payroll != null)
                {
                    payroll.FromPos = true;

                    payroll.DateTo = DateTime.Now;

                    PayrollFlows.Update(DBInfo, payroll);
                }
                else
                {
                    logger.Error("Failed to retrieve clockin information during clockout");
                }
            }
            else
            {
                logger.Error("Failed to retrieve staff with code " + staff);
            }
        }

        public PayrollNewModel GeneratePayrollModel(DBInfoModel DBInfo, string staff)
        {
            PayrollNewModel payroll = new PayrollNewModel();

            StaffModels staffInfo = staffDT.GetStaffByCode(DBInfo, staff);

            payroll.StaffId = staffInfo.Id;

            payroll.Identification = staffInfo.Identification;

            payroll.FromPos = true;

            payroll.DateFrom = DateTime.Now;

            long posInfoId = 0;

            try
            {
                posInfoId = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drDefaultPosInfoId");
            }
            catch(Exception)
            {
                logger.Warn("Failed to retrieve default posInfoId from configuration");

                posInfoId = 1;
            }

            payroll.PosInfoId = posInfoId;

            return payroll;
        }
    }
}
