using Symposium.Models.Models.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_SchedulerJob.Model
{
    /// <summary>
    /// Model with elements from da_stores to send updated data from server to stores
    /// </summary>
    public class Da_Stores_ParametersModel
    {
        /// <summary>
        /// da Store Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// da Store Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Timer to send data from Server to Store
        /// </summary>
        public string CronScheduler { get; set; }

    }
}
