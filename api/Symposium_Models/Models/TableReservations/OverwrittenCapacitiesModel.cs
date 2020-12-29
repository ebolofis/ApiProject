using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class OverwrittenCapacitiesModel
    {
        /// <summary>
        /// Id record key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Required]
        public long RestId { get; set; }

        /// <summary>
        /// TR_Capacities.Id
        /// </summary>
        [Required]
        public long CapacityId { get; set; }

        /// <summary>
        /// num of max customers
        /// </summary>
        [Required]
        public int Capacity { get; set; }

        /// <summary>
        /// Date of overwrite
        /// </summary>
        [Required]
        public DateTime Date { get; set; }

    }

    public class OverwrittenCapacitiesListModel
    {
        public List<OverwrittenCapacitiesModel> OverwrittenCapacitiesModelList { get; set; }
    }
}
