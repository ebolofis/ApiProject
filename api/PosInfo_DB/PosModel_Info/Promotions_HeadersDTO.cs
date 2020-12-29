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
    [Table("Promotions_Headers")]
    [DisplayName("Promotions_Headers")]
    public class Promotions_HeadersDTO
    {
        /// <summary>
        /// Promotion Header Id
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        public long Id { get; set; }

        /// <summary>
        /// Περιγραφή promo
        /// </summary>
        [Column("Description", Order = 2, TypeName = "NVARCHAR(1000)")]
        public string Description { get; set; }

        /// <summary>
        /// External code
        /// </summary>
        [Column("Code", Order = 3, TypeName = "NVARCHAR(100)")]
        public string Code { get; set; }


        /// <summary>
        /// discount % [0..100]
        /// </summary>
        [Column("Discount", Order = 4, TypeName = "DECIMAL(19,2)")]
        public Nullable<Decimal> Discount { get; set; }


        /// <summary>
        /// true: ρώτα τον χειριστή pos αν θα εφαρμόσει το promotion
        /// </summary>
        [Column("AskOperator", Order = 5, TypeName = "BIT")]
        public Nullable<bool> AskOperator { get; set; }

        /// <summary>
        /// σχολια για τον EXTCR
        /// </summary>
        [Column("ReceiptNote", Order = 6, TypeName = "NVARCHAR(1000)")]
        public string ReceiptNote { get; set; }

        /// <summary>
        /// 0: Discount all products, 1:cheepest only, 2: MostExpensive only, 3: UserDecision
        /// </summary>
        [Column("DiscountType", Order = 7, TypeName = "INT")]
        public Nullable<int> DiscountType { get; set; }

        /// <summary>
        /// true: το UI ζητά την εισαγωγή κωδικου κουπονιού
        /// </summary>
        [Column("AskCode", Order = 8, TypeName = "BIT")]
        public Nullable<bool> AskCode { get; set; }

        /// <summary>
        /// true: όταν Δεν Επιστρέπεται  διαγραφή και γίνεται Update, διαφορετικά false
        /// </summary>
        [Column("IsDeleted", Order = 9, TypeName = "BIT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_Promotions_Headers_IsDeleted", NullDisplayText = "0")]//DefaultValue (Name, Value)
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Agent Id saved to store
        /// </summary>
        [Column("DAId", Order = 10, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
