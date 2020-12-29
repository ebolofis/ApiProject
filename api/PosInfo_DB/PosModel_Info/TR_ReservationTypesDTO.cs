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
    [DisplayName("TR_ReservationTypes")]
    [Table("TR_ReservationTypes")]
    public class TR_ReservationTypesDTO
    {
        /// <summary>
        /// Id key
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TR_ReservationTypes")]
        public long Id { get; set; }

        /// <summary>
        /// Reservation type description
        /// </summary>
        [Column("Description", Order = 2, TypeName = "NVARCHAR(200)")]
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Reservation type
        /// </summary>
        [Column("Type", Order = 3, TypeName = "INT")]
        [Required]
        public int Type { get; set; }
    }
}
