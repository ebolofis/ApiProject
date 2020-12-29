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
    [Table("OrderDetailIgredients")]
    [DisplayName("OrderDetailIgredients")]
    public class OrderDetailIgredientsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_OrderDetailIgredients")]
        public long Id { get; set; }

        [Column("IngredientId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetailIgredients_Ingredients")]
        [Association("Ingredients", "IngredientId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> IngredientId { get; set; }

        [Column("Qty", Order = 3, TypeName = "FLOAT")]
        public Nullable<double> Qty { get; set; }

        [Column("UnitId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> UnitId { get; set; }

        [Column("Price", Order = 5, TypeName = "MONEY")]
        public Nullable<decimal> Price { get; set; }

        [Column("OrderDetailId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetailIgredients_OrderDetail")]
        [Association("OrderDetail", "OrderDetailId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]//ON UPDATE CASCADE
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> OrderDetailId { get; set; }

        [Column("PriceListDetailId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetailIgredients_PricelistDetail")]
        [Association("PricelistDetail", "PriceListDetailId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PriceListDetailId { get; set; }

        [Column("Discount", Order = 8, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Discount { get; set; }

        [Column("TotalAfterDiscount", Order = 9, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> TotalAfterDiscount { get; set; }

        [Column("IsDeleted", Order = 10, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("PendingQty", Order = 11, TypeName = "FLOAT")]
        public Nullable<double> PendingQty { get; set; }
    }
}
