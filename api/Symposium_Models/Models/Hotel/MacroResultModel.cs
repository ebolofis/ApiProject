using Symposium.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Symposium.Models.Models.Hotel
{
    public class MacroResultModel : IGuidModel
    {
        /// <summary>
        ///  Id
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// flag ενεργοποίησης έκπτωσης σε είδος
        /// </summary>
        public bool ItemDiscountFlag { get; set; }

        /// <summary>
        /// λιστα με τα είδη σε έκπτωση
        /// </summary>
        public List<long> DiscountItems { get; set; }

        /// <summary>
        /// λιστα με τα κατηγορίες προϊόντων
        /// </summary>
        public List<long> ProductCategories { get; set; }

        /// <summary>
        /// Ποσό έκτπωσης
        /// </summary>
        public decimal ItemDiscount { get; set; }

        /// <summary>
        /// Ποσοστό έκπτωσης
        /// </summary>
        public decimal ItemDiscountPercentage { get; set; }

        /// <summary>
        /// flag ενεργοποίησης έκπτωσης σε ομάδα ειδών
        /// </summary>
        public bool GroupDiscountFlag { get; set; }

        /// <summary>
        /// λιστα με τις ομάδες ειδών σε έκπτωση
        /// </summary>
        public List<long> DiscountGroups { get; set; }

        /// <summary>
        /// Ποσό έκτπωσης
        /// </summary>
        public decimal GroupDiscount { get; set; }

        /// <summary>
        /// Ποσοστό έκπτωσης
        /// </summary>
        public decimal GroupDiscountPercentage { get; set; }

        /// <summary>
        /// flag ενεργοποίησης έκπτωσης στην απόδειξη
        /// </summary>
        public bool OrderDiscountFlag { get; set; }

        /// <summary>
        /// Μέγιστο Ποσό έκτπωσης
        /// </summary>
        public decimal OrderDiscount { get; set; }

        /// <summary>
        /// Ποσοστό έκπτωσης
        /// </summary>
        public decimal OrderDiscountPercentage { get; set; }

        /// <summary>
        /// flag ενεργοποίησης για ορισμό τιμοκαταλόγου
        /// </summary>
        public bool PriceListFlag { get; set; }

        /// <summary>
        /// τιμοκαταλόγος
        /// </summary>
        public int PriceList { get; set; }

        /// <summary>
        /// flag ενεργοποίησης έκπτωσης σε περίπτωση χρήσης δικαιούμενου.
        /// Ενεργό μονο οταν AllowanceMacroRule.UseAllowance=true
        /// </summary>
        public bool AllowanceDiscountFlag { get; set; }

        /// <summary>
        /// Ποσό έκτπωσης ενός ενηλικα
        /// </summary>
        public decimal AllowanceAdultDiscount { get; set; }

        /// <summary>
        /// Ποσοστό έκπτωσης ενός ενηλικα
        /// </summary>
        public decimal AllowanceAdultDiscountPercentage { get; set; }

        /// <summary>
        /// Ποσό έκτπωσης ενός παιδιού
        /// </summary>
        public decimal AllowanceChildDiscount { get; set; }
        
        /// <summary>
        /// Ποσοστό έκπτωσης ενός παιδιού
        /// </summary>
        public decimal AllowanceChildDiscountPercentage { get; set; }

        /// <summary>
        /// όταν υπάρχει τιμή τότε το είδος θα εισαχθεί υποχρεωτικά στην παραγγελία όταν γίνεται χρήση δικαιούμενου.
        /// </summary>
        public List<long> DefaultItem { get; set; }

        /// <summary>
        /// σελιδα ανακατεύθυνσης της ροής μετά τη χρήση του macro
        /// </summary>
        public long PosPageRedirection { get; set; }

        public MacroResultModel()
        {
            DiscountItems = new List<long>();
            DiscountGroups = new List<long>();
        }

    }
}