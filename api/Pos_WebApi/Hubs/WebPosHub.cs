using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Transports;
using Pos_WebApi.Helpers;
using Symposium.Helpers;
using Symposium.Helpers.Hubs;
using Symposium.Models.Models;
using Symposium.Models.Models.Delivery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Delivery;
using Symposium.WebApi.MainLogic.Flows.Delivery;
using Symposium.Helpers.Interfaces;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Delivery;
using Newtonsoft.Json;
using Symposium.Models.Enums;

namespace Pos_WebApi.Hubs
{
    


    [HubName("webPosHub")]
    public class WebPosHub : Hub
    {
        //     private readonly static ConnectionMapping<string> _connections =
        //new ConnectionMapping<string>();
        public readonly static GroupedConnectionMapping _connections = new GroupedConnectionMapping();// System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\signalRConnections.xml");

        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IDeliveryRoutingTasks genDeliveryRoutingTasks;

        IDeliveryRoutingFlows genDeliveryRoutingFlows;

        IStoreIdsPropertiesHelper AllStores;

        DeliveryRoutingHubParticipants drHubParticipants;

        public WebPosHub(
            IDeliveryRoutingTasks genDeliveryRoutingTasks, 
            IDeliveryRoutingFlows genDeliveryRoutingFlows, 
            IStoreIdsPropertiesHelper AllStores,
            DeliveryRoutingHubParticipants drHubParticipants)
        {
            this.genDeliveryRoutingTasks = genDeliveryRoutingTasks;
            this.genDeliveryRoutingFlows = genDeliveryRoutingFlows;
            this.AllStores = AllStores;
            this.drHubParticipants = drHubParticipants;
        }

        public WebPosHub()
        {
            var config = System.Web.Http.GlobalConfiguration.Configuration;
            System.Web.Http.Dependencies.IDependencyResolver autofac;
            autofac = config.DependencyResolver;

            this.genDeliveryRoutingTasks = (IDeliveryRoutingTasks)autofac.GetService(typeof(IDeliveryRoutingTasks));
            this.genDeliveryRoutingFlows = (IDeliveryRoutingFlows)autofac.GetService(typeof(IDeliveryRoutingFlows));
            this.AllStores = (IStoreIdsPropertiesHelper)autofac.GetService(typeof(IStoreIdsPropertiesHelper));
            this.drHubParticipants = (DeliveryRoutingHubParticipants)autofac.GetService(typeof(DeliveryRoutingHubParticipants));
        }

        public async override Task OnConnected()
        {
            var name = Context.QueryString["name"];
            if (name == null)
            {
                name = Context.Headers["name"];

                if (name == null)
                    name = "Console";
            }
            if (_connections.IsUserConnected(name))
            {
                //TODO: add Bussiness Logic to avoid 2 registrations

            }
            var ret = _connections.Add(name, Context.ConnectionId);
            PlainMessage("User : " + name + " with Id: " + Context.ConnectionId + " is Connected.");
            //    ConnectedUsers(ret.GroupId);
            //   return Clients.All.connectedUsers(_connections.GetAllGroupedConnections(ret.GroupId));
            await base.OnConnected();
         if (ret.GroupId!=null)   await JoinGroup(ret.GroupId, ret.User);

            logger.Info("SignalR users: " + _connections.Count());
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionToDisconect = _connections.GetConnectionById(Context.ConnectionId);
            if (connectionToDisconect != null)
            {
                PlainMessage("User : " + connectionToDisconect.UserName + " with Id: " + Context.ConnectionId + " Disonnected");

                drHubParticipants.RemoveClient(Context.ConnectionId);
            }
            else
                PlainMessage("User not in _connections list with Id: " + Context.ConnectionId + " is Disonnected");

            //var name = Context.QueryString["name"];
            //if (name == null)
            //{
            //    name = Context.Headers["name"];
            //    if (name == null)
            //        name = "Console";
            //}
            if (stopCalled)
            {
                PlainMessage(String.Format("Client {0} : {1} explicitly closed the connection.", Context.ConnectionId, connectionToDisconect != null ? connectionToDisconect.UserName : ""));
            }
            else
            {
                PlainMessage(String.Format("Client {0} : {1} timed out .", Context.ConnectionId, connectionToDisconect != null ? connectionToDisconect.UserName : ""));
            }
            try
            {
                if (connectionToDisconect != null)
                {
                    _connections.Remove(connectionToDisconect.UserName, Context.ConnectionId);
                    ConnectedUsers(connectionToDisconect.GroupId);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                PlainMessage(ex.InnerException != null ? ex.InnerException.Message : ex.Message, warn: true);
                Console.WriteLine(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                Console.WriteLine(String.Format("Client {0} explicitly closed the connection.", Context.ConnectionId));
                //  Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("SignalR: OnDisconnected : '" + ex.InnerException != null ? ex.InnerException.Message : ex.Message + " '  timed out ."));
            }
            var res = base.OnDisconnected(stopCalled);
            logger.Info("SignalR users: " + _connections.Count());
            return base.OnDisconnected(stopCalled); 
        }
        public override Task OnReconnected()
        {
            var user = _connections.GetConnectionById(Context.ConnectionId);
            var name = Context.QueryString["name"];
            if (name == null)
            {
                name = Context.Headers["name"];

                if (name == null)
                    name = "Console";
            }
            if (user != null)
            {
                PlainMessage(user.UserName + " trying to reconnect...");
                ConnectedUsers(user.GroupId);
            }
            else
            {
                PlainMessage(Context.ConnectionId + " trying to reconnect...");
            }
            var ret = _connections.Add(name, Context.ConnectionId);
            return base.OnReconnected();
        }

        public void NewWebPosHubMessage(string name, string message)
        {
           
            Clients.All.addNewMessageToPage(name, message);
        }

        public void NewChatMessage(string message)
        {
            Clients.All.hubMessage(message);
        }


        public void ResponseNewReceiptNos(string clientName, string posName, string res)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).responseNewReceiptNos(clientName, posName, res);
                PlainMessage("ResponseNewReceiptNos responds with " + res + " to " + clientName);
            }
            else
                PlainMessage("ResponseNewReceiptNos  : Failed To Sent to " + clientName, warn: true);
        }




