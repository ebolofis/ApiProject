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
    [Table("AuthorizedGroupDetail")]
    [DisplayName("AuthorizedGroupDetail")]
    public class AuthorizedGroupDetailDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_AuthorizedGroupDetail")]
        public long Id { get; set; }

        [Column("AuthorizedGroupId", Order = 1, TypeName = "BIGINT")]
        [ForeignKey("FK_AuthorizedGroupDetail_AuthorizedGroup")]
        [Association("AuthorizedGroup", "AuthorizedGroupId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> AuthorizedGroupId { get; set; }

        [Column("ActionId", Order = 1, TypeName = "BIGINT")]
        [ForeignKey("FK_AuthorizedGroupDetail_Actions")]
        [Association("Actions", "ActionId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ActionId { get; set; }

    }
}
