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
    [Table("TransferToPms")]
    [DisplayName("TransferToPms")]
    public class TransferToPmsDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TransferToPms")]
        public long Id { get; set; }

        [Column("RegNo", Order = 2, TypeName = "NVARCHAR(150)")]
        public string RegNo { get; set; }

        [Column("PmsDepartmentId", Order = 3, TypeName = "NVARCHAR(100)")]
        public string PmsDepartmentId { get; set; }

        [Column("Description", Order = 4, TypeName = "NVARCHAR(500)")]
        public string Description { get; set; }

        [Column("ProfileId", Order = 5, TypeName = "NVARCHAR(150)")]
        public string ProfileId { get; set; }

        [Column("ProfileName", Order = 6, TypeName = "NVARCHAR(150)")]
        public string ProfileName { get; set; }

        [Column("TransactionId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_TransferToPms_Transactions")]
        [Association("Transactions", "TransactionId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> TransactionId { get; set; }

        [Column("TransferType", Order = 8, TypeName = "SMALLINT")]
        public Nullable<short> TransferType { get; set; }

        [Column("RoomId", Order = 9, TypeName = "NVARCHAR(50)")]
        public string RoomId { get; set; }

        [Column("RoomDescription", Order = 10, TypeName = "NVARCHAR(150)")]
        public string RoomDescription { get; set; }

        [Column("ReceiptNo", Order = 11, TypeName = "NVARCHAR(50)")]
        public string ReceiptNo { get; set; }

        [Column("PosInfoDetailId", Order = 12, TypeName = "BIGINT")]
        public Nullable<long> PosInfoDetailId { get; set; }

        [Column("SendToPMS", Order = 13, TypeName = "BIT")]
        public Nullable<bool> SendToPMS { get; set; }

        [Column("Total", Order = 14, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Total { get; set; }

        [Column("SendToPmsTS", Order = 15, TypeName = "DATETIME")]
        public Nullable<System.DateTime> SendToPmsTS { get; set; }

        [Column("ErrorMessage", Order = 16, TypeName = "NVARCHAR(1000)")]
        public string ErrorMessage { get; set; }

        [Column("PmsResponseId", Order = 17, TypeName = "NVARCHAR(150)")]
        public string PmsResponseId { get; set; }

        [Column("TransferIdentifier", Order = 18, TypeName = "UNIQUEIDENTIFIER    ")]
        public Nullable<System.Guid> TransferIdentifier { get; set; }

        [Column("PmsDepartmentDescription", Order = 19, TypeName = "NVARCHAR(300)")]
        public string PmsDepartmentDescription { get; set; }

        [Column("Status", Order = 20, TypeName = "SMALLINT")]
        public Nullable<short> Status { get; set; }

        [Column("PosInfoId", Order = 21, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("EndOfDayId", Order = 22, TypeName = "BIGINT")]
        [ForeignKey("FK_dbo.TransferToPms_dbo.EndOfDay_EndOfDayId")]
        [Association("EndOfDay", "EndOfDayId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> EndOfDayId { get; set; }

        [Column("HotelId", Order = 23, TypeName = "BIGINT")]
        public Nullable<long> HotelId { get; set; }

        [Column("IsDeleted", Order = 24, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("Points", Order = 25, TypeName = "INT")]
        public int Points { get; set; }

        [Column("PMSPaymentId", Order = 26, TypeName = "BIGINT")]
        public Nullable<long> PMSPaymentId { get; set; }

        [Column("PMSInvoiceId", Order = 27, TypeName = "BIGINT")]
        public Nullable<long> PMSInvoiceId { get; set; }

        [Column("InvoiceId", Order = 28, TypeName = "BIGINT")]
        public Nullable<long> InvoiceId { get; set; }

        [Column("HtmlReceipt", Order = 29, TypeName = "IMAGE")]
        public byte[] HtmlReceipt { get; set; }

    }
}
