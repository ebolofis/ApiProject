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
    [Table("Delivery_AddressTypes")]
    [DisplayName("Delivery_AddressTypes")]
    public class Delivery_AddressTypesDTO
    {
        [Column("ID", Order = 1, TypeName = "INT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Delivery_AddressTypes")]
        public int ID { get; set; }

        [Column("Descr", Order = 2, TypeName = "NVARCHAR(20)")]
        [Required]
        public string Descr { get; set; }
    }
}
