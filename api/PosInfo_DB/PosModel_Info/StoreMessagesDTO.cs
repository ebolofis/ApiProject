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
    [Table("StoreMessages")]
    [DisplayName("StoreMessages")]
    public class StoreMessagesDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_StoreMessages")]
        public long Id { get; set; }

        [Column("Message", Order = 2, TypeName = "NVARCHAR(MAX)")]
        public string Message { get; set; }

        [Column("CreationDate", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> CreationDate { get; set; }

        [Column("Title", Order = 4, TypeName = "NVARCHAR(500)")]
        public string Title { get; set; }

        [Column("StoreId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_StoreMessages_Store")]
        [Association("Store", "StoreId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> StoreId { get; set; }

        [Column("ShowFrom", Order = 6, TypeName = "DATETIME")]
        public Nullable<System.DateTime> ShowFrom { get; set; }

        [Column("ShowTo", Order = 7, TypeName = "DATETIME")]
        public Nullable<System.DateTime> ShowTo { get; set; }

        [Column("ImageUri", Order = 8, TypeName = "NVARCHAR(250)")]
        public string ImageUri { get; set; }

        [Column("Status", Order = 9, TypeName = "TINYINT")]
        public Nullable<byte> Status { get; set; }
    }
}
