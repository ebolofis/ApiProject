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
    [Table("Transactions_Hist")]
    [DisplayName("Transactions_Hist")]
    public class Transactions_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_Transactions_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("Day", Order = 2, TypeName = "DATETIME")]
        public Nullable<System.DateTime> Day { get; set; }

        [Column("PosInfoId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("StaffId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }

        [Column("OrderId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> OrderId { get; set; }

        [Column("TransactionType", Order = 6, TypeName = "SMALLINT")]
        public Nullable<short> TransactionType { get; set; }

        [Column("Amount", Order = 7, TypeName = "MONEY")]
        public Nullable<decimal> Amount { get; set; }

        [Column("DepartmentId", Order = 8, TypeName = "BIGINT")]
        public Nullable<long> DepartmentId { get; set; }

        [Column("Description", Order = 9, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("AccountId", Order = 10, TypeName = "BIGINT")]
        public Nullable<long> AccountId { get; set; }

        [Column("InOut", Order = 11, TypeName = "SMALLINT")]
        public Nullable<short> InOut { get; set; }

        [Column("Gross", Order = 12, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> Gross { get; set; }

        [Column("Net", Order = 13, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> Net { get; set; }

        [Column("Vat", Order = 14, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> Vat { get; set; }

        [Column("Tax", Order = 15, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> Tax { get; set; }

        [Column("EndOfDayId", Order = 16, TypeName = "BIGINT")]
        public Nullable<long> EndOfDayId { get; set; }

        [Column("ExtDescription", Order = 17, TypeName = "NVARCHAR(500)")]
        public string ExtDescription { get; set; }

        [Column("InvoicesId", Order = 18, TypeName = "BIGINT")]
        public Nullable<long> InvoicesId { get; set; }

        [Column("IsDeleted", Order = 19, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("SupplierId", Order = 20, TypeName = "BIGINT")]
        public Nullable<long> SupplierId { get; set; }
    }
}
