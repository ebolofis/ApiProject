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
    [Table("HotelCustomerDataConfig")]
    [DisplayName("HotelCustomerDataConfig")]
    public class HotelCustomerDataConfigDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_HotelCustomerDataConfig")]
        public long Id { get; set; }

        [Column("Property", Order = 2, TypeName = "NVARCHAR(100)")]
        public string Property { get; set; }

        [Column("FieldType", Order = 3, TypeName = "NVARCHAR(100)")]
        public string FieldType { get; set; }

        [Column("Description", Order = 4, TypeName = "NVARCHAR(100)")]
        public string Description { get; set; }

        [Column("Priority", Order = 5, TypeName = "INT")]
        public int Priority { get; set; }
    }
}
