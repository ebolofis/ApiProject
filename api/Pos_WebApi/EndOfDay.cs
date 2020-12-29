
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
    
public partial class EndOfDay
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EndOfDay()
    {

        this.CreditAccounts = new HashSet<CreditAccounts>();

        this.CreditTransactions = new HashSet<CreditTransactions>();

        this.OrderDetailInvoices = new HashSet<OrderDetailInvoices>();

        this.Transactions = new HashSet<Transactions>();

        this.TransferToPms = new HashSet<TransferToPms>();

        this.EndOfDayDetail = new HashSet<EndOfDayDetail>();

        this.EndOfDayPaymentAnalysis = new HashSet<EndOfDayPaymentAnalysis>();

        this.EndOfDayVoidsAnalysis = new HashSet<EndOfDayVoidsAnalysis>();

        this.KitchenInstructionLogger = new HashSet<KitchenInstructionLogger>();

        this.Lockers = new HashSet<Lockers>();

        this.MealConsumption = new HashSet<MealConsumption>();

        this.Order = new HashSet<Order>();

        this.StaffCash = new HashSet<StaffCash>();

        this.Invoices = new HashSet<Invoices>();

    }


    public long Id { get; set; }

    public Nullable<System.DateTime> FODay { get; set; }

    public Nullable<long> PosInfoId { get; set; }

    public Nullable<int> CloseId { get; set; }

    public Nullable<decimal> Gross { get; set; }

    public Nullable<decimal> Net { get; set; }

    public Nullable<int> TicketsCount { get; set; }

    public Nullable<int> ItemCount { get; set; }

    public Nullable<decimal> TicketAverage { get; set; }

    public Nullable<long> StaffId { get; set; }

    public Nullable<decimal> Discount { get; set; }

    public Nullable<decimal> Barcodes { get; set; }

    public Nullable<System.DateTime> dtDateTime { get; set; }

    public string zlogger { get; set; }

    public Nullable<System.DateTime> eodPMS { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<CreditAccounts> CreditAccounts { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<CreditTransactions> CreditTransactions { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<OrderDetailInvoices> OrderDetailInvoices { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Transactions> Transactions { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<TransferToPms> TransferToPms { get; set; }

    public virtual PosInfo PosInfo { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<EndOfDayDetail> EndOfDayDetail { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<EndOfDayPaymentAnalysis> EndOfDayPaymentAnalysis { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<EndOfDayVoidsAnalysis> EndOfDayVoidsAnalysis { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<KitchenInstructionLogger> KitchenInstructionLogger { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Lockers> Lockers { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<MealConsumption> MealConsumption { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Order> Order { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<StaffCash> StaffCash { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Invoices> Invoices { get; set; }

}

}
