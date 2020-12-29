using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class StoreTasks : IStoreTasks
    {
        IStoreDT storeDB;
        public StoreTasks(IStoreDT stDT)
        {
            this.storeDB = stDT;
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
            StoreDetailsModel storeDetails = storeDB.GetStores(Store, storeid);

            return storeDetails;
        }
    }
}
