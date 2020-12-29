using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations
{
    public interface IExcludeRestrictionsDT
    {
        /// <summary>
        /// Get the List of Exclude Restrictions
        /// </summary>
        /// <returns></returns>
        ExcludeRestrictionsListModel GetExcludeRestrictions(DBInfoModel Store);

        /// <summary>
        /// Get details for a specific ExcludeRestriction 
        /// </summary>
        /// <param name="Id">ExcludeRestrictionID</param>
        /// <returns></returns>
        ExcludeRestrictionsModel GetExcludeRestrictionById(DBInfoModel Store, long Id);

        /// <summary>
        /// Insert new ExcludeRestriction
        /// </summary>
        /// <returns></returns>
        long insertExcludeRestriction(DBInfoModel Store, ExcludeRestrictionsModel model);

        /// <summary>
        /// Update an ExcludeRestriction
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ExcludeRestrictionsModel UpdateExcludeRestriction(DBInfoModel Store, ExcludeRestrictionsModel Model);

        /// <summary>
        /// Delete an ExcludeRestriction
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteExcludeRestriction(DBInfoModel Store, long Id);

        /// <summary>
        /// Deleting old Exclude Restrictions from DB
        /// </summary>
        /// <returns></returns>
        bool DeleteOldExcludeRestrictions(DBInfoModel Store);
    }
}
