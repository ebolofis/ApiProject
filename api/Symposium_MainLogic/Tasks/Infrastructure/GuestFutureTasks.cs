using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;
using Symposium.WebApi.DataAccess.Interfaces.DT.Infrastructure;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.Infrastructure
{
    public class GuestFutureTasks : IGuestFutureTasks
    {
        IGuestFutureDT guestFutureDt;

        public GuestFutureTasks(IGuestFutureDT _guestFutureDt)
        {
            this.guestFutureDt = _guestFutureDt;
        }

        public List<GuestFutureModel> GetAllGuestFuture(DBInfoModel Store)
        {
            return guestFutureDt.GetAllGuestFuture(Store);
        }

    }
}
