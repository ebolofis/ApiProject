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
    [Table("CreditAccounts")]
    [DisplayName("CreditAccounts")]
    public class CreditAccountsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_CreditAccounts")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("EndOfDayId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_CreditAccounts_EndOfDay")]
        [Association("EndOfDay", "EndOfDayId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> EndOfDayId { get; set; }

        [Column("ActivateTS", Order = 4, TypeName = "DATETIME")]
        public Nullable<System.DateTime> ActivateTS { get; set; }

        [Column("DeactivateTS", Order = 5, TypeName = "DATETIME")]
        public Nullable<System.DateTime> DeactivateTS { get; set; }
    }
}
