using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_OpeningHoursFlows : IDA_OpeningHoursFlows
    {
        IDA_OpeningHoursTasks openinghourstasks;
        protected ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DA_OpeningHoursFlows(IDA_OpeningHoursTasks _openinghourstasks)
        {
            this.openinghourstasks = _openinghourstasks;
        }

        public List<DA_OpeningHoursModel> GetHours(DBInfoModel dbInfo)
        {
              return openinghourstasks.GetHours(dbInfo);
        }

        public void SaveForStore(DBInfoModel dbInfo,List<DA_OpeningHoursModel> hourslist)
        {
             openinghourstasks.SaveForStore(dbInfo,hourslist);
        }

        public void SaveForAllStores(DBInfoModel Store, List<DA_OpeningHoursModel> hourslist)
        {
            openinghourstasks.SaveForAllStores(Store, hourslist);
        }
        public bool  CheckDA_OpeningHours(DBInfoModel Store, DateTime date, long StoreId)
        {
            return openinghourstasks.CheckDA_OpeningHours(Store, date, StoreId);
        }
    }
}
