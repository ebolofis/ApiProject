using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    /// <summary>
    /// class that handles clients into a SignalR Hub 
    /// </summary>
    public static class HubParticipantsController
    {

        /// <summary>
        /// add/delete/get SignalR Clients
        /// </summary>
        public static HubParticipants Participants = new HubParticipants();
    }


    /// <summary>
    /// class that handles clients into a SignalR Hub
    /// </summary>
    public class HubParticipants
    {
        private HashSet<SignalRClientModel> clients;
        private readonly object obj = new object();

        public HubParticipants()
        {
            clients = new HashSet<SignalRClientModel>();
        }

        /// <summary>
        /// Return the list of ConnectionIds for users belonging to the given group
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public List<string> GetUsersInGroup(string groupName)
        {
            return clients.Where(x => x.Groups.ToList<string>().Exists(y => y == groupName)).Select(i => i.ConnectionId).ToList<string>();
        }

        /// <summary>
        /// Return the connectionId of a user (or null)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GetConnectionId(string user)
        {
            SignalRClientModel client = clients.FirstOrDefault(x => x.Name == user);
            if (client != null)
                return client.ConnectionId;
            else
                return null;
        }

        /// <summary>
        /// Return a SignalRClientModel from a given ConnectionId
        /// </summary>
        /// <param name="ConnectionId"></param>
        /// <returns></returns>
        public SignalRClientModel GetConnectionModelById(string ConnectionId)
        {
            return clients.FirstOrDefault(x => x.ConnectionId == ConnectionId);
        }

        /// <summary>
        /// check if client exists, based on ConnectionId
        /// </summary>
        /// <param name="ConnectionId">ConnectionId</param>
        /// <returns></returns>
        public bool ClientExists(string ConnectionId)
        {
            return clients.ToList().Exists(x => x.ConnectionId == ConnectionId);
        }

        /// <summary>
        /// check if client exists, based on name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        public bool NameExists(string name)
        {
            return clients.ToList().Exists(x => x.Name == name);
        }

        /// <summary>
        /// Return a ConnectionModel from a given Name
        /// </summary>
        /// <param name="ConnectionId"></param>
        /// <returns></returns>
        public SignalRClientModel GetConnectionModelByName(string Name)
        {
            return clients.FirstOrDefault(x => x.Name == Name);
        }

        /// <summary>
        /// Add a client to list (ConnectionId and Name are unique)
        /// </summary>
        /// <param name="ConnectionId"></param>
        /// <param name="Name">the unique name</param>
        public void AddClient(string ConnectionId, string Name)
        {
            lock (obj)
            {
                if (ClientExists(ConnectionId)) RemoveClient(ConnectionId);
                if (NameExists(Name)) RemoveName(Name);

                clients.Add(new SignalRClientModel(ConnectionId, Name));
            }
           
        }

        /// <summary>
        /// Remove a client from list based on ConnectionId
        /// </summary>
        /// <param name="ConnectionId">ConnectionId</param>
        public void RemoveClient(string ConnectionId)
        {
            lock (obj)
            {
                clients.RemoveWhere(x => x.ConnectionId == ConnectionId);
            }
        }


        /// <summary>
        /// Remove a client from list based on Name
        /// </summary>
        /// <param name="Name">the unique name</param>
        public void RemoveName(string Name)
        {
            lock (obj)
            {
                if (NameExists(Name))
                {
                    SignalRClientModel existing = GetConnectionModelByName(Name);
                    RemoveClient(existing.ConnectionId);
                }
            }
        }
    }



    //---------------------------------------------------------



    /// <summary>
    /// Model representing a SignalR client
    /// </summary>
    public class SignalRClientModel
    {
        private readonly object obj = new object();
        public SignalRClientModel(string ConnectionId)
        {
            this.ConnectionId = ConnectionId;
            Groups = new HashSet<string>();
        }

        public SignalRClientModel(string ConnectionId, string Name) : this(ConnectionId)
        {
            this.Name = Name;
        }

        public string ConnectionId { get; set; }

        public string Name { get; set; }

        public HashSet<string> Groups { get; set; }

        /// <summary>
        /// Join user to group
        /// </summary>
        /// <param name="groupName"></param>
        public void AddGroup(string groupName)
        {
            lock (obj)
            {
                if (Groups.FirstOrDefault(x => x == groupName) == null)
                {
                    Groups.Add(groupName);
                }
            }
            
        }

        /// <summary>
        /// remove user from group
        /// </summary>
        /// <param name="groupName"></param>
        public void RemoveGroup(string groupName)
        {
            lock (obj)
            {
                Groups.Remove(groupName);
            }
        }
    }
}
