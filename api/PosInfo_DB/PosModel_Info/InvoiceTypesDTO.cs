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
    [Table("InvoiceTypes")]
    [DisplayName("InvoiceTypes")]
    public class InvoiceTypesDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_InvoiceTypes")]
        public long Id { get; set; }

        [Column("Code", Order = 2, TypeName = "NVARCHAR(15)")]
        public string Code { get; set; }

        [Column("Abbreviation", Order = 3, TypeName = "NVARCHAR(50)")]
        public string Abbreviation { get; set; }

        [Column("Description", Order = 4, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("Type", Order = 5, TypeName = "SMALLINT")]
        public Nullable<short> Type { get; set; }

        [Column("IsDeleted", Order = 6, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }
    }
}
