using System;
using System.Collections.Generic;

namespace Pos_WebApi.Models
{
    public partial class InvoiceForDisplay
    {
        public long Id { get; set; }
        public DateTime? Day { get; set; }
        public string Description { get; set; }
        public long Counter { get; set; }
        public long? TableId { get; set; }
        public string Rooms { get; set; }
        public short IsPaid
        {
            get
            {
                return (short)(this.Total > this.PaidAmount ? 0
                : this.Total == this.PaidAmount ? 1 : 2)
                ;
            }
        }
        public int Points { get; set; }
        public Decimal? LoyaltyDiscount { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVoided { get; set; }
        public bool IsPrinted { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal PaidAmount { get; set; }
        public long Cover { get; set; }
        public long? InvoiceTypeId { get; set; }
        public short InvoiceType { get; set; }
        public string AccountDescriptions { get; set; }
        public string TableCode { get; set; }
        public long? StaffId { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public string AFM { get; set; }
        public string DOY { get; set; }
        public IEnumerable<long> OrderIds { get; set; }
        public IEnumerable<long> OrderΝοs { get; set; }
       
    }
}