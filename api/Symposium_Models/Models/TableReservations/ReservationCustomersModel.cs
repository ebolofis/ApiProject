using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class ReservationCustomersModel
    {
        /// <summary>
        /// Id Record Key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Protel Profile Id
        /// </summary>
        [Required]
        public long ProtelId { get; set; }

        /// <summary>
        /// ProtelName (encrypted)
        /// </summary>
        [Required]
        public string ProtelName { get; set; }

        /// <summary>
        /// Name given by the customer (encrypted)
        /// </summary>
        [Required]
        public string ReservationName { get; set; }

        /// <summary>
        /// Room number
        /// </summary>
        [Required]
        public string RoomNum { get; set; }

        /// <summary>
        /// email (encrypted)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// TR_Reservations.Id
        /// </summary>
        public long ReservationId { get; set; }

        /// <summary>
        /// Hotel info index
        /// </summary>
        public long HotelId { get; set; }
    }

    public class ReservationCustomersListModel
    {
        public List<ReservationCustomersModel> ReservationCustomersModelList { get; set; }
    }

    public class CustomersInfo
    {
        /// <summary>
        /// Protel Profile Id
        /// </summary>
        public int? ProfileNo { get; set; }

        /// <summary>
        /// Customer First Name
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Customer Last Name
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Number of people
        /// </summary>
        public int? NumberOfPeople { get; set; }

        /// <summary>
        /// Contact Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Contact Room
        /// </summary>
        public string Room { get; set; }
    }

    public class ExtendedReservetionModel
    {
        public ReservationsModel Reservation { get; set; }

        public List<ReservationCustomersModel> ReservationCustomers { get; set; }

        public string Language { get; set; } // 'GR', 'EN', 'RU', 'FR', 'DE'

        public bool SendEmail { get; set; }

        public bool PrintToExtECR { get; set; }

    }

    public class ExtecrTableReservetionModel
    {
        public string RestaurantName { get; set; }
        public ReservationsModel Reservation { get; set; }
        public List<ReservationCustomersModel> ReservationCustomers { get; set; }
    }
}
