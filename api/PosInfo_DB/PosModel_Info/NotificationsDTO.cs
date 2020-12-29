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
    [Table("Notifications")]
    [DisplayName("Notifications")]
    public class NotificationsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Notifications")]
        public long Id { get; set; }

        [Column("Message", Order = 2, TypeName = "NVARCHAR(MAX)")]
        public string Message { get; set; }

        [Column("MessageTS", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> MessageTS { get; set; }

        [Column("UserList", Order = 4, TypeName = "NVARCHAR(MAX)")]
        public string UserList { get; set; }

        [Column("Sender", Order = 5, TypeName = "NVARCHAR(150)")]
        public string Sender { get; set; }

        [Column("PosInfoId", Order = 6, TypeName = "BIGINT")]
        [ForeignKey("FK_Notifications_Notifications")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]//ON UPDATE CASCADE
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> PosInfoId { get; set; }

        [Column("StoreId", Order = 7, TypeName = "BIGINT")]
        [ForeignKey("FK_Notifications_Store")]
        [Association("Store", "StoreId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]//ON UPDATE CASCADE
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> StoreId { get; set; }
    }
}
