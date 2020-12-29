using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi
{
    public class ProductMappingClass
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
    }

    public class ProductGroupedByCategories
    {
        public string Category { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }


    public partial class OrderDetail
    {
        public Vat Vat { get; set; }
        public Tax Tax { get; set; }
        public int? AA { get; set; }
    }

}