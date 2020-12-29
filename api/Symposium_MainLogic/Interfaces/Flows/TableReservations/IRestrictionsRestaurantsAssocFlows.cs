using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations
{
    public interface IRestrictionsRestaurantsAssocFlows
    {
        /// <summary>
        /// Get the List of Restrictions Restaurants Associations
        /// </summary>
        /// <returns></returns>
        RestrictionsRestaurantsAssocListModel GetRestrictionsRestaurantsAssoc(DBInfoModel Store);

        /// <summary>
        /// Gets a specific Restrictions_Restaurants_Assoc by Id
        /// </summary>
        /// <param name="Id">RestrictionsRestaurantsAssocID</param>
        /// <returns></returns>
        RestrictionsRestaurantsAssocModel GetRestrictionsRestaurantsAssocById(DBInfoModel Store, long Id);

        /// <summary>
        /// Insert new Restrictions_Restaurants_Assoc
        /// </summary>
        /// <returns></returns>
        long insertRestrictionsRestaurantsAssoc(DBInfoModel Store, RestrictionsRestaurantsAssocModel model);

        /// <summary>
        /// Update a Restrictions_Restaurants_Assoc
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        RestrictionsRestaurantsAssocModel UpdateRestrictionsRestaurantsAssoc(DBInfoModel Store, RestrictionsRestaurantsAssocModel Model);

        /// <summary>
        /// Delete a Restrictions_Restaurants_Assoc
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteRestrictionsRestaurantsAssoc(DBInfoModel Store, long Id);
    }
}
