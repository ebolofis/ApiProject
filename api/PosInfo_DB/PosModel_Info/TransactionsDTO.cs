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
    [Table("Transactions")]
    [DisplayName("Transactions")]
    public class TransactionsDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Transactions")]
        public long Id { get; set; }

        [Column("Day", Order = 2, TypeName = "DATETIME")]
        public Nullable<System.DateTime> Day { get; set; }

        [Column("PosInfoId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_Transactions_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MinLength(-1)]
        public Nullable<long> PosInfoId { get; set; }

        [Column("StaffId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_Transactions_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffId { get; set; }

        [Column("OrderId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_Transactions_Order")]
        [Association("Order", "OrderId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> OrderId { get; set; }

        [Column("TransactionType", Order = 6, TypeName = "SMALLINT")]
        public Nullable<short> TransactionType { get; set; }

        [Column("Amount", Order = 7, TypeName = "MONEY")]
        public Nullable<decimal> Amount { get; set; }

        [Column("DepartmentId", Order = 8, TypeName = "BIGINT")]
        [ForeignKey("FK_Transactions_Department")]
        [Association("Department", "DepartmentId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> DepartmentId { get; set; }

        [Column("Description", Order = 9, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("AccountId", Order = 10, TypeName = "BIGINT")]
        [ForeignKey("FK_Transactions_Accounts")]
        [Association("Accounts", "AccountId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
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
        [ForeignKey("FK_dbo.Transactions_dbo.EndOfDay_EndOfDayId")]
        [Association("EndOfDay", "EndOfDayId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> EndOfDayId { get; set; }

        [Column("ExtDescription", Order = 17, TypeName = "NVARCHAR(500)")]
        public string ExtDescription { get; set; }

        [Column("InvoicesId", Order = 18, TypeName = "BIGINT")]
        [ForeignKey("FK_Transactions_Invoices")]
        [Association("Invoices", "InvoicesId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> InvoicesId { get; set; }

        [Column("IsDeleted", Order = 19, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("SupplierId", Order = 20, TypeName = "BIGINT")]
        [ForeignKey("FK_Transactions_Suppliers")]
        [Association("Suppliers", "SupplierId", "Id")]
        public Nullable<long> SupplierId { get; set; }
    }
}
