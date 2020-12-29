using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers
{
    public class DateTimeHelper
    {
        /// <summary>
        /// iterate days in date range
        /// </summary>
        /// <param name="from">DateTime</param>
        /// <param name="to">DateTime</param>
        /// <returns></returns>
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }
    }
}
