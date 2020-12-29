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
    [Table("Version")]
    [DisplayName("Version")]
    public class VersionsDTO
    {
        [Column("Version", Order = 1, TypeName = "NVARCHAR(50)")]
        [Key]
        [DisplayName("PK_Version")]
        public string Version { get; set; }

        [Column("ReportVersion", Order = 2, TypeName = "NVARCHAR(50)")]
        public string ReportVersion { get; set; }
    }
}
