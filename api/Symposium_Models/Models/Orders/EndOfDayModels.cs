using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{

    /// <summary>
    /// Contains the data for the (EndOf)Day Preview page for a specific POS. Contains:
    /// <para>List of Totals by type for all staffs :EndOfDayTotalModel <seealso cref="Symposium.Models.Models.EndOfDayTotalModel"/></para>
    /// <para>List of Staffs. Each list's item contains a list of Totals by type (EndOfDayByStaffModel) <seealso cref="Symposium.Models.Models.EndOfDayByStaffModel"/></para>
    /// </summary>
    public class EndOfDayPreviewModel 
    {
        /// <summary>
        /// List of EndOfDayTotalModel <seealso cref="Symposium.Models.Models.EndOfDayTotalModel"/>
        /// </summary>
        public List<EndOfDayTotalModel> Totals { get; set; }

        /// <summary>
        /// List of EndOfDayByStaffModel <seealso cref="Symposium.Models.Models.EndOfDayByStaffModel"/>
        /// </summary>
        public List<List<EndOfDayByStaffModel>> TotalsByStaff { get; set; }

        /// <summary>
        /// Sum of barcode add or remove transactions
        /// </summary>
        public EndOfDayBarcodesModel BarcodeTotals { get; set; }

        /// <summary>
        /// Sum of locker transactions
        /// </summary>
        public long LockerTotals { get; set; }
    }



    /// <summary>
    /// Represent a single Type of Totals for the current day (mainly used into EndOfDay preview). 
    /// <para>ex: Not Paid or Canceled or Not Printed</para>
    /// </summary>
    public class EndOfDayTotalModel 
    {
        /// <summary>
        /// Type of Totals. 
        /// <seealso cref="Symposium.Models.Enums.EndOfDayReceiptTypes"/>
        /// </summary>
        public EndOfDayReceiptTypes Id { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Number of receipts
        /// </summary>
        public int ReceiptCount { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// AccountId
        /// </summary>
        public long AccountId { get; set; }

        /// <summary>
        /// AccountType
        /// </summary>
        public int AccountType { get; set; }
    }

    /// <summary>
    ///Represent a single Type of Totals for the current day for a Staff(mainly used into EndOfDay preview). 
    /// <para>ex: Not Paid or Canceled or Not Printed</para>
    /// <seealso cref="Symposium.Models.Models.EndOfDayTotalModel"/>
    /// </summary>
    public class EndOfDayByStaffModel: EndOfDayTotalModel
    {
        /// <summary>
        /// Staff.Id
        /// </summary>
        public long StaffId { get; set; }

        /// <summary>
        /// Staff name
        /// </summary>
        public string StaffName { get; set; }
    }

    public class EndOfDayBarcodesModel
    {
        /// <summary>
        /// Cash
        /// </summary>
        public decimal Cash { get; set; }

        /// <summary>
        /// Credit Card
        /// </summary>
        public decimal CreditCard { get; set; }
    }

    /// <summary>
    /// Model that describes a receipt for End-of-Day analysis
    /// </summary>
    public class EODAnalysisReceiptModel 
    {
        public long Id { get; set; }
        public DateTime Day { get; set; }
        public string Description { get; set; }
        public string DepartmentDescription { get; set; }
        public long PosInfoId { get; set; }
        /// <summary>
        /// Receipt Abbreviation
        /// </summary>
        public string Abbreviation { get; set; }
        public int Couver { get; set; }
        public string StaffName { get; set; }
        public long StaffId { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal PaidTotal { get; set; }
        public int ItemsCount { get; set; }
        /// <summary>
        /// one or more Order numbers (in case a receipt comes from 2 or more orders), OrderNo is comma separeted values
        /// </summary>
        public string OrderNo { get; set; }
        public long ReceiptNo { get; set; }
        public int InvoiceType { get; set; }
        public bool IsInvoiced { get; set; }
        public bool IsVoided { get; set; }
        public int IsPaid { get; set; }
        public long InvoiceId { get; set; }
        public string Room { get; set; }
        public string TableCode { get; set; }
        public long TableId { get; set; }

        public long AccountId { get; set; }

    }

    /// <summary>
    /// Model containing the list of Receipts with a specific Account
    /// </summary>
    public class EODReceiptListPerAccountModel 
    {

        /// <summary>
        /// the list of Receipts with a specific Account
        /// </summary>
        public List<EODAnalysisReceiptModel> List { get; set; }

        /// <summary>
        /// Account's description
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Account's id
        /// </summary>
        public long AccountId { get; set; }

        /// <summary>
        /// the number of Receipts in the list
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Receipt's total ammount
        /// </summary>
        public decimal Total { get; set; }

    }

    /// <summary>
    /// Model containing ListS of Receipts. Every item in the list contains a list of receipts with a specific Account
    /// </summary>
    public class EodTotalReportAnalysisModel 
    {
        /// <summary>
        /// a list of receipts with a specific accounts
        /// </summary>
        public List<EODReceiptListPerAccountModel> ReceiptsPerAccount { get; set; }
    }

    public class EodXAndZTotalsForExtecr
    {
        public Nullable<long> EndOfDayId { get; set; }
        public long PosInfoId { get; set; }
        public string Day { get; set; }
        public string dtDateTime { get; set; }
        public string PosCode { get; set; }
        public string PosDescription { get; set; }
        public Nullable<long> ReportNo { get; set; }
        public decimal Gross { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Net { get; set; }
        public decimal Discount { get; set; }
        public long TicketCount { get; set; }
        public long ItemsCount { get; set; }
        public Nullable<decimal> Barcodes { get; set; }
        public LockersModel Lockers { get; set; }
        public List<PaymentAnalysisModel> PaymentAnalysis { get; set; }
        public List<VoidAnalysisModel> VoidAnalysis { get; set; }
        public List<CardAnalysisModel> CardAnalysis { get; set; }
        public List<VatAnalysisModel> vatAnalysis { get; set; }
        public List<ProductAnalysisModel> ProductsForEODStats { get; set; }
        public string FiscalName { get; set; }

    }

    public class PaymentAnalysisModel
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class VoidAnalysisModel
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class CardAnalysisModel
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class VatAnalysisModel
    {
        public long VatId { get; set; }
        public decimal VatRate { get; set; }
        public decimal Tax { get; set; }
        public decimal Gross { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Net { get; set; }
    }

    public class ProductAnalysisModel
    {
        public string Description { get; set; }
        public decimal Qty { get; set; }
        public decimal Total { get; set; }
    }

    public class EodXAndZTotalsModel
    {
        public List<EodVatAnalysisModel> vatAnalysis { get; set; }
        public List<EodAccountAnalysisModel> paymentAnalysis { get; set; }
        public List<EodAccountAnalysisModel> voidAnalysis { get; set; }
        public List<EodAccountAnalysisModel> barcodeAnalysis { get; set; }
        public List<ProductForEodAnalysisModel> productsForEodAnalysis { get; set; }
        public LockersModel lockers { get; set; }
        public PosInfoModel posInfo { get; set; }
        public EndOfDayModel endOfDay { get; set; }
        public long ticketsCount { get; set; }
    }

    public class EodVatAnalysisModel
    {
        public int VatId { get; set; }
        public decimal VatRate { get; set; }
        public decimal Gross { get; set; }
        public decimal Net { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public long ItemCount { get; set; }
        public long TicketCount { get; set; }      
    }

    public class EodAccountAnalysisModel
    {
        public long AccountId { get; set; }
        public string Description { get; set; }
        public int AccountType { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProductForEodAnalysisModel
    {
        public long ProductId { get; set; }
        public string Description { get; set; }
        public long Quantity { get; set; }
        public decimal Total { get; set; }
    }

    public class EodTransferToPmsModel
    {
        public int PmsDepartmentId { get; set; }
        public string PmsDepartmentDescription { get; set; }
        public int hotelId { get; set; }
        public string roomDescription { get; set; }
        public string Profilename { get; set; }
        public decimal total { get; set; }
    }

    public class CashierStatisticsModel
    {
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public DateTime Date { get; set; }
    }

    public class CashierTotalAmountsModel
    {
        public decimal IncomeAmount { get; set; }
        public decimal OutcomeAmount { get; set; }
        public long AccountId { get; set; }
        public string AccountDescr { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CashierTotals
    {
        public decimal TotalIncomeAmount { get; set; }
        public decimal TotalOutcomeAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CreditCardsReceiptsCounts
    {
        public long AlreadyPaid { get; set; }
        public decimal AlreadyPaidAmount { get; set; }
        public long ExpectedPaid { get; set; }
        public decimal ExpectedPaidAmount { get; set; }
    }
}
