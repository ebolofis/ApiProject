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
    [Table("ReportList")]
    [DisplayName("ReportList")]
    public class ReportListDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ReportList")]
        public long Id { get; set; }

        [Column("ReportName", Order = 2, TypeName = "NVARCHAR(350)")]
        public string ReportName { get; set; }

        [Column("ReportType", Order = 3, TypeName = "INT")]
        public Nullable<int> ReportType { get; set; }

        [Column("ReportJson", Order = 4, TypeName = "NVARCHAR(MAX)")]
        public string ReportJson { get; set; }

        [Column("AppearsInMenu", Order = 5, TypeName = "BIT")]
        public Nullable<bool> AppearsInMenu { get; set; }

        [Column("DateCreated", Order = 6, TypeName = "DATETIME")]
        public Nullable<System.DateTime> DateCreated { get; set; }

        [Column("Datemodified", Order = 7, TypeName = "DATETIME")]
        public Nullable<System.DateTime> Datemodified { get; set; }

        [Column("Version", Order = 8, TypeName = "NVARCHAR(50)")]
        public string Version { get; set; }
    }
}
