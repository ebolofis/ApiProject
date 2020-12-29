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
    [Table("StaffAuthorization")]
    [DisplayName("StaffAuthorization")]
    public class StaffAuthorizationDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("[PK_StaffAuthorization]")]
        public long Id { get; set; }

        [Column("AuthorizedGroupId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_StaffAuthorization_AuthorizedGroup")]
        [Association("AuthorizedGroup", "AuthorizedGroupId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> AuthorizedGroupId { get; set; }

        [Column("StaffId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_StaffAuthorization_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffId { get; set; }
    }
}
