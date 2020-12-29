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
    [Table("TR_ExcludeRestrictions")]
    [DisplayName("TR_ExcludeRestrictions")]
    public class TR_ExcludeRestrictionsDTO
    {
        /// <summary>
        /// Id Record key
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TR_ExcludeRestrictions")]
        public long Id { get; set; }

        [Column("RestRestrictAssocId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_TR_ExcludeRestrictions_TR_Restrictions_Restaurants_Assoc")]
        [Association("TR_Restrictions_Restaurants_Assoc", "RestRestrictAssocId", "Id")]
        public long RestRestrictAssocId { get; set; }

        /// <summary>
        /// TR_Restaurants.Id
        /// </summary>
        [Column("RestId", Order = 3, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_TR_ExcludeRestrictions_TR_Restaurants")]
        [Association("TR_Restaurants", "RestId", "Id")]
        public long RestId { get; set; }

        [Column("Date", Order = 4, TypeName = "DATE")]
        [Required]
        public DateTime Date { get; set; }

    }
}
