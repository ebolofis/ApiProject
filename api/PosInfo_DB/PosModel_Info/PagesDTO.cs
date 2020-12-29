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
    [Table("Pages")]
    [DisplayName("Pages")]
    public class PagesDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Pages")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("ExtendedDesc", Order = 3, TypeName = "NVARCHAR(500)")]
        public string ExtendedDesc { get; set; }

        [Column("Status", Order = 4, TypeName = "BIT")]
        public Nullable<bool> Status { get; set; }

        [Column("Sort", Order = 5, TypeName = "SMALLINT")]
        public Nullable<short> Sort { get; set; }

        [Column("DefaultPriceList", Order = 6, TypeName = "BIGINT")]
        public Nullable<long> DefaultPriceList { get; set; }

        [Column("PageSetId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_Pages_PageSet")]
        [Association("PageSet", "PageSetId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PageSetId { get; set; }

        [Column("DAId", Order = 8, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }
    }
}
