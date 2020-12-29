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
    [Table("PagePosAssoc")]
    [DisplayName("PagePosAssoc")]
    public class PagePosAssocDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PagePosAssoc")]
        public long Id { get; set; }

        [Column("PageSetId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PagePosAssoc_PageSet")]
        [Association("PageSet", "PageSetId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PageSetId { get; set; }

        [Column("PosInfoId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_PagePosAssoc_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(-1)]//negative is NULL
        [MinLength(-1)]
        public Nullable<long> PosInfoId { get; set; }
    }
}
