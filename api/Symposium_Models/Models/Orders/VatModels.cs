using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models {
    public class VatModel {
        [Required]
        [Range(0, long.MaxValue)]
        public long Id { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Description { get; set; }
        [Required]
        [Range(0, 100)]
        public Nullable<decimal> Percentage { get; set; }
        [Required]
        [Range(0, long.MaxValue)]
        public Nullable<int> Code { get; set; }
        public Nullable<long> DAId { get; set; }
    }
}
