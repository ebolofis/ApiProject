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
    [Table("ExternalStateMatch")]
    [DisplayName("ExternalStateMatch")]
    public class ExternalStateMatchDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ExternalStateMatch")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Description { get; set; }

        [Column("Status", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> Status { get; set; }

        [Column("ExtType", Order = 4, TypeName = "INT")]
        [DisplayFormatAttribute(DataFormatString = "DF_ExternalStateMatch_ExtType", NullDisplayText = "NULL")]//DefaultValue (Name, Value)
        public Nullable<int> ExtType { get; set; }

        [Column("MatchValue", Order = 5, TypeName = "INT")]
        [DisplayFormatAttribute(DataFormatString = "DF_ExternalStateMatch_MatchValue", NullDisplayText = "NULL")]//DefaultValue (Name, Value)
        public Nullable<int> MatchValue { get; set; }
    }
}
