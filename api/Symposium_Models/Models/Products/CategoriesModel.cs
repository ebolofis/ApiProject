using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class CategoriesModel
    {
        /// <summary>
        /// Table Id
        /// </summary>
        public Nullable<long> Id { get; set; }

        /// <summary>
        /// Desctiprion
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public Nullable<byte> Status { get; set; }

        /// <summary>
        /// DA Server Id
        /// </summary>
        public Nullable<long> DAId { get; set; }
    }
}
