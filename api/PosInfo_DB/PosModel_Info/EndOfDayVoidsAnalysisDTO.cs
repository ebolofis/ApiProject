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
    [Table("EndOfDayVoidsAnalysis")]
    [DisplayName("EndOfDayVoidsAnalysis")]
    public class EndOfDayVoidsAnalysisDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_EndOfDayVoids")]
        public long Id { get; set; }

        [Column("EndOfDayId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_EndOfDayVoids_EndOfDay")]
        [Association("EndOfDay", "EndOfDayId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]//ON UPDATE CASCADE
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> EndOfDayId { get; set; }

        [Column("AccountId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_EndOfDayVoids_Accounts")]
        [Association("Accounts", "AccountId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]//ON UPDATE CASCADE
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> AccountId { get; set; }

        [Column("Total", Order = 4, TypeName = "DECIMAL(9,2)")]
        public Nullable<decimal> Total { get; set; }

        [Column("Description", Order = 5, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("AccountType", Order = 6, TypeName = "SMALLINT")]
        public Nullable<short> AccountType { get; set; }
    }
}
