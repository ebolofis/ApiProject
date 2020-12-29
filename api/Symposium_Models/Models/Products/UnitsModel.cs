using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    /// <summary>
    /// Model for Units
    /// </summary>
    public class UnitsModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        [Range(0, long.MaxValue)]
        public Nullable<long> Id { get; set; }

        /// <summary>
        /// Unit Description
        /// </summary>
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }

        /// <summary>
        /// Unit Abbreviation
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string Abbreviation { get; set; }

        /// <summary>
        /// ex TEM,KIL....
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public Nullable<double> Unit { get; set; }

        /// <summary>
        /// Delivery Agent ID
        /// </summary>
        public Nullable<long> DAId { get; set; }
    }
}
