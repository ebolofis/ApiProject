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
    public class DA_ScheduledTaskesFlows : IDA_ScheduledTaskesFlows
    {
        IDA_ScheduledTaskesTasks tasks;

        public DA_ScheduledTaskesFlows(IDA_ScheduledTaskesTasks tasks)
        {
            this.tasks = tasks;
        }

        /// <summary>
        /// Return's List of records to update Store
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public List<RecordsForUpdateStoreModel> GetListDataToUpdateFromServer(DBInfoModel dbInfo, out List<RecordsForUpdateStoreModel> Deleted, long? ClientId)
        {
            return tasks.GetListDataToUpdateFromServer(dbInfo, out Deleted, ClientId);
        }

    }
}
