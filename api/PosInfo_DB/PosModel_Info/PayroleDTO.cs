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
    [Table("Payrole")]
    [DisplayName("Payrole")]
    public class PayroleDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Payrole")]
        public long Id { get; set; }

        [Column("Identification", Order = 2, TypeName = "NVARCHAR(250)")]
        [Required]
        public string Identification { get; set; }

        [Column("ActionDate", Order = 3, TypeName = "DATETIME")]
        [Required]
        public System.DateTime ActionDate { get; set; }

        [Column("Type", Order = 4, TypeName = "INT")]
        [Required]
        public int Type { get; set; }

        [Column("PosInfoId", Order = 5, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_Payrole_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]
        public long PosInfoId { get; set; }

        [Column("StaffId", Order = 6, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_Payrole_Staff")]
        [Association("Staff", "StaffId", "Id")]
        public long StaffId { get; set; }

        [Column("ShopId", Order = 7, TypeName = "NVARCHAR(250)")]
        public string ShopId { get; set; }
    }
}
