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
    [Table("TR_Restrictions_Restaurants_Assoc")]
    [DisplayName("TR_Restrictions_Restaurants_Assoc")]
    public class TR_Restrictions_Restaurants_AssocDTO
    {
        /// <summary>
        /// Id Record key
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TR_Restrictions_Restaurants_Assoc")]
        public long Id { get; set; }

        /// <summary>
        /// TR_Restrictions.Id
        /// </summary>
        [Column("RestrictId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_TR_Restrictions_Restaurants_Assoc_TR_Restrictions")]
        [Association("TR_Restrictions", "RestrictId", "Id")]
        public long RestrictId { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Column("RestId", Order = 3, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_TR_Restrictions_Restaurants_Assoc_TR_Restaurants")]
        [Association("TR_Restaurants", "RestId", "Id")]
        public long RestId { get; set; }

        /// <summary>
        /// Restriction number
        /// </summary>
        [Column("N", Order = 4, TypeName = "INT")]
        [Required]
        public int N { get; set; }
    }
}
