using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Kds
{
    public class KdsUpdateKitchenStatusRequestModel
    {
        public long LoginStaffId { get; set; }
        public string Storeid { get; set; }
        public long OrderId { get; set; }
        public List<long> KdsIds { get; set; }
        public List<long> SaleTypesIds { get; set; }
        public long OrderDetailId { get; set; }
        public int KitchenStatus { get; set; }
        public int CurrentKitchenStatus { get; set; }
        public long ProductId { get; set; }
    }
}
