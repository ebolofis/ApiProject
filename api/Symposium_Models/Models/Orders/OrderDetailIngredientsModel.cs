using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class OrderDetailIngredientsModel
    {
        public Nullable<long> Id { get; set; }
        public Nullable<double> Qty { get; set; }

        public Nullable<long> UnitId { get; set; }

        public Nullable<decimal> Price { get; set; }

        public Nullable<long> OrderDetailId { get; set; }

        public Nullable<long> PriceListDetailId { get; set; }

        public Nullable<decimal> Discount { get; set; }

        public Nullable<decimal> TotalAfterDiscount { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> IngredientId { get; set; }
        public string IngredientDescr { get; set; }
        public Nullable<double> PendingQty { get; set; }
    }
}
