using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.Orders;

namespace Symposium.Plugins
{
    public abstract class CardPayment : BasePlugin
    {
        public CardPayment():base(){ }

        /// <summary>
        /// invoke card transaction
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbInfo"></param>
        /// <param name="logger"></param>
        /// <returns>PluginCardPaymentResultModel</returns>
       public abstract  PluginCardPaymentResultModel InvokePluginTransaction(PluginCardPaymentModel model, DBInfoModel dbInfo, ILog logger);
    }
}
