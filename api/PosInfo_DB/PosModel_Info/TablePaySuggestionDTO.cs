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
    [Table("TablePaySuggestion")]
    [DisplayName("TablePaySuggestion")]
    public class TablePaySuggestionDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TablePaySuggestion")]
        public long Id { get; set; }

        [Column("AccountId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_TablePaySuggestion_Accounts")]
        [Association("Accounts", "AccountId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> AccountId { get; set; }

        [Column("GuestId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_TablePaySuggestion_Guest")]
        [Association("Guest", "GuestId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> GuestId { get; set; }

        [Column("Amount", Order = 4, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Amount { get; set; }

        [Column("OrderDetailId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_TablePaySuggestion_OrderDetail")]
        [Association("OrderDetail", "OrderDetailId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> OrderDetailId { get; set; }

        [Column("IsUsed", Order = 6, TypeName = "BIT")]
        public Nullable<bool> IsUsed { get; set; }

        [Column("CreditCodeId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_TablePaySuggestion_CreditCodes")]
        [Association("CreditCodes", "CreditCodeId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(-1)]
        [MinLength(-1)]
        public Nullable<long> CreditCodeId { get; set; }
    }
}
