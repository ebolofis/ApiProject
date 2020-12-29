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
    [Table("TablePaySuggestion_Hist")]
    [DisplayName("TablePaySuggestion_Hist")]
    public class TablePaySuggestion_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_TablePaySuggestion_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("AccountId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> AccountId { get; set; }

        [Column("GuestId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> GuestId { get; set; }

        [Column("Amount", Order = 4, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Amount { get; set; }

        [Column("OrderDetailId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> OrderDetailId { get; set; }

        [Column("IsUsed", Order = 6, TypeName = "BIT")]
        public Nullable<bool> IsUsed { get; set; }

        [Column("CreditCodeId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> CreditCodeId { get; set; }
    }
}
