using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class ExcludeDaysModel
    {
        /// <summary>
        /// Id Record Key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Required]
        public long RestId { get; set; }

        /// <summary>
        /// Date of Unavailability
        /// </summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// 0:AllDay,1:Lunch,2:Dinner
        /// </summary>
        [Required]
        public int Type { get; set; }

    }

    public class ExcludeDaysListModel
    {
        public List<ExcludeDaysModel> ExcludeDaysModelList { get; set; }
    } 
}
