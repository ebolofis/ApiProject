using Symposium.Models.Models.MealBoards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Helper
{
    public class PosReservationHelper
    {
       public  int HotelId { get; set; } //mpehotel
        public string Name { get; set; }
        public string Room { get; set; }
        public string ReservationId { get; set; } // encrypted buchnr
        public int? Page { get; set; }
        public int? Pagesize { get; set; }
        public int PosDepartmentId { get; set; }
    }

    public class PosReservationDateHelper
    {
        public PosReservationHelper helper { get; set; }
        public int hotelInfoId { get; set; }
        public Nullable<DateTime> dt { get; set; }
    }

    public class PosReservationPeriodHelper
    {
        public PosReservationHelper helper { get; set; }
        public int hotelInfoId { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
    }

    public class PosReservationAllowanceHelper
    {
        public PosReservationHelper helper { get; set; }
        public CustomerModel customer { get; set; }
        public DateTime? dt { get; set; }
        public int ovride { get; set; }

        public PosReservationAllowanceHelper()
        {
            ovride = 0;
        }
    }
}
