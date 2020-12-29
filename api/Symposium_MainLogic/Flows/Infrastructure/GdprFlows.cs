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
    public class GdprFlows : IGdprFlows
    {
        IGdprTasks gdprTasks;
        public GdprFlows(IGdprTasks gTasks)
        {
            this.gdprTasks = gTasks;
        }

        /// <summary>
        /// GDPR For WaterPark Customers(Update table OnlineRegistration).
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public bool UpdateOnlineRegistration(DBInfoModel Store)
        {
            bool result = gdprTasks.UpdateOnlineRegistration(Store);
            return result;
        }

        //GDPR For Pms Customers(Update table Quest).
        public bool UpdateGuest(DBInfoModel Store)
        {
            bool result = gdprTasks.UpdateGuest(Store);
            return result;
        }

        //GDPR For Delivery Customers.
    }
}
