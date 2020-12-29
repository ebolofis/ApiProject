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
    [Table("StaffSchedule")]
    [DisplayName("StaffSchedule")]
    public class StaffScheduleDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_StaffSchedule")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("Day", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> Day { get; set; }

        [Column("DepartmentId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_StaffSchedule_Department")]
        [Association("Department", "DepartmentId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> DepartmentId { get; set; }
    }
}
