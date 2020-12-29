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
    [Table("PosInfoDetail_Excluded_Accounts")]
    [DisplayName("PosInfoDetail_Excluded_Accounts")]
    public class PosInfoDetail_Excluded_AccountsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PosInfoDetail_Excluded_Accounts")]
        public long Id { get; set; }

        [Column("PosInfoDetailId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfoDetail_Excluded_Accounts_PosInfoDetail")]
        [Association("PosInfoDetail", "PosInfoDetailId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PosInfoDetailId { get; set; }

        [Column("AccountId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfoDetail_Excluded_Accounts_Accounts")]
        [Association("Accounts", "AccountId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(-1)]
        [MinLength(-1)]
        public Nullable<long> AccountId { get; set; }
    }
}
