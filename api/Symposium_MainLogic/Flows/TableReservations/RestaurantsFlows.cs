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
    public class RestaurantsFlows : IRestaurantsFlows
    {
        IRestaurantsTasks RestaurantsTasks;
        public RestaurantsFlows(IRestaurantsTasks restaurantsTasks)
        {
            this.RestaurantsTasks = restaurantsTasks;
        }

        /// <summary>
        /// Returns Trading Hours
        /// </summary>
        /// <returns></returns>
        public TradingHoursModel GetTradingHours(DBInfoModel Store)
        {
            return RestaurantsTasks.GetTradingHours(Store);
        }

        /// <summary>
        /// Update Trading Hours
        /// </summary>
        /// <returns></returns>
        public TradingHoursModel UpdateTradingHours(DBInfoModel Store, TradingHoursModel model)
        {
            return RestaurantsTasks.UpdateTradingHours(Store, model);
        }

        /// <summary>
        /// Returns the List of Restaurants
        /// </summary>
        /// <returns></returns>
        public RestaurantsListModel GetRestaurants(DBInfoModel Store)
        {
            // get the results
            RestaurantsListModel restaurantDetails = RestaurantsTasks.GetRestaurants(Store);

            return restaurantDetails;
        }

        /// <summary>
        /// Returns details for a specific Restaurant 
        /// </summary>
        /// <param name="Id">RestaurantID</param>
        /// <returns></returns>
        public RestaurantsModel GetRestaurantById(DBInfoModel Store, long Id)
        {
            // get the results
            RestaurantsModel restaurantDetails = RestaurantsTasks.GetRestaurantById(Store, Id);

            return restaurantDetails;
        }

        /// <summary>
        /// Select a list optimised for Combo Boxes. Return only Ids and Names from Table.
        /// </summary>
        /// <param name="language">proper Language</param>
        /// <returns>Return only Ids and a proper Restaurant Name from Table.</returns>
        public List<ComboListModel> GetComboList(DBInfoModel Store, string language)
        {
            return RestaurantsTasks.GetComboList(Store, language);
        }

        /// <summary>
        /// Insert new Restaurant
        /// </summary>
        /// <returns></returns>
        public long insertRestaurant(DBInfoModel Store, RestaurantsModel model)
        {
            return RestaurantsTasks.insertRestaurant(Store, model);
        }

        /// <summary>
        /// Update a Restaurant
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public RestaurantsModel UpdateRestaurant(DBInfoModel Store, RestaurantsModel Model)
        {
            return RestaurantsTasks.UpdateRestaurant(Store, Model);
        }

        /// <summary>
        /// Delete a Restaurant
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRestaurant(DBInfoModel Store, long Id)
        {
            return RestaurantsTasks.DeleteRestaurant(Store, Id);
        }
    }
}
