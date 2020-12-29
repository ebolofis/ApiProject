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
    [Table("PredefinedCredits")]
    [DisplayName("PredefinedCredits")]
    public class PredefinedCreditsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PredefinedCredits")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("Amount", Order = 3, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> Amount { get; set; }

        [Column("InvoiceTypeId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_PredefinedCredits_InvoiceTypes")]
        [Association("InvoiceTypes", "InvoiceTypeId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> InvoiceTypeId { get; set; }
    }
}
