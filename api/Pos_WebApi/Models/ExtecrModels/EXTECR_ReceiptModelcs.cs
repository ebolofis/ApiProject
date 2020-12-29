using Pos_WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos_WebApi.Models.ExtecrModels
{

    public class EXTECR_ReceiptModel
    {
        public EXTECR_ReceiptModel()
        {
            Details = new List<EXTECR_ReceiptItemsModel>();
            PaymentsList = new List<EXTECR_PaymentTypeModel>();
            CreditTransactions = new List<EXTECR_CreditTransaction>();
            RelatedReceipts = new List<string>();
        }
        public decimal? TableTotal { get; set; }
        public int PaymentTypeId { get; set; }
        public List<EXTECR_PaymentTypeModel> PaymentsList { get; set; }
        public bool PrintKitchen { get; set; }
        public string ReceiptTypeDescription { get; set; }
        public string DepartmentTypeDescription { get; set; }
        public decimal? PaidAmount { get; set; }
        public string SalesTypeDescription { get; set; }
        public int? ItemsCount { get; set; }
        public int? Couver { get; set; }
        public bool? PrintFiscalSign { get; set; }
        public FiscalTypeEnum FiscalType { get; set; }
        public string DetailsId { get; set; }
        public string InvoiceIndex { get; set; }
        public string OrderId { get; set; }
        public String TableNo { get; set; }
        public String RoomNo { get; set; }
        public String Waiter { get; set; }
        public String WaiterNo { get; set; }
        public String Pos { get; set; }
        public String PosDescr { get; set; }
        public String DepartmentDesc { get; set; }
        public String Department { get; set; }
        public String PaymentType { get; set; } // cash, credit card
        // Customer
        public String CustomerName { get; set; }
        public String CustomerAddress { get; set; }
        public String CustomerDeliveryAddress { get; set; }
        public String CustomerPhone { get; set; }
        public String Floor { get; set; }
        public String City { get; set; }
        public String CustomerComments { get; set; }
        public String CustomerAfm { get; set; }
        public String CustomerDoy { get; set; }
        public String CustomerJob { get; set; }
        public String RegNo { get; set; }
        public String SumOfLunches { get; set; }
        public String SumofConsumedLunches { get; set; }
        public String GuestTerm { get; set; }
        public String Adults { get; set; }
        public String Kids { get; set; }

        //Billing add 4 timologio
        public string BillingName { get; set; }
        public string BillingJob { get; set; }
        public string BillingVatNo { get; set; }
        public string BillingDOY { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingZipCode { get; set; }
        // Totals
        public Int16 InvoiceType { get; set; }
        public decimal? TotalVat { get; set; }
        public decimal? TotalVat1 { get; set; }
        public decimal? TotalVat2 { get; set; }
        public decimal? TotalVat3 { get; set; }
        public decimal? TotalVat4 { get; set; }
        public decimal? TotalVat5 { get; set; }
        public decimal? TotalDiscount { get; set; }
        public Int16 Bonus { get; set; }
        public Int16 PriceList { get; set; }
        public String ReceiptNo { get; set; }
        public String OrderNo { get; set; }
        public String OrderComments { get; set; }
        public decimal TotalNet { get; set; }
        public decimal Total { get; set; }
        public string Change { get; set; }
        public string CashAmount { get; set; }
        public string BuzzerNumber { get; set; }
        public string ExtECRCode { get; set; }
        /// <summary>
        /// How to print a receipt (print the whole receipt or a part of it)
        /// </summary>
        public PrintType PrintType { get; set; }
        /// <summary>
        /// contains the method which is going to be print as item's second line (for OPOS/OPOS3)
        /// </summary>
        public string ItemAdditionalInfo { get; set; }
        /// <summary>
        /// true if print without ADHME
        /// </summary>
        public bool TempPrint { get; set; }
        public long? PdaId { get; set; }
        public string PdaDescription { get; set; }
        public List<EXTECR_ReceiptItemsModel> Details { get; set; }
        public List<EXTECR_CreditTransaction> CreditTransactions { get; set; }
        public List<string> RelatedReceipts { get; set; }

        /// <summary>
        /// In use with section type 23, we get the SalesTypeDescription within receipt printout
        /// </summary>
        public List<SalesTypeDescriptionsList> SalesTypeDescriptions
        {

            get
            {
                return this.Details
                       .Select(s => s.SalesTypeExtDesc)
                       .Distinct()
                       .Select(f => new SalesTypeDescriptionsList { SalesTypeDescriptions = f })
                       .Distinct()
                       .ToList();
            }

        }
        public bool IsVoid { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class EXTECR_ReceiptItemsModel
    {
        public EXTECR_ReceiptItemsModel()
        {
            Extras = new List<EXTECR_ReceiptExtrasModel>();
        }
        public string InvoiceNo { get; set; }
        public string ItemCustomRemark { get; set; }
        public int? KitchenId { get; set; }
        public String ItemCode { get; set; }
        public String ItemDescr { get; set; }
        public decimal ItemQty { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemVatRate { get; set; }
        public decimal ItemVatValue { get; set; }
        public string ItemVatDesc { get; set; }
        public decimal? ItemDiscount { get; set; }
        public decimal ItemNet { get; set; }
        public decimal ItemGross { get; set; }
        public decimal ItemTotal { get; set; }
        public Int32 ItemPosition { get; set; }
        public Int32 ItemSort { get; set; }
        public String ItemRegion { get; set; }
        public int? RegionPosition { get; set; }
        public Int32 ItemBarcode { get; set; }
        public string SalesTypeExtDesc { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public List<EXTECR_ReceiptExtrasModel> Extras { get; set; }
        public bool IsVoid { get; set; }
        public bool IsChangeItem { get; set; }
        public String ExtraDescription { get; set; }
        public String SalesDescription { get; set; }

    }

    public class EXTECR_ReceiptExtrasModel
    {
        public EXTECR_ReceiptExtrasModel() { }
        public String ItemCode { get; set; }
        public decimal? ItemDiscount { get; set; }
        public String ItemDescr { get; set; }
        public decimal ItemQty { get; set; }
        public decimal? ItemPrice { get; set; }
        public decimal? ItemGross { get; set; }
        public int ItemVatRate { get; set; }
        public string ItemVatDesc { get; set; }
        public bool IsChangeItem { get; set; }
        public decimal ItemVatValue { get; set; }
        public decimal ItemNet { get; set; }
    }

    public class EXTECR_CustomerModel
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
        public String DeliveryAddress { get; set; }
        public String Phone { get; set; }
        public String Comments { get; set; }
        public String Afm { get; set; }
        public String Doy { get; set; }
        public String Job { get; set; }
        public String RegNo { get; set; }
        public String SumOfLunches { get; set; }
        public String SumofConsumedLunches { get; set; }
        public String GuestTerm { get; set; }
        public String Adults { get; set; }
        public String Kids { get; set; }

    }

    public class EXTECR_CreditTransaction
    {
        public Decimal? Amount { get; set; }
        public DateTime? CreationTS { get; set; }
        public Int32? Type { get; set; }
        public Int64? StaffId { get; set; }
        public String Description { get; set; }
        public Int64? PosInfoId { get; set; }
        public Int64? CreditAccountId { get; set; }
        public Int64? CreditCodeId { get; set; }
        public Decimal? Balance { get; set; }
    }

    public class SalesTypeDescriptionsList
    {
        public string SalesTypeDescriptions { get; set; }
    }
    public class EXTECR_PaymentTypeModel
    {
        public String Description { get; set; }
        public int? AccountType { get; set; }
        public decimal? Amount { get; set; }
        public EXTECR_Guest Guest { get; set; }
    }

    public class EXTECR_Guest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Room { get; set; }
    }

    public enum FiscalTypeEnum
    {

        /// <summary>
        /// Generic fiscal type
        /// </summary>
        Generic = 1,

        /// <summary>
        /// OPOS fiscal type
        /// </summary>
        Opos = 2,

    }

}
