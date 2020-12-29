using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class RestrictionsRestaurantsAssocModel
    {
        /// <summary>
        /// Id Record key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// TR_Restrictions.Id
        /// </summary>
        [Required]
        public long RestrictId { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Required]
        public long RestId { get; set; }

        /// <summary>
        /// Restriction number
        /// </summary>
        [Required]
        public int N { get; set; }
    }

    public class RestrictionsRestaurantsAssocListModel
    {
        public List<RestrictionsRestaurantsAssocModel> RestrictionsRestaurantsAssocList { get; set; }
    }
}
