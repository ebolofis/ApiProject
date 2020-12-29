using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    /// <summary>
    /// Address Proximity
    /// </summary>
    public enum AddressProximityEnum
    {
        /// <summary>
        /// The returned result is approximate. (Worst Result) 
        /// </summary>
        Unown_Proximity = -1,
        /// <summary>
        /// The returned result is approximate. (Worst Result) 
        /// </summary>
        APPROXIMATE = 0,

        /// <summary>
        /// The returned result is the geometric center of a result such a line (e.g. street) or polygon (region).
        /// </summary>
        GEOMETRIC_CENTER = 1,

        /// <summary>
        /// The returned result reflects an approximation (usually on a road) interpolated between two precise points (such as intersections).
        /// Interpolated results are generally returned when rooftop geocodes are unavailable for a street address.
        /// </summary>
        RANGE_INTERPOLATED = 2,

        /// <summary>
        /// The returned result reflects a precise geocode. (Best Result)
        /// </summary>
        ROOFTOP = 3
    }
}
