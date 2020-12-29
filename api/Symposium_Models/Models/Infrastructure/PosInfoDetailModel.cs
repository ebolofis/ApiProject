using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class PosInfoDetailModel
    {
        public long Id { get; set; }
        public Nullable<long> PosInfoId { get; set; }
        public Nullable<long> Counter { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public Nullable<bool> ResetsAfterEod { get; set; }
        public Nullable<short> InvoiceId { get; set; }
        public string ButtonDescription { get; set; }
        public Nullable<short> Status { get; set; }
        public Nullable<bool> CreateTransaction { get; set; }
        public Nullable<byte> FiscalType { get; set; }
        public Nullable<bool> IsInvoice { get; set; }
        public Nullable<bool> IsCancel { get; set; }
        public Nullable<int> GroupId { get; set; }
        public Nullable<long> InvoicesTypeId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsPdaHidden { get; set; }
        public Nullable<short> SendsVoidToKitchen { get; set; }
    }
}
