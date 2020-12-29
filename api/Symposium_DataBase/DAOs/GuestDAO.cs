using Dapper;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class GuestDAO : IGuestDAO
    {
        public long upsertGuestFromDeliveryCustomer(IDbConnection db, DeliveryCustomerModel model)
        {

            long retid = 0;
            GuestDTO guest = db.GetList<GuestDTO>("where ProfileNo = @ProfileNo", new { ProfileNo = model.ID }).FirstOrDefault();
            if (guest != null)
            {
                guest = GuestFromDeliveryCustomer(db, guest, model);
                var i = db.Update(guest);
                retid = guest.Id;
            }
            else
            {
                guest = new GuestDTO();
                guest = GuestFromDeliveryCustomer(db, guest, model);
                retid = db.Insert<long>(guest);
            }
            return retid;
        }


        /// <summary>
        /// Provide a guest DTO by call or new and a deliveryCustomer Model 
        /// Updates Guest Model with respect to delivery Fields
        /// </summary>
        /// <param name="db"></param>
        /// <param name="editguest">an object DTO to update or insert </param>
        /// <param name="model">DeliveryCustomer to use for update values</param>
        /// <returns>Guest DTO provided and changed</returns>
        public GuestDTO GuestFromDeliveryCustomer(IDbConnection db, GuestDTO editguest, DeliveryCustomerModel model)
        {
            try
            {

                //get selected phone or first phone of user
                DeliveryCustomersPhonesModel phone = model.Phones.FirstOrDefault(x => x.IsSelected == true);
                if (phone == null)
                {
                    phone = model.Phones.FirstOrDefault();
                }
                //define addresses of guest for shipping details 
                DeliveryCustomersShippingAddressModel address = model.ShippingAddresses.FirstOrDefault(x => x.IsSelected == true);
                if (address == null)
                {
                    address = model.ShippingAddresses.FirstOrDefault();
                }
                //long Id = zaza ;
                //Nullable<System.DateTime> arrivalDT = zaza ;
                //Nullable<System.DateTime> departureDT = zaza ;
                //Nullable<System.DateTime> birthdayDT = zaza ;
                //string Room = zaza ;
                //Nullable<int> RoomId = zaza ;
                //string Arrival = zaza ;
                //string Departure = zaza ;
                //string ReservationCode = zaza ;
                editguest.ProfileNo = (int?)model.ID;
                editguest.FirstName = model.FirstName;
                editguest.LastName = model.LastName;
                //string Member = zaza ;
                //string Password = zaza ;
                editguest.Address = address.AddressStreet + " " + address.AddressNo;
                editguest.City = address.City;
                editguest.PostalCode = address.Zipcode;
                //editguest.Country = address.Country;
                //editguest.Birthday = zaza;
                editguest.Email = model.email;
                editguest.Telephone = phone.PhoneNumber;
                //string VIP = zaza ;
                //string Benefits = zaza ;
                //string NationalityCode = zaza ;
                //string ConfirmationCode = zaza;
                //Nullable<int> Type = zaza;
                //string Title = zaza;
                //Nullable<int> Adults = zaza ;
                //Nullable<int> Children = zaza ;
                //string BoardCode = zaza;
                //string BoardName = zaza;
                editguest.Note1 = model.Comments;
                //string Note2 = zaza;
                //Nullable<int> ReservationId = zaza ;
                //Nullable<bool> IsSharer = zaza ;
                //Nullable<long> HotelId = zaza ;
                //Nullable<int> ClassId = zaza ;
                //string ClassName = zaza ;
                //Nullable<int> AvailablePoints = zaza ;
                //Nullable<int> fnbdiscount = zaza ;
                //Nullable<int> ratebuy = zaza ;

                return editguest;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }



}
