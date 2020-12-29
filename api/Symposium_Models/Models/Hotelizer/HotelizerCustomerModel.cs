using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotelizer
{
    /// <summary>
    /// Response from hotelizer for Customers
    /// </summary>
    public class HotelizerResponceCustomerModel
    {
        public bool success { get; set; }

        public List<HotelizerCustomerModel> data { get; set; }
    }

    /// <summary>
    /// Model to get Rooms
    /// </summary>
    public class HotelizerCustomerModel
    {
        /// <summary>
        /// Total result records
        /// </summary>
        public int TotalRecs { get; set; }

        /// <summary>
        /// unique row id for pagination
        /// </summary>
        public int Row_ID { get; set; }

        /// <summary>
        /// Reservation id
        /// </summary>
        public int accommodation_id { get; set; }

        /// <summary>
        /// Arrival
        /// </summary>
        public DateTime arrival { get; set; }

        /// <summary>
        /// Departure
        /// </summary>
        public DateTime departure { get; set; }

        /// <summary>
        /// Adults
        /// </summary>
        public int adults { get; set; }

        /// <summary>
        /// Children
        /// </summary>
        public int children { get; set; }


        /// <summary>
        /// Profile Id from Hotelizer
        /// </summary>
        public int? guest_id { get; set; }

        /// <summary>
        /// Room no (not Id)
        /// </summary>
        public string room_name { get; set; }

        /// <summary>
        /// Boardsd (full name e.f. Bed and Breakfast)
        /// </summary>
        public string board_type { get; set; }

        /// <summary>
        /// Room type (not id full name e.g. Familly)
        /// </summary>
        public string room_type_name { get; set; }

        /// <summary>
        /// Profile name (last and first name)
        /// </summary>
        public string guest_name { get; set; }
    }
}
