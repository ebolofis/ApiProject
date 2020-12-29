using Dapper;
using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{

    public class DA_CustomerMessagesDT : IDA_CustomerMessagesDT
    {
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IGenericDAO<DA_CustomerMessagesDTO> daMessagesDAO;
        IGenericDAO<DA_MainMessagesDTO> maindamessagesDAO;
        IGenericDAO<DA_MessagesDTO> messagesDAO;
        IGenericDAO<DA_MessagesDetailsDTO> messagesdetailsDAO;
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;

        public DA_CustomerMessagesDT(IUsersToDatabasesXML usersToDatabases, IGenericDAO<DA_CustomerMessagesDTO> daMessagesDAO, IGenericDAO<DA_MainMessagesDTO> maindamessagesDAO, IGenericDAO<DA_MessagesDTO> messagesDAO, IGenericDAO<DA_MessagesDetailsDTO> messagesdetailsDAO)
        {
            this.daMessagesDAO = daMessagesDAO;
            this.usersToDatabases = usersToDatabases;
            this.maindamessagesDAO = maindamessagesDAO;
            this.messagesDAO = messagesDAO;
            this.messagesdetailsDAO = messagesdetailsDAO;
        }

        public List<DA_MainMessagesModel> GetAll(DBInfoModel dbInfo)
        {

            List<DA_MainMessagesModel> MainMessagesModelList = new List<DA_MainMessagesModel>();
            List<DA_MessagesModel> messagesModelList = new List<DA_MessagesModel>();
            List<DA_MessagesDetailsModel> messagesDetailModel = new List<DA_MessagesDetailsModel>();

            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

            string sqlHeader = "SELECT * FROM DA_MainMessages  as cd WHERE cd.IsDeleted = 0";
            string sqlData = "SELECT * FROM DA_Messages  as ms WHERE ms.IsDeleted = 0";
            string sqlDetails = "SELECT * FROM DA_MessagesDetails  as md WHERE md.IsDeleted = 0";


            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    messagesModelList = db.Query<DA_MessagesModel>(sqlData).ToList();
                    messagesDetailModel = db.Query<DA_MessagesDetailsModel>(sqlDetails).ToList();
                    MainMessagesModelList = db.Query<DA_MainMessagesModel>(sqlHeader).ToList();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DAMESSAGES);
                }
            }

            foreach (DA_MainMessagesModel header in MainMessagesModelList)
            {
                header.DA_MessagesModel = new List<DA_MessagesModel>();
                foreach (DA_MessagesModel message in messagesModelList)
                {
                    message.DA_MessagesDetailsModel = new List<DA_MessagesDetailsModel>();
                    if (header.Id == message.MainDAMessagesID)
                    {
                        header.DA_MessagesModel.Add(message);
                    }
                    foreach (DA_MessagesDetailsModel detail in messagesDetailModel)
                    {
                        if (detail.HeaderId == message.Id)
                        {
                            message.DA_MessagesDetailsModel.Add(detail);
                        }
                    }
                }

            }
            return MainMessagesModelList;
        }

        public List<DA_MessagesModel> Get_DA_MessageById(DBInfoModel DBInfo, long MessageId, long HeaderDetailId)
        {
            List<DA_MessagesModel> messagesModelList = new List<DA_MessagesModel>();
            List<DA_MessagesDetailsModel> messagesDetailModel = new List<DA_MessagesDetailsModel>();
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            string sqlData1 = @"select * from DA_Messages where Id=@MessageId";
            string sqlData2 = @"select * from DA_MessagesDetails where Id=@HeaderDetailId";


            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    messagesModelList = db.Query<DA_MessagesModel>(sqlData1).ToList();
                    messagesDetailModel = db.Query<DA_MessagesDetailsModel>(sqlData2).ToList();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DAMESSAGES);
                }
            }

            foreach (DA_MessagesModel model in messagesModelList)
            {
                model.DA_MessagesDetailsModel = new List<DA_MessagesDetailsModel>();
            }
            foreach (DA_MessagesModel message in messagesModelList)
            {
                foreach (DA_MessagesDetailsModel detail in messagesDetailModel)
                {
                    if (detail.HeaderId == message.Id)
                    {
                        message.DA_MessagesDetailsModel.Add(detail);

                    }

                }
            }

            return messagesModelList;

        }
        public List<DA_MessagesModel> GetMessageByMainMessageId(DBInfoModel DBInfo, long id)
        {
            List<DA_MessagesModel> messagesModelList = new List<DA_MessagesModel>();
            foreach (DA_MessagesModel msg in messagesModelList)
                msg.DA_MessagesDetailsModel = new List<DA_MessagesDetailsModel>();


            string sqlData = "SELECT * FROM DA_Messages  as ms WHERE ms.IsDeleted = 0 and ms.MainDAMessagesId=@MainDAMessagesId";
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    messagesModelList = db.Query<DA_MessagesModel>(sqlData, new { MainDAMessagesId = id }).ToList();
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DAMESSAGES);
                }
            }
            return messagesModelList;
        }
        public List<DA_MessagesDetailsModel> GetMessageDetailsByMainMessageId(DBInfoModel DBInfo, long id)
        {
            List<DA_MessagesDetailsModel> messagedetailslist = new List<DA_MessagesDetailsModel>();

            string sqlData = "SELECT * FROM DA_MessagesDetails  as ms WHERE ms.IsDeleted = 0 and ms.HeaderId=@HeaderId";
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    messagedetailslist = db.Query<DA_MessagesDetailsModel>(sqlData, new { HeaderId = id }).ToList();
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DAMESSAGES);
                }
            }
            return messagedetailslist;
        }

        public DA_MainMessagesModel GetMainMessageById(DBInfoModel DBInfo, long Id)
        {
            DA_MainMessagesDTO mainMessage;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DA_MainMessages WHERE Id = @id";
                mainMessage = maindamessagesDAO.SelectFirst(sql, new { id = Id }, db);
            }
            return AutoMapper.Mapper.Map<DA_MainMessagesModel>(mainMessage);
        }
        public DA_MessagesModel GetMessageById(DBInfoModel DBInfo, long Id)
        {
            DA_MessagesDTO message;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DA_Messages WHERE Id = @id";
                message = messagesDAO.SelectFirst(sql, new { id = Id }, db);
            }
            return AutoMapper.Mapper.Map<DA_MessagesModel>(message);
        }
        public DA_MessagesDetailsModel GetMessageDetailById(DBInfoModel DBInfo, long Id)
        {
            DA_MessagesDetailsDTO messageDetail;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DA_MessagesDetails WHERE Id = @id";
                messageDetail = messagesdetailsDAO.SelectFirst(sql, new { id = Id }, db);
            }
            return AutoMapper.Mapper.Map<DA_MessagesDetailsModel>(messageDetail);
        }

        public List<DA_MessagesModel> GetOnCreateMessages(DBInfoModel DBInfo)
        {
            List<DA_MessagesDTO> messages;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DA_Messages WHERE OnOrderCreate = 1";
                messages = messagesDAO.Select(sql, null, db);
            }
            return AutoMapper.Mapper.Map<List<DA_MessagesModel>>(messages);
        }
        public List<DA_MessagesDetailsModel> GetOnCreateMessageDetails(DBInfoModel DBInfo)
        {
            List<DA_MessagesDetailsDTO> messageDetails;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DA_MessagesDetails WHERE OnOrderCreate = 1";
                messageDetails = messagesdetailsDAO.Select(sql, null, db);
            }
            return AutoMapper.Mapper.Map<List<DA_MessagesDetailsModel>>(messageDetails);
        }
        public List<DA_MessagesModel> GetOnUpdateMessages(DBInfoModel DBInfo)
        {
            List<DA_MessagesDTO> messages;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DA_Messages WHERE OnOrderUpdate = 1";
                messages = messagesDAO.Select(sql, null, db);
            }
            return AutoMapper.Mapper.Map<List<DA_MessagesModel>>(messages);
        }
        public List<DA_MessagesDetailsModel> GetOnUpdateMessageDetails(DBInfoModel DBInfo)
        {
            List<DA_MessagesDetailsDTO> messageDetails;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DA_MessagesDetails WHERE OnOrderUpdate= 1";
                messageDetails = messagesdetailsDAO.Select(sql, null, db);
            }
            return AutoMapper.Mapper.Map<List<DA_MessagesDetailsModel>>(messageDetails);
        }
        public List<DA_MessagesModel> GetOnCallMessages(DBInfoModel DBInfo)
        {
            List<DA_MessagesDTO> messages;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DA_Messages WHERE OnNewCall = 1";
                messages = messagesDAO.Select(sql, null, db);
            }
            return AutoMapper.Mapper.Map<List<DA_MessagesModel>>(messages);
        }
        public List<DA_MessagesDetailsModel> GetOnCallMessageDetails(DBInfoModel DBInfo)
        {
            List<DA_MessagesDetailsDTO> messageDetails;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DA_MessagesDetails WHERE OnNewCall = 1";
                messageDetails = messagesdetailsDAO.Select(sql, null, db);
            }
            return AutoMapper.Mapper.Map<List<DA_MessagesDetailsModel>>(messageDetails);
        }

        public List<MessagesLookup> GetMessagesLookups(DBInfoModel DBInfo, long MainDAMessagesID)
        {
            List<MessagesLookup> messageslookup = new List<MessagesLookup>();

            string sqlData = "SELECT Id,Description FROM DA_MainMessages  as ms WHERE ms.IsDeleted = 0 and ms.Id=@MainDAMessagesID";
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    messageslookup = db.Query<MessagesLookup>(sqlData, new { MainDAMessagesID = MainDAMessagesID }).ToList();
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DAMESSAGES);
                }
            }
            return messageslookup;
        }
        public List<MessagesDetailLookup> GetMessagesDetailsLookups(DBInfoModel DBInfo, long HeaderId)
        {
            List<MessagesDetailLookup> messagedetaillookup = new List<MessagesDetailLookup>();

            string sqlData = "SELECT Id,Description FROM DA_Messages  as ms WHERE ms.IsDeleted = 0 and ms.Id=@HeaderId";
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    messagedetaillookup = db.Query<MessagesDetailLookup>(sqlData, new { HeaderId = HeaderId }).ToList();
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DAMESSAGES);
                }
            }
            return messagedetaillookup;
        }

        public List<DA_CustomerMessagesModelExt> GetAll_DA_CustomerMessages(DBInfoModel DBInfo, long CustId)
        {
            List<DA_CustomerMessagesModelExt> dacustomersModelList = new List<DA_CustomerMessagesModelExt>();

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);


            string sqlData = @"  select DA_CustomerMessages.Id as Id,DA_CustomerMessages.CustomerId,CreationDate,MessageId,MessageDetailsId,MainDAMessageId,DA_CustomerMessages.StaffId,OrderId,DA_CustomerMessages.StoreId,MessageText, IsTemporary, Staff.FirstName as StaffName,Staff.LastName as StaffLastname,
                                                        DA_Orders.OrderNo, DA_Orders.StoreOrderNo,
                                                        DA_MainMessages.Description as MainMessageDesc,
                                                        DA_Messages.Description as MessageDesc,
                                                        DA_MessagesDetails.Description as MessageDetailsDesc
                                               from DA_CustomerMessages
                                                        left outer join DA_Orders on DA_Orders.Id = DA_CustomerMessages.OrderId
                                                        left outer join Staff on Staff.Id=DA_CustomerMessages.StaffId
                                                        left outer join DA_MessagesDetails on DA_MessagesDetails.Id=DA_CustomerMessages.MessageDetailsId
                                                        left outer join DA_Messages on DA_Messages.Id=DA_CustomerMessages.MessageId
                                                        left outer join DA_MainMessages on DA_MainMessages.Id=DA_CustomerMessages.MainDAMessageId
                                           where DA_CustomerMessages.CustomerId=@CustomerId
                                           order by CreationDate desc ";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    dacustomersModelList = db.Query<DA_CustomerMessagesModelExt>(sqlData, new { CustomerId = CustId }).ToList();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DACUSTOMERMESSAGES);
                }
            }

            return dacustomersModelList;
        }

        public List<DA_CustomerMessagesModelExt> GetAllCustomerMessages(DBInfoModel DBInfo)
        {
            List<DA_CustomerMessagesModelExt> allCustomerMessages;
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            string sqlData = @"select DA_CustomerMessages.Id as Id, DA_CustomerMessages.CustomerId, CreationDate, MessageId, MessageDetailsId, MainDAMessageId, DA_CustomerMessages.StaffId, OrderId, DA_CustomerMessages.StoreId, MessageText, IsTemporary,
                                        DA_Customers.FirstName as CustomerFirstName, DA_Customers.LastName as CustomerLastName, DA_Customers.Phone1 as CustomerPhone1, DA_Customers.Phone2 as CustomerPhone2, DA_Customers.Mobile as CustomerMobile,
                                        DA_Orders.OrderNo, DA_Orders.StoreOrderNo,
                                        Staff.FirstName as StaffName, Staff.LastName as StaffLastname,
                                        DA_MainMessages.Description as MainMessageDesc,
                                        DA_Messages.Description as MessageDesc,
                                        DA_MessagesDetails.Description as MessageDetailsDesc
                                    from DA_CustomerMessages
                                    left outer join DA_Customers on DA_Customers.Id = DA_CustomerMessages.CustomerId
                                    left outer join DA_Orders on DA_Orders.Id = DA_CustomerMessages.OrderId
                                    left outer join Staff on Staff.Id = DA_CustomerMessages.StaffId
                                    left outer join DA_MessagesDetails on DA_MessagesDetails.Id = DA_CustomerMessages.MessageDetailsId
                                    left outer join DA_Messages on DA_Messages.Id = DA_CustomerMessages.MessageId
                                    left outer join DA_MainMessages on DA_MainMessages.Id = DA_CustomerMessages.MainDAMessageId";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    allCustomerMessages = db.Query<DA_CustomerMessagesModelExt>(sqlData).ToList();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    throw new Exception(Symposium.Resources.Errors.DACUSTOMERMESSAGES);
                }
            }
            return allCustomerMessages;
        }

        public long InsertMainMessage(DBInfoModel DBInfo, DA_MainMessagesModel Model)
        {

            long res = 0;
            //configHlp.CheckDeliveryAgent();       
            DA_MainMessagesDTO dto = AutoMapper.Mapper.Map<DA_MainMessagesDTO>(Model);
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = maindamessagesDAO.Insert(db, dto);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DACUSTMESSAGEINSERTION);
                }
            }
            return res;
        }
        public long InsertMessage(DBInfoModel DBInfo, DA_MessagesModel Model)
        {
            long res = 0;


            DA_MessagesDTO dto = AutoMapper.Mapper.Map<DA_MessagesDTO>(Model);

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = messagesDAO.Insert(db, dto);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DACUSTMESSAGEINSERTION);
                }
            }
            return res;
        }
        public long InsertMessageDetail(DBInfoModel DBInfo, DA_MessagesDetailsModel Model)
        {
            long res = 0;


            DA_MessagesDetailsDTO dto = AutoMapper.Mapper.Map<DA_MessagesDetailsDTO>(Model);

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = messagesdetailsDAO.Insert(db, dto);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DACUSTMESSAGEINSERTION);
                }
            }
            return res;

        }
        public long InsertDaCustomerMessage(DBInfoModel DBInfo, DA_CustomerMessagesModel Model)
        {
            long res = 0;
            //configHlp.CheckDeliveryAgent();       
            DA_CustomerMessagesDTO dto = AutoMapper.Mapper.Map<DA_CustomerMessagesDTO>(Model);
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = daMessagesDAO.Insert(db, dto);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DACUSTMESSAGEINSERTION);
                }
            }
            return res;
        }

        public long UpdateDaCustomerMessage(DBInfoModel DBInfo, DA_CustomerMessagesModel Model)
        {
            long res = Model.Id;
            DA_CustomerMessagesDTO dto = AutoMapper.Mapper.Map<DA_CustomerMessagesDTO>(Model);
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    int rowsAffected = daMessagesDAO.Update(db, dto);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DACUSTMESSAGEUPDATE);
                }
            }
            return res;
        }

        public long UpdateMainMessage(DBInfoModel DBInfo, DA_MainMessagesModel Model)
        {
            long res = 0;

            DA_MainMessagesDTO dto = AutoMapper.Mapper.Map<DA_MainMessagesDTO>(Model);
            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = maindamessagesDAO.Update(db, dto);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DACUSTMESSAGEINSERTION);
                }
            }
            return res;
        }
        public long UpdateMessage(DBInfoModel DBInfo, DA_MessagesModel Model)
        {
            long res = 0;


            DA_MessagesDTO dto = AutoMapper.Mapper.Map<DA_MessagesDTO>(Model);

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = messagesDAO.Update(db, dto);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DACUSTMESSAGEINSERTION);
                }
            }
            return res;
        }
        public long UpdateMessageDetail(DBInfoModel DBInfo, DA_MessagesDetailsModel Model)
        {
            long res = 0;


            DA_MessagesDetailsDTO dto = AutoMapper.Mapper.Map<DA_MessagesDetailsDTO>(Model);

            connectionString = usersToDatabases.ConfigureConnectionString(DBInfo);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    res = messagesdetailsDAO.Update(db, dto);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw new Exception(Symposium.Resources.Errors.DACUSTMESSAGEINSERTION);
                }
            }
            return res;
        }

        public long DeleteMainMessage(DBInfoModel dbInfo, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

            string updateMainMessage = "UPDATE DA_MainMessages SET IsDeleted = 1 WHERE Id =@ID";
            string updateMessageHeader = "UPDATE DA_Messages SET IsDeleted = 1 WHERE MainDAMessagesID =@ID";
            string updateMessageDetails = "UPDATE DA_MessagesDetails SET IsDeleted = 1 WHERE HeaderId =@ID";


            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        db.Execute(updateMainMessage, new { ID = Id });
                        db.Execute(updateMessageHeader, new { ID = Id });
                        db.Execute(updateMessageDetails, new { ID = Id });
                    }
                    catch (Exception e)
                    {

                        logger.Error(e.ToString());
                        throw new Exception(Symposium.Resources.Errors.DACUSTOMERMESSAGES);
                    }
                    scope.Complete();
                }
            }
            res = Id;
            return res;
        }
        public long DeleteMessage(DBInfoModel dbInfo, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);

            string updateMessageHeader = "UPDATE DA_Messages SET IsDeleted = 1 WHERE Id =@ID";
            string updateMessageDetails = "UPDATE  DA_MessagesDetails WHERE HeaderId =@headerID";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        db.Execute(updateMessageHeader, new { ID = Id });
                        db.Execute(updateMessageDetails, new { headerID = Id });
                    }
                    catch (Exception ex)
                    {

                        logger.Error(ex.ToString());

                    }
                    // Commit transaction
                    scope.Complete();
                }
            }
            res = Id;
            return res;
        }
        public long DeleteMessageDetail(DBInfoModel dbInfo, long Id)
        {
            long res = 0;
            connectionString = usersToDatabases.ConfigureConnectionString(dbInfo);
            string deleteMessageDetails = "DELETE FROM DA_MessagesDetails WHERE Id =@ID";
            string updatemessagedetails = "UPDATE DA_MessagesDetails SET IsDeleted = 1 WHERE Id =@ID";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    db.Execute(deleteMessageDetails, new { ID = Id });
                }
                catch (Exception ex)
                {
                    db.Execute(updatemessagedetails, new { ID = Id });
                    logger.Error(ex.ToString());
                    // throw new Exception(Symposium.Resources.Errors.FAILEDDAMESSAGESDELETE);
                }

            }
            return res;

        }

    }
}
