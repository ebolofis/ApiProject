using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models {
    public class TransferToPmsModel
    {
        public Nullable<long> Id { get; set; }
        public string RegNo { get; set; }
        public string PmsDepartmentId { get; set; }
        public string Description { get; set; }
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public Nullable<long> TransactionId { get; set; }
        public Nullable<int> TransferType { get; set; }
        public string RoomId { get; set; }
        public string RoomDescription { get; set; }
        public string ReceiptNo { get; set; }
        public Nullable<long> PosInfoDetailId { get; set; }
        public int Points { get; set; }


        //0 = den prepi na stalei sto pms (ektos an to status=2). 1 = prepi na stalei sto pms
        public bool SendToPMS { get; set; }
        public decimal Total { get; set; }
        public DateTime SendToPmsTS { get; set; }
        public string ErrorMessage { get; set; }

        //id pou epistrefei to pms. an den exei stalei sto pms exei timi null
        public string PmsResponseId { get; set; }
        public string TransferIdentifier { get; set; }
        public string PmsDepartmentDescription { get; set; }

        //null = arxiki timi an den prepi na stalei sto pms, 0 = arxiki timi an prepi na stalei sto pms, 1 = exei stalei sto pms, 2 = ginete apostoli sto pms
        public Nullable<int> Status { get; set; }
        public Nullable<long> PosInfoId { get; set; }
        public Nullable<long> EndOfDayId { get; set; }
        public long HotelId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<long> PMSPaymentId { get; set; }

        public Nullable<long> PMSInvoiceId { get; set; }

        public Nullable<long> InvoiceId { get; set; }

        public byte[] HtmlReceipt { get; set; }
    }

    public class PmsRoomModel
    {
        public Nullable<long> PmsRoom { get; set; }

        public Nullable<bool> SendsTransfer { get; set; }
    }
}
