using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IRegionLockersTasks
    {
        /// <summary>
        /// Get RegionLockers Products
        /// </summary>
        /// <param name="Store">StoreId</param>
        List<RegionLockersModel> GetProducts(DBInfoModel Store);
    }
}
