using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.MealBoards
{
    public class CustomerModel
    {
        public enum ReservStatusEnum { Reservation = 0, CheckedIn = 1, CheckedOut = 2 }


        public int TotalRecs { get; set; }
        public string ReservationCode { get; set; }
        public int ReservationId { get; set; }
        public int OriginalReservationId { get; set; }
        public int RoomId { get; set; }
        public string Room { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int ProfileNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Member { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DateTime BirthdayDate { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string VIP { get; set; }
        public string Benefits { get; set; }
        public int NationalityId { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        /// <summary>
        /// kdnr
        /// </summary>
        public string CustomerId { get; set; }
        public bool IsSharer { get; set; }
        public string BoardCode { get; set; }
        public string BoardName { get; set; }
        public int NoPos { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int AvailablePoints { get; set; }
        public int FnbDiscount { get; set; }
        public int Ratebuy { get; set; }
        public int TravelAgentId { get; set; }
        public string TravelAgent { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string GuestFuture { get; set; }

        public int BookedRoomTypeId { get; set; }
        public string BookedRoomType { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomType { get; set; }
        public int SourceId { get; set; }
        public string Source { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int Mpehotel { get; set; }
        public ReservStatusEnum ReservStatus {get;set;}
        public string NationalityCode { get; set; }
        public int LoyaltyProgramId { get; set; }
        public int LoyaltyLevelId { get; set; }
        public int RateCodeId { get; set; }
        public string RateCodeDescr { get; set; }
        public string CardType { get; set; }
    }
}
