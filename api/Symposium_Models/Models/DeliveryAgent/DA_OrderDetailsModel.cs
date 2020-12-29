using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_OrderDetailsModel
    {
        public long Id { get; set; }
        /// <summary>
        /// Product.Id
        /// </summary>
        public long DAOrderId { get; set; }
        /// <summary>
        /// Product.Id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public long ProductId { get; set; }
        /// <summary>
        /// περιγραφή προϊόντος
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        public long PriceListId { get; set; }
        /// <summary>
        /// Συνολικό ποσό (πριν την έκπτωση)
        /// </summary>
        [Required]
        public decimal Price { get; set; }
        /// <summary>
        /// Ποσότητα
        /// </summary>
         [Required]
        [Range(0.001, Double.MaxValue)]
        public decimal Qnt { get; set; }
        /// <summary>
        /// Ποσό έκπτωσης
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal Discount { get; set; }
        /// <summary>
        /// Συνολικό ποσό (μετά την έκπτωση)
        /// </summary>
        [Required]
        public decimal Total { get; set; }
        /// <summary>
        /// ποσό ΦΠΑ
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal TotalVat { get; set; }
        /// <summary>
        /// % ΦΠΑ
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal RateVat { get; set; }
        /// <summary>
        /// % Φόρου
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal RateTax { get; set; }
        /// <summary>
        /// ποσό Φόρου
        /// </summary>
        [Required]
        [Range(0, Double.MaxValue)]
        public decimal TotalTax { get; set; }
        /// <summary>
        /// Price-Discount-TotalVat-TotalTax
        /// </summary>
        [Required]
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Remarks for main product of a order detail. Sended to OrderDetailInvoices Table on Store and on Field ItemRemark
        /// </summary>
        public string ItemRemark { get; set; }
    }
    public class DA_OrderDetailExtModel : DA_OrderDetails
    {
        public long StoreProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public string ProductCategory { get; set; }
        public Nullable<long> CategoryId { get; set; }
        public string Category { get; set; }
        public Nullable<long> UnitId { get; set; }
        public string Unit { get; set; }
        public Nullable<long> StorePriceListId { get; set; }
        public Nullable<long> PriceListDetailId { get; set; }
        public string PriceList { get; set; }
        public Nullable<int> PreparationTime { get; set; }
        public Nullable<long> KdsId { get; set; }
        public Nullable<long> KitchenId { get; set; }
        public byte IsExtra { get; set; }
    }

    /// <summary>
    /// Get's Code, Description and Ids from client to set To DA Order
    /// </summary>
    public class ClientsIDsAndDescrModel
    {
        public long StoreProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<long> ProductCategoryId { get; set; }
        public string ProductCategory { get; set; }
        public Nullable<long> CategoryId { get; set; }
        public string Category { get; set; }
        public Nullable<long> UnitId { get; set; }
        public string Unit { get; set; }
        public Nullable<long> StorePriceListId { get; set; }
        public Nullable<long> PriceListDetailId { get; set; }
        public string PriceList { get; set; }
        public Nullable<int> PreparationTime { get; set; }
        public Nullable<long> KdsId { get; set; }
        public Nullable<long> KitchenId { get; set; }
        public byte IsExtra { get; set; }
    }
}