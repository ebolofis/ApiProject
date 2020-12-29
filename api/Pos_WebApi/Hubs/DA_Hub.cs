using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Hubs
{
   // /// <summary>
   // /// SignaR for DA
   // /// </summary>
   // [HubName("DAHub")]
   //public class DA_Hub : Hub
   // {

   //     public ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

   //     /// <summary>
   //     /// Send a message to a specific user
   //     /// </summary>
   //     /// <param name="message"></param>
   //     /// <param name="user"></param>
   //     public void SendPhoneToUser(string phone, string agentId)
   //     {
   //         if (string.IsNullOrWhiteSpace(agentId)) return;

   //         string conId = HubParticipantsController.Participants.GetConnectionId(agentId);
   //         if (conId != null)
   //             Clients.Client(conId).GetCustPhone(phone);
   //         else
   //             logger.Error("SignalR User '" + agentId + "' NOT FOUND!!!");
   //     }

   //     /// <summary>
   //     /// Add a user to a group
   //     /// </summary>
   //     /// <param name="groupName"></param>
   //     /// <returns></returns>
   //     public Task JoinGroup(string groupName)
   //     {
   //         SignalRClientModel client = HubParticipantsController.Participants.GetConnectionModelById(Context.ConnectionId);

   //         client.AddGroup(groupName);
   //         return Groups.Add(Context.ConnectionId, groupName);
   //     }

   //     /// <summary>
   //     /// Remove a user from a group
   //     /// </summary>
   //     /// <param name="groupName"></param>
   //     /// <returns></returns>
   //     public Task LeaveGroup(string groupName)
   //     {
   //         SignalRClientModel client = HubParticipantsController.Participants.GetConnectionModelById(Context.ConnectionId);
   //         client.RemoveGroup(groupName);
   //         return Groups.Remove(Context.ConnectionId, groupName);
   //     }

   //     public override Task OnConnected()
   //     {
   //         //get client's unique id
   //         string conId = Context.ConnectionId;

   //         //if signalR client does not exist then add him to the dictionary
   //         string name = getName();
   //         if (!HubParticipantsController.Participants.ClientExists(Context.ConnectionId))
   //             HubParticipantsController.Participants.AddClient(conId, name);
   //         return base.OnConnected();
   //     }

   //     public override Task OnDisconnected(bool stopCalled)
   //     {
   //         SignalRClientModel client = HubParticipantsController.Participants.GetConnectionModelById(Context.ConnectionId);

   //         if (client != null) {
   //             var groups = client.Groups;
   //             //remove client from groups
   //             foreach (string group in client.Groups.ToList())
   //             {
   //                 Groups.Remove(Context.ConnectionId, group);
   //             }
   //         }
   //         //remove client
   //         HubParticipantsController.Participants.RemoveClient(Context.ConnectionId);
   //         return base.OnDisconnected(stopCalled);
   //     }


   //     public override Task OnReconnected()
   //     {
   //         //if signalR client does not exist then add him to the dictionary
   //         if (!HubParticipantsController.Participants.ClientExists(Context.ConnectionId))
   //             HubParticipantsController.Participants.AddClient(Context.ConnectionId, getName());
   //         return base.OnReconnected();
   //     }

   //     /// <summary>
   //     /// Get username from QueryString or Headers
   //     /// </summary>
   //     /// <returns></returns>
   //     private string getName()
   //     {
   //         var name = Context.QueryString["name"];
   //         if (name == null)
   //         {
   //             name = Context.Headers["name"];

   //             if (name == null)
   //                 name = "Console";
   //         }
   //         return name;
   //     }

   // }
}
