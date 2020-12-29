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
    [Table("Department")]
    [DisplayName("Department")]
    public class DepartmentDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Department")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("IsDeleted", Order = 3, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }
    }
}
