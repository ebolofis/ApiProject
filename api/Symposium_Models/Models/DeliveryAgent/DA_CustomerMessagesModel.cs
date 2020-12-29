using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_CustomerMessagesModel
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public Nullable<long> MainDAMessageID { get; set; }
        public Nullable<long> MessageId { get; set; }
        public Nullable<long> MessageDetailsId { get; set; }
        public DateTime CreationDate { get; set; }
        public Nullable<long> StaffId { get; set; }
        public String MessageText {get;set; }
        public Nullable<long> OrderId { get; set; }
        public Nullable<long> StoreId { get; set; }
        public bool IsTemporary { get; set; } = false;
    }

    public class DA_CustomerMessagesModelExt : DA_CustomerMessagesModel
    {
        public string MainMessageDesc { get; set; }
        public string MessageDesc { get; set; }
        public string MessageDetailsDesc { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhone1 { get; set; }
        public string CustomerPhone2 { get; set; }
        public string CustomerMobile { get; set; }
        public string OrderNo { get; set; }
        public string StoreOrderNo { get; set; }
        public string StaffName { get; set; }
        public string StaffLastname { get; set; }
    }

    public class DA_CustomerMessagesCustomerNote : DA_CustomerMessagesModel
    {
        public string LastOrderNote { get; set; }
    }

}
