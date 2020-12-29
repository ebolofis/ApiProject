using Symposium.WebApi.MainLogic.Interfaces.Flows.Infrastructure;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Infrastructure
{
    public class GuestFutureFlows : IGuestFutureFlows
    {
        IGuestFutureTasks guestFutureTasks;

        public GuestFutureFlows(IGuestFutureTasks _guestFutureTasks)
        {
            this.guestFutureTasks = _guestFutureTasks;
        }

    }
}
