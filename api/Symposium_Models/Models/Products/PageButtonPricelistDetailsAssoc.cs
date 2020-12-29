using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Products
{
    public class PageButtonPricelistDetailsAssoc
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public Nullable<short> PreparationTime { get; set; }
        public string ImageUri { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<long> PriceListId { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public Nullable<short> Sort { get; set; }
        public Nullable<long> NavigateToPage { get; set; }
        public Nullable<long> SetDefaultPriceListId { get; set; }
        public Nullable<byte> Type { get; set; }
        public Nullable<long> PageId { get; set; }
        public string Color { get; set; }
        public string Background { get; set; }
        public Nullable<long> ProductId { get; set; }
        public Nullable<long> SetDefaultSalesType { get; set; }
        public Nullable<bool> CanSavePrice { get; set; }
        public Nullable<long> DAId { get; set; }
        public List<PricelistDetailNewModel> PricelistDetails { get; set; }
    }

    public class PricelistDetailNewModel
    {
        public long Id { get; set; }
        public Nullable<long> PricelistId { get; set; }
        public Nullable<long> ProductId { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<long> VatId { get; set; }
        public Nullable<long> TaxId { get; set; }
        public Nullable<long> IngredientId { get; set; }
        public Nullable<decimal> PriceWithout { get; set; }
        public Nullable<int> MinRequiredExtras { get; set; }
        public Nullable<long> DAId { get; set; }
        public VatModel Vat { get; set; }
    }
}
