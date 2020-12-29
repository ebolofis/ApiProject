using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.DeliveryAgent
{
    public class DA_OpeningHoursTasks : IDA_OpeningHoursTasks
    {
        IDA_OpeningHoursDT openinghoursdt;
        public  DA_OpeningHoursTasks(IDA_OpeningHoursDT _openinghoursdt)
            {
            this.openinghoursdt = _openinghoursdt;
        }
        public  List<DA_OpeningHoursModel> GetHours(DBInfoModel dbInfo)
        {
        return openinghoursdt.GetHours(dbInfo);
        }

       public void SaveForStore(DBInfoModel dbInfo,List<DA_OpeningHoursModel> hourslist)
        {
             openinghoursdt.SaveForStore(dbInfo, hourslist);
        }

        public void SaveForAllStores(DBInfoModel Store, List<DA_OpeningHoursModel> hourslist)
        {
            openinghoursdt.SaveForAllStores(Store, hourslist);
        }

        public bool CheckDA_OpeningHours(DBInfoModel Store, DateTime date, long StoreId)
        {
            return openinghoursdt.CheckDA_OpeningHours(Store, date, StoreId);
        }
    }
}
