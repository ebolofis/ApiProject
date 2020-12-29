using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class Invoice_Guests_TransModel
    {
        public Nullable<long> Id { get; set; }

        public Nullable<long> InvoiceId { get; set; }

        public Nullable<long> GuestId { get; set; }

        public Nullable<long> TransactionId { get; set; }

        public Nullable<long> DeliveryCustomerId { get; set; }
    }
}
