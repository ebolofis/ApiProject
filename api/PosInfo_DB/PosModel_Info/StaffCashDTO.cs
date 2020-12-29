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
    [Table("StaffCash")]
    [DisplayName("StaffCash")]
    public class StaffCashDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_StaffCash")]
        public long Id { get; set; }

        [Column("StaffId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_StaffCash_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffId { get; set; }

        [Column("CashAmount", Order = 3, TypeName = "MONEY")]
        public Nullable<decimal> CashAmount { get; set; }

        [Column("EndOfDayId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_StaffCash_EndOfDay")]
        [Association("EndOfDay", "EndOfDayId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> EndOfDayId { get; set; }

        [Column("Date", Order = 5, TypeName = "DATETIME")]
        public DateTime Date { get; set; }
    }
}
