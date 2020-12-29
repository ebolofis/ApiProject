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
    [Table("ComboDetail")]
    [DisplayName("ComboDetail")]
    public class ComboDetailDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ComboDetail")]
        public long Id { get; set; }

        [Column("ComboId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_ComboDetail_Combo")]
        [Association("Combo", "ComboId", "Id")]
        public long ComboId { get; set; }

        [Column("ProductComboItemId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_ComboDetail_Product")]
        [Association("Product", "ProductComboItemId", "Id")]
        public long ProductComboItemId { get; set; }
    }
}
