using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotel
{
    public class GuestIdsList
    {
        public List<int> guestIdsList { get; set; }
    }

    public class guestResultModel
    {
        public int GuestId { get; set; }

        public int ProfileNo { get; set; }
    }

    public class GuestGetIdModel
    {
        public int ProfileNo { get; set; }

        public int RoomId { get; set; }
    }
}
