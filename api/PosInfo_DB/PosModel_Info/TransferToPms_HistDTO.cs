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
    [Table("TransferToPms_Hist")]
    [DisplayName("TransferToPms_Hist")]
    public class TransferToPms_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_TransferToPms_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("RegNo", Order = 3, TypeName = "NVARCHAR(150)")]
        public string RegNo { get; set; }

        [Column("PmsDepartmentId", Order = 4, TypeName = "NVARCHAR(100)")]
        public string PmsDepartmentId { get; set; }

        [Column("Description", Order = 5, TypeName = "NVARCHAR(500)")]
        public string Description { get; set; }

        [Column("ProfileId", Order = 6, TypeName = "NVARCHAR(150)")]
        public string ProfileId { get; set; }

        [Column("ProfileName", Order = 7, TypeName = "NVARCHAR(150)")]
        public string ProfileName { get; set; }

        [Column("TransactionId", Order = 8, TypeName = "BIGINT")]
        public Nullable<long> TransactionId { get; set; }

        [Column("TransferType", Order = 9, TypeName = "SMALLINT")]
        public Nullable<short> TransferType { get; set; }

        [Column("RoomId", Order = 10, TypeName = "NVARCHAR(50)")]
        public string RoomId { get; set; }

        [Column("RoomDescription", Order = 11, TypeName = "NVARCHAR(150)")]
        public string RoomDescription { get; set; }

        [Column("ReceiptNo", Order = 12, TypeName = "NVARCHAR(50)")]
        public string ReceiptNo { get; set; }

        [Column("PosInfoDetailId", Order = 13, TypeName = "BIGINT")]
        public Nullable<long> PosInfoDetailId { get; set; }

        [Column("SendToPMS", Order = 14, TypeName = "BIT")]
        public Nullable<bool> SendToPMS { get; set; }

        [Column("Total", Order = 15, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Total { get; set; }

        [Column("SendToPmsTS", Order = 16, TypeName = "DATETIME")]
        public Nullable<System.DateTime> SendToPmsTS { get; set; }

        [Column("ErrorMessage", Order = 17, TypeName = "NVARCHAR(1000)")]
        public string ErrorMessage { get; set; }

        [Column("PmsResponseId", Order = 18, TypeName = "NVARCHAR(150)")]
        public string PmsResponseId { get; set; }

        [Column("TransferIdentifier", Order = 19, TypeName = "UNIQUEIDENTIFIER")]
        public Nullable<System.Guid> TransferIdentifier { get; set; }

        [Column("PmsDepartmentDescription", Order = 20, TypeName = "NVARCHAR(300)")]
        public string PmsDepartmentDescription { get; set; }

        [Column("Status", Order = 21, TypeName = "SMALLINT")]
        public Nullable<short> Status { get; set; }

        [Column("PosInfoId", Order = 22, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("EndOfDayId", Order = 23, TypeName = "BIGINT")]
        public Nullable<long> EndOfDayId { get; set; }

        [Column("HotelId", Order = 24, TypeName = "BIGINT")]
        public Nullable<long> HotelId { get; set; }

        [Column("IsDeleted", Order = 25, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("Points", Order = 26, TypeName = "INT")]
        public int Points { get; set; }

        [Column("PMSPaymentId", Order = 27, TypeName = "BIGINT")]
        public Nullable<long> PMSPaymentId { get; set; }

        [Column("PMSInvoiceId", Order = 28, TypeName = "BIGINT")]
        public Nullable<long> PMSInvoiceId { get; set; }

        [Column("InvoiceId", Order = 29, TypeName = "BIGINT")]
        public Nullable<long> InvoiceId { get; set; }

        [Column("HtmlReceipt", Order = 30, TypeName = "IMAGE")]
        public byte[] HtmlReceipt { get; set; }
    }
}
