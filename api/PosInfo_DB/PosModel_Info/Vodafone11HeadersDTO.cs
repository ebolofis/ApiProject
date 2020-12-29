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
    [Table("Vodafone11Headers")]
    [DisplayName("Vodafone11Headers")]
    public class Vodafone11HeadersDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Vodafone11Headers")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(500)")]
        public string Description { get; set; }

        [Column("FromItems", Order = 3, TypeName = "INT")]
        public int FromItems { get; set; }

        [Column("RemoveItems", Order = 4, TypeName = "INT")]
        public int RemoveItems { get; set; }

    }
}
