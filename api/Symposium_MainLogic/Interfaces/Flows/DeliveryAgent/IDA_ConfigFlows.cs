using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_ConfigFlows
    {
        /// <summary>
        /// Get DA Config 
        /// </summary>
        /// <returns></returns>
        DA_ConfigModel GetConfig(DBInfoModel dbInfo);

        /// <summary>
        /// Get StoreId and PosId(The FirstOrDefault PosId)
        /// </summary>
        /// <returns></returns>
        DA_GetStorePosModel GetStorePos(DBInfoModel dbInfo);

        /// <summary>
        /// Is Delivery Agent (true or false)
        /// </summary>
        /// <returns></returns>
        bool isDA();

        /// <summary>
        /// Is Delivery Store (true or false)
        /// </summary>
        /// <returns></returns>
        bool isDAclient();

        /// <summary>
        /// DA Store Id
        /// </summary>
        /// <returns></returns>
        string getDAStoreId();

        /// <summary>
        /// DA cancelable statuses
        /// </summary>
        /// <returns></returns>
        List<string> getDACancel();
    }
}
