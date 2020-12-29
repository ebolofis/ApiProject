using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_CustomerMessagesFlows
    {
        List<DA_MainMessagesModel> GetAll(DBInfoModel dbInfo);
        List<DA_MessagesModel> Get_DA_MessageById(DBInfoModel DBInfo, long MessageId, long HeaderDetailId);
        List<DA_MessagesModel> GetMessageByMainMessageId(DBInfoModel DBInfo, long id);   // Main Message matches to messages
        List<DA_MessagesDetailsModel> GetMessageDetailsByMainMessageId(DBInfoModel DBInfo, long id);   // Main Message matches to messages
        List<MessagesLookup> GetMessagesLookups(DBInfoModel DBInfo, long MainDAMessagesID);
        List<MessagesDetailLookup> GetMessagesDetailsLookups(DBInfoModel DBInfo, long HeaderId);
        List<DA_CustomerMessagesModelExt> GetAllDACustomerMessages(DBInfoModel DBInfo, long CustomerId);
        List<DA_CustomerMessagesModelExt> GetAllCustomerMessages(DBInfoModel DBInfo);
        long InsertMainMessage(DBInfoModel DBInfo, DA_MainMessagesModel Model);
        long InsertMessage(DBInfoModel DBInfo, DA_MessagesModel Model);
        long InsertMessageDetail(DBInfoModel DBInfo, DA_MessagesDetailsModel Model);
        long InsertDaCustomerMessage(DBInfoModel dBInfo, DA_CustomerMessagesCustomerNote Model);
        long UpdateDaCustomerMessage(DBInfoModel DBInfo, DA_CustomerMessagesCustomerNote Model);
        long UpdateMainMessage(DBInfoModel DBInfo, DA_MainMessagesModel Model);
        long UpdateMessage(DBInfoModel DBInfo, DA_MessagesModel Model);
        long UpdateMessageDetail(DBInfoModel DBInfo, DA_MessagesDetailsModel Model);
        long DeleteMainMessage(DBInfoModel dbInfo, long Id);
        long DeleteMessage(DBInfoModel dbInfo, long Id);
        long DeleteMessageDetail(DBInfoModel dbInfo, long Id);
    }
}
