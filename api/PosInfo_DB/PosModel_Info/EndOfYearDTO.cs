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
    [Table("EndOfYear")]
    [DisplayName("EndOfYear")]
    public class EndOfYearDTO:ITables
    {

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_EndOfYear")]
        public long Id { get; set; }

        [Column("ClosedYear", Order = 2, TypeName = "INT")]
        public Nullable<int> ClosedYear { get; set; }


        [Column("ClosedDate", Order = 3, TypeName = "DATETIME")]
        public Nullable<DateTime> ClosedDate { get; set; }


        [Column("Description", Order = 4, TypeName = "VARCHAR(255)")]
        public string Description { get; set; }


    }
}
