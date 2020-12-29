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
    public class DA_Store_PriceListAssocFlows : IDA_Store_PriceListAssocFlows
    {
        IDA_Store_PriceListAssocTasks da_Store_PriceListAssocTasks;
        public DA_Store_PriceListAssocFlows(IDA_Store_PriceListAssocTasks _da_Store_PriceListAssocTasks)
        {
            this.da_Store_PriceListAssocTasks = _da_Store_PriceListAssocTasks;
        }

        /// <summary>
        /// Επιστρέφει όλες τις  pricelist ανα κατάστημα
        /// </summary>
        /// <returns>DAStore_PriceListAssocModel</returns>
        public List<DAStore_PriceListAssocModel> GetDAStore_PriceListAssoc(DBInfoModel dbInfo)
        {
            return da_Store_PriceListAssocTasks.GetDAStore_PriceListAssoc(dbInfo);
        }
    }
}
