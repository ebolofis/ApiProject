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
    [Table("ProductCategories")]
    [DisplayName("ProductCategories")]
    public class ProductCategoriesDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ProductCategories")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("Type", Order = 3, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("Status", Order = 4, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("Code", Order = 5, TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("CategoryId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_ProductCategories_Categories")]
        [Association("Categories", "CategoryId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> CategoryId { get; set; }

        [Column("DAId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
