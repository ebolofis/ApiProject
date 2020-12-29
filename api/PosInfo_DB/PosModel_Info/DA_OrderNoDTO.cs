using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [Table("DA_OrderNo")]
    [DisplayName("DA_OrderNo")]
    public class DA_OrderNoDTO
    {
        [Column("OrderNo", Order = 1, TypeName = "BIGINT")]
        public long OrderNo { get; set; }
    }
}
