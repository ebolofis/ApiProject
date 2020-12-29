using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class UpsertListResultModel
    {
        /// <summary>
        /// List of items to inserted, updated or deleted on data base
        /// </summary>
        public List<UpsertResultsModel> Results { get; set; }

        /// <summary>
        /// Total records for database action
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Total succeded records for database action
        /// </summary>
        public int TotalSucceded { get; set; }

        /// <summary>
        /// Total failed records for database action
        /// </summary>
        public int TotalFailed { get; set; }

        /// <summary>
        /// Total inserted records
        /// </summary>
        public int TotalInserted { get; set; }

        /// <summary>
        /// Total updated records
        /// </summary>
        public int TotalUpdated { get; set; }

        /// <summary>
        /// Total deleted records
        /// </summary>
        public int TotalDeleted { get; set; }

    }

    /// <summary>
    /// Model for Item Result
    /// </summary>
    public class UpsertResultsModel
    {
        /// <summary>
        /// Id for table DA_ScheduledTaskes
        /// </summary>
        public long MasterID { get; set; }

        /// <summary>
        /// Delivery Agent Id
        /// </summary>
        public long DAID { get; set; }

        /// <summary>
        /// Client Id
        /// -1 not exists on client
        /// </summary>
        public long ClientID { get; set; }

        /// <summary>
        /// -1 Unknown, 0 -> Insert, 1-> Update, 2 -> Delete
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// true if succeded action to database
        /// </summary>
        public bool Succeded { get; set; }

        /// <summary>
        /// Reson not succeded action to database
        /// </summary>
        public string ErrorReason { get; set; }

        /// <summary>
        /// Store Id. saved on DA_Stores on Delivary Agent
        /// </summary>
        public long StoreId { get; set; }
    }
}
