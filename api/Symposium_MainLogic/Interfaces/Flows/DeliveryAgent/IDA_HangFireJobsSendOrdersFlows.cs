using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_HangFireJobsSendOrdersFlows : ISchedulerJobs
    {
        /// <summary>
        /// Send's orders from DA Server to Client
        /// </summary>
        void DA_ServerOrder();

    }
}
