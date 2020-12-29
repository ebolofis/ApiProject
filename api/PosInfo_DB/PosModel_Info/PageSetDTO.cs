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
    [Table("PageSet")]
    [DisplayName("PageSet")]
    public class PageSetDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PageSet")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("ActivationDate", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> ActivationDate { get; set; }

        [Column("DeactivationDate", Order = 4, TypeName = "DATETIME")]
        public Nullable<System.DateTime> DeactivationDate { get; set; }

        [Column("DAId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
