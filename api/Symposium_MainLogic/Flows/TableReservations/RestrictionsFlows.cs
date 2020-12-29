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
    public class RestrictionsFlows : IRestrictionFlows
    {
        IRestrictionTasks RestrictionsTasks;
        public RestrictionsFlows(IRestrictionTasks restTasks)
        {
            this.RestrictionsTasks = restTasks;
        }

        /// <summary>
        /// Returns the List of Restrictions
        /// </summary>
        /// <returns></returns>
        public RestrictionsListModel GetRestrictions(DBInfoModel Store)
        {
            // get the results
            RestrictionsListModel restrictionDetails = RestrictionsTasks.GetRestrictions(Store);

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
            RestrictionsModel restrictionDetails = RestrictionsTasks.GetRestrictionById(Store, Id);

            return restrictionDetails;
        }

        /// <summary>
        /// Insert new Restriction
        /// </summary>
        /// <returns></returns>
        public long insertRestriction(DBInfoModel Store, RestrictionsModel model)
        {
            return RestrictionsTasks.insertRestriction(Store, model);
        }

        /// <summary>
        /// Update a Restriction
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public RestrictionsModel UpdateRestriction(DBInfoModel Store, RestrictionsModel Model)
        {
            return RestrictionsTasks.UpdateRestriction(Store, Model);
        }

        /// <summary>
        /// Delete a Restriction
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRestriction(DBInfoModel Store, long Id)
        {
            return RestrictionsTasks.DeleteRestriction(Store, Id);
        }
    }
}
