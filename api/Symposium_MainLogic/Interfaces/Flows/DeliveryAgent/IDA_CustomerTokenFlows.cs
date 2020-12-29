using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_CustomerTokenFlows
    {
        /// <summary>
        /// Gets customer token by customer id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        DA_CustomerTokenModel GetCustomerToken(DBInfoModel dbInfo, long customerId);

        /// <summary>
        /// Upserts customer token
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        long SetCustomerToken(DBInfoModel dbInfo, DATokenModel model);

        /// <summary>
        /// Deletes customer token by customer id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        long DeleteCustomerToken(DBInfoModel dbInfo, long customerId);

    }
}
