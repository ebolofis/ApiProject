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
    [Table("PosInfo_KitchenInstruction_Assoc")]
    [DisplayName("PosInfo_KitchenInstruction_Assoc")]
    public class PosInfo_KitchenInstruction_AssocDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PosInfo_KitchenInstruction_Assoc")]
        public long Id { get; set; }

        [Column("PosInfoId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfo_KitchenInstruction_Assoc_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PosInfoId { get; set; }

        [Column("KitchenInstructionId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfo_KitchenInstruction_Assoc_KitchenInstruction")]
        [Association("KitchenInstruction", "KitchenInstructionId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> KitchenInstructionId { get; set; }
    }
}
