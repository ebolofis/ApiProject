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
    [Table("TransactionTypes")]
    [DisplayName("TransactionTypes")]
    public class TransactionTypesDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_TransactionTypes")]
        public long Id { get; set; }

        [Column("Code", Order = 2, TypeName = "SMALLINT")]
        public Nullable<short> Code { get; set; }

        [Column("Description", Order = 3, TypeName = "NVARCHAR(MAX)")]
        public string Description { get; set; }

        [Column("IsIncome", Order = 4, TypeName = "BIT")]
        public Nullable<bool> IsIncome { get; set; }

        [Column("IsDeleted", Order = 5, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }
    }
}
