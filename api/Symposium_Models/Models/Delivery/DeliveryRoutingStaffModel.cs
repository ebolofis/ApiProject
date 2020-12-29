using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Delivery
{
    public class DeliveryRoutingStaffModel
    {
        public long id { get; set; }
        public string name { get; set; }
    }

    public class DeliveryRoutingStaffCredentialsModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class DeliveryRoutingChangeStaffModel
    {
        public long Id { get; set; }
        public long OldStaffId { get; set; }
        // Routing Model with new staff id
        public Routing3Model Routing { get; set; }
    }
}
