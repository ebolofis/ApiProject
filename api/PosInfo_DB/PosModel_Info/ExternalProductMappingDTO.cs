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
    [Table("ExternalProductMapping")]
    [DisplayName("ExternalProductMapping")]
    public class ExternalProductMappingDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ExternalProductMapping")]
        public long Id { get; set; }

        [Column("ProductId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_ExternalProductMapping_Product")]
        [Association("Product", "ProductId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [DisplayFormatAttribute(DataFormatString = "DF_ExternalProductMapping_ProductId", NullDisplayText = "NULL")]//DefaultValue (Name, Value)
        public Nullable<long> ProductId { get; set; }

        [Column("ProductEnumType", Order = 3, TypeName = "INT")]
        [DisplayFormatAttribute(DataFormatString = "DF_ExternalProductMapping_ProductEnumType", NullDisplayText = "NULL")]//DefaultValue (Name, Value)
        public Nullable<int> ProductEnumType { get; set; }
    }
}
