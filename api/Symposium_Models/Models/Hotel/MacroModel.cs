using Symposium.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Symposium.Models.Models.Hotel
{
    public class MacroModel : IGuidModel
    {
        /// <summary>
        ///  Table Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Descriptive name of macro
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// συνθήκες για την εφαρμογή ενός δικαιούμενου 
        /// </summary>
        public MacroRuleModel MacroRules { get; set; }


        /// <summary>
        /// προσφορές στον πελάτη με τη χρήση της μακροεντολής
        /// </summary>
        public MacroResultModel MacroResults { get; set; }

        /// <summary>
        /// Ορίζει τη προτεραιώτητα των macros. 
        /// Οταν ικανοποιούνται περισσότερα από ένα macros τότε θα εκτελείται μόνο αυτό με τη μεγαλύτερη προτεραιότητα. Τιμές: 0-1000
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Ημερομηνία ενεργοποίησης macro. 
        /// αν είναι null τότε δεν υπάρχει ημερομηνία ενεργοποίησης
        /// </summary>
        public DateTime? ActiveFrom { get; set; }
        /// <summary>
        /// Ημερομηνία απενεργοποίησης macro.
        ///  αν είναι null τότε δεν υπάρχει ημερομηνία απενεργοποίησης
        /// </summary>
        public DateTime? ActiveTo { get; set; }

        public MacroModel()
        {
            MacroRules = new MacroRuleModel();
            MacroResults = new MacroResultModel();
        }
    }
}