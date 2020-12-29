using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IGuestDT
    {

        GuestModel UpdateGuestFromDeliveryCustomer(DBInfoModel Store, DeliveryCustomerModel model);

        /// <summary>
        /// Return's a guest model using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        GuestModel GetGuestById(DBInfoModel Store, long Id);

        /// <summary>
        /// Return's A Guest using External Key (ProfileNo)
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ProfileNo"></param>
        /// <returns></returns>
        GuestModel GetGuestByExternalKey(DBInfoModel Store, int ProfileNo);
    }
}
