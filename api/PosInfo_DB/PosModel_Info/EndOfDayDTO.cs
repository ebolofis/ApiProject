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
    [Table("EndOfDay")]
    [DisplayName("EndOfDay")]
    public class EndOfDayDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_EndOfDay")]
        public long Id { get; set; }

        [Column("FODay", Order = 2, TypeName = "DATETIME")]
        public Nullable<System.DateTime> FODay { get; set; }

        [Column("PosInfoId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_EndOfDay_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PosInfoId { get; set; }

        [Column("CloseId", Order = 4, TypeName = "INT")]
        public Nullable<int> CloseId { get; set; }

        [Column("Gross", Order = 5, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Gross { get; set; }

        [Column("Net", Order = 6, TypeName = "DECIMAL(12,4)")]
        public Nullable<decimal> Net { get; set; }

        [Column("TicketsCount", Order = 7, TypeName = "INT")]
        public Nullable<int> TicketsCount { get; set; }

        [Column("ItemCount", Order = 8, TypeName = "INT")]
        public Nullable<int> ItemCount { get; set; }

        [Column("TicketAverage", Order = 9, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> TicketAverage { get; set; }

        [Column("StaffId", Order = 10, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }

        [Column("Discount", Order = 11, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> Discount { get; set; }

        [Column("Barcodes", Order = 12, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> Barcodes { get; set; }

        [Column("dtDateTime", Order = 13, TypeName = "DATETIME")]
        public Nullable<System.DateTime> dtDateTime { get; set; }

        [Column("zlogger", Order = 14, TypeName = "NVARCHAR(120)")]
        public string zlogger { get; set; }

        [Column("eodPMS", Order = 15, TypeName = "DATETIME")]
        public Nullable<System.DateTime> eodPMS { get; set; }
    }
}
