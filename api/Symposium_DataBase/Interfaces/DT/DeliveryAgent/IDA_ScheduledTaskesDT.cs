using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_ScheduledTaskesDT
    {
        /// <summary>
        /// Return's List of records to update Store
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        List<RecordsForUpdateStoreModel> GetListDataToUpdateFromServer(DBInfoModel Store, out List<RecordsForUpdateStoreModel> Deleted, long? ClientId);
    }
}
