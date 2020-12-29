using Symposium.Models.Enums.PmsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Plugins.PmsInterface
{
    /// <summary>
    /// Parameters for Customer selection
    /// </summary>
    public class GetPmsCustomersModel
    {
        /// <summary>
        /// mpehotel only for protel
        /// </summary>
        public Nullable<int> mpehotel { get; set; }

        /// <summary>
        /// Room no
        /// </summary>
        public string roomNo { get; set; }

        /// <summary>
        /// Profile name
        /// </summary>
        public string profileName { get; set; }

        /// <summary>
        /// Reservation reference
        /// </summary>
        public string reservationReference { get; set; }

        /// <summary>
        /// Reservation period from 
        /// </summary>
        public Nullable<DateTime> fromDate { get; set; }

        /// <summary>
        /// Reservation period to
        /// </summary>
        public Nullable<DateTime> toDate { get; set; }

        /// <summary>
        /// selected page no
        /// </summary>
        public Nullable<int> pageNo { get; set; }

        /// <summary>
        /// Records per page
        /// </summary>
        public Nullable<int> pageSize { get; set; }

        /// <summary>
        /// get all hotels (only for protel)
        /// </summary>
        public Nullable<bool> allHotels { get; set; }
    }

    public class GetDepartmentsPms
    {
        /// <summary>
        /// Pms Hotel Id
        /// </summary>
        public int mpeHotel { get; set; }

        /// <summary>
        /// 0 => only visible on web
        /// 1 => not visible on web
        /// 2 => both all records
        /// </summary>
        public VisibleOnWebEnum visibleOnWeb { get; set; }
    }
}
