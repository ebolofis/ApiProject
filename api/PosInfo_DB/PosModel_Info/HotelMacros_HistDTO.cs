
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
    [Table("HotelMacros_Hist")]
    [DisplayName("HotelMacros_Hist")]
    public class HotelMacros_HistDTO
    {
        [Column("hisId", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_HotelMacros_Hist")]
        public long hisId { get; set; }

        [Column("Id", Order = 2, TypeName = "uniqueidentifier")]
        public Nullable<Guid> Id { get; set; }

        [Column("Model", Order = 3, TypeName = "TEXT")]
        public string Model { get; set; }
    }
}
