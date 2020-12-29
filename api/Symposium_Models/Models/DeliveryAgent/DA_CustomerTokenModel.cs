using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_CustomerTokenModel
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string Token { get; set; }
    }

    public class DATokenModel
    {
        public long CustomerId { get; set; }
        public string Token { get; set; }
    }

}
