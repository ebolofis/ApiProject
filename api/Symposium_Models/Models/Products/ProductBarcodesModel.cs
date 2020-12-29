using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Products
{
    public class ProductBarcodesModel
    {
        public long Id { get; set; }

        public string Barcode { get; set; }

        public Nullable<long> ProductId { get; set; }

        public Nullable<byte> Type { get; set; }

        public Nullable<long> DAId { get; set; }
    }
}
