using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class SalesTypeModel
    {
        public Nullable<long> Id { get; set; }

        public string Description { get; set; }

        public string Abbreviation { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<long> DAId { get; set; }
    }
}
