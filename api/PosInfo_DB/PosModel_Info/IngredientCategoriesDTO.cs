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
    [Table("IngredientCategories")]
    [DisplayName("IngredientCategories")]
    public class IngredientCategoriesDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_IngredientCategories")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("Status", Order = 3, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("Code", Order = 4, TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("DAId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }

        [Column("IsUnique", Order = 6, TypeName = "BIT")]
        public Nullable<bool> IsUnique { get; set; }
    }
}
