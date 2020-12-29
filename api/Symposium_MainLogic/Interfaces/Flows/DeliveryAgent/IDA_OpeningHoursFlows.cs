using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_OpeningHoursFlows
    {
        List<DA_OpeningHoursModel> GetHours(DBInfoModel dbInfo);

        void SaveForStore(DBInfoModel dbInfo,List<DA_OpeningHoursModel> hourslist);

        void SaveForAllStores(DBInfoModel Store, List<DA_OpeningHoursModel> hourslist);
        bool CheckDA_OpeningHours(DBInfoModel Store, DateTime date, long StoreId);
    }
}
