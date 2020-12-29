using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_CustomerMessagesDT
    {
        List<DA_MainMessagesModel> GetAll(DBInfoModel dbInfo);
        List<DA_MessagesModel> Get_DA_MessageById(DBInfoModel DBInfo, long MessageId, long HeaderDetailId);
        List<DA_MessagesModel> GetMessageByMainMessageId(DBInfoModel DBInfo, long id);
        List<DA_MessagesDetailsModel> GetMessageDetailsByMainMessageId(DBInfoModel DBInfo, long id);
        DA_MainMessagesModel GetMainMessageById(DBInfoModel DBInfo, long Id);
        DA_MessagesModel GetMessageById(DBInfoModel DBInfo, long Id);
        DA_MessagesDetailsModel GetMessageDetailById(DBInfoModel DBInfo, long Id);
        List<DA_MessagesModel> GetOnCreateMessages(DBInfoModel DBInfo);
        List<DA_MessagesDetailsModel> GetOnCreateMessageDetails(DBInfoModel DBInfo);
        List<DA_MessagesModel> GetOnUpdateMessages(DBInfoModel DBInfo);
        List<DA_MessagesDetailsModel> GetOnUpdateMessageDetails(DBInfoModel DBInfo);
        List<DA_MessagesModel> GetOnCallMessages(DBInfoModel DBInfo);
        List<DA_MessagesDetailsModel> GetOnCallMessageDetails(DBInfoModel DBInfo);
        List<MessagesLookup> GetMessagesLookups(DBInfoModel DBInfo, long MainDAMessagesID);
        List<MessagesDetailLookup> GetMessagesDetailsLookups(DBInfoModel DBInfo, long Headerid);
        List<DA_CustomerMessagesModelExt> GetAll_DA_CustomerMessages(DBInfoModel DBInfo, long CustomerId);
        List<DA_CustomerMessagesModelExt> GetAllCustomerMessages(DBInfoModel DBInfo);
        long InsertMainMessage(DBInfoModel DBInfo, DA_MainMessagesModel Model);
        long InsertMessage(DBInfoModel DBInfo, DA_MessagesModel Model);
        long InsertMessageDetail(DBInfoModel DBInfo, DA_MessagesDetailsModel Model);
        long InsertDaCustomerMessage(DBInfoModel DBInfo, DA_CustomerMessagesModel Model);
        long UpdateDaCustomerMessage(DBInfoModel DBInfo, DA_CustomerMessagesModel Model);
        long UpdateMainMessage(DBInfoModel DBInfo, DA_MainMessagesModel Model);
        long UpdateMessage(DBInfoModel DBInfo, DA_MessagesModel Model);
        long UpdateMessageDetail(DBInfoModel DBInfo, DA_MessagesDetailsModel Model);
        long DeleteMainMessage(DBInfoModel dbInfo, long Id);
        long DeleteMessage(DBInfoModel dbInfo, long Id);
        long DeleteMessageDetail(DBInfoModel dbInfo, long Id);
    }
}
