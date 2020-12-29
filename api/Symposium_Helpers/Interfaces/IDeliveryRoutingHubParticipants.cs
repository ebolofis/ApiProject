using Symposium.Helpers.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
    public interface IDeliveryRoutingHubParticipants
    {
        string GetSessionId(long staffId);

        DeliveryStaffsModel GetConnectionModelById(string sessionId);

        bool ClientExists(string sessionId);

        bool IdExists(long staffId);

        DeliveryStaffsModel GetConnectionModelById(long staffId);

        void AddClient(string sessionId, long staffId);

        void RemoveClient(string sessionId);

        void RemoveId(long staffId);


    }
}
