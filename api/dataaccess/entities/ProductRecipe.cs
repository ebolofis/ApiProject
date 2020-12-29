namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductRecipe")]
    public partial class ProductRecipe
    {
        public long Id { get; set; }

        public long? ProductId { get; set; }

        public long? IngredientId { get; set; }

        public long? UnitId { get; set; }

        public double? Qty { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        public byte? IsProduct { get; set; }

        public double? MinQty { get; set; }

        public double? MaxQty { get; set; }

        public long? ItemsId { get; set; }

        public long? ProductAsIngridientId { get; set; }

        public double? DefaultQty { get; set; }

        public int? Sort { get; set; }

        public virtual Ingredient Ingredient { get; set; }

        public virtual Item Item { get; set; }

        public virtual Product Product { get; set; }

        public virtual Unit Unit { get; set; }
    }
}
