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
    [Table("DA_LoyalRedeemFreeProduct")]
    [DisplayName("DA_LoyalRedeemFreeProduct")]
    public class DA_LoyalRedeemFreeProductDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_LoyalRedeemFreeProduct")]
        public long Id { get; set; }

        /// <summary>
        /// πόντοι εξαργύρωσης
        /// </summary>
        [Column("Points", Order = 2, TypeName = "INT")]
        [Required]
        public int Points { get; set; }

        /// <summary>
        /// δωρεάν είδος ή
        /// </summary>
        [Column("ProductId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_LoyalRedeemFreeProduct_Product")]
        [Association("Product", "ProductId", "Id")]
        public Nullable<long> ProductId { get; set; }

        [Column("ProductName", Order = 4, TypeName = "NVARCHAR(500)")]
        public string ProductName { get; set; }

        /// <summary>
        /// δωρεάν είδος από κατηγρία προϊόντων
        /// </summary>
        [Column("ProdCategoryId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_LoyalRedeemFreeProduct_ProductCategories")]
        [Association("ProductCategories", "ProdCategoryId", "Id")]
        public Nullable<long> ProdCategoryId { get; set; }

        [Column("ProdCategoryName", Order = 6, TypeName = "NVARCHAR(500)")]
        public string ProdCategoryName { get; set; }

        [Column("Qnt", Order = 7, TypeName = "DECIMAL(12,4)")]
        [Required]
        public decimal Qnt { get; set; }
    }
}
