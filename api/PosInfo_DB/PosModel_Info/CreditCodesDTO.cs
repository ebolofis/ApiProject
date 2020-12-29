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
    [Table("CreditCodes")]
    [DisplayName("CreditCodes")]
    public class CreditCodesDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_CreditCodes")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("Code", Order = 3, TypeName = "NVARCHAR(150)")]
        public string Code { get; set; }

        [Column("Type", Order = 4, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("CreditAccountId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_CreditCodes_CreditAccounts")]
        [Association("CreditAccounts", "CreditAccountId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> CreditAccountId { get; set; }
    }
}
