using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations
{
     public interface IRestaurantsDT
    {
        /// <summary>
        /// Returns Trading Hours
        /// </summary>
        /// <returns></returns>
        TradingHoursModel GetTradingHours(DBInfoModel Store);

        /// <summary>
        /// Update Trading Hours
        /// </summary>
        /// <returns></returns>
        TradingHoursModel UpdateTradingHours(DBInfoModel Store, TradingHoursModel model);

        /// <summary>
        /// Returns the List of Restaurants
        /// </summary>
        /// <returns></returns>
        RestaurantsListModel GetRestaurants(DBInfoModel Store);

        /// <summary>
        /// Get details for a specific Restaurant 
        /// </summary>
        /// <param name="Id">RestaurantID</param>
        /// <returns></returns>
        RestaurantsModel GetRestaurantById(DBInfoModel Store, long Id);

        /// <summary>
        /// Select a list optimised for Combo Boxes. Return only Ids and Names from Table.
        /// </summary>
        /// <param name="language">proper Language</param>
        /// <returns>Return only Ids and a proper Restaurant Name from Table.</returns>
        List<ComboListModel> GetComboList(DBInfoModel Store, string language);

        /// <summary>
        /// Insert new Restaurant
        /// </summary>
        /// <returns></returns>
        long insertRestaurant(DBInfoModel Store, RestaurantsModel model);

        /// <summary>
        /// Update a Restaurant
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        RestaurantsModel UpdateRestaurant(DBInfoModel Store, RestaurantsModel Model);

        /// <summary>
        /// Delete a Restaurant
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteRestaurant(DBInfoModel Store, long Id);
    }
}
