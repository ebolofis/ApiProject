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
    [Table("DA_OrderDetailsExtras")]
    [DisplayName("DA_OrderDetailsExtras")]
    public class DA_OrderDetailsExtrasDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_OrderDetailsExtras")]
        public long Id { get; set; }

        [Column("OrderDetailId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DA_OrderDetailsExtras_DA_OrderDetails")]
        [Association("DA_OrderDetails", "OrderDetailId", "Id")]
        public long OrderDetailId { get; set; }

        [Column("ExtrasId", Order = 3, TypeName = "BIGINT")]
        [Required]
        public long ExtrasId { get; set; }

        [Column("ExtrasCode", Order = 3, TypeName = "NVARCHAR(150)")]
        public string ExtrasCode { get; set; }

        /// <summary>
        /// περιγραφή extras
        /// </summary>
        [Column("Description", Order = 4, TypeName = "NVARCHAR(500)")]
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Ποσότητα
        /// </summary>
        [Column("Qnt", Order = 5, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal Qnt { get; set; }

        /// <summary>
        /// Συνολικό ποσό extras
        /// </summary>
        [Column("Price", Order = 6, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// ποσό ΦΠΑ
        /// </summary>
        [Column("TotalVat", Order = 7, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal TotalVat { get; set; }

        /// <summary>
        /// % ΦΠΑ
        /// </summary>
        [Column("RateVat", Order = 8, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal RateVat { get; set; }

        /// <summary>
        /// % Φόρου
        /// </summary>
        [Column("RateTax", Order = 9, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal RateTax { get; set; }

        /// <summary>
        /// ποσό Φόρου
        /// </summary>
        [Column("TotalTax", Order = 10, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Total-TotalVat-TotalTax
        /// </summary>
        [Column("NetAmount", Order = 11, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal NetAmount { get; set; }

    }
}
