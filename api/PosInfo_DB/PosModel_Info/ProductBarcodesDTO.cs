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
    [Table("ProductBarcodes")]
    [DisplayName("ProductBarcodes")]
    public class ProductBarcodesDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ProductBarcodes")]
        public long Id { get; set; }

        [Column("Barcode", Order = 2, TypeName = "NVARCHAR(250)")]
        public string Barcode { get; set; }

        [Column("ProductId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_ProductBarcodes_Product")]
        [Association("Product", "ProductId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> ProductId { get; set; }

        [Column("Type", Order = 4, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        /// <summary>
        /// ID for Delivery Agent
        /// </summary>
        [Column("DAId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
