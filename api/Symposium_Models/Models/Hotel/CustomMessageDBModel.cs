using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotel
{
    public class CustomMessageDBModel
    {
        /// <summary>
        ///  Table Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// custom messages model in json format
        /// </summary>
        public string Model { get; set; }
    }
}
