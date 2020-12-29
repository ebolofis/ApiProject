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
    [Table("TransferToPms_LastIds")]
    [DisplayName("TransferToPms_LastIds")]
    public class TransferToPms_LastIdsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TransferToPms_LastIds")]
        public long Id { get; set; }

        [Column("WebPosId", Order = 2, TypeName = "INT")]
        public int WebPosId { get; set; }

        [Column("WebPosLastId", Order = 2, TypeName = "BIGINT")]
        public long WebPosLastId { get; set; }

    }
}
