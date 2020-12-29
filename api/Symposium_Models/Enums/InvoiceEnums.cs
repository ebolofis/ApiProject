using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    /// <summary>
    /// Desribe what part of the receipt EXTECR will print.
    /// </summary>
    public enum PrintTypeEnum
    {
        /// <summary>
        /// When a receipt signaled from webapi, Print the whole receipt at once.
        /// </summary>
        PrintWhole = 0,
        /// <summary>
        /// When a receipt signaled from webapi, Print only the last item.
        /// </summary>
        PrintItem = 1,
        /// <summary>
        /// Print the receipt's footer only
        /// </summary>
        PrintEnd = 2,
        /// <summary>
        /// Cancel the current receipt
        /// </summary>
        CancelCurrentReceipt = 3,
        /// <summary>
        /// Print only the last extra of the last item.
        /// </summary>
        PrintExtra = 4,
        /// <summary>
        /// Print the discount of the last item.
        /// </summary>
        PrintItemDiscount = 5
    }

    /// <summary>
    /// Ενέργειες που πρέπει να γίνουν σε μία απόδειξη (νεα απόδειξη, αλλαγή σε απόδειξη κτλ )  
    /// </summary>
    public enum ModifyOrderDetailsEnum
    {
        /// <summary>
        /// νέα απόδειξη
        /// </summary>
        FromScratch = 0,
        /// <summary>
        /// Σε αρχικό ΔΠ  θέλουμε να εκδόσουμε απόδειξη (με εξόφληση ή χωρίς εξόφληση) χωρίς αλλαγή σε αυτό
        /// </summary>
        FromOtherUnmodified = 1,
        /// <summary>
        /// Σε αρχικό ΔΠ θέλουμε να εκδόσουμε απόδειξη (με εξόφληση ή χωρίς εξόφληση) αλλά με αλλαγή σε αυτό (πχ εισαγωγή έκπτωση)
        /// </summary>
        FromOtherUpated = 2,
        /// <summary>
        /// Σε αρχική απόδειξη χωρίς εξόφληση θέλουμε να την εξοφλήσουμε
        /// </summary>
        PayOffOnly = 3,
        /// <summary>
        /// Αλλαγή τρόπου πληρωμής
        /// </summary>
        ChangePaymentType = 4,

        /// <summary>
        /// Update Order From Delivary Agent
        /// </summary>
        Update = 5
    }

    public enum IsPaidEnum
    {
        /// <summary>
        /// για κανένα από τα αντικείμενα δεν έχει δημιουργηθεί transaction (δεν έχει εξοφληθεί κανένα αντικείμενο),   
        /// </summary>
        None = 0,
        /// <summary>
        /// για μερικά αντικείμενα δεν έχει δημιουργηθεί transaction (μερικά αντικείμενα έχουν εξοφληθεί), 
        /// </summary>
        Some = 1,
        /// <summary>
        /// έχουν εξοφληθεί όλα τα αντικείμενα. 
        /// </summary>
        All = 2,
    }

}
