using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class CapacitiesModel
    {
        /// <summary>
        /// Id Key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Required]
        public long RestId { get; set; }

        /// <summary>
        /// num of max customers
        /// </summary>
        [Required]
        public int Capacity { get; set; }

        /// <summary>
        /// 0:AllDay,1:Lunch,2:Dinner
        /// </summary>
        [Required]
        public int Type { get; set; }

        /// <summary>
        /// Time ex: 23:30
        /// </summary>
        [Required]
        public TimeSpan Time { get; set; }
    }
    public class CapacitiesListModel
    {
        public List<CapacitiesModel> CapacitiesModelList { get; set; }
    }
}
