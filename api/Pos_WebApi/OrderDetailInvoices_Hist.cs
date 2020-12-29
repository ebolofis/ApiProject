
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Pos_WebApi
{

using System;
    using System.Collections.Generic;
    
public partial class OrderDetailInvoices_Hist
{

    public int nYear { get; set; }

    public long Id { get; set; }

    public Nullable<long> OrderDetailId { get; set; }

    public Nullable<long> StaffId { get; set; }

    public Nullable<long> PosInfoDetailId { get; set; }

    public Nullable<long> Counter { get; set; }

    public Nullable<System.DateTime> CreationTS { get; set; }

    public Nullable<bool> IsPrinted { get; set; }

    public string CustomerId { get; set; }

    public Nullable<long> InvoicesId { get; set; }

    public Nullable<bool> IsDeleted { get; set; }

    public Nullable<long> OrderDetailIgredientsId { get; set; }

    public Nullable<decimal> Price { get; set; }

    public Nullable<decimal> Discount { get; set; }

    public Nullable<decimal> Net { get; set; }

    public Nullable<decimal> VatRate { get; set; }

    public Nullable<decimal> VatAmount { get; set; }

    public Nullable<long> VatId { get; set; }

    public Nullable<long> TaxId { get; set; }

    public Nullable<int> VatCode { get; set; }

    public Nullable<decimal> TaxAmount { get; set; }

    public Nullable<double> Qty { get; set; }

    public Nullable<decimal> Total { get; set; }

    public Nullable<long> PricelistId { get; set; }

    public Nullable<long> ProductId { get; set; }

    public string Description { get; set; }

    public string ItemCode { get; set; }

    public string ItemRemark { get; set; }

    public Nullable<int> InvoiceType { get; set; }

    public Nullable<long> TableId { get; set; }

    public string TableCode { get; set; }

    public Nullable<long> RegionId { get; set; }

    public Nullable<long> OrderNo { get; set; }

    public Nullable<long> OrderId { get; set; }

    public bool IsExtra { get; set; }

    public Nullable<long> PosInfoId { get; set; }

    public Nullable<long> EndOfDayId { get; set; }

    public string Abbreviation { get; set; }

    public Nullable<long> DiscountId { get; set; }

    public Nullable<long> SalesTypeId { get; set; }

    public Nullable<long> ProductCategoryId { get; set; }

    public Nullable<long> CategoryId { get; set; }

    public int ItemPosition { get; set; }

    public int ItemSort { get; set; }

    public string ItemRegion { get; set; }

    public Nullable<int> RegionPosition { get; set; }

    public int ItemBarcode { get; set; }

    public Nullable<decimal> TotalBeforeDiscount { get; set; }

    public Nullable<decimal> TotalAfterDiscount { get; set; }

    public Nullable<decimal> ReceiptSplitedDiscount { get; set; }

    public string TableLabel { get; set; }

}

}