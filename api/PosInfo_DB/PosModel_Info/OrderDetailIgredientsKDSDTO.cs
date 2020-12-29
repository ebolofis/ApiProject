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
    [Table("OrderDetailIgredientsKDS")]
    [DisplayName("OrderDetailIgredientsKDS")]
    public class OrderDetailIgredientsKDSDTO
    {
        /// <summary>
        /// Table Id auto incrment
        /// </summary>
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_OrderDetailIgredientsKDS")]
        public long Id { get; set; }

        /// <summary>
        /// Order Id
        /// </summary>
        [Column("OrderId", Order = 2, TypeName = "BIGINT")]
        [Required]
        public long OrderId { get; set; }

        /// <summary>
        /// Order Detail Id
        /// </summary>
        [Column("OrderDetailId", Order = 3, TypeName = "BIGINT")]
        [Required]
        public long OrderDetailId { get; set; }

        /// <summary>
        /// Product Id
        /// </summary>
        [Column("ProductId", Order = 4, TypeName = "BIGINT")]
        [Required]
        public long ProductId { get; set; }

        /// <summary>
        /// Igredients Id
        /// </summary>
        [Column("IgredientsId", Order = 5, TypeName = "BIGINT")]
        [Required]
        public long IgredientsId { get; set; }

        /// <summary>
        /// Igredients Description
        /// </summary>
        [Column("Description", Order = 6, TypeName = "NVARCHAR(100)")]
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Totla qty per Orderdetail,product and Igredients
        /// </summary>
        [Column("Qty", Order = 7, TypeName = "FLOAT")]
        [Required]
        public float Qty { get; set; }

        /// <summary>
        /// SalesType Id
        /// </summary>
        [Column("SalesTypeId", Order = 8, TypeName = "BIGINT")]
        [Required]
        public long SalesTypeId { get; set; }

        /// <summary>
        /// KDS Id
        /// </summary>
        [Column("KDSId", Order = 9, TypeName = "BIGINT")]
        [Required]
        public long KDSId { get; set; }
    }
}
