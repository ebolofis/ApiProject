using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class PricelistMasterModel
    {
        public Nullable<long> Id { get; set; }

        public string Description { get; set; }

        public Nullable<byte> Status { get; set; }

        public Nullable<bool> Active { get; set; }

        public Nullable<long> DAId { get; set; }
    }
}
