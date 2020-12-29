using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations
{
    public interface IReservationCustomersFlows
    {
        /// <summary>
        /// Get the List of Reservation Customers
        /// </summary>
        /// <returns></returns>
        ReservationCustomersListModel GetReservationCustomers(DBInfoModel Store);

        /// <summary>
        /// Gets the Customer's Informations From Room Number
        /// </summary>
        /// <param name="Room">RoomNumber</param>
        /// <returns></returns>
        CustomersInfo GetCustomersInfo(DBInfoModel Store, string Room);

        /// <summary>
        /// Gets details for a specific Reservation Customer
        /// </summary>
        /// <param name="Id">ReservationCustomerID</param>
        /// <returns></returns>
        ReservationCustomersModel GetReservationCustomerById(DBInfoModel Store, long Id);

        /// <summary>
        /// Get details for a specific Customers Reservation full model
        /// </summary>
        /// <param name="Id">ReservationCustomerID</param>
        /// <returns></returns>
        ExtecrTableReservetionModel GetCustomersReservation(DBInfoModel Store, long Id);

        /// <summary>
        /// Insert new Reservation Customer
        /// </summary>
        /// <returns></returns>
        ExtendedReservetionModel insertReservationCustomer(DBInfoModel Store, ExtendedReservetionModel model);

        /// <summary>
        /// Send an Email to Customers
        /// </summary>
        /// <returns></returns>
        string SendEmailToCustomers(DBInfoModel Store, ExtendedReservetionModel Model);

        /// <summary>
        /// Update a Reservation Customer
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ReservationCustomersModel UpdateReservationCustomer(DBInfoModel Store, ReservationCustomersModel Model);

        /// <summary>
        /// Delete a Reservation Customer
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteReservationCustomer(DBInfoModel Store, long Id);
    }
}
