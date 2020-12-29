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
    [Table("HotelCustomMessages")]
    [DisplayName("HotelCustomMessages")]
    public class HotelCustomMessagesDTO : INoSql
    {
        [Column("Id", Order = 1, TypeName = "uniqueidentifier")]
        [Key]
        //[Editable(true)]
        [DisplayName("PK_HotelCustomMessages")]
        public Guid Id { get; set; }

        [Column("Model", Order = 2, TypeName = "TEXT")]
        public string Model { get; set; }
    }
}
