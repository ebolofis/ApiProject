using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations
{
    public interface ICapacitiesFlows
    {
        /// <summary>
        /// Get the List of Capacities
        /// </summary>
        /// <returns></returns>
        CapacitiesListModel GetCapacities(DBInfoModel Store);

        /// <summary>
        /// Returns Capacities for a specific Restaurant 
        /// </summary>
        /// <param name="RestId">Restaurant Id</param>
        /// <returns></returns>
        CapacitiesListModel GetCapacitiesByRestId(DBInfoModel Store, long RestId);

        /// <summary>
        /// Get details for a specific Capacity 
        /// </summary>
        /// <param name="Id">CapacityID</param>
        /// <returns></returns>
        CapacitiesModel GetCapacityById(DBInfoModel Store, long Id);

        /// <summary>
        /// Insert new Capacity
        /// </summary>
        /// <returns></returns>
        long insertCapacity(DBInfoModel Store, CapacitiesModel model);

        /// <summary>
        /// Update a Capacity
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        CapacitiesModel UpdateCapacity(DBInfoModel Store, CapacitiesModel Model);

        /// <summary>
        /// Delete a Capacity
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteCapacity(DBInfoModel Store, long Id);
    }
}
