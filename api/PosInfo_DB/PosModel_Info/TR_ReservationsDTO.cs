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
    [Table("TR_Reservations")]
    [DisplayName("TR_Reservations")]
    public class TR_ReservationsDTO
    {
        /// <summary>
        /// Id Record key
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TR_Reservations")]
        public long Id { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Column("RestId", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long RestId { get; set; }

        /// <summary>
        /// TR_Capacities.Id
        /// </summary>
        [Column("CapacityId", Order = 3, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_TR_Reservations_TR_Capacities")]
        [Association("TR_Capacities", "CapacityId", "Id")]
        public long CapacityId { get; set; }

        /// <summary>
        /// Num of people
        /// </summary>
        [Column("Couver", Order = 4, TypeName = "INT")]
        [Required]
        public int Couver { get; set; }

        /// <summary>
        /// Reservation Date
        /// </summary>
        [Column("ReservationDate", Order = 5, TypeName = "DATE")]
        [Required]
        public DateTime ReservationDate { get; set; }

        /// <summary>
        /// TR_Capacities.Time
        /// </summary>
        [Column("ReservationTime", Order = 6, TypeName = "TIME(7)")]
        [Required]
        public TimeSpan ReservationTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CreateDate", Order = 7, TypeName = "DATETIME")]
        [Required]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 0: Active, 1: Cancel
        /// </summary>
        [Column("Status", Order = 8, TypeName = "INT")]
        [Required]
        public int Status { get; set; }


        /// <summary>
        ///Description
        /// </summary>
        [Column("Description", Order = 9, TypeName = "NVARCHAR(100)")]
        public string Description { get; set; }
    }
}
