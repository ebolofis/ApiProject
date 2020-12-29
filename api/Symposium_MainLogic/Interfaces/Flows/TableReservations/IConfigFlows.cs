using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations
{
    public interface IConfigFlows
    {
        /// <summary>
        /// Get the Config
        /// </summary>
        /// <returns></returns>
        ConfigModel GetConfig(DBInfoModel Store);

        /// <summary>
        /// Insert new Config
        /// </summary>
        /// <returns></returns>
        long insertConfig(DBInfoModel Store, ConfigModel model);

        /// <summary>
        /// Update a Config
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ConfigModel UpdateConfig(DBInfoModel Store, ConfigModel Model);

        /// <summary>
        /// Delete a Config
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteConfig(DBInfoModel Store, long Id);
    }
}
