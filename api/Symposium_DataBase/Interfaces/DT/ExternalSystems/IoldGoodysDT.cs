using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.ExternalSystems
{
  public  interface IoldGoodysDT
    {
        long getCustomerbyExtId(DBInfoModel dbinfo, string ExtId, int k);
        long DeleteAddress(DBInfoModel dbInfo, long Id, long CustomerId);
        long getAddressbyExtId(DBInfoModel dbInfo, string extId, int k);

    }
}
