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
    [Table("EFoodBucket")]
    [DisplayName("EFoodBucket")]
   public class EFoodBucketDTO
    {
        /// <summary>
        /// Order id from efood ()
        /// </summary>
        [Column("Id", Order = 1, TypeName = "nvarchar(50)")]
        [Key]
        [Required]
        [DisplayName("PK_EFoodBucket")]
        public string Id { get; set; }

        /// <summary>
        /// Efood Order as json
        /// </summary>
        [Column("Json", Order = 2, TypeName = "text")]
        public string Json { get; set; }

        /// <summary>
        /// IsDeleted
        /// </summary>
        [Column("IsDeleted", Order = 3, TypeName = "bit")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        [Column("CreateDate", Order = 4, TypeName = "date")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Errors-Mismatches
        /// </summary>
        // [Column("Errors", Order = 3, TypeName = "text")]
        //  public string Errors { get; set; }
    }
}
