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
    [Table("PageButtonDetail")]
    [DisplayName("PageButtonDetail")]
    public class PageButtonDetailDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PageProductsDetail")]
        public long Id { get; set; }

        [Column("ProductId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> ProductId { get; set; }

        [Column("IsRequired", Order = 3, TypeName = "BIT")]
        public Nullable<bool> IsRequired { get; set; }

        [Column("MinQty", Order = 4, TypeName = "FLOAT")]
        public Nullable<double> MinQty { get; set; }

        [Column("MaxQty", Order = 5, TypeName = "FLOAT")]
        public Nullable<double> MaxQty { get; set; }

        [Column("Description", Order = 6, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("AddCost", Order = 7, TypeName = "MONEY")]
        public Nullable<decimal> AddCost { get; set; }

        [Column("RemoveCost", Order = 8, TypeName = "MONEY")]
        public Nullable<decimal> RemoveCost { get; set; }

        [Column("Type", Order = 9, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("Sort", Order = 10, TypeName = "SMALLINT")]
        public Nullable<short> Sort { get; set; }

        [Column("PageButtonId", Order = 11, TypeName = "BIGINT")]
        [ForeignKey("FK_PageButtonDetail_PageButton")]
        [Association("PageButton", "PageButtonId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> PageButtonId { get; set; }

        [Column("Qty", Order = 12, TypeName = "FLOAT")]
        public Nullable<double> Qty { get; set; }

        [Column("DAId", Order = 13, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }

    }
}
