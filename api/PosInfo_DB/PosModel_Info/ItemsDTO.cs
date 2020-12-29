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
    [Table("Items")]
    [DisplayName("Items")]
    public class ItemsDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Items")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Description { get; set; }

        [Column("ExtendedDescription", Order = 3, TypeName = "NVARCHAR(500)")]
        public string ExtendedDescription { get; set; }

        [Column("Qty", Order = 4, TypeName = "FLOAT")]
        public Nullable<double> Qty { get; set; }

        [Column("UnitId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_Items_Units")]
        [Association("Units", "UnitId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> UnitId { get; set; }

        [Column("VatId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_Items_Vat")]
        [Association("Vat", "VatId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> VatId { get; set; }
    }
}
