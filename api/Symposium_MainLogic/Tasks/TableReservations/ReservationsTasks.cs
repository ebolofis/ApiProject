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
    public class ReservationsTasks : IReservationsTasks
    {
        IReservationsDT reservationsDT;
        public ReservationsTasks(IReservationsDT resDT)
        {
            this.reservationsDT = resDT;
        }

        /// <summary>
        /// Returns True or False, True in case that Reservation is in Restaurants Trading Hours else false
        /// </summary>
        /// <returns></returns>
        public bool IsRestaurantOpen(DBInfoModel Store)
        {
            return reservationsDT.IsRestaurantOpen(Store);
        }

        /// <summary>
        /// Returns the List of Reservations
        /// </summary>
        /// <returns></returns>
        public ReservationsListModel GetReservations(DBInfoModel Store)
        {
            // get the results
            ReservationsListModel reservationsDetails = reservationsDT.GetReservations(Store);

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
            ReservationsModel reservationDetails = reservationsDT.GetReservationById(Store, Id);

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
            AvailabilityListModel availabilityDetails = reservationsDT.GetAvailability(Store, TotProfiles, TotRooms, Paxes, language);

            return availabilityDetails;
        }

        public AvailabilityListModelMaxTime GetMaxTimeAvailability(DBInfoModel Store, string TotProfiles, string TotRooms, int Paxes, string language)
        {
            // get the results
            AvailabilityListModelMaxTime availabilityDetails = reservationsDT.GetMaxTimeAvailability(Store, TotProfiles, TotRooms, Paxes, language);

            return availabilityDetails;
        }

        /// <summary>
        /// Task parses filter to inner Data Access Layer and returns 
        /// a list of reservations from to date included ids in filter
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ReservationsAndCustomersListModel GetFilteredReservations(DBInfoModel Store, ReservationFilter filter)
        {
            return reservationsDT.GetFilteredReservations(Store, filter);
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
            RestaurantAvailabilityListModel restaurantAvailabilityDetails = reservationsDT.GetRestaurantAvailability(Store, TotProfiles, TotRooms, Paxes, language, RestId, ActiveDate);

            return restaurantAvailabilityDetails;
        }


        /// <summary>
        /// Update a Reservation
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ReservationsModel UpdateReservation(DBInfoModel Store, ReservationsModel Model)
        {
            return reservationsDT.UpdateReservation(Store, Model);
        }

        /// <summary>
        /// Delete a Reservation
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteReservation(DBInfoModel Store, long Id)
        {
            return reservationsDT.DeleteReservation(Store, Id);
        }

        /// <summary>
        /// Deleting old Reservations from DB
        /// </summary>
        /// <returns></returns>
        public bool DeleteOldReservations(DBInfoModel Store)
        {
            return reservationsDT.DeleteOldReservations(Store);
        }
    }
}
