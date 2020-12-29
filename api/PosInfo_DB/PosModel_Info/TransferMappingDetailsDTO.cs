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
    [Table("TransferMappingDetails")]
    [DisplayName("TransferMappingDetails")]
    public class TransferMappingDetailsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_dbo.TransferMappingDetails")]
        public long Id { get; set; }

        [Column("TransferMappingsId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_dbo.TransferMappingDetails_dbo.TransferMappings_TransferMappingsId")]
        [Association("TransferMappings", "TransferMappingsId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> TransferMappingsId { get; set; }

        [Column("ProductCategoryId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> ProductCategoryId { get; set; }

        [Column("ProductId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> ProductId { get; set; }
    }
}
