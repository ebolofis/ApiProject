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
    [Table("TR_OvewrittenCapacities")]
    [DisplayName("TR_OvewrittenCapacities")]
    public class TR_OvewrittenCapacitiesDTO
    {
        /// <summary>
        /// Id record key
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TR_OvewrittenCapacities")]
        public long Id { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Column("RestId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_TR_OvewrittenCapacities_TR_Restaurants")]
        [Association("TR_Restaurants", "RestId", "Id")]
        public long RestId { get; set; }

        /// <summary>
        /// TR_Capacities.Id
        /// </summary>
        [Column("CapacityId", Order = 3, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_TR_OvewrittenCapacities_TR_Capacities")]
        [Association("TR_Capacities", "CapacityId", "Id")]
        public long CapacityId { get; set; }

        /// <summary>
        /// num of max customers
        /// </summary>
        [Column("Capacity", Order = 4, TypeName = "INT")]
        [Required]
        public int Capacity { get; set; }

        /// <summary>
        /// Date of overwrite
        /// </summary>
        [Column("Date", Order = 5, TypeName = "DATE")]
        [Required]
        public DateTime Date { get; set; }
    }
}
