using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Kds
{
    public class KdsIngredientsResponceModel
    {
        public Nullable<long> IngredientId { get; set; }
        public string IngredientDescr { get; set; }
        public Nullable<double> IngredientPendingQty { get; set; }
    }
}
