using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class PriceList_EffectiveHoursModel
    {
        public Nullable<long> Id { get; set; }

        public Nullable<long> PricelistId { get; set; }

        public Nullable<System.DateTime> FromTime { get; set; }

        public Nullable<System.DateTime> ToTime { get; set; }

        public Nullable<long> DAId { get; set; }
    }
}
