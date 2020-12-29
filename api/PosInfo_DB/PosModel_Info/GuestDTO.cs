using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [Table("Guest")]
    [DisplayName("Guest")]
    public class GuestDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Guest")]
        public long Id { get; set; }

        [Column("arrivalDT", Order = 2, TypeName = "DATETIME")]
        public Nullable<System.DateTime> arrivalDT { get; set; }

        [Column("departureDT", Order = 3, TypeName = "DATETIME")]
        public Nullable<System.DateTime> departureDT { get; set; }

        [Column("birthdayDT", Order = 4, TypeName = "DATETIME")]
        public Nullable<System.DateTime> birthdayDT { get; set; }

        [Column("Room", Order = 5, TypeName = "NVARCHAR(150)")]
        public string Room { get; set; }

        [Column("RoomId", Order = 6, TypeName = "INT")]
        public Nullable<int> RoomId { get; set; }

        [Column("Arrival", Order = 7, TypeName = "NVARCHAR(50)")]
        public string Arrival { get; set; }

        [Column("Departure", Order = 8, TypeName = "NVARCHAR(50)")]
        public string Departure { get; set; }

        [Column("ReservationCode", Order = 9, TypeName = "NVARCHAR(150)")]
        public string ReservationCode { get; set; }

        [Column("ProfileNo", Order = 10, TypeName = "INT")]
        public Nullable<int> ProfileNo { get; set; }

        [Column("FirstName", Order = 11, TypeName = "NVARCHAR(150)")]
        public string FirstName { get; set; }

        [Column("LastName", Order = 12, TypeName = "NVARCHAR(150)")]
        public string LastName { get; set; }

        [Column("Member", Order = 13, TypeName = "NVARCHAR(150)")]
        public string Member { get; set; }

        [Column("Password", Order = 14, TypeName = "NVARCHAR(150)")]
        public string Password { get; set; }

        [Column("Address", Order = 15, TypeName = "NVARCHAR(550)")]
        public string Address { get; set; }

        [Column("City", Order = 16, TypeName = "NVARCHAR(550)")]
        public string City { get; set; }

        [Column("PostalCode", Order = 17, TypeName = "NVARCHAR(150)")]
        public string PostalCode { get; set; }

        [Column("Country", Order = 18, TypeName = "NVARCHAR(150)")]
        public string Country { get; set; }

        [Column("Birthday", Order = 19, TypeName = "NVARCHAR(150)")]
        public string Birthday { get; set; }

        [Column("Email", Order = 20, TypeName = "NVARCHAR(550)")]
        public string Email { get; set; }

        [Column("Telephone", Order = 21, TypeName = "NVARCHAR(150)")]
        public string Telephone { get; set; }

        [Column("VIP", Order = 22, TypeName = "NVARCHAR(550)")]
        public string VIP { get; set; }

        [Column("Benefits", Order = 23, TypeName = "NVARCHAR(MAX)")]
        public string Benefits { get; set; }

        [Column("NationalityCode", Order = 24, TypeName = "NVARCHAR(150)")]
        public string NationalityCode { get; set; }

        [Column("ConfirmationCode", Order = 25, TypeName = "NVARCHAR(150)")]
        public string ConfirmationCode { get; set; }

        [Column("Type", Order = 26, TypeName = "INT")]
        public Nullable<int> Type { get; set; }

        [Column("Title", Order = 27, TypeName = "NVARCHAR(150)")]
        public string Title { get; set; }

        [Column("Adults", Order = 28, TypeName = "INT")]
        public Nullable<int> Adults { get; set; }

        [Column("Children", Order = 29, TypeName = "INT")]
        public Nullable<int> Children { get; set; }

        [Column("BoardCode", Order = 30, TypeName = "NVARCHAR(150)")]
        public string BoardCode { get; set; }

        [Column("BoardName", Order = 31, TypeName = "NVARCHAR(150)")]
        public string BoardName { get; set; }

        [Column("Note1", Order = 32, TypeName = "NVARCHAR(MAX)")]
        public string Note1 { get; set; }

        [Column("Note2", Order = 33, TypeName = "NVARCHAR(MAX)")]
        public string Note2 { get; set; }

        [Column("ReservationId", Order = 34, TypeName = "INT")]
        public Nullable<int> ReservationId { get; set; }

        [Column("IsSharer", Order = 35, TypeName = "BIT")]
        public Nullable<bool> IsSharer { get; set; }

        [Column("HotelId", Order = 36, TypeName = "BIGINT")]
        public Nullable<long> HotelId { get; set; }

        [Column("ClassId", Order = 37, TypeName = "INT")]
        public Nullable<int> ClassId { get; set; }

        [Column("ClassName", Order = 38, TypeName = "NVARCHAR(100)")]
        public string ClassName { get; set; }

        [Column("AvailablePoints", Order = 39, TypeName = "INT")]
        public Nullable<int> AvailablePoints { get; set; }

        [Column("fnbdiscount", Order = 40, TypeName = "INT")]
        public Nullable<int> fnbdiscount { get; set; }

        [Column("ratebuy", Order = 41, TypeName = "INT")]
        public Nullable<int> ratebuy { get; set; }
    }
}
