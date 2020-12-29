using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class PricelistDetailModel: PricelistDetailBasicModel
    {

        public double? PriceWithout { get; set; }
        public int? MinRequiredExtras { get; set; }
        public Nullable<long> DAId { get; set; }
    }

    public class PricelistDetailBasicModel
    {
        public long Id { get; set; }
        public long? PricelistId { get; set; }
        public long? ProductId { get; set; }
        public long? IngredientId { get; set; }
        public double? Price { get; set; }
        public long? VatId { get; set; }
        public long? TaxId { get; set; }
       

    }
}
