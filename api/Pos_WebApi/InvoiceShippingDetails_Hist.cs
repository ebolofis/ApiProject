
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
    
public partial class InvoiceShippingDetails_Hist
{

    public int nYear { get; set; }

    public long Id { get; set; }

    public Nullable<long> InvoicesId { get; set; }

    public Nullable<long> ShippingAddressId { get; set; }

    public string ShippingAddress { get; set; }

    public string ShippingCity { get; set; }

    public string ShippingZipCode { get; set; }

    public Nullable<long> BillingAddressId { get; set; }

    public string BillingAddress { get; set; }

    public string BillingCity { get; set; }

    public string BillingZipCode { get; set; }

    public string BillingName { get; set; }

    public string BillingVatNo { get; set; }

    public string BillingDOY { get; set; }

    public string BillingJob { get; set; }

    public string Floor { get; set; }

    public string CustomerRemarks { get; set; }

    public string StoreRemarks { get; set; }

    public Nullable<long> CustomerID { get; set; }

    public string CustomerName { get; set; }

    public Nullable<double> Longtitude { get; set; }

    public Nullable<double> Latitude { get; set; }

    public string Phone { get; set; }

}

}