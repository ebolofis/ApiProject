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
    [Table("PosInfoKdsAssoc")]
    [DisplayName("PosInfoKdsAssoc")]
    public class PosInfoKdsAssocDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PosInfoKdsAssoc")]
        public long Id { get; set; }

        [Column("PosInfoId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfoKdsAssoc_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> PosInfoId { get; set; }

        [Column("KdsId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfoKdsAssoc_Kds")]
        [Association("Kds", "KdsId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> KdsId { get; set; }
    }
}
