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
    [Table("TR_Restaurants")]
    [DisplayName("TR_Restaurants")]
    public class TR_RestaurantsDTO
    {
        /// <summary>
        /// Id record key
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TR_Restaurants")]
        public long Id { get; set; }


        [Column("NameGR", Order = 2, TypeName = "nvarchar(200)")]
        [Required]
        public string NameGR { get; set; }

        [Column("NameEn", Order = 3, TypeName = "nvarchar(200)")]
        [Required]
        public string NameEn { get; set; }

        [Column("NameRu", Order = 4, TypeName = "nvarchar(200)")]
        [Required]
        public string NameRu { get; set; }

        [Column("NameFr", Order = 5, TypeName = "nvarchar(200)")]
        [Required]
        public string NameFr { get; set; }

        [Column("NameDe", Order = 6, TypeName = "nvarchar(200)")]
        [Required]
        public string NameDe { get; set; }

        [Column("Image", Order = 7, TypeName = "nvarchar(200)")]
        public string Image { get; set; }

        [Column("DescriptionGR", Order = 8, TypeName = "TEXT")]
        [Required]
        public string DescriptionGR { get; set; }

        [Column("DescriptionEn", Order = 9, TypeName = "TEXT")]
        [Required]
        public string DescriptionEn { get; set; }

        [Column("DescriptionRu", Order = 10, TypeName = "TEXT")]
        [Required]
        public string DescriptionRu { get; set; }

        [Column("DescriptionFr", Order = 11, TypeName = "TEXT")]
        [Required]
        public string DescriptionFr { get; set; }

        [Column("DescriptionDe", Order = 12, TypeName = "TEXT")]
        [Required]
        public string DescriptionDe { get; set; }
    }
}
