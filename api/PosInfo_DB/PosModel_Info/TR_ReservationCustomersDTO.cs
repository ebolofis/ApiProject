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
    [Table("TR_ReservationCustomers")]
    [DisplayName("TR_ReservationCustomers")]
    public class TR_ReservationCustomersDTO
    {
        /// <summary>
        /// Id Record Key
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TR_ReservationCustomers")]
        public long Id { get; set; }

        /// <summary>
        /// Protel Profile Id
        /// </summary>
        [Column("ProtelId", Order = 1, TypeName = "INT")]
        [Required]
        public long ProtelId { get; set; }

        /// <summary>
        /// ProtelName (encrypted)
        /// </summary>
        [Column("ProtelName", Order = 1, TypeName = "varbinary(MAX)")]
        [Required]
        public string ProtelName { get; set; }

        /// <summary>
        /// Name given by the customer (encrypted)
        /// </summary>
        [Column("ReservationName", Order = 1, TypeName = "varbinary(MAX)")]
        [Required]
        public string ReservationName { get; set; }

        /// <summary>
        /// Room number
        /// </summary>
        [Column("RoomNum", Order = 1, TypeName = "nvarchar(10)")]
        [Required]
        public string RoomNum { get; set; }

        /// <summary>
        /// email (encrypted)
        /// </summary>
        [Column("Email", Order = 1, TypeName = "varbinary(MAX)")]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// TR_Reservations.Id
        /// </summary>
        [Column("ReservationId", Order = 1, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_TR_ReservationCustomers_TR_Reservations")]
        [Association("TR_Reservations", "ReservationId", "Id")]
        public long ReservationId { get; set; }

        /// <summary>
        /// Hotel info index
        /// </summary>
        [Column("HotelId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_TR_ReservationCustomers_HotelInfo")]
        [Association("HotelInfo", "HotelId", "Id")]
        public long HotelId { get; set; }
    }
}
