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
    [DisplayName("Accounts")]
    [Table("Accounts")]
    public class AccountDTO:ITables,IIsDeleted
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Accounts")]
        public long Id { get; set; }

        [Column("Description", Order = 2, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("Type", Order = 3, TypeName = "SMALLINT")]
        public Nullable<short> Type { get; set; }

        [Column("IsDefault", Order = 4, TypeName = "BIT")]
        [DisplayFormatAttribute(DataFormatString = "DF_Accounts_IsDefault", NullDisplayText = "0")]//DefaultValue (Name, Value)
        public Nullable<bool> IsDefault { get; set; }

        [Column("SendsTransfer", Order = 5, TypeName = "BIT")]
        public Nullable<bool> SendsTransfer { get; set; }

        [Column("CardOnly", Order = 6, TypeName = "BIT")]
        [DisplayFormatAttribute(DataFormatString = "DF_Accounts_CardOnly", NullDisplayText = "0")]//DefaultValue (Name, Value)
        public Nullable<bool> CardOnly { get; set; }

        [Column("IsDeleted", Order = 7, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("KeyboardType", Order = 8, TypeName = "SMALLINT")]
        [DisplayFormatAttribute(DataFormatString = "DF_Accounts_KeyboartType", NullDisplayText = "1")]//DefaultValue (Name, Value)
        public Nullable<short> KeyboardType { get; set; }

        [Column("PMSPaymentId", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> PMSPaymentId { get; set; }

        [Column("DAId", Order = 10, TypeName = "BIGINT")]
        public Nullable<long> DAId { get; set; }

        [Column("Sort", Order = 11, TypeName = "INT")]
        public Nullable<int> Sort { get; set; }
    }
}
