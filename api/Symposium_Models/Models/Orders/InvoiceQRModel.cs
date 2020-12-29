using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Orders
{
    public class InvoiceQRModel
    {
        // autoincrement id
        public long id { get; set; }
        // related invoice id
        public long InvoiceId { get; set; }
        // qr code image byte array
        public byte[] QR { get; set; }
    }

    public class InvoiceQRPostModel
    {
        public long invoiceId { get; set; }
        public string url { get; set; }
    }
}
