using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class PageButtonDetailModel
    {
        public Nullable<long> Id { get; set; }

        public Nullable<long> ProductId { get; set; }

        public Nullable<bool> IsRequired { get; set; }

        public Nullable<double> MinQty { get; set; }

        public Nullable<double> MaxQty { get; set; }

        public string Description { get; set; }

        public Nullable<decimal> AddCost { get; set; }

        public Nullable<decimal> RemoveCost { get; set; }

        public Nullable<byte> Type { get; set; }

        public Nullable<short> Sort { get; set; }

        public Nullable<long> PageButtonId { get; set; }

        public Nullable<double> Qty { get; set; }

        public Nullable<long> DAId { get; set; }

    }
}
