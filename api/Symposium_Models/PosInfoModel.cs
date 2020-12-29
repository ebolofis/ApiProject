using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models
{
    public class PosInfoModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> FODay { get; set; }
        public Nullable<long> CloseId { get; set; }
        public string IPAddress { get; set; }
        public Nullable<byte> Type { get; set; }
        public string wsIP { get; set; }
        public string wsPort { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public string FiscalName { get; set; }
        public Nullable<byte> FiscalType { get; set; }
        public Nullable<bool> IsOpen { get; set; }
        public Nullable<long> ReceiptCount { get; set; }
        public Nullable<bool> ResetsReceiptCounter { get; set; }
        public string Theme { get; set; }
        public Nullable<long> AccountId { get; set; }
        public Nullable<bool> LogInToOrder { get; set; }
        public Nullable<bool> ClearTableManually { get; set; }
        public Nullable<bool> ViewOnly { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> InvoiceSumType { get; set; }
        public Nullable<short> LoginToOrderMode { get; set; }
        public Nullable<short> KeyboardType { get; set; }
        public string CustomerDisplayGif { get; set; }
        public Nullable<int> DefaultHotelId { get; set; }
        public string NfcDevice { get; set; }
        public string Configuration { get; set; }
    }
}
