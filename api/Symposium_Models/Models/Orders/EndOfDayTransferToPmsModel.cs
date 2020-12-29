using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class EndOfDayTransferToPmsModel
    {
        public long Id { get; set; }

        //arithmos kratisis
        public long RegNo { get; set; }

        //Id pms department
        public int PmsDepartmentId { get; set; }
        public string Description { get; set; }

        //Id pelati
        public Nullable<long> ProfileId { get; set; }

        //onoma pelati
        public string ProfileName { get; set; }

        //eggrafi transaction. null gia ton athroisma metriton
        public Nullable<long> TransactionId { get; set; }
        public int TransferType { get; set; }
        public string RoomId { get; set; }

        //arithmos domatiou
        public string RoomDescription { get; set; }

        //arithmos parastatikou
        public Nullable<long> ReceiptNo { get; set; }

        //o tipos tou parastatikou apo POS
        public Nullable<long> PosInfoDetailId { get; set; }

        //gia enimerosi tou pms: 0 = den prepi na stalei sto pms (ektos an to status=2). 1 = prepei na stalei sto pms
        public int SendToPms { get; set; }
        public decimal Total { get; set; }

        //Timestamp
        public DateTime SendToPmsTS { get; set; }
        public string ErrorMessage { get; set; }

        //id pou pistrefei to pms. an den exei stalei sto pms exei timi null
        public Nullable<long> PmsResponseId { get; set; }

        //hash gia apofigi diploeggrafon
        public Nullable<System.Guid> TransferIdentifier { get; set; }

        //perigrafi pms department
        public string PmsDepartmentDescription { get; set; }

        //katastasi apostolis sto pms: null = arxiki timi an den prepi na stalei sto pms, 0 = arxiki timi an prepi na stalei sto pms, 1 = exei stalei sto pms, 2 = ginete apostoli sto pms
        public Nullable<int> Status { get; set; }
        public long PosInfoId { get; set; }
        public long EndOfDayId { get; set; }
        public long HotelId { get; set; }

        //1 = deleted
        public Nullable<bool> IsDeleted { get; set; }
    }
}
