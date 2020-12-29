using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class Ingredient_ProdCategoryAssocModel
    {
        public Nullable<long> Id { get; set; }

        public Nullable<long> IngredientId { get; set; }

        public Nullable<long> ProductCategoryId { get; set; }

        public Nullable<long> Sort { get; set; }

        public Nullable<long> DAId { get; set; }
    }
}
