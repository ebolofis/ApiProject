using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Models
{
    public partial class ExternalProductPricelistDetailMapping
    {
        public virtual IEnumerable<ExternalProductMapping> ExternalProductMapping { get; set; }
        public Nullable<long> VatId { get; set; }
        public Nullable<long> SalesTypeId { get; set; }
    }
}