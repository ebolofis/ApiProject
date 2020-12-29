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
    [Table("DAStore_PriceListAssoc")]
    [DisplayName("DAStore_PriceListAssoc")]
    public class DAStore_PriceListAssocDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PriceList_DAStoreAssoc")]
        public long Id { get; set; }

        /// <summary>
        /// Pricelist.Id
        /// </summary>
        [Column("PriceListId", Order = 2, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DAStore_PriceListAssoc_Pricelist")]
        [Association("Pricelist", "PriceListId", "Id")]
        public long PriceListId { get; set; }

        /// <summary>
        /// DA_Store.Id
        /// </summary>
        [Column("DAStoreId", Order = 3, TypeName = "BIGINT")]
        [Required]
        [ForeignKey("FK_DAStore_PriceListAssoc_DA_Stores")]
        [Association("DA_Stores", "DAStoreId", "Id")]
        public long DAStoreId { get; set; }

        /// <summary>
        /// 20 Delivery, 21 TakeOut
        /// </summary>
        [Column("PriceListType", Order = 4, TypeName = "INT")]
        [Required]
        public int PriceListType { get; set; }
    }
}
