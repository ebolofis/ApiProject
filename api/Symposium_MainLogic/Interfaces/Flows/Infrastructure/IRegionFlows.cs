﻿using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IRegionFlows
    {
        /// <summary>
        /// Get regions based on posinfo.Id
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="notables">not used</param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        RegionModelsPreview GetRegions(DBInfoModel Store, string storeid, bool notables, long posInfoId);
    }
}
