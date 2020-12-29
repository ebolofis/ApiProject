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
    [Table("DA_OrderStatus")]
    [DisplayName("DA_OrderStatus")]
    public class DA_OrderStatusDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_OrderStatus")]
        public long Id { get; set; }

        [Column("OrderDAId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DA_OrderStatus_DA_Orders")]
        [Association("DA_Orders", "OrderDAId", "Id")]
        public long OrderDAId { get; set; }

        [Column("Status", Order = 3, TypeName = "SMALLINT")]
        [Required]
        public Int16 Status { get; set; }

        [Column("StatusDate", Order = 4, TypeName = "DATETIME")]
        [Required]
        public DateTime StatusDate { get; set; }
    }
}
