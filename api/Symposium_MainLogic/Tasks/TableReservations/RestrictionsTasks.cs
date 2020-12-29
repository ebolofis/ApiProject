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
    public class RestrictionsTasks : IRestrictionTasks
    {
        IRestrictionDT restrictionsDT;
        public RestrictionsTasks(IRestrictionDT restDT)
        {
            this.restrictionsDT = restDT;
        }

        /// <summary>
        /// Returns the List of Restrictions
        /// </summary>
        /// <returns></returns>
        public RestrictionsListModel GetRestrictions(DBInfoModel Store)
        {
            // get the results
            RestrictionsListModel restrictionDetails = restrictionsDT.GetRestrictions(Store);

            return restrictionDetails;
        }

        /// <summary>
        /// Returns details for a specific Restriction 
        /// </summary>
        /// <param name="Id">RestrictionID</param>
        /// <returns></returns>
        public RestrictionsModel GetRestrictionById(DBInfoModel Store, long Id)
        {
            // get the results
            RestrictionsModel restrictionDetails = restrictionsDT.GetRestrictionById(Store, Id);

            return restrictionDetails;
        }

        /// <summary>
        /// Insert new Restriction
        /// </summary>
        /// <returns></returns>
        public long insertRestriction(DBInfoModel Store, RestrictionsModel model)
        {
            return restrictionsDT.insertRestriction(Store, model);
        }

        /// <summary>
        /// Update a Restriction
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public RestrictionsModel UpdateRestriction(DBInfoModel Store, RestrictionsModel Model)
        {
            return restrictionsDT.UpdateRestriction(Store, Model);
        }

        /// <summary>
        /// Delete a Restriction
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRestriction(DBInfoModel Store, long Id)
        {
            return restrictionsDT.DeleteRestriction(Store, Id);
        }
    }
}
