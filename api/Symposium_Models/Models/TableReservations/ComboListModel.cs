using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    /// <summary>
    /// Model optimised for ComboBoxes' lists
    /// </summary>
    public class ComboListModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        [Required]
        /// <summary>
        /// Description
        /// </summary>
        public string RestaurantName { get; set; }
    }
}
