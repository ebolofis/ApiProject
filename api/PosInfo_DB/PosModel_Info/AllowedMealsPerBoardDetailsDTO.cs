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
    [Table("AllowedMealsPerBoardDetails")]
    [DisplayName("AllowedMealsPerBoardDetails")]
    public class AllowedMealsPerBoardDetailsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_AllowedMealsPerBoardDetails")]
        public long Id { get; set; }

        [Column("ProductCategoryId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_AllowedMealsPerBoardDetails_ProductCategories")]
        [Association("ProductCategories", "ProductCategoryId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ProductCategoryId { get; set; }

        [Column("AllowedMealsPerBoardId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_AllowedMealsPerBoardDetails_AllowedMealsPerBoard")]
        [Association("AllowedMealsPerBoard", "AllowedMealsPerBoardId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> AllowedMealsPerBoardId { get; set; }

    }
}
