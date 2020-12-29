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
    [DisplayName("TR_Capacities")]
    [Table("TR_Capacities")]
    public class TR_CapacitiesDTO
    {
        /// <summary>
        /// Id Key
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TR_Capacities")]
        public long Id { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Column("RestId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_TR_Capacities_TR_Restaurants")]
        [Association("TR_Restaurants", "RestId", "Id")]
        public long RestId { get; set; }

        /// <summary>
        /// num of max customers
        /// </summary>
        [Column("Capacity", Order = 3, TypeName = "INT")]
        [Required]
        public int Capacity { get; set; }

        /// <summary>
        /// 0:AllDay,1:Lunch,2:Dinner
        /// </summary>
        [Column("Type", Order = 4, TypeName = "INT")]
        [Required]
        public int Type { get; set; }

        /// <summary>
        /// Time ex: 23:30
        /// </summary>
        [Column("Time", Order = 5, TypeName = "TIME(7)")]
        [Required]
        public TimeSpan Time { get; set; }


    }
}
