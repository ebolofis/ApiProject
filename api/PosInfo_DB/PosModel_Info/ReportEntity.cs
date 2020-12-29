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

    [DisplayName("ReportEntity")]
    public class ReportEntity
    {
        [Column("Id", Order = 1, TypeName = "INT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ReportEntity")]
        public int Id { get; set; }

        [Column("Url", Order = 1, TypeName = "NVARCHAR(100)")]
        [Required]        
        public string Url { get; set; }

        [Column("Layout", Order = 1, TypeName = "VARBINARY(MAX)")]
        [Required]
        public byte[] Layout { get; set; }

        [Column("OptimisticLockField", Order = 1, TypeName = "int")]        
        public Nullable<int> OptimisticLockField { get; set; }

        [Column("ReportName", Order = 1, TypeName = "NVARCHAR(100)")]        
        public string ReportName { get; set; }

        [Column("Menu", Order = 1, TypeName = "NVARCHAR(50)")]
        public string Menu { get; set; }

        [Column("Submenu", Order = 1, TypeName = "NVARCHAR(50)")]
        public string Submenu { get; set; }

    }
}
