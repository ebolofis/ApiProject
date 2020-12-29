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
    [Table("PdaModule")]
    [DisplayName("PdaModule")]
    public class PdaModuleDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PdaModule")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("Status", Order = 3, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("MaxActiveUsers", Order = 4, TypeName = "INT")]
        public Nullable<int> MaxActiveUsers { get; set; }

        [Column("PosInfoId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_PdaModule_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(-1)]
        [MinLength(-1)]
        public Nullable<long> PosInfoId { get; set; }

        [Column("PageSetId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_PdaModule_PageSet")]
        [Association("PageSet", "PageSetId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(-1)]
        [MinLength(-1)]
        public Nullable<long> PageSetId { get; set; }

        [Column("Code", Order = 7, TypeName = "NVARCHAR(20)")]
        public string Code { get; set; }

        [Column("IsDeleted", Order = 8, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }
    }
}
