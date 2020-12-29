using Autofac;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{

    public class RegionLockersTasks : IRegionLockersTasks
    {
        IRegionLockersDT regionLockers;  

        public RegionLockersTasks(IRegionLockersDT regionLockers)
        {
            this.regionLockers = regionLockers;
        }


        /// <summary>
        /// Get RegionLockers Products
        /// </summary>
        /// <param name="Store">StoreId</param>
        public List<RegionLockersModel> GetProducts(DBInfoModel Store)
        {
            List<RegionLockersModel> model = regionLockers.GetProducts(Store);
            return model;
        }
    }
}
