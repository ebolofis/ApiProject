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
    [Table("ValidModules")]
    [DisplayName("ValidModules")]
    public class ValidModulesDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ValidModules")]
        public long Id { get; set; }

        [Column("ModuleType", Order = 2, TypeName = "SMALLINT")]
        public Nullable<short> ModuleType { get; set; }

        [Column("MaxInstances", Order = 3, TypeName = "INT")]
        public Nullable<int> MaxInstances { get; set; }
    }
}
