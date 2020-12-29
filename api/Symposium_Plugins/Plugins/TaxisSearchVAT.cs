using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.Infrastructure;

namespace Symposium.Plugins
{
    public abstract class TaxisSearchVAT : BasePlugin
    {

        public TaxisSearchVAT() : base() { }

        /// <summary>
        /// invoke taxis transaction
        /// </summary>
        /// <param name="afm"></param>
        /// <param name="dbInfo"></param>
        /// <param name="logger"></param>
        /// <returns>VATCustomerResultModel</returns>
        public abstract VATCustomerResultModel InvokePluginTransaction(string afm, DBInfoModel dbInfo, ILog logger);
    }
}