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
    [DisplayName("DA_CustomersTokens")]
    [Table("DA_CustomersTokens")]
    public class DA_CustomersTokensDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_CustomersTokens")]
        public long Id { get; set; }

        [Column("CustomerId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_CustomersTokens_DA_Customers")]
        [Association("DA_Customers", "CustomerId", "Id")]
        public long CustomerId { get; set; }

        [Column("Token", Order = 3, TypeName = "NVARCHAR(MAX)")]
        public string Token { get; set; }
    }
}
