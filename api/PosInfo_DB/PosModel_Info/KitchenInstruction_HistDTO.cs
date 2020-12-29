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
    [Table("KitchenInstruction_Hist")]
    [DisplayName("KitchenInstruction_Hist")]
    public class KitchenInstruction_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_KitchenInstruction_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("KitchenId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> KitchenId { get; set; }

        [Column("Message", Order = 3, TypeName = "NVARCHAR(300)")]
        public string Message { get; set; }

        [Column("Description", Order = 4, TypeName = "NVARCHAR(100)")]
        public string Description { get; set; }
    }
}
