using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Infrastructure
{
    public interface IGuestFutureDT
    {
        List<GuestFutureModel> GetAllGuestFuture(DBInfoModel Store);
    }
}
