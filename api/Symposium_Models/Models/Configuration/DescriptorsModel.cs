using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Configuration
{
    public class DescriptorsModel
    {
        /// <summary>
        /// Name of Configuration Value
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Description of Configuration Value
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Typr of Value (string, boolean, datetime, integer, ...)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The Default Value when Configuration Load
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// The Api Version that We Create the value 
        /// </summary>
        public string ApiVersion { get; set; }
    }
}
