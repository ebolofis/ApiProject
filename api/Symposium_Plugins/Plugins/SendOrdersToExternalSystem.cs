using log4net;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;


namespace Symposium.Plugins
{
    public abstract class SendOrdersToExternalSystem : BasePlugin
    {

        public SendOrdersToExternalSystem() : base() { }

        /// <summary>
        /// Invoke Goodys Omnirest Plugin
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="logger"></param>
        /// <param name="model">OrderFromDAToClientForWebCallModel</param>
        /// <returns>true if order is Omnirest otherwise false</returns>
        public abstract bool InvokeOmnirestOrders(DBInfoModel dbInfo, ILog logger, IWebApiClientRestSharpHelper webHlp, IDA_HangFireJobsDT dt, IDA_OrderStatusDT dt1, IDA_OrdersDT dt2, OrderFromDAToClientForWebCallModel model);
    }
}
