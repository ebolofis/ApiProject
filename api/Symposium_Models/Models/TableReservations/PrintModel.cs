using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class PrintModel
    {
        /// <summary>
        /// Id Record Key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// TR_Reservations.Id
        /// </summary>
        [Required]
        public long ReservationId { get; set; }

        /// <summary>
        /// Name given by the customer (encrypted)
        /// </summary>
        [Required]
        public List<string> ReservationName { get; set; }

        /// <summary>
        /// Room number
        /// </summary>
        [Required]
        public List<string> RoomNum { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Required]
        public long RestId { get; set; }

        /// <summary>
        /// Ονομασία Ξενοδοχείου, το πεδίο να μην υπερβαίνει τους 200 χαρακτήρες
        /// </summary>
        [MaxLength(200)]
        [Required]
        public string Name { get; set; }

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

    }
}
