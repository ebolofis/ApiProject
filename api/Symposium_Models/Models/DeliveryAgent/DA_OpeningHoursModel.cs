using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_OpeningHoursModel
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public int Weekday { get; set; }
        public int OpenHour { get; set; }
        public int OpenMinute { get; set; }
        public int CloseHour { get; set; }
        public int CloseMinute { get; set; }

    }
}
