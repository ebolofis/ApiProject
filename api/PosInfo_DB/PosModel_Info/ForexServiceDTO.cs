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
    [Table("ForexService")]
    [DisplayName("ForexService")]
    public class ForexServiceDTO : ITables
    {

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ForexService")]
        public long Id { get; set; }

        [Column("CurFrom", Order = 2, TypeName = "NVARCHAR(20)")]
        public string CurFrom { get; set; }

        [Column("CurTo", Order = 3, TypeName = "NVARCHAR(20)")]
        public string CurTo { get; set; }

        [Column("Rate", Order = 4, TypeName = "DECIMAL(18,6)")]
        public decimal Rate { get; set; }

        [Column("Date", Order = 5, TypeName = "DATETIME")]
        public System.DateTime Date { get; set; }

    }
}
