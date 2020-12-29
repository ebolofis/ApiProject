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
    [Table("TR_Restrictions")]
    [DisplayName("TR_Restrictions")]
    public class TR_RestrictionsDTO
    {
        /// <summary>
        /// HardCode Id
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        //[Editable(false)]
        [DisplayName("PK_TR_Restrictions")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(200)")]
        public string Description { get; set; }
    }
}
