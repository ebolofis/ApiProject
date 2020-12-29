using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
   public interface IDA_OpeningHoursDT
    {
        List<DA_OpeningHoursModel> GetHours(DBInfoModel moodel);
        void SaveForStore(DBInfoModel Store, List<DA_OpeningHoursModel> hourslist);
        void SaveForAllStores(DBInfoModel Store, List<DA_OpeningHoursModel> hourslist);

        bool CheckDA_OpeningHours(DBInfoModel Store, DateTime date, long StoreId);
    }
}
