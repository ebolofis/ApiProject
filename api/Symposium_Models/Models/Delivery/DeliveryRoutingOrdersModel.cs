using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Delivery
{
    public class DeliveryRoutingOrdersModel
    {
        public DeliveryRoutingModel DeliveryRouting { get; set; }
        public List<OrderModel> OrdersList { get; set; }
    }

    public class DeliveryRoutingOrdersListModel
    {
        public long? deliveryRoutingId { get; set; }
        public List<long> orderIds { get; set; }
    }

    public class DeliveryRoutingMoveOrdersModel
    {
        public long deliveryRoutingSourceId { get; set; }
        public long? deliveryRoutingTargetId { get; set; }
        public List<long> orderIds { get; set; }
    }

    public class DeliveryRoutingOrdersStatusModel
    {
        public int status { get; set; }
        public List<long> orderIds { get; set; }
    }

    public class DeliveryRoutingNewModel
    {
        public List<long> orderIds { get; set; }
        public long? staffId { get; set; }
    }

    public class DeliveryRoutingIdsListModel
    {
        public List<long> routeIds { get; set; }
    }
}
