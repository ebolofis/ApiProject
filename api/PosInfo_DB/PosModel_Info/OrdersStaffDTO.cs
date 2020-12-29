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
    [Table("OrdersStaff")]
    [DisplayName("OrdersStaff")]
    public class OrdersStaffDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_OrdersStaff")]
        public long Id { get; set; }

        [Column("StaffId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_OrdersStaff_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> StaffId { get; set; }

        [Column("OrderId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_OrdersStaff_Order")]
        [Association("Order", "OrderId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> OrderId { get; set; }

        [Column("Type", Order = 4, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }
    }
}
