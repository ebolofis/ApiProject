using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class GuestModel
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> arrivalDT { get; set; }
        public Nullable<System.DateTime> departureDT { get; set; }
        public Nullable<System.DateTime> birthdayDT { get; set; }
        public string Room { get; set; }
        public Nullable<int> RoomId { get; set; }
        public string Arrival { get; set; }
        public string Departure { get; set; }
        public string ReservationCode { get; set; }
        public Nullable<int> ProfileNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Member { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string VIP { get; set; }
        public string Benefits { get; set; }
        public string NationalityCode { get; set; }
        public string ConfirmationCode { get; set; }
        public Nullable<int> Type { get; set; }
        public string Title { get; set; }
        public Nullable<int> Adults { get; set; }
        public Nullable<int> Children { get; set; }
        public string BoardCode { get; set; }
        public string BoardName { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public Nullable<int> ReservationId { get; set; }
        public Nullable<bool> IsSharer { get; set; }
        public Nullable<long> HotelId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public string ClassName { get; set; }
        public Nullable<int> AvailablePoints { get; set; }
        public Nullable<int> fnbdiscount { get; set; }
        public Nullable<int> ratebuy { get; set; }

        //public Nullable<int> NoPos { get; set; }
        //public Int64? AllowdPriceList { get; set; }
        //public Decimal? AllowdDiscount { get; set; }
        //public Decimal? AllowdDiscountChild { get; set; }
        //public Int32? AllowedAdultMeals { get; set; }
        //public Int32? AllowedChildMeals { get; set; }
        //public Int32? ConsumedMeals { get; set; }
        //public Int32? TotalRecs { get; set; }
    }

    public class Guest : GuestModel
    { }


}
