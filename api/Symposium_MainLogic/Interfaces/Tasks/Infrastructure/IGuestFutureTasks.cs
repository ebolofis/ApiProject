using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.Infrastructure
{
    public interface IGuestFutureTasks
    {
        List<GuestFutureModel> GetAllGuestFuture(DBInfoModel Store);
    }
}
