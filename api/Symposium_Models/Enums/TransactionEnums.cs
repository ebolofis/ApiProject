using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    public enum TransactionTypesEnum
    {
        OpenCashier = 1,
        CloseCashier = 2,
        Sale = 3,
        Cancel = 4,
        Other = 5,
        Antilogismos = 6,
        CreditCode = 7,
        OpenLocker = 8,
        CloseLocker = 9,
        Tips = 10
    }
}
