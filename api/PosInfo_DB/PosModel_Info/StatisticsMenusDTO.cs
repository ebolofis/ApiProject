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
    [Table("StatisticsMenus")]
    [DisplayName("StatisticsMenus")]
    public class StatisticsMenusDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_SatisticsMenus")]
        public long Id { get; set; }

        [Column("MenuDescription", Order = 2, TypeName = "NVARCHAR(250)")]
        public string MenuDescription { get; set; }

        [Column("CategoryOrder", Order = 3, TypeName = "SMALLINT")]
        public Nullable<short> CategoryOrder { get; set; }

        [Column("MenuItemDescription", Order = 4, TypeName = "NVARCHAR(250)")]
        public string MenuItemDescription { get; set; }

        [Column("MenuItemOrder", Order = 5, TypeName = "SMALLINT")]
        public Nullable<short> MenuItemOrder { get; set; }

        [Column("MenuLevel", Order = 6, TypeName = "SMALLINT")]
        public Nullable<short> MenuLevel { get; set; }

        [Column("ReportListId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_SatisticsMenus_ReportList")]
        [Association("ReportList", "ReportListId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ReportListId { get; set; }

        [Column("ParentMenuId", Order = 8, TypeName = "BIGINT")]
        public Nullable<long> ParentMenuId { get; set; }

        [Column("Url", Order = 9, TypeName = "NVARCHAR(2048)")]
        public string Url { get; set; }
    }
}
