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
    [Table("OrderStatus_Hist")]
    [DisplayName("OrderStatus_Hist")]
    public class OrderStatus_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_OrderStatus_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("Status", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> Status { get; set; }

        [Column("TimeChanged", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> TimeChanged { get; set; }

        [Column("OrderId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> OrderId { get; set; }

        [Column("StaffId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }

        [Column("ExtState", Order = 6, TypeName = "INT")]
        public Nullable<int> ExtState { get; set; }

        /// <summary>
        /// Status sended to DA Delivery Server
        /// </summary>
        [Column("IsSend", Order = 7, TypeName = "BIT")]

        public Nullable<bool> IsSend { get; set; }

        /// <summary>
        /// Order Id from Delivery Agent Server
        /// </summary>
        [Column("DAOrderId", Order = 8, TypeName = "BIGINT")]

        public Nullable<bool> DAOrderId { get; set; }
    }
}
