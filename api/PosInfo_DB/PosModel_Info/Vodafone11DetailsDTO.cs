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
    [Table("Vodafone11Details")]
    [DisplayName("Vodafone11Details")]
   public class Vodafone11DetailsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Vodafone11Details")]
        public long Id { get; set; }

        [Column("HeaderId", Order = 2, TypeName = "BIGINT")]
        public long HeaderId { get; set; }

        [Column("ProdCategoryId", Order = 3, TypeName = "BIGINT")]
        public long ProdCategoryId { get; set; }

        [Column("Position", Order = 3, TypeName = "INT")]
        public int Position { get; set; }
    }
}
