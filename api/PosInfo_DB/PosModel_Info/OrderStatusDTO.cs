using Symposium.Models.Enums;
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
    [Table("OrderStatus")]
    [DisplayName("OrderStatus")]
    public class OrderStatusDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_OrderStatus")]
        public long Id { get; set; }

        [Column("Status", Order = 2, TypeName = "BIGINT")]
        public Nullable<OrderStatusEnum> Status { get; set; }

        [Column("TimeChanged", Order = 3, TypeName = "DATETIME")]
        [DisplayFormatAttribute(DataFormatString = "DF_OrderStatus_TimeChanged", NullDisplayText = "GETDATE()")]//DefaultValue (Name, Value)
        public Nullable<System.DateTime> TimeChanged { get; set; }

        [Column("OrderId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderStatus_Order")]
        [Association("Order", "OrderId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> OrderId { get; set; }

        [Column("StaffId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderStatus_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffId { get; set; }

        [Column("ExtState", Order = 6, TypeName = "INT")]
        [DisplayFormatAttribute(DataFormatString = "DF_OrderStatus_ExtState", NullDisplayText = "NULL")]//DefaultValue (Name, Value)
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

        public Nullable<long> DAOrderId { get; set; }
    }
}
