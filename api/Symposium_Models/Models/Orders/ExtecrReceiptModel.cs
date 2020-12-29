using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class ExtecrReceiptModel
    {
        //### Added fields on Extecr on 22/5/19 ###
        //InvoiceShippingDetails
        public float Longtitude { get; set; }
        public float Latitude { get; set; }
        public string Phone { get; set; }

        //Order
        public Nullable <DateTime> EstTakeoutDate { get; set; }
        public Nullable<bool> IsDelay { get; set; }
        public string OrderNotes { get; set; }
        public string StoreNotes { get; set; }
        public string CustomerSecretNotes { get; set; }
        public string CustomerLastOrderNotes { get; set; }
        public string LogicErrors { get; set; }
        public Nullable<OrderOriginEnum> DA_Origin { get; set; }
        public Nullable<int> ExtType { get; set; }
        public string LoyaltyCode { get; set; }
        public string TelephoneNumber { get; set; }
        //### Added fields on Extecr on 22/5/19 ###

        /// <summary>
        /// Total from Invoices
        /// </summary>
        public decimal? TableTotal { get; set; }
        /// <summary>
        /// PaymentTypeId (never used)
        /// </summary>
        public int PaymentTypeId { get; set; }
        /// <summary>
        /// PaymentsList
        /// </summary>
        public List<Extecr_PaymentTypeModel> PaymentsList { get; set; }
        /// <summary>
        /// PrintKitchen (never used)
        /// </summary>
        public bool PrintKitchen { get; set; }
        /// <summary>
        /// Description from Invoices and if Description is NULL get Abbreviation from OrderDetailInvoices 
        /// </summary>
        public string ReceiptTypeDescription { get; set; }
        /// <summary>
        /// Description from Department
        /// </summary>
        public string DepartmentTypeDescription { get; set; }
        /// <summary>
        /// PaidAmount (never used)
        /// </summary>
        public decimal? PaidAmount { get; set; }
        /// <summary>
        /// Description from SalesType
        /// </summary>
        public string SalesTypeDescription { get; set; }
        /// <summary>
        /// ItemsCount (never used)
        /// </summary>
        public int? ItemsCount { get; set; }
        /// <summary>
        /// Cover from Invoices
        /// </summary>
        public int? Couver { get; set; }
        /// <summary>
        /// PrintFiscalSign (never used)
        /// </summary>
        public bool? PrintFiscalSign { get; set; }
        /// <summary>
        /// FiscalType Enum
        /// </summary>
        public FiscalTypeEnum FiscalType { get; set; }
        /// <summary>
        /// DetailsId (never used)
        /// </summary>
        public string DetailsId { get; set; }
        /// <summary>
        /// InvoiceId from PosInfoDetail
        /// </summary>
        public string InvoiceIndex { get; set; }
        /// <summary>
        /// OrderNo from Invoices
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        ///Code from Table
        /// </summary>
        public String TableNo { get; set; }
        public Nullable<bool> DA_IsPaid { get; set; }
        public Nullable<bool> ItemsChanged { get; set; }
        /// <summary>
        /// RegionId from Table
        /// </summary>
        public long? RegionId { get; set; }
        /// <summary>
        /// Description from Region
        /// </summary>
        public string RegionDescription { get; set; }
        /// <summary>
        ///Rooms from Invoices
        /// </summary>
        public String RoomNo { get; set; }
        /// <summary>
        ///FullName(FirstName, LastName) from Staff
        /// </summary>
        public String Waiter { get; set; }
        /// <summary>
        ///Code from Staff
        /// </summary>
        public String WaiterNo { get; set; }
        /// <summary>
        ///PosInfoId from Invoices
        /// </summary>
        public String Pos { get; set; }
        /// <summary>
        ///Description from PosInfo
        /// </summary>
        public String PosDescr { get; set; }
        /// <summary>
        /// Description from Department
        /// </summary>
        public String DepartmentDesc { get; set; }
        /// <summary>
        /// DepartmentId from PosInfo
        /// </summary>
        public String Department { get; set; }
        /// <summary>
        /// PaymentsDesc from Invoices
        /// </summary>
        public String PaymentType { get; set; } // cash, credit card
        /// <summary>
        /// CustomerName from InvoiceShippingDetails (if NULL then "Πελάτης Λιανικής")
        /// </summary>
        public String CustomerName { get; set; }
        /// <summary>
        /// BillingAddress from InvoiceShippingDetails
        /// </summary>
        public String CustomerAddress { get; set; }
        /// <summary>
        /// ShippingAddress from InvoiceShippingDetails
        /// </summary>
        public String CustomerDeliveryAddress { get; set; }
        /// <summary>
        /// Phone from InvoiceShippingDetails
        /// </summary>
        public String CustomerPhone { get; set; }
        /// <summary>
        /// Floor from InvoiceShippingDetails
        /// </summary>
        public String Floor { get; set; }
        /// <summary>
        /// BillingCity from InvoiceShippingDetails
        /// </summary>
        public String City { get; set; }
        /// <summary>
        /// CustomerRemarks from InvoiceShippingDetails
        /// </summary>
        public String CustomerComments { get; set; }
        /// <summary>
        /// CustomerAfm = ""
        /// </summary>
        public String CustomerAfm { get; set; }
        /// <summary>
        /// CustomerDoy = ""
        /// </summary>
        public String CustomerDoy { get; set; }
        /// <summary>
        /// CustomerJob = ""
        /// </summary>
        public String CustomerJob { get; set; }
        /// <summary>
        /// RegNo (never used)
        /// </summary>
        public String RegNo { get; set; }
        /// <summary>
        /// SumOfLunches (never used)
        /// </summary>
        public String SumOfLunches { get; set; }
        /// <summary>
        /// SumofConsumedLunches (never used)
        /// </summary>
        public String SumofConsumedLunches { get; set; }
        /// <summary>
        /// BoardCode from Guest
        /// </summary>
        public String GuestTerm { get; set; }
        /// <summary>
        /// Adults from Guest
        /// </summary>
        public String Adults { get; set; }
        /// <summary>
        /// Children from Guest
        /// </summary>
        public String Kids { get; set; }
        /// <summary>
        /// BillingName from InvoiceShippingDetails
        /// </summary>
        public string BillingName { get; set; }
        /// <summary>
        /// BillingJob from InvoiceShippingDetails
        /// </summary>
        public string BillingJob { get; set; }
        /// <summary>
        /// BillingVatNo from InvoiceShippingDetails
        /// </summary>
        public string BillingVatNo { get; set; }
        /// <summary>
        /// BillingDOY from InvoiceShippingDetails
        /// </summary>
        public string BillingDOY { get; set; }
        /// <summary>
        /// BillingAddress from InvoiceShippingDetails
        /// </summary>
        public string BillingAddress { get; set; }
        /// <summary>
        /// BillingCity from InvoiceShippingDetails
        /// </summary>
        public string BillingCity { get; set; }
        /// <summary>
        /// BillingZipCode from InvoiceShippingDetails
        /// </summary>
        public string BillingZipCode { get; set; }
        /// <summary>
        /// Type from InvoiceTypes
        /// </summary>
        public Int16 InvoiceType { get; set; }
        /// <summary>
        /// Vat from Invoices
        /// </summary>
        public decimal ? TotalNetVat1 { get; set; }
        public decimal? TotalNetVat2 { get; set; }
        public decimal? TotalNetVat3 { get; set; }
        public decimal? TotalNetVat4 { get; set; }
        public decimal? TotalNetVat5 { get; set; }
     
        public decimal? TotalVat { get; set; }
        /// <summary>
        /// TotalVat1 (never used)
        /// </summary>
        public decimal? TotalVat1 { get; set; }
        /// <summary>
        /// TotalVat2 (never used)
        /// </summary>
        public decimal? TotalVat2 { get; set; }
        /// <summary>
        /// TotalVat3 (never used)
        /// </summary>
        public decimal? TotalVat3 { get; set; }
        /// <summary>
        /// TotalVat4 (never used)
        /// </summary>
        public decimal? TotalVat4 { get; set; }
        /// <summary>
        /// TotalVat5 (never used)
        /// </summary>
        public decimal? TotalVat5 { get; set; }
        /// <summary>
        /// TotalDiscount (na to koitaksw )
        /// </summary>
        public decimal? TotalDiscount { get; set; }
        /// <summary>
        /// Bonus (never used)
        /// </summary>
        public Int16 Bonus { get; set; }
        /// <summary>
        /// PriceList (never used)
        /// </summary>
        public Int16 PriceList { get; set; }
        /// <summary>
        /// Counter from Invoices
        /// </summary>
        public String ReceiptNo { get; set; }
        /// <summary>
        /// OrderNo from Invoices
        /// </summary>
        public String OrderNo { get; set; }
        /// <summary>
        /// OrderComments (never used)
        /// </summary>
        public String OrderComments { get; set; }
        /// <summary>
        /// Net from Invoices
        /// </summary>
        public decimal TotalNet { get; set; }
        /// <summary>
        /// Total from Invoices
        /// </summary>
        public decimal Total { get; set; }
        /// <summary>
        /// Change (never used)
        /// </summary>
        public string Change { get; set; }
        /// <summary>
        /// CashAmount from Invoices
        /// </summary>
        public string CashAmount { get; set; }
        /// <summary>
        /// BuzzerNumber from Invoices
        /// </summary>
        public string BuzzerNumber { get; set; }
        /// <summary>
        /// How to print a receipt (print the whole receipt or a part of it)
        /// </summary>
        public PrintTypes PrintType { get; set; }
        /// <summary>
        /// contains the method which is going to be print as item's second line (for OPOS/OPOS3)
        /// </summary>
        public string ItemAdditionalInfo { get; set; }
        /// <summary>
        /// true if print without ADHME
        /// </summary>
        public bool TempPrint { get; set; }
        /// <summary>
        /// PdaModuleId from Invoices
        /// </summary>
        public long? PdaId { get; set; }
        public string PdaDescription { get; set; }

        public Byte[] DigitalSignature { get; set; }
        public string ExtECRCode { get; set; }

        public List<EXTECR_ReceiptItemsModel> Details { get; set; }
        public List<EXTECR_CreditTransaction> CreditTransactions { get; set; }
        public List<string> RelatedReceipts { get; set; }
         public List<CalculateVatPrice> CalculateVatPrice { get; set; }
        /// <summary>
        /// In use with section type 23, we get the SalesTypeDescription within receipt printout
        /// </summary>
        public List<SalesTypeDescriptionsList> SalesTypeDescriptions { get; set; }

        public bool IsVoid { get; set; }

        public bool IsForKitchen { get; set; }

        public Nullable<int> PointsGain { get; set; }

        public Nullable<int> PointsRedeem { get; set; }

        public string CouponCodes { get; set; }

        public Nullable<int> CouverAdults { get; set; }

        public Nullable<int> CouverChildren { get; set; }
    }



    public class EXTECR_ReceiptItemsModel
    {
        public EXTECR_ReceiptItemsModel()
        {
            Extras = new List<EXTECR_ReceiptExtrasModel>();
        }
        public string InvoiceNo { get; set; }
        public int InvoiceType { get; set; }
        public long OrderDetailId { get; set; }
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
        public long SalesTypeId { get; set; }

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
    public class Extecr_PaymentTypeModel
    {
        public String Description { get; set; }
        public int? AccountType { get; set; }
        public decimal? Amount { get; set; }
        public long TransactionId { get; set; }
        public EXTECR_Guest Guest { get; set; }
    }

    public class EXTECR_Related
    {
        public string Abbreviation { get; set; }

        public long Counter { get; set; }
    }

    public class EXTECR_Guest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Room { get; set; }
    }

    public class CalculateVatPrice
    {
        public decimal? Total { get; set; }
        public string VatRate { get; set; }
        public decimal? VatPrice { get; set; }
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

    public enum PrintTypes
    {
        /// <summary>
        /// When a receipt signaled from webapi, Print the whole receipt at once.
        /// </summary>
        PrintWhole = 0,
        /// <summary>
        /// When a receipt signaled from webapi, Print only the last item.
        /// </summary>
        PrintItem = 1,
        /// <summary>
        /// Print the receipt's footer only
        /// </summary>
        PrintEnd = 2,
        /// <summary>
        /// Cancel the current receipt
        /// </summary>
        CancelCurrentReceipt = 3,
        /// <summary>
        /// Print only the last extra of the last item.
        /// </summary>
        PrintExtra = 4,
        /// <summary>
        /// Print the discount of the last item.
        /// </summary>
        PrintItemDiscount = 5
    }

}
