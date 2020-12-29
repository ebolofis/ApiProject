using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class PageButtonPreviewModel
    {
        public List<PageButtonExtentModel> PageButtons { get; set; }
    }

    public class PageButtonExtentModel
    {
        public long Id { get; set; }
        public Nullable<long> PageSetId { get; set; }
        public Nullable<long> ProductId { get; set; }
        public string Description { get; set; }
        public string ExtraDescription { get; set; }
        public string SalesDescription { get; set; }
        public Nullable<short> PreparationTime { get; set; }
        public Nullable<short> Sort { get; set; }
        public Nullable<long> NavigateToPage { get; set; }
        public Nullable<long> SetDefaultPriceListId { get; set; }
        public Nullable<long> SetDefaultSalesType { get; set; }
        public Nullable<byte> Type { get; set; }
        public Nullable<long> PageId { get; set; }
        public string Color { get; set; }
        public string Background { get; set; }
        public Nullable<long> KdsId { get; set; }
        public string Code { get; set; }
        public Nullable<long> KitchenId { get; set; }
        public string KitchenCode { get; set; }
        public string ItemRegion { get; set; }
        public Nullable<int> RegionPosition { get; set; }
        public string ItemRegionAbbr { get; set; }
        public List<PriceListDetailsModel> PricelistDetails { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public List<PageButtonDetailsModel> PageButtonDetail { get; set; }
    }

    public class PriceListDetailsModel
    {
        public long Id { get; set; }
        public Nullable<long> PricelistId { get; set; }
        public Nullable<long> ProductId { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<long> VatId { get; set; }
        public VatDetailModel Vat { get; set; }
        public Nullable<long> TaxId { get; set; }

    }
    public class PageButtonDetailsModel
    {
        public Nullable<long> PageButtonId { get; set; }
        public Nullable<long> ProductId { get; set; }
        public string Description { get; set; }
        public string Background { get; set; }
        public string Color { get; set; }
        public Nullable<double> MaxQty { get; set; }
        public Nullable<double> MinQty { get; set; }
        public Nullable<byte> Type { get; set; }
        public Nullable<double> Qty { get; set; }
        public List<PriceListDetailModel> PricelistDetails { get; set; }
        public Nullable<short> Sort { get; set; }


    }

    public class VatDetailModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Percentage { get; set; }
        public Nullable<int> Code { get; set; }

    }

    public class PriceListDetailModel
    {
        public long Id { get; set; }
        public Nullable<long> PricelistId { get; set; }
        public Nullable<long> ProductId { get; set; }
        public Nullable<long> IngredientId { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<long> VatId { get; set; }
        public Nullable<long> TaxId { get; set; }
        public Nullable<decimal> PriceWithout { get; set; }
        public Nullable<int> MinRequiredExtras { get; set; }
        public VatDetailModel Vat { get; set; }
        public Nullable<long> DAId { get; set; }
    }


    /// <summary>
    /// Based on DTO
    /// </summary>
    public class PageButtonsModel
    {
        public Nullable<long> Id { get; set; }

        public string Description { get; set; }

        public Nullable<short> PreparationTime { get; set; }

        public string ImageUri { get; set; }

        public Nullable<decimal> Price { get; set; }

        public Nullable<long> PriceListId { get; set; }

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
    }
}
