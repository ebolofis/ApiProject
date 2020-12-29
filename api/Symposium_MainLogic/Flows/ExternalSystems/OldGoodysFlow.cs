using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.ExternalSystems
{
    public class OldGoodysFlow : IOldGoodysFlow
    {

        IOldGoodysTask oldgoodysTask;
        public OldGoodysFlow(IOldGoodysTask _oldgoodysTask)
        {
            oldgoodysTask = _oldgoodysTask;
        }

        public long DeleteAddress(DBInfoModel dbInfo, long Id, long CustomerId)
        {
            return oldgoodysTask.DeleteAddress(dbInfo,Id, CustomerId);
        }

        public long getAddressbyExtId(DBInfoModel dbInfo, string extId, int k)
        {
         return   oldgoodysTask.getAddressbyExtId(dbInfo, extId, k);
        }

        public long getCustomerbyExtId(DBInfoModel dbinfo, string ExtId, int k)
        {
           return oldgoodysTask.getCustomerbyExtId(dbinfo, ExtId, k);
        }

 
    }
}

