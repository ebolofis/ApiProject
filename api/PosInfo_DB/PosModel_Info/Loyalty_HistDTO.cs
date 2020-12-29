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
    [Table("Loyalty_Hist")]
    [DisplayName("Loyalty_Hist")]
    public class Loyalty_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_Loyalty_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }


        [Column("Id", Order = 8, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }


        [Column("Day", Order = 1, TypeName = "DATETIME")]
        [Required]
        public System.DateTime DateTime { get; set; }

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
        public string Channel { get; set; }

        [Column("InvoicesId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> InvoicesId { get; set; }

        [Column("ErrorDescription", Order = 9, TypeName = "TEXT")]
        public string ErrorDescription { get; set; }

        [Column("GiftCardCouponType", Order = 10, TypeName = "NVARCHAR(50)")]
        public string GiftCardCouponType { get; set; }

        [Column("GiftCardCampaign", Order = 11, TypeName = "NVARCHAR(50)")]
        public string GiftCardCampaign { get; set; }

        [Column("DAOrderId", Order = 12, TypeName = "BIGINT")]
        public Nullable<long> DAOrderId { get; set; }
    }
}
