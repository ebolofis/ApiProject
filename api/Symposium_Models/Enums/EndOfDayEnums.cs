using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    /// <summary>
    /// Types for End Of Day preview
    /// </summary>
    public enum EndOfDayReceiptTypes
    {
        /// <summary>
        /// All invoices (not orders) that have no transaction yet
        /// </summary>
        NotPaid = -99,
        /// <summary>
        ///  All invoices (orders, not paid invoices, not printed orders/invoices are excluded)
        /// </summary>
        ReceiptTotal = -100,
        /// <summary>
        /// All orders that are not invoiced yet
        /// </summary>
        NotInvoiced = -101,
        /// <summary>
        /// All invoices that are canceled
        /// </summary>
        Canceled = -102,
        /// <summary>
        /// All invoices that are not printed
        /// </summary>
        NotPrinted = -103,
        /// <summary>
        /// Discount in invoices table
        /// </summary>
        DiscountTotal = -104,
        /// <summary>
        /// Couvers in order table
        /// </summary>
        Couvers = -105,
        /// <summary>
        /// Amount of cash a waiter holds ( (transaction type 1 && (transaction type 3 && account type 1) )
        /// </summary>
        OpenCashier = -106,
        /// <summary>
        /// Amount of money a waiter has returned ( transaction type 2 )
        /// </summary>
        CloseCashier = -107,
        /// <summary>
        /// Amount of money a waiter has to return grouped by account
        /// </summary>
        AccountReturn = -108,
        /// <summary>
        /// Total amount of money a waiter has to return
        /// </summary>
        TotalToReturn = -109,
        /// Loyalty Discount in invoices table
        /// </summary>
        LoyaltyDiscountTotal = -110,
        /// <summary>
        /// represents a list of all payment/account types that have transactions
        /// </summary>
        Default = 0

    }

    /// <summary>
    /// Types of end of day actions
    /// </summary>
    public enum EndOfDayActions
    {
        /// <summary>
        /// Prints Z report for current day
        /// </summary>
        PrintZ = 0,
        /// <summary>
        /// Reprints a specific Z report
        /// </summary>
        ReprintZ = 1,
        /// <summary>
        /// Prints X report for current day
        /// </summary>
        PrintX = 2
    }

}
