using Pos_WebApi.Helpers;
using System;
using System.Collections.Generic;

namespace Pos_WebApi.Models
{
    //public partial class Receipts
    //{
    //    public Receipts()
    //    {
    //        ReceiptDetails = new HashSet<ReceiptDetails>();
    //        ReceiptPayments = new HashSet<ReceiptPayments>();
    //    }
    //    public long Id { get; set; }
    //    public DateTime? Day { get; set; }
    //    public string Abbreviation { get; set; }
    //    public string InvoiceDescription { get; set; }
    //    public int? ReceiptNo { get; set; }
    //    public long? InvoiceTypeId { get; set; }
    //    public long? EndOfDayId { get; set; }
    //    public DateTime? FODay { get; set; }
    //    public short? InvoiceTypeType { get; set; }
    //    public long? PosInfoId { get; set; }
    //    public long? PosInfoDetailId { get; set; }
    //    public long? ClientPosId { get; set; }
    //    public long? PdaModuleId { get; set; }
    //    public string PosInfoDescription { get; set;  }
    //    public long? DepartmentId { get; set; }
    //    public string DepartmentDescription { get; set; }
    //    public short? InvoiceIndex { get; set; }
    //    public int? Cover { get; set; }
    //    public long? TableId { get; set; }
    //    public string TableCode { get; set; }
    //    public decimal? Total { get; set; }
    //    public decimal? Discount { get; set; }
    //    public string DiscountRemark { get; set; }
    //    public decimal? Net { get; set; }
    //    public decimal? Vat { get; set; }
    //    public long? StaffId { get; set; }
    //    public string StaffCode { get; set; }
    //    public string StaffName { get; set; }
    //    public bool IsVoided { get; set; }
    //    public bool IsPrinted { get; set; }
    //    public string BillingAddress { get; set; }
    //    public long? BillingAddressId { get; set; }
    //    public string BillingCity { get; set; }
    //    public string BillingZipCode { get; set; }
    //    public string BillingName { get; set; }
    //    public string BillingVatNo { get; set; }
    //    public string BillingDOY { get; set; }
    //    public string BillingJob { get; set; }
    //    public string CustomerName { get; set; }
    //    public long? CustomerID { get; set; }
    //    public string CustomerRemarks { get; set; }
    //    public string Floor { get; set; }
    //    public double? Latitude { get; set; }
    //    public double? Longtitude { get; set; }
    //    public string Phone { get; set; }
    //    public string ShippingAddress { get; set; }
    //    public long? ShippingAddressId { get; set; }
    //    public string ShippingCity { get; set; }
    //    public string ShippingZipCode { get; set; }
    //    //Extra Fields
    //    public string OrderNo { get; set; }
    //    public string Room { get; set; }
    //    public string PaymentsDesc { get; set; }
    //    public short IsPaid { get; set; }
    //    public decimal? PaidTotal { get; set; }
    //    public string StaffLastName { get; set; }
    //    public bool CreateTransactions { get; set; }
    //    public short ModifyOrderDetails { get; set; }
    //    public int Points { get; set; }
    //    public decimal? LoyaltyDiscount { get; set; }

    //    //New Variable for DigitalSignature
    //    //public Byte[] DigitalSignatureImage { get; set; }
    //    public string DigitalSignatureImage { get; set; }

    //    /// <summary>
    //    /// The remaining amount to pay in respective table (if there is a table, else null)
    //    /// </summary>
    //    public decimal? TableSum { get; set; }
    //    /// <summary>
    //    /// The ammount the customer give us so we will compute his change.
    //    /// </summary>
    //    public string CashAmount { get; set; }
    //    /// <summary>
    //    /// The number of the buzzer related with current invoice
    //    /// </summary>
    //    public string BuzzerNumber { get; set; }
    //    /// <summary>
    //    /// An enumerator concerning the origin of an order (eg. 'local', 'call center', see origin enumerator in BOEnums.cs) 
    //    /// </summary>
    //    public int? OrderOrigin { get; set; }
    //    /// <summary>
    //    /// How to print a receipt (print the whole receipt or a part of it)
    //    /// </summary>
    //    public PrintType PrintType { get; set; }
    //    /// <summary>
    //    /// contains the method which is going to be print as item's second line (for OPOS/OPOS3)
    //    /// </summary>
    //    public string ItemAdditionalInfo { get; set; }
    //    /// <summary>
    //    /// true if print without ADHME
    //    /// </summary>
    //    public bool TempPrint { get; set; }

    //    public  ICollection<ReceiptDetails> ReceiptDetails { get; set; }
    //    public  ICollection<ReceiptPayments> ReceiptPayments { get; set; }
        

    //    //New Delivery Fields
    //    public String ExtKey { get; set; }
    //    public int? ExtType { get; set; }
    //    public String ExtObj { get; set; }

    //}
}