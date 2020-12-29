using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Infrastructure
{
    public class SupplierModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string FullName { get; set; }
        public string VatNo { get; set; }
        public string TaxOffice { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
