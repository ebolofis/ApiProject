using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotelizer
{
    public class HotelizerPostOrderModel
    {
        public int accommodation_id { get; set; }

        public DateTime due_date { get; set; }

        public string order_no { get; set; }

        public string guid { get; set; }

        public List<HotelizerPostOrderProductsModel> products { get; set; }

    }

    public class HotelizerPostOrderProductsModel
    {
        public string name { get; set; }

        public string description { get; set; }

        public string external_code { get; set; }

        public int service_id { get; set; }

        public decimal gross_value { get; set; }

        public decimal price { get; set; }

        public decimal quantity { get; set; }

        public string currency { get; set; }

        public int[] taxes { get; set; }
    }

    public class HotelizerPostOrderPaymentsModel
    {
        public int payment_option_id { get; set; }

        public DateTime due_date { get; set; }

        public decimal amount { get; set; }
    }

    public class HotelizerCancelReceiptModel
    {
        public string token { get; set; }

        public string external_id { get; set; }

        public int order_id { get; set; }
    }

}
