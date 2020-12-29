using log4net;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;

namespace Symposium.Plugins
{
    public abstract class DA_StatusChange : BasePlugin
    {

        public DA_StatusChange() : base() { }

        /// <summary>
        /// Invoke DA Order Status Changes Plugin
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="logger"></param>
        /// <param name="model">DA_OrderStatusModel</param>
        /// <returns></returns>
        public abstract bool InvokeDaStatusChange(DBInfoModel dbInfo, IUsersToDatabasesXML users, ILog logger, DA_OrderStatusModel model);
    }
}
