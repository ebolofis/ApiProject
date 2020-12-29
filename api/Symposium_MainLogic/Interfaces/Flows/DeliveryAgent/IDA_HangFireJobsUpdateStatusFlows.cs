using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_HangFireJobsUpdateStatusFlows : ISchedulerJobs
    {
        /// <summary>
        /// Update's orders from client to DA Server
        /// </summary>
        void DA_UpdateOrderStatus();
    }
}
