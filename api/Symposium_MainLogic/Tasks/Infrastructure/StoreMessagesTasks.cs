using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class StoreMessagesTasks : IStoreMessagesTasks
    {
        IStoreMessagesDT storeMessagesDT;
        public StoreMessagesTasks(IStoreMessagesDT smDT)
        {
            this.storeMessagesDT = smDT;
        }

        /// <summary>
        ///  Επιστρέφει τα μηνύματα που εμφανίζονται στην κύρια σελίδα
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="filtered"></param>
        /// <returns></returns>
        public StoreMessagesModelsPreview GetStoreMessages(DBInfoModel Store, string storeid, bool filtered)
        {
            // get the results
            StoreMessagesModelsPreview storeMessages = storeMessagesDT.GetStoreMessages(Store, storeid, filtered);

            return storeMessages;
        }
    }
}
