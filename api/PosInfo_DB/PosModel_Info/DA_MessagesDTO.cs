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

    [Table("DA_Messages")]
    [DisplayName("DA_Messages")]
    public class DA_MessagesDTO
    {

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_Messages")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(1000)")]
        public String Description { get; set; }

        [Column("MainDAMessagesID", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_Messages_DA_MainMessages")]
        [Association("DA_MainMessages", "MainDAMessagesID", "Id")]  /*Foreign Table, Table Field, Foreign Field*/
        public long MainDAMessagesID { get; set; }

        [Column("IsDeleted", Order = 4, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("Email", Order = 5, TypeName = "NVARCHAR(1000)")]
        public String Email { get; set; }

        [Column("OnOrderCreate", Order = 6, TypeName = "BIT")]
        public Nullable<bool> OnOrderCreate { get; set; }

        [Column("OnOrderUpdate", Order = 7, TypeName = "BIT")]
        public Nullable<bool> OnOrderUpdate { get; set; }

        [Column("OnNewCall", Order = 8, TypeName = "BIT")]
        public Nullable<bool> OnNewCall { get; set; }

    }
}
