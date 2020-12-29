using Dapper;
using log4net;
using Newtonsoft.Json;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Symposium.WebApi.DataAccess.DT
{
    public class ReceiptDetailsForExtecrDT : IReceiptDetailsForExtecrDT
    {
        string connectionString;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IUsersToDatabasesXML usersToDatabases;

        public ReceiptDetailsForExtecrDT(IUsersToDatabasesXML usersToDatabases)
        {
            this.usersToDatabases = usersToDatabases;
        }

        public List<VatComboList> VatList(DBInfoModel Store)
        {
            List<VatComboList> model = new List<VatComboList>();
            string vatlist = "select Code as Id,Description as Descr  from vat";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                model = db.Query<VatComboList>(vatlist).ToList();
            }
            return model;
        }
        public List<VatPercentageModel> VatPercentage(DBInfoModel Store)
        {
            List<VatPercentageModel> modelist = new List<VatPercentageModel>();
            string vatlist = "select Code ,Percentage  from vat";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                modelist = db.Query<VatPercentageModel>(vatlist).ToList();
            }
            return modelist;
        }

        /// <summary>
        /// Get Receipt Details For Extecr
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>
        public ExtecrReceiptModel GetReceiptDetailsForExtecr(DBInfoModel Store, Int64 invoiceId, bool groupReceiptItems, bool isForKitchen)
        {
            ExtecrReceiptModel receiptModel = new ExtecrReceiptModel();
            connectionString = usersToDatabases.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string receiptQuery = @"SELECT i.TableSum AS TableTotal, i.[Description] AS ReceiptTypeDescription, i.Cover AS Couver, pid.InvoiceId AS InvoiceIndex,
	                                        CASE WHEN CHARINDEX('-',LTRIM(RTRIM(i.OrderNo))) > 0 THEN  -1 ELSE o.OrderId END AS OrderId, t.Code AS TableNo, t.RegionId AS RegionId, r.Description AS RegionDescription, i.Rooms AS RoomNo, (s.FirstName +' '+ s.LastName) AS Waiter, s.Code AS WaiterNo,
	                                        i.PosInfoId AS Pos, pi1.[Description] AS PosDescr, i.PaymentsDesc AS PaymentType, pi1.DepartmentId AS Department, CASE WHEN isd.CustomerName IS NOT NULL THEN isd.CustomerName ELSE 'Πελάτης Λιανικής' END CustomerName , isd.BillingAddress AS CustomerAddress,
	                                        isd.ShippingAddress AS CustomerDeliveryAddress, isd.Phone AS CustomerPhone, isd.[Floor], isd.BillingCity AS City,
	                                        isd.CustomerRemarks AS CustomerComments, isd.BillingName, isd.BillingJob, isd.BillingVatNo, isd.BillingDOY, isd.BillingCity,
	                                        isd.BillingZipCode, it.[Type] AS InvoiceType, i.Vat AS TotalVat, i.[Counter] AS ReceiptNo, i.OrderNo, i.Net AS TotalNet,
	                                        i.Total, i.CashAmount, i.BuzzerNumber, i.PdaModuleId AS PdaId, i.IsVoided AS IsVoid, i.ExtECRCode, isd.Longtitude, isd.Latitude, isd.Phone 
                                        FROM Invoices AS i 
                                        INNER JOIN PosInfo AS pi1 ON i.PosInfoId = pi1.Id
                                        INNER JOIN Staff AS s ON i.StaffId = s.Id
                                        INNER JOIN PosInfoDetail AS pid ON i.PosInfoDetailId = pid.Id
                                        LEFT OUTER JOIN [Table] AS t ON i.TableId = t.Id
                                        LEFT OUTER JOIN Region AS r ON t.RegionId = r.Id
                                        LEFT OUTER JOIN InvoiceShippingDetails AS isd ON i.Id = isd.InvoicesId
                                        LEFT OUTER JOIN InvoiceTypes AS it ON i.InvoiceTypeId = it.Id
                                        OUTER APPLY(
	                                        SELECT TOP 1 od.OrderId
	                                        FROM OrderDetailInvoices AS odi
	                                        INNER JOIN OrderDetail AS od ON od.Id = odi.OrderDetailId
	                                        WHERE odi.InvoicesId = i.Id	
                                        )	o
                WHERE ISNULL(i.IsDeleted,0)=0 AND i.Id =@InvoiceID";

                receiptModel = db.Query<ExtecrReceiptModel>(receiptQuery, new { InvoiceID = invoiceId }).FirstOrDefault();

                int newOrderNo = 0;
                int OrderId = -1;
                bool canConvert = false;
                if (int.Parse(receiptModel.OrderId) > 0)
                {
                    canConvert = true;
                    OrderId = int.Parse(receiptModel.OrderId);
                }
                else
                    canConvert = Int32.TryParse(receiptModel.OrderNo.Trim(), out newOrderNo);

                if (canConvert == true)
                {
                    string ExtObjStringified = "";
                    string sql = "SELECT TOP 1 * FROM [Order] o WHERE CASE WHEN @tmpId > 0 AND o.Id = @tmpId THEN 1 ELSE 0 END = 1 OR \n"
                                        + "  CASE WHEN @tmpOrdeNo > 0 AND o.OrderNo = @tmpOrdeNo THEN 1 ELSE 0 END = 1 ";

                    OrderModel tmpOrder = db.Query<OrderModel>(sql, new { tmpId = OrderId, tmpOrdeNo = newOrderNo }).FirstOrDefault();
                    if (tmpOrder != null)
                    {
                        receiptModel.DA_IsPaid = tmpOrder.DA_IsPaid;
                        receiptModel.ItemsChanged = tmpOrder.ItemsChanged;
                        receiptModel.EstTakeoutDate = tmpOrder.EstTakeoutDate;
                        receiptModel.IsDelay = tmpOrder.IsDelay;
                        receiptModel.OrderNotes = tmpOrder.OrderNotes;
                        receiptModel.CustomerSecretNotes = tmpOrder.CustomerSecretNotes;
                        receiptModel.CustomerLastOrderNotes = tmpOrder.CustomerLastOrderNotes;
                        receiptModel.LogicErrors = tmpOrder.LogicErrors;
                        receiptModel.LoyaltyCode = tmpOrder.LoyaltyCode;
                        receiptModel.TelephoneNumber = tmpOrder.TelephoneNumber;
                        receiptModel.ExtType = tmpOrder.ExtType;
                        receiptModel.CouverAdults = tmpOrder.CouverAdults;
                        receiptModel.CouverChildren = tmpOrder.CouverChildren;
                        if (tmpOrder.DA_Origin != null)
                        {
                            receiptModel.DA_Origin = (OrderOriginEnum)tmpOrder.DA_Origin;
                        }
                        ExtObjStringified = tmpOrder.ExtObj;
                    }

                    if (!string.IsNullOrEmpty(ExtObjStringified))
                    {
                        try
                        {
                            ExternalDA_ObjectModel ExtObjObjectified = JsonConvert.DeserializeObject<ExternalDA_ObjectModel>(ExtObjStringified);
                            receiptModel.PointsGain = ExtObjObjectified.PointsGain;
                            receiptModel.PointsRedeem = ExtObjObjectified.PointsRedeem;
                            receiptModel.CouponCodes = "";
                            if (ExtObjObjectified.DA_ExternalModel != null && ExtObjObjectified.DA_ExternalModel.Coupons != null)
                            {
                                string couponCode = "";
                                int index = 0;
                                foreach (ExternalCouponsModel couponModel in ExtObjObjectified.DA_ExternalModel.Coupons)
                                {
                                    couponCode += couponModel.Code;
                                    index++;
                                    if (index < ExtObjObjectified.DA_ExternalModel.Coupons.Count)
                                    {
                                        couponCode += ", ";
                                    }
                                }
                                receiptModel.CouponCodes = couponCode;
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Warn("Error deserializing external object. Possibly different format. " + e.ToString());
                        }
                    }
                }

                receiptModel.CustomerAfm = "";
                receiptModel.CustomerDoy = "";
                receiptModel.CustomerJob = "";

                string paymentQuery = @"SELECT t.Id AS TransactionId , a.[Description] AS [DESCRIPTION], a.[Type] AS AccountType, t.Amount AS Amount
                                        FROM Transactions AS t
                                        INNER JOIN Invoices AS i ON t.InvoicesId = i.Id
                                        INNER JOIN InvoiceTypes AS it ON i.InvoiceTypeId = it.Id
                                        INNER JOIN Accounts AS a ON t.AccountId = a.Id
                                        WHERE t.InvoicesId IS NOT NULL AND ISNULL(t.IsDeleted,0)=0 AND t.InvoicesId =@InvoiceID";

                List<Extecr_PaymentTypeModel> paymentModel = db.Query<Extecr_PaymentTypeModel>(paymentQuery, new { InvoiceID = invoiceId }).ToList();
                if (paymentModel.Count > 0)
                {
                    receiptModel.PaymentsList = paymentModel;

                    string guestQuery = @"SELECT g.FirstName AS FirstName, g.LastName AS LastName, g.Room AS Room
                                      FROM Invoice_Guests_Trans AS igt 
                                      LEFT OUTER JOIN Guest AS g ON g.Id = igt.GuestId
                                      WHERE igt.TransactionId =@transactionId";

                    foreach (Extecr_PaymentTypeModel pay in paymentModel)
                    {
                        EXTECR_Guest guestModel = new EXTECR_Guest();
                        if (pay.AccountType == 3 || pay.AccountType == 2 || pay.AccountType == 9)
                        {
                            guestModel = db.Query<EXTECR_Guest>(guestQuery, new { transactionId = pay.TransactionId }).FirstOrDefault();
                        }
                        else
                        {
                            guestModel.FirstName = "";
                            guestModel.LastName = "";
                            guestModel.Room = "";
                        }
                        pay.Guest = guestModel;
                    }
                }
                else
                {
                    string checkGuestQuery = @"SELECT g.Id 
                                               FROM Invoice_Guests_Trans AS igt 
                                               LEFT OUTER JOIN Guest AS g ON g.Id = igt.GuestId
                                               WHERE igt.InvoiceId = @InvoiceID";

                    List<long> checkGuest = db.Query<long>(checkGuestQuery, new { InvoiceID = invoiceId }).ToList();
                    if (checkGuest.Count > 0)
                    {

                        string paymentQuery1 = @"SELECT i.PaymentsDesc AS [DESCRIPTION], a.[Type] AS AccountType, i.Total AS Amount
                                             FROM Invoices AS i
                                             INNER JOIN PosInfo AS pi1 ON i.PosInfoId = pi1.Id
                                             INNER JOIN Accounts AS a ON pi1.AccountId = a.Id
                                             WHERE i.Id IS NOT NULL AND ISNULL(i.IsDeleted,0)=0 AND i.Id =@InvoiceID";
                        List<Extecr_PaymentTypeModel> paymentModel1 = db.Query<Extecr_PaymentTypeModel>(paymentQuery1, new { InvoiceID = invoiceId }).ToList();
                        receiptModel.PaymentsList = paymentModel1;

                        string guestQuery1 = @"SELECT g.FirstName AS FirstName, g.LastName AS LastName, g.Room AS Room
                                           FROM Invoice_Guests_Trans AS igt 
                                           LEFT OUTER JOIN Guest AS g ON g.Id = igt.GuestId
                                           WHERE igt.InvoiceId = @InvoiceID";
                        if (checkGuest.Count > 1)
                        {
                            string GuestRoomQuery = @"SELECT g.Room 
                                               FROM Invoice_Guests_Trans AS igt 
                                               LEFT OUTER JOIN Guest AS g ON g.Id = igt.GuestId
                                               WHERE igt.InvoiceId = @InvoiceID";

                            List<string> GuestRoom = db.Query<string>(GuestRoomQuery, new { InvoiceID = invoiceId }).ToList();

                            foreach (Extecr_PaymentTypeModel pay1 in paymentModel1)
                            {
                                EXTECR_Guest guestModel1 = new EXTECR_Guest();

                                guestModel1 = db.Query<EXTECR_Guest>(guestQuery1, new { InvoiceID = invoiceId }).FirstOrDefault();
                                string room = string.Join(",", GuestRoom);
                                guestModel1.Room = room;
                                pay1.Guest = guestModel1;
                            }
                        }
                        else
                        {
                            foreach (Extecr_PaymentTypeModel pay1 in paymentModel1)
                            {
                                EXTECR_Guest guestModel1 = new EXTECR_Guest();
                                guestModel1 = db.Query<EXTECR_Guest>(guestQuery1, new { InvoiceID = invoiceId }).FirstOrDefault();
                                pay1.Guest = guestModel1;
                            }
                        }
                    }
                    else
                    {
                        receiptModel.PaymentsList = paymentModel;
                    }
                }

                //Payment To
                //List<string> PaymentToList = new List<string>();
                //foreach (var payment in receiptModel.PaymentsList)
                //{
                //    PaymentToList.Add(payment.Description);
                //}
                //string PaymentToString = string.Join(",", PaymentToList);

                string departmentIdQuery = @"SELECT pi1.DepartmentId FROM Invoices AS i
                                             INNER JOIN PosInfo AS pi1 ON i.PosInfoId = pi1.Id
                                             WHERE i.Id =@InvoiceID";

                long DepartmentId = db.Query<long>(departmentIdQuery, new { InvoiceID = invoiceId }).FirstOrDefault();

                string departmentDescQuery = "SELECT d.[Description] FROM Department AS d WHERE d.Id =@departmentId";

                string DepartmentDesc = db.Query<string>(departmentDescQuery, new { departmentId = DepartmentId }).FirstOrDefault();
                receiptModel.DepartmentTypeDescription = DepartmentDesc;

                string salesTypeDescQuery = @"SELECT st.[Description]
                                                FROM OrderDetailInvoices AS odi
                                                INNER JOIN OrderDetail AS od ON odi.OrderDetailId = od.Id
                                                LEFT OUTER JOIN Product AS p ON odi.ProductId = p.Id
                                                LEFT OUTER JOIN Kitchen AS k ON od.KitchenId = k.Id
                                                LEFT OUTER JOIN SalesType AS st ON odi.SalesTypeId = st.Id
                                                WHERE ISNULL(odi.IsDeleted,0)=0 AND odi.InvoicesId =@InvoiceID";

                string salesTypeDesc = db.Query<string>(salesTypeDescQuery, new { InvoiceID = invoiceId }).FirstOrDefault();
                receiptModel.SalesTypeDescription = salesTypeDesc;

                string totalDiscountQuery = @"SELECT (i.Discount - SUM(odi.Discount)) AS TotalDiscount
                                             FROM Invoices AS i
                                             INNER JOIN OrderDetailInvoices AS odi ON odi.InvoicesId = i.Id
                                             WHERE i.Id =@InvoiceID
                                             GROUP BY i.Discount";

                decimal? totalDiscount = db.Query<decimal?>(totalDiscountQuery, new { InvoiceID = invoiceId }).FirstOrDefault();
                receiptModel.TotalDiscount = totalDiscount;

                if (receiptModel.CashAmount != null)
                {
                    if (String.IsNullOrEmpty(receiptModel.CashAmount))
                    {
                        receiptModel.CashAmount = "0";
                    }
                    double change = Double.Parse(receiptModel.CashAmount, CultureInfo.GetCultureInfo("en-US")) - (double)receiptModel.Total;
                    receiptModel.Change = String.Format("{0:0.00}", change, CultureInfo.CurrentUICulture);
                }

                string GuestTermQuery = @"SELECT g.BoardCode AS GuestTerm
                                            FROM Transactions AS t 
                                            INNER JOIN Invoices AS i ON t.InvoicesId = i.Id
                                            INNER JOIN InvoiceTypes AS it ON i.InvoiceTypeId = it.Id 
                                            INNER JOIN Accounts AS a ON t.AccountId = a.Id 
                                            LEFT OUTER JOIN Invoice_Guests_Trans AS igt ON igt.InvoiceId = t.InvoicesId
                                            LEFT OUTER JOIN Guest AS g ON g.Id = igt.GuestId
                                            WHERE t.InvoicesId IS NOT NULL AND ISNULL(t.IsDeleted,0)=0 AND t.InvoicesId =@InvoiceID";

                string guestTerm = db.Query<string>(GuestTermQuery, new { InvoiceID = invoiceId }).FirstOrDefault();
                receiptModel.GuestTerm = guestTerm;

                string AdultsQuery = @"SELECT g.Adults AS Adults
                                            FROM Transactions AS t 
                                            INNER JOIN Invoices AS i ON t.InvoicesId = i.Id
                                            INNER JOIN InvoiceTypes AS it ON i.InvoiceTypeId = it.Id 
                                            INNER JOIN Accounts AS a ON t.AccountId = a.Id 
                                            LEFT OUTER JOIN Invoice_Guests_Trans AS igt ON igt.InvoiceId = t.InvoicesId
                                            LEFT OUTER JOIN Guest AS g ON g.Id = igt.GuestId
                                            WHERE t.InvoicesId IS NOT NULL AND ISNULL(t.IsDeleted,0)=0 AND t.InvoicesId =@InvoiceID";

                string adults = db.Query<string>(AdultsQuery, new { InvoiceID = invoiceId }).FirstOrDefault();
                receiptModel.Adults = adults;

                string KidsQuery = @"SELECT g.Children AS Kids
                                            FROM Transactions AS t 
                                            INNER JOIN Invoices AS i ON t.InvoicesId = i.Id
                                            INNER JOIN InvoiceTypes AS it ON i.InvoiceTypeId = it.Id 
                                            INNER JOIN Accounts AS a ON t.AccountId = a.Id 
                                            LEFT OUTER JOIN Invoice_Guests_Trans AS igt ON igt.InvoiceId = t.InvoicesId
                                            LEFT OUTER JOIN Guest AS g ON g.Id = igt.GuestId
                                            WHERE t.InvoicesId IS NOT NULL AND ISNULL(t.IsDeleted,0)=0 AND t.InvoicesId =@InvoiceID";

                string kids = db.Query<string>(KidsQuery, new { InvoiceID = invoiceId }).FirstOrDefault();
                receiptModel.Kids = kids;


                string pdaModuleIdQuery = @"SELECT i.PdaModuleId FROM Invoices AS i WHERE i.Id =@InvoiceID";
                long? pdaModuleId = db.Query<long?>(pdaModuleIdQuery, new { InvoiceID = invoiceId }).FirstOrDefault();
                receiptModel.PdaId = pdaModuleId;

                string pdaDescQuery = @"SELECT pm.[Description] FROM PdaModule AS pm WHERE pm.Id =@pdaID";
                string pdaDesc = db.Query<string>(pdaDescQuery, new { pdaID = pdaModuleId }).FirstOrDefault();
                receiptModel.PdaDescription = pdaDesc;

                string digitalSignatureQuery = @"SELECT ds.Images FROM DigitalSignature AS ds WHERE ds.InvoiceId = @InvoiceId";
                Byte[] digitalSignature = db.Query<Byte[]>(digitalSignatureQuery, new { InvoiceId = invoiceId }).FirstOrDefault();
                receiptModel.DigitalSignature = digitalSignature;

                string detailsQuery = @"SELECT odi.[Counter] AS InvoiceNo, odi.ItemRemark AS ItemCustomRemark, k.Code AS KitchenId, odi.ItemCode AS ItemCode,
	                                        odi.[Description] AS ItemDescr, CASE WHEN odi.Qty IS NOT NULL THEN odi.Qty ELSE 0 END ItemQty, odi.OrderDetailId AS OrderDetailId,
	                                        CASE WHEN odi.Price IS NOT NULL THEN odi.Price ELSE 0 END ItemPrice, odi.VatCode AS ItemVatRate, 
	                                        CASE WHEN odi.VatAmount IS NOT NULL THEN odi.VatAmount ELSE 0 END ItemVatValue, 
	                                        CASE WHEN odi.VatRate IS NOT NULL THEN odi.VatRate ELSE 0 END ItemVatDesc, odi.Discount AS ItemDiscount, 
	                                        CASE WHEN odi.Net IS NOT NULL THEN odi.Net ELSE 0 END ItemNet, CASE WHEN odi.Price IS NOT NULL THEN odi.Price ELSE 0 END ItemGross,
	                                        CASE WHEN odi.Total IS NOT NULL THEN odi.Total ELSE 0 END ItemTotal, odi.ItemPosition AS ItemPosition, odi.ItemSort AS ItemSort, 
	                                        odi.ItemRegion AS ItemRegion, odi.RegionPosition AS RegionPosition, odi.ItemBarcode AS ItemBarcode, st.[Description] AS SalesTypeExtDesc,
	                                        CASE WHEN odi.CreationTS IS NOT NULL THEN odi.CreationTS ELSE GETDATE() END [DATE], 
	                                        CASE WHEN odi.CreationTS IS NOT NULL THEN odi.CreationTS ELSE GETDATE() END [TIME],
	                                        CASE WHEN p.ExtraDescription IS NOT NULL THEN p.ExtraDescription ELSE '' END ExtraDescription, 
	                                        CASE WHEN p.SalesDescription IS NOT NULL THEN P.SalesDescription ELSE '' END SalesDescription,
                                            CASE WHEN odi.SalesTypeId IS NOT NULL THEN odi.SalesTypeId ELSE 0 END SalesTypeId
                                        FROM OrderDetailInvoices AS odi
                                        INNER JOIN OrderDetail AS od ON odi.OrderDetailId = od.Id
                                        LEFT OUTER JOIN Product AS p ON odi.ProductId = p.Id
                                        LEFT OUTER JOIN Kitchen AS k ON od.KitchenId = k.Id
                                        LEFT OUTER JOIN SalesType AS st ON odi.SalesTypeId = st.Id
                                        WHERE ISNULL(odi.IsDeleted,0)=0 AND odi.InvoicesId =@InvoiceID AND odi.IsExtra = 0";

                string detailsExtraQuery = @"SELECT odi.ItemCode AS ItemCode, odi.Discount AS ItemDiscount, odi.[Description] AS ItemDescr, 
	                                            CASE WHEN odi.Qty IS NOT NULL THEN odi.Qty ELSE 0 END ItemQty,
	                                            CASE WHEN odi.Price IS NOT NULL THEN odi.Price ELSE 0 END ItemPrice, CASE WHEN odi.Total IS NOT NULL THEN odi.Total ELSE 0 END ItemGross,
                                                odi.VatCode AS ItemVatRate, CASE WHEN odi.VatRate IS NOT NULL THEN odi.VatRate ELSE 0 END ItemVatDesc,
	                                            CASE WHEN odi.VatAmount IS NOT NULL THEN odi.VatAmount ELSE 0 END ItemVatValue, 
	                                            CASE WHEN odi.Net IS NOT NULL THEN odi.Net ELSE 0 END ItemNet
                                            FROM OrderDetailInvoices AS odi
                                            INNER JOIN OrderDetail AS od ON odi.OrderDetailId = od.Id
                                            LEFT OUTER JOIN Product AS p ON odi.ProductId = p.Id
                                            LEFT OUTER JOIN Kitchen AS k ON od.KitchenId = k.Id
                                            LEFT OUTER JOIN SalesType AS st ON odi.SalesTypeId = st.Id
                                            WHERE ISNULL(odi.IsDeleted,0)=0 AND odi.InvoicesId =@InvoiceID AND od.Id =@orderDetailID AND odi.IsExtra = 1";

                string checkForExtrasQuery = @"SELECT COUNT(*) FROM OrderDetailInvoices AS odi WHERE odi.OrderDetailId =@orderDetId AND odi.InvoicesId =@InvoiceID";




                List<EXTECR_ReceiptItemsModel> details = db.Query<EXTECR_ReceiptItemsModel>(detailsQuery, new { InvoiceID = invoiceId }).ToList();
                foreach (EXTECR_ReceiptItemsModel det in details)
                {
                    if (det.ItemTotal < 0 && ((det.InvoiceType != 8) && (det.InvoiceType != 3)))
                    {
                        det.ItemDescr = "ΑΛΛΑΓΗ " + det.ItemDescr;
                        det.ExtraDescription = "ΑΛΛΑΓΗ " + det.ExtraDescription;
                        det.SalesDescription = "ΑΛΛΑΓΗ " + det.SalesDescription;
                        det.IsChangeItem = true;
                    }
                    else
                    {
                        det.ItemDescr = det.ItemDescr;
                        det.ExtraDescription = det.ExtraDescription;
                        det.SalesDescription = det.SalesDescription;
                        det.IsChangeItem = false;
                    }
                    int checkForExtras = db.Query<int>(checkForExtrasQuery, new { orderDetId = det.OrderDetailId, InvoiceID = invoiceId }).FirstOrDefault();
                    if (checkForExtras > 1)
                    {
                        det.Extras = db.Query<EXTECR_ReceiptExtrasModel>(detailsExtraQuery, new { InvoiceID = invoiceId, orderDetailID = det.OrderDetailId }).ToList();
                    }
                }

                if (groupReceiptItems == false)
                {
                    receiptModel.Details = details;
                }

                else if (groupReceiptItems == true)
                {
                    List<EXTECR_ReceiptItemsModel> groupDetails = new List<EXTECR_ReceiptItemsModel>();
                    var tempGroupDetails = details.Where(d => d.ItemDiscount == 0).GroupBy(d => new { d.ItemCode, d.ItemPrice }).Select(g => g.ToList()).ToList();
                    foreach (var group in tempGroupDetails)
                    {
                        EXTECR_ReceiptItemsModel temp = new EXTECR_ReceiptItemsModel();
                        foreach (var item in group)
                        {
                            temp.Date = item.Date;
                            temp.ExtraDescription = item.ExtraDescription;
                            temp.Extras = item.Extras;
                            temp.InvoiceNo = item.InvoiceNo;
                            temp.InvoiceType = item.InvoiceType;
                            temp.IsChangeItem = item.IsChangeItem;
                            temp.IsVoid = item.IsVoid;
                            temp.ItemBarcode = item.ItemBarcode;
                            temp.ItemCode = item.ItemCode;
                            temp.ItemCustomRemark = item.ItemCustomRemark;
                            temp.ItemDescr = item.ItemDescr;
                            temp.ItemDiscount = item.ItemDiscount;
                            temp.ItemGross = item.ItemGross;

                            decimal net = 0;
                            group.ToList().ForEach(q => { net += q.ItemNet; });
                            temp.ItemNet = net;

                            decimal counter = 0;
                            group.ToList().ForEach(s => { counter += s.ItemQty; });
                            temp.ItemQty = counter;

                            decimal TotalPrice = 0;
                            group.ToList().ForEach(price => { TotalPrice += price.ItemTotal; });
                            temp.ItemTotal = TotalPrice;

                            decimal vat = 0;
                            group.ToList().ForEach(q => { vat += q.ItemVatValue; });
                            temp.ItemVatValue = vat;

                            temp.ItemPosition = item.ItemPosition;
                            temp.ItemPrice = item.ItemPrice;
                            temp.ItemRegion = item.ItemRegion;
                            temp.ItemSort = item.ItemSort;
                            temp.ItemVatDesc = item.ItemVatDesc;
                            temp.ItemVatRate = item.ItemVatRate;
                            temp.KitchenId = item.KitchenId;
                            temp.OrderDetailId = item.OrderDetailId;
                            temp.RegionPosition = item.RegionPosition;
                            temp.SalesDescription = item.SalesDescription;
                            temp.SalesTypeExtDesc = item.SalesTypeExtDesc;
                            temp.Time = item.Time;
                            groupDetails.Add(temp);
                            break;
                        }
                    }

                    var tempGroupDetailsWithDiscount = details.Where(d => d.ItemDiscount > 0).ToList();
                    foreach (var disItem in tempGroupDetailsWithDiscount)
                    {
                        EXTECR_ReceiptItemsModel temp = new EXTECR_ReceiptItemsModel();
                        temp.Date = disItem.Date;
                        temp.ExtraDescription = disItem.ExtraDescription;
                        temp.Extras = disItem.Extras;
                        temp.InvoiceNo = disItem.InvoiceNo;
                        temp.InvoiceType = disItem.InvoiceType;
                        temp.IsChangeItem = disItem.IsChangeItem;
                        temp.IsVoid = disItem.IsVoid;
                        temp.ItemBarcode = disItem.ItemBarcode;
                        temp.ItemCode = disItem.ItemCode;
                        temp.ItemCustomRemark = disItem.ItemCustomRemark;
                        temp.ItemDescr = disItem.ItemDescr;
                        temp.ItemDiscount = disItem.ItemDiscount;
                        temp.ItemGross = disItem.ItemGross;
                        temp.ItemNet = disItem.ItemNet;
                        temp.ItemQty = disItem.ItemQty;
                        temp.ItemPosition = disItem.ItemPosition;
                        temp.ItemPrice = disItem.ItemPrice;
                        temp.ItemRegion = disItem.ItemRegion;
                        temp.ItemSort = disItem.ItemSort;
                        temp.ItemTotal = disItem.ItemTotal;
                        temp.ItemVatDesc = disItem.ItemVatDesc;
                        temp.ItemVatRate = disItem.ItemVatRate;
                        temp.ItemVatValue = disItem.ItemVatValue;
                        temp.KitchenId = disItem.KitchenId;
                        temp.OrderDetailId = disItem.OrderDetailId;
                        temp.RegionPosition = disItem.RegionPosition;
                        temp.SalesDescription = disItem.SalesDescription;
                        temp.SalesTypeExtDesc = disItem.SalesTypeExtDesc;
                        temp.Time = disItem.Time;
                        groupDetails.Add(temp);
                    }

                    receiptModel.Details = groupDetails;
                }

                string salesTypeQuery = @"SELECT DISTINCT st.[Description] AS SalesTypeDescriptions
                                          FROM OrderDetailInvoices AS odi
                                          INNER JOIN OrderDetail AS od ON odi.OrderDetailId = od.Id
                                          LEFT OUTER JOIN Product AS p ON odi.ProductId = p.Id
                                          LEFT OUTER JOIN Kitchen AS k ON od.KitchenId = k.Id
                                          LEFT OUTER JOIN SalesType AS st ON odi.SalesTypeId = st.Id
                                          WHERE ISNULL(odi.IsDeleted,0)=0 AND odi.InvoicesId =@InvoiceID AND odi.IsExtra = 0 ";

                List<SalesTypeDescriptionsList> salesTypeList = new List<SalesTypeDescriptionsList>();
                salesTypeList = db.Query<SalesTypeDescriptionsList>(salesTypeQuery, new { InvoiceID = invoiceId }).ToList();
                receiptModel.SalesTypeDescriptions = salesTypeList;

                string CreditTransactionsQuery = @"SELECT ct.CreditAccountId AS CreditAccountId, SUM(ct.Amount) AS Amount
                                                    FROM CreditTransactions AS ct
                                                    WHERE ct.CreditAccountId IN
                                                        (SELECT CreditAccountId FROM CreditTransactions WHERE InvoiceId = @InvoiceID)
                                                    GROUP BY ct.CreditAccountId";

                List<EXTECR_CreditTransaction> creditTrasactionsList = new List<EXTECR_CreditTransaction>();
                creditTrasactionsList = db.Query<EXTECR_CreditTransaction>(CreditTransactionsQuery, new { InvoiceID = invoiceId }).ToList();
                receiptModel.CreditTransactions = creditTrasactionsList;

                string getOrderDetailsQuery = @"SELECT od.Id
                                                FROM OrderDetailInvoices AS odi
                                                INNER JOIN OrderDetail AS od ON odi.OrderDetailId = od.Id
                                                WHERE ISNULL(odi.IsDeleted,0)=0 AND odi.IsExtra = 0 and odi.InvoicesId =@InvoiceID";

                List<long> orderDetailIds = db.Query<long>(getOrderDetailsQuery, new { InvoiceID = invoiceId }).ToList();
                string input = "";
                input = String.Join(",", orderDetailIds);

                string relatedReceiptsQuery = @"SELECT odi.Abbreviation, odi.[Counter]
                                        FROM OrderDetailInvoices AS odi 
                                        WHERE odi.OrderDetailId IN (" + input + @") AND odi.InvoicesId !=@InvoiceID
                                        GROUP BY odi.Abbreviation, odi.[Counter]";

                List<EXTECR_Related> relatedReseiptsList = new List<EXTECR_Related>();
                List<string> RelReceiptsList = new List<string>();
                relatedReseiptsList = db.Query<EXTECR_Related>(relatedReceiptsQuery, new { InvoiceID = invoiceId }).ToList();

                foreach (EXTECR_Related rel in relatedReseiptsList)
                {
                    string relatedReceipts = "" + rel.Abbreviation + "" + rel.Counter + "";
                    RelReceiptsList.Add(relatedReceipts);
                }
                receiptModel.RelatedReceipts = RelReceiptsList;

                receiptModel.IsForKitchen = isForKitchen;

            }
            //Set RegionId to 1000 In case of Simply Burger
            bool hasDineIn = false;
            foreach (EXTECR_ReceiptItemsModel receiptItem in receiptModel.Details)
            {
                if (receiptItem.SalesTypeId == (long)OrderTypeStatus.DineIn)
                {
                    hasDineIn = true;
                    break;
                }
            }
            if (receiptModel.ExtType == (int)ExternalSystemOrderEnum.DeliveryAgent && !hasDineIn)
            {
                receiptModel.RegionId = 1000;
            }
            return receiptModel;
        }

    }
}
