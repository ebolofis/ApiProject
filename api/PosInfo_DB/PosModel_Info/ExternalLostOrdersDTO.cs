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
    [Table("ExternalLostOrders")]
    [DisplayName("ExternalLostOrders")]
    public class ExternalLostOrdersDTO
    {
        [Column("OrderNo", Order = 1, TypeName = "VARCHAR(255)")]
        [Key]
        [DisplayName("PK_ExternalLostOrders")]
        [Association("OrderNo", "ExtType", "")]/*Master Key With 2 Columns*/
        public string OrderNo { get; set; }

        [Column("ExtType", Order = 2, TypeName = "INT")]
        [Required]
        public int ExtType { get; set; }
    }
}
