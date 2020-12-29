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
    [Table("CreditTransactions")]
    [DisplayName("CreditTransactions")]
    public class CreditTransactionsDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_CreditTransactions")]
        public long Id { get; set; }

        [Column("CreditAccountId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_CreditTransactions_CreditAccounts")]
        [Association("CreditAccounts", "CreditAccountId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> CreditAccountId { get; set; }

        [Column("CreditCodeId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_CreditTransactions_CreditCodes")]
        [Association("CreditCodes", "CreditCodeId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> CreditCodeId { get; set; }

        [Column("Amount", Order = 4, TypeName = "DECIMAL(18,4)")]
        public Nullable<decimal> Amount { get; set; }

        [Column("CreationTS", Order = 5, TypeName = "DATETIME")]
        public Nullable<System.DateTime> CreationTS { get; set; }

        [Column("Type", Order = 6, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("StaffId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_CreditTransactions_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffId { get; set; }

        [Column("Description", Order = 8, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("PosInfoId", Order = 9, TypeName = "BIGINT")]
        [ForeignKey("FK_CreditTransactions_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PosInfoId { get; set; }

        [Column("EndOfDayId", Order = 10, TypeName = "BIGINT")]
        [ForeignKey("FK_CreditTransactions_EndOfDay")]
        [Association("EndOfDay", "EndOfDayId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> EndOfDayId { get; set; }

        [Column("InvoiceId", Order = 11, TypeName = "BIGINT")]
        [ForeignKey("FK_CreditTransactions_Invoices")]
        [Association("Invoices", "InvoiceId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> InvoiceId { get; set; }

        [Column("TransactionId", Order = 12, TypeName = "BIGINT")]
        [ForeignKey("FK_dbo.CreditTransactions_dbo.Transactions_TransactionId")]
        [Association("Transactions", "TransactionId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> TransactionId { get; set; }
    }
}
