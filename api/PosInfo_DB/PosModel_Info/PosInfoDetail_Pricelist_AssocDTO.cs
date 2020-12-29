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
    [Table("PosInfoDetail_Pricelist_Assoc")]
    [DisplayName("PosInfoDetail_Pricelist_Assoc")]
    public class PosInfoDetail_Pricelist_AssocDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PosInfoDetail_Pricelist_Assoc")]
        public long Id { get; set; }

        [Column("PosInfoDetailId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfoDetail_Pricelist_Assoc_PosInfoDetail")]
        [Association("PosInfoDetail", "PosInfoDetailId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> PosInfoDetailId { get; set; }

        [Column("PricelistId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfoDetail_Pricelist_Assoc_Pricelist")]
        [Association("Pricelist", "PricelistId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> PricelistId { get; set; }
    }
}
