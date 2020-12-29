using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.TableReservations
{
    public class ConfigModel
    {
        /// <summary>
        /// Id record key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Hμέρες προεπισκόπησης του E-mail
        /// </summary>
        [Required]
        public int PreviewDays { get; set; }

        /// <summary>
        /// Πρότυπα e-mail
        /// </summary>
        public string EmailTemplate { get; set; }

        /// <summary>
        /// Θέμα του e-mail
        /// </summary>
        [MaxLength(150)]
        public string EmailSubject { get; set; }

        /// <summary>
        /// Hotel info index
        /// </summary>
        public long DefaultHotelId { get; set; }

        /// <summary>
        /// Extcr to send receipts
        /// </summary>
        public string ExtECR { get; set; }
    }
}
