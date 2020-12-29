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
    [Table("ProductForBarcodeEod")]
    [DisplayName("ProductForBarcodeEod")]
    public class ProductForBarcodeEodDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ProductForBarcodeEod")]
        public long Id { get; set; }

        [Column("ProductId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_ProductForBarcodeEod_Product")]
        [Association("Product", "ProductId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public long ProductId { get; set; }
    }
}
