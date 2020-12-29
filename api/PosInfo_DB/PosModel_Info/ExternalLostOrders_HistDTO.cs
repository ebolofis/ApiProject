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
    [Table("ExternalLostOrders_Hist")]
    [DisplayName("ExternalLostOrders_Hist")]
    public class ExternalLostOrders_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_ExternalLostOrders_Hist")]
        [Association("nYear", "OrderNo", "ExtType")]/*Master Key With 2 Columns*/
        public int nYear { get; set; }

        [Column("OrderNo", Order = 2, TypeName = "VARCHAR(255)")]
        [Required]
        public string OrderNo { get; set; }

        [Column("ExtType", Order = 3, TypeName = "INT")]
        [Required]
        public int ExtType { get; set; }
    }
}
