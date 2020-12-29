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
    public class CapacitiesFlows : ICapacitiesFlows
    {
        ICapacitiesTasks CapacitiesTasks;
        public CapacitiesFlows(ICapacitiesTasks capTasks)
        {
            this.CapacitiesTasks = capTasks;
        }

        /// <summary>
        /// Returns the List of Capacities
        /// </summary>
        /// <returns></returns>
        public CapacitiesListModel GetCapacities(DBInfoModel dbInfo)
        {
            // get the results
            CapacitiesListModel capacitiesDetails = CapacitiesTasks.GetCapacities(dbInfo);

            return capacitiesDetails;
        }

        /// <summary>
        /// Returns Capacities for a specific Restaurant 
        /// </summary>
        /// <param name="RestId">Restaurant Id</param>
        /// <returns></returns>
        public CapacitiesListModel GetCapacitiesByRestId(DBInfoModel dbInfo, long RestId)
        {
            // get the results
            CapacitiesListModel capacityDetails = CapacitiesTasks.GetCapacitiesByRestId(dbInfo, RestId);

            return capacityDetails;
        }

        /// <summary>
        /// Returns details for a specific Capacity 
        /// </summary>
        /// <param name="Id">CapacityID</param>
        /// <returns></returns>
        public CapacitiesModel GetCapacityById(DBInfoModel dbInfo, long Id)
        {
            // get the results
            CapacitiesModel capacityDetails = CapacitiesTasks.GetCapacityById(dbInfo, Id);

            return capacityDetails;
        }

        /// <summary>
        /// Insert new Capacity
        /// </summary>
        /// <returns></returns>
        public long insertCapacity(DBInfoModel dbInfo, CapacitiesModel model)
        {
            return CapacitiesTasks.insertCapacity(dbInfo, model);
        }

        /// <summary>
        /// Update a Capacity
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public CapacitiesModel UpdateCapacity(DBInfoModel dbInfo, CapacitiesModel Model)
        {
            return CapacitiesTasks.UpdateCapacity(dbInfo, Model);
        }

        /// <summary>
        /// Delete a Capacity
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteCapacity(DBInfoModel dbInfo, long Id)
        {
            return CapacitiesTasks.DeleteCapacity(dbInfo, Id);
        }
    }
}
