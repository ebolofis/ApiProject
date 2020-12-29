namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Table")]
    public partial class Table
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Table()
        {
            Invoices = new HashSet<Invoice>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public long Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(350)]
        public string SalesDescription { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public int? MinCapacity { get; set; }

        public int? MaxCapacity { get; set; }

        public long? RegionId { get; set; }

        public byte? Status { get; set; }

        [StringLength(100)]
        public string YPos { get; set; }

        [StringLength(100)]
        public string XPos { get; set; }

        public bool? IsOnline { get; set; }

        public short? ReservationStatus { get; set; }

        public long? Shape { get; set; }

        public int? TurnoverTime { get; set; }

        [StringLength(500)]
        public string ImageUri { get; set; }

        public double? Width { get; set; }

        public double? Height { get; set; }

        [StringLength(100)]
        public string Angle { get; set; }

        public bool? IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual Region Region { get; set; }
    }
}
