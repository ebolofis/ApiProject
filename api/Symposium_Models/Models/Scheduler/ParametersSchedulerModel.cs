using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Scheduler
{
    /// <summary>
    /// Model to pass parameters on Start() void on Interface ISchedulerJobs.
    /// Default null.
    /// You can add as many parameters you need.
    /// </summary>
    public class ParametersSchedulerModel
    {
        /// <summary>
        /// DA_Store Id (Record for Store to send new changes from Server to Client if field CronScheduler has value
        /// if Value less than 1 (One) then all store are enabled to get changes
        /// </summary>
        public Nullable<long> StoreIdForUpdate { get; set; }

        /// <summary>
        /// Store Id from config json
        /// </summary>
        public Nullable<Guid> StoreId { get; set; }

        /// <summary>
        /// Is delivery or not from config json
        /// </summary>
        public Nullable<bool> isDeliveryAgent { get; set; }

        /// <summary>
        /// Is Store or not from config json
        /// </summary>
        public Nullable<bool> isDeliveryStore { get; set; }

        /// <summary>
        /// Del on hold orders after ... from config json
        /// </summary>
        public Nullable<int> delHour { get; set; }

        /// <summary>
        /// Base Url to send order status to server from config json
        /// </summary>
        public string DA_BaseURL { get; set; }

        /// <summary>
        /// Staff user name to send order status to server from config json
        /// </summary>
        public string DA_Staff_Username { get; set; }

        /// <summary>
        /// Staff Password to send order status to server from config json
        /// </summary>
        public string DA_Staff_Password { get; set; }

        /// <summary>
        /// Time to send order to kitchen for delay orders from config json
        /// </summary>
        public Nullable<int> DA_SendToKitchenTime { get; set; }
    }
}
