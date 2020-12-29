using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class RegionFlows : IRegionFlows
    {
        IRegionTasks regionTasks;
        public RegionFlows(IRegionTasks region)
        {
            this.regionTasks = region;
        }

        /// <summary>
        /// Return regions based on posinfo.Id
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="notables">not used</param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        public RegionModelsPreview GetRegions(DBInfoModel Store, string storeid, bool notables, long posInfoId)
        {
            // get the results
            RegionModelsPreview getRegions = regionTasks.GetRegions(Store, storeid, notables, posInfoId);

            return getRegions;
        }
    }
}
