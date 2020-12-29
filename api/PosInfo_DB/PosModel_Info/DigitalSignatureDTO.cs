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
    [Table("DigitalSignature")]
    [DisplayName("DigitalSignature")]
    public class DigitalSignature
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DigitalSignature")]
        public long Id { get; set; }

        [Column("InvoiceId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> InvoiceId { get; set; }

        [Column("Images", Order = 3, TypeName = "IMAGE")]
        public Byte[] Images { get; set; }
    }
}
