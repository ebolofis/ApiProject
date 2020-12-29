using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    /// <summary>
    /// Model returned after Invert new invoice
    /// </summary>
    public class InsertNewInvoiceModel
    {
        public string ExtecrName { get; set; }

        public long InvoiceId { get; set; }
    }
}
