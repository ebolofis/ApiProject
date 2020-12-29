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
    [Table("PageButton")]
    [DisplayName("PageButton")]
    public class PageButtonDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PageProducts")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("PreparationTime", Order = 3, TypeName = "SMALLINT")]
        public Nullable<short> PreparationTime { get; set; }

        [Column("ImageUri", Order = 4, TypeName = "NVARCHAR(500)")]
        public string ImageUri { get; set; }

        [Column("Price", Order = 5, TypeName = "MONEY")]
        public Nullable<decimal> Price { get; set; }

        [Column("PriceListId", Order = 6, TypeName = "BIGINT")]
        public Nullable<long> PriceListId { get; set; }

        [Column("Sort", Order = 7, TypeName = "SMALLINT")]
        public Nullable<short> Sort { get; set; }

        [Column("NavigateToPage", Order = 8, TypeName = "BIGINT")]
        public Nullable<long> NavigateToPage { get; set; }

        [Column("SetDefaultPriceListId", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> SetDefaultPriceListId { get; set; }

        [Column("Type", Order = 10, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("PageId", Order = 11, TypeName = "BIGINT")]
        [ForeignKey("FK_PageButton_Pages")]
        [Association("Pages", "PageId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> PageId { get; set; }

        [Column("Color", Order = 12, TypeName = "NVARCHAR(25)")]
        public string Color { get; set; }

        [Column("Background", Order = 13, TypeName = "NVARCHAR(25)")]
        public string Background { get; set; }

        [Column("ProductId", Order = 14, TypeName = "BIGINT")]
        [ForeignKey("FK_PageButton_Product")]
        [Association("Product", "ProductId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ProductId { get; set; }

        [Column("SetDefaultSalesType", Order = 15, TypeName = "BIGINT")]
        public Nullable<long> SetDefaultSalesType { get; set; }

        [Column("CanSavePrice", Order = 16, TypeName = "BIT")]
        public Nullable<bool> CanSavePrice { get; set; }

        [Column("DAId", Order = 17, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
