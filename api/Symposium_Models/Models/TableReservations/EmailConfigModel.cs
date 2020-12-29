using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class EmailConfigModel
    {
        /// <summary>
        /// Id record key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Smtp Server
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Smtp { get; set; }

        /// <summary>
        /// Port for Smtp Server
        /// </summary>
        [Range(1, 65535)]
        [Required]
        public int Port { get; set; }

        /// <summary>
        /// Ssl
        /// </summary>
        [Required]
        public bool Ssl { get; set; }

        /// <summary>
        /// Username for Server connection
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Email Sender
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Sender { get; set; }

        /// <summary>
        /// Email Sender
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// If record is active
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }

    public class EmailSendModel
    {
        [EmailAddress]
        /// <summary>
        /// From email address
        /// </summary>
        public string From { get; set; }


        /// <summary>
        /// List of email addresses to send email
        /// </summary>
        public List<string> To { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Email body
        /// </summary>
        public string Body { get; set; }
    }
}
