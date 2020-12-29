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
    [Table("ClientPos")]
    [DisplayName("ClientPos")]
    public class ClientPosDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ClientPos")]
        public long Id { get; set; }

        [Column("Code", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("PosInfoId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_ClientPos_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(-1)]
        [MinLength(-1)]
        public Nullable<long> PosInfoId { get; set; }

        [Column("Theme", Order = 4, TypeName = "NVARCHAR(50)")]
        public string Theme { get; set; }

        [Column("LogInToOrder", Order = 5, TypeName = "BIT")]
        public Nullable<bool> LogInToOrder { get; set; }

        [Column("Status", Order = 6, TypeName = "INT")]
        public Nullable<int> Status { get; set; }

        [Column("IsDeleted", Order = 7, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }
    }
}
