using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_CustomerMessagesFlows : IDA_CustomerMessagesFlows
    {
        IDA_CustomerMessagesTasks task;
        IDA_CustomerTasks customerTasks;

        public DA_CustomerMessagesFlows(IDA_CustomerMessagesTasks task, IDA_CustomerTasks customerTasks)
        {
            this.task = task;
            this.customerTasks = customerTasks;
        }

        public List<DA_MainMessagesModel> GetAll(DBInfoModel dbInfo)
        {
            return task.GetAll(dbInfo);
        }

        public List<DA_MessagesModel> Get_DA_MessageById(DBInfoModel DBInfo, long MessageId, long HeaderDetailId)
        {
            return task.Get_DA_MessageById(DBInfo, MessageId, HeaderDetailId);
        }
        public List<DA_MessagesModel> GetMessageByMainMessageId(DBInfoModel DBInfo, long id)
        {
            return task.GetMessageByMainMessageId(DBInfo, id);
        }
        public List<DA_MessagesDetailsModel> GetMessageDetailsByMainMessageId(DBInfoModel DBInfo, long id)
        {
            return task.GetMessageDetailsByMainMessageId(DBInfo, id);
        }

        public List<MessagesLookup> GetMessagesLookups(DBInfoModel DBInfo, long MainDAMessagesID)
        {
            return task.GetMessagesLookups(DBInfo, MainDAMessagesID);
        }
        public List<MessagesDetailLookup> GetMessagesDetailsLookups(DBInfoModel DBInfo, long HeaderId)
        {
            return task.GetMessagesDetailsLookups(DBInfo, HeaderId);
        }

        public List<DA_CustomerMessagesModelExt> GetAllDACustomerMessages(DBInfoModel DBInfo, long CustomerId)
        {
            return task.GetAll_DA_CustomerMessages(DBInfo, CustomerId);
        }

        public List<DA_CustomerMessagesModelExt> GetAllCustomerMessages(DBInfoModel DBInfo)
        {
            return task.GetAllCustomerMessages(DBInfo);
        }

        public long InsertMainMessage(DBInfoModel DBInfo, DA_MainMessagesModel Model)
        {
            return task.InsertMainMessage(DBInfo, Model);
        }
        public long InsertMessage(DBInfoModel DBInfo, DA_MessagesModel Model)
        {
            return task.InsertMessage(DBInfo, Model);
        }
        public long InsertMessageDetail(DBInfoModel DBInfo, DA_MessagesDetailsModel Model)
        {
            return task.InsertMessageDetail(DBInfo, Model);
        }
        public long InsertDaCustomerMessage(DBInfoModel DBInfo, DA_CustomerMessagesCustomerNote Model)
        {
            long customerMessageId = task.InsertDaCustomerMessage(DBInfo, Model);
            if (Model.LastOrderNote != null)
            {
                customerTasks.UpdateLastOrderNote(DBInfo, Model.CustomerId, Model.LastOrderNote);
            }
            return customerMessageId;
        }

        public long UpdateDaCustomerMessage(DBInfoModel DBInfo, DA_CustomerMessagesCustomerNote Model)
        {
            long customerMessageId = task.UpdateDaCustomerMessage(DBInfo, Model);
            if (Model.LastOrderNote != null)
            {
                customerTasks.UpdateLastOrderNote(DBInfo, Model.CustomerId, Model.LastOrderNote);
            }
            return customerMessageId;
        }

        public long UpdateMainMessage(DBInfoModel DBInfo, DA_MainMessagesModel Model)
        {
            return task.UpdateMainMessage(DBInfo, Model);
        }
        public long UpdateMessage(DBInfoModel DBInfo, DA_MessagesModel Model)
        {
            return task.UpdateMessage(DBInfo, Model);
        }
        public long UpdateMessageDetail(DBInfoModel DBInfo, DA_MessagesDetailsModel Model)
        {
            return task.UpdateMessageDetail(DBInfo, Model);
        }

        public long DeleteMainMessage(DBInfoModel dbInfo, long Id)
        {
            return task.DeleteMainMessage(dbInfo, Id);
        }
        public long DeleteMessage(DBInfoModel dbInfo, long Id)
        {
            return task.DeleteMessage(dbInfo, Id);
        }
        public long DeleteMessageDetail(DBInfoModel dbInfo, long Id)
        {
            return task.DeleteMessageDetail(dbInfo, Id);
        }

    }
}
