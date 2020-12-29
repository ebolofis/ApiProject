namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tax")]
    public partial class Tax
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tax()
        {
            EndOfDayDetails = new HashSet<EndOfDayDetail>();
            OrderDetailIgredientVatAnals = new HashSet<OrderDetailIgredientVatAnal>();
            OrderDetailVatAnals = new HashSet<OrderDetailVatAnal>();
            PricelistDetails = new HashSet<PricelistDetail>();
        }

        public long Id { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public decimal? Percentage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EndOfDayDetail> EndOfDayDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailIgredientVatAnal> OrderDetailIgredientVatAnals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailVatAnal> OrderDetailVatAnals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PricelistDetail> PricelistDetails { get; set; }
    }
}
