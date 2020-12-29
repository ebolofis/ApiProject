namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderDetailIgredient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderDetailIgredient()
        {
            OrderDetailInvoices = new HashSet<OrderDetailInvoice>();
            OrderDetailIgredientVatAnals = new HashSet<OrderDetailIgredientVatAnal>();
        }

        public long Id { get; set; }

        public long? IngredientId { get; set; }

        public double? Qty { get; set; }

        public long? UnitId { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        public long? OrderDetailId { get; set; }

        public long? PriceListDetailId { get; set; }

        public decimal? Discount { get; set; }

        public decimal? TotalAfterDiscount { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual Ingredient Ingredient { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailInvoice> OrderDetailInvoices { get; set; }

        public virtual PricelistDetail PricelistDetail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailIgredientVatAnal> OrderDetailIgredientVatAnals { get; set; }
    }
}
