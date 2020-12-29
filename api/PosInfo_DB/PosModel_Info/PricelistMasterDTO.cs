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
    [Table("PricelistMaster")]
    [DisplayName("PricelistMaster")]
    public class PricelistMasterDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PricelistMaster")]
        public long Id { get; set; }
        
        [Column("Description", Order = 2, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("Status", Order = 3, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("Active", Order = 4, TypeName = "BIT")]
        public Nullable<bool> Active { get; set; }

        [Column("DAId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
