namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalesType")]
    public partial class SalesType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalesType()
        {
            OrderDetails = new HashSet<OrderDetail>();
            SalesType_PricelistMaster_Assoc = new HashSet<SalesType_PricelistMaster_Assoc>();
            TransferMappings = new HashSet<TransferMapping>();
        }

        public long Id { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        [StringLength(10)]
        public string Abbreviation { get; set; }

        public bool? IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesType_PricelistMaster_Assoc> SalesType_PricelistMaster_Assoc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransferMapping> TransferMappings { get; set; }
    }
}
