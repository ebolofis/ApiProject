
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
    
public partial class Order
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Order()
    {

        this.OrderDetail = new HashSet<OrderDetail>();

        this.OrdersStaff = new HashSet<OrdersStaff>();

        this.OrderStatus = new HashSet<OrderStatus>();

        this.Transactions = new HashSet<Transactions>();

    }


    public long Id { get; set; }

    public Nullable<long> OrderNo { get; set; }

    public Nullable<System.DateTime> Day { get; set; }

    public Nullable<decimal> Total { get; set; }

    public Nullable<long> PosId { get; set; }

    public Nullable<long> StaffId { get; set; }

    public Nullable<long> EndOfDayId { get; set; }

    public Nullable<decimal> Discount { get; set; }

    public Nullable<int> ReceiptNo { get; set; }

    public Nullable<decimal> TotalBeforeDiscount { get; set; }

    public Nullable<long> PdaModuleId { get; set; }

    public Nullable<long> ClientPosId { get; set; }

    public Nullable<bool> IsDeleted { get; set; }

    public Nullable<int> ExtType { get; set; }

    public string ExtObj { get; set; }

    public string ExtKey { get; set; }

    public Nullable<int> OrderOrigin { get; set; }

    public Nullable<int> Couver { get; set; }

    public Nullable<bool> DA_IsPaid { get; set; }

    public Nullable<System.DateTime> EstTakeoutDate { get; set; }

    public Nullable<bool> IsDelay { get; set; }

    public string OrderNotes { get; set; }

    public string StoreNotes { get; set; }

    public string CustomerNotes { get; set; }

    public string CustomerSecretNotes { get; set; }

    public string CustomerLastOrderNotes { get; set; }

    public string LogicErrors { get; set; }

    public Nullable<bool> ItemsChanged { get; set; }

    public Nullable<short> DA_Origin { get; set; }

    public string LoyaltyCode { get; set; }

    public string TelephoneNumber { get; set; }

    public Nullable<int> CouverAdults { get; set; }

    public Nullable<int> CouverChildren { get; set; }

    public Nullable<System.Guid> MacroGuidId { get; set; }

    public Nullable<long> DeliveryRoutingId { get; set; }



    public virtual EndOfDay EndOfDay { get; set; }

    public virtual HotelMacros HotelMacros { get; set; }

    public virtual PdaModule PdaModule { get; set; }

    public virtual PosInfo PosInfo { get; set; }

    public virtual Staff Staff { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<OrderDetail> OrderDetail { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<OrdersStaff> OrdersStaff { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<OrderStatus> OrderStatus { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Transactions> Transactions { get; set; }

    public virtual DeliveryRouting DeliveryRouting { get; set; }

}

}
