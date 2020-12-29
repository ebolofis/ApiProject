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
    [Table("OrderDetail")]
    [DisplayName("OrderDetail")]
    public class OrderDetailDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_OrderDetailId")]
        public long Id { get; set; }

        [Column("OrderId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetail_Order")]
        [Association("Order", "OrderId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]//ON UPDATE CASCADE
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> OrderId { get; set; }

        [Column("ProductId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetail_Product")]
        [Association("Product", "ProductId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ProductId { get; set; }

        [Column("KitchenId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> KitchenId { get; set; }

        [Column("KdsId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> KdsId { get; set; }

        [Column("PreparationTime", Order = 6, TypeName = "INT")]
        public Nullable<int> PreparationTime { get; set; }

        [Column("TableId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetail_Table")]
        [Association("Table", "TableId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]//ON UPDATE CASCADE
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> TableId { get; set; }

        [Column("Status", Order = 8, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("StatusTS", Order = 9, TypeName = "DATETIME")]
        [DisplayFormatAttribute(DataFormatString = "DF_OrderDetail_StatusTS", NullDisplayText = "GETDATE()")]
        public Nullable<System.DateTime> StatusTS { get; set; }

        [Column("Price", Order = 10, TypeName = "MONEY")]
        public Nullable<decimal> Price { get; set; }

        [Column("PriceListDetailId", Order = 11, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetail_PricelistDetail")]
        [Association("PricelistDetail", "PriceListDetailId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PriceListDetailId { get; set; }

        [Column("Qty", Order = 12, TypeName = "FLOAT")]
        public Nullable<double> Qty { get; set; }

        [Column("SalesTypeId", Order = 13, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetail_SalesType")]
        [Association("SalesType", "SalesTypeId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> SalesTypeId { get; set; }

        [Column("Discount", Order = 14, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Discount { get; set; }

        [Column("PaidStatus", Order = 15, TypeName = "TINYINT")]
        public Nullable<byte> PaidStatus { get; set; }

        [Column("TransactionId", Order = 16, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetail_Transactions")]
        [Association("Transactions", "TransactionId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> TransactionId { get; set; }

        [Column("TotalAfterDiscount", Order = 17, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> TotalAfterDiscount { get; set; }

        [Column("GuestId", Order = 18, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetail_Guest")]
        [Association("Guest", "GuestId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> GuestId { get; set; }

        [Column("Couver", Order = 19, TypeName = "INT")]
        public Nullable<int> Couver { get; set; }

        [Column("Guid", Order = 20, TypeName = "UNIQUEIDENTIFIER")]
        public Nullable<System.Guid> Guid { get; set; }

        [Column("IsDeleted", Order = 21, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("PendingQty", Order = 22, TypeName = "FLOAT")]
        public Nullable<double> PendingQty { get; set; }

        /// <summary>
        /// Type of special discount that is used. 0: Hit Loyalty, 1: Goodys discounts, 2: Vodafone discounts
        /// </summary>
        [Column("OtherDiscount", Order = 23, TypeName = "SMALLINT")]
        public Nullable<DA_OrderDetail_OtherDiscountEnum> OtherDiscount { get; set; }

        /// <summary>
        /// 0: Ready , 1:Pending , 2:Kitchen
        /// </summary>
        [Column("KitchenStatus", Order = 24, TypeName = "INT")]
        public Nullable<int> KitchenStatus { get; set; }

        [Column("LoginStaffId", Order = 25, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetail_Staff")]
        [Association("Staff", "LoginStaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> LoginStaffId { get; set; }
    }
}
