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
    [Table("DA_CustomerMessages")]
    [DisplayName("DA_CustomerMessages")]
    public class DA_CustomerMessagesDTO
    {


        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_DA_CustomerMessages")]
        public long Id { get; set; }

        [Column("CustomerId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_CustomerMessages_DA_Customers")] 
        [Association("DA_Customers", "CustomerId", "Id")] /*Foreign Table, Table Field, Foreign Field*/
        public long CustomerId { get; set; }

        [Column("MessageId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_CustomerMessages_DA_Messages")] 
        [Association("DA_Messages", "MessageId", "Id")] /*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> MessageId { get; set; }


        [Column("MessageDetailsId", Order = 4, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_CustomerMessages_DA_MessagesDetails")] 
        [Association("DA_MessagesDetails", "MessageDetailsId", "Id")] /*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> MessageDetailsId { get; set; }


        [Column("MainDAMessageId", Order = 5, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_CustomerMessages_DA_MainMessages")]
        [Association("DA_MainMessages", "MainDAMessageId", "Id")] /*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> MainDAMessageId { get; set; }


        [Column("CreationDate", Order = 6, TypeName = "DATETIME")]
        public DateTime CreationDate { get; set; }


        [Column("StaffId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }


        [Column("MessageText", Order = 8, TypeName = "NVARCHAR(1000)")]
        public String MessageText { get; set; }

        [Column("OrderId", Order = 9, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_CustomerMessages_DA_Orders")]
        [Association("DA_Orders", "OrderId", "Id")] /*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> OrderId { get; set; }

        [Column("StoreId", Order = 10, TypeName = "BIGINT")]
        [ForeignKey("FK_DA_CustomerMessages_DA_Stores")]
        [Association("DA_Stores", "StoreId", "Id")] /*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> StoreId { get; set; }

        [Column("IsTemporary", Order = 11, TypeName = "BIT")]
        public bool IsTemporary { get; set; } = false;

    }
}
