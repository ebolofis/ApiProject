using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Plugins.PmsInterface
{

    /// <summary>
    /// Pms Vat Model
    /// </summary>
    public class PmsVatModel
    {
        /// <summary>
        /// Vat Id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Vat description
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Vat percentance
        /// </summary>
        public decimal value { get; set; }
    }
}
