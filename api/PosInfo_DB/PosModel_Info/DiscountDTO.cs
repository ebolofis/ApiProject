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
    [Table("Discount")]
    [DisplayName("Discount")]
    public class DiscountDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Discount")]
        public long Id { get; set; }

        [Column("Type", Order = 2, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("Amount", Order = 3, TypeName = "DECIMAL(9,2)")]
        public Nullable<decimal> Amount { get; set; }

        [Column("Status", Order = 4, TypeName = "BIT")]
        public Nullable<bool> Status { get; set; }

        [Column("Sort", Order = 5, TypeName = "INT")]
        public Nullable<int> Sort { get; set; }

        [Column("Description", Order = 6, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }
    }
}
