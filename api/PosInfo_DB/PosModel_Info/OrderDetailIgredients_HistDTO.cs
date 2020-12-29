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
    [Table("OrderDetailIgredients_Hist")]
    [DisplayName("OrderDetailIgredients_Hist")]
    public class OrderDetailIgredients_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_OrderDetailIgredients_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("IngredientId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> IngredientId { get; set; }

        [Column("Qty", Order = 3, TypeName = "FLOAT")]
        public Nullable<double> Qty { get; set; }

        [Column("UnitId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> UnitId { get; set; }

        [Column("Price", Order = 5, TypeName = "MONEY")]
        public Nullable<decimal> Price { get; set; }

        [Column("OrderDetailId", Order = 6, TypeName = "BIGINT")]
        public Nullable<long> OrderDetailId { get; set; }

        [Column("PriceListDetailId", Order = 7, TypeName = "BIGINT")]
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
