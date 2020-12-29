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
    [Table("PriceList_EffectiveHours")]
    [DisplayName("PriceList_EffectiveHours")]
    public class PriceList_EffectiveHoursDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PriceList_EffectiveHours")]
        public long Id { get; set; }

        [Column("PricelistId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PriceList_EffectiveHours_Pricelist")]
        [Association("Pricelist", "PricelistId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PricelistId { get; set; }

        [Column("FromTime", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> FromTime { get; set; }

        [Column("ToTime", Order = 4, TypeName = "DATETIME")]
        public Nullable<System.DateTime> ToTime { get; set; }

        [Column("DAId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
