using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotelizer
{
    /// <summary>
    /// Responce for hotelizer for Method of paymens
    /// </summary>
    public class HotelizerResponceMethodOfPaymentModel
    {
        public bool success { get; set; }

        public List<HotelizerMethodOfPaymentModel> data { get; set; }
    }

    /// <summary>
    /// Model for method of paymens for hotelizer
    /// </summary>
    public class HotelizerMethodOfPaymentModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Payment type (cash, credid card ....)
        /// </summary>
        public string payment_type { get; set; }

        /// <summary>
        /// Card name in case payment_type is credit card (amex, mastercard ...)
        /// </summary>
        public string credit_card_type { get; set; }
    }
}
