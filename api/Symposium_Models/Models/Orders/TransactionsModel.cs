using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models {
    public class TransactionsModel
    {
        public long Id { get; set; }
        public DateTime Day { get; set; }
        public long PosInfoId { get; set; }
        public long StaffId { get; set; }
        public Nullable<long> OrderId { get; set; }

        // 1 = anigma tamiou (OpenCashier), 2 = klisimo tamiou (CloseCashier), 3 = ejoflisi (sale), 4 = akirosi, 5 = alo
        public int TransactionType { get; set; }
        public decimal Amount { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public string Description { get; set; }
        public int AccountId { get; set; }

        //1 = out, gia ejerxomena posa 0 = in, gia iserxomena posa
        public int InOut { get; set; }

        //Gross = Net + Vat + Tax
        public decimal Gross { get; set; }
        public decimal Net { get; set; }
        public decimal Vat { get; set; }
        
        //foros ektos ΦΠΑ
        public decimal Tax { get; set; }
        public Nullable<long> EndOfDayId { get; set; }
        public string ExtDescription { get; set; }
        public Nullable<long> InvoicesId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }    

    }

    /// <summary>
    /// Transaction Model With Invoice Guest Transacion List and 
    /// CreditTransaction List and TransferToPms Record
    /// </summary>
    public class TransactionsExtraModel : TransactionsModel
    {
        public List<Invoice_Guests_TransModel> InvoiceGuest { get; set; }

        public List<CreditTransactionsModel> CreditTransaction { get; set; }

        public List<TransferToPmsModel> TransferToPms { get; set; }
    }

}
