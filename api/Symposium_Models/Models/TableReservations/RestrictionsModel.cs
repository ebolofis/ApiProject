using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class RestrictionsModel
    {
        /// <summary>
        /// HardCode Id
        /// </summary>
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// Περιγραφή των Περιορισμών
        /// </summary>
        [Required]
        public string Description { get; set; }
    }
    public class RestrictionsListModel
    {
        public List<RestrictionsModel> RestrictionsModelList { get; set; }
    }
}
