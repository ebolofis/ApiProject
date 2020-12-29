using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Plugins.PmsInterface
{

    /// <summary>
    /// Results from GetReservationInfo2 store procedure
    /// </summary>
    public class GetReservationInfoModel
    {
        /// <summary>
        /// Total Records
        /// </summary>
        public int TotalRecs { get; set; }

        /// <summary>
        /// Current row Id
        /// </summary>
        public int Row_ID { get; set; }

        /// <summary>
        /// Reservation reference (string1 or bekraref)
        /// </summary>
        public string ReservationCode { get; set; }

        /// <summary>
        /// Reservation Id (buchnr or bekrapri)
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// First creation Reservation Id (leistacc or bekrapri)
        /// </summary>
        public int OriginalReservationId { get; set; }

        /// <summary>
        /// Room id (protel.zimmer or -1 on Ermis)
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Room name (zimname or bereroom)
        /// </summary>
        public string Room { get; set; }

        /// <summary>
        /// Reservations arrival (globdvon or bekraarr)
        /// </summary>
        public DateTime arrivalDT { get; set; }

        /// <summary>
        /// Reservations departure (globdbis or bekradep)
        /// </summary>
        public DateTime departureDT { get; set; }

        /// <summary>
        /// Profile Id (kundennr or bereaaa)
        /// </summary>
        public int ProfileNo { get; set; }

        /// <summary>
        /// Profile first name (vorname or berenam)
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Profile last name (name1 or bereepo)
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Profile mebmer id (member or '')
        /// </summary>
        public string Member { get; set; }

        /// <summary>
        /// Profile password (passwd or '')
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Profile addresss (strasse or bereadr)
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Profile city (ort or beretge)
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Profile post code (plz or '')
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Profile Country (land or bereeth)
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Profile birthday (gebdat or berehge)
        /// </summary>
        public DateTime birthdayDT { get; set; }

        /// <summary>
        /// Profile email (email or bereemail)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Profile telephone (telephonnr or berephone)
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Profile vip status (VIP or -1)
        /// </summary>
        public string VIP { get; set; }

        /// <summary>
        /// Profile benefits (remindtx or '')
        /// </summary>
        public string Benefits { get; set; }

        /// <summary>
        /// Profile nationality id (nat or (not in Ermis))
        /// </summary>
        public int NatId { get; set; }

        /// <summary>
        ///  Reservations adults (anzerw + zbet or bekrapax + bekraepax)
        /// </summary>
        public int Adults { get; set; }

        /// <summary>
        /// Reservations Children (anzkin1+anzkin2+anzkin3+anzkin4 or bekrapaxc+bekrapaxd+bekrachc+bekrachd)
        /// </summary>
        public int Children { get; set; }

        /// <summary>
        /// Reservations notice (not1txt or bekracom)
        /// </summary>
        public string not1txt { get; set; }

        /// <summary>
        /// Reservations notice (not2txt or bekracom1)
        /// </summary>
        public string not2txt { get; set; }

        /// <summary>
        /// Profile Id (kundennr or bereaaa)
        /// </summary>
        public int kdnr { get; set; }

        /// <summary>
        /// If reservtion shares the room (sharenr or bekrares1)
        /// </summary>
        public bool IsSharer { get; set; }

        /// <summary>
        /// Profile boards (short or bereboard)
        /// </summary>
        public string BoardCode { get; set; }

        /// <summary>
        /// Profile boards (short or bereboard)
        /// </summary>
        public string BoardName { get; set; }

        /// <summary>
        /// If reservation can use pos to charge the room (ifcdata.kd or 0)
        /// </summary>
        public int noPos { get; set; }

        /// <summary>
        /// Loyalty class id (hit_loyalty_classes.id or -1)
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// Loyalty class name (hit_loyalty_classes.name or '')
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Available loyalty points (hit_loyalty_kunden.points or 0)
        /// </summary>
        public int AvailablePoints { get; set; }

        /// <summary>
        /// discount on pos for loyalty custiomers (hit_loyalty_classes.fnbdiscount or 0)
        /// </summary>
        public int fnbdiscount { get; set; }

        /// <summary>
        /// point and euro analogy for loyalty custiomers (hit_loyalty_generalrules.ratebuy or 0)
        /// </summary>
        public int ratebuy { get; set; }

        /// <summary>
        /// Profile future benefits (gfeat.short or '')
        /// </summary>
        public string future { get; set; }

        /// <summary>
        /// Room type Id (katnr or (not in Ermis))
        /// </summary>
        public int RoomTypeId { get; set; }

        /// <summary>
        /// Room type (kat or beretyp)
        /// </summary>
        public string RoomType { get; set; }

        /// <summary>
        /// Booked room type id (orgkatnr or (not in Ermis))
        /// </summary>
        public int BookedRoomTypeId { get; set; }

        /// <summary>
        /// Booked room type (kat or beretyp2)
        /// </summary>
        public string BookedRoomType { get; set; }

        /// <summary>
        /// Travel agent id (reisernr or (not in Ermis))
        /// </summary>
        public int TA_ID { get; set; }

        /// <summary>
        /// Travel agent (kunden.name1 or bekraprakt)
        /// </summary>
        public string TAName { get; set; }

        /// <summary>
        /// Company id (firmnr or (not in Ermis))
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Company (kunden.name1 or (not in Ermis))
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Group id (gruppenr or (not in Ermis))
        /// </summary>
        public int GrpoupId { get; set; }

        /// <summary>
        /// Group name (kunden.name1 or (not in Ermis))
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Source id (sourcenr or (not in Ermis))
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// Source name (kunden.name1 or (not in Ermis))
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// Hotel Id (mpehotel or (not in Ermis))
        /// </summary>
        public int mpehotel { get; set; }

        /// <summary>
        /// Reservations status (confirm, checkin or checkout) (buchstatus or (not in Ermis))
        /// </summary>
        public int ReservStatus { get; set; }

        /// <summary>
        /// Profile Nationality code (codenr or beretge)
        /// </summary>
        public string NationalityCode { get; set; }

        /// <summary>
        /// Profile Loyalty Program id (loyalcrd.prgid or (not in Ermis))
        /// </summary>
        public int LoyaltyProgramId { get; set; }

        /// <summary>
        /// Profile Loyalty Level id (loyalcrd.llevelid or (not in Ermis))
        /// </summary>
        public int LoyaltyLevelId { get; set; }

        /// <summary>
        /// Rate code Id (preistyp or (not in Ermis))
        /// </summary>
        public int RateCodeId { get; set; }

        /// <summary>
        /// Rate code description (ptyp.grp+' '+ptyp.ptypnr or (not in Ermis))
        /// </summary>
        public string RateCodeDescr { get; set; }

        /// <summary>
        /// Profile Loyalty card type (loylvl.short_text or (not in Ermis))
        /// </summary>
        public string CardType { get; set; }
    }
}
