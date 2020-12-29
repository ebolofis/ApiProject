using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [Table("Invoices_Hist")]
    [DisplayName("Invoices_Hist")]
    public class Invoices_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_Invoices_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("InvoiceTypeId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> InvoiceTypeId { get; set; }

        [Column("Counter", Order = 4, TypeName = "INT")]
        public Nullable<int> Counter { get; set; }

        [Column("Day", Order = 5, TypeName = "DATETIME")]
        public Nullable<System.DateTime> Day { get; set; }

        [Column("IsPrinted", Order = 6, TypeName = "BIT")]
        public Nullable<bool> IsPrinted { get; set; }

        [Column("GuestId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> GuestId { get; set; }

        [Column("Total", Order = 8, TypeName = "MONEY")]
        public Nullable<decimal> Total { get; set; }

        [Column("Discount", Order = 9, TypeName = "MONEY")]
        public Nullable<decimal> Discount { get; set; }

        [Column("Vat", Order = 10, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> Vat { get; set; }

        [Column("Tax", Order = 11, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> Tax { get; set; }

        [Column("Net", Order = 12, TypeName = "MONEY")]
        public Nullable<decimal> Net { get; set; }

        [Column("StaffId", Order = 13, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }

        [Column("Cover", Order = 14, TypeName = "INT")]
        public Nullable<int> Cover { get; set; }

        [Column("TableId", Order = 15, TypeName = "BIGINT")]
        public Nullable<long> TableId { get; set; }

        [Column("PosInfoId", Order = 16, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("PdaModuleId", Order = 17, TypeName = "BIGINT")]
        public Nullable<long> PdaModuleId { get; set; }

        [Column("ClientPosId", Order = 18, TypeName = "BIGINT")]
        public Nullable<long> ClientPosId { get; set; }

        [Column("EndOfDayId", Order = 19, TypeName = "BIGINT")]
        public Nullable<long> EndOfDayId { get; set; }

        [Column("PosInfoDetailId", Order = 20, TypeName = "BIGINT")]
        public Nullable<long> PosInfoDetailId { get; set; }

        [Column("IsVoided", Order = 21, TypeName = "BIT")]
        public Nullable<bool> IsVoided { get; set; }

        [Column("IsDeleted", Order = 22, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("DiscountRemark", Order = 23, TypeName = "NVARCHAR(500)")]
        public string DiscountRemark { get; set; }

        [Column("PaymentsDesc", Order = 24, TypeName = "NVARCHAR(200)")]
        public string PaymentsDesc { get; set; }

        [Column("IsPaid", Order = 25, TypeName = "SMALLINT")]
        [Required]
        public short IsPaid { get; set; }

        [Column("PaidTotal", Order = 26, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> PaidTotal { get; set; }

        [Column("Rooms", Order = 27, TypeName = "NVARCHAR(200)")]
        public string Rooms { get; set; }

        [Column("OrderNo", Order = 28, TypeName = "NVARCHAR(200)")]
        public string OrderNo { get; set; }

        [Column("IsInvoiced", Order = 29, TypeName = "BIT")]
        [Required]
        public bool IsInvoiced { get; set; }

        [Column("Hashcode", Order = 30, TypeName = "NVARCHAR(200)")]
        public string Hashcode { get; set; }

        [Column("TableSum", Order = 31, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> TableSum { get; set; }

        [Column("CashAmount", Order = 32, TypeName = "NVARCHAR(20)")]
        public string CashAmount { get; set; }

        [Column("BuzzerNumber", Order = 33, TypeName = "NVARCHAR(20)")]
        public string BuzzerNumber { get; set; }

        [Column("LoyaltyDiscount", Order = 34, TypeName = "MONEY")]
        public Nullable<decimal> LoyaltyDiscount { get; set; }

        [Column("ForeignExchangeCurrency", Order = 35, TypeName = "NVARCHAR(20)")]
        public string ForeignExchangeCurrency { get; set; }

        [Column("ForeignExchangeTotal", Order = 36, TypeName = "MONEY")]
        public Nullable<decimal> ForeignExchangeTotal { get; set; }

        [Column("ForeignExchangeDiscount", Order = 37, TypeName = "MONEY")]
        public Nullable<decimal> ForeignExchangeDiscount { get; set; }

        [Column("ExtECRCode", Order = 38, TypeName = "NVARCHAR(500)")]
        public string ExtECRCode { get; set; }

        [Column("CustomersId", Order = 39, TypeName = "NVARCHAR(200)")]
        public string CustomersId { get; set; }

        [Column("ReservationsId", Order = 40, TypeName = "NVARCHAR(200)")]
        public string ReservationIds { get; set; }
    }
}
