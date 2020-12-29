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
    public class OverwrittenCapacitiesFlows : IOverwrittenCapacitiesFlows
    {
        IOverwrittenCapacitiesTasks OverwrittenCapacitiesTasks;
        public OverwrittenCapacitiesFlows(IOverwrittenCapacitiesTasks overTasks)
        {
            this.OverwrittenCapacitiesTasks = overTasks;
        }

        /// <summary>
        /// Returns the List of Overwritten Capacities
        /// </summary>
        /// <returns></returns>
        public OverwrittenCapacitiesListModel GetOverwrittenCapacities(DBInfoModel Store)
        {
            // get the results
            OverwrittenCapacitiesListModel overwrittenCapacitiesDetails = OverwrittenCapacitiesTasks.GetOverwrittenCapacities(Store);

            return overwrittenCapacitiesDetails;
        }

        /// <summary>
        /// Returns details for a specific Overwritten Capacity 
        /// </summary>
        /// <param name="Id">OverwrittenCapacityID</param>
        /// <returns></returns>
        public OverwrittenCapacitiesModel GetOverwrittenCapacityById(DBInfoModel Store, long Id)
        {
            // get the results
            OverwrittenCapacitiesModel overwrittenCapacitiesDetails = OverwrittenCapacitiesTasks.GetOverwrittenCapacityById(Store, Id);

            return overwrittenCapacitiesDetails;
        }

        /// <summary>
        /// Insert new OverwrittenCapacity
        /// </summary>
        /// <returns></returns>
        public long insertOverwrittenCapacity(DBInfoModel Store, OverwrittenCapacitiesModel model)
        {
            return OverwrittenCapacitiesTasks.insertOverwrittenCapacity(Store, model);
        }

        /// <summary>
        /// Update an OverwrittenCapacity
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public OverwrittenCapacitiesModel UpdateOverwrittenCapacity(DBInfoModel Store, OverwrittenCapacitiesModel Model)
        {
            return OverwrittenCapacitiesTasks.UpdateOverwrittenCapacity(Store, Model);
        }

        /// <summary>
        /// Delete an OverwrittenCapacity
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteOverwrittenCapacity(DBInfoModel Store, long Id)
        {
            return OverwrittenCapacitiesTasks.DeleteOverwrittenCapacity(Store, Id);
        }
    }
}
