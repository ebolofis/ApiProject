using Dapper;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class TableForDisplayDT : ITableForDisplayDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public TableForDisplayDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        /// <summary>
        /// Return TableForDisplay Model
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public TableForDisplayPreviewModel getSigleTable(DBInfoModel Store, string storeid, long tableId)
        {
            TableForDisplayPreviewModel model = new TableForDisplayPreviewModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string itemsquery = "SELECT odi.InvoicesId AS ReceiptId, odi.RegionId, odi.OrderDetailId AS Id, odi.[Counter] AS ReceiptNo, \n"
                           + " odi.PosInfoId, odi.TableId, odi.TableCode, ISNULL(od.OrderId, 0) AS OrderId, odi.OrderNo, 0 AS Cover, odi.ItemCode, \n"
                           + " odi.[Description] AS Product, odi.ProductId, odi.Price, odi.VatId, odi.VatCode, odi.VatRate AS VatDesc, od.PriceListDetailId, \n"
                           + " odi.PricelistId, odi.Qty, od.KitchenId, od.Guid, od.[Status], od.StatusTS, od.PaidStatus, odi.Total AS TotalAfterDiscount, odi.StaffId AS Staff, \n"
                           + " odi.Discount, odi.SalesTypeId, odi.OrderDetailIgredientsId, odi.ProductCategoryId, odi.ReceiptSplitedDiscount, odi.IsExtra, odi.ItemRemark \n"
                           + "FROM OrderDetailInvoices AS odi  \n"
                           + "INNER JOIN OrderDetail AS od ON odi.OrderDetailId = od.Id \n"
                           + "LEFT OUTER JOIN Product AS p ON odi.ProductId = p.Id \n"
                           + "LEFT OUTER JOIN Kitchen AS k ON od.KitchenId = k.Id \n"
                           + "LEFT OUTER JOIN SalesType AS st ON odi.SalesTypeId = st.Id \n"
                           + "WHERE odi.EndOfDayId IS NULL  \n"
                           + "AND od.[Status] != 5  \n"
                           + "AND od.TableId =@tId  \n"
                           + "AND od.PaidStatus != 2  \n"
                           + "AND ((odi.InvoiceType = 2 AND od.PaidStatus = 0) OR (odi.InvoiceType != 2 AND od.PaidStatus < 2 ))";
                List<TempReceiptDetailsModel> ItemsList = db.Query<TempReceiptDetailsModel>(itemsquery, new { tId = tableId }).ToList();
                model.Items = ItemsList;

                string invoiceQuery = "SELECT odi.InvoicesId  \n"
                           + "FROM OrderDetailInvoices AS odi  \n"
                           + "INNER JOIN OrderDetail AS od ON odi.OrderDetailId = od.Id \n"
                           + "WHERE odi.EndOfDayId IS NULL  \n"
                           + "AND od.[Status] != 5  \n"
                           + "AND od.TableId =@tId  \n"
                           + "AND od.PaidStatus != 2  \n"
                           + "AND (odi.InvoiceType IN (2,0) OR (odi.InvoiceType != 2 AND odi.InvoiceType < 2 ))";
                long invId = db.Query<long>(invoiceQuery, new { tId = tableId }).FirstOrDefault();

                string pay = "SELECT g.Id AS GuestId, g.Room, g.RoomId, g.ProfileNo, g.ReservationCode, g.FirstName, g.LastName, \n"
                           + " igt.InvoiceId AS InvoicesId, igt.TransactionId, g.HotelId, g.ClassId, g.ClassName, g.AvailablePoints, g.fnbdiscount, g.ratebuy  \n"
                           + "FROM Invoice_Guests_Trans AS igt INNER JOIN Guest AS g \n"
                           + "ON igt.GuestId = g.Id \n"
                           + "WHERE igt.InvoiceId =@invoiceId";
                TempGuestPaymentsModel payments = db.Query<TempGuestPaymentsModel>(pay, new { invoiceId = invId }).FirstOrDefault();

                string orderIdQuery = "SELECT od.OrderId  \n"
                           + "FROM OrderDetailInvoices AS odi  \n"
                           + "INNER JOIN OrderDetail AS od ON odi.OrderDetailId = od.Id \n"
                           + "WHERE odi.EndOfDayId IS NULL  \n"
                           + "AND od.[Status] != 5  \n"
                           + "AND od.TableId =@tId  \n"
                           + "AND od.PaidStatus != 2  \n"
                           + "AND (odi.InvoiceType IN (2,0) OR (odi.InvoiceType != 2 AND odi.InvoiceType < 2 ))";
                long oId = db.Query<long>(orderIdQuery, new { tId = tableId }).FirstOrDefault();
                string cov = "SELECT o.Id AS OrderId, o.OrderNo, o.Couver AS Cover FROM [Order] AS o WHERE o.Id =@ID";
                model.Payments = payments;
                TableCoversModel cover = db.Query<TableCoversModel>(cov, new { ID = oId }).FirstOrDefault();
                model.Covers = cover;
            }
            return model;
        }

        /// <summary>
        /// Return Kitchen Instructions for a single Table
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public KitchenInstructionPreviewModel GetKitchenInstructionsForTable(DBInfoModel Store, string storeid, long tableId)
        {
            // get the results
            KitchenInstructionPreviewModel kitcheninstructionsModel = new KitchenInstructionPreviewModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string kitchenQuery = "SELECT kil.Id, kil.[Description] AS MESSAGE, s.Code AS StaffCode, s.FirstName AS Staff, kil.SendTS,  \n"
                                    + "kil.[Status], CASE WHEN ISNULL(kil.PdaModulId,0) = 0 THEN 0 ELSE 1 END AS Origin \n"
                                    + "FROM KitchenInstructionLogger AS kil \n"
                                    + "INNER JOIN Staff AS s ON kil.StaffId = s.Id \n"
                                    + "WHERE kil.EndOfDayId IS NULL AND kil.TableId =@tId";

                List<KitchenInstructionModel> KitchenInstructions = db.Query<KitchenInstructionModel>(kitchenQuery, new { tId = tableId }).ToList();
                kitcheninstructionsModel.KitchenInstrustions = KitchenInstructions;
            }

            return kitcheninstructionsModel;
        }

        /// <summary>
        /// Return Tables Per Region
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public TablesPerRegionPreviewModel GetOpenTablesPerRegionStatusOnly(DBInfoModel Store, string storeid, long regionId)
        {
            // get the results
            TablesPerRegionPreviewModel TablesPerRegionmodel = new TablesPerRegionPreviewModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                
           string tablesperregionQuery = "DECLARE @TableId INT, @PosInfoDesc INT, @Staff VARCHAR(150),@StaffIds INT, @ColorStatusId INT, @idx INT, @tRec INT \n"
           + " \n"
           + "DECLARE @tmpRes TABLE (ID INT IDENTITY (1,1), TableId INT, PosInfoDesc INT, Staff VARCHAR(150), \n"
           + "	StaffIds INT, ColorStatusId INT) \n"
           + "DECLARE @Res TABLE (ID INT IDENTITY (1,1), TableId INT, PosInfoDesc INT, Staff VARCHAR(150), \n"
           + "	StaffIds INT, ColorStatusId INT) \n"
           + "	 \n"
           + "	 \n"
           + "INSERT INTO @tmpRes (TableId, PosInfoDesc, Staff, StaffIds, ColorStatusId)	 \n"
           + "SELECT distinct odi.TableId, odi.PosInfoId AS PosInfoDesc, s.FirstName Staff, s.Id AS StaffIds, \n"
           + "CASE WHEN od.PaidStatus = 0 THEN 1 \n"
           + "	WHEN od.PaidStatus = 1 THEN 2 \n"
           + "	ELSE NULL END ColorStatusId \n"
           + "FROM OrderDetailInvoices AS odi \n"
           + "INNER JOIN Invoices AS i ON i.id = odi.InvoicesId  \n"
           + "INNER JOIN InvoiceTypes AS it ON it.Id = i. InvoiceTypeId \n"
           + "INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId AND od.[Status] != 5 AND \n"
           + "((od.PaidStatus = 0 AND it.[Type] = 2) OR (od.PaidStatus = 1 AND it.[Type] != 2)) \n"
           + "INNER JOIN Staff AS s ON s.Id = odi.StaffId \n"
           + "WHERE odi.EndOfDayId IS NULL AND odi.RegionId = @rId AND ISNULL(odi.IsDeleted,0) = 0 \n"
           + "ORDER BY s.Id \n"
           + " \n"
           + "SELECT @idx = MIN(ID), @tRec=MAX(ID) FROM @tmpRes \n"
           + "WHILE @idx <= @tRec \n"
           + "BEGIN \n"
           + "	SELECT @TableId = TableId, @PosInfoDesc = PosInfoDesc, @Staff = Staff, @StaffIds = StaffIds, @ColorStatusId = ColorStatusId \n"
           + "	FROM @tmpRes \n"
           + "	WHERE id = @idx \n"
           + "	 \n"
           + "	IF (SELECT COUNT(*) FROM @tmpRes WHERE TableId = @TableId) <> 1 \n"
           + "	BEGIN \n"
           + "		IF NOT EXISTS(SELECT 1 FROM @Res WHERE TableId = @TableId) \n"
           + "			INSERT INTO @Res(TableId, PosInfoDesc, Staff, StaffIds, ColorStatusId) \n"
           + "			SELECT @TableId, @PosInfoDesc, @Staff, @StaffIds, 3 \n"
           + "	END \n"
           + "	ELSE \n"
           + "	IF NOT EXISTS(SELECT 1 FROM @Res WHERE TableId = @TableId) \n"
           + "			INSERT INTO @Res(TableId, PosInfoDesc, Staff, StaffIds, ColorStatusId) \n"
           + "			SELECT @TableId, @PosInfoDesc, @Staff, @StaffIds, @ColorStatusId \n"
           + "	 \n"
           + "	SET @idx = @idx + 1 \n"
           + "END \n"
           + " \n"
           + "SELECT * FROM @Res";

                List<TablesPerRegionModel> tablePerRegionPreview = db.Query<TablesPerRegionModel>(tablesperregionQuery, new { rId = regionId }).ToList();
                TablesPerRegionmodel.TablesPerRegion = tablePerRegionPreview;
            }

            return TablesPerRegionmodel;
        }

        /// <summary>
        /// Return all tables for a specific pos
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        public GetAllTablesModelPreview GetAllTables(DBInfoModel Store, string storeid, long posInfoId)
        {
            GetAllTablesModelPreview getAllTablesModel = new GetAllTablesModelPreview();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string getAllQuery = "SELECT t.Id, t.Angle, t.Code, t.[Description] ,t.Height , t.ImageUri, t.IsOnline, t.MaxCapacity,t.MinCapacity, \n"
                       + "	t.RegionId, t.ReservationStatus, t.SalesDescription, t.Shape, t.[Status], t.TurnoverTime, t.Width, t.XPos, t.YPos \n"
                       + "FROM [Table] AS t  \n"
                       + "LEFT OUTER JOIN PosInfo_Region_Assoc AS pira ON t.RegionId = pira.RegionId \n"
                       + "WHERE pira.PosInfoId =@pId  \n"
                       + "AND (t.IsDeleted IS NULL OR t.IsDeleted = 0)  \n"
                       + "ORDER BY t.RegionId, t.Code";
                List<GetAllTablesModel> getAllTablesPreview = db.Query<GetAllTablesModel>(getAllQuery, new { pId = posInfoId }).ToList();
                getAllTablesModel.GetAllTable = getAllTablesPreview;
            }

            return getAllTablesModel;
        }

    }
}
