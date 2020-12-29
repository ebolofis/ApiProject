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
    [Table("DeliveryRoutingHist")]
    [DisplayName("DeliveryRoutingHist")]
    public class DeliveryRoutingHistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_DeliveryRoutingHist")]
        [Association("nYear", "Id", "")]
        [DefaultValue(1900)]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("CreateDate", Order = 3, TypeName = "DATETIME")]
        public Nullable<DateTime> CreateDate { get; set; }

        [Column("Orders", Order = 4, TypeName = "INT")]
        [DefaultValue(0)]
        public int Orders { get; set; }

        [Column("StaffId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }

        [Column("StaffName", Order = 6, TypeName = "NVARCHAR(150)")]
        public string StaffName { get; set; }

        [Column("AssignDate", Order = 7, TypeName = "DATETIME")]
        public Nullable<DateTime> AssignDate { get; set; }

        [Column("AcceptDate", Order = 8, TypeName = "DATETIME")]
        public Nullable<DateTime> AcceptDate { get; set; }

        [Column("RejectedNames", Order = 9, TypeName = "NVARCHAR(MAX)")]
        public string RejectedNames { get; set; }

        [Column("Status", Order = 10, TypeName = "INT")]
        [DefaultValue(3)]
        public int Status { get; set; }

        [Column("AssignStatus", Order = 11, TypeName = "INT")]
        [DefaultValue(0)]
        public int AssignStatus { get; set; }

        [Column("ReturnDate", Order = 12, TypeName = "DATETIME")]
        public Nullable<DateTime> ReturnDate { get; set; }

        [Column("Failure3th", Order = 13, TypeName = "BIT")]
        public Nullable<bool> Failure3th { get; set; }
    }
}
