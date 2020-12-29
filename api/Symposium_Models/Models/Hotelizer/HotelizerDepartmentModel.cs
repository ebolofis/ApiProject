using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotelizer
{
    /// <summary>
    /// Responce from Hotelizer
    /// </summary>
    public class HotelizerDepartmentResponceModel
    {
        public bool success { get; set; }

        public List<HotelizerDepartmentModel> data { get; set; }
    }

    public class HotelizerDepartmentModel
    {
        /// <summary>
        /// Department Id ==> DepartmentId
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Department name ==> Description
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Department group type ==> GroupName
        /// </summary>
        public string revenue_type { get; set; }
    }
}
