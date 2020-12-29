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
    [Table("Kitchen")]
    [DisplayName("Kitchen")]
    public class KitchenDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Kitchen")]
        public long Id { get; set; }

        [Column("Code", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("Description", Order = 3, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("Status", Order = 4, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("IsDeleted", Order = 5, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }
    }
}
