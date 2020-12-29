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
    [Table("ProductRecipe")]
    [DisplayName("ProductRecipe")]
    public class ProductRecipeDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ProductRecipe")]
        public long Id { get; set; }
    
        [Column("ProductId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_ProductRecipe_Product")]
        [Association("Product", "ProductId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ProductId { get; set; }

        [Column("UnitId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_ProductRecipe_Units")]
        [Association("Units", "UnitId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> UnitId { get; set; }

        [Column("Qty", Order = 4, TypeName = "FLOAT")]
        public Nullable<double> Qty { get; set; }

        [Column("Price", Order = 5, TypeName = "MONEY")]
        public Nullable<decimal> Price { get; set; }

        [Column("IsProduct", Order = 6, TypeName = "TINYINT")]
        public Nullable<byte> IsProduct { get; set; }

        [Column("MinQty", Order = 7, TypeName = "FLOAT")]
        public Nullable<double> MinQty { get; set; }

        [Column("MaxQty", Order = 8, TypeName = "FLOAT")]
        public Nullable<double> MaxQty { get; set; }

        [Column("IngredientId", Order = 9, TypeName = "BIGINT")]
        [ForeignKey("FK_ProductRecipe_Ingredients")]
        [Association("Ingredients", "IngredientId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> IngredientId { get; set; }

        [Column("ItemsId", Order = 10, TypeName = "BIGINT")]
        [ForeignKey("FK_ProductRecipe_Items")]
        [Association("Items", "ItemsId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ItemsId { get; set; }

        [Column("ProductAsIngridientId", Order = 11, TypeName = "BIGINT")]
        public Nullable<long> ProductAsIngridientId { get; set; }

        [Column("DefaultQty", Order = 12, TypeName = "FLOAT")]
        public Nullable<double> DefaultQty { get; set; }

        [Column("Sort", Order = 13, TypeName = "INT")]
        public Nullable<int> Sort { get; set; }

        /// <summary>
        /// ID for Delivery Agent
        /// </summary>
        [Column("DAId", Order = 14, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }

        [Column("CanSavePrice", Order = 15, TypeName = "BIT")]
        public Nullable<bool> CanSavePrice { get; set; }
    }
}
