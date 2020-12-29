using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Plugins
{
    public abstract class ExternalLoyalty : BasePlugin
    {
        public ExternalLoyalty() : base() { }

        public abstract bool RedeemLoyalty(DBInfoModel dbInfo, ILog logger, DA_OrderModel order);

        public abstract bool CancelLoyalty(DBInfoModel dbInfo, ILog logger, string externalOrderId);

        public abstract string GetCouponDetails(DBInfoModel dbInfo, ILog logger, GoodysCouponDetailInfo orderInfo);
    }
}
