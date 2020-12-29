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
    public class RestrictionsRestaurantsAssocFlows : IRestrictionsRestaurantsAssocFlows
    {
        IRestrictionsRestaurantsAssocTasks RestrictionsRestaurantsAssocTasks;
        public RestrictionsRestaurantsAssocFlows(IRestrictionsRestaurantsAssocTasks restrictionsRestaurantsAssocTasks)
        {
            this.RestrictionsRestaurantsAssocTasks = restrictionsRestaurantsAssocTasks;
        }

        /// <summary>
        /// Returns the List of Restrictions Restaurants Associations
        /// </summary>
        /// <returns></returns>
        public RestrictionsRestaurantsAssocListModel GetRestrictionsRestaurantsAssoc(DBInfoModel Store)
        {
            // get the results
            RestrictionsRestaurantsAssocListModel restaurantDetails = RestrictionsRestaurantsAssocTasks.GetRestrictionsRestaurantsAssoc(Store);

            return restaurantDetails;
        }

        /// <summary>
        /// Gets a specific Restrictions_Restaurants_Assoc by Id
        /// </summary>
        /// <param name="Id">RestrictionsRestaurantsAssocID</param>
        /// <returns></returns>
        public RestrictionsRestaurantsAssocModel GetRestrictionsRestaurantsAssocById(DBInfoModel Store, long Id)
        {
            // get the results
            RestrictionsRestaurantsAssocModel restrictionsRestaurantsAssocDetails = RestrictionsRestaurantsAssocTasks.GetRestrictionsRestaurantsAssocById(Store, Id);

            return restrictionsRestaurantsAssocDetails;
        }

        /// <summary>
        /// Insert new Restrictions_Restaurants_Assoc
        /// </summary>
        /// <returns></returns>
        public long insertRestrictionsRestaurantsAssoc(DBInfoModel Store, RestrictionsRestaurantsAssocModel model)
        {
            return RestrictionsRestaurantsAssocTasks.insertRestrictionsRestaurantsAssoc(Store, model);
        }

        /// <summary>
        /// Update a Restrictions_Restaurants_Assoc
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public RestrictionsRestaurantsAssocModel UpdateRestrictionsRestaurantsAssoc(DBInfoModel Store, RestrictionsRestaurantsAssocModel Model)
        {
            return RestrictionsRestaurantsAssocTasks.UpdateRestrictionsRestaurantsAssoc(Store, Model);
        }

        /// <summary>
        /// Delete a Restrictions_Restaurants_Assoc
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRestrictionsRestaurantsAssoc(DBInfoModel Store, long Id)
        {
            return RestrictionsRestaurantsAssocTasks.DeleteRestrictionsRestaurantsAssoc(Store, Id);
        }
    }
}
