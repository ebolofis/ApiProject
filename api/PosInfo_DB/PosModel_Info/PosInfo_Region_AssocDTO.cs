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
    [Table("PosInfo_Region_Assoc")]
    [DisplayName("PosInfo_Region_Assoc")]
    public class PosInfo_Region_AssocDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PosInfo_Region_Assoc")]
        public long Id { get; set; }

        [Column("PosInfoId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfo_Region_Assoc_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PosInfoId { get; set; }

        [Column("RegionId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfo_Region_Assoc_Region")]
        [Association("Region", "RegionId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> RegionId { get; set; }

        [Column("Description", Order = 4, TypeName = "NVARCHAR(300)")]
        public string Description { get; set; }
    }
}
