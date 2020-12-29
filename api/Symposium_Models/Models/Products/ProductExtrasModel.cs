using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    /// <summary>
    /// ProductExtras (based on DB)
    /// </summary>
    public class ProductExtrasModel
    {
        public Nullable<long> Id { get; set; }

        public Nullable<long> ProductId { get; set; }

        public Nullable<bool> IsRequired { get; set; }

        public Nullable<long> IngredientId { get; set; }

        public Nullable<double> MinQty { get; set; }

        public Nullable<double> MaxQty { get; set; }

        public Nullable<long> UnitId { get; set; }

        public Nullable<long> ItemsId { get; set; }

        public Nullable<decimal> Price { get; set; }

        public Nullable<long> ProductAsIngridientId { get; set; }

        public Nullable<int> Sort { get; set; }

        public Nullable<long> DAId { get; set; }

        public Nullable<bool> CanSavePrice { get; set; }

    }

    /// <summary>
    /// ProductExtras/Ingredients (ProductExtras join with Ingredients  and  ProductRecipe join with Ingredients)
    /// </summary>
    public class ProductExtrasIngredientsModel : ProductExtrasModel
    {
        //  public long Id { get; set; }  <-- from ProductExtrasModel
        //   public long ProductId { get; set; }<-- from ProductExtrasModel
        //  public long MinQty { get; set; }<-- from ProductExtrasModel
        //  public long MaxQty { get; set; }<-- from ProductExtrasModel
        //  public long UnitId { get; set; }<-- from ProductExtrasModel
        public string Description { get; set; }
        public string ExtendedDescription { get; set; }
        public string SalesDescription { get; set; }

        public string Code { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        /// <summary>
        /// true if extra is part of recipe
        /// </summary>
        public bool isRecipe { get; set; }

    }


    
}
