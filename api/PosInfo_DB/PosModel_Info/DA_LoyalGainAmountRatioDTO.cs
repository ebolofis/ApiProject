using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [Table("DA_LoyalGainAmountRatio")]
    [DisplayName("DA_LoyalGainAmountRatio")]
    public class DA_LoyalGainAmountRatioDTO
    {
        /// <summary>
        /// πόντοι στους οποίους αντιστοιχεί 1 ευρω
        /// </summary>
        [Column("ToPoints", Order = 1, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal ToPoints { get; set; }
    }
}
