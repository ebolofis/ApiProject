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
    [Table("AssignedPositions")]
    [DisplayName("AssignedPositions")]
    public class AssignedPositionsDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_AssignedPositions")]
        public long Id { get; set; }

        [Column("StaffPositionId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_AssignedPositions_StaffPosition")]
        [Association("StaffPosition", "StaffPositionId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffPositionId { get; set; }

        [Column("StaffId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_AssignedPositions_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffId { get; set; }
    }
}
