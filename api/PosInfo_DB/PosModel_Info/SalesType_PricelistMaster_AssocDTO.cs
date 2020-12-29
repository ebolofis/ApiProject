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
    [Table("SalesType_PricelistMaster_Assoc")]
    [DisplayName("SalesType_PricelistMaster_Assoc")]
    public class SalesType_PricelistMaster_AssocDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_SalesType_PricelistMaster_Assoc")]
        public long Id { get; set; }

        [Column("PricelistMasterId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_SalesType_PricelistMaster_Assoc_PricelistMaster")]
        [Association("PricelistMaster", "PricelistMasterId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MinLength(1)]
        public Nullable<long> PricelistMasterId { get; set; }

        [Column("SalesTypeId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_SalesType_PricelistMaster_Assoc_SalesType_PricelistMaster_Assoc")]
        [Association("SalesType", "SalesTypeId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MinLength(1)]
        public Nullable<long> SalesTypeId { get; set; }
    }
}
