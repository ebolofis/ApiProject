using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Orders
{
    public class LoyaltyModel 
    {
        /// <summary>
        /// Unique Key 
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Loyalty post date
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Loyalty Id
        /// </summary>
        public string LoyalltyId { get; set; }

        /// <summary>
        /// coupon code
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// Gift Card Code
        /// </summary>
        public string GiftcardCode { get; set; }

        /// <summary>
        /// Coupon Type
        /// </summary>
        public string CouponType { get; set; }

        /// <summary>
        /// campaign
        /// </summary>
        public string Campaign { get; set; }

        /// <summary>
        /// Channel came from
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Invoice belongs to
        /// </summary>
        public Nullable<long> InvoicesId { get; set; }

        /// <summary>
        /// Errors
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gift card type
        /// </summary>
        public string GiftCardCouponType { get; set; }

        /// <summary>
        /// Gift card campaign
        /// </summary>
        public string GiftCardCampaign { get; set; }

        /// <summary>
        /// da order belongs to
        /// </summary>
        public Nullable<long> DAOrderId { get; set; }
    }
}
