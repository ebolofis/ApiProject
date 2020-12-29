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
    [DisplayName("NFCcard")]
    [Table("NFCcard")]
    public class NFCcardDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_NFCcard")]
        public long Id { get; set; }

        [Column("Type", Order = 2, TypeName = "SMALLINT")]
        public short Type { get; set; }

        [Column("RoomSector", Order = 3, TypeName = "INT")]
        public int RoomSector { get; set; }

        [Column("FirstDateSector", Order = 4, TypeName = "INT")]
        public int FirstDateSector { get; set; }

        [Column("SecondDateSector", Order = 5, TypeName = "INT")]
        public int SecondDateSector { get; set; }

        [Column("ValidateDate", Order = 6, TypeName = "BIT")]
        public Boolean ValidateDate { get; set; }

    }
}
