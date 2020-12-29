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
    public class ExcludeDaysFlows : IExcludeDaysFlows
    {
        IExcludeDaysTasks ExcludeDaysTasks;
        public ExcludeDaysFlows(IExcludeDaysTasks ExTasks)
        {
            this.ExcludeDaysTasks = ExTasks;
        }

        /// <summary>
        /// Returns the List of ExcludeDays
        /// </summary>
        /// <returns></returns>
        public ExcludeDaysListModel GetExcludeDays(DBInfoModel Store)
        {
            // get the results
            ExcludeDaysListModel ExcludeDaysDetails = ExcludeDaysTasks.GetExcludeDays(Store);

            return ExcludeDaysDetails;
        }

        /// <summary>
        /// Returns details for a specific ExcludeDay 
        /// </summary>
        /// <param name="Id">ExcludeDayID</param>
        /// <returns></returns>
        public ExcludeDaysModel GetExcludeDayById(DBInfoModel Store, long Id)
        {
            // get the results
            ExcludeDaysModel excludeDayDetails = ExcludeDaysTasks.GetExcludeDayById(Store, Id);

            return excludeDayDetails;
        }

        /// <summary>
        /// Insert new ExcludeDay
        /// </summary>
        /// <returns></returns>
        public long insertExcludeDay(DBInfoModel Store, ExcludeDaysModel model)
        {
            return ExcludeDaysTasks.insertExcludeDay(Store, model);
        }

        /// <summary>
        /// Update a ExcludeDay
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ExcludeDaysModel UpdateExcludeDay(DBInfoModel Store, ExcludeDaysModel Model)
        {
            return ExcludeDaysTasks.UpdateExcludeDay(Store, Model);
        }

        /// <summary>
        /// Delete a ExcludeDay
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteExcludeDay(DBInfoModel Store, long Id)
        {
            return ExcludeDaysTasks.DeleteExcludeDay(Store, Id);
        }

        /// <summary>
        /// Deleting old Exclude Days from DB
        /// </summary>
        /// <returns></returns>
        public bool DeleteOldExcludeDays(DBInfoModel Store)
        {
            return ExcludeDaysTasks.DeleteOldExcludeDays(Store);
        }

    }
}
