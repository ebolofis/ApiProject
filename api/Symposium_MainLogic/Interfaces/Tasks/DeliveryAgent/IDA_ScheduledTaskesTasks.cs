using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_ScheduledTaskesTasks
    {
        /// <summary>
        /// Return's List of records to update Store
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        List<RecordsForUpdateStoreModel> GetListDataToUpdateFromServer(DBInfoModel dbInfo, out List<RecordsForUpdateStoreModel> Deleted, long? ClientId);
    }
}
