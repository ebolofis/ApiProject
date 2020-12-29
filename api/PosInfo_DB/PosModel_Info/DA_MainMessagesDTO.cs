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

    [Table("DA_MainMessages")]
    [DisplayName("DA_MainMessages")]
    public class DA_MainMessagesDTO
    {

        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_MainMessages")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(1000)")]
        public String Description { get; set; }

        [Column("IsDeleted", Order = 3, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("Email", Order = 4, TypeName = "NVARCHAR(1000)")]
        public String Email { get; set; }

        [Column("OnOrderCreate", Order = 5, TypeName = "BIT")]
        public Nullable<bool> OnOrderCreate { get; set; }

        [Column("OnOrderUpdate", Order = 6, TypeName = "BIT")]
        public Nullable<bool> OnOrderUpdate { get; set; }

        [Column("OnNewCall", Order = 7, TypeName = "BIT")]
        public Nullable<bool> OnNewCall { get; set; }

    }
}
