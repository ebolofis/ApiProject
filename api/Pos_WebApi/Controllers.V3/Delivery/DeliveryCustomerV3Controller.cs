using Microsoft.AspNet.SignalR;
using Pos_WebApi.Models;
using ServiceStack.Web;
using Symposium.Helpers;
using Symposium.Models;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/DeliveryCustomer")]
    public class DeliveryCustomerV3Controller : BasicV3Controller
    {
        IDeliveryCustomerFlows dcustflow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public DeliveryCustomerV3Controller(IDeliveryCustomerFlows _dcustflow)
        {
            this.dcustflow = _dcustflow;
        }

        /// <summary>
        /// Provide model of types loaded
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetLookupEnums")]
        public HttpResponseMessage GetLookupEnums()
        {
            DeliveryCustomerLookupModel res = dcustflow.GetLookups(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
        [HttpGet, Route("GetCustomer/{Id}/{PhoneId}/{SAddressId}/{BAddressId}")]
        public HttpResponseMessage GetCustomer(long Id, long PhoneId, long SAddressId, long BAddressId)
        {
            DeliveryCustomerModel res = dcustflow.GetCustomerById(DBInfo, Id, PhoneId, SAddressId, BAddressId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Search api call with page size and filter model
        /// goes through flow to provide a search mech under DB 
        /// </summary>
        /// <param name="page">Page to search</param>
        /// <param name="pageSize">Count of page registers</param>
        /// <param name="filters">Name phone address trn</param>
        /// <returns>Paged Result </returns>
        [HttpPost, Route("SearchPaged/{page}/{pageSize}")]
        public HttpResponseMessage SearchPaged(int page, int pageSize, DeliveryCustomerFilterModel filters)
        {
            PaginationModel<DeliveryCustomerSearchModel> res = dcustflow.SearchPagedCustomersFlow(DBInfo, page, pageSize, filters);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
        /// <summary>
        /// Search api call with page size and filter model
        /// goes through flow to provide a search mech under DB 
        /// </summary>
        /// <param name="page">Page to search</param>
        /// <param name="pageSize">Count of page registers</param>
        /// <param name="filters">Name phone address trn</param>
        /// <returns>Paged Result </returns>
        [HttpPost, Route("UpsertCustomer")]
        public HttpResponseMessage UpsertCustomer(DeliveryCustomerModel Model)
        {
            DeliveryCustomerModel res = dcustflow.UpsertCustomer(DBInfo, Model);
            DeliveryCustomerSignalModel signalmodel = new DeliveryCustomerSignalModel
            {
                CustomerID = res.ID,
                Cause = ((Model.ID == 0) ? (int)DeliveryCustomerSignalCause.New : (int)DeliveryCustomerSignalCause.Edit)
            };
            signalUserChange(DBInfo.Id, "agent_SignalCustomerUpdated", signalmodel);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        [HttpPost, Route("UpsertCustomerByExternalId")]
        public HttpResponseMessage UpsertCustomerByExternalId(DeliveryCustomerModel Model)
        {
            DeliveryCustomerModel res = dcustflow.UpsertCustomerByExternalId(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        [HttpPost, Route("SaveCustomerReturnGuest")]
        public HttpResponseMessage SaveCustomerReturnGuest(DeliveryCustomerModel Model)
        {
            GuestModel res = dcustflow.UpdateCustomerAndGuest(DBInfo, Model);
            DeliveryCustomerSignalModel signalmodel = new DeliveryCustomerSignalModel
            {
                CustomerID = Model.ID,
                GuestID = res.Id,
                Cause = (int)DeliveryCustomerSignalCause.Edit
            };
            signalUserChange(DBInfo.Id, "agent_SignalCustomerUpdated", signalmodel);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Functional call of delivery service posts a model and procedure Inserts or Updates a Customer then 
        /// Updates or inserts a Guest and return model to service 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpsertCustomerFromService")]
        public HttpResponseMessage UpdateCustomerFromService(DeliveryCustomerModelDS Model)
        {
            DeliveryCustomerModelDS res = dcustflow.UpsertCustomerAndGuest(DBInfo, Model);
            DeliveryCustomerSignalModel signalmodel = new DeliveryCustomerSignalModel
            {
                CustomerID = Model.ID,
                GuestID = res.GuestId,
                Cause = (int)DeliveryCustomerSignalCause.Edit
            };
            signalUserChange(DBInfo.Id, "agent_SignalCustomerUpdated", signalmodel);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        [HttpPost, Route("DeleteCustomer/{id}")]
        public HttpResponseMessage DeleteCustomer(long id)
        {
            var res = dcustflow.DeleteCustomer(DBInfo, id);
            DeliveryCustomerSignalModel signalmodel = new DeliveryCustomerSignalModel
            {
                CustomerID = id,
                Cause = (int)DeliveryCustomerSignalCause.Delete
            };
            signalUserChange(DBInfo.Id, "agent_SignalCustomerDeleted", signalmodel);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Uses  Signal-R to update POS via msg
        /// </summary>
        /// <param name="model"></param>
        /// <param name="res"></param>
        private void signalUserChange(Guid storeid, string msg, DeliveryCustomerSignalModel model)
        {
            //switch (model.Cause)
            //{
            //    case DeliveryCustomerSignalCause.Delete: msg = "agent_SignalCustomerDeleted"; break;
            //    case DeliveryCustomerSignalCause.New: msg = "agent_SignalCustomerAdded"; break;
            //    case DeliveryCustomerSignalCause.Edit: msg = "agent_SignalCustomerUpdated"; break;
            //    default: break;
            //}
            hub.Clients.Group(DBInfo.Id.ToString()).deliveryCustomerMessage(storeid, msg, model);
        }
    }
}