using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Symposium.Models.Models.Hotel
{
    public class MacroTimezoneDBModel
    {
        /// <summary>
        ///  Table Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Timezone model in json format
        /// </summary>
        public string Model { get; set; }
    }
}
