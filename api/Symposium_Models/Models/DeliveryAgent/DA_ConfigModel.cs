using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_ConfigModel
    {
        public string DA_BaseUrl { get; set; }

        public string DA_StaffUserName { get; set; }

        public string DA_StaffPassword { get; set; }
    }

    public class DA_GetStorePosModel
    {
       public string StoreId { get; set; }

       public long PosId { get; set; }
    }
}
