using Microsoft.AspNet.SignalR;
using Pos_WebApi.Hubs;
using Pos_WebApi.Modules;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/ReservationCustomers")]
    public class ReservationCustomersV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IReservationCustomersFlows ReservationCustomersFlow;
        IConfigFlows conFlow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public ReservationCustomersV3Controller(IReservationCustomersFlows resCusFlows, IConfigFlows conFlows)
        {
            this.ReservationCustomersFlow = resCusFlows;
            this.conFlow = conFlows;
        }

        /// <summary>
        /// Gets the List of Reservation Customers
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList")]
        public HttpResponseMessage GetReservationCustomers()
        {
            ReservationCustomersListModel result = ReservationCustomersFlow.GetReservationCustomers(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result.ReservationCustomersModelList);
        }

        /// <summary>
        /// Gets the Customer's Informations From Room Number
        /// </summary>
        /// <param name="Room">RoomNumber</param>
        /// <returns></returns>
        [HttpGet, Route("GetCustomerInfo/Room/{Room}")]
        public HttpResponseMessage GetCustomersInfo(string Room)
        {
            CustomersInfo result = ReservationCustomersFlow.GetCustomersInfo(DBInfo, Room);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Gets a specific Reservation Customer by Id
        /// </summary>
        /// <param name="Id">ReservationID</param>
        /// <returns></returns>
        [HttpGet, Route("Get/Id/{Id}")]
        public HttpResponseMessage GetReservationCustomerById(long Id)
        {
            ReservationCustomersModel result = ReservationCustomersFlow.GetReservationCustomerById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        /// <summary>
        /// Gets a specific Customers Reservation full model
        /// </summary>
        /// <param name="Id">ReservationID</param>
        /// <returns></returns>
        [HttpGet, Route("GetCustomersReservation/Id/{Id}")]
        public HttpResponseMessage GetCustomersReservation(long Id)
        {
            ExtecrTableReservetionModel result = ReservationCustomersFlow.GetCustomersReservation(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Insert new Reservation Customer 
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [ValidateModelState]
        public HttpResponseMessage insertReservationCustomer(ExtendedReservetionModel model)
        {
            ExtendedReservetionModel result = new ExtendedReservetionModel();
            //case 0 is when we have new model, case -1 is for extecr error but reservation saved
            if (model.Reservation.Id == 0)
            {
                result = ReservationCustomersFlow.insertReservationCustomer(DBInfo, model);
            }

            //send Email
            if(model.SendEmail == true)
            {
                string restName = ReservationCustomersFlow.SendEmailToCustomers(DBInfo, result);
            }

            //Send Message to ExtECR
            ConfigModel configInfo = conFlow.GetConfig(DBInfo);
            if (model.PrintToExtECR == true)
            {
                string conId = WebPosHub._connections.GetConnections(configInfo.ExtECR);
                if (conId != null)
                {
                    hub.Clients.Client(conId).newTableReservation(result.Reservation.Id);
                }
                else
                {
                    throw new Exception("Reservation with Id : " + result.Reservation.Id + " was Successful! However, there was no Printer Connected...!");
                }
            }

            if (!model.SendEmail && !model.PrintToExtECR)
                return Request.CreateResponse(HttpStatusCode.OK, model);
            else
                return Request.CreateResponse(HttpStatusCode.OK, result);
        }



        /// <summary>
        /// Update a specific Reservation Customer to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateReservationCustomer(ReservationCustomersModel Model)
        {
            ReservationCustomersModel result = ReservationCustomersFlow.UpdateReservationCustomer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a specific Reservation Customer from DB
        /// </summary>
        /// <param name="Id">ReservationID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteReservationCustomer(long Id)
        {
            long result = ReservationCustomersFlow.DeleteReservationCustomer(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}