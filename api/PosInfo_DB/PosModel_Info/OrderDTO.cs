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
    [Table("Order")]
    [DisplayName("Order")]
    public class OrderDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Order")]
        public long Id { get; set; }

        [Column("OrderNo", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> OrderNo { get; set; }

        [Column("Day", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> Day { get; set; }

        [Column("Total", Order = 4, TypeName = "MONEY")]
        public Nullable<decimal> Total { get; set; }

        [Column("PosId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_Order_PosInfo")]
        [Association("PosInfo", "PosId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PosId { get; set; }

        [Column("StaffId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_Order_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StaffId { get; set; }

        [Column("EndOfDayId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_Order_EndOfDay")]
        [Association("EndOfDay", "EndOfDayId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> EndOfDayId { get; set; }

        [Column("Discount", Order = 8, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> Discount { get; set; }

        [Column("ReceiptNo", Order = 9, TypeName = "INT")]
        public Nullable<int> ReceiptNo { get; set; }

        [Column("TotalBeforeDiscount", Order = 10, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> TotalBeforeDiscount { get; set; }

        [Column("PdaModuleId", Order = 11, TypeName = "BIGINT")]
        [ForeignKey("FK_Order_PdaModule")]
        [Association("PdaModule", "PdaModuleId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PdaModuleId { get; set; }

        [Column("ClientPosId", Order = 12, TypeName = "BIGINT")]
        public Nullable<long> ClientPosId { get; set; }

        [Column("IsDeleted", Order = 13, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("ExtType", Order = 14, TypeName = "INT")]
        public Nullable<int> ExtType { get; set; }

        [Column("ExtObj", Order = 15, TypeName = "VARCHAR(MAX)")]
        public string ExtObj { get; set; }

        [Column("ExtKey", Order = 16, TypeName = "VARCHAR(250)")]
        public string ExtKey { get; set; }

        [Column("OrderOrigin", Order = 17, TypeName = "INT")]
        public Nullable<int> OrderOrigin { get; set; }

        [Column("Couver", Order = 18, TypeName = "INT")]
        public Nullable<int> Couver { get; set; }

        [Column("DA_IsPaid", Order = 19, TypeName = "BIT")]
        public Nullable<bool> DA_IsPaid { get; set; }
        
        [Column("EstTakeoutDate", Order = 20, TypeName = "DATETIME")]
        public Nullable<DateTime> EstTakeoutDate { get; set; }

        [Column("IsDelay", Order = 21, TypeName = "BIT")]
        public Nullable<bool> IsDelay { get; set; }

        /// <summary>
        /// DA Notes For Order
        /// </summary>
        [Column("OrderNotes", Order = 22, TypeName = "NVARCHAR(1500)")]
        public string OrderNotes { get; set; }

        /// <summary>
        /// DA Notes For Store
        /// </summary>
        [Column("StoreNotes", Order = 23, TypeName = "NVARCHAR(1500)")]
        public string StoreNotes { get; set; }

        /// <summary>
        /// DA Notes For Customer
        /// </summary>
        [Column("CustomerNotes", Order = 24, TypeName = "NVARCHAR(1500)")]
        public string CustomerNotes { get; set; }

        /// <summary>
        /// DA Secret Notes For Customer
        /// </summary>
        [Column("CustomerSecretNotes", Order = 25, TypeName = "NVARCHAR(1500)")]
        public string CustomerSecretNotes { get; set; }

        /// <summary>
        /// DA Last Order Notes For Customer
        /// </summary>
        [Column("CustomerLastOrderNotes", Order = 26, TypeName = "NVARCHAR(1500)")]
        public string CustomerLastOrderNotes { get; set; }

        /// <summary>
        /// Logical Error message etc. prices not same, different vats ....
        /// </summary>
        [Column("LogicErrors", Order = 27, TypeName = "TEXT")]
        public string LogicErrors { get; set; }

        /// <summary>
        /// true: at least one item has changed
        /// </summary>
        [Column("ItemsChanged", Order = 28, TypeName = "BIT")]
        public Nullable<bool> ItemsChanged { get; set; }

        /// <summary>
        /// προέλευση παραγγ.: 0: agent, 1: website, 2: mobile, 3: e-food
        /// </summary>
        [Column("DA_Origin", Order = 29, TypeName = "SMALLINT")]
        public Nullable<Int16> DA_Origin { get; set; }


        /// <summary>
        /// Loyalty Code concerning DA Requests
        /// </summary>
        [Column("LoyaltyCode", Order = 30, TypeName = "NVARCHAR(200)")]
        public string LoyaltyCode { get; set; }
        /// <summary>
        ///TelephoneNumber concerning  DA Requests
        /// </summary>
        [Column("TelephoneNumber", Order = 31, TypeName = "NVARCHAR(30)")]
        public string TelephoneNumber { get; set; }

        [Column("CouverAdults", Order = 32, TypeName = "INT")]
        public Nullable<int> CouverAdults { get; set; }

        [Column("CouverChildren", Order = 33, TypeName = "INT")]
        public Nullable<int> CouverChildren { get; set; }

        [Column("MacroGuidId", Order = 34, TypeName = "uniqueidentifier")]
        [ForeignKey("FK_Order_MacroGuid")]
        [Association("HotelMacros", "MacroGuidId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<Guid> MacroGuidId { get; set; }

        [Column("DeliveryRoutingId", Order = 35, TypeName = "BIGINT")]
        [ForeignKey("FK_Order_DeliveryRouting")]
        [Association("DeliveryRouting", "DeliveryRoutingId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> DeliveryRoutingId { get; set; }
    }
}
