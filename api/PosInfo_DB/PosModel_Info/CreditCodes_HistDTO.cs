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
    [Table("CreditCodes_Hist")]
    [DisplayName("CreditCodes_Hist")]
    public class CreditCodes_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_CreditCodes_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("Description", Order = 3, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("Code", Order = 4, TypeName = "NVARCHAR(150)")]
        public string Code { get; set; }

        [Column("Type", Order = 5, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("CreditAccountId", Order = 6, TypeName = "BIGINT")]
        public Nullable<long> CreditAccountId { get; set; }
    }
}
