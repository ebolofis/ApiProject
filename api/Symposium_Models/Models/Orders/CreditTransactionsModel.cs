using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models {
    public class CreditTransactionsModel
    {
        public Nullable<long> Id { get; set; }

        //sinalagi me kapoion logariasmo
        public long CreditAccountId { get; set; }

        //sinalagi me kapoio vraxiolaki
        public long CreditCodeId { get; set; }
        public decimal Amount { get; set; }

        //Timestamp
        public DateTime CreationTS { get; set; }

        //RemoveCredit = 0 AddCredit = 1, ReturnAllCredits = 2, ReturnCredit = 3
        public int Type { get; set; }
        public long StaffId { get; set; }

        //perigrafi tis sinalagis. gia paradigma "Add amount on Barcode Credit Account"
        public string Description { get; set; }
        public long PosInfoId { get; set; }

        //Id gia tin imera klisimatos. an einai null den exei ginei akoma klisimo imeras
        public long EndOfDayId { get; set; }

        //Id parastatikou
        public long InvoiceId { get; set; }

        //Id gia tin sinalagi tis pliromis
        public long TransactionId { get; set; }

    }
}
