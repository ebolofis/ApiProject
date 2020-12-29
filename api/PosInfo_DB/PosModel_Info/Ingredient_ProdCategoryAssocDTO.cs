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
    [Table("Ingredient_ProdCategoryAssoc")]
    [DisplayName("Ingredient_ProdCategoryAssoc")]
    public class Ingredient_ProdCategoryAssocDTO:ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Ingredient_ProdCategoryAssoc")]
        public long Id { get; set; }

        [Column("IngredientId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> IngredientId { get; set; }

        [Column("ProductCategoryId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> ProductCategoryId { get; set; }

        [Column("Sort", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> Sort { get; set; }

        [Column("DAId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }

        [Column("MinQty", Order = 6, TypeName = "FLOAT")]
        public Nullable<double> MinQty { get; set; }

        [Column("MaxQty", Order = 7, TypeName = "FLOAT")]
        public Nullable<double> MaxQty { get; set; }

        [Column("CanSavePrice", Order = 8, TypeName = "BIT")]
        public Nullable<bool> CanSavePrice { get; set; }
    }
}
