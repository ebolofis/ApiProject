using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IGdprDT
    {
        /// <summary>
        /// GDPR For WaterPark Customers(Update table OnlineRegistration).
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        bool UpdateOnlineRegistration(DBInfoModel Store);

        /// <summary>
        /// Get Hotel Info
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        HotelsInfoModel GetHotelInfo(DBInfoModel Store);

        /// <summary>
        /// GDPR For Pms Customers(Update table Quest).
        /// </summary>
        /// <param name="hi"></param>
        /// <returns></returns>
        bool UpdateGuest(DBInfoModel Store, HotelsInfoModel hi);
    }
}
