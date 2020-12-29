using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class PredefinedCreditsModelsPreview
    {
        public List<PredefinedCreditsModel> PredefinedCreditsModel { get; set; }
    }

    public class PredefinedCreditsModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<long> InvoiceTypeId { get; set; }
    }
}
