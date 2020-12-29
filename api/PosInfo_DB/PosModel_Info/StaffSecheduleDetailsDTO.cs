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
    [Table("StaffSecheduleDetails")]
    [DisplayName("StaffSecheduleDetails")]
    public class StaffSecheduleDetailsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_StaffSecheduleDetails")]
        public long Id { get; set; }

        [Column("StaffSceduleId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_StaffSecheduleDetails_StaffSchedule")]
        [Association("StaffSchedule", "StaffSceduleId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffSceduleId { get; set; }

        [Column("StaffId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_StaffSecheduleDetails_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffId { get; set; }

        [Column("StaffPositionId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> StaffPositionId { get; set; }

        [Column("Hour", Order = 5, TypeName = "DATETIME")]
        public Nullable<System.DateTime> Hour { get; set; }
    }
}
