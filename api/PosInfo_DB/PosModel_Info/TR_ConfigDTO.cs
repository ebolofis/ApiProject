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
    [Table("TR_Config")]
    [DisplayName("TR_Config")]
    public class TR_ConfigDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TR_Config")]
        public long Id { get; set; }

        [Column("PreviewDays", Order = 2, TypeName = "INT")]
        [Required]
        public int PreviewDays { get; set; }

        [Column("EmailTemplate", Order = 3, TypeName = "TEXT")]
        public string EmailTemplate { get; set; }

        [Column("EmailSubject", Order = 4, TypeName = "NVARCHAR(150)")]
        public string EmailSubject { get; set; }

        /// <summary>
        /// Hotel info index
        /// </summary>
        [Column("DefaultHotelId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_TR_Config_HotelInfo")]
        [Association("HotelInfo", "DefaultHotelId", "Id")]
        public long DefaultHotelId { get; set; }

        /// <summary>
        /// Extcr to send receipts
        /// </summary>
        [Column("ExtECR", Order = 6, TypeName = "NVARCHAR(250)")]
        public string ExtECR { get; set; }

    }
}
