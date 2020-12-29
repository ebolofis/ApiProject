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
    [Table("DA_MessagesDetails")]
    [DisplayName("DA_MessagesDetails")]
    public class DA_MessagesDetailsDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_MessagesDetails")]
        public long Id { get; set; }

        [Column("HeaderId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_MesaggesDetails_DA_Messages")]
        [Association("DA_Messages", "HeaderId", "Id")]  /*Foreign Table, Table Field, Foreign Field*/
        public long HeaderId { get; set; }

        [Column("Description", Order = 3, TypeName = "NVARCHAR(1000)")]
        public String Description { get; set; }

        [Column("ToOrder", Order = 4, TypeName = "BIT")]
        public Nullable<bool> ToOrder { get; set; }

        [Column("IsDeleted", Order = 5, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("Email", Order = 6, TypeName = "NVARCHAR(1000)")]
        public String Email { get; set; }

        [Column("OnOrderCreate", Order = 7, TypeName = "BIT")]
        public Nullable<bool> OnOrderCreate { get; set; }

        [Column("OnOrderUpdate", Order = 8, TypeName = "BIT")]
        public Nullable<bool> OnOrderUpdate { get; set; }

        [Column("OnNewCall", Order = 9, TypeName = "BIT")]
        public Nullable<bool> OnNewCall { get; set; }
    }
}
