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
    [Table("PosInfo_Pricelist_Assoc")]
    [DisplayName("PosInfo_Pricelist_Assoc")]
    public class PosInfo_Pricelist_AssocDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PosInfo_Pricelist_Assoc")]
        public long Id { get; set; }

        [Column("PosInfoId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfo_Pricelist_Assoc_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> PosInfoId { get; set; }

        [Column("PricelistId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfo_Pricelist_Assoc_Pricelist")]
        [Association("Pricelist", "PricelistId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> PricelistId { get; set; }
    }
}
