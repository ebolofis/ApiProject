using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class EndOfDayModel
    {
        public long Id { get; set; }

        //enarksi imeras
        public Nullable<System.DateTime> FODay { get; set; }

        //to POS me to opoio egine klisimo imeras
        public long PosInfoId { get; set; }
        public Nullable<long> CloseId { get; set; }

        ////meikta kerdh
        public decimal Gross { get; set; }

        ////kathara kerdh
        public decimal Net { get; set; }

        //plithos parastatikos
        public long TicketsCount { get; set; }

        //athroisma ton order items
        public long ItemCount { get; set; }

        //mesos oros mias podijis
        public decimal TicketAverage { get; set; }

        //servitoros pou ekane to klisimo tis imeras
        public long StaffId { get; set; }

        //sinoliko poso ekptosis olon ton apodijeon
        public decimal Discount { get; set; }

        //akrivis imera ora klisimatos
        public DateTime dtDateTime { get; set; }

        //sinoliko poso kiniseon me barcodes
        public Nullable<decimal> Barcodes { get; set; }

        //PMS Date sto kleisimo imeras
        public Nullable<System.DateTime> eodPMS { get; set; }
    }
}
