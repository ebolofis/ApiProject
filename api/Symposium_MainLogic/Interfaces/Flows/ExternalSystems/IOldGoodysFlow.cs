using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems
{
   public  interface IOldGoodysFlow
    {

        long getCustomerbyExtId(DBInfoModel dbinfo, string ExtId,int k);
        long getAddressbyExtId(DBInfoModel dbInfo, string extId, int k);
        long DeleteAddress(DBInfoModel dbInfo, long Id, long CustomerId);
    }
}
