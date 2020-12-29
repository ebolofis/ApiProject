using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class PagesModelsPreview
    {
        public List<PagesModel> PagesModel { get; set; }
    }

    public class PagesModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string ExtendedDesc { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<short> Sort { get; set; }
        public Nullable<long> DefaultPriceList { get; set; }
        public Nullable<long> PageSetId { get; set; }
        public Nullable<long> DAId { get; set; }
    }
}
