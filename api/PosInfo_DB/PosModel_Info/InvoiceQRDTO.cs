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
    [Table("InvoiceQR")]
    [DisplayName("InvoiceQR")]
    public class InvoiceQRDTO : ITables
    {
        // autoincrement id
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_InvoiceQR")]
        public long Id { get; set; }

        // related invoice id
        [Column("InvoiceId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_InvoiceQR_InvoiceId")]
        [Association("Invoices", "InvoiceId", "Id")]
        public long InvoiceId { get; set; }

        // qr code image byte array
        [Column("QR", Order = 3, TypeName = "IMAGE")]
        public byte[] QR { get; set; }
    }
}
