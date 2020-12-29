using Symposium.Models.Models.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_SchedulerJob.Model
{
    /// <summary>
    /// Timer model. Hold's Jobs to execute.
    /// </summary>
    public class TimerModel
    {
        /// <summary>
        /// Class to execute (Flow, Task, DT .....)
        /// From this class execure void Start()
        /// </summary>
        public object ExecuteClass { get; set; }

        /// <summary>
        /// If Job is currently running then true else false.
        /// If false timer is going to execure it again.
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Next execution for job.
        /// After Job completed updated to new execution time 
        /// </summary>
        public DateTime NextExecution { get; set; }

        /// <summary>
        /// Cron counter for next execute
        /// </summary>
        public string cron { get; set; }

        /// <summary>
        /// Name of Job for Log File
        /// </summary>
        public string JobName { get; set; }

        public ParametersSchedulerModel StartParams { get; set; } = null;
    }
}
