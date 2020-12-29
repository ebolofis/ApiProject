namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PosInfoDetail")]
    public partial class PosInfoDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PosInfoDetail()
        {
            OrderDetailInvoices = new HashSet<OrderDetailInvoice>();
            PosInfoDetail_Excluded_Accounts = new HashSet<PosInfoDetail_Excluded_Accounts>();
            PosInfoDetail_Pricelist_Assoc = new HashSet<PosInfoDetail_Pricelist_Assoc>();
        }

        public long Id { get; set; }

        public long? PosInfoId { get; set; }

        public long? Counter { get; set; }

        [StringLength(50)]
        public string Abbreviation { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public bool? ResetsAfterEod { get; set; }

        public short? InvoiceId { get; set; }

        [StringLength(250)]
        public string ButtonDescription { get; set; }

        public short? Status { get; set; }

        public bool? CreateTransaction { get; set; }

        public byte? FiscalType { get; set; }

        public bool? IsInvoice { get; set; }

        public bool? IsCancel { get; set; }

        public int? GroupId { get; set; }

        public long? InvoicesTypeId { get; set; }

        public bool? IsPdaHidden { get; set; }

        public bool? IsDeleted { get; set; }

        public short? SendsVoidToKitchen { get; set; }

        public virtual InvoiceType InvoiceType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailInvoice> OrderDetailInvoices { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfoDetail_Excluded_Accounts> PosInfoDetail_Excluded_Accounts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfoDetail_Pricelist_Assoc> PosInfoDetail_Pricelist_Assoc { get; set; }
    }
}
