using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations
{
    public interface IRestrictionDT
    {
        /// <summary>
        /// Get the List of Restrictions
        /// </summary>
        /// <returns></returns>
        RestrictionsListModel GetRestrictions(DBInfoModel Store);

        /// <summary>
        /// Returns details for a specific Restriction 
        /// </summary>
        /// <param name="Id">RestrictionID</param>
        /// <returns></returns>
        RestrictionsModel GetRestrictionById(DBInfoModel Store, long Id);

        /// <summary>
        /// Insert new Restriction
        /// </summary>
        /// <returns></returns>
        long insertRestriction(DBInfoModel Store, RestrictionsModel model);

        /// <summary>
        /// Update a Restriction
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        RestrictionsModel UpdateRestriction(DBInfoModel Store, RestrictionsModel Model);

        /// <summary>
        /// Delete a Restriction
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteRestriction(DBInfoModel Store, long Id);
    }
}
