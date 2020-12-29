using System;
using System.Collections.Generic;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System.Data.SqlClient;
using System.Data;
using log4net;
using Symposium.WebApi.DataAccess.Interfaces.DT.Delivery;
using Symposium.Models.Models.Delivery;
using AutoMapper;
using System.Linq;
using Symposium.Helpers;
using Symposium.Helpers.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using System.Net;
using System.Net.Http.Headers;
using Dapper;

namespace Symposium.WebApi.DataAccess.DT.Delivery
{
    public class DeliveryRoutingDT : IDeliveryRoutingDT
    {
        string connectionString;
        protected ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IGenericDAO<DeliveryRoutingDTO> genDeliveryRouting;
        IGenericDAO<DeliveryRoutingHistDTO> genDeliveryRoutingHist;
        IGenericDAO<OrderDTO> genOrder;
        IGenericDAO<OrderStatusDTO> genOrderStatus;
        IGenericDAO<StaffDTO> genStaff;
        IGenericDAO<InvoiceShippingDetailsDTO> genShippingDetails;
        IUsersToDatabasesXML usersToDatabases;
        IWebApiClientHelper webHlp;
        IDeliveryOrdersDT genDeliveryOrdersDT;
        IStaffDT genStaffDT;

        public DeliveryRoutingDT(
            IGenericDAO<DeliveryRoutingDTO> genDeliveryRouting, 
            IGenericDAO<DeliveryRoutingHistDTO> genDeliveryRoutingHist, 
            IGenericDAO<OrderDTO> genOrder,
            IGenericDAO<OrderStatusDTO> genOrderStatus,
            IGenericDAO<StaffDTO> genStaff,
            IGenericDAO<InvoiceShippingDetailsDTO> genShippingDetails,
            IWebApiClientHelper webHlp,
            IDeliveryOrdersDT genDeliveryOrdersDT,
            IStaffDT genStaffDT,
            IUsersToDatabasesXML usersToDatabases
            )
        {
            this.usersToDatabases = usersToDatabases;
            this.webHlp = webHlp;
            this.genDeliveryOrdersDT = genDeliveryOrdersDT;
            this.genDeliveryRouting = genDeliveryRouting;
            this.genDeliveryRoutingHist = genDeliveryRoutingHist;
            this.genStaff = genStaff;
            this.genShippingDetails = genShippingDetails;
            this.genStaffDT = genStaffDT;
            this.genOrderStatus = genOrderStatus;
            this.genOrder = genOrder;
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
            DeliveryRoutingDTO data;

            try
            { 
                data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
            }
            catch(Exception ex)
            {
                logger.Error("Failed to map route model for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return false;
            }

            if (data == null)
            {
                logger.Error("Failed to retrieve route " + deliveryRoutingId);

                transact.Rollback();

                return false;
            }

            data.Status = status;

            if (assignStatus != null)
            {
                data.AssignStatus = Convert.ToInt32(assignStatus);

                if (assignStatus == (int)DeliveryRoutingAssignStatusEnum.staffAccepted && data.AcceptDate == null)
                {
                    data.AcceptDate = DateTime.Now;
                }
            }

            if (complete != null && complete == true)
            {
                data.ReturnDate = DateTime.Now;
            }

            int res = genDeliveryRouting.Update(db, data, transact, out string err);

            if (res != 1)
            {
                logger.Info("Failed to update route " + deliveryRoutingId);

                transact.Rollback();

                return false;
            }

            return true;
        }

        public bool updateRoute(DBInfoModel DBInfo, DeliveryRoutingModel route, IDbConnection db = null, IDbTransaction transact = null)
        {
            if (route != null)
            {
                try
                {
                    if (db == null)
                    {
                        connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                        db = new SqlConnection(connectionString);
                    }

                    using (db)
                    {
                        int res = genDeliveryRouting.Update(db, Mapper.Map<DeliveryRoutingDTO>(route), transact, out string err);

                        if (res != 1)
                        {
                            logger.Error("Failed to update route " + route.Id);

                            if (transact != null)
                            {
                                transact.Rollback();
                            }

                            return false;
                        }
                    }

                    return true;
                }
                catch(Exception ex)
                {
                    logger.Error("Failed to update route " + route.Id);

                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public long? getOrderRouteId(DBInfoModel DBInfo, long orderid)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (SqlConnection db = new SqlConnection(connectionString))
            {
                OrderDTO data = Mapper.Map<OrderDTO>(genOrder.Select(db, orderid));

                return data.DeliveryRoutingId;
            }
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
            DeliveryRoutingDTO data;

            try
            {
                data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map route model for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return false;
            }

            if (data == null)
            {
                if (transact != null)
                {
                    logger.Info("Failed to retrieve route " + deliveryRoutingId);

                    transact.Rollback();
                }

                return false;
            }

            if (staffId == null && staffUsername == null)
            {
                data.StaffId = null;

                data.StaffName = null;

                data.AssignDate = null;
            }
            else
            {
                DeliveryRoutingStaffModel staff = getStaffInfo(DBInfo, staffId, staffUsername);

                if (staff == null)
                {
                    if (transact != null)
                    {
                        logger.Info("Failed to retrieve staff " + staffId != null ? staffId : staffUsername);

                        transact.Rollback();
                    }

                    return false;
                }

                data.StaffId = staff.id;

                data.StaffName = staff.name;

                if (data.AssignDate == null)
                {
                    data.AssignDate = DateTime.Now;
                }
            }

            int res = genDeliveryRouting.Update(db, data, transact, out string err);

            if (res == 1)
            {
                bool resUpd = true;

                if (data != null && data.StaffId != null)
                    resUpd = updateStaffIsAssignedToRoute(DBInfo, Convert.ToInt64(data.StaffId), true, db, transact, data.Status);

                return resUpd;
            }
            else
            {
                if (transact != null)
                {
                    logger.Info("Failed to update route " + deliveryRoutingId);

                    transact.Rollback();
                }

                return false;
            }
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
        public bool updateStaffIsAssignedToRoute(DBInfoModel DBInfo, long staffId, bool assigned, IDbConnection db = null, IDbTransaction transact = null, int? routeStatus = null )
        {
            if (db == null)
            {
                connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                db = new SqlConnection(connectionString);
            }

            StaffDTO data = genStaff.SelectFirst(db, "WHERE Id = @Id", new { Id = staffId }, transact);

            if (data == null)
            {
                logger.Error("Failed to retrieve staff " + staffId);

                if (transact != null)
                {
                    transact.Rollback();
                }

                return false;
            }

            data.isAssignedToRoute = assigned;

            if (routeStatus != null)
            {
                data.CurrentOrderStatus = routeStatus == 4 || data.isAssignedToRoute == true ? true : false;
            }

            logger.Info("updating staff " + staffId + "assigned:" + data.isAssignedToRoute + ", onroad:" + data.CurrentOrderStatus + ", routestatus:" + routeStatus);

            int res = genStaff.Update(db, data, transact, out string err);

            if (res != 1)
            {
                logger.Error("Failed to update route assign status for staff " + staffId);

                if (transact != null)
                {
                    transact.Rollback();
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// get routes with specified status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="assignStatus">int</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        public List<DeliveryRoutingModel> getRoutesByAssignStatus(DBInfoModel DBInfo, int assignStatus)
        {
            List<DeliveryRoutingModel> result = new List<DeliveryRoutingModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    result = Mapper.Map<List<DeliveryRoutingModel>>(genDeliveryRouting.Select(db, "WHERE AssignStatus = @AssignStatus", new { AssignStatus = assignStatus }));
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to map route models");

                    logger.Error(ex.ToString());

                    return null;
                }

                return result;
            }
        }

        /// <summary>
        /// get routes with specified status assigned to provided staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="assignStatus">int</param>
        /// <param name="staffId">long</param>
        /// <returns>List<DeliveryRoutingModel></returns>
        public List<DeliveryRoutingModel> getStaffRoutesByAssignStatus(DBInfoModel DBInfo, int assignStatus, long staffId)
        {
            List<DeliveryRoutingModel> result = new List<DeliveryRoutingModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    result = Mapper.Map<List<DeliveryRoutingModel>>(genDeliveryRouting.Select(db, "WHERE AssignStatus = @AssignStatus AND StaffId = @StaffId", new { AssignStatus = assignStatus, StaffId = staffId }));
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to map route models");

                    logger.Error(ex.ToString());

                    return null;
                }

                return result;
            }
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
            modifiedDeliveryRouteId = null;

            modifiedDeliveryRouteStatus = null;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                DeliveryRoutingDTO data;

                try
                {
                    data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }));
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to map route model for route " + deliveryRoutingId);

                    logger.Error(ex.ToString());

                    return;
                }

                if (data == null)
                {
                    return;
                }

                HttpContent content;

                string json;

                if (oldStaffId != null) 
                { 
                    DeliveryRoutingChangeStaffModel postModel = new DeliveryRoutingChangeStaffModel();

                    postModel.Id = deliveryRoutingId;

                    postModel.OldStaffId = Convert.ToInt64(oldStaffId);

                    postModel.Routing = mapDeliveryRoutingModelToRouting3Model(DBInfo, data);

                    json = JsonConvert.SerializeObject(postModel);
                }
                else
                {
                    Routing3Model postModel3 = mapDeliveryRoutingModelToRouting3Model(DBInfo, data);

                    json = JsonConvert.SerializeObject(postModel3);
                }

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

                data.Failure3th = false;

                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

                request.Content = content;

                request.Headers.Add("Authorization", "Basic " + authorizationString);

                logger.Info("Sending route info to external api for route " + deliveryRoutingId);

                //logger.Debug("Send route model: " + json);

                var t = PostURI(client, request);

                if (!t.Result.IsSuccessStatusCode)
                {
                    logger.Error("Send route info to external api failed for route " + deliveryRoutingId);

                    logger.Error("Send route model: " + json);

                    logger.Error("External api url: " + drExternalApiUrl);

                    logger.Error(t.Result);

                    data.Failure3th = true;

                    int resUpd = genDeliveryRouting.Update(db, data);

                    if (resUpd != 1)
                    {
                        logger.Error("Failed to update Failure3th field for route " + deliveryRoutingId);
                    }
                
                    // SIGNALR DeliveryPos:DeliveryRoutingChange
                    modifiedDeliveryRouteId = data.Id;

                    modifiedDeliveryRouteStatus = data.Status;
                }
                else
                {
                    logger.Info("Send route info to external api success for route " + deliveryRoutingId);
                }
            }
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
        /// maps DeliveryRoutingModel fields to Routing3Model fields
        /// </summary>
        /// <param name="data">DeliveryRoutingDTO</param>
        /// <returns>Routing3Model</returns>
        public Routing3Model mapDeliveryRoutingModelToRouting3Model(DBInfoModel DBInfo, DeliveryRoutingDTO data)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            List<OrderDTO> orders = new List<OrderDTO>();

