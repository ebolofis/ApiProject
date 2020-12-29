using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.MainLogic.Flows.Hotelizer;
using Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.TableReservations
{
    public class ReservationCustomersFlows : IReservationCustomersFlows
    {
        IReservationCustomersTasks ReservationCustomersTasks;

        public ReservationCustomersFlows(IReservationCustomersTasks resCusTasks)
        {
            this.ReservationCustomersTasks = resCusTasks;
        }


        /// <summary>
        /// Returns the List of Reservation Customers
        /// </summary>
        /// <returns></returns>
        public ReservationCustomersListModel GetReservationCustomers(DBInfoModel Store)
        {
            // get the results
            ReservationCustomersListModel reservationCustomersDetails = ReservationCustomersTasks.GetReservationCustomers(Store);

            return reservationCustomersDetails;
        }

        /// <summary>
        /// Returns the Customer's Informations From Room Number
        /// </summary>
        /// <param name="Room">RoomNumber</param>
        /// <returns></returns>
        public CustomersInfo GetCustomersInfo(DBInfoModel Store, string Room)
        {
            // get the results
            CustomersInfo customersInfoDetails;

            //checks if nedd to call Api Url for Hotelizer or other external systems
            bool extApi = false;
            extApi = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "isExternalApi");
            if (extApi)
            {
                HotelizerFlows hotelizer = new HotelizerFlows();
                List<CustomersDetails> hotCustomers = hotelizer.GetRoomsAsCustomersDetails(Room, 0, 0, 2000);
                if (hotCustomers == null || hotCustomers.Count < 1)
                    throw new Exception("No Customer found for Room Number : " + Room + ".");
                else
                {
                    customersInfoDetails = new CustomersInfo();
                    customersInfoDetails.Email = hotCustomers[0].Email;
                    customersInfoDetails.FirstName = hotCustomers[0].FirstName;
                    customersInfoDetails.LastName = hotCustomers[0].LastName;
                    customersInfoDetails.NumberOfPeople = hotCustomers[0].Adults + hotCustomers[0].Children;
                    customersInfoDetails.ProfileNo = hotCustomers[0].ProfileNo;
                    customersInfoDetails.Room = Room;
                }
            }
            else
                customersInfoDetails = ReservationCustomersTasks.GetCustomersInfo(Store, Room);

            return customersInfoDetails;
        }

        /// <summary>
        /// Returns details for a specific Reservation Customer
        /// </summary>
        /// <param name="Id">ReservationCustomerID</param>
        /// <returns></returns>
        public ReservationCustomersModel GetReservationCustomerById(DBInfoModel Store, long Id)
        {
            // get the results
            ReservationCustomersModel reservationCustomerDetails = ReservationCustomersTasks.GetReservationCustomerById(Store, Id);

            return reservationCustomerDetails;
        }

        /// <summary>
        /// Returns details for a specific Customers Reservation full model
        /// </summary>
        /// <param name="Id">ReservationCustomerID</param>
        /// <returns></returns>
        public ExtecrTableReservetionModel GetCustomersReservation(DBInfoModel Store, long Id)
        {
            // get the results
            ExtecrTableReservetionModel reservationCustomerDetails = ReservationCustomersTasks.GetCustomersReservation(Store, Id);

            return reservationCustomerDetails;
        }

        /// <summary>
        /// Insert new Reservation Customer
        /// </summary>
        /// <returns></returns>
        public ExtendedReservetionModel insertReservationCustomer(DBInfoModel Store, ExtendedReservetionModel model)
        {
            return ReservationCustomersTasks.insertReservationCustomer(Store, model);
        }

        /// <summary>
        /// Send an Email to Customers
        /// </summary>
        /// <returns></returns>
        public string SendEmailToCustomers(DBInfoModel Store, ExtendedReservetionModel Model)
        {
            return ReservationCustomersTasks.SendEmailToCustomers(Store, Model);
        }

        /// <summary>
        /// Update a Reservation Customer
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ReservationCustomersModel UpdateReservationCustomer(DBInfoModel Store, ReservationCustomersModel Model)
        {
            return ReservationCustomersTasks.UpdateReservationCustomer(Store, Model);
        }

        /// <summary>
        /// Delete a Reservation Customer
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteReservationCustomer(DBInfoModel Store, long Id)
        {
            return ReservationCustomersTasks.DeleteReservationCustomer(Store, Id);
        }
    }
}
