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
    [Table("TransferMappingDetails_Hist")]
    [DisplayName("TransferMappingDetails_Hist")]
    public class TransferMappingDetails_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_dbo.TransferMappingDetails_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("TransferMappingsId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> TransferMappingsId { get; set; }

        [Column("ProductCategoryId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> ProductCategoryId { get; set; }

        [Column("ProductId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> ProductId { get; set; }
    }
}
