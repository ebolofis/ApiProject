
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
    
public partial class DA_Orders
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public DA_Orders()
    {

        this.DA_CustomerMessages = new HashSet<DA_CustomerMessages>();

        this.DA_OrderDetails = new HashSet<DA_OrderDetails>();

        this.DA_OrderStatus = new HashSet<DA_OrderStatus>();

        this.Loyalty = new HashSet<Loyalty>();

    }


    public long Id { get; set; }

    public long CustomerId { get; set; }

    public long StoreId { get; set; }

    public string StoreCode { get; set; }

    public Nullable<long> GeoPolygonId { get; set; }

    public Nullable<long> ShippingAddressId { get; set; }

    public System.DateTime OrderDate { get; set; }

    public Nullable<System.DateTime> EstBillingDate { get; set; }

    public Nullable<System.DateTime> BillingDate { get; set; }

    public Nullable<System.DateTime> EstTakeoutDate { get; set; }

    public Nullable<System.DateTime> TakeoutDate { get; set; }

    public Nullable<System.DateTime> FinishDate { get; set; }

    public Nullable<int> Duration { get; set; }

    public Nullable<long> BillingAddressId { get; set; }

    public decimal Price { get; set; }

    public decimal Discount { get; set; }

    public decimal Total { get; set; }

    public short AccountType { get; set; }

    public short InvoiceType { get; set; }

    public short OrderType { get; set; }

    public short Status { get; set; }

    public System.DateTime StatusChange { get; set; }

    public string Remarks { get; set; }

    public long StoreOrderId { get; set; }

    public short IsSend { get; set; }

    public short Origin { get; set; }

    public string ExtObj { get; set; }

    public decimal TotalVat { get; set; }

    public decimal TotalTax { get; set; }

    public decimal NetAmount { get; set; }

    public bool ItemsChanged { get; set; }

    public bool IsPaid { get; set; }

    public Nullable<long> StoreOrderNo { get; set; }

    public Nullable<int> PointsGain { get; set; }

    public Nullable<int> PointsRedeem { get; set; }

    public bool IsDelay { get; set; }

    public string ExtId1 { get; set; }

    public string ExtId2 { get; set; }

    public Nullable<int> Cover { get; set; }

    public string ErrorMessage { get; set; }

    public string ExtData { get; set; }

    public string LogicErrors { get; set; }

    public string DiscountRemark { get; set; }

    public Nullable<long> Staffid { get; set; }

    public string AgentNo { get; set; }

    public string PaymentId { get; set; }

    public string LoyaltyCode { get; set; }

    public string TelephoneNumber { get; set; }

    public string Metadata { get; set; }

    public Nullable<long> OrderNo { get; set; }

    public Nullable<long> TableId { get; set; }



    public virtual DA_Addresses DA_Addresses { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DA_CustomerMessages> DA_CustomerMessages { get; set; }

    public virtual DA_Customers DA_Customers { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DA_OrderDetails> DA_OrderDetails { get; set; }

    public virtual DA_Stores DA_Stores { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DA_OrderStatus> DA_OrderStatus { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Loyalty> Loyalty { get; set; }

}

}