        #region "NFC Driver Messages"

        /// <summary>
        /// Message from Pos to the NFC reader/writer (joined the group) to write a new card
        /// </summary>
        /// <param name="customer">NfcCustomerModel: Model the keeps Data read/write from/to NFC card </param>
        /// <param name="receiver"> receiver of message </param>
        /// <param name="sender"> sender of message </param>
        public void WriteNfcCard(NfcCustomerModel customer, string receiver, string sender)
        {
            PlainMessage("Write Card for group " + receiver + ". RoomNo: "+ customer.RoomNo);
            var clientToSent = _connections.GetUser(receiver);
            string connectionid = clientToSent.Connections.LastOrDefault();
            Clients.Client(connectionid).writeNfcCard(customer, sender);
        }

        /// <summary>
        /// Message from Pos to the NFC reader/writer (joined the group) to read from the card
        /// </summary>
        /// <param name="receiver"> receiver of message </param>
        /// <param name="sender"> sender of message </param>
        public void ReadNfcCard(string receiver, string sender)
        {
            PlainMessage("Read Card for receiver " + receiver);
            var clientToSent = _connections.GetUser(receiver);
            string connectionid = clientToSent.Connections.LastOrDefault();
            Clients.Client(connectionid).readNfcCard(sender, true);
        }

        /// <summary>
        /// Message from  NFC reader/writer to Pos (joined the group)
        /// </summary>
        /// <param name="customer">NfcCustomerModel: Model the keeps Data read/write from/to NFC card </param>
        /// <param name="receiver"> receiver of message </param>
        /// <param name="isRead"> distinction between call from read card or write card </param>
        public void ReturnNfcCard(NfcCustomerModel customer, string receiver, bool isRead)
        {
            PlainMessage("Return Card for group " + receiver);
            var clientToSent = _connections.GetUser(receiver);
            string connectionid = clientToSent.Connections.LastOrDefault();
            Clients.Client(connectionid).returnNfcCard(customer, isRead);
        }

        #endregion

