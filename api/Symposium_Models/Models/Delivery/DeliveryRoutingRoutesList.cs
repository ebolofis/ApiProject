using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Delivery
{
    public class DeliveryRoutingRoutesList
    {
        public SortedList<long, long> pendingRoutes { get; set; }

        public DeliveryRoutingRoutesList()
        {
            pendingRoutes = new SortedList<long, long>();
        }
    }
}
