using AutoMapper;
using Dapper;
using log4net;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DateRange;
using Symposium.Models.Models.Kds;
using Symposium.Models.Models.Payroll;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.Kds;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Symposium.WebApi.DataAccess.DT.Kds
{
    public class KdsDT : IKdsDT
    {

        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public KdsDT(IUsersToDatabasesXML usertodbs)
        {
            this.usersToDatabases = usertodbs;
        }

        public List<KdsResponceModel> GetKdsOrdersMain(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model)
        {
            List<KdsResponceModel> res = new List<KdsResponceModel>();
            if (Model.SaleTypesIds.Contains(22))
            {
                if (Model.SaleTypesIds.Contains(1) == false)
                {
                    Model.SaleTypesIds.Add(1);
                }
            }
            else
            {
                if (Model.SaleTypesIds.Contains(1))
                {
                    Model.SaleTypesIds.Remove(1);
                }
            }
            string strKdsIds = String.Join(",", Model.KdsIds);
            string strSaleTypesIds = String.Join(",", Model.SaleTypesIds);
            string sql = @"SELECT od.Id AS OrderDetailsId, o.Id AS OrderId, o.[Day], o.EndOfDayId, o.OrderNo, 
                            o.ReceiptNo, od.[Status], od.StatusTS, od.PreparationTime, o.PosId, o.Total, od.ProductId, p.[Description] AS ProductDescr,
                            od.Qty, od.KdsId, od.SalesTypeId, st.[Description], o.OrderNotes, od.KitchenStatus, od.LoginStaffId
                            FROM OrderDetail AS od 
                            INNER JOIN [Order] AS o ON od.OrderId = o.Id
                            INNER JOIN Product AS p ON od.ProductId = p.Id
                            INNER JOIN SalesType AS st ON od.SalesTypeId = st.Id
                            WHERE o.EndOfDayId IS NULL AND ISNULL(o.IsDeleted, 0) = 0
                            AND od.[Status] = 1 AND od.KdsId IN (" + strKdsIds + ") AND od.SalesTypeId IN (" + strSaleTypesIds + @")
                            ORDER BY od.KitchenStatus DESC, od.SalesTypeId DESC, o.[Day] ASC";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = db.Query<KdsResponceModel>(sql).ToList();
                }
                catch(Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            foreach(KdsResponceModel item in res)
            {
                if(item.SalesTypeId == 1)
                {
                    item.SalesTypeId = 22;
                }
            }
            List<KdsResponceModel> orderedList = new List<KdsResponceModel>();
            orderedList = res.OrderByDescending(o => o.KitchenStatus).ThenByDescending(o => o.SalesTypeId).ThenBy(o => o.Day).ToList();
            return orderedList;
        }

        public OrderStatusModel GetOrderStatusModel(DBInfoModel dbInfo, long OrderId, byte? Status)
        {
            OrderStatusModel res = new OrderStatusModel();
            string sql = @"SELECT * FROM OrderStatus AS os WHERE os.OrderId =@orderId AND os.[Status] =@status";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = db.Query<OrderStatusModel>(sql, new { orderId = OrderId, status = Status }).FirstOrDefault();
                }
                catch(Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            return res;
        }

        public List<OrderDetailIngredientsModel> GetDetailIgredients(DBInfoModel dbInfo, long OrderDetailsId)
        {
            List<OrderDetailIngredientsModel> res = new List<OrderDetailIngredientsModel>();
            string sql = @"SELECT odi.*, i.[Description] AS IngredientDescr, i.DisplayOnKds
                            FROM OrderDetailIgredients AS odi
                            INNER JOIN Ingredients AS i ON odi.IngredientId = i.Id
                            WHERE odi.OrderDetailId =@orderDetailId";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = db.Query<OrderDetailIngredientsModel>(sql, new { orderDetailId = OrderDetailsId }).ToList();
                }
                catch(Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            return res;
        }

        public StaffModels GetOrderStaffModel(DBInfoModel dbInfo, long StaffId)
        {
            StaffModels staffModel = new StaffModels();
            string sql = @"SELECT * FROM Staff AS s WHERE s.Id =@staffId";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    staffModel = db.Query<StaffModels>(sql, new { staffId = StaffId }).FirstOrDefault();
                }
                catch(Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            return staffModel;
        }

        public string SetRemarks(DBInfoModel dbInfo, long OrderDetailsId, string OrderNotes)
        {
            string finalRemark = "";
            List<OrderDetailInvoicesModel> res = new List<OrderDetailInvoicesModel>();
            string sql = @"SELECT * 
                            FROM OrderDetailInvoices AS odi
                            WHERE odi.OrderDetailId =@orderDetailId";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = db.Query<OrderDetailInvoicesModel>(sql, new { orderDetailId = OrderDetailsId }).ToList();
                }
                catch(Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            if (!string.IsNullOrWhiteSpace(OrderNotes))
            {
                if (res != null)
                {
                    foreach (OrderDetailInvoicesModel item in res)
                    {
                        if (item.IsExtra == false)
                        {
                            if (string.IsNullOrWhiteSpace(item.ItemRemark))
                            {
                                finalRemark = OrderNotes;
                            }
                            else
                            {
                                finalRemark = item.ItemRemark + ". " + OrderNotes;
                            }
                        }
                    }
                }
            }
            else
            {
                if (res != null)
                {
                    foreach (OrderDetailInvoicesModel item in res)
                    {
                        if (item.IsExtra == false)
                        {
                            if (!string.IsNullOrWhiteSpace(item.ItemRemark))
                            {
                                finalRemark = item.ItemRemark;
                            }
                        }
                    }
                }
            }
            return finalRemark;
        }

        /// <summary>
        /// Get All Ingredients In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>List OrderDetailIngredientsKDSModel</returns>
        public List<OrderDetailIngredientsKDSModel> GetKdsIngredients(DBInfoModel dbInfo, KdsGetOrdersRequestModel Model)
        {
            List<OrderDetailIngredientsKDSModel> res = new List<OrderDetailIngredientsKDSModel>();
            if (Model.SaleTypesIds.Contains(22))
            {
                if (Model.SaleTypesIds.Contains(1) == false)
                {
                    Model.SaleTypesIds.Add(1);
                }
            }
            else
            {
                if (Model.SaleTypesIds.Contains(1))
                {
                    Model.SaleTypesIds.Remove(1);
                }
            }
            string strKdsIds = String.Join(",", Model.KdsIds);
            string strSaleTypesIds = String.Join(",", Model.SaleTypesIds);
            string sql = @"SELECT * 
                            FROM OrderDetailIgredientsKDS AS odik 
                            WHERE odik.KDSId IN (" + strKdsIds + ") AND odik.SalesTypeId IN (" + strSaleTypesIds + @")
                            ORDER BY odik.[Description] ASC";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = db.Query<OrderDetailIngredientsKDSModel>(sql).ToList();
                }
                catch(Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            return res;
        }

        public bool UpdateKitchenStatus(DBInfoModel dbInfo, KdsUpdateKitchenStatusRequestModel Model)
        {
            bool res = false;
            string sql = @"UPDATE OrderDetail SET KitchenStatus =@kitchenStatus, LoginStaffId=@loginStaffId WHERE id =@orderDetailId";
            string sqlUpdateAll = @"UPDATE OrderDetail SET KitchenStatus =@kitchenStatus, LoginStaffId=@loginStaffId WHERE OrderId =@orderId AND KitchenStatus != 0";
            string sqlUpdateAllWithReady = @"UPDATE OrderDetail SET KitchenStatus =@kitchenStatus, LoginStaffId=@loginStaffId WHERE OrderId =@orderId";
            string sqlDelete = @"DELETE FROM OrderDetailIgredientsKDS WHERE OrderDetailId =@orderDetailId";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    //KitchenStatus --> 0: Ready , 1:Pending , 2:Kitchen
                    if (Model.KitchenStatus == 0)
                    {
                        db.Execute(sql, new { kitchenStatus = Model.KitchenStatus, loginStaffId = Model.LoginStaffId, orderDetailId = Model.OrderDetailId });
                        db.Execute(sqlDelete, new { orderDetailId = Model.OrderDetailId });
                    }
                    else if(Model.KitchenStatus > 0 && Model.CurrentKitchenStatus == 0)
                    {
                        db.Execute(sqlUpdateAllWithReady, new { kitchenStatus = Model.KitchenStatus, loginStaffId = Model.LoginStaffId, orderId = Model.OrderId });
                    }
                    else
                    {
                        db.Execute(sqlUpdateAll, new { kitchenStatus = Model.KitchenStatus, loginStaffId = Model.LoginStaffId, orderId = Model.OrderId });
                    }
                    res = true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    res = false;
                }
            }
            return res;
        }

        public KdsTopRowResponceModel GetTopOrderDetailIngredientId(DBInfoModel dbInfo, KdsDeleteIngredientsRequestModel Model)
        {
            if (Model.SaleTypesIds.Contains(22))
            {
                if (Model.SaleTypesIds.Contains(1) == false)
                {
                    Model.SaleTypesIds.Add(1);
                }
            }
            else
            {
                if (Model.SaleTypesIds.Contains(1))
                {
                    Model.SaleTypesIds.Remove(1);
                }
            }
            string strKdsIds = String.Join(",", Model.KdsIds);
            string strSaleTypesIds = String.Join(",", Model.SaleTypesIds);
            KdsTopRowResponceModel topRowModel = new KdsTopRowResponceModel();
            string sql = @"SELECT odi.Id AS OrderDetailIgredientId, odi.Qty
                            FROM OrderDetail AS od 
                            INNER JOIN [Order] AS o ON od.OrderId = o.Id
                            INNER JOIN Product AS p ON od.ProductId = p.Id
                            INNER JOIN SalesType AS st ON od.SalesTypeId = st.Id
                            LEFT OUTER JOIN OrderDetailIgredientsKDS AS odi ON odi.OrderDetailId = od.Id
                            WHERE o.EndOfDayId IS NULL AND ISNULL(o.IsDeleted, 0) = 0
                            AND od.[Status] = 1 AND od.KdsId IN (" + strKdsIds + ") AND od.SalesTypeId IN (" + strSaleTypesIds + @")
                            AND odi.Qty > 0 AND odi.IgredientsId =@ingredientId
                            ORDER BY od.KitchenStatus DESC, od.SalesTypeId DESC, o.[Day] ASC";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    topRowModel = db.Query<KdsTopRowResponceModel>(sql, new { ingredientId = Model.IngredientId }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            return topRowModel;
        }

        public bool UpdatePendingQty(DBInfoModel dbInfo, KdsTopRowResponceModel topRowModel)
        {
            bool res = false;
            double newPendingQty = topRowModel.PendingQty - 1;
            string sql = @"UPDATE OrderDetailIgredientsKDS SET Qty =@pendingQty WHERE Id =@orderDetailIgredientId";
            string sqlDelete = @"DELETE FROM OrderDetailIgredientsKDS WHERE Id =@orderDetailIgredientId";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    if (newPendingQty > 0)
                    {
                        db.Execute(sql, new { pendingQty = newPendingQty, orderDetailIgredientId = topRowModel.OrderDetailIgredientId });
                    }
                    else
                    {
                        db.Execute(sqlDelete, new { orderDetailIgredientId = topRowModel.OrderDetailIgredientId });
                    }
                    res = true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    res = false;
                }
            }
            return res;
        }

        public List<long> GetKdsIds(DBInfoModel dbInfo)
        {
            List<long> KdsIds = new List<long>();
            string sql = @"SELECT k.id FROM Kds AS k";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    KdsIds = db.Query<long>(sql).ToList();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }
            return KdsIds;
        }

        public bool CheckRestOrderDetails(DBInfoModel dbInfo, KdsUpdateKitchenStatusRequestModel Model)
        {
            List<KdsResponceModel> OrderDetailsList = new List<KdsResponceModel>();
            bool sendSignalR = false;
            if (Model.SaleTypesIds.Contains(22))
            {
                if (Model.SaleTypesIds.Contains(1) == false)
                {
                    Model.SaleTypesIds.Add(1);
                }
            }
            else
            {
                if (Model.SaleTypesIds.Contains(1))
                {
                    Model.SaleTypesIds.Remove(1);
                }
            }
            string strKdsIds = String.Join(",", Model.KdsIds);
            string strSaleTypesIds = String.Join(",", Model.SaleTypesIds);
            string sql = @"SELECT od.Id AS OrderDetailsId, o.Id AS OrderId, o.[Day], o.EndOfDayId, o.OrderNo, 
                            o.ReceiptNo, od.[Status], od.StatusTS, od.PreparationTime, o.PosId, o.Total, od.ProductId, p.[Description] AS ProductDescr,
                            od.Qty, od.KdsId, od.SalesTypeId, st.[Description], o.OrderNotes, od.KitchenStatus, od.LoginStaffId
                            FROM OrderDetail AS od 
                            INNER JOIN [Order] AS o ON od.OrderId = o.Id
                            INNER JOIN Product AS p ON od.ProductId = p.Id
                            INNER JOIN SalesType AS st ON od.SalesTypeId = st.Id
                            WHERE o.EndOfDayId IS NULL AND ISNULL(o.IsDeleted, 0) = 0
                            AND od.[Status] = 1 AND od.KdsId IN (" + strKdsIds + ") AND od.SalesTypeId IN (" + strSaleTypesIds + @") AND o.Id =@orderId
                            ORDER BY od.KitchenStatus DESC, od.SalesTypeId DESC, o.[Day] ASC";

            string sql2 = @"UPDATE OrderDetail SET [Status] =@status WHERE OrderId =@orderId";

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    OrderDetailsList = db.Query<KdsResponceModel>(sql, new { orderId = Model.OrderId }).ToList();
                    if(OrderDetailsList != null)
                    {
                        bool readyFlag = true;
                        foreach(KdsResponceModel orderDtl in OrderDetailsList)
                        {
                            if(orderDtl.KitchenStatus != (int)KdsKitchenStatusEnums.Ready)
                            {
                                readyFlag = false;
                            }
                        }
                        if(readyFlag == true)
                        {
                            db.Execute(sql2, new { status = OrderStatusEnum.Ready, orderId = Model.OrderId });
                            //Set Flag to true in order to Send SignalR Message to Dispatcher and Print Receipt
                            sendSignalR = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    sendSignalR = false;
                    logger.Error(ex.ToString());
                }
            }
            return sendSignalR;
        }

        /// <summary>
        /// Insert on OrderDetailIgredientsKDS a returnd product based on OrderId , OrderDetailId and productid
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="orderId"></param>
        /// <param name="orderDetailId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool ReturnProductOnKdsTable(DBInfoModel dbInfo, long orderId, long orderDetailId, long productId)
        {
            bool result = true;
            string SQL = "";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    SQL = "INSERT INTO OrderDetailIgredientsKDS(OrderId,OrderDetailId,ProductId,IgredientsId,[Description],Qty,SalesTypeId,KDSId) \n"
                       + "SELECT fn.OrderId, fn.OrderDetailId, fn.ProductId, fn.IngredientsId,  \n"
                       + "		fn.IngredientsDescription, fn.Qty, fn.SalesTypeId, fn.KdsId  \n"
                       + "FROM (  \n"
                       + "	SELECT lst.OrderId, lst.OrderDetailId, lst.ProductId, lst.IngredientsId, lst.IngredientsDescription, SUM(lst.Qty) Qty, lst.SalesTypeId, lst.KdsId   \n"
                       + "	FROM (  \n"
                       + "		SELECT od.OrderId, od.Id OrderDetailId, od.ProductId ,i.Id IngredientsId, i.[Description] IngredientsDescription, (CASE WHEN ISNULL(i.Qty,0) = 1 THEN 1 ELSE i.Qty END * od.Qty) Qty, od.SalesTypeId, od.KdsId   \n"
                       + "		FROM OrderDetail AS od  \n"
                       + "		INNER JOIN ProductRecipe AS pr ON pr.ProductId = od.ProductId  \n"
                       + "		INNER JOIN Ingredients AS i ON i.Id = pr.IngredientId AND ISNULL(i.DisplayOnKds,0) <> 0  \n"
                       + "		WHERE od.OrderId = @OrderId AND od.Id = @OrderDetailId AND od.ProductId = @ProductId \n"
                       + "		UNION ALL  \n"
                       + "		SELECT od.OrderId, od.Id OrderDetailId, od.ProductId ,i.Id IngredientsId, i.[Description] IngredientsDescription, (odi.Qty * od.Qty) Qty, od.SalesTypeId, od.KdsId  \n"
                       + "		FROM OrderDetail AS od  \n"
                       + "		INNER JOIN OrderDetailIgredients AS odi ON odi.OrderDetailId = od.Id  \n"
                       + "		INNER JOIN Ingredients AS i ON i.Id = odi.IngredientId AND ISNULL(i.DisplayOnKds,0) <> 0  \n"
                       + "		WHERE od.OrderId = @OrderId AND od.Id = @OrderDetailId AND od.ProductId = @ProductId \n"
                       + "	) lst  \n"
                       + "	GROUP BY lst.OrderId, lst.OrderDetailId, lst.ProductId, lst.IngredientsId, lst.IngredientsDescription,lst.SalesTypeId, lst.KdsId   \n"
                       + ") fn  \n"
                       + "WHERE fn.Qty > 0 ";
                    db.Execute(SQL, new { OrderId = orderId, OrderDetailId = orderDetailId, ProductId = productId });
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("[SQL : " + SQL + "] \r\n" + ex.ToString());
            }
            return result;
        }
    }
}
