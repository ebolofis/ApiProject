using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations
{
    public interface IReservationsFlows
    {
        /// <summary>
        /// Returns True or False, True in case that Reservation is in Restaurants Trading Hours else false
        /// </summary>
        /// <returns></returns>
        bool IsRestaurantOpen(DBInfoModel Store);

        /// <summary>
        /// Get the List of Reservations
        /// </summary>
        /// <returns></returns>
        ReservationsListModel GetReservations(DBInfoModel Store);

        /// <summary>
        /// Get details for a specific Reservation 
        /// </summary>
        /// <param name="Id">ReservationID</param>
        /// <returns></returns>
        ReservationsModel GetReservationById(DBInfoModel Store, long Id);

        /// <summary>
        /// Gets the List of Available Restaurants Dates And Time
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        AvailabilityListModel GetAvailability(DBInfoModel Store, string TotProfiles, string TotRooms, int Paxes, string language);

        /// <summary>
        /// Return the List of Available Restaurants Dates And MaxTime
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        AvailabilityListModelMaxTime GetMaxTimeAvailability(DBInfoModel Store, string TotProfiles, string TotRooms, int Paxes, string language);

        /// <summary>
        /// Flow parses filter to inner Data Access Layer and returns 
        /// a list of reservations from to date included ids in filter and reservation customers list on same filter
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        ReservationsAndCustomersListModel GetFilteredReservations(DBInfoModel Store, ReservationFilter filter);

        /// <summary>
        /// Get the List of Available Times for a specific Restaurant
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <param name="RestId"></param>
        /// <param name="ActiveDate"></param>
        /// <returns></returns>
        RestaurantAvailabilityListModel GetRestaurantAvailability(DBInfoModel Store, string TotProfiles, string TotRooms, int Paxes, string language, long RestId, DateTime ActiveDate);


        /// <summary>
        /// Update a Reservation
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ReservationsModel UpdateReservation(DBInfoModel Store, ReservationsModel Model);

        /// <summary>
        /// Delete a Reservation
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteReservation(DBInfoModel Store, long Id);

        /// <summary>
        /// Deleting old Reservations from DB
        /// </summary>
        /// <returns></returns>
        bool DeleteOldReservations(DBInfoModel Store);
    }
}
