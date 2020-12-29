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
    [RoutePrefix("api/v3/Reservations")]
    public class ReservationsV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IReservationsFlows ReservationsFlow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public ReservationsV3Controller(IReservationsFlows resFlows)
        {
            this.ReservationsFlow = resFlows;
        }

        /// <summary>
        /// Returns True or False, True in case that Reservation is in Restaurants Trading Hours else false
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("IsRestaurantOpen")]
        public HttpResponseMessage IsRestaurantOpen()
        {
            bool result = ReservationsFlow.IsRestaurantOpen(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Gets the List of Reservations
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList")]
        public HttpResponseMessage GetReservations()
        {
            ReservationsListModel result = ReservationsFlow.GetReservations(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result.ReservationsModelList);
        }

        /// <summary>
        /// Gets a specific Reservation by Id
        /// </summary>
        /// <param name="Id">ReservationID</param>
        /// <returns></returns>
        [HttpGet, Route("Get/Id/{Id}")]
        public HttpResponseMessage GetReservationById(long Id)
        {
            ReservationsModel result = ReservationsFlow.GetReservationById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Gets the List of Available Restaurants Dates
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpGet, Route("GetAvailability/TotProfiles/{TotProfiles}/TotRooms/{TotRooms}/Paxes/{Paxes}/language/{language}")]
        public HttpResponseMessage GetAvailability(string TotProfiles, string TotRooms, int Paxes, string language)
        {
            AvailabilityListModel result = ReservationsFlow.GetAvailability(DBInfo, TotProfiles, TotRooms, Paxes, language);
            return Request.CreateResponse(HttpStatusCode.OK, result.AvailabilityModelList);
        }

        [HttpGet, Route("GetMaxTimeAvailability/TotProfiles/{TotProfiles}/TotRooms/{TotRooms}/Paxes/{Paxes}/language/{language}")]
        public HttpResponseMessage GetMaxTimeAvailability(string TotProfiles, string TotRooms, int Paxes, string language)
        {
            AvailabilityListModelMaxTime result = ReservationsFlow.GetMaxTimeAvailability(DBInfo, TotProfiles, TotRooms, Paxes, language);
            return Request.CreateResponse(HttpStatusCode.OK, result.AvailabilityModelListMaxTime);
        }
        /// <summary>
        /// Gets the List of Available Times for a specific Restaurant
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <param name="RestId"></param>
        /// <param name="ActiveDate"></param>
        /// <returns></returns>
        [HttpGet, Route("GetRestaurantAvailability/TotProfiles/{TotProfiles}/TotRooms/{TotRooms}/Paxes/{Paxes}/language/{language}/RestId/{RestId}/ActiveDate/{ActiveDate}")]
        public HttpResponseMessage GetRestaurantAvailability(string TotProfiles, string TotRooms, int Paxes, string language, long RestId, DateTime ActiveDate)
        {
            RestaurantAvailabilityListModel result = ReservationsFlow.GetRestaurantAvailability(DBInfo, TotProfiles, TotRooms, Paxes, language, RestId, ActiveDate);
            return Request.CreateResponse(HttpStatusCode.OK, result.RestaurantAvailabilityModelList);
        }

        [HttpPost, Route("GetReservationsFiltered")]
        public HttpResponseMessage GetReservationsFiltered(ReservationFilter filter)
        {
            ReservationsAndCustomersListModel result = ReservationsFlow.GetFilteredReservations(DBInfo, filter);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Update a specific Reservation to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateReservation(ReservationsModel Model)
        {
            ReservationsModel result = ReservationsFlow.UpdateReservation(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a specific Reservation from DB
        /// </summary>
        /// <param name="Id">ReservationID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteReservation(long Id)
        {
            long result = ReservationsFlow.DeleteReservation(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Deleting old Reservations from DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("DeleteOld")]
        public HttpResponseMessage DeleteOldReservations()
        {
            bool result = ReservationsFlow.DeleteOldReservations(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}