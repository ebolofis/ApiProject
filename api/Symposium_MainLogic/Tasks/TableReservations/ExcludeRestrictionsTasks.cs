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
    public class ExcludeRestrictionsTasks : IExcludeRestrictionsTasks
    {
        IExcludeRestrictionsDT excludeRestrictionsDT;
        public ExcludeRestrictionsTasks(IExcludeRestrictionsDT exRestrictionsDT)
        {
            this.excludeRestrictionsDT = exRestrictionsDT;
        }

        /// <summary>
        /// Returns the List of Exclude Restrictions
        /// </summary>
        /// <returns></returns>
        public ExcludeRestrictionsListModel GetExcludeRestrictions(DBInfoModel Store)
        {
            // get the results
            ExcludeRestrictionsListModel excludeRestrictionsDetails = excludeRestrictionsDT.GetExcludeRestrictions(Store);

            return excludeRestrictionsDetails;
        }

        /// <summary>
        /// Returns details for a specific ExcludeRestriction
        /// </summary>
        /// <param name="Id">ExcludeRestrictionID</param>
        /// <returns></returns>
        public ExcludeRestrictionsModel GetExcludeRestrictionById(DBInfoModel Store, long Id)
        {
            // get the results
            ExcludeRestrictionsModel excludeRestrictionDetails = excludeRestrictionsDT.GetExcludeRestrictionById(Store, Id);

            return excludeRestrictionDetails;
        }

        /// <summary>
        /// Insert new ExcludeRestriction
        /// </summary>
        /// <returns></returns>
        public long insertExcludeRestriction(DBInfoModel Store, ExcludeRestrictionsModel model)
        {
            return excludeRestrictionsDT.insertExcludeRestriction(Store, model);
        }

        /// <summary>
        /// Update an ExcludeRestriction
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ExcludeRestrictionsModel UpdateExcludeRestriction(DBInfoModel Store, ExcludeRestrictionsModel Model)
        {
            return excludeRestrictionsDT.UpdateExcludeRestriction(Store, Model);
        }

        /// <summary>
        /// Delete an ExcludeRestriction
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteExcludeRestriction(DBInfoModel Store, long Id)
        {
            return excludeRestrictionsDT.DeleteExcludeRestriction(Store, Id);
        }

        /// <summary>
        /// Deleting old Exclude Restrictions from DB
        /// </summary>
        /// <returns></returns>
        public bool DeleteOldExcludeRestrictions(DBInfoModel Store)
        {
            return excludeRestrictionsDT.DeleteOldExcludeRestrictions(Store);
        }

    }
}
