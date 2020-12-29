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
    [Table("Combo")]
    [DisplayName("Combo")]
    public class ComboDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Combo")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Description { get; set; }

        [Column("StartDate", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> StartDate { get; set; }

        [Column("EndDate", Order = 4, TypeName = "DATETIME")]
        public Nullable<System.DateTime> EndDate { get; set; }

        [Column("ProductComboId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_Combo_Product")]
        [Association("Product", "ProductComboId", "Id")]
        public long ProductComboId { get; set; }

        [Column("DepartmentId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_Combo_Department")]
        [Association("Department", "DepartmentId", "Id")]
        public long DepartmentId { get; set; }

        [Column("StartTime", Order = 7, TypeName = "DATETIME")]
        public Nullable<System.DateTime> StartTime { get; set; }

        [Column("EndTime", Order = 8, TypeName = "DATETIME")]
        public Nullable<System.DateTime> EndTime { get; set; }

    }
}
