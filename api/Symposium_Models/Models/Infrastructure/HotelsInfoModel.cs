using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class HotelsInfoModel : HotelInfoBaseModel
    {
        public Nullable<long> StoreId { get; set; }
        public string HotelUri { get; set; }
        public string RedirectToCustomerCard { get; set; }
        public Nullable<short> PmsType { get; set; }
        public string HotelType { get; set; }
        public Nullable<short> allHotels { get; set; }
        public string ServerName { get; set; }
        public string DBName { get; set; }
        public string DBUserName { get; set; }
        public string DBPassword { get; set; }
    }

    public class HotelInfoBaseModel
    {
        public long Id { get; set; }
        public Nullable<int> HotelId { get; set; }
        public string HotelName { get; set; }
        public Nullable<short> MPEHotel { get; set; }
        public Nullable<byte> Type { get; set; }
    }

}
