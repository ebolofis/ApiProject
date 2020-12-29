using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class ProductPricesModel
    {
        public ProductPricesModel()
        {
            ProductPricesModelDetails = new HashSet<ProductPricesModelDetails>();
        }

        public long? ProductId { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public long? ProductCategoryId { get; set; }

        public ICollection<ProductPricesModelDetails> ProductPricesModelDetails { get; set; }

    }

    public class ProductPricesModelDetails
    {
        public decimal? Price { get; set; }
        public decimal? PriceWithout { get; set; }
        public long? PriceListId { get; set; }

        public long? PricelistMasterId { get; set; }
        public long? PricelistDetailId { get; set; }
        public long? VatId { get; set; }
        public long? TaxId { get; set; }
        public long? LookUpPriceListId { get; set; }
        public double? Percentage { get; set; }


    }
}
