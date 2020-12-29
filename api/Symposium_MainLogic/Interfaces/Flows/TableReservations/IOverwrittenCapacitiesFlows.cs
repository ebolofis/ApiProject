using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations
{
    public interface IOverwrittenCapacitiesFlows
    {
        /// <summary>
        /// Returns the List of Overwritten Capacities
        /// </summary>
        /// <returns></returns>
        OverwrittenCapacitiesListModel GetOverwrittenCapacities(DBInfoModel Store);

        /// <summary>
        /// Get details for a specific Overwritten Capacity  
        /// </summary>
        /// <param name="Id">OverwrittenCapacityID</param>
        /// <returns></returns>
        OverwrittenCapacitiesModel GetOverwrittenCapacityById(DBInfoModel Store, long Id);

        /// <summary>
        /// Insert new OverwrittenCapacity
        /// </summary>
        /// <returns></returns>
        long insertOverwrittenCapacity(DBInfoModel Store, OverwrittenCapacitiesModel model);

        /// <summary>
        /// Update an OverwrittenCapacity
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        OverwrittenCapacitiesModel UpdateOverwrittenCapacity(DBInfoModel Store, OverwrittenCapacitiesModel Model);

        /// <summary>
        /// Delete an OverwrittenCapacity
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteOverwrittenCapacity(DBInfoModel Store, long Id);
    }
}
