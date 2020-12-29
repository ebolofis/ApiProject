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
    [Table("EndOfDay_Hist")]
    [DisplayName("EndOfDay_Hist")]
    public class EndOfDay_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_EndOfDay_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("FODay", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> FODay { get; set; }

        [Column("PosInfoId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("CloseId", Order = 5, TypeName = "INT")]
        public Nullable<int> CloseId { get; set; }

        [Column("Gross", Order = 6, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Gross { get; set; }

        [Column("Net", Order = 7, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Net { get; set; }

        [Column("TicketsCount", Order = 8, TypeName = "INT")]
        public Nullable<int> TicketsCount { get; set; }

        [Column("ItemCount", Order = 9, TypeName = "INT")]
        public Nullable<int> ItemCount { get; set; }

        [Column("TicketAverage", Order = 10, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> TicketAverage { get; set; }

        [Column("StaffId", Order = 11, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }

        [Column("Discount", Order = 12, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> Discount { get; set; }

        [Column("Barcodes", Order = 13, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> Barcodes { get; set; }

        [Column("dtDateTime", Order = 14, TypeName = "DATETIME")]
        public Nullable<System.DateTime> dtDateTime { get; set; }

        [Column("zlogger", Order = 15, TypeName = "NVARCHAR(120)")]
        public string zlogger { get; set; }

        [Column("eodPMS", Order = 16, TypeName = "DATETIME")]
        public Nullable<System.DateTime> eodPMS { get; set; }
    }
}
