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
    [Table("CreditAccounts_Hist")]
    [DisplayName("CreditAccounts_Hist")]
    public class CreditAccounts_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_CreditAccounts_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("Description", Order = 3, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("EndOfDayId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> EndOfDayId { get; set; }

        [Column("ActivateTS", Order = 5, TypeName = "DATETIME")]
        public Nullable<System.DateTime> ActivateTS { get; set; }

        [Column("DeactivateTS", Order = 6, TypeName = "DATETIME")]
        public Nullable<System.DateTime> DeactivateTS { get; set; }
    }
}
