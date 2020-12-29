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
    [Table("Invoice_Guests_Trans")]
    [DisplayName("Invoice_Guests_Trans")]
    public class Invoice_Guests_TransDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Invoice_Guests_Trans")]
        public long Id { get; set; }

        [Column("InvoiceId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_Invoice_Guests_Trans_Invoices")]
        [Association("Invoices", "InvoiceId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> InvoiceId { get; set; }

        [Column("GuestId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_Invoice_Guests_Trans_Guest")]
        [Association("Guest", "GuestId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> GuestId { get; set; }

        [Column("TransactionId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_Invoice_Guests_Trans_Transactions")]
        [Association("Transactions", "TransactionId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> TransactionId { get; set; }

        [Column("DeliveryCustomerId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_Invoice_Guests_Trans_Delivery_Customers")]
        [Association("Delivery_Customers", "DeliveryCustomerId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> DeliveryCustomerId { get; set; }
    }
}
