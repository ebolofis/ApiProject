namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PdaModule")]
    public partial class PdaModule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PdaModule()
        {
            Invoices = new HashSet<Invoice>();
            Orders = new HashSet<Order>();
            PdaModuleDetails = new HashSet<PdaModuleDetail>();
        }

        public long Id { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public byte? Status { get; set; }

        public int? MaxActiveUsers { get; set; }

        public long? PosInfoId { get; set; }

        public long? PageSetId { get; set; }

        [StringLength(20)]
        public string Code { get; set; }

        public bool? IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }

        public virtual PageSet PageSet { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PdaModuleDetail> PdaModuleDetails { get; set; }
    }
}
