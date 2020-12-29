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
    [Table("DA_LoyalGainAmountRange")]
    [DisplayName("DA_LoyalGainAmountRange")]
    public class DA_LoyalGainAmountRangeDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_LoyalGainAmountRange")]
        public long Id { get; set; }

        /// <summary>
        /// ποσό παραγγελίας από
        /// </summary>
        [Column("FromAmount", Order = 2, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal FromAmount { get; set; }

        /// <summary>
        /// ποσό παραγγελίας έως
        /// </summary>
        [Column("ToAmount", Order = 3, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal ToAmount { get; set; }

        /// <summary>
        /// πόντοι κερδισμένοι
        /// </summary>
        [Column("Points", Order = 4, TypeName = "INT")]
        [Required]
        public int Points { get; set; }
    }
}
