using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class ReservationsModel
    {
        /// <summary>
        /// Id Record key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Required]
        public long RestId { get; set; }

        /// <summary>
        /// TR_Capacities.Id
        /// </summary>
        [Required]
        public long CapacityId { get; set; }

        /// <summary>
        /// Number of people
        /// </summary>
        [Required]
        public int Couver { get; set; }

        /// <summary>
        /// Reservation Date
        /// </summary>
        [Required]
        public DateTime ReservationDate { get; set; }

        /// <summary>
        /// Reservation Time
        /// </summary>
        [Required]
        public TimeSpan ReservationTime { get; set; }

        /// <summary>
        /// Date of Reservation
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 0: Active, 1: Cancel
        /// </summary>
        [Required]
        public int Status { get; set; }

        public string Description { get; set; }
    }

    public class ReservationFilter {
        public List<long> Restaurants { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }

    }
    public class ReservationsAndCustomersListModel
    {
        public List<ReservationsModel> ReservationsModelList { get; set; }
        public List<ReservationCustomersModel> ReservationsCustomerModelList { get; set; }
    }

    public class ReservationsListModel
    {
        public List<ReservationsModel> ReservationsModelList { get; set; }
    }
    public class AvailabilityModel
    {
        public DateTime AvailDate { get; set; }

        public long RestId { get; set; }

        public string RestaurantName { get; set; }

        public int Type { get; set; }
    }
    public class AvailabilityListModel
    {
        public List<AvailabilityModel> AvailabilityModelList { get; set; }
    }
    public class RestaurantAvailabilityModel
    {
        public long CapacityId { get; set; }
        public TimeSpan Time { get; set; }
        public int Type { get; set; }
    }
    public class RestaurantAvailabilityListModel
    {
        public List<RestaurantAvailabilityModel> RestaurantAvailabilityModelList { get; set; }
    }

    public class AvailabilityModelMaxTime
    {

        public long RestId { get; set; }
        public TimeSpan Time { get; set; }
        public int Type { get; set; }
    }
    public class AvailabilityListModelMaxTime
    {
        public List<AvailabilityModelMaxTime> AvailabilityModelListMaxTime { get; set; }
    }
}
