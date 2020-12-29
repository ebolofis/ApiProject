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
    public class ReservationsFlows : IReservationsFlows
    {
        IReservationsTasks ReservationsTasks;
        public ReservationsFlows(IReservationsTasks resTasks)
        {
            this.ReservationsTasks = resTasks;
        }

        /// <summary>
        /// Returns True or False, True in case that Reservation is in Restaurants Trading Hours else false
        /// </summary>
        /// <returns></returns>
        public bool IsRestaurantOpen(DBInfoModel Store)
        {
            return ReservationsTasks.IsRestaurantOpen(Store);
        }

        /// <summary>
        /// Returns the List of Reservations
        /// </summary>
        /// <returns></returns>
        public ReservationsListModel GetReservations(DBInfoModel Store)
        {
            // get the results
            ReservationsListModel reservationsDetails = ReservationsTasks.GetReservations(Store);

            return reservationsDetails;
        }

        /// <summary>
        /// Returns details for a specific Reservation 
        /// </summary>
        /// <param name="Id">ReservationID</param>
        /// <returns></returns>
        public ReservationsModel GetReservationById(DBInfoModel Store, long Id)
        {
            // get the results
            ReservationsModel reservationDetails = ReservationsTasks.GetReservationById(Store, Id);

            return reservationDetails;
        }

        /// <summary>
        /// Return the List of Available Restaurants Dates And Time
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public AvailabilityListModel GetAvailability(DBInfoModel Store, string TotProfiles, string TotRooms, int Paxes, string language)
        {
            // get the results
            AvailabilityListModel availabilityDetails = ReservationsTasks.GetAvailability(Store, TotProfiles, TotRooms, Paxes, language);

            return availabilityDetails;
        }

        /// <summary>
        /// Return the List of Available Restaurants Dates And MaxTime
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public AvailabilityListModelMaxTime GetMaxTimeAvailability(DBInfoModel Store, string TotProfiles, string TotRooms, int Paxes, string language)
        {
            // get the results
            AvailabilityListModelMaxTime availabilityDetails = ReservationsTasks.GetMaxTimeAvailability(Store, TotProfiles, TotRooms, Paxes, language);

            return availabilityDetails;
        }

        /// <summary>
        /// Flow parses filter to inner Data Access Layer and returns 
        /// a list of reservations from to date included ids in filter
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ReservationsAndCustomersListModel GetFilteredReservations(DBInfoModel Store, ReservationFilter filter) {
            return ReservationsTasks.GetFilteredReservations(Store, filter);
        }

        /// <summary>
        /// Return the List of Available Times for a specific Restaurant
        /// </summary>
        /// <param name="TotProfiles"></param>
        /// <param name="TotRooms"></param>
        /// <param name="Paxes"></param>
        /// <param name="language"></param>
        /// <param name="RestId"></param>
        /// <param name="ActiveDate"></param>
        /// <returns></returns>
        public RestaurantAvailabilityListModel GetRestaurantAvailability(DBInfoModel Store, string TotProfiles, string TotRooms, int Paxes, string language, long RestId, DateTime ActiveDate)
        {
            // get the results
            RestaurantAvailabilityListModel restaurantAvailabilityDetails = ReservationsTasks.GetRestaurantAvailability(Store, TotProfiles, TotRooms, Paxes, language, RestId, ActiveDate);

            return restaurantAvailabilityDetails;
        }


        /// <summary>
        /// Update a Reservation
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ReservationsModel UpdateReservation(DBInfoModel Store, ReservationsModel Model)
        {
            return ReservationsTasks.UpdateReservation(Store, Model);
        }

        /// <summary>
        /// Delete a Reservation
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteReservation(DBInfoModel Store, long Id)
        {
            return ReservationsTasks.DeleteReservation(Store, Id);
        }

        /// <summary>
        /// Deleting old Reservations from DB
        /// </summary>
        /// <returns></returns>
        public bool DeleteOldReservations(DBInfoModel Store)
        {
            return ReservationsTasks.DeleteOldReservations(Store);
        }
    }
}
