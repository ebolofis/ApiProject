namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PricelistDetail")]
    public partial class PricelistDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PricelistDetail()
        {
            OrderDetails = new HashSet<OrderDetail>();
            OrderDetailIgredients = new HashSet<OrderDetailIgredient>();
        }

        public long Id { get; set; }

        public long? PricelistId { get; set; }

        public long? ProductId { get; set; }

        public long? IngredientId { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        public long? VatId { get; set; }

        public long? TaxId { get; set; }

        [Column(TypeName = "money")]
        public decimal? PriceWithout { get; set; }

        public int? MinRequiredExtras { get; set; }

        public virtual Ingredient Ingredient { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailIgredient> OrderDetailIgredients { get; set; }

        public virtual Pricelist Pricelist { get; set; }

        public virtual Product Product { get; set; }

        public virtual Tax Tax { get; set; }

        public virtual Vat Vat { get; set; }
    }
}
