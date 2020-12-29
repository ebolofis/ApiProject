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
    [Table("KitchenInstructionLogger_Hist")]
    [DisplayName("KitchenInstructionLogger_Hist")]
    public class KitchenInstructionLogger_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_KitchenInstructionLogger_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("KicthcenInstuctionId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> KicthcenInstuctionId { get; set; }

        [Column("StaffId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }

        [Column("PdaModulId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> PdaModulId { get; set; }

        [Column("ClientId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> ClientId { get; set; }

        [Column("PosInfoId", Order = 6, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("SendTS", Order = 7, TypeName = "DATETIME")]
        public Nullable<System.DateTime> SendTS { get; set; }

        [Column("ReceivedTS", Order = 8, TypeName = "DATETIME")]
        public Nullable<System.DateTime> ReceivedTS { get; set; }

        [Column("EndOfDayId", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> EndOfDayId { get; set; }

        [Column("Status", Order = 7, TypeName = "SMALLINT")]
        public Nullable<short> Status { get; set; }

        [Column("TableId", Order = 11, TypeName = "BIGINT")]
        public Nullable<long> TableId { get; set; }

        [Column("ExtecrName", Order = 12, TypeName = "NVARCHAR(200)")]
        public string ExtecrName { get; set; }

        [Column("Description", Order = 13, TypeName = "NVARCHAR(2000)")]
        public string Description { get; set; }
    }
}
