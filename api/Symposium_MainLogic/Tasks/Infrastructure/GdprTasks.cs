using Symposium.Helpers.Classes;
using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class GdprTasks : IGdprTasks
    {
        IGdprDT GdprDT;
        public GdprTasks(IGdprDT gDT)
        {
            this.GdprDT = gDT;
        }

        /// <summary>
        /// GDPR For WaterPark Customers(Update table OnlineRegistration).
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public bool UpdateOnlineRegistration(DBInfoModel Store)
        {
            return GdprDT.UpdateOnlineRegistration(Store);
        }

        /// <summary>
        /// GDPR For Pms Customers(Update table Quest).
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public bool UpdateGuest(DBInfoModel Store)
        {
            HotelsInfoModel hi = GdprDT.GetHotelInfo(Store);
            if ((hi.Type == 0 || hi.Type == 4 || hi.Type == 10) && hi.ServerName != null && hi.DBUserName != null && StringCipher.Decrypt(hi.DBPassword) != null && hi.DBName != null)
            {
                bool result = GdprDT.UpdateGuest(Store, hi);
                return result;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Updates Delivery entities
        /// Delivery Customer - InvoiceShipping Details
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public bool UpdateDeliveryEntities(DBInfoModel Store) {
            ///Delivery Customer

            ///Invoice Shipping Details
            return true;
        }
    }
}
