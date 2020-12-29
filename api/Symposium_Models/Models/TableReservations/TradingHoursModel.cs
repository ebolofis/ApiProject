using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class TradingHoursModel
    {
        /// <summary>
        /// Time From ex: 23:30
        /// </summary>
        public TimeSpan TimeFrom { get; set; }

        /// <summary>
        /// Time To ex: 23:30
        /// </summary>
        public TimeSpan TimeTo { get; set; }
    }
}
