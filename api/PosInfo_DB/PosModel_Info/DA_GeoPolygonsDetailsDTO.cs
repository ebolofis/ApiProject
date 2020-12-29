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
    [Table("DA_GeoPolygonsDetails")]
    [DisplayName("DA_GeoPolygonsDetails")]
    public class DA_GeoPolygonsDetailsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_GeoPolygonsDetails")]
        public long Id { get; set; }

        /// <summary>
        /// DA_GeoPolygons.Id
        /// </summary>
        [Column("PolygId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DA_GeoPolygonsDetails_DA_GeoPolygons")]
        [Association("DA_GeoPolygons", "PolygId", "Id")]
        public long PolygId { get; set; }

        [Column("Latitude", Order = 3, TypeName = "FLOAT")]
        [Required]
        public float Latitude { get; set; }

        [Column("Longtitude", Order = 4, TypeName = "FLOAT")]
        [Required]
        public float Longtitude { get; set; }
    }
}
