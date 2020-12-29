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
    [Table("OrderDetailVatAnal")]
    [DisplayName("OrderDetailVatAnal")]
    public class OrderDetailVatAnalDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_OrderDetailVatAnal")]
        public long Id { get; set; }

        [Column("OrderDetailId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetailVatAnal_OrderDetail")]
        [Association("OrderDetail", "OrderDetailId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> OrderDetailId { get; set; }

        [Column("Gross", Order = 3, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Gross { get; set; }

        [Column("Net", Order = 4, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Net { get; set; }

        [Column("VatRate", Order = 5, TypeName = "DECIMAL(8,4)")]
        public Nullable<decimal> VatRate { get; set; }

        [Column("VatAmount", Order = 6, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> VatAmount { get; set; }

        [Column("VatId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> VatId { get; set; }

        [Column("TaxId", Order = 8, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetailVatAnal_Tax")]
        [Association("Tax", "TaxId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> TaxId { get; set; }

        [Column("TaxAmount", Order = 9, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> TaxAmount { get; set; }

        [Column("IsDeleted", Order = 10, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

    }
}