        #region extecrMessages
        public void ExtecrError(string extecrName, string errorMsg)
        {
           
            var clientToSent = _connections.GetUserGroup(extecrName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).extecrError(extecrName, errorMsg);
                PlainMessage("ExtecrError :" + errorMsg + " to " + extecrName, warn: true);
                logger.Error("ExtecrError :" + errorMsg + " to " + extecrName);
            }
            else
            {
                PlainMessage("ExtecrError  : Failed To Sent to " + extecrName, warn: true);
                logger.Error("ExtecrError  : Failed To Sent to " + extecrName);
            }
                
        }

        public void CreditCardAmount(string clientName, string posName, decimal amount)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).creditCardAmount(clientName, posName, amount);
                PlainMessage("CreditCardAmount " + amount.ToString() + " to " + clientName);
            }
            else
                PlainMessage("CreditCardAmount  : Failed To Sent to " + clientName, warn: true);
        }
        public void Drawer(string clientName, string posName)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).drawer(clientName, posName);
                PlainMessage("Drawer to " + clientName);
            }
            else
                PlainMessage("Drawer  : Failed To Sent to " + clientName, warn: true);
        }

        public void Image(string clientName, string posName, string message)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).image(clientName, posName, message);
                PlainMessage("Image " + message + " to " + clientName);
            }
            else
                PlainMessage("Image  : Failed To Sent to " + clientName);
        }
        public void Kitchen(string clientName, string posName, string message)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).kitchen(clientName, posName, message);
                PlainMessage("Kitchen " + message + " to " + clientName);
            }
            else
                PlainMessage("Image  : Failed To Sent to " + clientName, warn: true);
        }
        public void KitchenInstruction(string clientName, string posName, string message)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).kitchenInstruction(clientName, posName, message);
                PlainMessage("Kitchen Instruction " + message + " to " + clientName);

            }
            else
                PlainMessage("Kitchen Instruction  : Failed To Sent to " + clientName, warn: true);
        }

        public void KitchenInstructionLogger(string clientName, string posName, long kitchenInstructionLoggerId, string ki)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).kitchenInstruction(clientName, posName, kitchenInstructionLoggerId, ki);
                PlainMessage("Kitchen Instruction " + kitchenInstructionLoggerId  + " to " + clientName);

            }
            else
                PlainMessage("Kitchen Instruction  : Failed To Sent to " + clientName, warn: true);
        }

        public void StartWeighting(string clientName, string posName)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).startWeighting(clientName, posName);
                PlainMessage("Start Weighting to " + clientName);
            }
            else
                PlainMessage("StartWeighting  : Failed To Sent to " + clientName, warn: true);
        }
        public void StopWeighting(string clientName, string posName)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).StopWeighting(clientName, posName);
                PlainMessage("Stop Weighting to " + clientName);
            }
            else
                PlainMessage("Stop Weighting  : Failed To Sent to " + clientName, warn: true);
        }

        public void ItemWeighted(string clientName, double weight)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!string.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).itemWeighted(clientName, weight);
                PlainMessage("ItemW Weighting from  " + clientName + " weight : " + weight.ToString());
            }
            else
            {
                PlainMessage("ItemW Weighting from  " + clientName + " weight : " + weight.ToString() + " No client found in this Group", warn: true);
            }
        }

        public void LoginWithMessage(string clientName, string userName, string password)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!string.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).loginWithMessage(clientName, userName, password);
                PlainMessage("Login from service  for Pos :" + clientName + " User : " + userName);
            }
            else
            {
                PlainMessage("Login from service  for Pos :" + clientName + " User : " + userName + " No client found in this Group", warn: true);
            }
        }

        public void LcdMessage(string clientName, string message)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (!String.IsNullOrEmpty(clientToSent))
            {
                Clients.Group(clientToSent).lcdMessage(clientName, message);
                PlainMessage("LcdMessage " + message + " to " + clientName);
            }
            else
                PlainMessage("LcdMessage : Failed To Sent" + message + " to " + clientName, warn:true);
        }

        public void NewCallReceived(string groupId, string phoneNumber)
        {
            Clients.Group(groupId).newCallReceived(phoneNumber);
            PlainMessage("NewCallReceived with number " + phoneNumber + " sent to Group" + groupId);
        }
        #endregion
        public void RequestNewReceiptNos(string clientName, string posName, string res)
        {
            var clientToSent = _connections.GetConnections(clientName);
            if (clientToSent != null)
                Clients.Client(clientToSent).requestNewReceiptNos(clientName, posName, res);
            PlainMessage("RequestNewReceiptNos requested by " + clientName + "from " + posName);
            //TODO: Generate a Global Error Handler
        }

        public void PrintItem (string extecrName, string partialReceiptJson, PrintType printType = PrintType.PrintWhole, string additionalInfo = null, bool tempPrint = false)
        {
            var clientToSent = _connections.GetUserGroup(extecrName);
            Clients.Group(clientToSent).PrintItem(extecrName, partialReceiptJson, printType, additionalInfo, tempPrint);
            PlainMessage("Partial item print " + partialReceiptJson + " sent to " + extecrName);
        }

        public void Drawer (string extecrName)
        {
            var clientToSent = _connections.GetUserGroup(extecrName);
            Clients.Group(clientToSent).Drawer(extecrName);
            PlainMessage("Drawer message sent to " + extecrName);
        }

        /// <summary>
        /// Get All connected uses joined a specific group. (Typically the GroupId is the StoreId)
        /// </summary>
        /// <param name="groupId">Typically the GroupId is the StoreId, but other groups may exist.</param>
        public void ConnectedUsers(string groupId)
        {
            if (groupId != null)
            {
                var str = _connections.GetAllGroupedConnections(groupId);
                Clients.Group(groupId).connectedUsers(str);
            }
        }

        public void PlainMessage(string msg,bool log=true, bool warn=false)
        {
            _connections.MarkAsLatestActivity(Context.ConnectionId);
           if (!warn && log)
                logger.Info(msg);
           else if (warn && log)
                logger.Warn(msg);
            Clients.All.plainMessage(msg);
        }

        /// <summary>
        /// broadcast the new receipt to group clients
        /// </summary>
        /// <param name="extecrName">Format: StoreId|FiscalName </param>
        /// <param name="invoiceId">invoiceId</param>
        /// <param name="blnPrintFiscalSign"></param>
        /// <param name="blnDoPrintInKitchen"></param>
        /// <param name="printType">for whole or partial print</param>
        /// <param name="additionalInfo">the method name for the item's second print line</param>
        /// <param name="tempPrint">if true then print without ADHME </param>
        public void NewReceipt(string extecrName, string receiptId, bool useFiscalSignature = true, bool printKithcen = true, PrintType printType = PrintType.PrintWhole, string additionalInfo = null, bool tempPrint = false)
        {
            var clientToSent = _connections.GetUserGroup(extecrName);
            if (clientToSent == null)
            {
                logger.Error("        Client '" + extecrName + "' NOT found in SignalRAfterInvoiceModel clients..." + Environment.NewLine);
                return;
            }
            Clients.Group(clientToSent).newReceipt(extecrName, receiptId, useFiscalSignature, printKithcen, printType, additionalInfo, tempPrint);
            PlainMessage("NewReceipt " + receiptId + " sent to " + extecrName + " Use Sig: " + useFiscalSignature + " reprint : " + printKithcen + " print type : " + printType + " additional info : " + additionalInfo + " temp print : " + tempPrint);
        }

        /// <summary>
        /// broadcast new receipt id to group clients (only pda handles that request)
        /// </summary>
        /// <param name="extecrName"></param>
        /// <param name="recId"></param>
        public void receiptId(string clientName, string recId)
        {
            var clientToSent = _connections.GetUserGroup(clientName);
            if (clientToSent == null)
            {
                logger.Error("        Client '" + clientName + "' NOT found in SignalRAfterInvoiceModel clients..." + Environment.NewLine);
                return;
            }
            Clients.Client(clientToSent).receiptId(recId);
        }

        public void OpenOrders(string msg)
        {
            Clients.All.OpenOrders(msg);
            PlainMessage("Open Orders Updated ");
        }


        /// <summary>
        /// Send the new receipt to Exctecr
        /// </summary>
        /// <param name="extecrName"></param>
        /// <param name="receiptId"></param>
        /// <param name="useFiscalSignature"></param>
        /// <param name="printKithcen"></param>
        /// <param name="printType"></param>
        /// <param name="additionalInfo"></param>
        /// <param name="tempPrint"></param>
        public void NewReceiptExtecr(string extecrName, string receiptId, bool useFiscalSignature = true, bool printKithcen = true, PrintType printType = PrintType.PrintWhole, string additionalInfo = null, bool tempPrint = false)
        {
            // var clientToSent = _connections.GetUserGroup(extecrName);
           ConnectionModel user= _connections.GetUser(extecrName);
            if (user == null)
            {
                logger.Error("        Client '" + extecrName + "' NOT found in SignalRAfterInvoiceModel clients..." + Environment.NewLine);
                return;
            }
            Clients.Client(user.ConnectionId).newReceipt(extecrName, receiptId, useFiscalSignature, printKithcen, printType, additionalInfo, tempPrint);
            PlainMessage("NewReceiptExtecr " + receiptId + " sent to " + extecrName + " Use Sig: " + useFiscalSignature + " reprint : " + printKithcen + " print type : " + printType + " additional info : " + additionalInfo + " temp print : " + tempPrint);
        }


        /// <summary>
        /// Send the new receipt to ot others except the extecr
        /// </summary>
        /// <param name="extecrName"></param>
        /// <param name="receiptId"></param>
        /// <param name="useFiscalSignature"></param>
        /// <param name="printKithcen"></param>
        /// <param name="printType"></param>
        /// <param name="additionalInfo"></param>
        /// <param name="tempPrint"></param>
        public void NewReceiptNotExtecr(string extecrName, string receiptId, bool useFiscalSignature = true, bool printKithcen = true, PrintType printType = PrintType.PrintWhole, string additionalInfo = null, bool tempPrint = false)
        {
            // var clientToSent = _connections.GetUserGroup(extecrName);
            ConnectionModel user = _connections.GetUser(extecrName);
            if (user == null)
            {
                logger.Error("        Client '" + extecrName + "' NOT found in SignalRAfterInvoiceModel clients..."+Environment.NewLine);
                return;
            }
            Clients.AllExcept(new string[] { user.ConnectionId }).newReceipt(extecrName, receiptId, useFiscalSignature, printKithcen, printType, additionalInfo, tempPrint);
            PlainMessage("NewReceiptNotExtecr " + receiptId + " sent to " + extecrName + " Use Sig: " + useFiscalSignature + " reprint : " + printKithcen + " print type : " + printType + " additional info : " + additionalInfo + " temp print : " + tempPrint);
        }

        /// <summary>
        /// Return to all clients a message of a new invoice
        /// </summary>
        /// <param name="client">posInfoId</param>
        /// <param name="receiptId">receiptId</param>
        public void NewInvoice(string client, string receiptId)
        {
            Clients.All.NewInvoice(client,receiptId);
            PlainMessage("NewInvoice " + receiptId + " sent to All. Client: " + client + ", receiptId : " + receiptId );
        }

        public void RefreshPageSet(string groupId, string pagesetId)
        {
            Clients.Group(groupId).refreshPageSet(pagesetId);
            PlainMessage("RefreshPageSet " + pagesetId + " sent to " + groupId);
        }

        public void RefreshTable(string storeid, string tableId)
        {

            Clients.Group(storeid).refreshTable(storeid, tableId);
        }

        public void Report(string extecrName, string ZJson)
        {
            var clientToSent = _connections.GetUserGroup(extecrName);
            Clients.Group(clientToSent).report(extecrName, ZJson);
            PlainMessage("Report " + ZJson + " sent to " + extecrName);
        }

        public void XReport(string extecrName, string ZJson)
        {
            var clientToSent = _connections.GetUserGroup(extecrName);
            Clients.Group(clientToSent).xReport(extecrName, ZJson);
            PlainMessage("XReport " + ZJson + " sent to " + extecrName);
        }

        public void ZReport(string extecrName, string ZJson)
        {
            var clientToSent = _connections.GetUserGroup(extecrName);
            Clients.Group(clientToSent).zReport(extecrName, ZJson);
            PlainMessage("ZReport " + ZJson + " sent to " + extecrName);
        }

        public void ZReportResponse(string extecrName, string statusMessage)
        {
            var clientToSent = _connections.GetUserGroup(extecrName);
            if (clientToSent != null)
            {
                Clients.Group(clientToSent).zReportResponse(extecrName, statusMessage);
                PlainMessage("ZReport  response " + statusMessage + " sent to " + extecrName);
            }
            else
                PlainMessage("ZReport  response failed " + statusMessage + " sent to " + extecrName, warn: true);
        }

        public void Heartbeat()
        {
            _connections.RemoveInActive();


            Clients.All.heartbeat();
            PlainMessage("WebPosHub Sending heartbeat",false);

        }

        public void partialPrintConnectivity(string clientName, string posName, bool resetOrder)
        {
            var clientToSent = _connections.GetConnections(clientName);
            if (clientToSent != null)
                Clients.Client(clientToSent).partialPrintConnectivity(clientName, posName, resetOrder);
            PlainMessage("Connectivity requested by " + clientName + " from " + posName + " with resetOrder value: " + resetOrder);
        }

        public void LockDownUIForEOD(string storeId, string posInfoId)
        {
            Clients.Group(storeId).lockDownUIForEOD(storeId, posInfoId);
            PlainMessage("Starting EndOfDay for pos " + posInfoId + " sent to " + storeId);
        }
        public void UnLockDownUIFromEOD(string storeId, string posInfoId)
        {
            Clients.Group(storeId).unLockDownUIFromEOD(storeId, posInfoId);
            PlainMessage("Ending EndOfDay for pos " + posInfoId + "sent to " + storeId);
        }

        public void IamAlive()
        {
            try
            {
                _connections.MarkAsLatestActivity(Context.ConnectionId);
            } catch(Exception ex)
            {
                logger.Error(ex.ToString());
                PlainMessage("I am alive failed for " + Context.ConnectionId);
            }
        }

        public async Task JoinGroup(string groupName, string usr)
        {
            var user = _connections.GetConnectionById(Context.ConnectionId);
            var name = Context.QueryString["name"];
            if (name == null)
            {
                name = Context.Headers["name"];

                if (name == null)
                    name = "Console";
            }
            if (user != null)
            {
               
                ConnectedUsers(user.GroupId);
            }
           
            var ret = _connections.Add(name, Context.ConnectionId);
            await Groups.Add(Context.ConnectionId, groupName);
            PlainMessage(usr + " joined group :" + groupName);
            ConnectedUsers(groupName);
            // Clients.Group(groupName).addChatMessage(user + " joined.");
        }
        public Task LeaveGroup(string groupName)
        {
            PlainMessage(Context.ConnectionId + " Removed  from group : " + groupName);
            return Groups.Remove(Context.ConnectionId, groupName);
        }

        //Kds signal Messages
        public void kdsMessage(string storeId, string msg)
        {
            //var clientToSent = _connections.GetUserGroup(storeId);
            //Clients.Group(clientToSent).kdsMessage(storeId, msg);
            Clients.Group(storeId).kdsMessage(storeId, msg);
            PlainMessage("KDS_MESSAGE " + msg + "sent to " + storeId);
        }
        //Kds signal Messages To Dispatcher
        //public void kdsMessageToDispatcher(string storeId, long orderId)
        //{
        //    Clients.Group(storeId).kdsMessageToDispatcher(storeId, orderId);
        //    PlainMessage("KDS_MESSAGE_SEND_DISPATCHER " + orderId + "sent to " + storeId);
        //}
        //Dispatcher signal Messages To KDS
        //public void dispatcherMessageToKds(string storeId, long orderId)
        //{
        //    Clients.Group(storeId).dispatcherMessageToKds(storeId, orderId);
        //    PlainMessage("DISPATCHER_MESSAGE_SEND_KDS" + orderId + "sent to " + storeId);
        //}
        //Delivery signal Messages
        public void deliveryMessage(string storeId, string msg , List<long?> salesTypeIDs) {
            //var clientToSent = _connections.GetUserGroup(storeId);
            //Clients.Group(clientToSent).kdsMessage(storeId, msg);
            Clients.Group(storeId).deliveryMessage(storeId, msg, salesTypeIDs);
            PlainMessage("DELIVERY_MESSAGE " + msg + "sent to " + storeId);
        }
        public void deliveryCustomerSignalMsg(string storeId, string msg, DeliveryCustomerSignalModel entity)
        {
            //var clientToSent = _connections.GetUserGroup(storeId);
            //Clients.Group(clientToSent).kdsMessage(storeId, msg);
            Clients.Group(storeId).deliveryMessage(storeId, msg, entity);
            PlainMessage("DELIVERY_CUSTOMER_MESSAGE " + msg + "sent to " + storeId);
        }

        /// <summary>
        /// fired when new DA Order is downloaded to store from DA
        /// </summary>
        /// <param name="posname">format 'storeId|posname'</param>
        /// <param name="orderid">local order id</param>
        /// <param name="oldstatus">old status (for new order oldstatus=-100)</param>
        /// <param name="newstatus">new status</param>
        public void daUpdateOrder(string posname, long orderid, int oldstatus, int newstatus )
        {
            string storeid = posname;
            if(posname.Contains("|")) storeid = posname.Split('|')[0];

            Clients.Group(storeid).daUpdateOrdertoPos(orderid, oldstatus, newstatus);
        }

        #region DeliveryRouting

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PosId"></param>
        /// <returns></returns>
        public async Task JoinDeliveryPos(string PosId)
        {
            bool drAutoAssignRouting = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drAutoAssignRouting");

            if (drAutoAssignRouting)
            {
                _connections.Add(PosId, Context.ConnectionId);

                PlainMessage("User : " + PosId + " with Id: " + Context.ConnectionId + " is Connected.");

                await Groups.Add(Context.ConnectionId, "DeliveryRouting");
                await Groups.Add(Context.ConnectionId, "DeliveryPos");
   
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public async Task JoinDeliveryStaff(string staffId)
        {
            _connections.Add(staffId, Context.ConnectionId);

            PlainMessage("User: " + staffId + " with Id: " + Context.ConnectionId + " is Connected.");

            await JoinGroup("DeliveryRouting", staffId);

            await JoinGroup("DeliveryStaff", staffId);

            drHubParticipants.AddClient(Context.ConnectionId, Convert.ToInt64(staffId));

            DBInfoModel DBInfo = null;

            string storeIDRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "drDefaultGuid");

            Guid StoreId = new Guid(storeIDRaw);

            DBInfo = AllStores.GetStoreById(StoreId);

            List<DeliveryRoutingModel> pendingRoutes = genDeliveryRoutingTasks.getStaffRoutesByAssignStatus(DBInfo, (int)DeliveryRoutingAssignStatusEnum.pendingResponse, Convert.ToInt64(staffId));

            foreach (DeliveryRoutingModel route in pendingRoutes)
            {
                DeliveryRoutingExtModel model = new DeliveryRoutingExtModel();

                model.route = route;

                model.orderNos = genDeliveryRoutingFlows.getRouteOrderIds(DBInfo, route.Id).ConvertAll(x => Convert.ToString(x));

                Clients.Client(drHubParticipants.GetSessionId(Convert.ToInt64(route.StaffId))).DeliveryManAssign(route.StaffId, model);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeliveryRoutingModel"></param>
        public void DeliveryManAssignPos(DeliveryRoutingModel DeliveryRoutingModel)
        {
            Clients.Group("DeliveryPos").DeliveryManAssignPos(DeliveryRoutingModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootingId"></param>
        /// <param name="rootingStatus"></param>
        public void DeliveryManFailure(long rootingId, int rootingStatus)
        {
            Clients.Group("DeliveryPos").DeliveryManFailure(rootingId, rootingStatus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootingId"></param>
        /// <param name="rootingStatus"></param>
        public void DeliveryRoutingChange(long rootingId, int rootingStatus)
        {
            Clients.Group("DeliveryPos").DeliveryRoutingChange(rootingId, rootingStatus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootingId"></param>
        public void DeliveryRoutingDelete(long rootingId)
        {
            Clients.Group("DeliveryPos").DeliveryRoutingDelete(rootingId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootingId"></param>
        public void DeliveryManReject(long rootingId)
        {
            Clients.Group("DeliveryPos").DeliveryManReject(rootingId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootingId"></param>
        public void DeliveryManReturned(DeliveryRoutingModel data) //long rootingId)
        {
            Clients.Group("DeliveryPos").DeliveryManReturned(data); //rootingId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="staffId"></param>
        /// <param name="DeliveryRoutingExtModel"></param>
        public void DeliveryManAssign(string staffId, DeliveryRoutingExtModel DeliveryRoutingExtModel)
        {
            Clients.Client(drHubParticipants.GetSessionId(Convert.ToInt64(staffId))).DeliveryManAssign(staffId, DeliveryRoutingExtModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="staffId"></param>
        /// <param name="rootingId"></param>
        public void DeliveryRoutingDelete(string staffId, long rootingId)
        {
            Clients.Client(drHubParticipants.GetSessionId(Convert.ToInt64(staffId))).DeliveryRoutingDelete(staffId, rootingId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="staffId"></param>
        /// <param name="rootingId"></param>
        public void DeliveryManDismiss(string staffId, long rootingId)
        {
            Clients.Client(drHubParticipants.GetSessionId(Convert.ToInt64(staffId))).DeliveryManDismiss(staffId, rootingId);
        }

        #endregion DeliveryRouting
    }

    public class ConnectionModel
    {
        public ConnectionModel()
        {
            Connections = new HashSet<string>();
        }
        public ConnectionModel(string usernameToParse, string connectionId)
        {
            UserName = usernameToParse;
            var parsed = usernameToParse.Split('|');
            if (parsed.Count() > 1)
            {
                User = parsed[1];
                GroupId = parsed[0];
            }
            else
            {
                User = usernameToParse;
            }
            Connections = new HashSet<string>();
            Connections.Add(connectionId);
            ConnectionId = connectionId;
        }
        public string ConnectionId { get; set; }
        public string GroupId { get; set; }
        public string UserName { get; set; }
        public string User { get; set; }
        public DateTime LatestActivity { get; set; }
        public HashSet<string> Connections { get; set; }
    }

    public class GroupedConnectionMapping
    {
        public static HashSet<ConnectionModel> _connections = null;

     //   private string xmlFilePath;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly object obj = new object();

        public GroupedConnectionMapping()//(string xmlpath)
        {
            lock (this)
            {
                if (_connections == null)
                {
                    logger.Info(">>> INITIALIZING HUB CONNECTIONS repository (_connections)...");
                    _connections = new HashSet<ConnectionModel>();
                    logger.Info("");
                }
            }
       }

 
        public int Count()
        {
            if (_connections != null)
                return _connections.Count();
            else
                return 0;
        }
        public string GetConnections(string userName)
        {
            return _connections.Where(w => w.UserName == userName).SelectMany(sm => sm.Connections).LastOrDefault();
        }

        public string GetUserGroup(string userName)
        {
            var user = _connections.Where(w => w.UserName == userName).FirstOrDefault();
            if (user != null)
            {
                return user.GroupId;
            }
            else return "";
        }
        public ConnectionModel GetUser(string userName)
        {
            var user = _connections.Where(w => w.UserName == userName).FirstOrDefault();
            if (user != null)
            {
                return user;
            }
            else return null;
        }
        public IEnumerable<string> GetConnectedPDAs()
        {
            var res = _connections.Where(a => a.UserName.ToUpper().Contains("PDA"))
                                  .Select(s => s.UserName.Contains("-") ? s.User.ToString().Split('-')[1] : "").ToList();
            return res;
        }

        public ConnectionModel GetConnectionById(string connectionId)
        {
            var res = _connections.Where(w => w.Connections.Contains(connectionId)).FirstOrDefault();

            return res;
        }
        public bool IsUserConnected(string userName)
        {
            return _connections.Any(a => a.UserName == userName);
        }

        public ConnectionModel Add(string userName, string connectionId)
        {
            ConnectionModel cm=null;
            try
            {
                    lock (_connections)
                   {
                        var userExists = _connections.Where(w => w.UserName == userName).FirstOrDefault();
                       
                        if (userExists != null)
                        {
                            if (DateTime.Now.Subtract(userExists.LatestActivity).Minutes > 10)
                            {
                                //userExists.Connections.Clear();
                            }
                            cm = userExists;
                            cm.Connections.Add(connectionId);
                             userExists.ConnectionId = connectionId;
                        }
                        else
                        {
                            cm = new ConnectionModel(userName, connectionId);
                        }                    
                        _connections.Add(cm);         
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return cm;
        }

        public void MarkAsLatestActivity(string connetionId)
        {
            try
            {
                lock (_connections)
                {
                    var item = _connections.FirstOrDefault(w => w.Connections.Contains(connetionId));
                    if (item != null)
                    {
                         item.LatestActivity = DateTime.Now;
                       //  WriteXML();
                     }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        public void RemoveInActive()
        {
            var contoRemove = _connections.ToList().Where(w => DateTime.Now.Subtract(w.LatestActivity).Minutes > 10);
            lock (_connections)
            {
                foreach (var c in contoRemove)
                {
                    try
                    {
                        //_connections.Remove(c);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }
                }
            }
        }

        public void Remove(string userName, string connectionId)
        {
            lock (_connections)
            {
                var userExists = _connections.Where(w => w.UserName == userName).FirstOrDefault();
                if (userExists != null)
                {
                    userExists.Connections.Remove(connectionId);
                    if (userExists.Connections.Count() == 0)
                        _connections.Remove(userExists);
                }
            }
        }

        public string GetAllGroupedConnections(string groupId)
        {
            string str = _connections.Where(w => w.GroupId == groupId).Select(s => s.User).Aggregate("", (previous, next) => previous + ", " + next);
            if (str == null)
                return "";
            else if (str.Length > 2)
                return str.Remove(0, 1);
            else
                return str;
        }
        public string GetAllConnections()
        {
            string str = _connections.Select(s => s.User).Aggregate("", (previous, next) => previous + ", " + next);
            if (str == null)
                return "";
            else if (str.Length >2)
                return str.Remove(0, 1);
            else
                return str;
          //  return _connections.Select(s => s.User).Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1);
        }

        public IEnumerable<ConnectionModel> GetConnectionsForHeartBeat()
        {
            return _connections;
        }

    }

    //public class ConnectionMapping<T>
    //{
    //    private readonly Dictionary<T, HashSet<string>> _connections =
    //        new Dictionary<T, HashSet<string>>();

    //    public int Count
    //    {
    //        get
    //        {
    //            return _connections.Count;
    //        }
    //    }

    //    public void Add(T key, string connectionId)
    //    {
    //        lock (_connections)
    //        {
    //            HashSet<string> connections;
    //            if (!_connections.TryGetValue(key, out connections))
    //            {
    //                connections = new HashSet<string>();
    //                _connections.Add(key, connections);
    //            }

    //            lock (connections)
    //            {
    //                connections.Add(connectionId);
    //            }
    //        }
    //    }


    //    public IEnumerable<string> GetConnections(T key)
    //    {
    //        HashSet<string> connections;
    //        if (_connections.TryGetValue(key, out connections))
    //        {
    //            return connections;
    //        }

    //        return Enumerable.Empty<string>();
    //    }

    //    public string GetAllConnetions()
    //    {
    //        return _connections.Keys.ToArray().Aggregate("", (previous, next) => previous + ", " + next).Remove(0, 1);
    //    }

    //    public void Remove(T key, string connectionId)
    //    {
    //        lock (_connections)
    //        {
    //            HashSet<string> connections;
    //            if (!_connections.TryGetValue(key, out connections))
    //            {
    //                return;
    //            }

    //            lock (connections)
    //            {
    //                connections.Remove(connectionId);

    //                if (connections.Count == 0)
    //                {
    //                    _connections.Remove(key);
    //                }
    //            }
    //        }
    //    }
    //}
}
