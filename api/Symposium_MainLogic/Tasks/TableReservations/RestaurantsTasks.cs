using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.TableReservations
{
    class RestaurantsTasks : IRestaurantsTasks
    {
        IRestaurantsDT restaurantsDT;
        public RestaurantsTasks(IRestaurantsDT rDT)
        {
            this.restaurantsDT = rDT;
        }

        /// <summary>
        /// Returns Trading Hours
        /// </summary>
        /// <returns></returns>
        public TradingHoursModel GetTradingHours(DBInfoModel Store)
        {
            return restaurantsDT.GetTradingHours(Store);
        }

        /// <summary>
        /// Update Trading Hours
        /// </summary>
        /// <returns></returns>
        public TradingHoursModel UpdateTradingHours(DBInfoModel Store, TradingHoursModel model)
        {
            return restaurantsDT.UpdateTradingHours(Store, model);
        }

        /// <summary>
        /// Returns the List of Restaurants
        /// </summary>
        /// <returns></returns>
        public RestaurantsListModel GetRestaurants(DBInfoModel Store)
        {
            return restaurantsDT.GetRestaurants(Store);
        }

        /// <summary>
        /// Returns details for a specific Restaurant 
        /// </summary>
        /// <param name="Id">RestaurantID</param>
        /// <returns></returns>
        public RestaurantsModel GetRestaurantById(DBInfoModel Store, long Id)
        {
            // get the results
            RestaurantsModel restaurantDetails = restaurantsDT.GetRestaurantById(Store, Id);

            return restaurantDetails;
        }

        /// <summary>
        /// Select a list optimised for Combo Boxes. Return only Ids and Names from Table.
        /// </summary>
        /// <param name="language">proper Language</param>
        /// <returns>Return only Ids and a proper Restaurant Name from Table.</returns>
        public List<ComboListModel> GetComboList(DBInfoModel Store, string language)
        {
            return restaurantsDT.GetComboList(Store, language);
        }

        /// <summary>
        /// Insert new Restaurant
        /// </summary>
        /// <returns></returns>
        public long insertRestaurant(DBInfoModel Store, RestaurantsModel model)
        {
            return restaurantsDT.insertRestaurant(Store, model);
        }

        /// <summary>
        /// Update a Restaurant
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public RestaurantsModel UpdateRestaurant(DBInfoModel Store, RestaurantsModel Model)
        {
            return restaurantsDT.UpdateRestaurant(Store, Model);
        }

        /// <summary>
        /// Delete a Restaurant
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteRestaurant(DBInfoModel Store, long Id)
        {
            return restaurantsDT.DeleteRestaurant(Store ,Id);
        }
    }
}
