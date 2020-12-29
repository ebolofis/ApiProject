using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_HangFireJobsTasks
    {
        /// <summary>
        /// Send's orders from DA Server to Client
        /// </summary>
        void DA_ServerOrder(DBInfoModel dbInfo, int delMinutes);

        /// <summary>
        /// Update's orders from client to DA Server
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="DA_URL"></param>
        /// <param name="DA_UserName"></param>
        /// <param name="DA_Password"></param>
        /// <param name="ExtType"></param>
        void DA_UpdateOrderStatus(DBInfoModel dbInfo, string DA_URL, string DA_UserName, string DA_Password, ExternalSystemOrderEnum ExtType);

    }

    
}
