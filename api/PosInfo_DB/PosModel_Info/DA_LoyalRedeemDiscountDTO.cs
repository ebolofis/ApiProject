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
    [Table("DA_LoyalRedeemDiscount")]
    [DisplayName("DA_LoyalRedeemDiscount")]
    public class DA_LoyalRedeemDiscountDTO
    {
        /// <summary>
        /// ευρω έκπτωσης στα οποία αντιστοιχεί 1 πόντος για εξαργύρωση
        /// </summary>
        [Column("DiscountRatio", Order = 1, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal DiscountRatio { get; set; }
    }
}
