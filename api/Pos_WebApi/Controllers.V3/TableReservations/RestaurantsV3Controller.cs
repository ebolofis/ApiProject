using Microsoft.AspNet.SignalR;
using Pos_WebApi.Modules;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/Restaurants")]
    public class RestaurantsV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IRestaurantsFlows RestaurantsFlow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public RestaurantsV3Controller(IRestaurantsFlows restaurantsFlows)
        {
            this.RestaurantsFlow = restaurantsFlows;
        }

        /// <summary>
        /// Gets Trading Hours
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetTradingHours")]
        public HttpResponseMessage GetTradingHours()
        {
            TradingHoursModel result = RestaurantsFlow.GetTradingHours(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Update Trading Hours
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("UpdateTradingHours")]
        public HttpResponseMessage UpdateTradingHours(TradingHoursModel model)
        {
            TradingHoursModel result = RestaurantsFlow.UpdateTradingHours(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Gets the List of Restaurants
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList")]
        public HttpResponseMessage GetRestaurants()
        {
            RestaurantsListModel result = RestaurantsFlow.GetRestaurants(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result.RestaurantsList);
        }

        /// <summary>
        /// Gets a specific Restaurant by Id
        /// </summary>
        /// <param name="Id">RestaurantID</param>
        /// <returns></returns>
        [HttpGet, Route("Get/Id/{Id}")]
        public HttpResponseMessage GetRestaurantById(long Id)
        {
            RestaurantsModel result = RestaurantsFlow.GetRestaurantById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        //TO DO: ComboList
        /// <summary>
        /// Get the list of Restaurants for ComboBox.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetComboList/{language}")]
        public HttpResponseMessage GetComboList(string language)
        {
            List<ComboListModel> results = RestaurantsFlow.GetComboList(DBInfo, language);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Insert new Restaurant
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [ValidateModelState]
        public HttpResponseMessage InsertRestaurant(RestaurantsModel model)
        {
            long id = RestaurantsFlow.insertRestaurant(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        /// <summary>
        /// Update a specific Restaurant to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateRestaurant(RestaurantsModel Model)
        {
            RestaurantsModel result = RestaurantsFlow.UpdateRestaurant(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a specific Restaurant from DB
        /// </summary>
        /// <param name="Id">RestaurantID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteRestaurant(long Id)
        {
            long result = RestaurantsFlow.DeleteRestaurant(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}