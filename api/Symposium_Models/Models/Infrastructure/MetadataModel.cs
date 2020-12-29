using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Infrastructure
{
    public class MetadataModel
    {
        public int iref { get; set; }
        public int mpehotel { get; set; }
        public int Ref { get; set; }
        public int type { get; set; }
        public string xkey { get; set; }
        public string data { get; set; }
        public string udata { get; set; }
        public int ldata { get; set; }
        public DateTime ddata { get; set; }
        public DateTime validfor { get; set; }
    }
}
