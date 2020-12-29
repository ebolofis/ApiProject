using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IGdprTasks
    {
        /// <summary>
        /// GDPR For WaterPark Customers(Update table OnlineRegistration).
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        bool UpdateOnlineRegistration(DBInfoModel Store);

        /// <summary>
        /// GDPR For Pms Customers(Update table Quest).
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        bool UpdateGuest(DBInfoModel Store);
    }
}
