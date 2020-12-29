using Symposium.Models.Models.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_HangFireJobsUpdateClientTableFlows : ISchedulerJobs
    {
        /// <summary>
        /// Update's client tables such as product, price list from DA Server to client
        /// </summary>
        void DA_UpdateClientTables(ParametersSchedulerModel Parameters = null);
    }
}
