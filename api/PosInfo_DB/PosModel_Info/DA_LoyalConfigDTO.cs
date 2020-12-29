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
    [Table("DA_LoyalConfig")]
    [DisplayName("DA_LoyalConfig")]
    public class DA_LoyalConfigDTO
    {
        /// <summary>
        /// τυπος λήψης πόντων. 0:AmountRange, 1:AmountRatio
        /// </summary>
        [Column("GainPointsType", Order = 1, TypeName = "SMALLINT")]
        [Required]
        public Int16 GainPointsType { get; set; }

        /// <summary>
        /// τύπος εξαργύρωσης. 0:FreeProduct, 1:Discount, 2: both
        /// </summary>
        [Column("RedeemType", Order = 2, TypeName = "SMALLINT")]
        [Required]
        public Int16 RedeemType { get; set; }

        /// <summary>
        /// μέγιστος αρ.πόντων. 0 για χωρίς περιορισμό
        /// </summary>
        [Column("MaxPoints", Order = 3, TypeName = "INT")]
        [Required]
        public int MaxPoints { get; set; }

        /// <summary>
        /// μέγιστη διάρκεια πόντων σε μήνες. 0 για χωρίς περιορισμό
        /// </summary>
        [Column("ExpDuration", Order = 4, TypeName = "INT")]
        [Required]
        public int ExpDuration { get; set; }

        /// <summary>
        /// ελλάχιστη παραγγελία για λήψη πόντων
        /// </summary>
        [Column("MinAmount", Order = 5, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal MinAmount { get; set; }

        /// <summary>
        /// αρχικοί πόντοι κατά την εγγραφή του πελάτη
        /// </summary>
        [Column("InitPoints", Order = 6, TypeName = "INT")]
        public Nullable<int> InitPoints { get; set; }
    }
}
