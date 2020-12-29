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
    public class OverwrittenCapacitiesTasks : IOverwrittenCapacitiesTasks
    {
        IOverwrittenCapacitiesDT overwrittenCapacitiesDT;
        public OverwrittenCapacitiesTasks(IOverwrittenCapacitiesDT overDT)
        {
            this.overwrittenCapacitiesDT = overDT;
        }

        /// <summary>
        /// Returns the List of Overwritten Capacities
        /// </summary>
        /// <returns></returns>
        public OverwrittenCapacitiesListModel GetOverwrittenCapacities(DBInfoModel Store)
        {
            // get the results
            OverwrittenCapacitiesListModel overwrittenCapacitiesDetails = overwrittenCapacitiesDT.GetOverwrittenCapacities(Store);

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
            OverwrittenCapacitiesModel overwrittenCapacitiesDetails = overwrittenCapacitiesDT.GetOverwrittenCapacityById(Store, Id);

            return overwrittenCapacitiesDetails;
        }

        /// <summary>
        /// Insert new OverwrittenCapacity
        /// </summary>
        /// <returns></returns>
        public long insertOverwrittenCapacity(DBInfoModel Store, OverwrittenCapacitiesModel model)
        {
            return overwrittenCapacitiesDT.insertOverwrittenCapacity(Store, model);
        }

        /// <summary>
        /// Update an OverwrittenCapacity
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public OverwrittenCapacitiesModel UpdateOverwrittenCapacity(DBInfoModel Store, OverwrittenCapacitiesModel Model)
        {
            return overwrittenCapacitiesDT.UpdateOverwrittenCapacity(Store, Model);
        }

        /// <summary>
        /// Delete an OverwrittenCapacity
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteOverwrittenCapacity(DBInfoModel Store, long Id)
        {
            return overwrittenCapacitiesDT.DeleteOverwrittenCapacity(Store, Id);
        }
    }
}
