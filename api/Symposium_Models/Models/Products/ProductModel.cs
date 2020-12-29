using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class ProductModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string ExtraDescription { get; set; }
        public Nullable<double> Qty { get; set; }
        public Nullable<long> UnitId { get; set; }
        public string SalesDescription { get; set; }
        public Nullable<int> PreparationTime { get; set; }
        public Nullable<long> KdsId { get; set; }
        public Nullable<long> KitchenId { get; set; }
        public string ImageUri { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public string Code { get; set; }
        public Nullable<bool> IsCustom { get; set; }
        public Nullable<long> KitchenRegionId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsCombo { get; set; }
        public Nullable<bool> IsComboItem { get; set; }
        public Nullable<bool> IsReturnItem { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<long> DAId { get; set; }
    }

    /// <summary>
    /// ProductModel with the list of Extras
    /// </summary>
    public class ProductExtModel : ProductModel
    {
        /// <summary>
        /// list of ProductExtras/Ingredients (ProductExtras join with Ingredients  union  ProductRecipe join with Ingredients)
        /// </summary>
        public List<ProductExtrasIngredientsModel> Extras { get; set; } = new List<ProductExtrasIngredientsModel>();
    }

    public class ProductDescription
    {
        public long Id { get; set; }

        public string Descr { get; set; }
    }

    public class ProductIdsList
    {
        public List<long> productIdList { get; set; }
    }

}
