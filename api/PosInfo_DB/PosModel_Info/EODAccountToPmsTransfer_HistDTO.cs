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
    [Table("EODAccountToPmsTransfer_Hist")]
    [DisplayName("EODAccountToPmsTransfer_Hist")]
    public class EODAccountToPmsTransfer_HistDTO
    {
        [Column("nYear", Order = 1, TypeName = "INT")]
        [Key]
        [DisplayName("PK_EODAccountToPmsTransfer_Hist")]
        [Association("nYear", "Id", "")]
        public int nYear { get; set; }

        [Column("Id", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        [Column("PosInfoId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("AccountId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> AccountId { get; set; }

        [Column("PmsRoom", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> PmsRoom { get; set; }

        [Column("Description", Order = 6, TypeName = "NVARCHAR(300)")]
        public string Description { get; set; }

        [Column("Status", Order = 7, TypeName = "BIT")]
        public Nullable<bool> Status { get; set; }

        [Column("PmsDepartmentId", Order = 8, TypeName = "NVARCHAR(100)")]
        public string PmsDepartmentId { get; set; }

        [Column("PmsDepDescription", Order = 9, TypeName = "NVARCHAR(250)")]
        public string PmsDepDescription { get; set; }
    }
}
