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
    [Table("CreditTransactions_Hist")]
    [System.ComponentModel.DisplayName("CreditTransactions_Hist")]
    public class CreditTransactions_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_CreditTransactions_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("CreditAccountId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> CreditAccountId { get; set; }

        [Column("CreditCodeId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> CreditCodeId { get; set; }

        [Column("Amount", Order = 5, TypeName = "DECIMAL(18,4)")]
        public Nullable<decimal> Amount { get; set; }

        [Column("CreationTS", Order = 6, TypeName = "DATETIME")]
        public Nullable<System.DateTime> CreationTS { get; set; }

        [Column("Type", Order = 7, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("StaffId", Order = 8, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }

        [Column("Description", Order = 9, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("PosInfoId", Order = 10, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("EndOfDayId", Order = 11, TypeName = "BIGINT")]
        public Nullable<long> EndOfDayId { get; set; }

        [Column("InvoiceId", Order = 12, TypeName = "BIGINT")]
        public Nullable<long> InvoiceId { get; set; }

        [Column("TransactionId", Order = 13, TypeName = "BIGINT")]
        public Nullable<long> TransactionId { get; set; }
    }
}
