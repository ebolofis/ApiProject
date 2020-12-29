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
    [Table("PdaModuleDetail")]
    [DisplayName("PdaModuleDetail")]
    public class PdaModuleDetailDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PdaModuleDetail")]
        public long Id { get; set; }

        [Column("PdaModuleId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PdaModuleDetail_PdaModule")]
        [Association("PdaModule", "PdaModuleId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(-1)]
        [MinLength(-1)]
        public Nullable<long> PdaModuleId { get; set; }

        [Column("LastLoginTS", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> LastLoginTS { get; set; }

        [Column("LastLogoutTS", Order = 4, TypeName = "DATETIME")]
        public Nullable<System.DateTime> LastLogoutTS { get; set; }

        [Column("StaffId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }
    }
}
