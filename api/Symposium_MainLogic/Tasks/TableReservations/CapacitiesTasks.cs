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
    public class CapacitiesTasks : ICapacitiesTasks
    {
        ICapacitiesDT capacitiesDT;
        public CapacitiesTasks(ICapacitiesDT capDT)
        {
            this.capacitiesDT = capDT;
        }

        /// <summary>
        /// Returns the List of Capacities
        /// </summary>
        /// <returns></returns>
        public CapacitiesListModel GetCapacities(DBInfoModel Store)
        {
            // get the results
            CapacitiesListModel capacitiesDetails = capacitiesDT.GetCapacities(Store);

            return capacitiesDetails;
        }


        /// <summary>
        /// Returns the List of Capacities by RestaurantID 
        /// </summary>
        /// <returns></returns>
        public CapacitiesListModel GetCapacitiesByRestId(DBInfoModel Store, long RestId)
        {
            // get the results
            CapacitiesListModel capacitiesDetails = capacitiesDT.GetCapacitiesByRestId(Store , RestId);

            return capacitiesDetails;
        } 

        /// <summary>
        /// Returns details for a specific Capacity 
        /// </summary>
        /// <param name="Id">CapacityID</param>
        /// <returns></returns>
        public CapacitiesModel GetCapacityById(DBInfoModel Store, long Id)
        {
            // get the results
            CapacitiesModel capacityDetails = capacitiesDT.GetCapacityById(Store, Id);

            return capacityDetails;
        }

        /// <summary>
        /// Insert new Capacity
        /// </summary>
        /// <returns></returns>
        public long insertCapacity(DBInfoModel Store, CapacitiesModel model)
        {
            return capacitiesDT.insertCapacity(Store, model);
        }

        /// <summary>
        /// Update a Capacity
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public CapacitiesModel UpdateCapacity(DBInfoModel Store, CapacitiesModel Model)
        {
            return capacitiesDT.UpdateCapacity(Store, Model);
        }

        /// <summary>
        /// Delete a Capacity
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteCapacity(DBInfoModel Store, long Id)
        {
            return capacitiesDT.DeleteCapacity(Store, Id);
        }
    }
}
