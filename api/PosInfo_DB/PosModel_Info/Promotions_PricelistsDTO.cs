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
    [Table("Promotions_Pricelists")]
    [DisplayName("Promotions_Pricelists")]
    public class Promotions_PricelistsDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        public long Id { get; set; }

        /// <summary>
        /// PricelistId
        /// </summary>
        [Column("PricelistId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_Promotions_Pricelists_Pricelist")]
        [Association("Pricelist", "PricelistId", "Id")]
        public long PricelistId { get; set; }
    }
}
