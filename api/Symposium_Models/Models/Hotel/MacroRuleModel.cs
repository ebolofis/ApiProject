using Symposium.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Symposium.Models.Models.Hotel
{
    public class MacroRuleModel : IGuidModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Hotel Id
        /// </summary>
        public List<long> HotelId { get; set; }

        /// <summary>
        /// TravelAgent
        /// </summary>
        public idNameModel TravelAgent { get; set; }

        /// <summary>
        /// Group name
        /// </summary>
        public idNameModel Group { get; set; }

        /// <summary>
        /// Reservation Source (ex: tel, internet,...)
        /// </summary>
        public string ReservationSource { get; set; }

        /// <summary>
        /// Board Code
        /// </summary>
        public List<string> Board { get; set; }

        /// <summary>
        /// Room Type
        /// </summary>
        public List<int> RoomType { get; set; }

        /// <summary>
        /// Booked Room Type (orgktnr της buch)
        /// </summary>
        public List<int> BookedRoomType { get; set; }


        /// <summary>
        /// Room No (μόνο σε περίπτωση overwritten macros)
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// Reservation User Defined Field----
        /// </summary>
        public string ReservationUserDefinedField { get; set; }

        /// <summary>
        /// Protel PriceList
        /// </summary>
        public long HotelPriceList { get; set; }

        /// <summary>
        /// Protel NationalityCode
        /// </summary>
        public string NationalityCode { get; set; }

        /// <summary>
        /// VIP description----
        /// </summary>
        public string VIP { get; set; }

        /// <summary>
        /// Membership
        /// </summary>
        public int Membership { get; set; }

        /// <summary>
        /// SubMembership -----
        /// </summary>
        public int SubMembership { get; set; }

        /// <summary>
        /// Profile User Defined Field ----
        /// </summary>
        public string ProfileUserDefinedField { get; set; }

        /// <summary>
        /// Λίστα με τα Pos Department Ids
        /// </summary>
        public List<long> PosDepartment { get; set; }

        /// <summary>
        /// Περιγράφει σε ποια time zones θα εφαρμοστεί το macro καθώς και τη σχέση μεταξύ των time zones. Πχ: ‘B’ ή ‘B+L’ ή ‘B+(L.D)’ όπου B,L,D τα codes των timezones
        /// </summary>
        public string TimeZones { get; set; }

        /// <summary>
        /// true όταν ο πελάτης κάνει χρήση του δικαιούμενου.
        /// </summary>
        public bool UseAllowance { get; set; }

        /// <summary>
        /// reservation id
        /// </summary>
        public int? ReservationId { get; set; }

        /// <summary>
        /// περιλαμβανει επτά θέσεις, μια για καθε ημέρα της εβδομάδας, για να υποδεικνύει πότε είναι ενεργό το macro
        /// </summary>
        public bool[] ActiveDays { get; set; }

        /// <summary>
        /// Συνολικός αριθμός διαθέσιμης χρήσης του macro
        /// </summary>
        public int TotalConsumption { get; set; } = 0;

        /// <summary>
        /// Υπολοιπόμενος αριθμός διαθέσιμης χρήσης του macro
        /// </summary>
        public int RemainingConsumption { get; set; } = 0;

        public MacroRuleModel()
        {
            HotelId = new List<long>();
            PosDepartment = new List<long>();
           // ActiveDays = new bool[7] { true, true, true, true, true, true, true };
        }

    }

    public class idNameModel
    {
        public long kdnr { get; set; }
        public string name1 { get; set; }
    }
}