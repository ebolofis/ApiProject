using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers
{
    public enum InvoiceTypesEnum
    {
        Receipt = 1,
        Order = 2,
        Void = 3,
        Coplimentary = 4,
        Allowance = 5,
        Timologio = 7,
        VoidOrder = 8,
        PaymentReceipt = 11,
        RefundReceipt = 12,
        Pistosi = 13,
        PreBill = 14
    }

    public enum AccountType
    {
        Cash = 1,
        Coplimentary = 2,
        Charge = 3,
        CreditCard = 4,
        Barcode = 5,
        Voucher = 6,
        Allowence = 9,
        /// <summary>
        /// Type like Credit Card but prepaid....
        /// </summary>
        TicketCompliment = 10
    }

    public enum CreditTransactionType
    {
        RemoveCredit = 0,
        AddCredit = 1,
        ReturnAllCredits = 2,
        ReturnCredit = 3,
        PayLocker = 4,
        ReturnLocker = 5,
        None = 6
    }
}
