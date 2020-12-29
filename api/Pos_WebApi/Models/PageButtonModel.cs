using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi
{
    public partial class PageButton
    {
        public long? VatId { get; set; }
        public double? Vat { get; set; }
        public int? VatCode { get; set; }
        public long? KdsId { get; set; }
        public long? KitchenId { get; set; }
        public string KitchenCode { get; set; }
        public Nullable<long> PriceListDetailId { get; set; }
        public IEnumerable<PricelistDetail> PricelistDetails { get; set; }
    }

    public partial class PageButtonDetail
    {
        public Nullable<long> PriceListDetailId { get; set; }
        public IEnumerable<PricelistDetail> PricelistDetails { get; set; }
        public string Background { get; set; }
        public string Color { get; set; }
    }


    //public partial class OrderDetailIgredients
    //{
    //    public Nullable<long> PriceListDetailId { get; set; }
    //}
}