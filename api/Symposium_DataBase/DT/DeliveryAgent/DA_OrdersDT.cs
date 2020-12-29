using Dapper;
using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_OrdersDT : IDA_OrdersDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<DA_OrdersDTO> daOrdersDao;
        IGenericDAO<DA_OrderDetailsDTO> daOrderDetailsDao;
        IGenericDAO<DA_OrderDetailsExtrasDTO> daOrderDetailsExtrasDao;
        LocalConfigurationHelper configHlp;
        IDA_OrderStatusDT orderStatusDt;
        IDA_LoyaltyDT loyaltyDT;
        IDA_OrderNoDT daOrderNoDT;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DA_OrdersDT(
            IUsersToDatabasesXML usersToDatabases,
            IGenericDAO<DA_OrdersDTO> daOrdersDao,
            IGenericDAO<DA_OrderDetailsDTO> daOrderDetailsDao,
            IGenericDAO<DA_OrderDetailsExtrasDTO> daOrderDetailsExtrasDao,
            LocalConfigurationHelper configHlp,
            IDA_OrderStatusDT orderStatusDt,
            IDA_LoyaltyDT loyaltyDT,
            IDA_OrderNoDT daOrderNoDT
            )
        {
            this.usersToDatabases = usersToDatabases;
            this.daOrdersDao = daOrdersDao;
            this.daOrderDetailsDao = daOrderDetailsDao;
            this.daOrderDetailsExtrasDao = daOrderDetailsExtrasDao;
            this.configHlp = configHlp;
            this.orderStatusDt = orderStatusDt;
            this.loyaltyDT = loyaltyDT;
            this.daOrderNoDT = daOrderNoDT;
        }

        /// <summary>
        /// Return the status of an Order
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        public OrderStatusEnum GetStatus(DBInfoModel Store, long Id)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string getOrderStatusSql = @"SELECT do.[Status] FROM DA_Orders AS do WHERE do.Id =@Id";
                return db.Query<OrderStatusEnum>(getOrderStatusSql, new { Id = Id }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get All Orders
        /// </summary>
        /// <returns></returns>
        public List<DA_OrderModelExt> GetAllOrders(DBInfoModel Store)
        {
            List<DA_OrderModelExt> orderModel = new List<DA_OrderModelExt>();
            List<DA_OrderModel> tmpList = new List<DA_OrderModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = "DECLARE @MaxDate DATETIME \n"
                               + "SELECT @MaxDate = MAX(CAST(CONVERT(NVARCHAR(10), do.OrderDate, 120) AS DATETIME)) FROM DA_Orders AS do \n"
                               + "DECLARE @OrderIds TABLE (OrderId BIGINT)  \n"
                               + "DECLARE @OrderDetailsIds TABLE (OrderId BIGINT)  \n"
                               + "\n"
                               + "INSERT INTO @OrderIds(OrderId)  \n"
                               + "SELECT Id FROM DA_Orders AS do  \n"
                               + "WHERE CAST(CONVERT(NVARCHAR(10), do.OrderDate, 120) AS DATETIME) = @MaxDate \n"
                               + "ORDER BY do.OrderDate DESC  \n"
                               + "\n"
                               + "INSERT INTO @OrderDetailsIds(OrderId)   \n"
                               + "SELECT DISTINCT dod.Id  \n"
                               + "FROM DA_OrderDetails AS dod   \n"
                               + "INNER JOIN @OrderIds o ON o.OrderId = dod.DAOrderId  \n"
                               + "\n"
                               + "SELECT do.*,ds.Title AS StoreDescr, da.AddressStreet, da.AddressNo, da.City, dc.FirstName, dc.LastName, dc.Phone1, dc.Phone2, dc.Mobile, dc.JobName, dc.PhoneComp, dc.Proffesion, dc.VatNo, dc.Doy, dc.Notes, dc.LastOrderNote AS LastOrderNotes, dc.SecretNotes  \n"
                               + "FROM DA_Orders AS do   \n"
                               + "INNER JOIN @OrderIds o ON o.OrderId = do.Id  \n"
                               + "INNER JOIN DA_Stores AS ds ON ds.Id = do.StoreId  \n"
                               + "INNER JOIN DA_Customers AS dc ON dc.Id = do.CustomerId  \n"
                               + "LEFT JOIN DA_Addresses AS da ON da.Id = do.ShippingAddressId  \n"
                               + "ORDER BY do.OrderDate DESC  \n"
                               + "\n"
                               + "SELECT dod.*   \n"
                               + "FROM DA_OrderDetails AS dod   \n"
                               + "INNER JOIN @OrderDetailsIds o ON o.OrderId = dod.Id  \n"
                               + "\n"
                               + "SELECT dode.*   \n"
                               + "FROM DA_OrderDetailsExtras AS dode  \n"
                               + "INNER JOIN @OrderDetailsIds o ON o.OrderId = dode.OrderDetailId ";

                using (var multipleResult = db.QueryMultiple(sqlData))
                {
                    orderModel = multipleResult.Read<DA_OrderModelExt>().ToList();
                    List<DA_OrderDetails> DetailsModel = multipleResult.Read<DA_OrderDetails>().ToList();
                    List<DA_OrderDetailsExtrasModel> ExtrasModel = multipleResult.Read<DA_OrderDetailsExtrasModel>().ToList();

                    foreach (DA_OrderDetails details in DetailsModel)
                    {
                        details.Extras = ExtrasModel.Where(w => w.OrderDetailId == details.Id).Select(s => s).ToList();
                    }

                    foreach (DA_OrderModel orders in orderModel)
                    {
                        orders.Details = DetailsModel.Where(d => d.DAOrderId == orders.Id).Select(s => s).ToList();
                    }

                }
            }
            return orderModel;
        }

        /// <summary>
        /// Get Orders By Date
        /// </summary>
        /// <returns></returns>
        public List<DA_OrderModelExt> GetOrdersByDate(DBInfoModel Store, string SelectedDate)
        {
            List<DA_OrderModelExt> orderModel = new List<DA_OrderModelExt>();
            List<DA_OrderModel> tmpList = new List<DA_OrderModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlData = "DECLARE @MaxDate DATETIME \n"
                               + "DECLARE @OrderIds TABLE (OrderId BIGINT)  \n"
                               + "DECLARE @OrderDetailsIds TABLE (OrderId BIGINT)  \n"
                               + "\n"
                               + "INSERT INTO @OrderIds(OrderId)  \n"
                               + "SELECT Id FROM DA_Orders AS do  \n"
                               + "WHERE CAST(CONVERT(NVARCHAR(10), do.OrderDate, 120) AS DATETIME) = '" + SelectedDate + "' \n"
                               + "ORDER BY do.OrderDate DESC  \n"
                               + "\n"
                               + "INSERT INTO @OrderDetailsIds(OrderId)   \n"
                               + "SELECT DISTINCT dod.Id  \n"
                               + "FROM DA_OrderDetails AS dod   \n"
                               + "INNER JOIN @OrderIds o ON o.OrderId = dod.DAOrderId  \n"
                               + "\n"
                               + "SELECT do.*,ds.Title AS StoreDescr, da.AddressStreet, da.AddressNo, da.City, dc.FirstName, dc.LastName, dc.Phone1, dc.Phone2, dc.Mobile, dc.Notes, dc.LastOrderNote AS LastOrderNotes, dc.SecretNotes  \n"
                               + "FROM DA_Orders AS do   \n"
                               + "INNER JOIN @OrderIds o ON o.OrderId = do.Id  \n"
                               + "INNER JOIN DA_Stores AS ds ON ds.Id = do.StoreId  \n"
                               + "INNER JOIN DA_Customers AS dc ON dc.Id = do.CustomerId  \n"
                               + "LEFT JOIN DA_Addresses AS da ON da.Id = do.ShippingAddressId  \n"
                               + "ORDER BY do.OrderDate DESC  \n"
                               + "\n"
                               + "SELECT dod.*   \n"
                               + "FROM DA_OrderDetails AS dod   \n"
                               + "INNER JOIN @OrderDetailsIds o ON o.OrderId = dod.Id  \n"
                               + "\n"
                               + "SELECT dode.*   \n"
                               + "FROM DA_OrderDetailsExtras AS dode  \n"
                               + "INNER JOIN @OrderDetailsIds o ON o.OrderId = dode.OrderDetailId ";

                using (var multipleResult = db.QueryMultiple(sqlData))
                {
                    orderModel = multipleResult.Read<DA_OrderModelExt>().ToList();
                    List<DA_OrderDetails> DetailsModel = multipleResult.Read<DA_OrderDetails>().ToList();
                    List<DA_OrderDetailsExtrasModel> ExtrasModel = multipleResult.Read<DA_OrderDetailsExtrasModel>().ToList();

                    foreach (DA_OrderDetails details in DetailsModel)
                    {
                        details.Extras = ExtrasModel.Where(w => w.OrderDetailId == details.Id).Select(s => s).ToList();
                    }

                    foreach (DA_OrderModel orders in orderModel)
                    {
                        orders.Details = DetailsModel.Where(d => d.DAOrderId == orders.Id).Select(s => s).ToList();
                    }

                }
            }
            return orderModel;
        }

        /// <summary>
        /// Get Customer Recent Orders
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <param name="filter">define filter for returning orders</param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη</returns>
        public List<DA_OrderModel> GetOrders(DBInfoModel Store, long id, int top, GetOrdersFilterEnum filter = GetOrdersFilterEnum.All)
        {
            List<DA_OrderModel> orderModel = new List<DA_OrderModel>();
            List<DA_OrderModel> tmpList = new List<DA_OrderModel>();

           

            string whereStr = " do.CustomerId =" + id.ToString();
            if (filter==GetOrdersFilterEnum.Historic)
                whereStr = whereStr + " and status in (5,6,7) ";
            else if (filter == GetOrdersFilterEnum.NotPending)
                whereStr = whereStr + " and status <> 11 ";

            string sqlData = "DECLARE @OrderIds TABLE (OrderId BIGINT) \n"
                              + "DECLARE @OrderDetailsIds TABLE (OrderId BIGINT) \n"
                              + " \n"
                              + "INSERT INTO @OrderIds(OrderId) \n"
                              + "SELECT top " + top + " Id FROM DA_Orders AS do WHERE " + whereStr + " ORDER BY do.Id DESC \n"
                              + " \n"
                              + "INSERT INTO @OrderDetailsIds(OrderId)  \n"
                              + "SELECT DISTINCT dod.Id \n"
                              + "FROM DA_OrderDetails AS dod  \n"
                              + "INNER JOIN @OrderIds o ON o.OrderId = dod.DAOrderId \n"
                              + " \n"
                              + "SELECT do.*  \n"
                              + "FROM DA_Orders AS do  \n"
                              + "INNER JOIN @OrderIds o ON o.OrderId = do.Id \n"
                              + "ORDER BY do.OrderDate DESC \n"
                              + "	 \n"
                              + "SELECT dod.*  \n"
                              + "FROM DA_OrderDetails AS dod  \n"
                              + "INNER JOIN @OrderDetailsIds o ON o.OrderId = dod.Id \n"
                              + " \n"
                              + "SELECT dode.*  \n"
                              + "FROM DA_OrderDetailsExtras AS dode \n"
                              + "INNER JOIN @OrderDetailsIds o ON o.OrderId = dode.OrderDetailId \n"
                              + "";

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var multipleResult = db.QueryMultiple(sqlData))
                {
                    orderModel = multipleResult.Read<DA_OrderModel>().ToList();
                    List<DA_OrderDetails> DetailsModel = multipleResult.Read<DA_OrderDetails>().ToList();
                    List<DA_OrderDetailsExtrasModel> ExtrasModel = multipleResult.Read<DA_OrderDetailsExtrasModel>().ToList();

                    foreach (DA_OrderDetails details in DetailsModel)
                    {
                        details.Extras = ExtrasModel.Where(w => w.OrderDetailId == details.Id).Select(s => s).ToList();
                    }

                    foreach (DA_OrderModel orders in orderModel)
                    {
                        orders.Details = DetailsModel.Where(d => d.DAOrderId == orders.Id).Select(s => s).ToList();
                    }

                }
            }

            return orderModel;
        }

        /// <summary>
        /// Get A Specific Order
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Order + details + ShippingAddress</returns>
        public DA_ExtOrderModel GetOrderById(DBInfoModel Store, long Id)
        {
            DA_ExtOrderModel orderExtModel = new DA_ExtOrderModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                string sqlOrder = @"SELECT * FROM DA_Orders AS do WHERE do.Id =@OrderId";
                orderExtModel.OrderModel = db.Query<DA_OrderModel>(sqlOrder, new { OrderId = Id }).FirstOrDefault();

                if (orderExtModel.OrderModel != null)
                {

                    string sqlOrderDetails = @"SELECT * FROM DA_OrderDetails AS dod WHERE dod.DAOrderId=@daOrderId";
                    orderExtModel.OrderModel.Details = db.Query<DA_OrderDetails>(sqlOrderDetails, new { daOrderId = Id }).ToList();


                    foreach (DA_OrderDetails detail in orderExtModel.OrderModel.Details)
                    {
                        string sqlOrderExtras = @"SELECT * FROM DA_OrderDetailsExtras AS dode WHERE dode.OrderDetailId=@orderDetailId";
                        detail.Extras = db.Query<DA_OrderDetailsExtrasModel>(sqlOrderExtras, new { orderDetailId = detail.Id }).ToList();
                    }

                }

                string sqlAddresses = @"SELECT da.* 
                                            FROM DA_Addresses AS da 
                                            INNER JOIN DA_Orders AS do ON do.ShippingAddressId = da.Id
											where do.Id = @OrderId";
                orderExtModel.AddressesModel = db.Query<DA_AddressModel>(sqlAddresses, new { OrderId = Id }).ToList();

            }

            return orderExtModel;
        }

        /// <summary>
        /// Get A Specific Order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="orderId"></param>
        /// <returns>Order without details</returns>
        public DA_OrderModel GetSingleOrderById(DBInfoModel Store, long orderId)
        {
            DA_OrdersDTO order;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                order = daOrdersDao.Select(db, orderId);
            }
            return AutoMapper.Mapper.Map<DA_OrderModel>(order);
        }

        /// <summary>
        /// Get a specific order with details
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="orderId"></param>
        /// <returns>Order with details</returns>
        public DA_OrderModel GetOrderWithDetailsById(DBInfoModel Store, long orderId)
        {
            DA_OrderModel order;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlOrder = @"SELECT * FROM DA_Orders AS do WHERE do.Id = @OrderId";
                order = db.Query<DA_OrderModel>(sqlOrder, new { OrderId = orderId }).FirstOrDefault();
                if (order != null)
                {
                    string sqlOrderDetails = @"SELECT * FROM DA_OrderDetails AS dod WHERE dod.DAOrderId = @OrderId";
                    order.Details = db.Query<DA_OrderDetails>(sqlOrderDetails, new { OrderId = orderId }).ToList();
                    if (order.Details != null && order.Details.Count > 0)
                    {
                        foreach (DA_OrderDetails detail in order.Details)
                        {
                            string sqlOrderDetailExtras = @"SELECT * FROM DA_OrderDetailsExtras AS dode WHERE dode.OrderDetailId = @OrderDetailId";
                            detail.Extras = db.Query<DA_OrderDetailsExtrasModel>(sqlOrderDetailExtras, new { OrderDetailId = detail.Id }).ToList();
                        }
                    }
                }
            }
            return order;
        }


        /// <summary>
        /// Update A Specific Order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="order"></param>
        /// <returns>oder id</returns>
        public long UpdateSingleOrder(DBInfoModel Store, DA_OrderModel order)
        {
            long orderId = 0;
            DA_OrdersDTO orderDto = AutoMapper.Mapper.Map<DA_OrdersDTO>(order);
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int rowsAffected = daOrdersDao.Update(db, orderDto);
                if (rowsAffected == 1)
                {
                    orderId = order.Id;
                }
            }
            return orderId;
        }


        /// <summary>
        /// Get A Specific Order based on ExtId1 (Efood order id). Return DA_Orders.Id. 
        /// If ExtId1 not found return 0;
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="ExtId1">Efood order id</param>
        /// <returns>Order id</returns>
        public long GetOrderByExtId1(DBInfoModel Store, string ExtId1)
        {
            DA_ExtOrderModel orderExtModel = new DA_ExtOrderModel();
            DA_OrderDetails detailsModel = new DA_OrderDetails();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlOrder = @"SELECT Id FROM DA_Orders AS do WHERE do.ExtId1 =@ExtId1";
                return db.QueryFirstOrDefault<long>(sqlOrder, new { ExtId1 = ExtId1 });
            }
            return 0;
        }
        public bool CheckDA_OpeningHours(DBInfoModel Store,long StoreId,DateTime checkDate)
        {
            DateTime OpenFrom = DateTime.Now;
            try
            {
                string connectionString = usersToDatabases.ConfigureConnectionString(Store);

                DayOfWeek weekday = checkDate.DayOfWeek; int weekdayint = (int)weekday; int day = 0;
                string sql = @"select * from DA_OpeningHours where StoreId=" + StoreId + " and Weekday=" + weekdayint;

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    DA_OpeningHoursModel openhours = db.Query<DA_OpeningHoursModel>(sql).FirstOrDefault();
                    if (openhours == null)
                    {
                        logger.Warn("The table DA_OpeningHours is not configured, please configure the aformentioned table if CheckDA_OpeningHours is true! ");
                        return false;
                    }

                    OpenFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, checkDate.Day, openhours.OpenHour, openhours.OpenMinute, 0);

                    if (openhours.CloseHour <= openhours.OpenHour)
                        day = checkDate.Day + 1;
                    else
                        day = checkDate.Day;

                    DateTime OpenTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day, openhours.CloseHour, openhours.CloseMinute, 0);

                    if (OpenFrom <= checkDate && checkDate <= OpenTo)
                        return true;
                    else
                        throw new Symposium.Helpers.BusinessException($"Working hours from {openhours.OpenHour} - {openhours.CloseHour}");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error while checking DA_OpeningHours : " + e);
                return false;
            }

    }
        /// <summary>
        /// Search for Orders
        /// </summary>
        /// <param name="Model">Filter Model</param>
        /// <returns>Επιστρέφει τις παραγγελίες βάση κριτηρίων</returns>
        public List<DA_OrderModel> SearchOrders(DBInfoModel Store, DA_SearchOrdersModel Model)
        {
            List<DA_OrderModel> orderModel = new List<DA_OrderModel>();
            List<DA_OrderModel> tmpList = new List<DA_OrderModel>();
            string where = "";
            if (Model.OrderDateFrom != null)
            {
                where = where + " AND do.OrderDate <='" + Model.OrderDateFrom + "'";
            }
            if (Model.OrderDateTo != null)
            {
                where = where + " AND do.OrderDate >='" + Model.OrderDateTo + "'";
            }
            if (Model.StoreId > 0)
            {
                where = where + " AND do.StoreId=" + Model.StoreId;
            }
            if (Model.CustomerId > 0)
            {
                where = where + " AND do.CustomerId=" + Model.CustomerId;
            }
            if (Model.Status > -1)
            {
                where = where + " AND do.Status=" + Model.Status;
            }
            if (Model.OrderType > -1)
            {
                where = where + " AND do.OrderType=" + Model.OrderType;
            }
            if (Model.WithRemarks == true)
            {
                where = where + " AND isnull(do.Remarks,'') <> ''";
            }
            if (Model.WithRemarks == false)
            {
                where = where + " AND isnull(do.Remarks,'') = ''";
            }
            if (Model.DurationLessThan > 0)
            {
                where = where + " AND do.Duration>=" + Model.DurationLessThan;
            }
            if (Model.DurationMoreThan > 0)
            {
                where = where + " AND do.Duration<=" + Model.DurationMoreThan;
            }

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlHeader = @"SELECT * FROM DA_Orders AS do WHERE do.Id > -1" + where;
                tmpList = db.Query<DA_OrderModel>(sqlHeader).ToList();

                foreach (DA_OrderModel order in tmpList)
                {
                    DA_OrderModel tmpOrder = new DA_OrderModel();
                    DA_OrderDetails tmpDetails = new DA_OrderDetails();
                    tmpOrder = order;
                    string sqlDetails = @"SELECT * FROM DA_OrderDetails AS dod WHERE dod.DAOrderId =@orderId";
                    tmpOrder.Details = db.Query<DA_OrderDetails>(sqlDetails, new { orderId = order.Id }).ToList();
                    foreach (DA_OrderDetails details in tmpOrder.Details)
                    {
                        string sqlExtras = @"SELECT * FROM DA_OrderDetailsExtras AS dode WHERE dode.OrderDetailId =@orderDetailsId";
                        tmpDetails.Extras = db.Query<DA_OrderDetailsExtrasModel>(sqlExtras, new { orderDetailsId = details.Id }).ToList();
                    }
                    tmpOrder.Details.Add(tmpDetails);
                    orderModel.Add(tmpOrder);
                }
            }

            return orderModel;
        }

        /// <summary>
        /// Mεταβάλλει το DA_orders. StatusChange και εισάγει νέα εγγραφή στον DA_OrderStatus
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        public long UpdateStatus(DBInfoModel Store, long Id, OrderStatusEnum Status)
        {
            long OrderId = 0;
            OrderStatusEnum StatusNum = OrderStatusEnum.Received;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlGet = @"SELECT do.[Status] FROM DA_Orders AS do WHERE do.Id =@orderId";
                StatusNum = db.Query<OrderStatusEnum>(sqlGet, new { orderId = Id }).FirstOrDefault();


                if (StatusNum != Status)
                {
                    using (var scope = new TransactionScope())
                    {
                        string sqlUpdate = @"UPDATE DA_Orders SET [Status] =@status, StatusChange =@statusChange  WHERE Id =@orderId";
                        db.Execute(sqlUpdate, new { status = Status, statusChange = DateTime.Now, orderId = Id });

                        string sqlInsert = @"INSERT INTO DA_OrderStatus(OrderDAId, [Status], StatusDate)
                                             VALUES(@orderDAId, @status, @statusDate)";
                        db.Execute(sqlInsert, new { orderId = Id, status = Status, statusChange = DateTime.Now, });

                        scope.Complete();
                    }
                    OrderId = Id;
                }
            }

            return OrderId;
        }

        /// <summary>
        /// Get Customer Recent Remarks
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη Remarks != null</returns>
        public List<DA_OrderModel> GetRemarks(DBInfoModel Store, long Id, int top)
        {
            string sqlData = "";
            List<DA_OrderModel> orderModel = new List<DA_OrderModel>();
            List<DA_OrderModel> tmpListHeader = new List<DA_OrderModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (top == 0)
                {
                    sqlData = @"SELECT * FROM DA_Orders AS do WHERE do.CustomerId =@CustId AND do.Remarks IS NOT NULL ORDER BY do.OrderDate DESC";
                }
                else if (top < 0)
                {
                    sqlData = @"SELECT top " + top + " * FROM DA_Orders AS do WHERE do.CustomerId =@CustId AND do.Remarks IS NOT NULL ORDER BY do.OrderDate DESC";
                    top = 5;
                }
                else
                {
                    sqlData = @"SELECT top " + top + " * FROM DA_Orders AS do WHERE do.CustomerId =@CustId AND do.Remarks IS NOT NULL ORDER BY do.OrderDate DESC";
                }

                tmpListHeader = db.Query<DA_OrderModel>(sqlData, new { CustId = Id }).ToList();

                foreach (DA_OrderModel order in tmpListHeader)
                {
                    DA_OrderModel tmpOrder = new DA_OrderModel();
                    DA_OrderDetails tmpDetails = new DA_OrderDetails();
                    tmpOrder = order;
                    string sqlDetails = @"SELECT * FROM DA_OrderDetails AS dod WHERE dod.DAOrderId =@orderId";
                    tmpOrder.Details = db.Query<DA_OrderDetails>(sqlDetails, new { orderId = order.Id }).ToList();
                    foreach (DA_OrderDetails details in tmpOrder.Details)
                    {
                        string sqlExtras = @"SELECT * FROM DA_OrderDetailsExtras AS dode WHERE dode.OrderDetailId =@orderDetailsId";
                        tmpDetails.Extras = db.Query<DA_OrderDetailsExtrasModel>(sqlExtras, new { orderDetailsId = details.Id }).ToList();
                    }
                    tmpOrder.Details.Add(tmpDetails);
                    orderModel.Add(tmpOrder);
                }
            }

            return orderModel;
        }

        /// <summary>
        /// Get Order Status For Specific Order by OrderId
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<DA_OrderStatusModel> GetOrderStatusTimeChanges(DBInfoModel dbInfo, long OrderId)
        {
            List<DA_OrderStatusModel> orderStatusModel = new List<DA_OrderStatusModel>();

            string sql = "SELECT * FROM DA_OrderStatus AS dos WHERE dos.OrderDAId =@orderDaId ORDER BY dos.StatusDate asc";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                orderStatusModel = db.Query<DA_OrderStatusModel>(sql, new { orderDaId = OrderId }).ToList();
            }

            return orderStatusModel;
        }

        /// <summary>
        /// Update Customer Remarks
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long UpdateRemarks(DBInfoModel Store, UpdateRemarksModel Model)
        {
            long OrderId = 0;
            long CustId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateSQL = @"UPDATE DA_Orders SET Remarks =@remarks WHERE Id =@orderId";
                db.Execute(updateSQL, new { remarks = Model.Remarks, orderId = Model.OrderId });
                OrderId = Model.OrderId;

                string getCustIdSQL = @"SELECT do.CustomerId FROM DA_Orders AS do WHERE do.Id =@orderId";
                CustId = db.Query<long>(getCustIdSQL, new { orderId = Model.OrderId }).FirstOrDefault();

                string updateCustSQL = @"UPDATE DA_Customers SET LastOrderNote =@Note WHERE Id =@custId";
                db.Execute(updateCustSQL, new { Note = Model.LastOrderNote, custId = CustId });
            }
            return OrderId;
        }

        /// <summary>
        /// Add new Order 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public long InsertOrder(DBInfoModel Store, DA_OrderModel Model)
        {
            configHlp.CheckDeliveryAgent();
            DA_OrdersDTO dto = AutoMapper.Mapper.Map<DA_OrdersDTO>(Model);
            long orderId = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateCustNote = @"UPDATE DA_Customers SET LastOrderNote =@note,LastAddressId =@address WHERE Id =@CustId";
                db.Execute(updateCustNote, new { note = Model.LastOrderNote, address = Model.ShippingAddressId, CustId = Model.CustomerId });
                //string updateCustAddr = @"UPDATE DA_Customers SET LastAddressId =@address WHERE Id =@CustId";
                //db.Query<DACustomerModel>(updateCustAddr, new { address = Model.ShippingAddressId, CustId = Model.CustomerId }).FirstOrDefault();

                using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(2)))
                {
                    long daOrderNo = daOrderNoDT.FetchOrderNo(db);
                    Model.OrderNo = daOrderNo;
                    dto.OrderNo = daOrderNo;

                    orderId = daOrdersDao.Insert(db, dto);
                    foreach (DA_OrderDetails detail in Model.Details)
                    {
                        detail.DAOrderId = orderId;
                        DA_OrderDetailsDTO orderDetailDto = AutoMapper.Mapper.Map<DA_OrderDetailsDTO>(detail);
                        long orderDetailId = daOrderDetailsDao.Insert(db, orderDetailDto);

                        if (detail.Extras != null && detail.Extras.Count > 0)
                        {
                            foreach (DA_OrderDetailsExtrasModel extras in detail.Extras)
                            {
                                extras.OrderDetailId = orderDetailId;
                                DA_OrderDetailsExtrasDTO orderDetailExtraDto = AutoMapper.Mapper.Map<DA_OrderDetailsExtrasDTO>(extras);
                                long orderDetailExtraId = daOrderDetailsExtrasDao.Insert(db, orderDetailExtraDto);
                            }
                        }
                    }

                    scope.Complete();
                }
            }
            return orderId;
        }

        /// <summary>
        /// Update an Order 
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="HasChanges"></param>
        /// <returns></returns>
        public long UpdateOrder(DBInfoModel Store, DA_OrderModel Model, bool HasChanges)
        {

            OrderStatusEnum oldStatus = 0;
            DA_OrderModel ordersModel = new DA_OrderModel();
            long orderId = 0;
            string orderDetails = "";
            DA_OrdersDTO dt = AutoMapper.Mapper.Map<DA_OrdersDTO>(Model);
            string getOrderStatusSql = @"SELECT do.[Status] FROM DA_Orders AS do WHERE do.Id =@ID";
            string getOrderDetailsIds = @"SELECT dod.Id FROM DA_OrderDetails AS dod WHERE dod.DAOrderId =@daOrderId";
            string insertStatusSql = @"INSERT INTO DA_OrderStatus(OrderDAId, [Status], StatusDate) 
                                                                VALUES (@orderDAId, @status, @statusDate )";
            string deleteDetails = @"DELETE FROM DA_OrderDetails WHERE DAOrderId =@daOrderId";

            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                oldStatus = db.Query<OrderStatusEnum>(getOrderStatusSql, new { ID = Model.Id }).FirstOrDefault();
                //   DA_OrdersDTO oldmodel = db.Query<DA_OrdersDTO>(getOrderStatusSql, new { ID = Model.Id }).FirstOrDefault();

                if (HasChanges == true)
                {
                    List<long> orderDetailsIds = new List<long>();
                   
                    orderDetailsIds = db.Query<long>(getOrderDetailsIds, new { daOrderId = Model.Id }).ToList();
                    orderDetails = String.Join(",", orderDetailsIds);
                }

                using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(40)))
                {
                    if (Model.Status != oldStatus)
                    {
                        Model.StatusChange = DateTime.Now;

                        db.Execute(insertStatusSql, new { orderDAId = Model.Id, status = Model.Status, statusDate = DateTime.Now });
                    }

                    if (HasChanges == true)
                    {
                        //-- Delete DA_OrderDetailsExtras --

                        //1.  Used to lock database against other users
                        db.Query(@"select * from Version with(updlock)").FirstOrDefault();

                        string deleteExtras = @"DELETE FROM DA_OrderDetailsExtras WHERE OrderDetailId IN (" + orderDetails + ")";
                        db.Execute(deleteExtras);

                        //2. delete DA_OrderDetails
                        
                        db.Execute(deleteDetails, new { daOrderId = Model.Id });

                        //3. Insert new Updated entries in DA_OrderDetails
                        foreach (DA_OrderDetails detail in Model.Details)
                        {
                            detail.Id = 0;
                            DA_OrderDetailsDTO orderDetailDto = AutoMapper.Mapper.Map<DA_OrderDetailsDTO>(detail);
                            long orderDetailId = daOrderDetailsDao.Insert(db, orderDetailDto);

                            //Insert new Updated entries in DA_OrderDetailsExtras
                            if (detail.Extras != null && detail.Extras.Count > 0)
                            {
                                foreach (DA_OrderDetailsExtrasModel extras in detail.Extras)
                                {
                                    extras.Id = 0;
                                    extras.OrderDetailId = orderDetailId;
                                    DA_OrderDetailsExtrasDTO orderDetailExtraDto = AutoMapper.Mapper.Map<DA_OrderDetailsExtrasDTO>(extras);
                                    long orderDetailExtraId = daOrderDetailsExtrasDao.Insert(db, orderDetailExtraDto);
                                }
                            }
                        }
                    }
                    daOrdersDao.Update(db, dt);
                    scope.Complete();
                }
            }
            return Model.Id;
        }

        /// <summary>
        /// Ακύρωση παραγγελίας από όλους εκτός από το κατάστημα. 
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        public long CancelOrder(DBInfoModel Store, long Id, OrderStatusEnum[] cancelStasus, bool isSend=true)
        {
            long OrderId = 0;
            DA_OrderModel orderModel = new DA_OrderModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string sqlUpdate = @"UPDATE DA_Orders SET [Status] =@status, StatusChange =@statusChange, IsSend =@isSend, PointsGain=0, PointsRedeem=0 WHERE Id =@orderId ";
            string insertSql = @"INSERT INTO DA_OrderStatus(OrderDAId, [Status], StatusDate) 
                                                                VALUES (@orderDAId, @status, @statusDate )";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //using (var scope = new TransactionScope())
                //{
                //Get Recent Order From DB
                string sqlOrder = @"SELECT * FROM DA_Orders AS do WHERE do.Id =@orderId";
                orderModel = db.Query<DA_OrderModel>(sqlOrder, new { orderId = Id }).FirstOrDefault();
                if (!isSend) orderModel.IsSend = -1;
                if (cancelStasus.Contains(orderModel.Status))
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(40)))
                    {
                        
                        db.Execute(sqlUpdate, new { status = OrderStatusEnum.Canceled, statusChange = DateTime.Now, isSend = orderModel.IsSend + 1, orderId = Id });
                        OrderId = orderModel.Id;
                        db.Execute(insertSql, new { orderDAId = Id, status = OrderStatusEnum.Canceled, statusDate = DateTime.Now });

                        scope.Complete();
                    }
                }
                else
                {
                    throw new BusinessException(Symposium.Resources.Errors.DACANCELFAILED);
                }
                // scope.Complete();
                // }
            }

            return OrderId;
        }

        /// <summary>
        /// Ακύρωση παραγγελίας από το κατάστημα MONO.  
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public long StoreCancelOrder(DBInfoModel Store, long Id, long StoreId, OrderStatusEnum[] cancelStasus)
        {
            long OrderId = 0;
            DA_OrderModel orderModel = new DA_OrderModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //using (var scope = new TransactionScope())
                //{
                //Get Recent Order From DB
                string sqlOrder = @"SELECT * FROM DA_Orders AS do WHERE do.Id =@orderId";
                orderModel = db.Query<DA_OrderModel>(sqlOrder, new { orderId = Id }).FirstOrDefault();

                if (cancelStasus.Contains(orderModel.Status))
                {
                    if (orderModel.StoreId == StoreId)
                    {
                        string sqlUpdate = @"UPDATE DA_Orders SET [Status] =@status, StatusChange =@statusChange, IsSend =@isSend WHERE Id =@orderId ";
                        db.Execute(sqlUpdate, new { status = OrderStatusEnum.Canceled, statusChange = DateTime.Now, isSend = orderModel.IsSend + 1, orderId = Id });
                        OrderId = orderModel.Id;
                    }
                    else
                    {
                        throw new BusinessException(string.Format(Symposium.Resources.Errors.STOREIDAUTHFAILDED, (StoreId.ToString() ?? "<null>")));
                    }
                }
                else
                {
                    throw new BusinessException(Symposium.Resources.Errors.DACANCELFAILED);
                }
                //    scope.Complete();
                //}
            }

            return OrderId;
        }


        /// <summary>
        /// return the number of orders in DB for a specific store  
        /// </summary>
        /// <param name="Store">connection string</param>
        /// <param name="StoreId">store id</param>
        /// <returns></returns>
        public int GetStoreOrderNo(DBInfoModel Store, long StoreId)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return daOrdersDao.RecordCount(db, "where StoreId=@StoreId", new { StoreId = StoreId });
            }
        }


        /// <summary>
        /// Delete's an DA_Order Record
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ErrorMess"></param>
        /// <returns></returns>
        public bool DeleteOrders(DBInfoModel Store, long DAOrderId, out string ErrorMess)
        {
            bool res = true;
            ErrorMess = "";
            try
            {
                //Get's external model of DA_Order with orderstails and extras
                DA_ExtOrderModel order = GetOrderById(Store, DAOrderId);
                //Order not exists;
                if (order == null)
                    return true;

                //Get's a delete's all da_orderstatuses
                List<DA_OrderStatusModel> orderStats = orderStatusDt.GetDA_OrderStatusesByOrderId(Store, DAOrderId);
                if (orderStats != null && orderStats.Count > 0)
                    if (!orderStatusDt.DeleteDA_OrderStatusList(Store, orderStats, out ErrorMess))
                        return false;
                //Delete Loyalty Points
                loyaltyDT.DeleteGainPoints(Store, DAOrderId, order.OrderModel.StoreId);

                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    foreach (DA_OrderDetails item in order.OrderModel.Details)
                    {
                        daOrderDetailsExtrasDao.DeleteList(db, "WHERE OrderDetailId = @OrderDetailId", new { OrderDetailId = item.Id });
                        daOrderDetailsDao.Delete(db, item.Id);
                    }
                    daOrdersDao.Delete(db, AutoMapper.Mapper.Map<DA_OrdersDTO>(order.OrderModel));
                }
            }
            catch (Exception ex)
            {
                res = false;
                ErrorMess = ex.ToString();
            }
            return res;
        }
        
        /// <summary>
        /// Update DAOrders Set Error Message
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <param name="ErrorMess"></param>
        public void SetErrorMessageToDAOrder(DBInfoModel Store, long DAOrderId, string ErrorMess)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "UPDATE DA_Orders SET ErrorMessage = @ErrorMessage WHERE Id = @Id";
                db.Execute(SQL, new { ErrorMessage = ErrorMess, Id = DAOrderId });
            }
        }

        /// <summary>
        /// Returns an order from orderno
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderNo"></param>
        /// <returns></returns>
        public DA_OrderModel GetOrderByOrderNo(DBInfoModel Store, long DAOrderNo)
        {
            DA_OrdersDTO order;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                order = daOrdersDao.Select(db, "WHERE OrderNo = @OrderNo", new { OrderNo = DAOrderNo }).FirstOrDefault();
            }
            return AutoMapper.Mapper.Map<DA_OrderModel>(order);
        }

        /// <summary>
        /// Update's ExtId1 with Omnirest External system OrderNo 
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public bool UpdateDA_OrderExtId1(DBInfoModel Store, long DA_OrderId, long OrderNo)
        {
            bool res = false;
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    string ExtId1Str = "OM-" + OrderNo.ToString();
                    string SQL = "UPDATE DA_Orders SET ExtId1 = @extId1 WHERE Id = @ID";
                    db.Execute(SQL, new { extId1 = ExtId1Str, ID = DA_OrderId });
                    res = true;
                }
                catch(Exception ex)
                {
                    res = false;
                }
            }
            return res;
        }

        /// <summary>
        /// Returns a list of ordera and statuses for Goodys Omnirest
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public List<DA_OrdersForGoodysOmnirestStatus> GetOrdersForGoodysOmnirest(DBInfoModel dbInfo, out string error)
        {
            error = "";
            string sql = "";
            List<DA_OrdersForGoodysOmnirestStatus> result = null;
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    sql = "SELECT do.Id, do.OrderNo, RIGHT(do.ExtId1, LEN(do.ExtId1) - 3) OmnRestOrderId, DATEDIFF(hh,do.OrderDate,GETDATE()) StayHours, dos.[Status] \n"
                       + "FROM DA_Orders AS do \n"
                       + "CROSS APPLY ( \n"
                       + "	SELECT TOP 1 * \n"
                       + "	FROM DA_OrderStatus AS dos \n"
                       + "	WHERE dos.OrderDAId = do.Id \n"
                       + "	ORDER BY dos.StatusDate DESC \n"
                       + ") dos \n"
                       + "WHERE LEFT(ISNULL(do.ExtId1,''),3) = 'OM-' AND do.[Status] NOT IN (" + ((int)OrderStatusEnum.Complete).ToString() + "," + ((int)OrderStatusEnum.Returned).ToString() + "," + ((int)OrderStatusEnum.Canceled).ToString() + ")";
                    result = db.Query<DA_OrdersForGoodysOmnirestStatus>(sql).ToList();
                }

            }
            catch(Exception ex)
            {
                error = "[SQL : " + sql + "] \r\n" + ex.ToString();
            }
            return result;
        }
    }
}
