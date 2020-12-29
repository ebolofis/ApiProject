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
    [Table("BoardMeals")]
    [DisplayName("BoardMeals")]
    public class BoardMealsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_BoardMeals")]
        public long Id { get; set; }

        [Column("BoardId", Order = 2, TypeName = "NVARCHAR(50)")]
        public string BoardId { get; set; }

        [Column("BoardDescription", Order = 3, TypeName = "NVARCHAR(100)")]
        public string BoardDescription { get; set; }

        [Column("ProductId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_BoardMeals_Product")]
        [Association("Product", "ProductId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ProductId { get; set; }

        [Column("MealsQty", Order = 5, TypeName = "INT")]
        public Nullable<int> MealsQty { get; set; }

        [Column("BoardCode", Order = 6, TypeName = "NVARCHAR(50)")]
        public string BoardCode { get; set; }
    }
}
