using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Plugins.PmsInterface
{
    /// <summary>
    /// Pms Method of Payment model
    /// </summary>
    public class PmsMethodOfPayments
    {
        /// <summary>
        /// Payment Id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Payment Description
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Payment type (cash, Credit card, cashless ...)
        /// </summary>
        public string payment_type { get; set; }

        /// <summary>
        /// Credit card type (visa, master...)
        /// </summary>
        public string credit_card_type { get; set; }

        /// <summary>
        /// Type of Method
        /// deferent for protel and ermis and hotelizer pms
        /// </summary>
        public int typ { get; set; }
    }
}
