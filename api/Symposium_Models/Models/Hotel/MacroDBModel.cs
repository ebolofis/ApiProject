using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotel
{
    public class MacroDBModel
    {
        /// <summary>
        ///  Table Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// macro model in json format
        /// </summary>
        public string Model { get; set; }
    }
}
