using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.ExternalSystems
{
    public class OldGoodysTask : IOldGoodysTask
    {

        IoldGoodysDT dt;

        public OldGoodysTask(IoldGoodysDT _dt)
        {
           dt = _dt;
        }

        public long DeleteAddress(DBInfoModel dbInfo, long Id, long CustomerId)
        {
           return dt.DeleteAddress(dbInfo, Id, CustomerId);
        }

        public long getAddressbyExtId(DBInfoModel dbInfo, string extId, int k)
        {
          return   dt.getAddressbyExtId(dbInfo, extId, k);
        }

        public long getCustomerbyExtId(DBInfoModel dbinfo, string ExtId, int k)
        {
           return dt.getCustomerbyExtId(dbinfo, ExtId, k);
        }


        }
    }