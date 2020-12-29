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
    [Table("HotelMacroRules")]
    [DisplayName("HotelMacroRules")]
    public class HotelMacroRulesDTO
    {
        [Column("Id", Order = 1, TypeName = "uniqueidentifier")]
        [Key]
        //[Editable(false)]
        [DisplayName("PK_HotelMacroRules")]
        public Guid Id { get; set; }

        [Column("Model", Order = 2, TypeName = "TEXT")]
        public string Model { get; set; }
    }
}