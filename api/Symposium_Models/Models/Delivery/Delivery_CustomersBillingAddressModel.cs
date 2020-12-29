using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class Delivery_CustomersBillingAddressModel
    {
        public Nullable<long> ID { get; set; }

        public long CustomerID { get; set; }

        public string AddressStreet { get; set; }

        public string AddressNo { get; set; }

        public string City { get; set; }

        public string Zipcode { get; set; }

        public Nullable<int> Type { get; set; }

        public string Latitude { get; set; }

        public string Longtitude { get; set; }

        public string Floor { get; set; }

        public Nullable<bool> IsSelected { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public string ExtKey { get; set; }

        public Nullable<int> ExtType { get; set; }

        public string ExtObj { get; set; }

        public string ExtId1 { get; set; }

        public string ExtId2 { get; set; }

        public string VerticalStreet { get; set; }

        public string Notes { get; set; }
    }
}
