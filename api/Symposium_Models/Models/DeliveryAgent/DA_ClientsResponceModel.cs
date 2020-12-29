using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_ClientsResponceModel
    {
        /// <summary>
        /// DA_Order Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Agents Id from table DA_Orders field AgentNo
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// DA_Order Origin Agent, Delivery....
        /// </summary>
        public DA_OrderOriginEnum Origin { get; set; }

        /// <summary>
        /// true: Order posted on client
        /// false:Order failed to post
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if not succeded
        /// </summary>
        public string Error { get; set; }
    }
}
