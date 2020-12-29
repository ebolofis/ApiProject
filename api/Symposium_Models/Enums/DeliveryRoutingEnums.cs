using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    /// <summary>
    /// delivery routing assignment to staff status
    /// </summary>
    public enum DeliveryRoutingAssignStatusEnum
    {
        // used when autoassignroutetostaff enabled and async method searches for proper staff to assign route
        searchingStaff = 0,
        // auto assignment has sent message to staffs mobile client and expects response
        pendingResponse = 1,
        // auto assignment has been accepted from staffs mobile client
        staffAccepted = 2,
        // auto assignment has been rejected from staffs mobile client
        staffRejected = 3,
        // auto assignment failed to find staff to assign route
        autoAssignStaffFailure = 4
    }
}
