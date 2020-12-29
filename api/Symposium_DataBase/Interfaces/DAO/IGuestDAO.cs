
using Symposium.Models.Models;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IGuestDAO
    {
        long upsertGuestFromDeliveryCustomer(IDbConnection db, DeliveryCustomerModel model);

        /// <summary>
        /// Provide a guest DTO by call or new and a deliveryCustomer Model 
        /// Updates Guest Model with respect to delivery Fields
        /// </summary>
        /// <param name="db"></param>
        /// <param name="editguest">an object DTO to update or insert </param>
        /// <param name="model">DeliveryCustomer to use for update values</param>
        /// <returns>Guest DTO provided and changed</returns>
        GuestDTO GuestFromDeliveryCustomer(IDbConnection db, GuestDTO editguest, DeliveryCustomerModel model);
    }
}
