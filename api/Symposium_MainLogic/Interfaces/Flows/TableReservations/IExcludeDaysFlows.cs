using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations
{
    public interface IExcludeDaysFlows
    {
        /// <summary>
        /// Get the List of ExcludeDays
        /// </summary>
        /// <returns></returns>
        ExcludeDaysListModel GetExcludeDays(DBInfoModel Store);

        /// <summary>
        /// Get details for a specific ExcludeDay 
        /// </summary>
        /// <param name="Id">ExcludeDayID</param>
        /// <returns></returns>
        ExcludeDaysModel GetExcludeDayById(DBInfoModel Store, long Id);

        /// <summary>
        /// Insert new ExcludeDay
        /// </summary>
        /// <returns></returns>
        long insertExcludeDay(DBInfoModel Store, ExcludeDaysModel model);

        /// <summary>
        /// Update a ExcludeDay
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ExcludeDaysModel UpdateExcludeDay(DBInfoModel Store, ExcludeDaysModel Model);

        /// <summary>
        /// Delete a ExcludeDay
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteExcludeDay(DBInfoModel Store, long Id);

        /// <summary>
        /// Deleting old Exclude Days from DB
        /// </summary>
        /// <returns></returns>
        bool DeleteOldExcludeDays(DBInfoModel Store);

    }
}
