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
    [Table("ExternalUsers")]
    [DisplayName("ExternalUsers")]
    public class ExternalUsersDTO
    {
        [Column("Username", Order = 1, TypeName = "NVARCHAR(50)")]
        [Key]
        [DisplayName("PK_ExternalUsers")]
        public string Username { get; set; }

        [Column("Password", Order = 2, TypeName = "NVARCHAR(50)")]
        [Required]
        public string Password { get; set; }
    }
}
