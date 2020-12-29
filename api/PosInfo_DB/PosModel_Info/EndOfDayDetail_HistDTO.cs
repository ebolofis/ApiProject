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
    [Table("EndOfDayDetail_Hist")]
    [DisplayName("EndOfDayDetail_Hist")]
    public class EndOfDayDetail_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_EndOfDayDetail_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("EndOfdayId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> EndOfdayId { get; set; }

        [Column("VatId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> VatId { get; set; }

        [Column("VatRate", Order = 5, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> VatRate { get; set; }

        [Column("VatAmount", Order = 6, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> VatAmount { get; set; }

        [Column("TaxId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> TaxId { get; set; }

        [Column("TaxAmount", Order = 8, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> TaxAmount { get; set; }

        [Column("Gross", Order = 9, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Gross { get; set; }

        [Column("Net", Order = 10, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Net { get; set; }

        [Column("Discount", Order = 11, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> Discount { get; set; }
    }
}
