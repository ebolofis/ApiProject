using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    /// <summary>
    /// Types of accounts
    /// </summary>
    public enum AccountTypeEnum
    {
        /// <summary>
        /// Cash payment
        /// </summary>
        Cash = 1,
        /// <summary>
        /// Complimentary payment (φιλοξενία)
        /// </summary>
        Coplimentary = 2,
        /// <summary>
        /// Room charge payment
        /// </summary>
        Charge = 3,
        /// <summary>
        /// Credit card payment
        /// </summary>
        CreditCard = 4,
        /// <summary>
        /// Barcode payment
        /// </summary>
        Barcode = 5,
        /// <summary>
        /// Coupon payment (not for goodys)
        /// </summary>
        Voucher = 6,
        /// <summary>
        /// Allowance payment (παραχώρηση)
        /// </summary>
        Allowence = 9,

        /// <summary>
        /// Type like Credit Card but prepaid....
        /// </summary>
        TicketCompliment = 10
    }
}
