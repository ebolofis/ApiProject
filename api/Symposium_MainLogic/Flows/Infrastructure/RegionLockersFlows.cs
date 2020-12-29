
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using Symposium.Helpers;

namespace Symposium.WebApi.MainLogic.Flows
{
    /// <summary>
    /// Main Logic Class that handles the RegionLockers activities
    /// </summary>
    public class RegionLockersFlows : IRegionLockersFlows
    {

        IRegionLockersTasks regionLockers;

        public RegionLockersFlows(IRegionLockersTasks regionLockers)
        {
            this.regionLockers = regionLockers;
        }


        /// <summary>
        /// Get RegionLockers Products
        /// </summary>
        /// <param name="dbInfo">StoreId</param>
        public List<RegionLockersModel> GetProducts(DBInfoModel dbInfo)
        {
            List<RegionLockersModel> model = regionLockers.GetProducts(dbInfo);
            return model;
        }
    }
}
