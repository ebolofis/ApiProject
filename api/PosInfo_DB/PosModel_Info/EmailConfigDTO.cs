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
    [Table("EmailConfig")]
    [DisplayName("EmailConfig")]
    public class EmailConfigDTO
    {
        /// <summary>
        /// Id Record key
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_EmailConfig")]
        public long Id { get; set; }

        /// <summary>
        /// Smtp Server
        /// </summary>
        [Column("Smtp", Order = 2, TypeName = "NVARCHAR(100)")]
        [Required]
        public string Smtp { get; set; }

        /// <summary>
        /// Port for Smtp Server
        /// </summary>
        [Column("Port", Order = 3, TypeName = "INT")]
        [Required]
        public int Port { get; set; }

        /// <summary>
        /// Ssl
        /// </summary>
        [Column("Ssl", Order = 4, TypeName = "BIT")]
        [Required]
        public bool Ssl { get; set; }

        /// <summary>
        /// Username for Server connection
        /// </summary>
        [Column("Username", Order = 5, TypeName = "NVARCHAR(100)")]
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Email Sender
        /// </summary>
        [Column("Sender", Order = 6, TypeName = "NVARCHAR(100)")]
        [Required]
        public string Sender { get; set; }

        /// <summary>
        /// Email Sender
        /// </summary>
        [Column("Password", Order = 7, TypeName = "VARBINARY(MAX)")]
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// If record is active
        /// </summary>
        [Column("IsActive", Order = 8, TypeName = "BIT")]
        [Required]
        public bool IsActive { get; set; }
    }
}
