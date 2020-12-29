using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Plugins;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_ConfigFlows : IDA_ConfigFlows
    {
        IDA_ConfigTasks configTasks;
        public DA_ConfigFlows(IDA_ConfigTasks _configTasks)
        {
            this.configTasks = _configTasks;
        }

        /// <summary>
        /// Get DA Config 
        /// </summary>
        /// <returns></returns>
        public DA_ConfigModel GetConfig(DBInfoModel dbInfo)
        {
            return configTasks.GetConfig(dbInfo);
        }

        /// <summary>
        /// Get StoreId and PosId(The FirstOrDefault PosId)
        /// </summary>
        /// <returns></returns>
        public DA_GetStorePosModel GetStorePos(DBInfoModel dbInfo)
        {
            DA_GetStorePosModel model = configTasks.GetStorePos(dbInfo);
            if (model.StoreId != null || model.PosId > 0)
            {
                return model;
            }
            else
                throw new BusinessException(Symposium.Resources.Errors.NOSTOREIDORPOSID);
        }

        /// <summary>
        /// Is Delivery Agent (true or false)
        /// </summary>
        /// <returns></returns>
        public bool isDA()
        {
            return configTasks.isDA();
        }

        /// <summary>
        /// Is Delivery Store (true or false)
        /// </summary>
        /// <returns></returns>
        public bool isDAclient()
        {
            return configTasks.isDAclient();
        }

        /// <summary>
        /// DA Store Id
        /// </summary>
        /// <returns></returns>
        public string getDAStoreId()
        {
            return configTasks.getDAStoreId();
        }

        /// <summary>
        /// DA cancelable statuses
        /// </summary>
        /// <returns></returns>
        public List<string> getDACancel()
        {
            return configTasks.getDACancel();
        }
    }
}
