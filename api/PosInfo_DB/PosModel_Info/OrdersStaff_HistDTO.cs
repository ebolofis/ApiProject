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
    [Table("OrdersStaff_Hist")]
    [DisplayName("OrdersStaff_Hist")]
    public class OrdersStaff_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_OrdersStaff_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("StaffId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }

        [Column("OrderId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> OrderId { get; set; }

        [Column("Type", Order = 4, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }
    }
}
