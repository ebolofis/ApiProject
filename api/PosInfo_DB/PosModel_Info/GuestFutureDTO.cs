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
    [Table("GuestFuture")]
    [DisplayName("GuestFuture")]
    public class GuestFutureDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_GuestFuture")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Description { get; set; }

    }
}
