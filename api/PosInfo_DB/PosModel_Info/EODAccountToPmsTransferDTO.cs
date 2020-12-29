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
    [Table("EODAccountToPmsTransfer")]
    [DisplayName("EODAccountToPmsTransfer")]
    public class EODAccountToPmsTransferDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_EODAccountToPmsTransfer")]
        public long Id { get; set; }

        [Column("PosInfoId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_EODAccountToPmsTransfer_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PosInfoId { get; set; }

        [Column("AccountId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_EODAccountToPmsTransfer_Accounts")]
        [Association("Accounts", "AccountId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> AccountId { get; set; }

        [Column("PmsRoom", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> PmsRoom { get; set; }

        [Column("Description", Order = 5, TypeName = "NVARCHAR(300)")]
        public string Description { get; set; }

        [Column("Status", Order = 6, TypeName = "BIT")]
        public Nullable<bool> Status { get; set; }

        [Column("PmsDepartmentId", Order = 7, TypeName = "NVARCHAR(100)")]
        public string PmsDepartmentId { get; set; }

        [Column("PmsDepDescription", Order = 8, TypeName = "NVARCHAR(250)")]
        public string PmsDepDescription { get; set; }
    }
}
