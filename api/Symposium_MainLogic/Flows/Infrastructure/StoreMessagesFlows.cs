using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class StoreMessagesFlows : IStoreMessagesFlows
    {
        IStoreMessagesTasks storeMessageTasks;
        public StoreMessagesFlows(IStoreMessagesTasks sm)
        {
            this.storeMessageTasks = sm;
        }

        /// <summary>
        ///  Επιστρέφει τα μηνύματα που εμφανίζονται στην κύρια σελίδα
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="filtered"></param>
        /// <returns></returns>
        public StoreMessagesModelsPreview GetStoreMessages(DBInfoModel dbInfo, string storeid, bool filtered)
        {
            // get the results
            StoreMessagesModelsPreview storeMessages = storeMessageTasks.GetStoreMessages(dbInfo, storeid, filtered);

            return storeMessages;
        }
    }
}
