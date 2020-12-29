namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Page
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Page()
        {
            PageButtons = new HashSet<PageButton>();
        }

        public long Id { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(500)]
        public string ExtendedDesc { get; set; }

        public byte? Status { get; set; }

        public short? Sort { get; set; }

        public long? DefaultPriceList { get; set; }

        public long? PageSetId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PageButton> PageButtons { get; set; }

        public virtual PageSet PageSet { get; set; }
    }
}
