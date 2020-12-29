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
    [Table("ProductExtras")]
    [DisplayName("ProductExtras")]
    public class ProductExtrasDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ProductExtras")]
        public long Id { get; set; }

        [Column("ProductId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_ProductExtras_Product")]
        [Association("Product", "ProductId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ProductId { get; set; }

        [Column("IsRequired", Order = 3, TypeName = "BIT")]
        public Nullable<bool> IsRequired { get; set; }

        [Column("IngredientId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_ProductExtras_Ingredients")]
        [Association("Ingredients", "IngredientId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> IngredientId { get; set; }

        [Column("MinQty", Order = 4, TypeName = "FLOAT")]
        public Nullable<double> MinQty { get; set; }

        [Column("MaxQty", Order = 5, TypeName = "FLOAT")]
        public Nullable<double> MaxQty { get; set; }

        [Column("UnitId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_ProductExtras_Units")]
        [Association("Units", "UnitId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> UnitId { get; set; }

        [Column("ItemsId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_ProductExtras_Items")]
        [Association("Items", "ItemsId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ItemsId { get; set; }

        [Column("Price", Order = 8, TypeName = "MONEY")]
        public Nullable<decimal> Price { get; set; }

        [Column("ProductAsIngridientId", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> ProductAsIngridientId { get; set; }

        [Column("Sort", Order = 10, TypeName = "INT")]
        public Nullable<int> Sort { get; set; }

        [Column("DAId", Order = 11, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }

        [Column("CanSavePrice", Order = 12, TypeName = "BIT")]
        public Nullable<bool> CanSavePrice { get; set; }
    }
}
