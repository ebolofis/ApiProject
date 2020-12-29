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
    [Table("KitchenRegion")]
    [DisplayName("KitchenRegion")]
    public class KitchenRegionDTO :ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_KitchenRegion")]
        public long Id { get; set; }

        [Column("ItemRegion", Order = 2, TypeName = "NVARCHAR(50)")]
        public string ItemRegion { get; set; }

        [Column("RegionPosition", Order = 3, TypeName = "INT")]
        public Nullable<int> RegionPosition { get; set; }

        [Column("Abbr", Order = 4, TypeName = "NVARCHAR(5)")]
        public string Abbr { get; set; }
    }
}
