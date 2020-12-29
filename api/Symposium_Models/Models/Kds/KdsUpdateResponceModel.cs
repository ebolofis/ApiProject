using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Kds
{
    public class KdsUpdateResponceModel
    {
        public List<KdsResponceModel> KdsOrdersModel { get; set; }
        
        public bool SendSignalR { get; set; }
    }
}
