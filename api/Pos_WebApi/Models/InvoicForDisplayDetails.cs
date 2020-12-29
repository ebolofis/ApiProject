using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos_WebApi.Models
{
    public partial class InvoiceForDisplayDetailsTemp
    {
        public Guid Id { get; set; }
        public Int64 InvoiceForDetailId { get; set; }
        public Int64? OrderDetailId { get; set; }
        public Int64? OrderDetailIdIngredientId { get; set; }
        public Int64? OrderDetailInvoicesId { get; set; }
        public String Description { get; set; }
        public Decimal? Price { get; set; }
        public Double? Qty { get; set; }
        public Decimal? Discount { get; set; }
        public Decimal? TotalAfterDiscount { get; set; }
        public Int64? VatId { get; set; }
        public Int64? VatCode { get; set; }
        public Decimal? VatRate { get; set; }
        public Decimal? VatAmount { get; set; }
        public Int64? TaxId { get; set; }
        public Decimal? TaxAmount { get; set; }
        public Decimal? TaxRate { get; set; }
        public Int64? ProductId { get; set; }
        public bool IsExtra { get; set; }
        public byte? PaidStatus { get; set; }
        public Int32? Status { get; set; }
        public Int64? TableId { get; set; }
        public Int64? KitchenId { get; set; }
        public Int64? KdsId { get; set; }


    }
}
