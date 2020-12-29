using Symposium.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Symposium.Models.Models.Hotel
{
    public class MacroTimezoneModel : IGuidModel
    {
        /// <summary>
        ///  Table Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Hotel Id
        /// </summary>
        public long HotelId { get; set; }

        /// <summary>
        /// Μοναδικός Κωδικός (ένας λατινικός κεφαλαίος χαρακτήρας, πχ B=Breakfast, L, D )
        /// </summary>
        public string Code { get; set; }


        /// <summary>
        ///  Περιγραφή (πχ πρωινό, γεύμα, δείπνο)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ώρα από
        /// </summary>
        public DateTime TimeFrom { get; set; }

        /// <summary>
        /// Ώρα έως
        /// </summary>
        public DateTime TimeTo { get; set; }
    }

    public class TimezoneExpressionModel
    {
        public string timezoneExpression { get; set; }
    }
}