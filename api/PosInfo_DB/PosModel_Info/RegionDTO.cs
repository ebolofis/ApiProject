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
    [Table("Region")]
    [DisplayName("Region")]
    public class RegionDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Region")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(500)")]
        public string Description { get; set; }

        [Column("StartTime", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> StartTime { get; set; }

        [Column("EndTime", Order = 4, TypeName = "DATETIME")]
        public Nullable<System.DateTime> EndTime { get; set; }

        [Column("BluePrintPath", Order = 5, TypeName = "NVARCHAR(MAX)")]
        public string BluePrintPath { get; set; }

        [Column("MaxCapacity", Order = 7, TypeName = "INT")]
        public Nullable<int> MaxCapacity { get; set; }

        [Column("PosInfoId", Order = 8, TypeName = "BIGINT")]
        [ForeignKey("FK_Region_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PosInfoId { get; set; }

        [Column("IsLocker", Order = 9, TypeName = "BIT")]
        public Nullable<bool> IsLocker { get; set; }
    }
}
