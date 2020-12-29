namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice()
        {
            CreditTransactions = new HashSet<CreditTransaction>();
            Invoice_Guests_Trans = new HashSet<Invoice_Guests_Trans>();
            InvoiceShippingDetails = new HashSet<InvoiceShippingDetail>();
            OrderDetailInvoices = new HashSet<OrderDetailInvoice>();
            Transactions = new HashSet<Transaction>();
        }

        public long Id { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public long? InvoiceTypeId { get; set; }

        public int? Counter { get; set; }

        public DateTime? Day { get; set; }

        public bool? IsPrinted { get; set; }

        public long? GuestId { get; set; }

        [Column(TypeName = "money")]
        public decimal? Total { get; set; }

        [Column(TypeName = "money")]
        public decimal? Discount { get; set; }

        public decimal? Vat { get; set; }

        public decimal? Tax { get; set; }

        [Column(TypeName = "money")]
        public decimal? Net { get; set; }

        public long? StaffId { get; set; }

        public int? Cover { get; set; }

        public long? TableId { get; set; }

        public long? PosInfoId { get; set; }

        public long? PdaModuleId { get; set; }

        public long? ClientPosId { get; set; }

        public long? EndOfDayId { get; set; }

        public long? PosInfoDetailId { get; set; }

        public bool? IsVoided { get; set; }

        public bool? IsDeleted { get; set; }

        [StringLength(500)]
        public string DiscountRemark { get; set; }

        [StringLength(200)]
        public string PaymentsDesc { get; set; }

        public short IsPaid { get; set; }

        public decimal? PaidTotal { get; set; }

        [StringLength(200)]
        public string Rooms { get; set; }

        [StringLength(200)]
        public string OrderNo { get; set; }

        public bool IsInvoiced { get; set; }

        [StringLength(200)]
        public string Hashcode { get; set; }

        public decimal? TableSum { get; set; }

        public virtual ClientPos ClientPos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditTransaction> CreditTransactions { get; set; }

        public virtual EndOfDay EndOfDay { get; set; }

        public virtual Guest Guest { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice_Guests_Trans> Invoice_Guests_Trans { get; set; }

        public virtual InvoiceType InvoiceType { get; set; }

        public virtual PdaModule PdaModule { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual Table Table { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceShippingDetail> InvoiceShippingDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailInvoice> OrderDetailInvoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
