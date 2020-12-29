using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class InvoiceShippingDetailsExtModel : InvoiceShippingDetailsModel
    {
        public string bell { get; set; }
    }

    public class InvoiceShippingDetailsModel
    {
        public long Id { get; set; }

        public Nullable<long> InvoicesId { get; set; }

        public Nullable<long> ShippingAddressId { get; set; }

        public string ShippingAddress { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingZipCode { get; set; }

        public Nullable<long> BillingAddressId { get; set; }

        public string BillingAddress { get; set; }

        public string BillingCity { get; set; }

        public string BillingZipCode { get; set; }

        public string BillingName { get; set; }

        public string BillingVatNo { get; set; }

        public string BillingDOY { get; set; }

        public string BillingJob { get; set; }

        public string Floor { get; set; }

        public string CustomerRemarks { get; set; }

        public string StoreRemarks { get; set; }

        public Nullable<long> CustomerID { get; set; }

        public string CustomerName { get; set; }

        public Nullable<double> Longtitude { get; set; }

        public Nullable<double> Latitude { get; set; }

        public string Phone { get; set; }
    }
}
