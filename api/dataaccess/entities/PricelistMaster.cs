namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PricelistMaster")]
    public partial class PricelistMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PricelistMaster()
        {
            Pricelists = new HashSet<Pricelist>();
            SalesType_PricelistMaster_Assoc = new HashSet<SalesType_PricelistMaster_Assoc>();
        }

        public long Id { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public byte? Status { get; set; }

        public bool? Active { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pricelist> Pricelists { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesType_PricelistMaster_Assoc> SalesType_PricelistMaster_Assoc { get; set; }
    }
}
