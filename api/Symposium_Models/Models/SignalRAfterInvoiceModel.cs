using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class SignalRAfterInvoiceModel
    {
        public long InvoiceId { get; set; }
        public short? SendsVoidToKitchen { get; set; }
        public short? InvoiceType { get; set; }
        public int? ExtType { get; set; }
        public string ExtecrName { get; set; }
        public PrintTypeEnum PrintType { get; set; }
        public string ItemAdditionalInfo { get; set; }
        public bool TempPrint { get; set; }
        public long PosInfoId { get; set; }
        public long? TableId { get; set; }
        public List<long> SalesTypes { get; set; }
        public string kdsMessage { get; set; }
        public string deliveryMessage { get; set; }
        public bool useFiscalSignature { get; set; }

    }
}
