using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class GetCustomersPreviewModel
    {
        public List<CustomersDetails> Data { get; set; }

        public long TotalPages { get; set; }

        public long PageSize { get; set; }
    }
    
    public class CustomersDetails
        {
            public long Id { get; set; }
            public Nullable<System.DateTime> arrivalDT { get; set; }
            public string ConfirmationCode { get; set; }
            public Nullable<System.DateTime> departureDT { get; set; }
            public string Password { get; set; }
            public Nullable<int> ProfileNo { get; set; }
            public string ReservationCode { get; set; }
            public Nullable<int> ReservationId { get; set; }
            public string Room { get; set; }
            public Nullable<int> RoomId { get; set; }
            public string Address { get; set; }
            public Nullable<int> Adults { get; set; }
            public string Benefits { get; set; }
            public Nullable<System.DateTime> birthdayDT { get; set; }
            public string BoardCode { get; set; }
            public string BoardName { get; set; }
            public Nullable<int> Children { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public Nullable<bool> IsSharer { get; set; }
            public string LastName { get; set; }
            public string Member { get; set; }
            public string NationalityCode { get; set; }
            public string Note1 { get; set; }
            public string Note2 { get; set; }
            public int NoPos { get; set; }
            public string PostalCode { get; set; }
            public string Telephone { get; set; }
            public string Title { get; set; }
            public Nullable<int> Type { get; set; }
            public string VIP { get; set; }
            public int AllowdPriceList { get; set; }
            public int AllowdDiscount { get; set; }
            public int AllowdDiscountChild { get; set; }
            public int AllowedAdultMeals { get; set; }
            public int AllowedChildMeals { get; set; }
            public int ConsumedMeals { get; set; }
            public Nullable<int> ClassId { get; set; }
            public string ClassName { get; set; }
            public Nullable<int> AvailablePoints { get; set; }
            public Nullable<int> fnbdiscount { get; set; }
            public Nullable<int> ratebuy { get; set; }
        }
    
}
