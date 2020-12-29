using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Plugins
{
    public abstract class ExternalDelivery : BasePlugin
    {

        public ExternalDelivery() : base() { }

        /// <summary>
        /// Get orders from Efood family delivery systems
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public abstract List<DA_ExtDeliveryModel> GetOrders(DBInfoModel dbInfo, ILog logger, Dictionary<string, Dictionary<string, dynamic>> mainConfigurationDictionary);

        /// <summary>
        /// confirm the order back to delivery system
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="orderId"></param>
        public abstract bool ConfirmOrder(DA_ExtDeliveryModel order, ILog logger);

    }
}
