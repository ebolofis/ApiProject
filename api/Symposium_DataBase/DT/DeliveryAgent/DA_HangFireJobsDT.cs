using Dapper;
using log4net;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_HangFireJobsDT : IDA_HangFireJobsDT
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public DA_HangFireJobsDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Return's all DA Order Ids to Send To Stores
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<long> GetDAOrderIdsToSend(DBInfoModel Store)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string SQL = "SELECT DISTINCT do.ID, do.OrderDate FROM DA_Orders do WHERE ISNULL(do.IsSend,0) > 0 AND do.[Status] <> " + ((int)OrderStatusEnum.OnHold).ToString() + " ORDER BY do.OrderDate DESC ";
                return db.Query<long>(SQL).ToList();
            }
        }

        /// <summary>
        /// Get's Order To Send To Clients
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public OrderFromDAToClientForWebCallModel GetOrdersToSend(DBInfoModel Store, long DaOrderId)
        {
            StringBuilder SQL = new StringBuilder();
            OrderFromDAToClientForWebCallModel res = new OrderFromDAToClientForWebCallModel();
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    //SQL.Append("DECLARE @OrderIds TABLE (OrderId BIGINT, CustomerId BIGINT, BillindAddressId BIGINT, ShippingAddressId BIGINT, StoreId INT)   \n"
                    //       + "DECLARE @OrderDetailsIds TABLE (OrderId BIGINT)   \n"
                    //       + "   \n"
                    //       + "INSERT INTO @OrderIds(OrderId, CustomerId, BillindAddressId, ShippingAddressId, StoreId )   \n"
                    //       + "SELECT Id, do.CustomerId, do.BillingAddressId, do.ShippingAddressId, do.StoreId  \n"
                    //       + "FROM DA_Orders AS do  \n"
                    //       + "WHERE (do.IsSend > 0) OR  \n"
                    //       + "  (do.IsSend = 0 AND ISNULL(do.IsDelay,0) <> 0 AND CAST(CONVERT(VARCHAR(10), do.EstTakeoutDate, 120) AS DATETIME) >= CAST(CONVERT(VARCHAR(10), GETDATE(), 120) AS DATETIME)) \n"
                    //       + "ORDER BY do.OrderDate DESC   \n"
                    //       + "  \n"
                    //       + "INSERT INTO @OrderDetailsIds(OrderId)    \n"
                    //       + "SELECT DISTINCT dod.Id   \n"
                    //       + "FROM DA_OrderDetails AS dod    \n"
                    //       + "INNER JOIN @OrderIds o ON o.OrderId = dod.DAOrderId   \n"
                    //       + "   \n"
                    //       + "SELECT do.*, 5  ExtType, ds.WebApi + CASE WHEN SUBSTRING(ds.WebApi,LEN(ds.WebApi),1) <> '/' THEN '/' ELSE '' END+'api/v3/Orders/InsertDeliveryOrders' WebApi,   \n"
                    //       + "  CASE WHEN do.StoreOrderId < 1 THEN NULL ELSE CAST(do.StoreOrderId AS VARCHAR(100)) END ClientStoreId,    \n"
                    //       + "  ds.Username Username, ds.Password Password, ds.PosId PosId  \n"
                    //       + "FROM DA_Orders AS do    \n"
                    //       + "INNER JOIN @OrderIds o ON o.OrderId = do.Id   \n"
                    //       + "INNER JOIN DA_Stores AS ds ON ds.Id = o.StoreId  \n"
                    //       + "ORDER BY do.OrderDate DESC   \n"
                    //       + "	   \n"
                    //       + "SELECT dod.*    \n"
                    //       + "FROM DA_OrderDetails AS dod    \n"
                    //       + "INNER JOIN @OrderDetailsIds o ON o.OrderId = dod.Id   \n"
                    //       + "   \n"
                    //       + "SELECT dode.*    \n"
                    //       + "FROM DA_OrderDetailsExtras AS dode   \n"
                    //       + "INNER JOIN @OrderDetailsIds o ON o.OrderId = dode.OrderDetailId   \n"
                    //       + "  \n"
                    //       + "SELECT c.OrderId, dc.*   \n"
                    //       + "FROM DA_Customers AS dc  \n"
                    //       + "INNER JOIN @OrderIds c ON c.CustomerId = dc.Id  \n"
                    //       + "  \n"
                    //       + "SELECT da.*, 'true' isShipping, o.OrderId    \n"
                    //       + "FROM DA_Addresses AS da   \n"
                    //       + "INNER JOIN @OrderIds o ON o.ShippingAddressId = da.Id  \n"
                    //       + "UNION ALL  \n"
                    //       + "SELECT da.*, 'false' isShipping, o.OrderId    \n"
                    //       + "FROM DA_Addresses AS da   \n"
                    //       + "INNER JOIN @OrderIds o ON o.BillindAddressId = da.Id");
                    SQL.Append("DECLARE @OrderIds TABLE (OrderId BIGINT, CustomerId BIGINT, BillindAddressId BIGINT, ShippingAddressId BIGINT, StoreId INT)     \n"
                           + "DECLARE @OrderDetailsIds TABLE (OrderId BIGINT)     \n"
                           + "     \n"
                           + "INSERT INTO @OrderIds(OrderId, CustomerId, BillindAddressId, ShippingAddressId, StoreId )     \n"
                           + "SELECT Id, do.CustomerId, do.BillingAddressId, do.ShippingAddressId, do.StoreId    \n"
                           + "FROM DA_Orders AS do    \n"
                           + "WHERE do.IsSend > 0 AND do.ID = " + DaOrderId.ToString() + " \n"
                           + "ORDER BY do.OrderDate DESC     \n"
                           + "    \n"
                           + "INSERT INTO @OrderDetailsIds(OrderId)      \n"
                           + "SELECT DISTINCT dod.Id     \n"
                           + "FROM DA_OrderDetails AS dod      \n"
                           + "INNER JOIN @OrderIds o ON o.OrderId = dod.DAOrderId     \n"
                           + "     \n"
                           + "SELECT do.*, 5  ExtType, ds.WebApi + CASE WHEN SUBSTRING(ds.WebApi,LEN(ds.WebApi),1) <> '/' THEN '/' ELSE '' END+'api/v3/Orders/InsertDeliveryOrders' WebApi,     \n"
                           //+ "  CASE WHEN do.StoreOrderId < 1 THEN NULL ELSE CAST(do.StoreOrderId AS VARCHAR(100)) END ClientStoreId,      \n"
                           + "  ds.Username Username, ds.Password Password, ds.PosId PosId    \n"
                           + "FROM DA_Orders AS do      \n"
                           + "INNER JOIN @OrderIds o ON o.OrderId = do.Id     \n"
                           + "INNER JOIN DA_Stores AS ds ON ds.Id = o.StoreId    \n"
                           + "ORDER BY do.OrderDate DESC     \n"
                           + "	     \n"
                           + "SELECT dod.*      \n"
                           + "FROM DA_OrderDetails AS dod      \n"
                           + "INNER JOIN @OrderDetailsIds o ON o.OrderId = dod.Id     \n"
                           + "     \n"
                           + "SELECT dode.*      \n"
                           + "FROM DA_OrderDetailsExtras AS dode     \n"
                           + "INNER JOIN @OrderDetailsIds o ON o.OrderId = dode.OrderDetailId     \n"
                           + "    \n"
                           + "SELECT c.OrderId, dc.*     \n"
                           + "FROM DA_Customers AS dc    \n"
                           + "INNER JOIN @OrderIds c ON c.CustomerId = dc.Id    \n"
                           + "    \n"
                           + "SELECT da.*, 'true' isShipping, o.OrderId      \n"
                           + "FROM DA_Addresses AS da     \n"
                           + "INNER JOIN @OrderIds o ON o.ShippingAddressId = da.Id    \n"
                           + "UNION ALL    \n"
                           + "SELECT da.*, 'false' isShipping, o.OrderId      \n"
                           + "FROM DA_Addresses AS da     \n"
                           + "INNER JOIN @OrderIds o ON o.BillindAddressId = da.Id   \n"
                           + "  \n"
                           + "SELECT DISTINCT ds.*   \n"
                           + "FROM DA_Stores AS ds  \n"
                           + "INNER JOIN DA_Orders AS do ON do.StoreId = ds.Id  \n"
                           + "INNER JOIN @OrderIds o ON o.OrderId = do.Id");


                    using (var multipleResult = db.QueryMultiple(SQL.ToString()))
                    {
                        res.Order = multipleResult.Read<DA_OrderModel>().FirstOrDefault();
                        res.OrderDetails = multipleResult.Read<DA_OrderDetails>().ToList();
                        res.OrderDetailExtras = multipleResult.Read<DA_OrderDetailsExtrasModel>().ToList();
                        res.Customer = multipleResult.Read<DACustomerModel>().FirstOrDefault();
                        res.Addresses = multipleResult.Read<DA_AddressModel>().ToList();
                        res.StoreModel = multipleResult.Read<DA_StoreModel>().FirstOrDefault();
                    }


                }

            }
            catch (Exception ex)
            {
                logger.Error("GetOrdersToSend [SQL : " + SQL.ToString() + "] \r\n" + ex.ToString());
            }
            return res;
        }

        /// <summary>
        /// Geting Customer With list of addresses from DA Server
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public List<DASearchCustomerModel> GetCustomer(DBInfoModel Store, List<long> CustomerId, ExternalSystemOrderEnum ExtType)
        {
            //List<DASearchCustomerModel> custModel = new List<DASearchCustomerModel>();
            List<DASearchCustomerModel> ret = new List<DASearchCustomerModel>();
            string SQL = "";
            string custIds = "";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                List<DA_AddressModel> tmpCustList = new List<DA_AddressModel>();

                foreach (long item in CustomerId)
                    custIds += item.ToString() + ",";
                if (!string.IsNullOrEmpty(custIds))
                    custIds = custIds.Substring(0, custIds.Length - 1);
                else
                    custIds = "-1";

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    SQL = "SELECT * FROM DA_Customers WHERE ID IN (" + custIds + ") \n"
                        + "SELECT * FROM DA_Addresses WHERE OwnerId IN (" + custIds + ")";

                    using (var multiRes = db.QueryMultiple(SQL))
                    {
                        List<DACustomerModel> custM = multiRes.Read<DACustomerModel>().ToList();
                        List<DA_AddressModel> custAdr = multiRes.Read<DA_AddressModel>().ToList();

                        foreach (DACustomerModel item in custM)
                        {
                            DASearchCustomerModel obj = new DASearchCustomerModel();
                            obj.CustomerId = item.Id;
                            obj.daCustomerModel = item;
                            obj.daAddrModel = custAdr.Where(w => w.OwnerId == item.Id).Select(s => s).ToList();
                            obj.ExtType = (int)ExtType;
                            ret.Add(obj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ret = null;
                logger.Error("GetCustomer [SQL: " + SQL + "] \r\n" + ex.ToString());
            }
            return ret;
        }

        /// <summary>
        /// Retruns Da Store Model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public DA_StoreModel GetStore(DBInfoModel Store, long StoreId)
        {
            string SQL = "";
            DA_StoreModel ret = new DA_StoreModel();
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    SQL = "SELECT * FROM DA_Stores WHERE Id = " + StoreId.ToString();
                    ret = db.Query<DA_StoreModel>(SQL).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetStore [SQL: " + SQL + "] \r\n" + ex.ToString());
                ret = null;
            }
            return ret;
        }

        /// <summary>
        /// Return's s list of da_stores
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<DA_StoreModel> GetStoresList(DBInfoModel Store)
        {
            string SQL = "";
            List<DA_StoreModel> ret = new List<DA_StoreModel>();
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    SQL = "SELECT * FROM DA_Stores";
                    ret = db.Query<DA_StoreModel>(SQL).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetStoresList [SQL: " + SQL + "] \r\n" + ex.ToString());
                ret = null;
            }
            return ret;
        }

        /// <summary>
        /// Delete's Records from Scheduled Taskes with FaildNo bigger than DelAfter parameter
        /// FaildNo Number of Failed
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DelAfter"></param>
        public void DeleteSchedulerAfterFaild(DBInfoModel Store, int DelAfter)
        {
            string SQL = "";
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    SQL = "DELETE FROM DA_ScheduledTaskes WHERE FaildNo >= " + DelAfter.ToString();
                    CommandDefinition cmd = new CommandDefinition(SQL);
                    db.Execute(cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error("DeleteSchedulerAfterFaild [SQL: " + SQL + "] \r\n" + ex.ToString());
            }
        }

        /// <summary>
        /// Update's Order From Server with IsSend Action
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public bool UpdateOrderWithSendStatus(DBInfoModel Store, long OrderId, long StoreOrderId, long StoreOrderNo, short StoreStatus, out string Error)
        {
            string SQL = "";
            Error = "";
            bool ret = true;
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    SQL = "UPDATE DA_Orders SET IsSend = CASE WHEN IsSend > 0 THEN IsSend - 1 ELSE 0 END, \n "
                         + "   ItemsChanged = CASE WHEN IsSend > 0 AND IsSend - 1 = 0 THEN 0 ELSE ItemsChanged END, \n"
                         + "   StoreOrderId = CASE WHEN ISNULL(StoreOrderId,0) = 0 THEN " + StoreOrderId + " ELSE StoreOrderId END, \n"
                         + "   StoreOrderNo = CASE WHEN ISNULL(StoreOrderNo,0) = 0 THEN " + StoreOrderNo + " ELSE StoreOrderNo END, \n"
                         + "   Status = " + StoreStatus + ", \n"
                         + "   ErrorMessage = '' \n"
                         + "WHERE Id = " + OrderId.ToString();
                    CommandDefinition cmd = new CommandDefinition(SQL);
                    db.Execute(cmd);
                }
            }
            catch (Exception ex)
            {
                logger.Error("UpdateOrderWithSendStatus [SQL : " + SQL + "] \r\n " + ex.ToString());
                Error = "UpdateOrderWithSendStatus [SQL : " + SQL + "] \r\n " + ex.ToString();
            }
            return ret;
        }

        /// <summary>
        /// Update DA_Order With Error Message
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="ErrorMess"></param>
        /// <returns></returns>
        public bool SetErrorToDA_Order(DBInfoModel Store, long OrderId, string ErrorMess)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                ErrorMess = ErrorMess.Replace("'", "");
                string SQL = "UPDATE DA_Orders SET ErrorMessage = '" + ErrorMess + "' \n"
                     + "WHERE Id = " + OrderId.ToString();
                CommandDefinition cmd = new CommandDefinition(SQL);
                db.Execute(cmd);
            }
            return true;
        }

        /// <summary>
        /// Execute's an SQL command
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="SQLComm"></param>
        /// <returns></returns>
        private bool ExecuteSQLCommand(DBInfoModel Store, string SQLComm)
        {
            try
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    CommandDefinition cmd = new CommandDefinition(SQLComm);
                    db.Execute(cmd);
                    return true;
                }
            }
            catch(Exception ex)
            {
                logger.Error("ExecuteSQLCommand [SQL : " + SQLComm + "] \r\n" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Delete Keys from DA Scheduler Taskes
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="sKeys"></param>
        /// <returns></returns>
        public bool DeleteSchedulerKeys(DBInfoModel Store, string sKeys)
        {
            string sCommand = "DELETE FROM DA_ScheduledTaskes WHERE ID IN (" + sKeys + ")";
            return ExecuteSQLCommand(Store, sCommand);
        }

        /// <summary>
        /// Increase FailNo to not succeded records
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="StoreId"></param>
        /// <param name="ShortId"></param>
        /// <returns></returns>
        public bool UpdateFaildNos(DBInfoModel Store, long StoreId, int ShortId)
        {
            string sCommand = "UPDATE DA_ScheduledTaskes SET FaildNo = FaildNo + 1 WHERE StoreId = " + StoreId.ToString() + " AND Short = " + ShortId.ToString();
            return ExecuteSQLCommand(Store, sCommand);
        }

        public void DeleteOnHoldOrders(DBInfoModel Store)
        {
            //OrderStatusEnum.OnHold
        }
    }
}
