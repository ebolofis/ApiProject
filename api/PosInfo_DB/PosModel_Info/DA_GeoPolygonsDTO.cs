using Microsoft.SqlServer.Types;
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
    [DisplayName("DA_GeoPolygons")]
    [Table("DA_GeoPolygons")]
    public class DA_GeoPolygonsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_GeoPolygons")]
        public long Id { get; set; }

        /// <summary>
        /// κατάστημα στο οποίο ανήκει το πολ. (μπορεί να μη ανήκει σε κανένα πολύγωνο)
        /// </summary>
        [Column("StoreId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_GeoPolygons_DA_Stores")]
        [Association("DA_Stores", "StoreId", "Id")]
        public Nullable<long> StoreId { get; set; }

        [Column("Name", Order = 3, TypeName = "NVARCHAR(200)")]
        [Required]
        public string Name { get; set; }

        [Column("Notes", Order = 4, TypeName = "NVARCHAR(1500)")]
        public string Notes { get; set; }

        [Column("IsActive", Order = 5, TypeName = "BIT")]
        [Required]
        public bool IsActive { get; set; }

        [Column("Shape", Order = 6, TypeName = "GEOMETRY")]
        public SqlGeometry Shape { get; set; }
    }
}
