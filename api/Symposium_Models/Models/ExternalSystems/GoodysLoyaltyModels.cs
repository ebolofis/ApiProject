using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.ExternalSystems
{
    public partial class GoodysLoyaltyJSON
    {
        [JsonIgnore]
        public long InvoiceId { get; set; }
        [JsonIgnore]
        public int PosId { get; set; }
        public string externalOrderId { get; set; }
        public string msisdn { get; set; }
        public string channel { get; set; }
        public string store { get; set; }
        [JsonIgnore]
        public string campaign { get; set; }
        [JsonIgnore]
        public string couponType { get; set; }
        public string couponCode { get; set; }
        [JsonIgnore]
        public List<string> giftCardCampaign { get; set; }
        [JsonIgnore]
        public List<string> giftCardTypes { get; set; }
        public List<string> giftCardCodes { get; set; }
        public string basketValue { get; set; }
        public GoodysOrderLine[] orderLines { get; set; }
    }

    public partial class GoodysJSON
    {
        public string channel { get; set; }
        public string store { get; set; }
        public string couponCode { get; set; }
        public GoodysOrderLine[] orderLines { get; set; }
    }

    public class GoodysOrderLine
    {
        public string orderLineId { get; set; }
        public string productId { get; set; }
        public Int64 quantity { get; set; }
        public Double cost { get; set; }
    }

    public class GoodysCouponDetailInfo
    {
        public long daStoreId { get; set; }
        public string couponCode { get; set; }
        public List<GoodysCouponDetailItems> orderItems { get; set; }
    }

    public class GoodysCouponDetailItems
    {
        public string productCode { get; set; }
        public int quantity { get; set; }
        public decimal cost { get; set; }
    }

}
