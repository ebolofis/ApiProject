using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class RegionTasks : IRegionTasks
    {
        IRegionDT regionDT;
        public RegionTasks(IRegionDT region)
        {
            this.regionDT = region;
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
            RegionModelsPreview getRegions = regionDT.GetRegions(Store, storeid, notables, posInfoId);

            return getRegions;
        }
    }
}
