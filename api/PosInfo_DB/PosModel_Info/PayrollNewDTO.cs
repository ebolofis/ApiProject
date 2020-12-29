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
    [Table("PayrollNew")]
    [DisplayName("PayrollNew")]
    public class PayrollNewDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PayrollNew")]
        public long Id { get; set; }

        [Column("Identification", Order = 2, TypeName = "NVARCHAR(250)")]
        [Required]
        public string Identification { get; set; }

        [Column("DateFrom", Order = 3, TypeName = "DATETIME")]
        public Nullable<DateTime> DateFrom { get; set; }

        [Column("DateTo", Order = 4, TypeName = "DATETIME")]
        public Nullable<DateTime> DateTo { get; set; }

        [Column("PosInfoId", Order = 5, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_PayrollNew_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]
        public long PosInfoId { get; set; }

        [Column("StaffId", Order = 6, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_PayrollNew_Staff")]
        [Association("Staff", "StaffId", "Id")]
        public long StaffId { get; set; }

        [Column("ShopId", Order = 7, TypeName = "NVARCHAR(250)")]
        public string ShopId { get; set; }

        [Column("TotalHours", Order = 8, TypeName = "NVARCHAR(250)")]
        public string TotalHours { get; set; }

        [Column("TotalMinutes", Order = 9, TypeName = "BIGINT")]
        public long TotalMinutes { get; set; }

        [Column("IsSend", Order = 10, TypeName = "SMALLINT")]
        public Int16 IsSend { get; set; }

        [Column("IsDeleted", Order = 11, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }
    }
}
