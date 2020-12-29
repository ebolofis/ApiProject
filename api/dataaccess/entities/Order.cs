namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            OrdersStaffs = new HashSet<OrdersStaff>();
            OrderStatus = new HashSet<OrderStatu>();
            Transactions = new HashSet<Transaction>();
        }

        public long Id { get; set; }

        public long? OrderNo { get; set; }

        public DateTime? Day { get; set; }

        [Column(TypeName = "money")]
        public decimal? Total { get; set; }

        public long? PosId { get; set; }

        public long? StaffId { get; set; }

        public long? EndOfDayId { get; set; }

        public decimal? Discount { get; set; }

        public int? ReceiptNo { get; set; }

        public decimal? TotalBeforeDiscount { get; set; }

        public long? PdaModuleId { get; set; }

        public long? ClientPosId { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual ClientPos ClientPos { get; set; }

        public virtual EndOfDay EndOfDay { get; set; }

        public virtual PdaModule PdaModule { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        public virtual Staff Staff { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdersStaff> OrdersStaffs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderStatu> OrderStatus { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
