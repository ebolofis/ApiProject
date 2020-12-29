using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    /// <summary>
    /// post model from BarPhone
    /// </summary>
   public class BarPhoneModel
    {
        /// <summary>
        /// Agent id
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string AgentId { get; set;}

        /// <summary>
        /// customer phone
        /// </summary>
        [Required]
        [MaxLength(20)]
        [MinLength(10)]
        public string CustPhone { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
