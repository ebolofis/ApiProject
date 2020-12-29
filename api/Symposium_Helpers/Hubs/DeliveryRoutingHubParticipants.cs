using Symposium.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Hubs
{
    /// <summary>
    /// class that handles delivery routing staff into a SignalR Hub
    /// </summary>
    public class DeliveryRoutingHubParticipants : IDeliveryRoutingHubParticipants
    {
        private HashSet<DeliveryStaffsModel> DeliveryStaffs;

        public DeliveryRoutingHubParticipants()
        {
            DeliveryStaffs = new HashSet<DeliveryStaffsModel>();
        }

        /// <summary>
        /// Return the sessionId of a user (or null)
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public string GetSessionId(long staffId)
        {
            DeliveryStaffsModel client = DeliveryStaffs.FirstOrDefault(x => x.staffId == staffId);

            if (client != null)
            {
                return client.sessionId;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return a DeliveryStaffsModel from a given sessionId
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public DeliveryStaffsModel GetConnectionModelById(string sessionId)
        {
            return DeliveryStaffs.FirstOrDefault(x => x.sessionId == sessionId);
        }

        /// <summary>
        /// check if client exists, based on sessionId
        /// </summary>
        /// <param name="sessionId">sessionId</param>
        /// <returns></returns>
        public bool ClientExists(string sessionId)
        {
            return DeliveryStaffs.ToList().Exists(x => x.sessionId == sessionId);
        }

        /// <summary>
        /// check if client exists, based on id
        /// </summary>
        /// <param name="staffId">staffId</param>
        /// <returns></returns>
        public bool IdExists(long staffId)
        {
            return DeliveryStaffs.ToList().Exists(x => x.staffId == staffId);
        }

        /// <summary>
        /// Return a ConnectionModel from a given (unique) staff id
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public DeliveryStaffsModel GetConnectionModelById(long staffId)
        {
            return DeliveryStaffs.FirstOrDefault(x => x.staffId == staffId);
        }

        /// <summary>
        /// Add a client to list (sessionId and staffId are unique)
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="staffId">unique id</param>
        public void AddClient(string sessionId, long staffId)
        {
            if (ClientExists(sessionId))
            { 
                RemoveClient(sessionId); 
            }

            if (IdExists(staffId))
            { 
                RemoveId(staffId); 
            }

            DeliveryStaffs.Add(new DeliveryStaffsModel(sessionId, staffId));
        }

        /// <summary>
        /// Remove a client from list based on sessionId
        /// </summary>
        /// <param name="sessionId">sessionId</param>
        public void RemoveClient(string sessionId)
        {
            DeliveryStaffs.RemoveWhere(x => x.sessionId == sessionId);
        }

        /// <summary>
        /// Remove a client from list based on id
        /// </summary>
        /// <param name="staffId">unique id</param>
        public void RemoveId(long staffId)
        {
            if (IdExists(staffId))
            {
                DeliveryStaffsModel existing = GetConnectionModelById(staffId);

                RemoveClient(existing.sessionId);
            }
        }
    }

    /// <summary>
    /// Model representing Delivery Staff client
    /// </summary>
    public class DeliveryStaffsModel
    {
        public string sessionId { get; set; }

        public long staffId { get; set; }

        public DeliveryStaffsModel(string sessionId, long staffId) 
        {
            this.sessionId = sessionId;

            this.staffId = staffId;
        }
}
}
