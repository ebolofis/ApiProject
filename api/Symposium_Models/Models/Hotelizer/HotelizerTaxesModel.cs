using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotelizer
{
    /// <summary>
    /// Responce from Hotelizer for Taxes
    /// </summary>
    public class HotelizerResponceTaxesModel
    {
        public bool success { get; set; }

        public List<HotelizerTaxesModel> data { get; set; }
    }

    /// <summary>
    /// Model for Hotelizer taxes
    /// </summary>
    public class HotelizerTaxesModel
    {
        /// <summary>
        /// Tax id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Tax description
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Tax percentance
        /// </summary>
        public decimal value { get; set; }
    }
}
