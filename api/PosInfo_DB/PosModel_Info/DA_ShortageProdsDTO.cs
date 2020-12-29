using Symposium.Models.Enums;
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
    [Table("DA_ShortageProds")]
    [DisplayName("DA_ShortageProds")]
    public class DA_ShortageProdsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_ShortageProds")]
        public long Id { get; set; }

        [Column("ProductId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DA_ShortageProds_Product")]
        [Association("Product", "ProductId", "Id")]
        public long ProductId { get; set; }

        [Column("StoreId", Order = 3, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DA_ShortageProds_DA_Stores")]
        [Association("DA_Stores", "StoreId", "Id")]
        public long StoreId { get; set; }

        /// <summary>
        /// 0: προσωρινή, 1: μόνημη
        /// </summary>
        [Column("ShortType", Order = 4, TypeName = "SMALLINT")]
        [Required]
        public DAShortageEnum ShortType { get; set; }
    }
}
