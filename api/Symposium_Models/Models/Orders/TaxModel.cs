using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class TaxModel
    {
        /// <summary>
        /// Tble ID
        /// </summary>
        public Nullable<long> Id { get; set; }

        /// <summary>
        /// Tax Dexcription
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Tax percent
        /// </summary>
        public Nullable<decimal> Percentage { get; set; }

        /// <summary>
        /// Id from DA Server
        /// </summary>
        public Nullable<long> DAId { get; set; }
    }
}
