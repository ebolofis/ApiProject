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
    [Table("MetadataTable")]
    [DisplayName("MetadataTable")]
    public class MetadataTableDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_MetadataTable")]
        public long Id { get; set; }

        [Column("ReportType", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> ReportType { get; set; }

        [Column("FieldName", Order = 3, TypeName = "NVARCHAR(350)")]
        public string FieldName { get; set; }

        [Column("Description", Order = 4, TypeName = "NVARCHAR(350)")]
        public string Description { get; set; }

        [Column("FieldType", Order = 5, TypeName = "SMALLINT")]
        public Nullable<short> FieldType { get; set; }

        [Column("DefaultStyle", Order = 6, TypeName = "NVARCHAR(350)")]
        public string DefaultStyle { get; set; }

        [Column("Summable", Order = 7, TypeName = "BIT")]
        public Nullable<bool> Summable { get; set; }

        [Column("Expression", Order = 8, TypeName = "NVARCHAR(300)")]
        public string Expression { get; set; }
    }
}
