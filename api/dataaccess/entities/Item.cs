namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item()
        {
            Ingredients = new HashSet<Ingredient>();
            ProductExtras = new HashSet<ProductExtra>();
            ProductRecipes = new HashSet<ProductRecipe>();
        }

        public long Id { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        [StringLength(500)]
        public string ExtendedDescription { get; set; }

        public double? Qty { get; set; }

        public long? UnitId { get; set; }

        public long? VatId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ingredient> Ingredients { get; set; }

        public virtual Unit Unit { get; set; }

        public virtual Vat Vat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductExtra> ProductExtras { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductRecipe> ProductRecipes { get; set; }
    }
}
