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
    [Table("Loyalty")]
    [DisplayName("Loyalty")]
    public class LoyaltyDTO
    {
        [Column("Id", Order = 8, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Loyalty")]
        public long Id { get; set; }


        [Column("Day", Order = 1, TypeName = "DATETIME")]
        [Required]
        public DateTime DateTime { get; set; }

        [Column("LoyalltyId", Order = 1, TypeName = "NVARCHAR(50)")]
        public string LoyalltyId { get; set; }

        [Column("CouponCode", Order = 2, TypeName = "NVARCHAR(50)")]
        public string CouponCode { get; set; }

        [Column("GiftcardCode", Order = 3, TypeName = "NVARCHAR(50)")]
        public string GiftcardCode { get; set; }

        [Column("CouponType", Order = 4, TypeName = "NVARCHAR(50)")]
        public string CouponType { get; set; }

        [Column("Campaign", Order = 5, TypeName = "NVARCHAR(50)")]
        public string Campaign { get; set; }

        [Column("Channel", Order = 6, TypeName = "NVARCHAR(50)")]
        [Required]
        public string Channel { get; set; }

        [Column("InvoicesId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_Loyalty_Invoices")]
        [Association("Invoices", "InvoicesId", "Id")]
        public Nullable<long> InvoicesId { get; set; }

        [Column("ErrorDescription", Order = 9, TypeName = "TEXT")]
        public string ErrorDescription { get; set; }

        [Column("GiftCardCouponType", Order = 10, TypeName = "NVARCHAR(50)")]
        public string GiftCardCouponType { get; set; }

        [Column("GiftCardCampaign", Order = 11, TypeName = "NVARCHAR(50)")]
        public string GiftCardCampaign { get; set; }

        [Column("DAOrderId", Order = 12, TypeName = "BIGINT")]
        [ForeignKey("FK_Loyalty_DA_Orders")]
        [Association("DA_Orders", "DAOrderId", "Id")]
        public Nullable<long> DAOrderId { get; set; }

    }
}
