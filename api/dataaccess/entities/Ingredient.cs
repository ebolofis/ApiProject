namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Ingredient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ingredient()
        {
            OrderDetailIgredients = new HashSet<OrderDetailIgredient>();
            PricelistDetails = new HashSet<PricelistDetail>();
            ProductExtras = new HashSet<ProductExtra>();
            ProductRecipes = new HashSet<ProductRecipe>();
        }

        public long Id { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        [StringLength(500)]
        public string ExtendedDescription { get; set; }

        [StringLength(500)]
        public string SalesDescription { get; set; }

        public double? Qty { get; set; }

        public long? ItemId { get; set; }

        public long? UnitId { get; set; }

        [StringLength(150)]
        public string Code { get; set; }

        public bool? IsDeleted { get; set; }

        [StringLength(25)]
        public string Background { get; set; }

        [StringLength(25)]
        public string Color { get; set; }

        public virtual Item Item { get; set; }

        public virtual Unit Unit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailIgredient> OrderDetailIgredients { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PricelistDetail> PricelistDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductExtra> ProductExtras { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductRecipe> ProductRecipes { get; set; }
    }
}
