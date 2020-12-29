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
    [Table("EndOfDayVoidsAnalysis_Hist")]
    [DisplayName("EndOfDayVoidsAnalysis_Hist")]
    public class EndOfDayVoidsAnalysis_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_EndOfDayVoids_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("EndOfDayId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> EndOfDayId { get; set; }

        [Column("AccountId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> AccountId { get; set; }

        [Column("Total", Order = 5, TypeName = "DECIMAL(9,2)")]
        public Nullable<decimal> Total { get; set; }

        [Column("Description", Order = 6, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("AccountType", Order = 7, TypeName = "SMALLINT")]
        public Nullable<short> AccountType { get; set; }
    }
}
