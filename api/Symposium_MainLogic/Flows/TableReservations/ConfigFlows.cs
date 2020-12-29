using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.TableReservations
{
    public class ConfigFlows : IConfigFlows
    {
        IConfigTasks ConfigTasks;
        public ConfigFlows(IConfigTasks conTasks)
        {
            this.ConfigTasks = conTasks;
        }

        /// <summary>
        /// Returns the Config
        /// </summary>
        /// <returns></returns>
        public ConfigModel GetConfig(DBInfoModel dbInfo)
        {
            // get the results
            ConfigModel configDetails = ConfigTasks.GetConfig(dbInfo);

            return configDetails;
        }

        /// <summary>
        /// Insert new Config
        /// </summary>
        /// <returns></returns>
        public long insertConfig(DBInfoModel dbInfo, ConfigModel model)
        {
            return ConfigTasks.insertConfig(dbInfo, model);
        }

        /// <summary>
        /// Update a Config
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ConfigModel UpdateConfig(DBInfoModel dbInfo, ConfigModel Model)
        {
            return ConfigTasks.UpdateConfig(dbInfo, Model);
        }

        /// <summary>
        /// Delete a Config
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteConfig(DBInfoModel dbInfo, long Id)
        {
            return ConfigTasks.DeleteConfig(dbInfo, Id);
        }

    }
}
