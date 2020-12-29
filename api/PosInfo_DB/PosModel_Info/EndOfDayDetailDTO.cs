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
    [Table("EndOfDayDetail")]
    [DisplayName("EndOfDayDetail")]
    public class EndOfDayDetailDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_EndOfDayDetail")]
        public long Id { get; set; }

        [Column("EndOfdayId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_EndOfDayDetail_EndOfDay")]
        [Association("EndOfDay", "EndOfdayId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]//ON UPDATE CASCADE
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> EndOfdayId { get; set; }

        [Column("VatId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_EndOfDayDetail_Vat")]
        [Association("Vat", "VatId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> VatId { get; set; }

        [Column("VatRate", Order = 4, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> VatRate { get; set; }

        [Column("VatAmount", Order = 5, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> VatAmount { get; set; }

        [Column("TaxId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_EndOfDayDetail_Tax")]
        [Association("Tax", "TaxId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> TaxId { get; set; }

        [Column("TaxAmount", Order = 7, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> TaxAmount { get; set; }

        [Column("Gross", Order = 8, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Gross { get; set; }

        [Column("Net", Order = 9, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Net { get; set; }

        [Column("Discount", Order = 10, TypeName = "DECIMAL(9,4)")]
        public Nullable<decimal> Discount { get; set; }
    }
}
