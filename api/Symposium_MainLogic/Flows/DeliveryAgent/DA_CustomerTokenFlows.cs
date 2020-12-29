using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_CustomerTokenFlows : IDA_CustomerTokenFlows
    {
        IDA_CustomerTokenTasks customerTokenTasks;

        public DA_CustomerTokenFlows(IDA_CustomerTokenTasks customerTokenTasks)
        {
            this.customerTokenTasks = customerTokenTasks;
        }

        /// <summary>
        /// Gets customer token by customer id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public DA_CustomerTokenModel GetCustomerToken(DBInfoModel dbInfo, long customerId)
        {
            return customerTokenTasks.GetCustomerToken(dbInfo, customerId);
        }

        /// <summary>
        /// Upserts customer token
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public long SetCustomerToken(DBInfoModel dbInfo, DATokenModel model)
        {
            return customerTokenTasks.SetCustomerToken(dbInfo, model);
        }

        /// <summary>
        /// Deletes customer token by customer id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public long DeleteCustomerToken(DBInfoModel dbInfo, long customerId)
        {
            return customerTokenTasks.DeleteCustomerToken(dbInfo, customerId);
        }

    }
}