            //List<long> orderIds = new List<long>();

            //List<string> orderIdStrings = new List<string>();

            Dictionary<string, string> orderIds = new Dictionary<string, string>();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    orders = Mapper.Map<List<OrderDTO>>(genOrder.Select(db, "WHERE DeliveryRoutingId = @DeliveryRoutingId", new { DeliveryRoutingId = data.Id }));
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to map order models for route " + data.Id);

                    logger.Error(ex.ToString());

                    return null;
                }
            }

            //orderIds = orders.Select(r => r.Id).ToList();

            //orderIdStrings = orderIds.ConvertAll(x => Convert.ToString(x));

            foreach (OrderDTO order in orders)
            {
                orderIds.Add(Convert.ToString(order.Id), Convert.ToString(order.OrderNo));
            }

            Routing3Model res = new Routing3Model();
            res.Id = data.Id;
            res.Orders = orderIds;
            res.StaffId = data.StaffId;
            res.Status = data.Status;
            res.AssignStatus = data.AssignStatus;
            res.CreateDate = data.CreateDate;
            res.StoreId = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drStoreId");
            return res;
        }

        /// <summary>
        /// get selected keys from orders.extobj json string
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderId">long?</param>
        /// <param name="keys"List<string>></param>
        /// <returns>Dictionary<string, string></returns>
        public Dictionary<string, string> GetOrderExtElements(DBInfoModel DBInfo, long orderId, List<string> keys)
        {
            if (keys != null && keys.Count > 0)
            {
                try
                {
                    connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                    using (IDbConnection db = new SqlConnection(connectionString))
                    {
                        OrderDTO order = Mapper.Map<OrderDTO>(genOrder.SelectFirst(db, "WHERE id = @id", new { id = orderId }));

                        if (order != null)
                        {
                            string query = @"             
	                            SELECT 
                                    [name] AS [key], 
                                    StringValue AS [value]
                                FROM 
                                    dbo.parseJSON('" + order.ExtObj + @"')
                                WHERE 
                                    [name] IN ('" + keys.Aggregate((i, j) => i + "','" + j) + @"')";

                            var result = db.Query(query).ToList();

                            return db.Query(query).ToDictionary(
                                row => (string)row.key,
                                row => (string)row.value); ;
                        }
                    }
                }
                catch(Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }

            return null;
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
            if (db == null)
            {
                connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                db = new SqlConnection(connectionString);
            }

            DeliveryRoutingDTO data;

            try
            {
                data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map route model for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return false;
            }

            if (data == null)
            {
                if (transact != null)
                {
                    logger.Error("Failed to retrieve route " + deliveryRoutingId);

                    transact.Rollback();
                }

                return false;
            }

            DeliveryRoutingStaffModel staff = getStaffInfo(DBInfo, null, Convert.ToInt64(staffId), db, transact);

            if (staff == null)
            {
                if (transact != null)
                {
                    logger.Error("Failed to retrieve staff " + staffId);

                    transact.Rollback();
                }

                return false;
            }

            if (string.IsNullOrEmpty(data.RejectedNames))
            {
                data.RejectedNames = staff.name;
            }
            else
            {
                string[] vals = new string[] { data.RejectedNames, staff.name };

                data.RejectedNames = string.Join(";", vals);
            }
            
            int res = genDeliveryRouting.Update(db, data, transact, out string err);

            if (res == 1)
            {
                return true;
            }
            else
            {
                logger.Error("Failed to update route " + deliveryRoutingId);

                if (transact != null)
                {
                    transact.Rollback();
                }

                return false;
            }
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
            if (db == null)
            {
                connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                db = new SqlConnection(connectionString);
            }

            int res = 0;

            DeliveryRoutingDTO data;

            long? staffId;

            try
            {
                data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map route model for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return false;
            }

            if (data == null)
            {
                logger.Error("Failed to retrieve route " + deliveryRoutingId);

                if (transact != null)
                {
                    transact.Rollback();
                }

                return false;
            }

            staffId = data.StaffId;

            res = genDeliveryRouting.Delete(db, transact, data);

            if (res == 1)
            {
                if (staffId != null && staffId != 0)
                {
                    bool resRelease = releaseStaff(DBInfo, Convert.ToInt64(staffId), db, transact);

                    if (!resRelease)
                    {
                        logger.Error("Delivery Routing: Failed to release staff " + staffId);

                        if (transact != null)
                        {
                            transact.Rollback();
                        }

                        return false;
                    }
                }

                return true;
            }
            else
            {
                logger.Error("Failed to delete route " + deliveryRoutingId);

                if (transact != null)
                {
                    transact.Rollback();
                }

                return false;
            }
        }

        /// <summary>
        /// check if specified route has no orders
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool isRouteEmpty(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db, IDbTransaction transact)
        {
            //bool result = true;

            //result = result && isRouteOrdersEmpty(DBInfo, deliveryRoutingId, db, transact);

            //if(result)
            //{
            //    result = result && !isRouteLinkedToOrders(DBInfo, deliveryRoutingId, db, transact);
            //}

            return !isRouteLinkedToOrders(DBInfo, deliveryRoutingId, db, transact);
        }

        /// <summary>
        /// check if delivery route counter is 0
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool isRouteOrdersEmpty(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db, IDbTransaction transact)
        {
            bool result = true;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            DeliveryRoutingModel data;

            try
            {
                data = Mapper.Map<DeliveryRoutingModel>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map route model for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return false;
            }

            if (data == null)
            {
                return true;
            }

            if (data.Orders != 0)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// check if specified route id has reference in any order
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool isRouteLinkedToOrders(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db, IDbTransaction transact)
        {
            bool result = true;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            List<OrderDTO> data;

            try
            {
                data = Mapper.Map<List<OrderDTO>>(genOrder.Select(db, "WHERE DeliveryRoutingId = @DeliveryRoutingId", new { DeliveryRoutingId = deliveryRoutingId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map order models for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return false;
            }

            if (data == null)
            {
                return false;
            }
            if (data.Count == 0)
            {
                result = false;
            }

            return result;
        }
        public int GetRouteOrdersCount(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db, IDbTransaction transact)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            List<OrderDTO> data;

            try
            {
                data = Mapper.Map<List<OrderDTO>>(genOrder.Select(db, "WHERE DeliveryRoutingId = @DeliveryRoutingId", new { DeliveryRoutingId = deliveryRoutingId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map order models for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return 0;
            }

            if (data == null)
            {
                return 0;
            }
            if (data.Count == 0)
            {
                return 0;
            }

            return data.Count;
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
            bool res = false;

            modifiedDeliveryRouteIds = new List<long>();

            deletedDeliveryRouteIds = new List<long>();

            assignDeliveryRouteIds = new List<long>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                using (IDbTransaction transact = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    DeliveryRoutingDTO data;

                    long? tempStaff = null;

                    if (deliveryRoutingId != null)
                    {
                        try
                        {
                            data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Failed to map route model for route " + deliveryRoutingId);

                            logger.Error(ex.ToString());

                            return false;
                        }

                        tempStaff = data.StaffId;
                    }
                    else
                    {
                        data = null;
                    }

                    res = updateOrdersDeliveryRoutingId(DBInfo, orderIds, deliveryRoutingId, db, transact, out modifiedDeliveryRouteIds);

                    //deliveryRoutingId = upsertRouteOrders(DBInfo, deliveryRoutingId, orderIds.Count, db, transact, out deletedDeliveryRouteIds, null, tempStaff);

                    if (res) // (deliveryRoutingId != null)
                    {
                        //res = updateOrdersDeliveryRoutingId(DBInfo, orderIds, deliveryRoutingId, db, transact, out modifiedDeliveryRouteIds);

                        deliveryRoutingId = upsertRouteOrders(DBInfo, deliveryRoutingId, orderIds.Count, db, transact, out deletedDeliveryRouteIds, null, tempStaff);

                        if (deliveryRoutingId != null) // (res)
                        {
                            try
                            {
                                data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
                            }
                            catch (Exception ex)
                            {
                                //logger.Error("Failed to map route model for route " + deliveryRoutingId);

                                //logger.Error(ex.ToString());

                                //return false;
                            }
                            if (data == null)
                            {
                                logger.Error("Failed to retrieve route " + deliveryRoutingId);

                                transact.Rollback();

                                return false;
                            }

                            if (data.AssignStatus == (int)DeliveryRoutingAssignStatusEnum.pendingResponse)
                            {
                                // SIGNALR mobile:DeliveryManAssign
                                assignDeliveryRouteIds.Add(Convert.ToInt64(deliveryRoutingId));
                            }

                            transact.Commit();

                            return true;
                        }
                        else
                        {
                            logger.Error("Failed to update orders delivery routing id for route " + deliveryRoutingId);

                            transact.Rollback();

                            return false;
                        }
                    }

                    return false;
                }
            }
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
            bool res = false;

            modifiedDeliveryRouteIds =  new List<long>();

            deletedDeliveryRouteIds = new List<long>();

            assignDeliveryRouteIds = new List<long>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                using (IDbTransaction transact = db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    res = updateOrdersDeliveryRoutingId(DBInfo, orderIds, null, db, transact, out modifiedDeliveryRouteIds);

                    //deliveryRoutingId = upsertRouteOrders(DBInfo, deliveryRoutingId, orderIds.Count * -1, db, transact, out deletedDeliveryRouteIds);

                    if (res) // (deliveryRoutingId != null)
                    {
                        //res = updateOrdersDeliveryRoutingId(DBInfo, orderIds, null, db, transact, out modifiedDeliveryRouteIds);

                        deliveryRoutingId = upsertRouteOrders(DBInfo, deliveryRoutingId, orderIds.Count * -1, db, transact, out deletedDeliveryRouteIds);

                        if (deliveryRoutingId != null) // (res)
                        {
                            DeliveryRoutingDTO data = null;

                            try
                            {
                                //data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
                                bool hasOrders = isRouteLinkedToOrders(DBInfo, Convert.ToInt64(deliveryRoutingId), db, transact);

                                if ((deletedDeliveryRouteIds != null && deletedDeliveryRouteIds.Count > 0) || !hasOrders)
                                {
                                    if (deletedDeliveryRouteIds == null || deletedDeliveryRouteIds.Count == 0)
                                    {
                                        deletedDeliveryRouteIds.Add(Convert.ToInt64(deliveryRoutingId));
                                    }

                                    try
                                    {
                                        data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
                                    }
                                    catch (Exception ex)
                                    {
                                        //logger.Error("Failed to map route model for route " + deliveryRoutingId);

                                        //logger.Error(ex.ToString());

                                        //return false;
                                    }

                                    if (data != null)
                                    {
                                        //if (data.StaffId != null)
                                        //{
                                        //    res = releaseStaff(DBInfo, Convert.ToInt64(data.StaffId), db, transact);
                                        //}

                                        if (data.AssignStatus == (int)DeliveryRoutingAssignStatusEnum.pendingResponse)
                                        {
                                            // SIGNALR mobile:DeliveryManAssign
                                            assignDeliveryRouteIds.Add(Convert.ToInt64(deliveryRoutingId));
                                        }

                                        int deleteRes = genDeliveryRouting.Delete(db, transact, data);

                                        if (deleteRes != 1)
                                        {
                                            logger.Error("Failed to delete route " + deliveryRoutingId);
                                        }
                                        else
                                        {
                                            data = null;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error("Failed to map route model for route " + deliveryRoutingId);

                                logger.Error(ex.ToString());

                                return false;
                            }

                            transact.Commit();

                            return true;
                        }
                        else
                        {
                            //logger.Error("Failed to update orders delivery routing id for route " + deliveryRoutingId);

                            //transact.Rollback();

                            //return false;

                            //route has been removed
                            transact.Commit();
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// get appropriate assign status for route depending on configuration
        /// </summary>
        /// <param name="staffId">long?</param>
        /// <returns></returns>
        public int getAppropriateAssignStatus(long? staffId = null)
        {
            int assignStatus = (int)DeliveryRoutingAssignStatusEnum.searchingStaff;

            if (staffId != null)
            {
                if (MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drActivateMobileApp"))
                {
                    assignStatus = (int)DeliveryRoutingAssignStatusEnum.pendingResponse;
                }
                else
                {
                    assignStatus = (int)DeliveryRoutingAssignStatusEnum.staffAccepted;
                }
            }

            return assignStatus;
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
        public long? upsertRouteOrders(DBInfoModel DBInfo, long? deliveryRoutingId, int orderOffset, IDbConnection db, IDbTransaction transact, out List<long> deletedDeliveryRouteIds, int? status = null, long? staffId = null)
        {
            long? returnId = null;

            string err = null;

            deletedDeliveryRouteIds = new List<long>();

            DeliveryRoutingStaffModel staff = new DeliveryRoutingStaffModel();

            int assignStatus = getAppropriateAssignStatus(staffId);

            bool isStaffProvided = false;

            if (staffId != null)
            {
                staff = getStaffInfo(DBInfo, Convert.ToInt64(staffId));

                isStaffProvided = true;
            }

            if (deliveryRoutingId == null || deliveryRoutingId == 0)
            {
                DeliveryRoutingModel data = new DeliveryRoutingModel();

                data.CreateDate = DateTime.Now;

                if (data.AssignStatus == null || data.AssignStatus == (int)DeliveryRoutingAssignStatusEnum.searchingStaff)
                {
                    data.AssignStatus = assignStatus;
                }

                if (status != null)
                {
                    data.Status = Convert.ToInt32(status);
                }

                if (isStaffProvided)
                {
                    data.StaffId = staff.id;

                    data.StaffName = staff.name;

                    if (data.AssignDate == null)
                    {
                        data.AssignDate = DateTime.Now; 
                    }
                }
                else
                {
                        data.StaffId = null;

                        data.StaffName = null;

                        data.AssignDate = null;
                }

                returnId = genDeliveryRouting.Insert(db, Mapper.Map<DeliveryRoutingDTO>(data), transact, out err);

                return returnId;
            }
            else
            {
                DeliveryRoutingDTO data = null;

                try
                {
                    data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
                }
                catch (Exception ex)
                {
                    //logger.Error("Failed to map route model for route " + deliveryRoutingId);

                    //logger.Error(ex.ToString());

                    //return null;
                }

                if (data == null)
                {
                    //logger.Error("Failed to retrieve route " + deliveryRoutingId);

                    return null;
                }

                if (status != null)
                {
                    data.Status = Convert.ToInt32(status);
                }

                if (data.StaffId != null)
                {
                    staff = getStaffInfo(DBInfo, Convert.ToInt64(data.StaffId));

                    isStaffProvided = true;
                }

                data.Orders = GetRouteOrdersCount(DBInfo, Convert.ToInt64(deliveryRoutingId), db, transact); //+= orderOffset;

                int res;

                if (data.Orders <= 0)
                {
                    res = 1;
                    //res = genDeliveryRouting.Delete(db, transact, data);

                    //if (res != 1)
                    //{
                    //    logger.Error("Failed to delete route " + deliveryRoutingId);
                    //}

                    // SIGNALR DeliveryPos:DeliveryRoutingDelete 
                    deletedDeliveryRouteIds.Add(data.Id);
                }
                else
                {
                    if (data.AssignStatus == null || data.AssignStatus == (int)DeliveryRoutingAssignStatusEnum.searchingStaff)
                    { 
                        data.AssignStatus = assignStatus;
                    }

                    if (status != null)
                    {
                        data.Status = Convert.ToInt32(status);
                    }

                    if (isStaffProvided)
                    {
                        data.StaffId = staff.id;

                        data.StaffName = staff.name;

                        if (data.AssignDate == null)
                        { 
                            data.AssignDate = DateTime.Now;
                        }
                        
                    }
                    else
                    {
                        data.StaffId = null;

                        data.StaffName = null;

                        data.AssignDate = null;
                    }

                    res = genDeliveryRouting.Update(db, data, transact, out err);

                    if (res != 1)
                    {
                        logger.Error("Failed to update route " + deliveryRoutingId);
                    }
                }                

                if (res == 1)
                {
                    return deliveryRoutingId;
                }
                else
                {
                    return null;
                }
            }
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
            List<DeliveryRoutingModel> result = new List<DeliveryRoutingModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            if (db == null)
            {
                db = new SqlConnection(connectionString);
            }
        
            try
            {
                if (deliveryRoutingId == null && status == null)
                {
                    result = Mapper.Map<List<DeliveryRoutingModel>>(genDeliveryRouting.Select(db, null, transact).OrderBy(x => x.CreateDate));
                }
                else if (status == null)
                {
                    result = Mapper.Map<List<DeliveryRoutingModel>>(genDeliveryRouting.Select(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact).OrderBy(x => x.CreateDate));
                }
                else if (deliveryRoutingId == null)
                {
                    result = Mapper.Map<List<DeliveryRoutingModel>>(genDeliveryRouting.Select(db, "WHERE Status = @Status", new { Status = status }, transact).OrderBy(x => x.CreateDate));
                }
                else
                {
                    result = Mapper.Map<List<DeliveryRoutingModel>>(genDeliveryRouting.Select(db, "WHERE Status = @Status AND Id = @Id", new { Status = status, Id = deliveryRoutingId }, transact).OrderBy(x => x.CreateDate));
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map route model");

                logger.Error(ex.ToString());

                return null;
            }

            return result;
        }

        /// <summary>
        /// get routes by ids provided
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="routeIds">List<long></param>
        /// <returns>List<DeliveryRoutingModel></returns>
        public List<DeliveryRoutingModel> getRoutes(DBInfoModel DBInfo, List<long> routeIds)
        {
            List<DeliveryRoutingModel> result = new List<DeliveryRoutingModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (long deliveryRoutingId in routeIds)
                {
                    DeliveryRoutingModel res;

                    try
                    {
                        res = Mapper.Map<DeliveryRoutingModel>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }));
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Failed to map route model for route " + deliveryRoutingId);

                        logger.Error(ex.ToString());

                        return null;
                    }

                    if (res != null)
                    {
                        result.Add(res);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// get route and associated orders data
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>DeliveryRoutingOrdersModel</returns>
        public DeliveryRoutingOrdersModel getRouteOrders(DBInfoModel DBInfo, long deliveryRoutingId, IDbConnection db = null, IDbTransaction transact = null)
        {
            DeliveryRoutingOrdersModel result = new DeliveryRoutingOrdersModel();

            if (db == null)
            {
                connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                db = new SqlConnection(connectionString);
            }

            try
            {
                result.DeliveryRouting = Mapper.Map<DeliveryRoutingModel>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map route model for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return null;
            }

            if (result.DeliveryRouting == null)
            {
                logger.Error("Failed to retrieve route " + deliveryRoutingId);

                return null;
            }

            try
            {
                result.OrdersList = Mapper.Map<List<OrderModel>>(genOrder.Select(db, "WHERE deliveryRoutingId = @deliveryRoutingId", new { deliveryRoutingId = deliveryRoutingId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map order models for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return null;
            }

            if (result.OrdersList == null || result.OrdersList.Count == 0)
            {
                logger.Error("Failed to retrieve orders for route " + deliveryRoutingId);

                return null;
            }

            return result;
        }

        public InvoiceShippingDetailsExtModel GetOrderCustomer(DBInfoModel DBInfo, long deliveryRoutingId, long orderId, IDbConnection db)
        {
            InvoiceShippingDetailsExtModel res = new InvoiceShippingDetailsExtModel();

            string queryCustomers = @"
                        SELECT *, '' AS bell FROM InvoiceShippingDetails WHERE InvoicesId IN (
                        SELECT id FROM invoices WHERE id IN (
                        SELECT InvoicesId FROM orderdetailinvoices WHERE orderid IN (
                        SELECT id FROM [order] o WHERE o.DeliveryRoutingId = @routingId AND o.Id = @orderId)))";

            try
            {
                //InvoiceShippingDetailsModel response = Mapper.Map<InvoiceShippingDetailsModel>(genShippingDetails.Select(queryCustomers, new { routingId = deliveryRoutingId, orderId = orderId }, db).FirstOrDefault());

                //res
                //res.bell = "";
                return Mapper.Map<InvoiceShippingDetailsExtModel>(genShippingDetails.Select(queryCustomers, new { routingId = deliveryRoutingId, orderId = orderId }, db).FirstOrDefault());

            }
            catch (Exception ex)
            {
                logger.Error("Failed to map route model");

                logger.Error(ex.ToString());

                return null;
            }
        }

        /// <summary>
        /// get route by id provided
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <returns>DeliveryRoutingModel</returns>
        public DeliveryRoutingModel getRoute(DBInfoModel DBInfo, long deliveryRoutingId)
        {
            DeliveryRoutingModel result;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    result = Mapper.Map<DeliveryRoutingModel>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }));
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to map route model for route " + deliveryRoutingId);

                    logger.Error(ex.ToString());

                    return null;
                }
            }

            return result;
        }

        /// <summary>
        /// modify specific route assign status
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="assignStatus">int</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool updateRouteAssignStatus(DBInfoModel DBInfo, long deliveryRoutingId, int assignStatus, IDbConnection db = null, IDbTransaction transact = null)
        {
            if (db == null)
            {
                connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                db = new SqlConnection(connectionString);
            }

            DeliveryRoutingDTO data;

            try
            {
                data = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map route model for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return false;
            }

            if (data == null)
            {
                logger.Error("Failed to retrieve route " + deliveryRoutingId);

                return false;
            }

            data.AssignStatus = assignStatus;

            if (assignStatus == 2)
            {
                data.AcceptDate = DateTime.Now;
            }
            else
            {
                data.AcceptDate = null;
            }


            int res = genDeliveryRouting.Update(db, data, transact, out string err);

            if (res != 1)
            {
                logger.Error("Failed to update route " + deliveryRoutingId);

                return false;
            }

            return true;
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
            bool res = false;

            if (db == null)
            {
                connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                db = new SqlConnection(connectionString);
            }

            foreach (long orderId in orderIds)
            {
                res = updateOrderStatus(orderId, status, db, transact);

                if(!res)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// modify provided order status
        /// </summary>
        /// <param name="orderId">long</param>
        /// <param name="status">int</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool updateOrderStatus(long orderId, int status, IDbConnection db, IDbTransaction transact = null)
        {
            int res = 0;

            OrderStatusDTO data = Mapper.Map<OrderStatusDTO>(genOrderStatus.SelectFirst(db, "WHERE OrderId = @OrderId", new { OrderId = orderId }, transact));

            data.Status = (OrderStatusEnum)status;

            res = genOrderStatus.Update(db, data, transact, out string err);

            if (res != 1)
            {
                logger.Error("Failed to update order " + orderId);

                if (transact != null)
                {
                    transact.Rollback();
                }

                return false;
            }

            return true;
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
            bool res = false;

            modifiedDeliveryRouteIds = new List<long>();

            foreach (long orderId in orderIds)
            {
                long? modifiedDeliveryRouteId = null;

                res = updateOrderDeliveryRoutingId(DBInfo, orderId, deliveryRoutingId, db, transact, out modifiedDeliveryRouteId);

                if (!res)
                {
                    return false;
                }

                if (modifiedDeliveryRouteId != null)
                { 
                    modifiedDeliveryRouteIds.Add(Convert.ToInt64(modifiedDeliveryRouteId));
                }
            }

            return true;
        }

        /// <summary>
        /// update routing id reference to provided order
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderId">long</param>
        /// <param name="deliveryRoutingId">long?</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <param name="modifiedDeliveryRouteId">out long?</param>
        /// <returns>bool</returns>
        public bool updateOrderDeliveryRoutingId(DBInfoModel DBInfo, long orderId, long? deliveryRoutingId, IDbConnection db, IDbTransaction transact, out long? modifiedDeliveryRouteId)
        {
            bool res = false;

            long? oldDeliveryRoutingId = null;

            modifiedDeliveryRouteId = null;

            OrderDTO data;

            try
            {
                data = Mapper.Map<OrderDTO>(genOrder.SelectFirst(db, "WHERE Id = @Id", new { Id = orderId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map order model for order " + orderId);

                logger.Error(ex.ToString());

                return false;
            }

            if (data == null)
            {
                logger.Error("Failed to retrieve order " + orderId);

                if (transact != null)
                {
                    transact.Rollback();
                }

                return false;
            }

            oldDeliveryRoutingId = data.DeliveryRoutingId;

            if (oldDeliveryRoutingId != null)
            {
                //res = updateDeliveryRoutingOrders(Convert.ToInt64(oldDeliveryRoutingId), -1, db, transact);

                //if(!res)
                //{
                //    transact.Rollback();

                //    return false;
                //}

                modifiedDeliveryRouteId = oldDeliveryRoutingId;
            }

            if (deliveryRoutingId != null)
            {
                res = updateDeliveryRoutingOrders(Convert.ToInt64(deliveryRoutingId), 1, db, transact);

                if (!res)
                {
                    if (transact != null)
                    {
                        transact.Rollback();
                    }

                    return false;
                }
            }

            data.DeliveryRoutingId = deliveryRoutingId;

            int resUpd = genOrder.Update(db, data, transact, out string err);

            if (resUpd != 1)
            {
                logger.Error("Failed to update order " + orderId);

                if (transact != null)
                {
                    transact.Rollback();
                }

                return false;
            }

            if (oldDeliveryRoutingId != null && oldDeliveryRoutingId != deliveryRoutingId)
            {
                res = isRouteEmpty(DBInfo, Convert.ToInt64(oldDeliveryRoutingId), db, transact);
            }
            else
            {
                res = false;
            }

            if(oldDeliveryRoutingId != null && res)
            {
                res = deleteRoute(DBInfo, Convert.ToInt64(oldDeliveryRoutingId), db, transact);

                if (!res)
                {
                    logger.Error("Failed to delete route " + oldDeliveryRoutingId);

                    if (transact != null)
                    { 
                        transact.Rollback();
                    }

                    return false;
                }
            }

            return true;
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
            bool res = true;

            if (db == null)
            {
                connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                db = new SqlConnection(connectionString);
            }

            StaffDTO staff = genStaff.SelectFirst(db, "WHERE Id = @Id", new { Id = staffId }, transact);

            if (staff == null)
            {
                logger.Error("Delivery Routing: Failed to release staff " + staffId);

                return false;
            }

            staff.isAssignedToRoute = false;

            staff.CurrentOrderStatus = false;

            staff.StatusTimeChange = DateTime.Now;
            
            logger.Info("releasing staff " + staffId + "assigned:" + staff.isAssignedToRoute + ", onroad:" + staff.CurrentOrderStatus);

            int resUpd = genStaff.Update(db, staff, transact, out string err);

            if (resUpd != 1)
            {
                logger.Error("Delivery Routing: Failed to update staff release changes for staff " + staffId);

                return false;
            }

            return true;
        }

        /// <summary>
        /// get staff data based on id or username
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staffId">long?</param>
        /// <param name="staffUsername">long?</param>
        /// <returns>DeliveryRoutingStaffModel</returns>
        public DeliveryRoutingStaffModel getStaffInfo(DBInfoModel DBInfo, long? staffId = null, long? staffUsername = null, IDbConnection db = null, IDbTransaction transact = null)
        {
            DeliveryRoutingStaffModel res = new DeliveryRoutingStaffModel();

            if (db == null)
            {
                connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

                db = new SqlConnection(connectionString);
            }

            StaffDTO data;
                
            if (staffId != null)
            {
                data= genStaff.SelectFirst(db, "WHERE Id = @Id", new { Id = staffId }, transact);
            }
            else if (staffUsername != null)
            {
                data = genStaff.SelectFirst(db, "WHERE Code = @Code", new { Code = Convert.ToString(staffUsername) }, transact);
            }
            else
            {
                data = null;
            }

            if (data != null)
            {
                res.id = data.Id;

                res.name = data.LastName + " " + data.FirstName;
            }

            return res;
        }

        /// <summary>
        /// update delivery routing orders counter
        /// </summary>
        /// <param name="deliveryRoutingId">long</param>
        /// <param name="orders">int</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>bool</returns>
        public bool updateDeliveryRoutingOrders(long deliveryRoutingId, int orders, IDbConnection db, IDbTransaction transact)
        {
            int result = 0;

            string err = null;

            DeliveryRoutingDTO deliveryRouting;

            try
            {
                deliveryRouting = Mapper.Map<DeliveryRoutingDTO>(genDeliveryRouting.SelectFirst(db, "WHERE Id = @Id", new { Id = deliveryRoutingId }, transact));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to map route model for route " + deliveryRoutingId);

                logger.Error(ex.ToString());

                return false;
            }

            deliveryRouting.Orders += orders;

            result = genDeliveryRouting.Update(db, deliveryRouting, transact, out err);

            if (result == 1)
            {
                return true;
            }
            else
            {
                logger.Error("Failed to update route " + deliveryRoutingId);

                return false;
            }
        }

        /// <summary>
        /// check provided orders for appropriate status when creating route or modifing staff 
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="orderIds">List<long></param>
        /// <returns>bool</returns>
        public bool checkValidOrderStatus(DBInfoModel DBInfo, List<long> orderIds)
        {
            if (orderIds == null)
            {
                return false;
            }

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {

                foreach (long orderId in orderIds)
                {
                    OrderStatusDTO data = Mapper.Map<OrderStatusDTO>(genOrderStatus.SelectFirst(db, "WHERE OrderId = @OrderId ORDER BY id desc", new { OrderId = orderId }));

                    if (data == null || (data.Status != OrderStatusEnum.Ready && data.Status != OrderStatusEnum.Onroad))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// task to move delivery routing records to hist table at end of day
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        public void moveDeliveryRoutingToHist(DBInfoModel DBInfo)
        {
            List<DeliveryRoutingHistModel> deliveryRouting;

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    deliveryRouting = Mapper.Map<List<DeliveryRoutingHistModel>>(genDeliveryRouting.Select(db));
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to map route models");

                    logger.Error(ex.ToString());

                    return;
                }

                using (IDbTransaction transact = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    long res = 0;

                    foreach (DeliveryRoutingHistModel row in deliveryRouting)
                    {
                        row.nYear = DateTime.Now.Year;

                        res = genDeliveryRoutingHist.Insert(db, Mapper.Map<DeliveryRoutingHistDTO>(row), transact, out string err);

                        if (res == -1 || res == 0)
                        {
                            logger.Error("Failed to insert record to delivery routing hist table");

                            transact.Rollback();
                        }

                        res = genDeliveryRouting.Delete(db, transact, Mapper.Map<DeliveryRoutingDTO>(row));

                        if (res != 1)
                        {
                            logger.Error("Failed to delete record from delivery routing table");

                            transact.Rollback();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// return last route for specified staff
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="staff">string</param>
        /// <returns>DeliveryRoutingExtModel</returns>
        public List<DeliveryRoutingLastRouteInfo> getLastStaffRoute(IDbConnection db, string staff)
        {
            List<DeliveryRoutingLastRouteInfo> res = new List<DeliveryRoutingLastRouteInfo>();
            //DeliveryRoutingModel res = new DeliveryRoutingModel();

            if (!string.IsNullOrWhiteSpace(staff))
            {
                StaffModels staffData = new StaffModels();

                string queryStaff = @"SELECT * FROM staff WHERE code = @staffCode";

                staffData = Mapper.Map<StaffModels>(genStaff.Select(queryStaff, new { staffCode = staff }, db).FirstOrDefault());

                if (staffData != null)
                {
                    string queryDeliveryRouting = @"
                        SELECT * 
                        FROM DeliveryRouting 
                        WHERE 
                            StaffId = @StaffId AND 
                            AssignStatus IN ( 1, 2 ) AND 
                            Status IN ( 3, 4 ) AND 
                            CreateDate >= DATEADD(HH, -20, GETDATE()) 
                        ORDER BY AssignDate DESC";

                    List<DeliveryRoutingModel> routes;

                    try
                    {
                        routes = Mapper.Map<List<DeliveryRoutingModel>>(genDeliveryRouting.Select(queryDeliveryRouting, new { StaffId = staffData.Id }, db));
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Failed to map route model");

                        logger.Error(ex.ToString());

                        return null;
                    }

                    foreach (DeliveryRoutingModel route in routes)
                    {
                        DeliveryRoutingLastRouteInfo routeExt = new DeliveryRoutingLastRouteInfo();

                        routeExt.route = route;

                        res.Add(routeExt);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// get list of active staff from database
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <returns>List<StaffDeliveryModel></returns>
        public List<StaffDeliveryModel> getActiveStaffList(DBInfoModel DBInfo)
        {
            //return genDeliveryOrdersDT.GetDeliveryStaffsOnly(DBInfo);

            return genDeliveryOrdersDT.GetAvailableDeliveryStaffs(DBInfo);
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
            StaffDTO stf;

            if (staffId != null)
            {
                stf = genStaff.SelectFirst(db, "WHERE Id = @Id", new { Id = staffId }, transact);
            }
            else if (staffUsername != null)
            {
                stf = genStaff.SelectFirst(db, "WHERE Code = @Code", new { Code = staffUsername }, transact);
            }
            else
            {
                stf = null;
            }            

            if (stf != null)
            { 
                if (position == "last")
                {
                    stf.StatusTimeChange = DateTime.Now.AddHours(2); //.Date;
                }
                else if (position == "first")
                {
                    stf.StatusTimeChange = DateTime.Now;
                }

                stf.CurrentOrderStatus = false;

                stf.isAssignedToRoute = false;

                logger.Info("updating active staff list for staff " + staffId + "assigned:" + stf.isAssignedToRoute + ", onroad:" + stf.CurrentOrderStatus);

                int res = genStaff.Update(db, stf, transact, out string err);

                if (res == 1)
                {
                    return true;
                }
            }

            transact.Rollback();

            return false;
        }

        /// <summary>
        /// delivery routing staff login
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="login">DeliveryRoutingStaffCredentialsModel</param>
        /// <returns>long?</returns>
        public long? staffLogin(DBInfoModel DBInfo, DeliveryRoutingStaffCredentialsModel login)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                StaffDTO data;

                try
                {
                    data = genStaff.SelectFirst(db, "WHERE Code=@username and Password=@password", new { username = login.username, password = login.password });
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to map staff model for staff " + login.username);

                    logger.Error(ex.ToString());

                    return null;
                }

                if (data == null)
                {
                    throw new BusinessException("Staff login failed");
                }

                return data.Id;
            }
        }
    }
}
