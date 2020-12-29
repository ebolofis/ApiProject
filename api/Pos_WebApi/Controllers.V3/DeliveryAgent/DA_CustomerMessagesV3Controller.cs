
using Pos_WebApi.Models.FilterModels;
using Pos_WebApi.Modules;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
    [RoutePrefix("api/v3/da/CustomerMessages")]
    public class DA_CustomerMessagesV3Controller : BasicV3Controller
    {
        IDA_CustomerMessagesFlows custmessagesflow;

        public DA_CustomerMessagesV3Controller(IDA_CustomerMessagesFlows _custmsgflow)
        {
            this.custmessagesflow = _custmsgflow;
        }

        [HttpGet, Route("GetAllMessages")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage Get_DA_CustomerMessages()
        {
            List<DA_MainMessagesModel> res = custmessagesflow.GetAll(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("GetMessagesLookups/id/{id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetMessagesLookups(long id)
        {
            List<MessagesLookup> res = custmessagesflow.GetMessagesLookups(DBInfo, id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("GetMessagesLookupsDetails/id/{id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetMessagesDetailsLookups(long id)
        {
            List<MessagesDetailLookup> res = custmessagesflow.GetMessagesDetailsLookups(DBInfo, id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("GetMessageByMainMessageId/id/{id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetMessageByMainMessageId(long id)
        {
            List<DA_MessagesModel> res = custmessagesflow.GetMessageByMainMessageId(DBInfo, id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("GetMessageDetailsMessageId/id/{id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetMessageDetailsByMainMessageId(long id)
        {
            List<DA_MessagesDetailsModel> res = custmessagesflow.GetMessageDetailsByMainMessageId(DBInfo, id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("GetMessageById")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage Get_DA_MessageById(long MessageId, long HeaderDetailId)
        {
            List<DA_MessagesModel> res = custmessagesflow.Get_DA_MessageById(DBInfo, MessageId, HeaderDetailId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("GetDACustomerMessage/{CustomerId}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetDACustomerMessage(long CustomerId)
        {
            List<DA_CustomerMessagesModelExt> res = custmessagesflow.GetAllDACustomerMessages(DBInfo, CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("GetAllCustomerMessage")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetAllCustomerMessages()
        {
            List<DA_CustomerMessagesModelExt> res = custmessagesflow.GetAllCustomerMessages(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        ////////////// INSERTS /////////////////////
        [HttpPost, Route("InsertMainMessage")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage InsertMainMessage(DA_MainMessagesModel Model)
        {
            long res = custmessagesflow.InsertMainMessage(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("InsertMessage")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage InsertMessage(DA_MessagesModel Model)
        {
            long res = custmessagesflow.InsertMessage(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("InsertMessageDetail")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage InsertDaCustomerMessageDetail(DA_MessagesDetailsModel Model)
        {
            long res = custmessagesflow.InsertMessageDetail(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("InsertDaCustomerMessage")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage InsertDaCustomerMessage(DA_CustomerMessagesCustomerNote Model)
        {
            long res = custmessagesflow.InsertDaCustomerMessage(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("UpdateDaCustomerMessage")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage UpdateDaCustomerMessage(DA_CustomerMessagesCustomerNote Model)
        {
            long res = custmessagesflow.UpdateDaCustomerMessage(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        ////////////// UPDATES /////////////////////
        [HttpPost, Route("UpdateMainMessage")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage UpdateMainMessage(DA_MainMessagesModel Model)
        {
            long res = custmessagesflow.UpdateMainMessage(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("UpdateMessage")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage UpdateMessage(DA_MessagesModel Model)
        {
            long res = custmessagesflow.UpdateMessage(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("UpdateMessageDetail")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage UpdateMessageDetail(DA_MessagesDetailsModel Model)
        {
            long res = custmessagesflow.UpdateMessageDetail(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        ////////////// DELETES /////////////////////
        [HttpPost, Route("DeleteMainMessage/Id/{Id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage DeleteMainMessage(long Id)
        {
            long res = custmessagesflow.DeleteMainMessage(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("DeleteMessage/Id/{Id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage DeleteMessage(long Id)
        {
            long res = custmessagesflow.DeleteMessage(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("DeleteMessageDetail/Id/{Id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage DeleteMessageDetail(long Id)
        {
            long res = custmessagesflow.DeleteMessageDetail(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

    }
}