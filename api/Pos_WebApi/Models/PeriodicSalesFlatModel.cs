using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Models
{
    public class PeriodicSalesFlatModel
    {
        public Int64? PriceListId;
        public String PriceList;
        public Int64? ProductId;
        public String Product;
        public Decimal? ProductPrice;
        public Double? Qty;
        public Decimal? Gross;
        public String SalesTypeId;
        public Int32? ReceiptCount;
    }

}