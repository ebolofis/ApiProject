using Dapper;
using log4net;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.Payroll;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Configuration;

namespace Symposium.WebApi.DataAccess.DT
{
    public class DeliveryOrdersDT : IDeliveryOrdersDT
    {
        string connectionString;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IGenericDAO<StatusCounts> genStatusCnt;
        IGenericDAO<DeliveryStatusOrders> genDeliveryStatusOrders;
        IUsersToDatabasesXML usersToDatabases;

        public DeliveryOrdersDT(IUsersToDatabasesXML _usersToDatabases
            , IGenericDAO<StatusCounts> _genStatusCnt
            , IGenericDAO<DeliveryStatusOrders> _genDeliveryStatusOrders
            )
        {
            this.usersToDatabases = _usersToDatabases;
            this.genStatusCnt = _genStatusCnt;
            this.genDeliveryStatusOrders = _genDeliveryStatusOrders;
        }

        /// <summary>
        /// Get A List of Order Status Time Changes
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<OrderStatusModel> GetOrderStatusTimeChanges(DBInfoModel store, long OrderId)
        {
            List<OrderStatusModel> orderStatusTimeList = new List<OrderStatusModel>();

            string select = "SELECT * FROM OrderStatus AS os WHERE os.OrderId =@orderId ORDER BY os.Id DESC";

            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    orderStatusTimeList = db.Query<OrderStatusModel>(select, new { orderId = OrderId }).ToList();
                    return orderStatusTimeList;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception("Error to Load Order Status Time Changes");
                }
            }
        }


        /// <summary>
        /// Providing filters apply flat data collection for Order Statuses 
        /// then returns a Status obj by state count and for all open -1 with count
        /// </summary>
        /// <param name="store"></param>
        /// <param name="flts"></param>
        /// <returns></returns>
        public List<StatusCounts> StatesCounts(DBInfoModel store, DeliveryFilters flts)
        {
            string slts = SelectedIdsToString(flts.SelectedSalesTypes);
            string exts = SelectedIdsToString(flts.ExternalTypes);
            string mainquery = MainStateSelectionQuery(slts, exts);
            /*Counter*/
            string select = "SELECT [Status], COUNT([Status]) OrdersCount  \n"
               + "FROM @orders \n"
               + "GROUP BY [Status] \n"
               + "UNION ALL \n"
               + "SELECT -1, COUNT([Status]) OrdersCount \n"
               + "FROM @orders \n"
               + "WHERE [Status] NOT IN (5,6,7) \n"
               + "UNION ALL \n"
               + "SELECT -2, COUNT([Status]) OrdersCount \n"
               + "FROM @orders \n"
               + " \n";
            string q = mainquery + select;

            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    List<StatusCounts> res = new List<StatusCounts>();
                    res = genStatusCnt.Select(q, "", db);
                    return res;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DELIVERYORDERSSTATECOUNTS);
                }
            }
        }
        /// <summary>
        /// Given a long array returns string "ID1, ID2 , ID3, ..."
        /// </summary>
        /// <param name="flts"></param>
        /// <returns></returns>
        public string SelectedIdsToString(List<long> list)
        {
            string retstr = "";
            if (list != null && list.Count() > 0)
            {
                retstr = string.Join(",", list);
            }
            return retstr;
        }
        public string MainStateSelectionQuery(string slts, string exts)
        {
            string sql = "";
            bool newDeliveryMask = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "NewDeliveryMask");
            if (newDeliveryMask)
            {
                sql = "DECLARE @invoices TABLE (ID INT IDENTITY(1,1), InvoiceId INT, OrderId INT, ExtType INT, InvoiceType INT, SalesType INT, Paidstatus INT) \n"
             + " \n"
             + "INSERT INTO @invoices (InvoiceId, OrderId, SalesType, Paidstatus) \n"
             + "SELECT DISTINCT odi.InvoicesId, od.OrderId, odi.SalesTypeId, od.PaidStatus \n"
             + "FROM OrderDetailInvoices AS odi \n"
             + "CROSS APPLY ( \n"
             + "	SELECT DISTINCT od.OrderId, od.PaidStatus \n"
             + "	FROM OrderDetail AS od  \n"
             + "	WHERE od.Id = odi.OrderDetailId	 \n"
             + ") od \n"
             + "WHERE odi.SalesTypeId != 22 AND odi.EndOfDayId IS NULL AND ISNULL(odi.IsDeleted,0) = 0 \n"
             + " \n"
             + "UPDATE iv SET iv.InvoiceType = i.InvoiceTypeId , iv.ExtType = ISNULL(o.ExtType,0) \n"
             + "FROM @invoices iv \n"
             + "INNER JOIN Invoices AS i ON i.Id = iv.InvoiceId \n"
             + "INNER JOIN [Order] AS o ON o.Id = iv.OrderId \n"
             + " \n"
             + "DECLARE @orders TABLE(ID INT IDENTITY(1, 1), OrderId INT, TimeStant DATETIME, [Status] INT, SalesTypeId INT, Paidstatus INT)   \n"
             + " \n"
             + "DECLARE @ordersTmp TABLE(ID INT IDENTITY(1, 1), OrderNo INT, TimeStant DATETIME, [Status] INT, SalesTypeId INT, Paidstatus INT)  \n"
             + " \n"
             + "INSERT INTO @orders(OrderId, TimeStant,[Status], SalesTypeId, Paidstatus)  \n"
             + "SELECT DISTINCT os.OrderId, os.TimeChanged, os.[status], i.SalesType, i.Paidstatus  \n"
             + "FROM OrderStatus os  \n"
             + "INNER JOIN @invoices i ON i.OrderId = os.OrderId \n"
             + "INNER JOIN InvoiceTypes it ON it.Id = i.InvoiceType  \n"
             + "INNER JOIN InvoiceShippingDetails isp ON isp.InvoicesId = i.InvoiceId and  \n"
             + "    CASE WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') <> '' THEN 'true' \n"
             + "         WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') = '' THEN 'false' \n"
             + "         WHEN i.SalesType = 21 THEN 'true'  END = 'true' \n"
             + "INNER JOIN [Order] AS o ON o.Id = os.OrderId AND o.ExtType = 5 AND o.ExtObj IS NOT NULL  \n"
             + "WHERE os.Status = 5";
                if (!string.IsNullOrEmpty(slts))
                {
                    sql += " AND i.SalesType IN (" + slts + ") \n";
                }
                if (!string.IsNullOrEmpty(exts))
                {
                    sql += " AND i.ExtType IN (" + exts + ") \n";
                }
                sql += " \n"
                + "INSERT INTO @orders(OrderId, TimeStant,[Status], SalesTypeId, Paidstatus)  \n"
                + "SELECT DISTINCT os.OrderId, os.TimeChanged, os.[status] , i.SalesType, i.Paidstatus \n"
                + "FROM OrderStatus os  \n"
                + "INNER JOIN @invoices i ON i.OrderId = os.OrderId \n"
                + "INNER JOIN InvoiceTypes it ON it.Id = i.InvoiceType \n"
                + "INNER JOIN InvoiceShippingDetails isp ON isp.InvoicesId = i.InvoiceId AND \n"
                + "    CASE WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') <> '' THEN 'true' \n"
                + "         WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') = '' THEN 'false' \n"
                + "         WHEN i.SalesType = 21 THEN 'true'  END = 'true' \n"
                + "LEFT OUTER JOIN @orders oss ON oss.OrderId = os.OrderId  \n"
                + "INNER JOIN [Order] AS o ON o.Id = os.OrderId AND o.ExtType = 5 AND o.ExtObj IS NOT NULL  \n"
                + "WHERE os.Status = 6 and oss.ID IS NULL ";
                if (!string.IsNullOrEmpty(slts))
                {
                    sql += " AND i.SalesType IN (" + slts + ") \n";
                }
                if (!string.IsNullOrEmpty(exts))
                {
                    sql += " AND i.ExtType IN (" + exts + ") \n";
                }
                sql += "\n"
                   + " \n"
                   + "INSERT INTO @ordersTmp(OrderNo, TimeStant,[Status], SalesTypeId, Paidstatus)  \n"
                   + "SELECT DISTINCT os.OrderId,os.TimeChanged, os.[status], i.SalesType, i.Paidstatus  \n"
                   + "FROM OrderStatus os  \n"
                   + "INNER JOIN @invoices i ON i.OrderId = os.OrderId \n"
                   + "INNER JOIN InvoiceTypes it ON it.Id = i.InvoiceType  \n"
                   + "INNER JOIN InvoiceShippingDetails isp ON isp.InvoicesId = i.InvoiceId AND  \n"
                   + "    CASE WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') <> '' THEN 'true' \n"
                   + "         WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') = '' THEN 'false' \n"
                   + "         WHEN i.SalesType = 21 THEN 'true'  END = 'true' \n"
                   + "LEFT OUTER JOIN @orders oss ON oss.OrderId = os.OrderId  \n"
                   + "INNER JOIN [Order] AS o ON o.Id = os.OrderId AND o.ExtType = 5 AND o.ExtObj IS NOT NULL  \n"
                   + "WHERE os.Status NOT IN(6, 5) AND oss.ID IS NULL ";
                if (!string.IsNullOrEmpty(slts))
                {
                    sql += " AND i.SalesType IN (" + slts + ") \n";
                }
                if (!string.IsNullOrEmpty(exts))
                {
                    sql += " AND i.ExtType IN (" + exts + ") \n";
                }
                sql += "\n"
                   + "INSERT INTO @orders(OrderId, TimeStant, [Status], SalesTypeId, Paidstatus)   \n"
                   + "SELECT ot.OrderNo, ot.TimeStant, ot.Status, ot.SalesTypeId,ot.Paidstatus  \n"
                   + "FROM @ordersTmp ot  \n"
                   + "INNER JOIN (  \n"
                   + "	SELECT otmp.OrderNo, otmp.TimeStant, MAX(otmp.[Status]) [Status]  \n"
                   + "	FROM @ordersTmp otmp  \n"
                   + "	INNER JOIN (  \n"
                   + "		SELECT OrderNo, MAX(TimeStant) TimeStant  \n"
                   + "		FROM @ordersTmp  \n"
                   + "		GROUP BY OrderNo  \n"
                   + "	) otm ON otm.OrderNo = otmp.OrderNo AND otm.TimeStant = otmp.TimeStant  \n"
                   + "	GROUP BY otmp.OrderNo, otmp.TimeStant   \n"
                   + ") ott ON ott.OrderNo = ot.OrderNo AND ott.TimeStant = ot.TimeStant AND ott.[Status] = ot.[Status]  \n"
                   + " \n"
                   + " \n"
                   + "DELETE FROM @ordersTmp  \n"
                   + " \n"
                   + "INSERT INTO @ordersTmp(OrderNo, [Status], SalesTypeId, Paidstatus)  \n"
                   + "SELECT DISTINCT o.InvoiceId ,odi.PricelistId PricelistId, o.SalesType, o.Paidstatus   \n"
                   + "FROM @invoices o  \n"
                   + "INNER JOIN @orders os ON  os.OrderId = o.OrderId  \n"
                   + "INNER JOIN OrderDetailInvoices odi ON odi.InvoicesId = o.InvoiceId ";
                if (!string.IsNullOrEmpty(slts))
                {
                    sql += " AND o.SalesType IN (" + slts + ") \n ";
                }
                if (!string.IsNullOrEmpty(exts))
                {
                    sql += " AND o.ExtType IN (" + exts + ") \n";
                }
                sql += "\n"
                   + " \n"
                   + "DECLARE @idx INT, @tRec INT, @OrderNo INT, @Status INT  \n"
                   + " \n"
                   + "SELECT @idx = min(ID), @tRec = max(ID)  \n"
                   + "FROM @ordersTmp \n"
                   + " \n"
                   + "DECLARE @priceListsTable TABLE(InvoiceId INT, PriceLists VARCHAR(100))  \n"
                   + " \n"
                   + "WHILE @idx <= @tRec  \n"
                   + "BEGIN \n"
                   + "	SELECT @OrderNo = OrderNo, @Status = ISNULL([Status],-1)  \n"
                   + "	FROM @ordersTmp \n"
                   + "	 WHERE id = @idx  \n"
                   + "	  \n"
                   + "	 IF NOT EXISTS (SELECT 1 FROM @priceListsTable WHERE InvoiceId = @OrderNo)  \n"
                   + "		INSERT INTO @priceListsTable(InvoiceId, PriceLists) VALUES (@OrderNo, CAST(@Status AS VARCHAR(100)) + ',')  \n"
                   + "	ELSE  \n"
                   + "	IF EXISTS (SELECT 1 FROM @priceListsTable WHERE InvoiceId = @OrderNo and CHARINDEX(CAST(@Status AS VARCHAR(10)) + ',', PriceLists) <= 0)  \n"
                   + "		UPDATE @priceListsTable SET PriceLists = PriceLists + CAST(@Status AS VARCHAR(100)) + ','  \n"
                   + "		WHERE InvoiceId = @OrderNo  \n"
                   + "		 \n"
                   + "	SET @idx = @idx + 1  \n"
                   + "END  \n";
            }
            else
            {
                sql = "DECLARE @invoices TABLE (ID INT IDENTITY(1,1), InvoiceId INT, OrderId INT, ExtType INT, InvoiceType INT, SalesType INT, Paidstatus INT) \n"
          + " \n"
          + "INSERT INTO @invoices (InvoiceId, OrderId, SalesType, Paidstatus) \n"
          + "SELECT DISTINCT odi.InvoicesId, od.OrderId, odi.SalesTypeId, od.PaidStatus \n"
          + "FROM OrderDetailInvoices AS odi \n"
          + "CROSS APPLY ( \n"
          + "	SELECT DISTINCT od.OrderId, od.PaidStatus \n"
          + "	FROM OrderDetail AS od  \n"
          + "	WHERE od.Id = odi.OrderDetailId	 \n"
          + ") od \n"
          + "WHERE odi.EndOfDayId IS NULL AND ISNULL(odi.IsDeleted,0) = 0 \n"
          + " \n"
          + "UPDATE iv SET iv.InvoiceType = i.InvoiceTypeId , iv.ExtType = ISNULL(o.ExtType,0) \n"
          + "FROM @invoices iv \n"
          + "INNER JOIN Invoices AS i ON i.Id = iv.InvoiceId \n"
          + "INNER JOIN [Order] AS o ON o.Id = iv.OrderId \n"
          + " \n"
          + "DECLARE @orders TABLE(ID INT IDENTITY(1, 1), OrderId INT, TimeStant DATETIME, [Status] INT, SalesTypeId INT, Paidstatus INT)   \n"
          + " \n"
          + "DECLARE @ordersTmp TABLE(ID INT IDENTITY(1, 1), OrderNo INT, TimeStant DATETIME, [Status] INT, SalesTypeId INT, Paidstatus INT)  \n"
          + " \n"
          + "INSERT INTO @orders(OrderId, TimeStant,[Status], SalesTypeId, Paidstatus)  \n"
          + "SELECT DISTINCT os.OrderId, os.TimeChanged, os.[status], i.SalesType, i.Paidstatus  \n"
          + "FROM OrderStatus os  \n"
          + "INNER JOIN @invoices i ON i.OrderId = os.OrderId \n"
          + "INNER JOIN InvoiceTypes it ON it.Id = i.InvoiceType  \n"
          + "INNER JOIN InvoiceShippingDetails isp ON isp.InvoicesId = i.InvoiceId and  \n"
          + "    CASE WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') <> '' THEN 'true' \n"
          + "         WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') = '' THEN 'false' \n"
          + "         WHEN i.SalesType = 21 THEN 'true'  END = 'true' \n"
          + "INNER JOIN [Order] AS o ON o.Id = os.OrderId \n"
          + "WHERE os.Status = 5";
                if (!string.IsNullOrEmpty(slts))
                {
                    sql += " AND i.SalesType IN (" + slts + ") \n";
                }
                if (!string.IsNullOrEmpty(exts))
                {
                    sql += " AND i.ExtType IN (" + exts + ") \n";
                }
                sql += " \n"
                + "INSERT INTO @orders(OrderId, TimeStant,[Status], SalesTypeId, Paidstatus)  \n"
                + "SELECT DISTINCT os.OrderId, os.TimeChanged, os.[status] , i.SalesType, i.Paidstatus \n"
                + "FROM OrderStatus os  \n"
                + "INNER JOIN @invoices i ON i.OrderId = os.OrderId \n"
                + "INNER JOIN InvoiceTypes it ON it.Id = i.InvoiceType \n"
                + "INNER JOIN InvoiceShippingDetails isp ON isp.InvoicesId = i.InvoiceId AND \n"
                + "    CASE WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') <> '' THEN 'true' \n"
                + "         WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') = '' THEN 'false' \n"
                + "         WHEN i.SalesType = 21 THEN 'true'  END = 'true' \n"
                + "LEFT OUTER JOIN @orders oss ON oss.OrderId = os.OrderId  \n"
                + "INNER JOIN [Order] AS o ON o.Id = os.OrderId \n"
                + "WHERE os.Status = 6 and oss.ID IS NULL ";
                if (!string.IsNullOrEmpty(slts))
                {
                    sql += " AND i.SalesType IN (" + slts + ") \n";
                }
                if (!string.IsNullOrEmpty(exts))
                {
                    sql += " AND i.ExtType IN (" + exts + ") \n";
                }
                sql += "\n"
                   + " \n"
                   + "INSERT INTO @ordersTmp(OrderNo, TimeStant,[Status], SalesTypeId, Paidstatus)  \n"
                   + "SELECT DISTINCT os.OrderId,os.TimeChanged, os.[status], i.SalesType, i.Paidstatus  \n"
                   + "FROM OrderStatus os  \n"
                   + "INNER JOIN @invoices i ON i.OrderId = os.OrderId \n"
                   + "INNER JOIN InvoiceTypes it ON it.Id = i.InvoiceType  \n"
                   + "INNER JOIN InvoiceShippingDetails isp ON isp.InvoicesId = i.InvoiceId AND  \n"
                   + "    CASE WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') <> '' THEN 'true' \n"
                   + "         WHEN i.SalesType <> 21 AND ISNULL(isp.ShippingAddress,'') = '' THEN 'false' \n"
                   + "         WHEN i.SalesType = 21 THEN 'true'  END = 'true' \n"
                   + "LEFT OUTER JOIN @orders oss ON oss.OrderId = os.OrderId  \n"
                   + "INNER JOIN [Order] AS o ON o.Id = os.OrderId \n"
                   + "WHERE os.Status NOT IN(6, 5) AND oss.ID IS NULL ";
                if (!string.IsNullOrEmpty(slts))
                {
                    sql += " AND i.SalesType IN (" + slts + ") \n";
                }
                if (!string.IsNullOrEmpty(exts))
                {
                    sql += " AND i.ExtType IN (" + exts + ") \n";
                }
                sql += "\n"
                   + "INSERT INTO @orders(OrderId, TimeStant, [Status], SalesTypeId, Paidstatus)   \n"
                   + "SELECT ot.OrderNo, ot.TimeStant, ot.Status, ot.SalesTypeId,ot.Paidstatus  \n"
                   + "FROM @ordersTmp ot  \n"
                   + "INNER JOIN (  \n"
                   + "	SELECT otmp.OrderNo, otmp.TimeStant, MAX(otmp.[Status]) [Status]  \n"
                   + "	FROM @ordersTmp otmp  \n"
                   + "	INNER JOIN (  \n"
                   + "		SELECT OrderNo, MAX(TimeStant) TimeStant  \n"
                   + "		FROM @ordersTmp  \n"
                   + "		GROUP BY OrderNo  \n"
                   + "	) otm ON otm.OrderNo = otmp.OrderNo AND otm.TimeStant = otmp.TimeStant  \n"
                   + "	GROUP BY otmp.OrderNo, otmp.TimeStant   \n"
                   + ") ott ON ott.OrderNo = ot.OrderNo AND ott.TimeStant = ot.TimeStant AND ott.[Status] = ot.[Status]  \n"
                   + " \n"
                   + " \n"
                   + "DELETE FROM @ordersTmp  \n"
                   + " \n"
                   + "INSERT INTO @ordersTmp(OrderNo, [Status], SalesTypeId, Paidstatus)  \n"
                   + "SELECT DISTINCT o.InvoiceId ,odi.PricelistId PricelistId, o.SalesType, o.Paidstatus   \n"
                   + "FROM @invoices o  \n"
                   + "INNER JOIN @orders os ON  os.OrderId = o.OrderId  \n"
                   + "INNER JOIN OrderDetailInvoices odi ON odi.InvoicesId = o.InvoiceId ";
                if (!string.IsNullOrEmpty(slts))
                {
                    sql += " AND o.SalesType IN (" + slts + ") \n ";
                }
                if (!string.IsNullOrEmpty(exts))
                {
                    sql += " AND o.ExtType IN (" + exts + ") \n";
                }
                sql += "\n"
                   + " \n"
                   + "DECLARE @idx INT, @tRec INT, @OrderNo INT, @Status INT  \n"
                   + " \n"
                   + "SELECT @idx = min(ID), @tRec = max(ID)  \n"
                   + "FROM @ordersTmp \n"
                   + " \n"
                   + "DECLARE @priceListsTable TABLE(InvoiceId INT, PriceLists VARCHAR(100))  \n"
                   + " \n"
                   + "WHILE @idx <= @tRec  \n"
                   + "BEGIN \n"
                   + "	SELECT @OrderNo = OrderNo, @Status = ISNULL([Status],-1)  \n"
                   + "	FROM @ordersTmp \n"
                   + "	 WHERE id = @idx  \n"
                   + "	  \n"
                   + "	 IF NOT EXISTS (SELECT 1 FROM @priceListsTable WHERE InvoiceId = @OrderNo)  \n"
                   + "		INSERT INTO @priceListsTable(InvoiceId, PriceLists) VALUES (@OrderNo, CAST(@Status AS VARCHAR(100)) + ',')  \n"
                   + "	ELSE  \n"
                   + "	IF EXISTS (SELECT 1 FROM @priceListsTable WHERE InvoiceId = @OrderNo and CHARINDEX(CAST(@Status AS VARCHAR(10)) + ',', PriceLists) <= 0)  \n"
                   + "		UPDATE @priceListsTable SET PriceLists = PriceLists + CAST(@Status AS VARCHAR(100)) + ','  \n"
                   + "		WHERE InvoiceId = @OrderNo  \n"
                   + "		 \n"
                   + "	SET @idx = @idx + 1  \n"
                   + "END  \n";
            }
            return sql;
        }


        public PaginationModel<DeliveryStatusOrders> PagedOrdersByState(DBInfoModel store, int status, int pageNumber, int pageLength, DeliveryFilters flts)
        {
            string slts = SelectedIdsToString(flts.SelectedSalesTypes);
            string exts = SelectedIdsToString(flts.ExternalTypes);
            string mainquery = MainStateSelectionQuery(slts, exts);
            /*analytical Data*/
            string ordtemp = @"  Declare @DPOrders table(OrderNum int, nType int) 
                                 insert into @DPOrders(OrderNum, nType) 
                                 SELECT ret.OrderNo , 2  FROM( ";
            string selection = "";
            bool newDeliveryMask = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "NewDeliveryMask");
            if (newDeliveryMask)
            {
                selection = @"SELECT TOP 10000000000 o.Id OrderId, o.OrderNo, i.id InvoiceId, it.Abbreviation InvoiceAbbr, i.Counter InvoiceCounter, it.Type InvoiceType,  

            os.SalesTypeId, o.day ReceivedDate, i.isPaid IsPaid, os.PaidStatus PaidStatus, CASE WHEN os.Status IN(5) THEN 1 ELSE 0 END IsVoided, i.Total Total,
             iss.CustomerName CustomerName, iss.CustomerID CustomerID, iss.ShippingAddressId AddressId, iss.ShippingAddress Address, iss.Floor Floor, iss.ShippingCity City,
              iss.ShippingZipCode ZipCode, iss.Phone Phone, iss.Longtitude Longtitude, iss.Latitude Latitude, os.Status OrderStatusId, os.status CurrentStatus,
              os.status DeliveryState, os.TimeStant StatusChangedTS, odi.PriceLists Pricelists, o.StaffId StaffId, ISNULL(o.ExtType, 0 ) ExtType, o.ExtObj ExtObj, o.ExtKey ExtKey, o.DA_IsPaid DA_IsPaid,
              ISNULL(st.LastName, '') +' ' + ISNULL(st.FirstName, '') StaffName, s.Id StatusStaffId, ISNULL(s.LastName, '') +' ' + ISNULL(s.FirstName, '') StatusStaffName, 
              DATEDIFF(minute, os.TimeStant, GETDATE()) StatusTimeDifference, DATEDIFF(minute, o.day, GETDATE()) OrderTotalTime,  o.OrderNotes, o.StoreNotes, o.CustomerNotes, 
              o.CustomerSecretNotes, o.CustomerLastOrderNotes, DATEDIFF(minute, o.day, os.TimeStant) AS NewStoreTime, DATEDIFF(minute, os.TimeStant, GETDATE()) AS NewDistributionTime, 
              LogicErrors, o.DA_Origin, o.DeliveryRoutingId
          FROM[order] o
         INNER JOIN @orders os on os.OrderId = o.Id ";
            }
            else
            {
                selection = @"SELECT TOP 10000000000 o.Id OrderId, o.OrderNo, i.id InvoiceId, it.Abbreviation InvoiceAbbr, i.Counter InvoiceCounter, it.Type InvoiceType,  

            os.SalesTypeId, o.day ReceivedDate, i.isPaid IsPaid, os.PaidStatus PaidStatus, CASE WHEN os.Status IN(5) THEN 1 ELSE 0 END IsVoided, i.Total Total,
             iss.CustomerName CustomerName, iss.CustomerID CustomerID, iss.ShippingAddressId AddressId, iss.ShippingAddress Address, iss.Floor Floor, iss.ShippingCity City,
              iss.ShippingZipCode ZipCode, iss.Phone Phone, iss.Longtitude Longtitude, iss.Latitude Latitude, os.Status OrderStatusId, os.status CurrentStatus,
              os.status DeliveryState, os.TimeStant StatusChangedTS, odi.PriceLists Pricelists, o.StaffId StaffId, ISNULL(o.ExtType, 0 ) ExtType, o.ExtObj ExtObj, o.ExtKey ExtKey, o.DA_IsPaid DA_IsPaid,
              ISNULL(st.LastName, '') +' ' + ISNULL(st.FirstName, '') StaffName, s.Id StatusStaffId, ISNULL(s.LastName, '') +' ' + ISNULL(s.FirstName, '') StatusStaffName
          FROM[order] o
         INNER JOIN @orders os on os.OrderId = o.Id ";
            }
            if (!string.IsNullOrEmpty(slts))
            {
                selection += " AND os.SalesTypeId IN (" + slts + ") \n";
            }

            selection += "CROSS APPLY( \n"
           + "			SELECT DISTINCT odi.InvoicesId \n"
           + "			FROM orderdetailinvoices odi \n"
           + "			INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.OrderId = o.Id \n"
           + "		) odii \n"
           + " INNER JOIN invoices i ON i.Id = odii.InvoicesId AND i.EndOfDayId IS NULL AND isnull(i.isdeleted,0) = 0 \n"
            + "		INNER JOIN @priceListsTable odi ON odi.InvoiceId = i.Id  \n"
            + "		INNER JOIN InvoiceTypes it ON it.id = i.InvoiceTypeId AND it.Type NOT IN (3, 8) \n"
            + "		INNER JOIN InvoiceShippingDetails iss ON iss.InvoicesId = i.id  \n"
            + "		INNER JOIN Staff st ON st.Id = o.StaffId  \n"
            + "		INNER JOIN Staff s ON s.Id = i.StaffId  \n"
            + "";
            string statusWhere = "";
            if (newDeliveryMask)
            {
                statusWhere = "	WHERE ((isnull(ret.PaidStatus, 0) = 0 AND ret.InvoiceType = 2 ) OR (isnull(ret.PaidStatus, 0) IN (1,2) AND ret.InvoiceType != 2 ) OR (isnull(ret.Paidstatus, 0) = 2 and ret.InvoiceType = 2 and IsVoided = 1) OR (isnull(ret.PaidStatus, 0) = 0 AND ret.InvoiceType != 2 ) ) AND ret.SalesTypeId != 22 AND ret.ExtType = 5 AND ret.ExtObj IS NOT NULL AND  \n";
            }
            else
            {
                statusWhere = "	WHERE ((isnull(ret.PaidStatus, 0) = 0 AND ret.InvoiceType = 2 ) OR (isnull(ret.PaidStatus, 0) IN (1,2) AND ret.InvoiceType != 2 ) OR (isnull(ret.Paidstatus, 0) = 2 and ret.InvoiceType = 2 and IsVoided = 1) OR (isnull(ret.PaidStatus, 0) = 0 AND ret.InvoiceType != 2 ) ) AND \n";
            }


            // WHERE fin.AA BETWEEN @StartRow AND @EndRow order by fin.AA asc ";
            switch ((OrderStatusEnum)status)
            {
                case OrderStatusEnum.Received:
                    statusWhere += " ret.CurrentStatus = 0 and ret.IsVoided = 0 "; break;
                case OrderStatusEnum.Preparing:
                    statusWhere += "  (ret.CurrentStatus = 1 or ret.CurrentStatus = 2) and ret.IsVoided = 0 "; break;
                case OrderStatusEnum.Ready:
                    statusWhere += "  ret.CurrentStatus = 3 and ret.IsVoided = 0 "; break;
                case OrderStatusEnum.Onroad:
                    statusWhere += " ret.CurrentStatus = 4 and ret.IsVoided = 0 "; break;
                case OrderStatusEnum.Canceled:
                    statusWhere += " ret.CurrentStatus = 5 "; break;
                case OrderStatusEnum.Complete:
                    statusWhere += " ret.CurrentStatus = 6 and ret.IsVoided = 0 "; break;
                case OrderStatusEnum.Returned:
                    statusWhere += " ret.CurrentStatus = 7 "; break;
                case OrderStatusEnum.PendingReady:
                    statusWhere += " (ret.CurrentStatus = 1 or ret.CurrentStatus = 2 or ret.CurrentStatus = 3) and ret.IsVoided = 0  "; break;
                case OrderStatusEnum.PendingOnroad:
                    statusWhere += " (ret.CurrentStatus = 1 or ret.CurrentStatus = 2 or ret.CurrentStatus = 4) and ret.IsVoided = 0  "; break;
                case OrderStatusEnum.ReadyOnroad:
                    statusWhere += " (ret.CurrentStatus = 3 or ret.CurrentStatus = 4) and ret.IsVoided = 0  "; break;
                case OrderStatusEnum.Open:
                    statusWhere += " (ret.CurrentStatus != 6 and ret.CurrentStatus != 5 and ret.CurrentStatus != 7) and ret.IsVoided = 0 "; break;
                case OrderStatusEnum.All:
                    statusWhere += " 1=1 "; break;
                default:
                    statusWhere += " (ret.CurrentStatus != 6 and ret.CurrentStatus != 5 and ret.CurrentStatus != 7) and ret.IsVoided = 0 "; break;
            }
            if (!String.IsNullOrEmpty(flts.Address))
                statusWhere += " and ret.Address like '%" + flts.Address + "%'";
            if (!String.IsNullOrEmpty(flts.CustomerName))
                statusWhere += " and ret.CustomerName like '%" + flts.CustomerName + "%'";
            if (flts.OrderId != null)
                statusWhere += " and ret.InvoiceCounter =" + flts.OrderId;
            if (flts.OrderNo != null)
                statusWhere += " and ret.OrderNo =" + flts.OrderNo;
            if (flts.ExtType != null)
                statusWhere += " and ret.ExtType =" + flts.ExtType;
            if (flts.ExternalTypes != null && flts.ExternalTypes.Count() > 0)
            {
                statusWhere += " and ret.ExtType in ( " + exts + " ) ";
            }


            string q1 = ordtemp + selection + "	) ret   \n" + statusWhere + " group by ret.OrderNo having count(ret.OrderNo) > 1 ";
            string q2 = "SELECT ROW_NUMBER() OVER(ORDER BY ret.ReceivedDate ASC) AS AA,* FROM ( \n" + selection + " ) ret  left outer join @DPOrders ord on ord.OrderNum = ret.OrderNo and ord.nType = ret.InvoiceType \n" + statusWhere + " and ord.OrderNum is null ";
            string sWhere = "WHERE fin.AA BETWEEN @StartRow AND @EndRow order by fin.AA asc ";
            string sqlData = mainquery + q1 + @"SELECT * FROM(" + q2 + ") fin ";
            string sqlCount = @"SELECT Count(*) FROM(" + q2 + ") fin";

            connectionString = usersToDatabases.ConfigureConnectionString(store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                PaginationModel<DeliveryStatusOrders> res = new PaginationModel<DeliveryStatusOrders>();
                try
                {
                    res = genDeliveryStatusOrders.GetPaginationQueryResult(db, sqlData, sWhere, sqlCount, pageNumber, pageLength);
                    foreach(DeliveryStatusOrders order in res.PageList)
                    {
                        string sqlSelectDeliveryTime=" select delivery_time from hitposorders where orderno=" + order.ExtKey;
                        if (order.ExtKey != null)
                            order.DeliveryTime = db.Query<DateTime>(sqlSelectDeliveryTime).FirstOrDefault();
                        else
                            order.DeliveryTime = order.ReceivedDate;
                        DateTime ReceivedDatePlus = Convert.ToDateTime(order.ReceivedDate);
                        long GoodysTakeoutDelayTime = (MainConfigurationHelper.GetSubConfiguration("testpos1", "GoodysTakeoutDelayTime"));
                        long GoodysDeliveryDelayTime = (MainConfigurationHelper.GetSubConfiguration("testpos1", "GoodysDeliveryDelayTime"));

                        if (order.ExtType == 1 && GoodysTakeoutDelayTime != 0)
                            ReceivedDatePlus= ReceivedDatePlus.AddMinutes(GoodysTakeoutDelayTime);
                        if (order.ExtType == 2 && GoodysDeliveryDelayTime != 0)
                            ReceivedDatePlus= ReceivedDatePlus.AddMinutes(GoodysDeliveryDelayTime);

                        if (order.DeliveryTime > ReceivedDatePlus && ReceivedDatePlus != null)
                            order.IsDelay = true;
                        else
                            order.IsDelay = false;
                       
                    }
                    return res;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DELIVERYORDERSPAGGEDBYSTATE + " " + status.ToString() + " " + pageNumber.ToString() + " " + pageLength.ToString());
                }
            }
        }


        /// <summary>
        /// Provided a model of deliverycustomer Address 
        /// updates InvoiceShipping Details of invoice Id provided and 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public DeliveryCustomersShippingAddressModel UpdateShippingCoordinates(DBInfoModel store, long InvoiceId, DeliveryCustomersShippingAddressModel model)
        {
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            string sql = "UPDATE InvoiceShippingDetails set Longtitude = @Long, Latitude = @Lat, ShippingAddress = @SAddress, ShippingCity = @SCity, ShippingZipCode = @SZip   where InvoicesId = @Invid ";
            string sqldcsa = "UPDATE Delivery_CustomersShippingAddress set Longtitude = @Long , Latitude = @Lat , AddressStreet = @AddStr, AddressNo = @AddNo   , City = @AddCity  , Zipcode = @AddZip where ID = @IDm ";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        /// <param name="sql">Insert or Update query.  ex: "UPDATE Author SET FirstName = @FirstName, LastName = @LastName " + "WHERE Id = @Id";</param>
                        /// <param name="parameters">parameters (optional). Parameters must match query parameters. ex: new {FirstName="Smith",Lastname="Tom", Id=30 } </param>
                        int res = genDeliveryStatusOrders.Execute(db, sql + sqldcsa, new
                        {
                            Long = model.Longtitude,
                            Lat = model.Latitude,
                            //shipping details params
                            SAddress = model.AddressStreet + " " + model.AddressNo,
                            SCity = model.City,
                            SZip = model.Zipcode,
                            Invid = InvoiceId,
                            //DeliveryCustomer Address params
                            AddStr = model.AddressStreet,
                            AddNo = model.AddressNo,
                            AddCity = model.City,
                            AddZip = model.Zipcode,
                            IDm = model.ID
                        });
                        scope.Complete();
                        if (res >= 1)
                        {
                            return model;
                        }
                        else
                        {
                            throw new Exception("UpdateFailed");
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        throw new Exception(Symposium.Resources.Errors.DELIVERYORDERSUPDATESHIPPINGCOORDS);
                    }
                }
            }
        }


        /// <summary>
        /// Update the Status of OrderStatus 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="OrderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateOrderStatusForReturned(DBInfoModel store, long OrderId, int status)
        {
            int res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(store);
            string sql = "UPDATE OrderStatus SET [Status] =@orderStatus WHERE OrderId =@orderId ";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    db.Query<int>(sql, new { orderId = OrderId, orderStatus = status }).FirstOrDefault();
                    res = 1;
                    return res;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DELIVERYORDERSUPDATESHIPPINGCOORDS);
                }
            }
        }

        /// <summary>
        /// Update Order's Status DeliveryBoyId 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="StaffId"></param>
        /// <param name="CurrentOrderStatus"></param>
        /// <returns></returns>
        public long UpdateStaffStatus(DBInfoModel Store, long StaffId, bool IsOnRoad, IDbConnection db = null, IDbTransaction transact = null)
        {
            long res = 0;
            string sql = "UPDATE Staff SET IsOnRoad =@status, StatusTimeChanged = GETDATE() WHERE Id =@staffId";

            if (db == null)
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);
                db = new SqlConnection(connectionString);
            }

            //using (db)
            //{
                try
                {
                    db.Query<long>(sql, new { status = IsOnRoad, staffId = StaffId }, transact).FirstOrDefault();
                    res = 1;
                    return res;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception("Error to Update Staff Order Status");
                }
            //}
        }

        /// <summary>
        /// Update staff IsOnRoad
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <param name="StaffId">long</param>
        /// <param name="IsOnRoad">bool</param>
        /// <param name="db">IDbConnection</param>
        /// <param name="transact">IDbTransaction</param>
        /// <returns>long</returns>
        public long UpdateStaffStatusTran(DBInfoModel Store, long StaffId, bool IsOnRoad, IDbConnection db = null, IDbTransaction transact = null)
        {
            if (db == null)
            {
                connectionString = usersToDatabases.ConfigureConnectionString(Store);

                db = new SqlConnection(connectionString);
            }

            long res = 0;

            string sql = "UPDATE Staff SET IsOnRoad =@status, StatusTimeChanged = GETDATE() WHERE Id =@staffId";

            try
            {
                res = db.Query<long>(sql, new { status = IsOnRoad, staffId = StaffId }, transact).FirstOrDefault();

                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());

                throw new Exception("Error to Update Staff IsOnRoad");
            }
        }

        /// <summary>
        /// get staff that is not deleted, has clocked in and not clocked out and is not assigned delivery route
        /// </summary>
        /// <param name="Store">DBInfoModel</param>
        /// <returns>List<StaffDeliveryModel></returns>
        public List<StaffDeliveryModel> GetAvailableDeliveryStaffs(DBInfoModel Store)
        {
            List<StaffDeliveryModel> res = new List<StaffDeliveryModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(Store);

            string sql = @"SELECT 
                                s.Id AS StaffId, 
                                s.Code, 
                                s.FirstName, 
                                s.LastName, 
                                s.ImageUri, 
                                s.[Password], 
                                s.IsOnRoad, 
                                s.StatusTimeChanged, 
                                ap.StaffPositionId,
                                s.AllowReporting,
                                s.isAssignedToRoute
                            FROM Staff AS s
                            INNER JOIN AssignedPositions AS ap ON ap.StaffId = s.Id
							INNER JOIN PayrollNew p ON p.StaffId = s.Id
                            WHERE 
                                ap.StaffPositionId = 5 AND 
                                --( s.IsDeleted IS NULL OR s.IsDeleted = 0) AND 
                                ISNULL(s.IsDeleted, 0)<> 1  AND
                                ISNULL(p.IsDeleted, 0)<> 1  AND
                                (s.isAssignedToRoute IS NULL OR s.isAssignedToRoute = 0)  AND
								p.DateFrom IS NOT NULL AND 
								p.DateTo IS NULL 
                            ORDER BY 
                                (CASE WHEN s.StatusTimeChanged IS NULL THEN 1 ELSE 0 END) DESC, 
                                s.StatusTimeChanged ASC";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = db.Query<StaffDeliveryModel>(sql).ToList();

                    return res;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());

                    throw new Exception("Error retrieving available staff");
                }
            }
        }

        public List<StaffDeliveryModel> GetDeliveryStaffsOnly(DBInfoModel Store)
        {
            List<StaffDeliveryModel> res = new List<StaffDeliveryModel>();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string sql = @"SELECT s.Id AS StaffId, s.Code, s.FirstName, s.LastName, s.ImageUri, s.[Password], s.IsOnRoad, s.StatusTimeChanged, ap.StaffPositionId
                            FROM Staff AS s
                            INNER JOIN AssignedPositions AS ap ON ap.StaffId = s.Id
                            WHERE ap.StaffPositionId = 5 AND s.IsDeleted IS NULL
                            ORDER BY (CASE WHEN s.StatusTimeChanged IS NULL THEN 1 ELSE 0 END) DESC, s.StatusTimeChanged ASC";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = db.Query<StaffDeliveryModel>(sql).ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception("Error to Load Delivery Staff");
                }
            }
        }

        public bool GetDeliveryStaffsSignInOnly(DBInfoModel Store, long PosInfoId, long StaffId)
        {
            bool res = false;
            PayrollNewModel tmpModel = new PayrollNewModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            string sql = @"SELECT TOP 1 * FROM PayrollNew AS p WHERE p.PosInfoId =@posInfoId AND p.StaffId =@staffId ORDER BY p.Id DESC";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    tmpModel = db.Query<PayrollNewModel>(sql, new { posInfoId = PosInfoId, staffId = StaffId }).FirstOrDefault();
                    if (tmpModel != null)
                    {
                        if (tmpModel.DateFrom != null && tmpModel.DateTo == null)
                        {
                            res = true;
                        }
                        else
                        {
                            res = false;
                        }
                    }
                    else
                    {
                        res = false;
                    }
                    return res;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception("Error to Load Payroll Staff");
                }
            }
        }

    }
}

