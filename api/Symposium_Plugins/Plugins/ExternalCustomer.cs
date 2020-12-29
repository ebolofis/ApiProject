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
    public abstract class ExternalCustomer : BasePlugin
    {

        public ExternalCustomer() : base() { }


        /// <summary>
        /// Invoke External Customer Plugin and Get Customer Informations by giving phone number
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="logger"></param>
        /// <param name="searchParameter"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public abstract  DACustomerModel InvokeExternalCustomerInfo(DBInfoModel dbInfo, ILog logger, string searchParameter, Dictionary<string, dynamic> configuration);
    }
}
