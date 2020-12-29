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
    public class StoreFlows : IStoreFlows
    {
        IStoreTasks storeTasks;
        public StoreFlows(IStoreTasks stTasks)
        {
            this.storeTasks = stTasks;
        }

        /// <summary>
        /// Επιστρέφει τη περιγραφή του καταστήματος (τυπικά επιστρέφει μία εγγραφή) 
        /// </summary>
        /// <remarks>GET api/Store</remarks>
        /// <param name="storeid"></param>
        /// <returns>
        public StoreDetailsModel GetStores(DBInfoModel Store, string storeid)
        {
            // get the results
            StoreDetailsModel storeDetails = storeTasks.GetStores(Store, storeid);

            return storeDetails;
        }
    }
}
