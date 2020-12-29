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
    [Table("Product")]
    [DisplayName("Product")]
    public class ProductDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Product")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Description { get; set; }

        [Column("ExtraDescription", Order = 3, TypeName = "NVARCHAR(500)")]
        public string ExtraDescription { get; set; }

        [Column("Qty", Order = 4, TypeName = "FLOAT")]
        public Nullable<double> Qty { get; set; }

        [Column("UnitId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_Product_Units")]
        [Association("Units", "UnitId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> UnitId { get; set; }

        [Column("SalesDescription", Order = 6, TypeName = "NVARCHAR(500)")]
        public string SalesDescription { get; set; }

        [Column("PreparationTime", Order = 7, TypeName = "INT")]
        public Nullable<int> PreparationTime { get; set; }

        [Column("KdsId", Order = 8, TypeName = "BIGINT")]
        [ForeignKey("FK_Product_Kds")]
        [Association("Kds", "KdsId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> KdsId { get; set; }

        [Column("KitchenId", Order = 9, TypeName = "BIGINT")]
        [ForeignKey("FK_Product_Kitchen")]
        [Association("Kitchen", "KitchenId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> KitchenId { get; set; }

        [Column("ImageUri", Order = 10, TypeName = "NVARCHAR(500)")]
        public string ImageUri { get; set; }

        [Column("ProductCategoryId", Order = 11, TypeName = "BIGINT")]
        [ForeignKey("FK_Product_ProductCategories")]
        [Association("ProductCategories", "ProductCategoryId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ProductCategoryId { get; set; }

        [Column("Code", Order = 12, TypeName = "NVARCHAR(150)")]
        public string Code { get; set; }

        [Column("IsCustom", Order = 13, TypeName = "BIT")]
        public Nullable<bool> IsCustom { get; set; }

        [Column("KitchenRegionId", Order = 14, TypeName = "BIGINT")]
        [ForeignKey("FK_Product_KitchenRegion")]
        [Association("KitchenRegion", "KitchenRegionId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> KitchenRegionId { get; set; }

        [Column("IsDeleted", Order = 15, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("IsCombo", Order = 16, TypeName = "BIT")]
        public Nullable<bool> IsCombo { get; set; }

        [Column("IsComboItem", Order = 17, TypeName = "BIT")]
        public Nullable<bool> IsComboItem { get; set; }

        [Column("IsReturnItem", Order = 18, TypeName = "BIT")]
        public Nullable<bool> IsReturnItem { get; set; }

        [Column("DAId", Order = 19, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }

    }
}
