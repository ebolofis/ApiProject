using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Products
{
    /// <summary>
    /// Product Recipe Mode;
    /// </summary>
    public class ProductRecipeModel
    {
        public long Id { get; set; }

        public Nullable<long> ProductId { get; set; }

        public Nullable<long> UnitId { get; set; }

        public Nullable<double> Qty { get; set; }

        public Nullable<decimal> Price { get; set; }

        public Nullable<byte> IsProduct { get; set; }

        public Nullable<double> MinQty { get; set; }

        public Nullable<double> MaxQty { get; set; }

        public Nullable<long> IngredientId { get; set; }

        public Nullable<long> ItemsId { get; set; }

        public Nullable<long> ProductAsIngridientId { get; set; }

        public Nullable<double> DefaultQty { get; set; }

        public Nullable<int> Sort { get; set; }

        public Nullable<long> DAId { get; set; }

        public Nullable<bool> CanSavePrice { get; set; }
    }
}
