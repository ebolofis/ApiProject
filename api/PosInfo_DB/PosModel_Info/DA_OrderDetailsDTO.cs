using Symposium.Models.Enums;
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
    [Table("DA_OrderDetails")]
    [DisplayName("DA_OrderDetails")]
    public class DA_OrderDetailsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_OrderDetails")]
        public long Id { get; set; }

        /// <summary>
        /// Product.Id
        /// </summary>
        [Column("DAOrderId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DA_OrderDetails_DA_Orders")]
        [Association("DA_Orders", "DAOrderId", "Id")]
        public long DAOrderId { get; set; }

        /// <summary>
        /// Product.Id
        /// </summary>
        [Column("ProductId", Order = 3, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DA_OrderDetails_Product")]
        [Association("Product", "ProductId", "Id")]
        public long ProductId { get; set; }

        [Column("ProductCode", Order = 3, TypeName = "NVARCHAR(150)")]
        public string ProductCode { get; set; }

        /// <summary>
        /// περιγραφή προϊόντος
        /// </summary>
        [Column("Description", Order = 4, TypeName = "NVARCHAR(500)")]
        [Required]
        public string Description { get; set; }

        [Column("PriceListId", Order = 5, TypeName = "BIGINT")]
        [Required]
        public long PriceListId { get; set; }

        /// <summary>
        /// Συνολικό ποσό (πριν την έκπτωση)
        /// </summary>
        [Column("Price", Order = 6, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// Ποσότητα
        /// </summary>
        [Column("Qnt", Order = 7, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal Qnt { get; set; }

        /// <summary>
        /// Ποσό έκπτωσης
        /// </summary>
        [Column("Discount", Order = 8, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal Discount { get; set; }

        /// <summary>
        /// Συνολικό ποσό (μετά την έκπτωση)
        /// </summary>
        [Column("Total", Order = 9, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal Total { get; set; }

        /// <summary>
        /// ποσό ΦΠΑ
        /// </summary>
        [Column("TotalVat", Order = 10, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal TotalVat { get; set; }

        /// <summary>
        /// % ΦΠΑ
        /// </summary>
        [Column("RateVat", Order = 11, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal RateVat { get; set; }

        /// <summary>
        /// % Φόρου
        /// </summary>
        [Column("RateTax", Order = 12, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal RateTax { get; set; }

        /// <summary>
        /// ποσό Φόρου
        /// </summary>
        [Column("TotalTax", Order = 13, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Total-TotalVat-TotalTax
        /// </summary>
        [Column("NetAmount", Order = 14, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Remarks for main product of a order detail. Sended to OrderDetailInvoices Table on Store and on Field ItemRemark
        /// </summary>
        [Column("ItemRemark", Order = 15, TypeName = "NVARCHAR(150)")]
        public string ItemRemark { get; set; }

        /// <summary>
        /// Type of special discount that is used. 0: Hit Loyalty, 1: Goodys discounts, 2: Vodafone discounts
        /// </summary>
        [Column("OtherDiscount", Order = 16, TypeName = "SMALLINT")]
        public Nullable<DA_OrderDetail_OtherDiscountEnum> OtherDiscount { get; set; }
    }
}
