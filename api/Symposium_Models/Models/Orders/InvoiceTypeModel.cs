using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class InvoiceTypeModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public Nullable<short> Type { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
