using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.TableReservations
{
    public class ConfigTasks : IConfigTasks
    {
        IConfigDT configDT;
        public ConfigTasks(IConfigDT conDT)
        {
            this.configDT = conDT;
        }

        /// <summary>
        /// Returns the Config
        /// </summary>
        /// <returns></returns>
        public ConfigModel GetConfig(DBInfoModel Store)
        {
            // get the results
            ConfigModel configDetails = configDT.GetConfig(Store);

            return configDetails;
        }

        /// <summary>
        /// Insert new Config
        /// </summary>
        /// <returns></returns>
        public long insertConfig(DBInfoModel Store, ConfigModel model)
        {
            return configDT.insertConfig(Store, model);
        }

        /// <summary>
        /// Update a Config
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ConfigModel UpdateConfig(DBInfoModel Store, ConfigModel Model)
        {
            return configDT.UpdateConfig(Store, Model);
        }

        /// <summary>
        /// Delete a Config
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteConfig(DBInfoModel Store, long Id)
        {
            return configDT.DeleteConfig(Store, Id);
        }
    }
}
