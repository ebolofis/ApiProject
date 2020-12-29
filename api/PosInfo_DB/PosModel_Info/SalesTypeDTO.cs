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
    [Table("SalesType")]
    [DisplayName("SalesType")]
    public class SalesTypeDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        //[Editable(false)]
        [Required]
        [DisplayName("PK_SalesType")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("Abbreviation", Order = 3, TypeName = "NVARCHAR(10)")]
        public string Abbreviation { get; set; }

        [Column("IsDeleted", Order = 4, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("DAId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
