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
    [Table("AllowedMealsPerBoard")]
    [DisplayName("AllowedMealsPerBoard")]
    public class AllowedMealsPerBoardDTO
    {

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_AllowedMealsPerBoard")]
        public long Id { get; set; }

        [Column("BoardId", Order = 2, TypeName = "NVARCHAR(150)")]
        public string BoardId { get; set; }

        [Column("BoardDescription", Order = 3, TypeName = "NVARCHAR(350)")]
        public string BoardDescription { get; set; }

        [Column("AllowedMeals", Order = 4, TypeName = "INT")]
        public Nullable<int> AllowedMeals { get; set; }

        [Column("AllowedDiscountAmount", Order = 5, TypeName = "MONEY")]
        public Nullable<decimal> AllowedDiscountAmount { get; set; }

        [Column("IsDeleted", Order = 6, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("PriceListId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_AllowedMealsPerBoard_Pricelist")]
        [Association("Pricelist", "PriceListId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PriceListId { get; set; }

        [Column("AllowedMealsChild", Order = 8, TypeName = "INT")]
        public Nullable<int> AllowedMealsChild { get; set; }

        [Column("AllowedDiscountAmountChild", Order = 9, TypeName = "MONEY")]
        public Nullable<decimal> AllowedDiscountAmountChild { get; set; }
    }
}
