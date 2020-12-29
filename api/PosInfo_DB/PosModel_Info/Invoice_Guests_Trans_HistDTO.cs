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
    [Table("Invoice_Guests_Trans_Hist")]
    [DisplayName("Invoice_Guests_Trans_Hist")]
    public class Invoice_Guests_Trans_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_Invoice_Guests_Trans_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("InvoiceId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> InvoiceId { get; set; }

        [Column("GuestId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> GuestId { get; set; }

        [Column("TransactionId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> TransactionId { get; set; }

        [Column("DeliveryCustomerId", Order = 6, TypeName = "BIGINT")]
        public Nullable<long> DeliveryCustomerId { get; set; }
    }
}
