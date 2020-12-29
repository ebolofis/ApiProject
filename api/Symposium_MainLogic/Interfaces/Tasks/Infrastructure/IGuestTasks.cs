using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IGuestTasks
    {
        // <summary>
        /// Update's table guest and return a guest model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        GuestModel UpdateGuestFromDeliveryCustomer(DBInfoModel Store, DeliveryCustomerModel model);

        /// <summary>
        /// Return's a guest model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        GuestModel GetGuestById(DBInfoModel Store, long Id);
    }
}
