using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Hotel
{
    public class MealConsumptionModel
    {
        /// <summary>
        /// Timezone Code (B,L,D)
        /// </summary>
        public string timezoneCode { get; set; }

        /// <summary>
        /// allowance consumed by adult for specific day and timezone 
        /// </summary>
        public int adultConsumption { get; set; }

        /// <summary>
        /// allowance consumed by child for specific day and timezone 
        /// </summary>
        public int childConsumption { get; set; }
    }

    public class MealConsumptionDetailModel
    {
        /// <summary>
        /// Timestamp of consumption
        /// </summary>
        public DateTime tmstamp { get; set; }
        /// <summary>
        /// Timezone Code (B,L,D)
        /// </summary>
        public string timezoneCode { get; set; }

        /// <summary>
        /// allowance consumed by adult for specific day and timezone 
        /// </summary>
        public int adultConsumption { get; set; }

        /// <summary>
        /// allowance consumed by child for specific day and timezone 
        /// </summary>
        public int childConsumption { get; set; }

        /// <summary>
        /// board used for consumption
        /// </summary>
        public string boardCode { get; set; }

        /// <summary>
        /// department used for consumption
        /// </summary>
        public int departmentID { get; set; }

        /// <summary>
        /// department description
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// pos used for consumption
        /// </summary>
        public int posInfoID { get; set; }

        /// <summary>
        /// pos description
        /// </summary>
        public string PosInfo { get; set; }
    }
}
