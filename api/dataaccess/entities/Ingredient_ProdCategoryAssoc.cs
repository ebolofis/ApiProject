namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Ingredient_ProdCategoryAssoc
    {
        public long Id { get; set; }

        public long? IngredientId { get; set; }

        public long? ProductCategoryId { get; set; }
    }
}
