namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Transaction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Transaction()
        {
            CreditTransactions = new HashSet<CreditTransaction>();
            Invoice_Guests_Trans = new HashSet<Invoice_Guests_Trans>();
            OrderDetails = new HashSet<OrderDetail>();
            TransferToPms = new HashSet<TransferToPm>();
        }

        public long Id { get; set; }

        public DateTime? Day { get; set; }

        public long? PosInfoId { get; set; }

        public long? StaffId { get; set; }

        public long? OrderId { get; set; }

        public short? TransactionType { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }

        public long? DepartmentId { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public long? AccountId { get; set; }

        public short? InOut { get; set; }

        public decimal? Gross { get; set; }

        public decimal? Net { get; set; }

        public decimal? Vat { get; set; }

        public decimal? Tax { get; set; }

        public long? EndOfDayId { get; set; }

        [StringLength(500)]
        public string ExtDescription { get; set; }

        public long? InvoicesId { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual Account Account { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditTransaction> CreditTransactions { get; set; }

        public virtual Department Department { get; set; }

        public virtual EndOfDay EndOfDay { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice_Guests_Trans> Invoice_Guests_Trans { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual Order Order { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        public virtual Staff Staff { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransferToPm> TransferToPms { get; set; }
    }
}
