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
    [Table("TR_ExcludeDays")]
    [DisplayName("TR_ExcludeDays")]
    public class TR_ExcludeDaysDTO
    {
        /// <summary>
        /// Id Record Key
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TR_ExcludeDays")]
        public long Id { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Column("RestId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_TR_ExcludeDays_TR_Restaurants")]
        [Association("TR_Restaurants", "RestId", "Id")]
        public long RestId { get; set; }

        /// <summary>
        /// Date of Unavailability
        /// </summary>
        [Column("Date", Order = 3, TypeName = "Date")]
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// 0:AllDay,1:Lunch,2:Dinner
        /// </summary>
        [Column("Type", Order = 4, TypeName = "INT")]
        [Required]
        public int Type { get; set; }
    }
}
