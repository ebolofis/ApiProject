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
    [Table("PricelistDetail")]
    [DisplayName("PricelistDetail")]
    public class PricelistDetailDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PricelistDetail")]
        public long Id { get; set; }

        [Column("PricelistId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PricelistDetail_Pricelist")]
        [Association("Pricelist", "PricelistId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MinLength(1)]
        public Nullable<long> PricelistId { get; set; }

        [Column("ProductId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_PricelistDetail_Product")]
        [Association("Product", "ProductId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ProductId { get; set; }

        [Column("Price", Order = 4, TypeName = "MONEY")]
        public Nullable<decimal> Price { get; set; }

        [Column("VatId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_PricelistDetail_Vat")]
        [Association("Vat", "VatId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> VatId { get; set; }

        [Column("TaxId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_PricelistDetail_Tax")]
        [Association("Tax", "TaxId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> TaxId { get; set; }

        [Column("IngredientId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_PricelistDetail_Ingredients")]
        [Association("Ingredients", "IngredientId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> IngredientId { get; set; }

        [Column("PriceWithout", Order = 8, TypeName = "MONEY")]
        public Nullable<decimal> PriceWithout { get; set; }

        [Column("MinRequiredExtras", Order = 9, TypeName = "INT")]
        public Nullable<int> MinRequiredExtras { get; set; }

        [Column("DAId", Order = 10, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
