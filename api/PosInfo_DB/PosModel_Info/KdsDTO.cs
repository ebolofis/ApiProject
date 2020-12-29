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
    [Table("Kds")]
    [DisplayName("Kds")]
    public class KdsDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Kds")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("Status", Order = 3, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("PosInfoId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_Kds_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PosInfoId { get; set; }

        [Column("IsDeleted", Order = 5, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }
    }
}
