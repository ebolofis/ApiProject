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
    [DisplayName("Categories")]
    [Table("Categories")]
    public class CategoriesDTO:ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Categories")]
        public virtual long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(500)")]
        public string Description { get; set; }

        [Column("Status", Order = 3, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }

        [Column("DAId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
