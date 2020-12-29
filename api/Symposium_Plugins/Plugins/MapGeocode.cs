using log4net;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Plugins
{
    public abstract class  MapGeocode : BasePlugin
    {

        public MapGeocode() : base() { }

        /// <summary>
        /// Invoke Maps Plugin and Get Coordinate Informations by giving DA_AddressModel
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="logger"></param>
        /// <param name="model">DA_AddressModel</param>
        /// <returns>DA_AddressModel</returns>
        public abstract DA_AddressModel InvokeMapGeocode(DBInfoModel dbInfo, ILog logger, DA_AddressModel model);
    }
}
