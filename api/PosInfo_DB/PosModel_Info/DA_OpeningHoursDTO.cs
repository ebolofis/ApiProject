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

    [Table("DA_OpeningHours")]
    [DisplayName("DA_OpeningHours")]
    public class DA_OpeningHoursDTO
    {
        /// <summary>
        /// PK
        /// </summary>
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_OpeningHours")]
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// Id καταστήματος
        /// </summary>
        [Column("StoreId", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long StoreId { get; set; }


        /// <summary>
        /// Ημέρα εβδομάδας
        /// </summary>
        [Column("Weekday", Order = 3, TypeName = "INT")]
        [Required]
        public int Weekday { get; set; }

        /// <summary>
        /// Ώρα ανοίγματος
        /// </summary>
        [Column("OpenHour", Order = 4, TypeName = "INT")]
        [Required]
        public int OpenHour { get; set; }


        /// <summary>
        /// Λεπτό ανοίγματος
        /// </summary>
        [Column("OpenMinute", Order = 5, TypeName = "INT")]
        [Required]
        public int OpenMinute { get; set; }

        /// <summary>
        /// Λεπτό ανοίγματος
        /// </summary>
        [Column("CloseHour", Order = 6, TypeName = "INT")]
        [Required]
        public int CloseHour { get; set; }

        /// <summary>
        /// Λεπτό ανοίγματος
        /// </summary>
        [Column("CloseMinute", Order = 7, TypeName = "INT")]
        [Required]
        public int CloseMinute { get; set; }
    }
}
