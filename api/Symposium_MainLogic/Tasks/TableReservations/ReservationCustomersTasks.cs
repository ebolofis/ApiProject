using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.TableReservations
{
    public class ReservationCustomersTasks : IReservationCustomersTasks
    {
        IReservationCustomersDT reservationCustomersDT;
        IEmailDT emailDT;
        IConfigDT configDT;
        IEmailTasks sendEmail;

        public ReservationCustomersTasks(IReservationCustomersDT resCusDT, IEmailDT eDT, IConfigDT conDT, IEmailTasks send)
        {
            this.reservationCustomersDT = resCusDT;
            this.emailDT = eDT;
            this.configDT = conDT;
            this.sendEmail = send;
        }

        /// <summary>
        /// Returns the List of Reservation Customers
        /// </summary>
        /// <returns></returns>
        public ReservationCustomersListModel GetReservationCustomers(DBInfoModel Store)
        {
            // get the results
            ReservationCustomersListModel reservationCustomersDetails = reservationCustomersDT.GetReservationCustomers(Store);

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
            CustomersInfo customersInfoDetails = reservationCustomersDT.GetCustomersInfo(Store, Room);

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
            ReservationCustomersModel reservationCustomerDetails = reservationCustomersDT.GetReservationCustomerById(Store, Id);

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
            ExtecrTableReservetionModel reservationCustomerDetails = reservationCustomersDT.GetCustomersReservation(Store, Id);

            return reservationCustomerDetails;
        }

        /// <summary>
        /// Insert new Reservation Customer
        /// </summary>
        /// <returns></returns>
        public ExtendedReservetionModel insertReservationCustomer(DBInfoModel Store, ExtendedReservetionModel model)
        {
            return reservationCustomersDT.insertReservationCustomer(Store, model);
        }

        /// <summary>
        /// Send an Email to Customers
        /// </summary>
        /// <returns></returns>
        public string SendEmailToCustomers(DBInfoModel Store, ExtendedReservetionModel Model)
        {
            string restaurantName = emailDT.SendEmailToCustomers(Store, Model);

            // Get email template from DB
            ConfigModel emailDetails = configDT.GetConfig(Store);

            List<string> emailReceivers = new List<string>();
            foreach (ReservationCustomersModel item in Model.ReservationCustomers)
            {
                emailReceivers.Add(item.Email);
            }

            if (emailReceivers != null)
            {
                string date = Model.Reservation.ReservationDate.ToString("dd/MM/yyyy");
                string time = Model.Reservation.ReservationTime.ToString(@"hh\:mm");
                string tdate = DateTime.Now.ToString("dd/MM/yyyy");

                EmailSendModel email = new EmailSendModel();
                email.To = new List<string>();
                email.Subject =
                    emailDetails.EmailSubject.Replace("@Restaurant", restaurantName);
                email.Body =
                    emailDetails.EmailTemplate.Replace("@ReservationId", Model.Reservation.Id.ToString())
                                              .Replace("@Restaurant", restaurantName)
                                              .Replace("@ReservationDate", date)
                                              .Replace("@ReservationTime", time)
                                              .Replace("@Cover", Model.Reservation.Couver.ToString())
                                              .Replace("@CreateDate", tdate);
                email.To.AddRange(emailReceivers);

                sendEmail.SendEmail(email, Store, false);
            }

            return restaurantName;
        }


        /// <summary>
        /// Update a Reservation Customer
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ReservationCustomersModel UpdateReservationCustomer(DBInfoModel Store, ReservationCustomersModel Model)
        {
            return reservationCustomersDT.UpdateReservationCustomer(Store, Model);
        }

        /// <summary>
        /// Delete a Reservation Customer
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public long DeleteReservationCustomer(DBInfoModel Store, long Id)
        {
            return reservationCustomersDT.DeleteReservationCustomer(Store, Id);
        }
    }
}
