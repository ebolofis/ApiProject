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
    [Table("Ingredients")]
    [DisplayName("Ingredients")]
    public class IngredientsDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Ingredients")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "  NVARCHAR(70)")]
        public string Description { get; set; }

        [Column("ExtendedDescription", Order = 3, TypeName = "NVARCHAR(500)")]
        public string ExtendedDescription { get; set; }

        [Column("SalesDescription", Order = 4, TypeName = "NVARCHAR(500)")]
        public string SalesDescription { get; set; }

        [Column("Qty", Order = 5, TypeName = "FLOAT")]
        public Nullable<double> Qty { get; set; }

        [Column("ItemId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_Ingredients_Ingredients")]
        [Association("Items", "ItemId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ItemId { get; set; }

        [Column("UnitId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_Ingredients_Units")]
        [Association("Units", "UnitId", "Id")]/*Foreign Table, Table Field, Foreign Field*/ 
        public Nullable<long> UnitId { get; set; }

        [Column("Code", Order = 8, TypeName = "NVARCHAR(150)")]
        public string Code { get; set; }

        [Column("IsDeleted", Order = 9, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("Background", Order = 10, TypeName = "NVARCHAR(25)")]
        public string Background { get; set; }

        [Column("Color", Order = 11, TypeName = "NVARCHAR(25)")]
        public string Color { get; set; }

        [Column("IngredientCategoryId", Order = 12, TypeName = "BIGINT")]
        [ForeignKey("FK_Ingredients_IngredientCategories")]
        [Association("IngredientCategories", "IngredientCategoryId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> IngredientCategoryId { get; set; }

        [Column("DAId", Order = 12, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }

        [Column("DisplayOnKds", Order = 13, TypeName = "BIT")]
        public Nullable<bool> DisplayOnKds { get; set; }
    }
}
