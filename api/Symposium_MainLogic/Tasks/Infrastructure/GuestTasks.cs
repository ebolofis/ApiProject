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
    public class GuestTasks : IGuestTasks
    {
        IGuestDT dt;

        public GuestTasks(IGuestDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Update's table guest and return a guest model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public GuestModel UpdateGuestFromDeliveryCustomer(DBInfoModel Store, DeliveryCustomerModel model)
        {
            return dt.UpdateGuestFromDeliveryCustomer(Store, model);
        }

        /// <summary>
        /// Return's a guest model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public GuestModel GetGuestById(DBInfoModel Store, long Id)
        {
            return dt.GetGuestById(Store, Id);
        }

    }
}
