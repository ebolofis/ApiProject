namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Kd
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kd()
        {
            PosInfoKdsAssocs = new HashSet<PosInfoKdsAssoc>();
            Products = new HashSet<Product>();
        }

        public long Id { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public byte? Status { get; set; }

        public long? PosInfoId { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfoKdsAssoc> PosInfoKdsAssocs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
