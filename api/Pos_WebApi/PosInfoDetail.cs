
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
    
public partial class PosInfoDetail
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public PosInfoDetail()
    {

        this.OrderDetailInvoices = new HashSet<OrderDetailInvoices>();

        this.PosInfoDetail_Excluded_Accounts = new HashSet<PosInfoDetail_Excluded_Accounts>();

        this.PosInfoDetail_Pricelist_Assoc = new HashSet<PosInfoDetail_Pricelist_Assoc>();

    }


    public long Id { get; set; }

    public Nullable<long> PosInfoId { get; set; }

    public Nullable<long> Counter { get; set; }

    public string Abbreviation { get; set; }

    public string Description { get; set; }

    public Nullable<bool> ResetsAfterEod { get; set; }

    public Nullable<short> InvoiceId { get; set; }

    public string ButtonDescription { get; set; }

    public Nullable<short> Status { get; set; }

    public Nullable<bool> CreateTransaction { get; set; }

    public Nullable<byte> FiscalType { get; set; }

    public Nullable<bool> IsInvoice { get; set; }

    public Nullable<bool> IsCancel { get; set; }

    public Nullable<int> GroupId { get; set; }

    public Nullable<long> InvoicesTypeId { get; set; }

    public Nullable<bool> IsPdaHidden { get; set; }

    public Nullable<bool> IsDeleted { get; set; }

    public Nullable<short> SendsVoidToKitchen { get; set; }

    public Nullable<long> PMSInvoiceId { get; set; }

    public string Background { get; set; }

    public string Color { get; set; }



    public virtual InvoiceTypes InvoiceTypes { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<OrderDetailInvoices> OrderDetailInvoices { get; set; }

    public virtual PosInfo PosInfo { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PosInfoDetail_Excluded_Accounts> PosInfoDetail_Excluded_Accounts { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PosInfoDetail_Pricelist_Assoc> PosInfoDetail_Pricelist_Assoc { get; set; }

}

}
