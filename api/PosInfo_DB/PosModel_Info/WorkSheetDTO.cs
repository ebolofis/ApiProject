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
    [Table("WorkSheet")]
    [DisplayName("WorkSheet")]
    public class WorkSheetDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_WorkSheet")]
        public long Id { get; set; }

        [Column("StaffId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_WorkSheet_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffId { get; set; }

        [Column("Day", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> Day { get; set; }

        [Column("Hour", Order = 4, TypeName = "DATETIME")]
        public Nullable<System.DateTime> Hour { get; set; }

        [Column("Type", Order = 5, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("DepartmentId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_WorkSheet_Department")]
        [Association("Department", "DepartmentId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> DepartmentId { get; set; }
    }
}
